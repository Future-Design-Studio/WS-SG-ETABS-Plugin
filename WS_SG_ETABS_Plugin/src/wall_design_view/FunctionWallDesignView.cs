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
        public static bool PlotWallDesignResult(WpfPlot ResultPlot, List<src.HelperEtabs.DatabaseTable> databaseTableList, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            bool isError = false;

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable wallObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, wallObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Wall Object Connectivity");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable assignmentPierLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentPierLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Pier Labels");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable assignmentSpandrelLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentSpandrelLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Spandrel Labels");
            if (isError) { return (isError); }

            //Filter group to only include area, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Area");
            if (isError) { return (isError); }

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
                bool? wallFail = false;
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
                return isError;
            }

            //Draw plot
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
                }
                else if (wallDesignDetail.IsFail == false)
                {
                    color = "green";
                }
                else
                {
                    color = "gray";
                }

                HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, System.Drawing.ColorTranslator.FromHtml(color), xWidth, yWidth, 2, System.Drawing.ColorTranslator.FromHtml("black"));
            }

            ResultPlot.Plot.AxisScaleLock(true);
            //ResultPlot.Plot.Title(plotTitle, true, null, titleSize, "Arial Narrow");
            ResultPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            //Axis
            //ResultPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            //.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            //ResultPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            //.Plot.XAxis.ManualTickSpacing(gridSpacingX);
            ResultPlot.Refresh();

            return isError;
        }

        public static bool? CheckIsAnyFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            if (wall.PierLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
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
                        if (row["ErrMsg"] != "No Message")
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return false;
        }

        

        public static bool? CheckIsPMNFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            if (wall.PierLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
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
                        if (row["ErrMsg"].Contains("flexural"))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return false;
        }

        
        public static bool? CheckIsShearFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            if (wall.PierLabel != null)
            {
                foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                {
                    if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                    {
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
                        if (row["ErrMsg"].Contains("Shear"))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return false;
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


    }
}
