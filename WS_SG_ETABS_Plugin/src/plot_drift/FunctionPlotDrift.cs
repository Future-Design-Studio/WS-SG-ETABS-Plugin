using Microsoft.Office.Interop.Excel;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Animation;
using WS_SG_ETABS_Plugin.src.pad_footing_plot;
using static WS_SG_ETABS_Plugin.src.plot_drift.ClassDriftPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    internal class FunctionDriftPlot
    {
        public static bool PlotDrift(ETABSv1.cSapModel SapModel, ETABSv1.cPluginCallback Plugin, List<string> listOutputCase, string displacementDirection, string maxAllowableDispType, string maxAllowableDisplacementFactor, string choosenGroup,
            int legendTextSize, int titleSize, int axisTitleSize, int tickLabelSize, double gridSpacingDisplacementX, double gridSpacingDriftX, double gridSpacingY, bool showMaxDisp,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath, WpfPlot DriftPlot, WpfPlot DisplacementPlot, 
            string displacementXAxisMaxType, string displacementXAxisMax, string driftXAxisMaxType, string driftXAxisMax)
        {
            bool isError = false;
            List<string> errorList = new List<string>();
            List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitions = new List<ClassPadFootPlot.GroupDefinition>();

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError); }

            //Filter group to only include points, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Point");
            if (isError) { return (isError); }

            //Container
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();
            List<src.HelperEtabs.Point3D> jointListFilteredByGroup = new List<src.HelperEtabs.Point3D>();
            List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList = new List<ClassPadFootPlot.JointPadFootDetails>();

            //Get all points
            jointList = HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjectConnectivityTable);

            //Get joint which is part of the choosen group
            jointListFilteredByGroup = src.HelperEtabs.GetJointListFromGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);

            //Check direction
            if (displacementDirection != "Ux and Uy (in one)")
            {
                string dispOrDriftLineColor = "";
                string dispOrDriftMaxMinLineColor = "#808080";

                double index = 0;
                foreach (string outputCase in listOutputCase)
                {

                    if (listOutputCase.Count > 1)
                    {

                        double colorMapFraction = index / (Convert.ToDouble(listOutputCase.Count) - 1);
                        System.Drawing.Color color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .6);
                        dispOrDriftLineColor = ColorTranslator.ToHtml(color);
                    }
                    else
                    {
                        dispOrDriftLineColor = "#FFCE00";
                    }

                    isError = AddLineToPlot(displacementDirection, showMaxDisp, databaseTableList, fileName, filePath, DriftPlot, DisplacementPlot,
                                            jointListFilteredByGroup, jointList, dispOrDriftLineColor,
                                            dispOrDriftMaxMinLineColor, outputCase);

                    if (isError) { return isError; }

                    //Adding legend
                    DisplacementPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, $"{outputCase}");
                    DriftPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, $"{outputCase}");

                    index++;
                }

            }
            else
            {
                string dispOrDriftLineColorUx = "#FFCE00";
                string dispOrDriftLineColorUy = "#00C3FF";
                string dispOrDriftMaxMinLineColor = "#808080";

                foreach (string outputCase in listOutputCase)
                {
                    


                    isError = AddLineToPlot("Ux", showMaxDisp, databaseTableList, fileName, filePath, DriftPlot, DisplacementPlot,
                                            jointListFilteredByGroup, jointList, dispOrDriftLineColorUx, 
                                            dispOrDriftMaxMinLineColor, outputCase);

                    if (isError) { return isError; }

                    isError = AddLineToPlot("Uy", showMaxDisp, databaseTableList, fileName, filePath, DriftPlot, DisplacementPlot,
                            jointListFilteredByGroup, jointList, dispOrDriftLineColorUy, 
                            dispOrDriftMaxMinLineColor, outputCase);

                    if (isError) { return isError; }

                }

                //Adding legend
                DisplacementPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColorUx), 2, LineStyle.Solid, $"In the X direction");
                DisplacementPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColorUy), 2, LineStyle.Solid, $"In the Y direction");
                DriftPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColorUx), 2, LineStyle.Solid, $"In the X direction");
                DriftPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColorUy), 2, LineStyle.Solid, $"In the Y direction");
            }

            //Plot limit
            string limitLineColor = "#FF0000";
            double[] zCoordinateList;
            double[] displacementList;
            double[] driftList;
            List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> limitJointDisplacementDetailList = GetJointDisplacementLimit(jointListFilteredByGroup, maxAllowableDisplacementFactor);
            if (maxAllowableDispType != "hide")
            {
                (zCoordinateList, displacementList, driftList) = ConvertToArray(limitJointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(limitLineColor), 2, LineStyle.Dash);
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(limitLineColor), 2, LineStyle.Dash);
            }

            //Adding legend
            var denominator = maxAllowableDisplacementFactor.Split('/')[1];
            if (maxAllowableDispType != "hide")
            {
                DisplacementPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(limitLineColor), 2, LineStyle.Dash, $"Limit H/{denominator}");

            }

            if (maxAllowableDispType != "hide")
            {
                DriftPlot.Plot.AddScatterLines(new double[] { 0 }, new double[] { 0 }, System.Drawing.ColorTranslator.FromHtml(limitLineColor), 2, LineStyle.Dash, $"Limit h/{denominator}");
            }

            //X Axis adjustment to user limit
            if (displacementXAxisMaxType != "auto")
            {
                DisplacementPlot.Plot.SetAxisLimits(null, Convert.ToDouble(displacementXAxisMax), null, null);
            }
            if (driftXAxisMaxType != "auto")
            {
                DriftPlot.Plot.SetAxisLimits(null, Convert.ToDouble(driftXAxisMax), null, null);
            }

            //Setting for displacement plot
            if (listOutputCase.Count == 1)
            {
                string outputCase = listOutputCase[0];

                if (displacementDirection == "Uz")
                {
                    DisplacementPlot.Plot.Title($"Vertical Displacement (Case: {outputCase})", true, null, titleSize, "Arial Narrow");
                }
                else if(displacementDirection == "Ux and Uy (in one)")
                {
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Case: {outputCase})", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux + Uy (SRSS)")
                {
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Case: {outputCase}, {displacementDirection})", true, null, titleSize, "Arial Narrow");
                }
                else
                {
                    char direction = char.ToUpper(displacementDirection[1]);
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Case: {outputCase}, {direction} direction)", true, null, titleSize, "Arial Narrow");
                }

            }
            else
            {
                if (displacementDirection == "Uz")
                {
                    DisplacementPlot.Plot.Title($"Vertical Displacement (Multi cases)", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux and Uy (in one)")
                {
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Multi cases)", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux + Uy (SRSS)")
                {
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Multi cases, {displacementDirection})", true, null, titleSize, "Arial Narrow");
                }
                else
                {
                    char direction = char.ToUpper(displacementDirection[1]);
                    DisplacementPlot.Plot.Title($"Lateral Displacement (Multi cases, {direction} direction)", true, null, titleSize, "Arial Narrow");
                }
            }

            DisplacementPlot.Plot.YAxis.Label("Height (m)", null, axisTitleSize, true, "Arial Narrow");
            DisplacementPlot.Plot.XAxis.Label("Displacement (mm)", null, axisTitleSize, true, "Arial Narrow");
            DisplacementPlot.Plot.XAxis.ManualTickSpacing(gridSpacingDisplacementX);
            DisplacementPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            DisplacementPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            DisplacementPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);

            var displacementLegend = DisplacementPlot.Plot.Legend();
            displacementLegend.FontSize = legendTextSize;

            //Setting for drift plot
            if (listOutputCase.Count == 1)
            {
                string outputCase = listOutputCase[0];
                if (displacementDirection == "Uz")
                {
                    DriftPlot.Plot.Title($"Vertical Displacement Increment (Case: {outputCase})", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux and Uy (in one)")
                {
                    DriftPlot.Plot.Title($"Inter-story Drift (Case: {outputCase})", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux + Uy (SRSS)")
                {
                    DriftPlot.Plot.Title($"Inter-story Drift (Case: {outputCase}, {displacementDirection})", true, null, titleSize, "Arial Narrow");
                }
                else
                {
                    char direction = char.ToUpper(displacementDirection[1]);
                    DriftPlot.Plot.Title($"Inter-story Drift (Case: {outputCase}, {direction} direction)", true, null, titleSize, "Arial Narrow");
                }
            }

            else
            {
                if (displacementDirection == "Uz")
                {
                    DriftPlot.Plot.Title($"Vertical Displacement Increment (Multi cases)", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux and Uy (in one)")
                {
                    DriftPlot.Plot.Title($"Inter-story Drift (Multi cases)", true, null, titleSize, "Arial Narrow");
                }
                else if (displacementDirection == "Ux + Uy (SRSS)")
                {
                    DriftPlot.Plot.Title($"Inter-story Drift (Multi cases, {displacementDirection})", true, null, titleSize, "Arial Narrow");
                }
                else
                {
                    char direction = char.ToUpper(displacementDirection[1]);
                    DriftPlot.Plot.Title($"Inter-story Drift (Multi cases, {direction} direction)", true, null, titleSize, "Arial Narrow");
                }

            }

            DriftPlot.Plot.YAxis.Label("Height (m)", null, axisTitleSize, true, "Arial Narrow");
            DriftPlot.Plot.XAxis.Label("Inter-story Drift", null, axisTitleSize, true, "Arial Narrow");
            DriftPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            DriftPlot.Plot.XAxis.ManualTickSpacing(gridSpacingDriftX);
            DriftPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            DriftPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);

            var driftLegend = DriftPlot.Plot.Legend();
            driftLegend.FontSize = legendTextSize;

            DisplacementPlot.Refresh();
            DriftPlot.Refresh();

            return isError;
        }

        public static bool AddLineToPlot(string displacementDirection, bool showMaxDisp,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath, WpfPlot DriftPlot, WpfPlot DisplacementPlot, 
            List<src.HelperEtabs.Point3D> jointListFilteredByGroup, List<src.HelperEtabs.Point3D> jointList, string dispOrDriftLineColor, string dispOrDriftMaxMinLineColor,
            string outputCase)
        {
            bool isError;

            //Get table for joint displacement. Put here because will be filtered in next step
            src.HelperEtabs.DatabaseTable jointDisplacementsTable = new HelperEtabs.DatabaseTable();
            (isError, jointDisplacementsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Displacements");
            if (isError) { return (isError); }

            //Filter joint displacement to only include selected load case, to improve performance
            (isError, jointDisplacementsTable) = src.HelperEtabs.FilterDatabaseTable(jointDisplacementsTable, "OutputCase", outputCase);
            if (isError) { return (isError); }

            //Check if case have multiple result for example seismic
            bool caseHaveMultipleResult = CheckCaseHaveMultipleResult(jointListFilteredByGroup, jointDisplacementsTable, outputCase);

            double[] zCoordinateList;
            double[] displacementList;
            double[] driftList;

            if (caseHaveMultipleResult)
            {
                if (showMaxDisp)
                {
                    //Get unique Z from the filtered joints
                    List<double> uniqueZCoord = GetUniqueZFromPointList(jointListFilteredByGroup);
                    List<HelperEtabs.Point3D> jointListFilteredZ = HelperEtabs.FilterPointBasedOnZCoord(jointList, uniqueZCoord);


                    List<ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailEveryZList = new List<ClassDriftPlot.JointDisplacementDetail>();
                    List<ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailEveryZList = new List<ClassDriftPlot.JointDisplacementDetail>();

                    (jointMaxDisplacementDetailEveryZList, jointMinDisplacementDetailEveryZList) = GetMaxDisplacement(jointListFilteredZ, uniqueZCoord, jointDisplacementsTable, displacementDirection);

                    //Plot max
                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMaxDisplacementDetailEveryZList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftMaxMinLineColor), 1, LineStyle.DashDot, "Displacement Max/Min (mm)");

                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMinDisplacementDetailEveryZList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftMaxMinLineColor), 1, LineStyle.DashDot, null);

                }

                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailList = new List<JointDisplacementDetail>();
                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailList = new List<JointDisplacementDetail>();

                (jointMinDisplacementDetailList, jointMaxDisplacementDetailList) = GetJointDisplacementMultiResult(jointListFilteredByGroup, jointDisplacementsTable, outputCase, displacementDirection);

                //Plot displacement min
                (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMinDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, "Displacement (mm)");
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, "Inter-story drift");

                //Plot displacement max
                (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMaxDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, null);
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid, null);

            }
            else
            {
                //Show maximum displacement on each Z
                if (showMaxDisp)
                {
                    //Get unique Z from the filtered joints
                    List<double> uniqueZCoord = GetUniqueZFromPointList(jointListFilteredByGroup);
                    List<HelperEtabs.Point3D> jointListFilteredZ = HelperEtabs.FilterPointBasedOnZCoord(jointList, uniqueZCoord);


                    List<ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailEveryZList = new List<ClassDriftPlot.JointDisplacementDetail>();
                    List<ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailEveryZList = new List<ClassDriftPlot.JointDisplacementDetail>();

                    (jointMaxDisplacementDetailEveryZList, jointMinDisplacementDetailEveryZList) = GetMaxDisplacement(jointListFilteredZ, uniqueZCoord, jointDisplacementsTable, displacementDirection);

                    //Plot max
                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMaxDisplacementDetailEveryZList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftMaxMinLineColor), 1, LineStyle.DashDot, "Displacement Max/Min (mm)");

                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMinDisplacementDetailEveryZList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftMaxMinLineColor), 1, LineStyle.DashDot, null);

                }

                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = GetJointDisplacement(jointListFilteredByGroup, jointDisplacementsTable, outputCase, displacementDirection);
                
                //Plot displacement
                (zCoordinateList, displacementList, driftList) = ConvertToArray(jointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid);
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml(dispOrDriftLineColor), 2, LineStyle.Solid);

            }

            return false;
        }


        public static bool CheckCaseHaveMultipleResult(List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointDisplacementsTable, string outputCase)
        {
            if (jointList.Count > 0)
            {
                //take first joint only
                src.HelperEtabs.Point3D joint = jointList[0];

                List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

                bool foundRowMatch = false;
                foreach (Dictionary<string, string> row in tableInList)
                {
                    if (row["UniqueName"] == joint.UniqueName)
                    {
                        //If foundRowMatch already true, meaning there are multiple results for each output case
                        if (foundRowMatch)
                        {
                            return true;
                        }
                        foundRowMatch = true;
                    }
                }
                return false;
            }
            return false;
        }

        public static List<ClassDriftPlot.JointDisplacementDetail> GetJointDisplacement(List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointDisplacementsTable,
            string outputCase, string displacementDirection)
        {

            List<ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();
            List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

            foreach (HelperEtabs.Point3D joint in jointList)
            {

                Dictionary<string, string> rowMatch = new Dictionary<string, string>();
                foreach (Dictionary<string, string> row in tableInList)
                {
                    if (row["UniqueName"] == joint.UniqueName)
                    {
                        double displacement;

                        if (displacementDirection == "Ux + Uy (SRSS)")
                        {
                            displacement = Math.Pow(Math.Pow(Convert.ToDouble(row["Ux"]), 2) + Math.Pow(Convert.ToDouble(row["Uy"]), 2), 0.5) * 1000; //m to mm
                        }
                        else
                        {
                            displacement = Math.Abs(Convert.ToDouble(row[displacementDirection])) * 1000; //m to mm
                        }

                        ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(joint.UniqueName, joint.ZCoordinate, displacement);
                        jointDisplacementDetailList.Add(jointDisplacementDetail);

                        rowMatch = row;
                        break;
                    }
                }

                //tableInList.Remove(rowMatch);
            }

            //Get the drift
            //Sort from lowest Z
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointDisplacementDetailList = jointDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            int index = 0;
            foreach (ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail in sortedJointDisplacementDetailList)
            {
                if (index == 0)
                {
                    jointDisplacementDetail.Drift = 0;
                }
                else
                {
                    jointDisplacementDetail.Drift = ((jointDisplacementDetail.Displacement - sortedJointDisplacementDetailList[index - 1].Displacement) / 1000) / (jointDisplacementDetail.ZCoordinate - sortedJointDisplacementDetailList[index - 1].ZCoordinate);
                }
                index++;
            }

            return sortedJointDisplacementDetailList;
        }

        public static (List<ClassDriftPlot.JointDisplacementDetail>, List<ClassDriftPlot.JointDisplacementDetail>) GetJointDisplacementMultiResult(List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointDisplacementsTable,
    string outputCase, string displacementDirection)
        {

            List<ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();
            List<ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();

            List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

            double maxDisp = 0;
            double minDisp = 0;
            bool firstMatch = true;

            foreach (HelperEtabs.Point3D joint in jointList)
            {

                Dictionary<string, string> rowMatch = new Dictionary<string, string>();
                foreach (Dictionary<string, string> row in tableInList)
                {
                    if (row["UniqueName"] == joint.UniqueName)
                    {
                        double displacement = Convert.ToDouble(row[displacementDirection]) * 1000; //m to mm

                        if (firstMatch)
                        {
                            maxDisp = displacement;
                            minDisp = displacement;
                            firstMatch = false;
                        }
                        else
                        {
                            if (displacement > maxDisp) { maxDisp = displacement;}
                            if (displacement < minDisp) { minDisp = displacement;}
                        }
                    }
                }
                ClassDriftPlot.JointDisplacementDetail jointMinDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(joint.UniqueName, joint.ZCoordinate, minDisp);
                jointMinDisplacementDetailList.Add(jointMinDisplacementDetail);
                ClassDriftPlot.JointDisplacementDetail jointMaxDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(joint.UniqueName, joint.ZCoordinate, maxDisp);
                jointMaxDisplacementDetailList.Add(jointMaxDisplacementDetail);


            }

            //Get the drift
            //Sort from lowest Z
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointMinDisplacementDetailList = jointMinDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            int index = 0;
            foreach (ClassDriftPlot.JointDisplacementDetail jointMinDisplacementDetail in sortedJointMinDisplacementDetailList)
            {
                if (index == 0)
                {
                    jointMinDisplacementDetail.Drift = 0;
                }
                else
                {
                    jointMinDisplacementDetail.Drift = ((jointMinDisplacementDetail.Displacement - sortedJointMinDisplacementDetailList[index - 1].Displacement) / 1000) / (jointMinDisplacementDetail.ZCoordinate - sortedJointMinDisplacementDetailList[index - 1].ZCoordinate);
                }
                index++;
            }

            List<ClassDriftPlot.JointDisplacementDetail> sortedJointMaxDisplacementDetailList = jointMaxDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            index = 0;
            foreach (ClassDriftPlot.JointDisplacementDetail jointMaxDisplacementDetail in sortedJointMaxDisplacementDetailList)
            {
                if (index == 0)
                {
                    jointMaxDisplacementDetail.Drift = 0;
                }
                else
                {
                    jointMaxDisplacementDetail.Drift = ((jointMaxDisplacementDetail.Displacement - sortedJointMaxDisplacementDetailList[index - 1].Displacement) / 1000) / (jointMaxDisplacementDetail.ZCoordinate - sortedJointMaxDisplacementDetailList[index - 1].ZCoordinate);
                }
                index++;
            }

            return (sortedJointMinDisplacementDetailList, sortedJointMaxDisplacementDetailList);
        }

        public static List<ClassDriftPlot.JointDisplacementDetail> GetJointDisplacementLimit(List<src.HelperEtabs.Point3D> jointList, string maxDisplacementFactor)
        {

            List<ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();

            //Get minimum Z
            double minimumZ = 0;
            double maximumZ = 0;
            bool firstZ = true;
            foreach (HelperEtabs.Point3D joint in jointList)
            {
                if (firstZ)
                {
                    minimumZ = joint.ZCoordinate;
                    maximumZ = joint.ZCoordinate;
                    firstZ = false;
                }
                else
                {
                    if (minimumZ > joint.ZCoordinate)
                    {
                        minimumZ = joint.ZCoordinate;
                    }
                    if (maximumZ < joint.ZCoordinate)
                    {
                        maximumZ = joint.ZCoordinate;
                    }
                }
            }

            var nominator = maxDisplacementFactor.Split('/')[0];
            var denominator = maxDisplacementFactor.Split('/')[1];

            double displacementLimit = (maximumZ - minimumZ) * Convert.ToDouble(nominator) / Convert.ToDouble(denominator) * 1000; //m to mm

            ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail("displimit", minimumZ, 0);
            jointDisplacementDetail.Drift = Convert.ToDouble(nominator) / Convert.ToDouble(denominator);
            jointDisplacementDetailList.Add(jointDisplacementDetail);

            jointDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail("displimit", maximumZ, displacementLimit);
            jointDisplacementDetail.Drift = Convert.ToDouble(nominator) / Convert.ToDouble(denominator);
            jointDisplacementDetailList.Add(jointDisplacementDetail);

            //Sort from lowest Z
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointDisplacementDetailList = jointDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            return sortedJointDisplacementDetailList;
        }

        public static (List<ClassDriftPlot.JointDisplacementDetail>, List<ClassDriftPlot.JointDisplacementDetail>) GetMaxDisplacement(List<HelperEtabs.Point3D> jointListFilteredZ, List<double> uniqueZCoord, 
            src.HelperEtabs.DatabaseTable jointDisplacementsTable, string displacementDirection)
        {

            List<ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();
            List<ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();

            bool isError = false;

            foreach (double eachUniqueZCoord in uniqueZCoord)
            {

                double maxDisp = 0;
                double minDisp = 0;
                string maxDispUniqueName = "";
                string minDispUniqueName = "";
                bool firstMatch = true;

                List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

                List<HelperEtabs.Point3D> matchedJointFilteredZList = new List<HelperEtabs.Point3D>();
                foreach (HelperEtabs.Point3D jointFilteredZ in jointListFilteredZ)
                {
                    if (jointFilteredZ.ZCoordinate == eachUniqueZCoord)
                    {
                        Dictionary<string, string> rowMatch = new Dictionary<string, string>();
                        foreach (Dictionary<string, string> row in tableInList)
                        {
                            if (row["UniqueName"] == jointFilteredZ.UniqueName)
                            {
                                double displacement;

                                if (displacementDirection == "Ux + Uy (SRSS)")
                                {
                                    displacement = Math.Pow(Math.Pow(Convert.ToDouble(row["Ux"]), 2) + Math.Pow(Convert.ToDouble(row["Uy"]), 2), 0.5) * 1000; //m to mm
                                }
                                else
                                {
                                    displacement = Math.Abs(Convert.ToDouble(row[displacementDirection])) * 1000; //m to mm
                                }

                                if (firstMatch)
                                {
                                    maxDisp = displacement;
                                    maxDispUniqueName = row["UniqueName"];
                                    minDisp = displacement;
                                    minDispUniqueName = row["UniqueName"];
                                    firstMatch = false;
                                }
                                else
                                {
                                    if (displacement > maxDisp) { maxDisp = displacement; maxDispUniqueName = row["UniqueName"]; }
                                    if (displacement < minDisp) { minDisp = displacement; minDispUniqueName = row["UniqueName"]; }
                                }

                                rowMatch = row;
                                matchedJointFilteredZList.Add(jointFilteredZ);
                                break;
                            }
                        }

                        //Remove matched row to save time
                        //tableInList.Remove(rowMatch);
                    }
                }

                //Removed matched list to save time
                foreach (HelperEtabs.Point3D matchedJointFilteredZ in matchedJointFilteredZList)
                {
                    jointListFilteredZ.Remove(matchedJointFilteredZ);
                }
                
                ClassDriftPlot.JointDisplacementDetail jointMaxDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(maxDispUniqueName, eachUniqueZCoord, maxDisp);
                ClassDriftPlot.JointDisplacementDetail jointMinDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(minDispUniqueName, eachUniqueZCoord, minDisp);
                jointMaxDisplacementDetailList.Add(jointMaxDisplacementDetail);
                jointMinDisplacementDetailList.Add(jointMinDisplacementDetail);
            }

            //Sort from lowest Z
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointMaxDisplacementDetailList = jointMaxDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointMinDisplacementDetailList = jointMinDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            return (sortedJointMaxDisplacementDetailList, sortedJointMinDisplacementDetailList);
        }

        public static (double[], double[], double[]) ConvertToArray(List<ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList)
        {
            List<double> zCoordinateList = new List<double>();
            List<double> displacementList = new List<double>();
            List<double> driftList = new List<double>();

            foreach (ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail in jointDisplacementDetailList)
            {
                zCoordinateList.Add(jointDisplacementDetail.ZCoordinate);
                displacementList.Add(jointDisplacementDetail.Displacement);
                driftList.Add(jointDisplacementDetail.Drift);
            }

            return (zCoordinateList.ToArray(), displacementList.ToArray(), driftList.ToArray());
        }

        public static List<double> GetUniqueZFromPointList(List<HelperEtabs.Point3D> jointList)
        {
            List<double> uniqueZCoordList = new List<double>();

            foreach (HelperEtabs.Point3D joint in jointList)
            {
                if (!uniqueZCoordList.Contains(joint.ZCoordinate))
                {
                    uniqueZCoordList.Add(joint.ZCoordinate);
                }
            }

            return uniqueZCoordList;
        }

    }
}
