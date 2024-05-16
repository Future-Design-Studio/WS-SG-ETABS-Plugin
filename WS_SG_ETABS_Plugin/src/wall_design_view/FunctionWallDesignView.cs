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
            WpfPlot PlanViewPlot, string choosenGroup, string viewType,
            int titleSize, int axisTitleSize,
            int tickLabelSize, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList, bool? showOk, bool? showFail, bool? showNull,
            double minReinfPercent, double maxReinfPercent)
        {
            bool isError = false;

            //Draw plot
            double countFail = 0;
            double countPass = 0;
            double areaFail = 0;
            double areaPass = 0;
            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();
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

                    //Check for planar
                    bool isPlanar = false;
                    (uniqueX, uniqueY, isPlanar) = IsPlanar(wallDesignDetail);
                    if (!isPlanar)
                    {
                        MessageBox.Show("Chosen walls is not in 1 plane! Walls must be on either XZ or YZ plane!");
                        isError = true;
                        return (isError, null, null, null, null, 0, 0);
                    }

                    List<double> xCoordList = new List<double>();
                    List<double> yCoordList = new List<double>();

                    GetResultAndPlanPlotCoordinate(wallDesignDetail, ref uniqueX, ref uniqueY, ref xCoordList, ref yCoordList, ref xCoordListChoosenGroupPlanView, ref yCoordListChoosenGroupPlanView);
                    DrawInResultPlot(ResultPlot, wallDesignDetail, xCoordList, yCoordList, viewType, ref countPass, ref countFail, showOk, showFail, showNull, 
                        minReinfPercent, maxReinfPercent, true, true, ref areaPass, ref areaFail);
                    DrawInPlanViewPlot(PlanViewPlot, xCoordListChoosenGroupPlanView, yCoordListChoosenGroupPlanView, true);
                }
                else
                {
                    GetPlanViewPlotCoordinate(wallDesignDetail, ref xCoordListNotChoosenGroupPlanView, ref yCoordListNotChoosenGroupPlanView);
                    DrawInPlanViewPlot(PlanViewPlot, xCoordListNotChoosenGroupPlanView, yCoordListNotChoosenGroupPlanView, false);
                }
                
            }

            SetPlotAnnotation(ResultPlot, viewType, minReinfPercent, maxReinfPercent, countFail, countPass, areaFail, areaPass);

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

        public static (List<ClassWallDesign.WallDesignDetail>, List<double>, List<double>) FilterWallDesignResultChangeGroup(WpfPlot ResultPlot, WpfPlot PlanViewPlot, bool? showOk, bool? showFail, bool? showNull, 
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            int titleSize, int axisTitleSize,
            int tickLabelSize, string choosenGroup, string viewType, double minReinfPercent, double maxReinfPercent, bool? showPier, bool? showSpandrel)
        {
            ScottPlot.AxisLimits axisLimitsResultPlot = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            ScottPlot.AxisLimits axisLimitsPlanViewPlot = PlanViewPlot.Plot.GetAxisLimits();
            PlanViewPlot.Reset();

            double countPass = 0;
            double countFail = 0;
            double areaPass = 0;
            double areaFail = 0;

            bool isError = false;

            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();

            List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredList = new List<ClassWallDesign.WallDesignDetail>();

            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                List<double> xCoordListChoosenGroupPlanView = new List<double>();
                List<double> yCoordListChoosenGroupPlanView = new List<double>();
                List<double> xCoordListNotChoosenGroupPlanView = new List<double>();
                List<double> yCoordListNotChoosenGroupPlanView = new List<double>();

                //Filter based on group and filter based on wall type pier vs spandrel
                if ((wallDesignDetail.Group == choosenGroup))
                {
                    wallDesignDetailFilteredList.Add(wallDesignDetail);

                    //Check for planar
                    bool isPlanar = false;
                    (uniqueX, uniqueY, isPlanar) = IsPlanar(wallDesignDetail);
                    if (!isPlanar)
                    {
                        MessageBox.Show("Chosen walls is not in 1 plane! Walls must be on either XZ or YZ plane!");
                        isError = true;
                        return (wallDesignDetailFilteredList, null, null);
                    }

                    List<double> xCoordList = new List<double>();
                    List<double> yCoordList = new List<double>();
                    GetResultAndPlanPlotCoordinate(wallDesignDetail, ref uniqueX, ref uniqueY, ref xCoordList, ref yCoordList,
                        ref xCoordListChoosenGroupPlanView, ref yCoordListChoosenGroupPlanView);

                    DrawInResultPlot(ResultPlot, wallDesignDetail, xCoordList, yCoordList, viewType, ref countPass, ref countFail, showOk, showFail, showNull, 
                        minReinfPercent, maxReinfPercent, showPier, showSpandrel, ref areaPass, ref areaFail);
                    DrawInPlanViewPlot(PlanViewPlot, xCoordListChoosenGroupPlanView, yCoordListChoosenGroupPlanView, true);
                }
                else
                {
                    GetPlanViewPlotCoordinate(wallDesignDetail, ref xCoordListNotChoosenGroupPlanView, ref yCoordListNotChoosenGroupPlanView);
                    DrawInPlanViewPlot(PlanViewPlot, xCoordListNotChoosenGroupPlanView, yCoordListNotChoosenGroupPlanView, false);
                }
            }

            SetPlotAnnotation(ResultPlot, viewType, minReinfPercent, maxReinfPercent, countFail, countPass, areaFail, areaPass);
            SetResultPlotSetting(ResultPlot, titleSize, axisTitleSize, tickLabelSize, axisLimitsResultPlot);

            PlanViewPlot.Plot.SetAxisLimits(axisLimitsPlanViewPlot);
            PlanViewPlot.Plot.AxisScaleLock(true);
            PlanViewPlot.Refresh();

            return (wallDesignDetailFilteredList, uniqueX, uniqueY);
        }

        public static void FilterWallDesignResultChangeViewType(WpfPlot ResultPlot, bool? showOk, bool? showFail, bool? showNull, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList,
            List<double> uniqueX, List<double> uniqueY, int titleSize, int axisTitleSize,
            int tickLabelSize, string viewType, double minReinfPercent, double maxReinfPercent, bool? showPier, bool? showSpandrel)
        {
            ScottPlot.AxisLimits axisLimits = ResultPlot.Plot.GetAxisLimits();
            ResultPlot.Reset();

            double countFail = 0;
            double countPass = 0;
            double areaFail = 0;
            double areaPass = 0;

            foreach (ClassWallDesign.WallDesignDetail wallDesignDetail in wallDesignDetailList)
            {
                List<double> xCoordList = new List<double>();
                List<double> yCoordList = new List<double>();
                GetResultPlotCoordinate(wallDesignDetail, ref uniqueX, ref uniqueY, ref xCoordList, ref yCoordList);
                DrawInResultPlot(ResultPlot, wallDesignDetail, xCoordList, yCoordList, viewType, ref countPass, ref countFail, showOk, showFail, showNull, 
                    minReinfPercent, maxReinfPercent, showPier, showSpandrel, ref areaPass, ref areaFail);
            }

            SetPlotAnnotation(ResultPlot, viewType, minReinfPercent, maxReinfPercent, countFail, countPass, areaFail, areaPass);
            SetResultPlotSetting(ResultPlot, titleSize, axisTitleSize, tickLabelSize, axisLimits);
        }

        public static bool? IsWallFail(string viewType, ClassWallDesign.WallDesignDetail wallDesignDetail)
        {

            //If required rebar return null
            if (viewType == "Pier Req. Rebar %")
            {
                return null;
            }
            else
            {
                //If no design exist
                if (wallDesignDetail.IsFailAll == null || wallDesignDetail.IsFailPMN == null || wallDesignDetail.IsFailShear == null)
                {
                    return null;
                }

                if (viewType == "All Failure" && wallDesignDetail.IsFailAll == true)
                {
                    return true;
                }
                else if (viewType == "PMN Failure" && wallDesignDetail.IsFailPMN == true)
                {
                    return true;
                }
                else if (viewType == "Shear Failure" && wallDesignDetail.IsFailShear == true)
                {
                    return true;
                }

                return false;
            }
        }

        private static (List<double>, List<double>, bool) IsPlanar (ClassWallDesign.WallDesignDetail wallDesignDetail)
        {
            List<double> uniqueX = new List<double>();
            List<double> uniqueY = new List<double>();

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

                //Check if planar
                if (uniqueX.Count == 1 || uniqueY.Count == 1)
                {
                    //continue still planar
                }
                else
                {
                    return (uniqueX, uniqueY, false);
                }
            }

            return (uniqueX, uniqueY, true);
        }

        public static void GetResultAndPlanPlotCoordinate (ClassWallDesign.WallDesignDetail wallDesignDetail, ref List<double>  uniqueX, ref List<double>  uniqueY,
            ref List<double> xCoordList, ref List<double> yCoordList, ref List<double> xCoordListPlanView, ref List<double> yCoordListPlanView)
        {
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

                //Plan view
                if (!xCoordListPlanView.Contains(point.XCoordinate))
                {
                    xCoordListPlanView.Add(point.XCoordinate);
                }
                if (!yCoordListPlanView.Contains(point.YCoordinate))
                {
                    yCoordListPlanView.Add(point.YCoordinate);
                }
            }

        }

        public static void GetResultPlotCoordinate(ClassWallDesign.WallDesignDetail wallDesignDetail, ref List<double> uniqueX, ref List<double> uniqueY, ref List<double> xCoordList, ref List<double> yCoordList)
        {
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

        }

        public static void GetPlanViewPlotCoordinate(ClassWallDesign.WallDesignDetail wallDesignDetail, 
            ref List<double> xCoordListPlanView, ref List<double> yCoordListPlanView)
        {

            foreach (HelperEtabs.Point3D point in wallDesignDetail.PointList)
            {
                //Plan view
                if (!xCoordListPlanView.Contains(point.XCoordinate))
                {
                    xCoordListPlanView.Add(point.XCoordinate);
                }
                if (!yCoordListPlanView.Contains(point.YCoordinate))
                {
                    yCoordListPlanView.Add(point.YCoordinate);
                }
            }

        }

        public static void DrawInResultPlot(WpfPlot ResultPlot, ClassWallDesign.WallDesignDetail wallDesignDetail, List<double> xCoordList, List<double> yCoordList, string viewType,
            ref double countPass, ref double countFail, bool? showOk, bool? showFail, bool? showNull, double minReinfPercent, double maxReinfPercent, bool? showPier, bool? showSpandrel,
            ref double areaPass, ref double areaFail)
        {

            //For result plot
            double xCenter = xCoordList.Average();
            double yCenter = yCoordList.Average();

            double xWidth = xCoordList.Max() - xCoordList.Min();
            double yWidth = yCoordList.Max() - yCoordList.Min();

            double area = xWidth * yWidth;

            //3 levels of filters
            //1. filter by wall type eg. pier or spandrel
            //2. filter by view type eg. pier req rebar or fail/pass
            //3. filter by fail type eg. all or PMN or shear

            if(showPier == true && wallDesignDetail.PierLabel != null || showSpandrel == true && wallDesignDetail.SpandrelLabel != null)
            {
                if (viewType != "Pier Req. Rebar %")
                {
                    string color = "";
                    if ((IsWallFail(viewType, wallDesignDetail) == false && showOk == true) || (IsWallFail(viewType, wallDesignDetail) == true && showFail == true) || (IsWallFail(viewType, wallDesignDetail) == null && showNull == true))
                    {
                        if (IsWallFail(viewType, wallDesignDetail) == true)
                        {
                            color = "red";
                        }
                        else if (IsWallFail(viewType, wallDesignDetail) == false)
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
                        {
                            color = "#00FFFFFF";
                        }
                    }

                    if (IsWallFail(viewType, wallDesignDetail) == true)
                    {
                        countFail++;
                        areaFail += area;
                    }
                    else if (IsWallFail(viewType, wallDesignDetail) == false)
                    {
                        countPass++;
                        areaPass += area;
                    }


                    HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, System.Drawing.ColorTranslator.FromHtml(color), xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));
                }
                else if (viewType == "Pier Req. Rebar %")
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

                    HelperPlot.RectanglePlotter(ResultPlot, xCenter, yCenter, "", 0, color, xWidth, yWidth, 0.01, System.Drawing.ColorTranslator.FromHtml("#80D3D3D3"));
                }
            }

        }

        public static void DrawInPlanViewPlot(WpfPlot PlanViewPlot, List<double> xCoordListPlanView, List<double> yCoordListPlanView, bool choosenGroup)
        {
            //Plot plan view
            double xCenterChoosenGroupPlanView = xCoordListPlanView.Average();
            double yCenterChoosenGroupPlanView = yCoordListPlanView.Average();
            double xWidthChoosenGroupPlanView = Math.Max(0.5, xCoordListPlanView.Max() - xCoordListPlanView.Min());
            double yWidthChoosenGroupPlanView = Math.Max(0.5, yCoordListPlanView.Max() - yCoordListPlanView.Min());

            string color;
            if (choosenGroup)
            {
                color = "red";
            }
            else
            {
                color = "#80D3D3D3";
            }


            HelperPlot.RectanglePlotter(PlanViewPlot, xCenterChoosenGroupPlanView, yCenterChoosenGroupPlanView, "", 0,
                System.Drawing.ColorTranslator.FromHtml(color), xWidthChoosenGroupPlanView, yWidthChoosenGroupPlanView, 0.01,
                System.Drawing.ColorTranslator.FromHtml(color));
        }

        public static void SetPlotAnnotation(WpfPlot ResultPlot, string viewType, double minReinfPercent, double maxReinfPercent, double countFail, double countPass, 
            double areaFail, double areaPass)
        {
            if (viewType != "Pier Req. Rebar %")
            {
                /*
                //Percent pass fail by count
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
                */

                //Percent pass fail by area
                double percentAreaPass = areaPass / (areaFail + areaPass) * 100;
                percentAreaPass = Math.Round(percentAreaPass, 2);
                double percentAreaFail = areaFail / (areaFail + areaPass) * 100;
                percentAreaFail = Math.Round(percentAreaFail, 2);

                string percentAreaPassText = $"Pass {percentAreaPass}%";
                string percentAreaFailText = $"Fail {percentAreaFail}%";

                //Add anotation
                HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentAreaPassText), 25, 10, 10, 1, System.Drawing.ColorTranslator.FromHtml("green"),
                    System.Drawing.ColorTranslator.FromHtml("white"), true);
                HelperPlot.AddAnnotationToPlot(ResultPlot, Convert.ToString(percentAreaFailText), 25, 10, 50, 1, System.Drawing.ColorTranslator.FromHtml("red"),
                    System.Drawing.ColorTranslator.FromHtml("white"), true);
            }
            else if (viewType == "Pier Req. Rebar %")
            {
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
        }

        public static void SetResultPlotSetting(WpfPlot ResultPlot, int titleSize, int axisTitleSize,
            int tickLabelSize, ScottPlot.AxisLimits axisLimits)
        {
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
