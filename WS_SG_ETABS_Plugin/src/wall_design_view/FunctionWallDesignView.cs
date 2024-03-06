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
            WpfPlot PlanViewPlot, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList, List<double> uniqueX, List<double> uniqueY)
        {
            bool isError = false;

            //Draw plot
            double countFail = 0;
            double countPass = 0;
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredList = new List<ClassWallDesign.WallDesignDetail>();
            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {

                List<double> xCoordListChoosenGroupPlanView = new List<double>();
                List<double> yCoordListChoosenGroupPlanView = new List<double>();
                List<double> xCoordListNotChoosenGroupPlanView = new List<double>();
                List<double> yCoordListNotChoosenGroupPlanView = new List<double>();

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

                        if (!xCoordListChoosenGroupPlanView.Contains(point.XCoordinate))
                        {
                            xCoordListChoosenGroupPlanView.Add(point.XCoordinate);
                        }
                        if (!yCoordListChoosenGroupPlanView.Contains(point.YCoordinate))
                        {
                            yCoordListChoosenGroupPlanView.Add(point.YCoordinate);
                        }

                    }

                    //For result plot
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

                    //Plot plan view
                    double xCenterChoosenGroupPlanView = xCoordListChoosenGroupPlanView.Average();
                    double yCenterChoosenGroupPlanView = yCoordListChoosenGroupPlanView.Average();
                    double xWidthChoosenGroupPlanView = Math.Max(0.5, xCoordListChoosenGroupPlanView.Max() - xCoordListChoosenGroupPlanView.Min());
                    double yWidthChoosenGroupPlanView = Math.Max(0.5, yCoordListChoosenGroupPlanView.Max() - yCoordListChoosenGroupPlanView.Min());

                    HelperPlot.RectanglePlotter(PlanViewPlot, xCenterChoosenGroupPlanView, yCenterChoosenGroupPlanView, "", 0,
                        System.Drawing.ColorTranslator.FromHtml("red"), xWidthChoosenGroupPlanView, yWidthChoosenGroupPlanView, 0.01,
                        System.Drawing.ColorTranslator.FromHtml("red"));
                }
                else
                {
                    foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                    {
                        if (!xCoordListNotChoosenGroupPlanView.Contains(point.XCoordinate))
                        {
                            xCoordListNotChoosenGroupPlanView.Add(point.XCoordinate);
                        }
                        if (!yCoordListNotChoosenGroupPlanView.Contains(point.YCoordinate))
                        {
                            yCoordListNotChoosenGroupPlanView.Add(point.YCoordinate);
                        }
                    }


                    //Plot plan view
                    double xCenterNotChoosenGroupPlanView = xCoordListNotChoosenGroupPlanView.Average();
                    double yCenterNotChoosenGroupPlanView = yCoordListNotChoosenGroupPlanView.Average();
                    double xWidthNotChoosenGroupPlanView = Math.Max(0.1, xCoordListNotChoosenGroupPlanView.Max() - xCoordListNotChoosenGroupPlanView.Min());
                    double yWidthNotChoosenGroupPlanView = Math.Max(0.1, yCoordListNotChoosenGroupPlanView.Max() - yCoordListNotChoosenGroupPlanView.Min());

                    HelperPlot.RectanglePlotter(PlanViewPlot, xCenterNotChoosenGroupPlanView, yCenterNotChoosenGroupPlanView, "", 0,
                        System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"), xWidthNotChoosenGroupPlanView, yWidthNotChoosenGroupPlanView, 0.01,
                        System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

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

            PlanViewPlot.Plot.AxisScaleLock(true);
            PlanViewPlot.Refresh();

            return (isError, wallDesignDetailList, wallDesignDetailFilteredList, uniqueX, uniqueY, countPass, countFail);
        }

        public static bool? IsWallFail(string typeOfFailure, ClassWallDesign.WallDesignDetail wallDesignDetail)
        {
            //If no design exist
            if (wallDesignDetail.IsFailAll == null || wallDesignDetail.IsFailPMN == null || wallDesignDetail.IsFailShear == null)
            {
                return null;
            }


            if (typeOfFailure == "All Failure" && wallDesignDetail.IsFailAll == true)
            {
                return true;
            }
            else if (typeOfFailure == "PMN Failure" && wallDesignDetail.IsFailPMN == true)
            {
                return true;
            }
            else if (typeOfFailure == "Shear Failure" && wallDesignDetail.IsFailShear == true)
            {
                return true;
            }

            return false;
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

        public static (List<ClassWallDesign.WallDesignDetail>, List<double>, List<double>, double, double) FilterWallDesignResultChangeGroup(WpfPlot ResultPlot, WpfPlot PlanViewPlot, bool? showOk, bool? showFail, bool? showNull, 
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            int titleSize, int axisTitleSize,
            int tickLabelSize, string choosenGroup, string typeOfFailure, double minReinfPercent, double maxReinfPercent)
        {
            ScottPlot.AxisLimits axisLimitsResultPlot = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            ScottPlot.AxisLimits axisLimitsPlanViewPlot = PlanViewPlot.Plot.GetAxisLimits();
            PlanViewPlot.Reset();

            double countPass = 0;
            double countFail = 0;

            bool isError = false;

            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();

            List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredList = new List<ClassWallDesign.WallDesignDetail>();

            if (typeOfFailure != "Pier Req. Rebar %")
            {
                foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
                {
                    List<double> xCoordListChoosenGroupPlanView = new List<double>();
                    List<double> yCoordListChoosenGroupPlanView = new List<double>();
                    List<double> xCoordListNotChoosenGroupPlanView = new List<double>();
                    List<double> yCoordListNotChoosenGroupPlanView = new List<double>();

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
                            return (wallDesignDetailFilteredList, null, null, 0, 0);
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

                            if (!xCoordListChoosenGroupPlanView.Contains(point.XCoordinate))
                            {
                                xCoordListChoosenGroupPlanView.Add(point.XCoordinate);
                            }
                            if (!yCoordListChoosenGroupPlanView.Contains(point.YCoordinate))
                            {
                                yCoordListChoosenGroupPlanView.Add(point.YCoordinate);
                            }
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


                        if (IsWallFail(typeOfFailure, wallDesignDetail) == true)
                        {
                            countFail++;
                        }
                        else if (IsWallFail(typeOfFailure, wallDesignDetail) == false)
                        {
                            countPass++;
                        }


                        HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, System.Drawing.ColorTranslator.FromHtml(color), xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

                        //Plot plan view
                        double xCenterChoosenGroupPlanView = xCoordListChoosenGroupPlanView.Average();
                        double yCenterChoosenGroupPlanView = yCoordListChoosenGroupPlanView.Average();
                        double xWidthChoosenGroupPlanView = Math.Max(0.5, xCoordListChoosenGroupPlanView.Max() - xCoordListChoosenGroupPlanView.Min());
                        double yWidthChoosenGroupPlanView = Math.Max(0.5, yCoordListChoosenGroupPlanView.Max() - yCoordListChoosenGroupPlanView.Min());

                        HelperPlot.RectanglePlotter(PlanViewPlot, xCenterChoosenGroupPlanView, yCenterChoosenGroupPlanView, "", 0,
                            System.Drawing.ColorTranslator.FromHtml("red"), xWidthChoosenGroupPlanView, yWidthChoosenGroupPlanView, 0.01,
                            System.Drawing.ColorTranslator.FromHtml("red"));

                    }
                    else
                    {
                        foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                        {
                            if (!xCoordListNotChoosenGroupPlanView.Contains(point.XCoordinate))
                            {
                                xCoordListNotChoosenGroupPlanView.Add(point.XCoordinate);
                            }
                            if (!yCoordListNotChoosenGroupPlanView.Contains(point.YCoordinate))
                            {
                                yCoordListNotChoosenGroupPlanView.Add(point.YCoordinate);
                            }
                        }


                        //Plot plan view
                        double xCenterNotChoosenGroupPlanView = xCoordListNotChoosenGroupPlanView.Average();
                        double yCenterNotChoosenGroupPlanView = yCoordListNotChoosenGroupPlanView.Average();
                        double xWidthNotChoosenGroupPlanView = Math.Max(0.1, xCoordListNotChoosenGroupPlanView.Max() - xCoordListNotChoosenGroupPlanView.Min());
                        double yWidthNotChoosenGroupPlanView = Math.Max(0.1, yCoordListNotChoosenGroupPlanView.Max() - yCoordListNotChoosenGroupPlanView.Min());

                        HelperPlot.RectanglePlotter(PlanViewPlot, xCenterNotChoosenGroupPlanView, yCenterNotChoosenGroupPlanView, "", 0,
                            System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"), xWidthNotChoosenGroupPlanView, yWidthNotChoosenGroupPlanView, 0.01,
                            System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

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
            }
            else if (typeOfFailure == "Pier Req. Rebar %")
            {
                foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
                {
                    List<double> xCoordListChoosenGroupPlanView = new List<double>();
                    List<double> yCoordListChoosenGroupPlanView = new List<double>();
                    List<double> xCoordListNotChoosenGroupPlanView = new List<double>();
                    List<double> yCoordListNotChoosenGroupPlanView = new List<double>();

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
                            return (wallDesignDetailFilteredList, null, null, 0, 0);
                        }

                        System.Drawing.Color color;
                        if (wallDesignDetail.PierReinfPercent > 0)
                        {
                            double colorFraction = (wallDesignDetail.PierReinfPercent - minReinfPercent) / (maxReinfPercent - minReinfPercent);
                            color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorFraction);

                        }
                        else
                        {
                            color = System.Drawing.ColorTranslator.FromHtml("#00FFFFFF");
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

                            if (!xCoordListChoosenGroupPlanView.Contains(point.XCoordinate))
                            {
                                xCoordListChoosenGroupPlanView.Add(point.XCoordinate);
                            }
                            if (!yCoordListChoosenGroupPlanView.Contains(point.YCoordinate))
                            {
                                yCoordListChoosenGroupPlanView.Add(point.YCoordinate);
                            }
                        }

                        double xCenter = xCoordList.Average();
                        double yCenter = yCoordList.Average();

                        double xWidth = xCoordList.Max() - xCoordList.Min();
                        double yWidth = yCoordList.Max() - yCoordList.Min();

                        HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, color, xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

                        //Plot plan view
                        double xCenterChoosenGroupPlanView = xCoordListChoosenGroupPlanView.Average();
                        double yCenterChoosenGroupPlanView = yCoordListChoosenGroupPlanView.Average();
                        double xWidthChoosenGroupPlanView = Math.Max(0.5, xCoordListChoosenGroupPlanView.Max() - xCoordListChoosenGroupPlanView.Min());
                        double yWidthChoosenGroupPlanView = Math.Max(0.5, yCoordListChoosenGroupPlanView.Max() - yCoordListChoosenGroupPlanView.Min());

                        HelperPlot.RectanglePlotter(PlanViewPlot, xCenterChoosenGroupPlanView, yCenterChoosenGroupPlanView, "", 0,
                            System.Drawing.ColorTranslator.FromHtml("red"), xWidthChoosenGroupPlanView, yWidthChoosenGroupPlanView, 0.01,
                            System.Drawing.ColorTranslator.FromHtml("red"));

                    }
                    else
                    {
                        foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
                        {
                            if (!xCoordListNotChoosenGroupPlanView.Contains(point.XCoordinate))
                            {
                                xCoordListNotChoosenGroupPlanView.Add(point.XCoordinate);
                            }
                            if (!yCoordListNotChoosenGroupPlanView.Contains(point.YCoordinate))
                            {
                                yCoordListNotChoosenGroupPlanView.Add(point.YCoordinate);
                            }
                        }


                        //Plot plan view
                        double xCenterNotChoosenGroupPlanView = xCoordListNotChoosenGroupPlanView.Average();
                        double yCenterNotChoosenGroupPlanView = yCoordListNotChoosenGroupPlanView.Average();
                        double xWidthNotChoosenGroupPlanView = Math.Max(0.1, xCoordListNotChoosenGroupPlanView.Max() - xCoordListNotChoosenGroupPlanView.Min());
                        double yWidthNotChoosenGroupPlanView = Math.Max(0.1, yCoordListNotChoosenGroupPlanView.Max() - yCoordListNotChoosenGroupPlanView.Min());

                        HelperPlot.RectanglePlotter(PlanViewPlot, xCenterNotChoosenGroupPlanView, yCenterNotChoosenGroupPlanView, "", 0,
                            System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"), xWidthNotChoosenGroupPlanView, yWidthNotChoosenGroupPlanView, 0.01,
                            System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

                    }
                }

                //Add color bar
                var cb = ResultPlot.Plot.AddColorbar(ScottPlot.Drawing.Colormap.Jet);
                cb.MaxIsClipped = true;

                // Add manual ticks (disabling automatic ticks)
                double tick = 0;
                for (double reinfPercent = minReinfPercent; Math.Round(reinfPercent, 2) <= maxReinfPercent; reinfPercent += (maxReinfPercent - minReinfPercent) / 10)
                {
                    cb.AddTick(tick, Convert.ToString(Math.Round(reinfPercent, 2) + " %"));
                    tick += 0.1; //1/10
                }
            }

            ResultPlot.Plot.AxisScaleLock(true);
            ResultPlot.Plot.Title("Wall Design Result", true, null, titleSize, "Arial Narrow");

            //Axis
            ResultPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            ResultPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);

            ResultPlot.Plot.SetAxisLimits(axisLimitsResultPlot);

            ResultPlot.Refresh();

            PlanViewPlot.Plot.SetAxisLimits(axisLimitsPlanViewPlot);
            PlanViewPlot.Plot.AxisScaleLock(true);
            PlanViewPlot.Refresh();

            return (wallDesignDetailFilteredList, uniqueX, uniqueY, countPass, countFail);
        }

        public static (double, double) FilterWallDesignResultChangeViewType(WpfPlot ResultPlot, bool? showOk, bool? showFail, bool? showNull, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            List<double> uniqueX, List<double> uniqueY, int titleSize, int axisTitleSize,
            int tickLabelSize, string typeOfFailure, double minReinfPercent, double maxReinfPercent)
        {
            ScottPlot.AxisLimits axisLimits = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            double countFail = 0;
            double countPass = 0;
            if (typeOfFailure != "Pier Req. Rebar %") 
            {

                foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
                {
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
            }
            else if (typeOfFailure == "Pier Req. Rebar %") 
            {
                foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
                {
                    System.Drawing.Color color;
                    if (wallDesignDetail.PierReinfPercent > 0)
                    {
                        double colorFraction = (wallDesignDetail.PierReinfPercent - minReinfPercent) / (maxReinfPercent - minReinfPercent);
                        color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorFraction);

                    }
                    else
                    {
                        color = System.Drawing.ColorTranslator.FromHtml("#00FFFFFF");
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

                    HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, color, xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));

                }
                //Add color bar
                var cb = ResultPlot.Plot.AddColorbar(ScottPlot.Drawing.Colormap.Jet);

                // Add manual ticks (disabling automatic ticks)
                double tick = 0;
                for (double reinfPercent = minReinfPercent; Math.Round(reinfPercent, 2) <= maxReinfPercent; reinfPercent += (maxReinfPercent - minReinfPercent) / 10)
                {
                    cb.AddTick(tick, Convert.ToString(Math.Round(reinfPercent, 2) + " %"));
                    tick += 0.1; //1/10
                }


            }

            ResultPlot.Plot.AxisScaleLock(true);
            ResultPlot.Plot.Title("Wall Design Result", true, null, titleSize, "Arial Narrow");

            //Axis
            ResultPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ResultPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            ResultPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);

            ResultPlot.Plot.SetAxisLimits(axisLimits);

            ResultPlot.Refresh();

            return (countPass, countFail);
        }

    }

}
