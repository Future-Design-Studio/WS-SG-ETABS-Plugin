using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using ETABSv1;
using Microsoft.Office.Interop.Excel;
using ScottPlot;
using WS_SG_ETABS_Plugin.src.pad_footing_plot;
using static WS_SG_ETABS_Plugin.src.plot_drift.ClassDriftPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    internal class FunctionDriftPlot
    {
        public static bool PlotDrift(ETABSv1.cSapModel SapModel, ETABSv1.cPluginCallback Plugin, string outputCase, string displacementDirection, string maxDisplacementFactor, string choosenGroup,
            int legendTextSize, int titleSize, int axisTitleSize, int tickLabelSize, double gridSpacingDisplacementX, double gridSpacingDriftX, double gridSpacingY, bool showMaxDisp,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath, WpfPlot DriftPlot, WpfPlot DisplacementPlot)
        {
            bool isError = false;
            List<string> errorList = new List<string>();
            List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitions = new List<ClassPadFootPlot.GroupDefinition>();

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError); }

            src.HelperEtabs.DatabaseTable jointDisplacementsTable = new HelperEtabs.DatabaseTable();
            (isError, jointDisplacementsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Displacements");
            if (isError) { return (isError); }

            //Filter joint displacement to only include selected load case, to improve performance
            (isError, jointDisplacementsTable) = src.HelperEtabs.FilterDatabaseTable(jointDisplacementsTable, "OutputCase", outputCase);
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

            //Check if case have multiple result for example seismic
            bool caseHaveMultipleResult = CheckCaseHaveMultipleResult(jointListFilteredByGroup, jointDisplacementsTable, outputCase);

            if (caseHaveMultipleResult)
            {
                //not implemented yet
            }
            else
            {
                double[] zCoordinateList;
                double[] displacementList;
                double[] driftList;

                if (showMaxDisp)
                {
                    //Get unique Z from the filtered joints
                    List<double> uniqueZCoord = GetUniqueZFromPointList(jointListFilteredByGroup);
                    List<HelperEtabs.Point3D> jointListFilteredZ = HelperEtabs.FilterPointBasedOnZCoord(jointList, uniqueZCoord);


                    List<ClassDriftPlot.JointDisplacementDetail> jointMaxDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();
                    List<ClassDriftPlot.JointDisplacementDetail> jointMinDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();

                    (jointMaxDisplacementDetailList, jointMinDisplacementDetailList) = GetMaxDisplacement(jointListFilteredZ, uniqueZCoord, jointDisplacementsTable, displacementDirection);

                    //Plot max
                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMaxDisplacementDetailList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#808080"), 1, LineStyle.DashDot, "Displacement Max (mm)");

                    (zCoordinateList, displacementList, driftList) = ConvertToArray(jointMinDisplacementDetailList);
                    DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#808080"), 1, LineStyle.DashDot, "Displacement Min (mm)");
                    
                }

                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = GetJointDisplacement(jointListFilteredByGroup, jointDisplacementsTable, outputCase, displacementDirection);
                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> limitJointDisplacementDetailList = GetJointDisplacementLimit(jointListFilteredByGroup, maxDisplacementFactor);

                //Plot displacement
                (zCoordinateList, displacementList, driftList) = ConvertToArray(jointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#ffce00"), 2, LineStyle.Solid, "Displacement (mm)");
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#ffce00"), 2, LineStyle.Solid, "Inter-story drift");

                //Plot limit
                var denominator = maxDisplacementFactor.Split('/')[1];
                (zCoordinateList, displacementList, driftList) = ConvertToArray(limitJointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#FF0000"), 2, LineStyle.Dash, $"Limit H/{denominator}");
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList, System.Drawing.ColorTranslator.FromHtml("#FF0000"), 2, LineStyle.Dash, $"Limit h/{denominator}");

            }


            DisplacementPlot.Plot.Title($"Lateral Deflection (Case: {outputCase}, Direction: {displacementDirection})", true, null, titleSize, "Arial Narrow");
            DisplacementPlot.Plot.YAxis.Label("Height (m)", null, axisTitleSize, true, "Arial Narrow");
            DisplacementPlot.Plot.XAxis.Label("Displacement (mm)", null, axisTitleSize, true, "Arial Narrow");
            DisplacementPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            DisplacementPlot.Plot.XAxis.ManualTickSpacing(gridSpacingDisplacementX);
            DisplacementPlot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            DisplacementPlot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            var displacementLegend = DisplacementPlot.Plot.Legend();
            displacementLegend.FontSize = legendTextSize;

            DriftPlot.Plot.Title($"Inter-story Drift (Case: {outputCase}, Direction: {displacementDirection})", true, null, titleSize, "Arial Narrow");
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

            return true;
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
                        double displacement = Convert.ToDouble(row[displacementDirection]) * 1000; //m to mm

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
                                double displacement = Convert.ToDouble(row[displacementDirection]) * 1000;

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
