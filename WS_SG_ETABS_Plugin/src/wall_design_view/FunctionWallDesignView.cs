using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ScottPlot;
using System.Windows.Media.TextFormatting;
using static WS_SG_ETABS_Plugin.src.wall_design_view.ClassWallDesign;

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    internal class FunctionWallDesignView
    {

        public static (bool, List<ClassWallDesign.WallDesignDetail>, List<ClassWallDesign.WallDesignDetail>, List<double>, List<double>, double, double) PlotWallDesignResult(WpfPlot ResultPlot, 
            List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList, List<double> uniqueX, List<double> uniqueY)
        {
            bool isError = false;

            /*
            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable wallObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, wallObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Wall Object Connectivity");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable assignmentPierLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentPierLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Pier Labels");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            src.HelperEtabs.DatabaseTable assignmentSpandrelLabels = new HelperEtabs.DatabaseTable();
            (isError, assignmentSpandrelLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Spandrel Labels");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            //Filter group to only include area, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Area");
            if (isError) { return (isError, null, null, null, null, 0, 0); }

            //Filter group to only include groups which are choosen, to improve performance
            src.HelperEtabs.DatabaseTable groupAssignmentsTableFilteredGroupCompiled = new HelperEtabs.DatabaseTable();
            groupAssignmentsTableFilteredGroupCompiled.TableInList = new List<Dictionary<string, string>>();
            foreach (string group in listChoosenGroup)
            {
                src.HelperEtabs.DatabaseTable groupAssignmentsTableFilteredGroup = new HelperEtabs.DatabaseTable();
                (isError, groupAssignmentsTableFilteredGroup) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "GroupName", group);
                if (isError) { return (isError, null, null, null, null, 0, 0); }

                groupAssignmentsTableFilteredGroupCompiled.TableInList.AddRange(groupAssignmentsTableFilteredGroup.TableInList);
            }

            //Convert to point 3D
            List<HelperEtabs.Point3D> pointList = HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjectConnectivityTable);

            //Filter by choosen group
            //(isError, wallObjectConnectivityTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(wallObjectConnectivityTable, groupAssignmentsTable, choosenGroup);

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
                        }
                    }
                }

                //Find story label
                (string label, string storyLabel) = GetLabelAndStoryLabel(shell.UniqueName, wallObjectConnectivityTable);

                //Collect pier or spandrel label
                string pierLabel = GetPierLabel(shell.UniqueName, assignmentPierLabels);
                string spandrelLabel = GetSpandrelLabel(shell.UniqueName, assignmentSpandrelLabels);

                //Get group name
                string groupName = HelperEtabs.GetElementGroupName(groupAssignmentsTableFilteredGroupCompiled, shell.UniqueName, "Area");

                ClassWallDesign.WallDesignDetail wallDesignDetail = new ClassWallDesign.WallDesignDetail(shell.UniqueName, label, shellPointList, storyLabel, pierLabel, spandrelLabel, groupName);

                //Check for failure
                bool? wallFail = null;
                if (typeOfFailure == "All")
                {
                    wallFail = CheckIsAnyFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }
                else if (typeOfFailure == "PMN")
                {
                    wallFail = CheckIsPMNFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }
                else if (typeOfFailure == "Shear")
                {
                    wallFail = CheckIsShearFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }

                wallDesignDetail.IsFail = wallFail;

                wallDesignDetailList.Add(wallDesignDetail);
            }
            */

            //Draw plot
            double countFail = 0;
            double countPass = 0;
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredList = new List<ClassWallDesign.WallDesignDetail>();
            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                //Filter based on group
                if (wallDesignDetail.Group == choosenGroup)
                {
                    wallDesignDetailFilteredList.Add(wallDesignDetail);

                    //Take note of unique coordinate
                    foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                    {
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

                    //Check for planar
                    if (uniqueX.Count == 1 || uniqueY.Count == 1)
                    {
                        //is planar
                    }
                    else
                    {
                        MessageBox.Show("Chosen walls is not in 1 plane! Walls must be on either XZ or YZ plane!");
                        isError = true;
                        return (isError, null, null, null, null, 0, 0);
                    }

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


                    if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                    {
                        color = "red";
                        countFail++;
                    }
                    else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
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

            return (isError, wallDesignDetailList, wallDesignDetailFilteredList, uniqueX, uniqueY, countPass, countFail);
        }

        public static bool? IsWallFail(string typeOfFailure, ClassWallDesign.WallDesignDetail wallDesignDetail)
        {
            //If no design exist
            if (wallDesignDetail.IsFailAll == null || wallDesignDetail.IsFailPMN == null || wallDesignDetail.IsFailShear == null)
            {
                return null;
            }


            if (typeOfFailure == "All" && wallDesignDetail.IsFailAll == true)
            {
                return true;
            }
            else if (typeOfFailure == "PMN" && wallDesignDetail.IsFailPMN == true)
            {
                return true;
            }
            else if (typeOfFailure == "Shear" && wallDesignDetail.IsFailShear == true)
            {
                return true;
            }

            return false;
        }

        public static (bool?, bool?, bool?) CheckIsAnyFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, 
            List<Dictionary<string, string>> tableInListSpandrelDesignResult, bool? extractEtabs, bool? extractExcel)
        {
            bool? failAll = null;
            bool? failPMN = null;
            bool? failShear = null;

            bool wallExist = false;

            //Check if wall design exist
            if (extractEtabs == true)
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                        {
                            wallExist = true;
                            break;
                        }
                    }

                }
                else if (wall.SpandrelLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            wallExist = true;
                            break;
                        }

                    }
                }
            }
            else if (extractExcel == true)
            {
                if (wall.PierLabel != null)
                {

                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            wallExist = true;
                            break;
                        }
                    }

                }
                else if (wall.SpandrelLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            wallExist = true;
                            break;
                        }

                    }
                }
            }

            if (wallExist)
            {
                failAll = false;
                failPMN = false;
                failShear = false;
            }
            else
            {
                return (failAll, failPMN, failShear);
            }

            if (extractEtabs == true)
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                        {
                            if (row["ErrMsg"] != "No Message")
                            {
                                failAll = true;
                            }

                            if (row["ErrMsg"].Contains("flexural"))
                            {
                                failPMN = true;
                            }

                            if (row["ErrMsg"].Contains("Shear"))
                            {
                                failShear = true;
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
                                failAll = true;
                            }

                            if (row["ErrMsg"].Contains("flexural"))
                            {
                                failPMN = true;
                            }

                            if (row["ErrMsg"].Contains("Shear"))
                            {
                                failShear = true;
                            }

                        }

                    }
                }
            }
            else if (extractExcel == true)
            {
                if (wall.PierLabel != null)
                {

                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            if (row["Errors"] != "No Message")
                            {
                                failAll = true;
                            }
                            if (row["Errors"].Contains("flexural"))
                            {
                                failPMN = true;
                            }
                            if (row["Errors"].Contains("Shear"))
                            {
                                failShear = true;
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
                            if (row["Errors"] != "No Message")
                            {
                                failAll = true;
                            }
                            if (row["Errors"].Contains("flexural"))
                            {
                                failPMN = true;
                            }
                            if (row["Errors"].Contains("Shear"))
                            {
                                failShear = true;
                            }
                        }

                    }
                }
            }

            return (failAll, failPMN, failShear);

        }

        

        public static bool? CheckIsPMNFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, 
            List<Dictionary<string, string>> tableInListSpandrelDesignResult, bool? extractEtabs, bool? extractExcel)
        {
            bool wallExistInResult = false;

            if (extractEtabs == true)
            {
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
            }
            else if (extractExcel == true)
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            wallExistInResult = true;
                            if (row["Errors"].Contains("flexural"))
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
                            if (row["Errors"].Contains("flexural"))
                            {
                                return true;
                            }
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

        
        public static bool? CheckIsShearFail(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult, 
            List<Dictionary<string, string>> tableInListSpandrelDesignResult, bool? extractEtabs, bool? extractExcel)
        {
            bool wallExistInResult = false;

            if (extractEtabs == true)
            {
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
            }
            else if ( extractExcel == true )
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            wallExistInResult = true;
                            if (row["Errors"].Contains("Shear"))
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
                            if (row["Errors"].Contains("Shear"))
                            {
                                return true;
                            }
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
            List<double> uniqueX, List<double> uniqueY, int titleSize, int axisTitleSize,
            int tickLabelSize, string typeOfFailure, double countPass, double countFail)
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
                if ((IsWallFail(typeOfFailure, wallDesignDetail) == false && showOk == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == true && showFail == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == null && showNull == true))
                {
                    if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                    {
                        color = "red";
                    }
                    else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
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

        public static List<ClassWallDesign.WallDesignDetail> FilterWallDesignResultChangeGroup(WpfPlot ResultPlot, bool? showOk, bool? showFail, bool? showNull, 
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            int titleSize, int axisTitleSize,
            int tickLabelSize, string choosenGroup, string typeOfFailure)
        {
            ScottPlot.AxisLimits axisLimits = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            int countPass = 0;
            int countFail = 0;

            bool isError = false;

            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();

            List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredList = new List<ClassWallDesign.WallDesignDetail>();
            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                //Filter based on group
                if (wallDesignDetail.Group == choosenGroup)
                {
                    wallDesignDetailFilteredList.Add(wallDesignDetail);

                    //Take note of unique coordinate
                    foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                    {
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

                    //Check for planar
                    if (uniqueX.Count == 1 || uniqueY.Count == 1)
                    {
                        //is planar
                    }
                    else
                    {
                        MessageBox.Show("Chosen walls is not in 1 plane! Walls must be on either XZ or YZ plane!");
                        isError = true;
                        return wallDesignDetailFilteredList;
                    }

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
                    if ((IsWallFail(typeOfFailure, wallDesignDetail) == false && showOk == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == true && showFail == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == null && showNull == true))
                    {
                        if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                        {
                            color = "red";
                            countFail++;
                        }
                        else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
                        {
                            color = "green";
                            countPass++;
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

            return wallDesignDetailFilteredList;
        }

        public static void FilterWallDesignResultChangeTypeOfFailure(WpfPlot ResultPlot, bool? showOk, bool? showFail, bool? showNull, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            List<double> uniqueX, List<double> uniqueY, int titleSize, int axisTitleSize,
            int tickLabelSize, string typeOfFailure, List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult,
            bool? extractEtabs, bool? extractExcel)
        {
            ScottPlot.AxisLimits axisLimits = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            double countFail = 0;
            double countPass = 0;
            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                /*
                //Check for failure
                bool? wallFail = null;
                if (typeOfFailure == "All")
                {
                    wallFail = CheckIsAnyFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }
                else if (typeOfFailure == "PMN")
                {
                    wallFail = CheckIsPMNFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }
                else if (typeOfFailure == "Shear")
                {
                    wallFail = CheckIsShearFail(wallDesignDetail, tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                }
                */

                //wallDesignDetail.IsFail = wallFail;

                //Count fail and pass
                if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                {
                    countFail++;
                }
                else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
                {
                    countPass++;
                }

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
                if ((IsWallFail(typeOfFailure, wallDesignDetail) == false && showOk == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == true && showFail == true) || (IsWallFail(typeOfFailure, wallDesignDetail) == null && showNull == true))
                {
                    if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                    {
                        color = "red";
                    }
                    else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
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
