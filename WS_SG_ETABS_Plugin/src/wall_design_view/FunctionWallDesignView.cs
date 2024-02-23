using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ScottPlot;
using System.Windows.Media.TextFormatting;

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    internal class FunctionWallDesignView
    {
        public static (bool, List<ClassWallDesign.WallDesignDetail>, List<double>, List<double>, double, double) PlotWallDesignResult(WpfPlot ResultPlot, List<src.HelperEtabs.DatabaseTable> databaseTableList, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize)
        {
            bool isError = false;

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable wallObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, wallObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Wall Object Connectivity");
            if (isError) { return (isError, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable assignmentPierLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentPierLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Pier Labels");
            if (isError) { return (isError, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable assignmentSpandrelLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentSpandrelLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Spandrel Labels");
            if (isError) { return (isError, null, null, null, 0, 0); }

            //Filter group to only include area, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Area");
            if (isError) { return (isError, null, null, null, 0, 0); }

            //Convert to point 3D
            List<HelperEtabs.Point3D> pointList = HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjectConnectivityTable);

            //Filter by choosen group
            (isError, wallObjectConnectivityTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(wallObjectConnectivityTable, groupAssignmentsTable, choosenGroup);

            //Get shell points
            List<HelperEtabs.Shell> shellList = HelperEtabs.GetShellsPoints(wallObjectConnectivityTable);

            //Create wall result instance
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailList = new List<ClassWallDesign.WallDesignDetail>();
            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();
            foreach (HelperEtabs.Shell shell in shellList)
            {
                //Collect all the points
                List<HelperEtabs.Point3D> shellPointList = new List<HelperEtabs.Point3D>();

                foreach (string shellPointUniqueName in shell.Points)
                {
                    foreach (HelperEtabs.Point3D point in pointList)
                    {
                        if (shellPointUniqueName == point.UniqueName)
                        {
                            shellPointList.Add(point);

                            //Take note of unique X and Y
                            if (!uniqueX.Contains(point.XCoordinate))
                            {
                                uniqueX.Add(point.XCoordinate);
                            }
                            if (!uniqueY.Contains(point.YCoordinate))
                            {
                                uniqueY.Add(point.YCoordinate);
                            }
                        }
                    }
                }

                //Find story label
                (string label, string storyLabel) = GetLabelAndStoryLabel(shell.UniqueName, wallObjectConnectivityTable);

                //Collect pier or spandrel label
                string pierLabel = GetPierLabel(shell.UniqueName, assignmentPierLabels);
                string spandrelLabel = GetSpandrelLabel(shell.UniqueName, assignmentSpandrelLabels);

                ClassWallDesign.WallDesignDetail wallDesignDetail = new ClassWallDesign.WallDesignDetail(shell.UniqueName, label, shellPointList, storyLabel, pierLabel, spandrelLabel);

                //Check for failure
                bool? wallFail = null;
                if (typeOfFailure == "All")
                {
                    wallFail = CheckIsAnyFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult);
                }
                else if (typeOfFailure == "PMN")
                {
                    wallFail = CheckIsPMNFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult);
                }
                else if (typeOfFailure == "Shear")
                {
                    wallFail = CheckIsShearFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult);
                }

                wallDesignDetail.IsFail = wallFail;

                wallDesignDetailList.Add(wallDesignDetail);
            }

            //Check for planar
            if (uniqueX.Count == 1 || uniqueY.Count == 1)
            {
                //is planar
            }
            else
            {
                MessageBox.Show("Chosen walls is not in 1 plane! Walls must be on either XZ or YZ plane!");
                isError = true;
                return (isError, null, null, null, 0, 0);
            }

            //Draw plot
            double countFail = 0;
            double countPass = 0;
            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {

                List<double> xCoordList = new List<double>();
                List<double> yCoordList = new List<double>();
                foreach(HelperEtabs.Point3D point in wallDesignDetail.PointList)
                {
                    //Decide which plane to use
                    if (uniqueX.Count == 1)
                    {
                        xCoordList.Add(point.YCoordinate);
                    }
                    else if (uniqueY.Count == 1)
                    {
                        xCoordList.Add(point.XCoordinate);
                    }
                        
                    yCoordList.Add(point.ZCoordinate);
                }

                double xCenter = xCoordList.Average();
                double yCenter = yCoordList.Average();

                double xWidth = xCoordList.Max() - xCoordList.Min();
                double yWidth = yCoordList.Max() - yCoordList.Min();

                string color = "";
                if (wallDesignDetail.IsFail == true)
                {
                    color = "red";
                    countFail++;
                }
                else if (wallDesignDetail.IsFail == false)
                {
                    color = "green";
                    countPass++;
                }
                else
                {
                    color = "#D3D3D3";
                }

                HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, System.Drawing.ColorTranslator.FromHtml(color), xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));
            }

            double percentPass = countPass / (countFail + countPass) * 100;
            percentPass = Math.Round(percentPass, 2);
            double percentFail = countFail / (countFail + countPass) * 100;
            percentFail = Math.Round(percentFail, 2);

            string percentPassText = $"Pass {percentPass}% ({countPass})";
            string percentFailText = $"Fail {percentFail}% ({countFail})";

            //Add anotation
            HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentPassText), 25, 10, 10, 1, System.Drawing.ColorTranslator.FromHtml("green"), 
                System.Drawing.ColorTranslator.FromHtml("white"), true);
            HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentFailText), 25, 10, 50, 1, System.Drawing.ColorTranslator.FromHtml("red"), 
                System.Drawing.ColorTranslator.FromHtml("white"), true);

            ResultPlot.Plot.AxisScaleLock(true);
            ResultPlot.Plot.Title("Wall Design Result", true, null, titleSize, "Arial Narrow");
            ResultPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            //Axis
            ResultPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            ResultPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            ResultPlot.Refresh();

            return (isError, wallDesignDetailList, uniqueX, uniqueY, countPass, countFail);
        }

        public static bool? CheckIsAnyFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            bool wallExistInResult = false;
            if (wall.PierLabel != null)
            {
                
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"] != "No Message")
                        {
                            return true;
                        }
                    }
                }
                
            }
            else if (wall.SpandrelLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"] != "No Message")
                        {
                            return true;
                        }
                    }

                }
            }

            if (wallExistInResult)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        

        public static bool? CheckIsPMNFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            bool wallExistInResult = false;
            if (wall.PierLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"].Contains("flexural"))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (wall.SpandrelLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"].Contains("flexural"))
                        {
                            return true;
                        }
                    }
                }
            }

            if (wallExistInResult)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        
        public static bool? CheckIsShearFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            bool wallExistInResult = false;
            if (wall.PierLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"].Contains("Shear"))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (wall.SpandrelLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                    {
                        wallExistInResult = true;
                        if (row["ErrMsg"].Contains("Shear"))
                        {
                            return true;
                        }
                    }
                }
            }

            if (wallExistInResult)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public static (string, string) GetLabelAndStoryLabel(string uniqueName, HelperEtabs.DatabaseTable wallObjectConnectivityTable)
        {
            foreach (Dictionary<string, string> row in wallObjectConnectivityTable.TableInList)
            {
                if (row["UniqueName"] == uniqueName)
                {
                    return (row["WallBay"], row["Story"]);
                }
            }
            return (null, null);
        }

        public static string GetPierLabel(string uniqueName, HelperEtabs.DatabaseTable assignmentPierLabels)
        {
            if (assignmentPierLabels.TableInList != null)
            {
                foreach (Dictionary<string, string> row in assignmentPierLabels.TableInList)
                {
                    string uniqueNameCheck = row["UniqueName"];
                    if (row["UniqueName"] == uniqueName)
                    {
                        return row["PierName"];
                    }
                }
            }
            return null;

        }

        public static string GetSpandrelLabel(string uniqueName, HelperEtabs.DatabaseTable assignmentSpandrelLabels)
        {
            if (assignmentSpandrelLabels.TableInList != null)
            {
                foreach (Dictionary<string, string> row in assignmentSpandrelLabels.TableInList)
                {
                    if (row["UniqueName"] == uniqueName)
                    {
                        return row["SpandName"];
                    }
                }
            }
            return null;
        }


        public static void FilterWallDesignResult(WpfPlot ResultPlot, bool? showOk, bool? showFail, bool? showNull, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList, 
            List<double> uniqueX, List<double> uniqueY, double countPass, double countFail, int titleSize, int axisTitleSize,
            int tickLabelSize)
        {
            ScottPlot.AxisLimits axisLimits = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                //Filter

                List<double> xCoordList = new List<double>();
                List<double> yCoordList = new List<double>();

                foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                {
                    //Decide which plane to use
                    if (uniqueX.Count == 1)
                    {
                        xCoordList.Add(point.YCoordinate);
                    }
                    else if (uniqueY.Count == 1)
                    {
                        xCoordList.Add(point.XCoordinate);
                    }

                    yCoordList.Add(point.ZCoordinate);
                }

                double xCenter = xCoordList.Average();
                double yCenter = yCoordList.Average();

                double xWidth = xCoordList.Max() - xCoordList.Min();
                double yWidth = yCoordList.Max() - yCoordList.Min();


                string color = "";
                if ((wallDesignDetail.IsFail == false && showOk == true) || (wallDesignDetail.IsFail == true && showFail == true) || (wallDesignDetail.IsFail == null && showNull == true))
                {
                    if (wallDesignDetail.IsFail == true)
                    {
                        color = "red";
                    }
                    else if (wallDesignDetail.IsFail == false)
                    {
                        color = "green";
                    }
                    else
                    {
                        color = "#D3D3D3";
                    }
                }
                else
                {
                    color = "#00FFFFFF";
                }

                HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, System.Drawing.ColorTranslator.FromHtml(color), xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

            }

            double percentPass = countPass / (countFail + countPass) * 100;
            percentPass = Math.Round(percentPass, 2);
            double percentFail = countFail / (countFail + countPass) * 100;
            percentFail = Math.Round(percentFail, 2);

            string percentPassText = $"Pass {percentPass}% ({countPass})";
            string percentFailText = $"Fail {percentFail}% ({countFail})";

            //Add anotation
            HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentPassText), 25, 10, 10, 1, System.Drawing.ColorTranslator.FromHtml("green"),
                System.Drawing.ColorTranslator.FromHtml("white"), true);
            HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentFailText), 25, 10, 50, 1, System.Drawing.ColorTranslator.FromHtml("red"),
                System.Drawing.ColorTranslator.FromHtml("white"), true);

            ResultPlot.Plot.AxisScaleLock(true);
            ResultPlot.Plot.Title("Wall Design Result", true, null, titleSize, "Arial Narrow");

            //Axis
            ResultPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            ResultPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);

            ResultPlot.Plot.SetAxisLimits(axisLimits);

            ResultPlot.Refresh();
        }

    }

}
