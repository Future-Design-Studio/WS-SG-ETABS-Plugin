using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using ETABSv1;
using ScottPlot;
using WS_SG_ETABS_Plugin.src.pad_footing_plot;
using static WS_SG_ETABS_Plugin.src.plot_drift.ClassDriftPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    internal class FunctionDriftPlot
    {
        public static bool PlotDrift(cSapModel SapModel, cPluginCallback Plugin, string outputCase, string displacementDirection, string maxDisplacementFactor, string choosenGroup, string plotTitle,
            int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY,
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

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError); }

            //Filter group to only include points, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Point");
            if (isError) { return (isError); }

            //Container
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();
            List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList = new List<ClassPadFootPlot.JointPadFootDetails>();

            //Get joint which is part of the choosen group
            jointList = src.HelperEtabs.GetJointListFromGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);

            //Check if case have multiple result for example seismic
            bool caseHaveMultipleResult = CheckCaseHaveMultipleResult(jointList, jointDisplacementsTable, outputCase);

            if (caseHaveMultipleResult)
            {
                //not implemented yet
            }
            else
            {
                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = GetJointDisplacement(jointList, jointDisplacementsTable, outputCase, displacementDirection);
                List<src.plot_drift.ClassDriftPlot.JointDisplacementDetail> maxJointDisplacementDetailList = GetJointDisplacementLimit(jointList, maxDisplacementFactor);

                //Plot displacement
                (double[] zCoordinateList, double[] displacementList, double[] driftList) = ConvertToArray(jointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList);
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList);

                //Plot limit
                (zCoordinateList, displacementList, driftList) = ConvertToArray(maxJointDisplacementDetailList);
                DisplacementPlot.Plot.AddScatterLines(displacementList, zCoordinateList);
                DriftPlot.Plot.AddScatterLines(driftList, zCoordinateList);

            }

            DisplacementPlot.Refresh();
            DriftPlot.Refresh();
            

            return true;
        }

        public static bool CheckCaseHaveMultipleResult(List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointDisplacementsTable, string outputCase)
        {
            foreach (src.HelperEtabs.Point3D joint in jointList)
            {
                List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

                bool foundRowMatch = false;
                foreach(Dictionary<string, string> row in tableInList)
                {
                    if (row["UniqueName"] == joint.UniqueName && row["OutputCase"] == outputCase)
                    {
                        //If foundRowMatch already true, meaning there are multiple results for each output case
                        if (foundRowMatch)
                        {
                            return true;
                        }
                        foundRowMatch = true;
                    }
                }
            }
            return false;
        }

        public static List<ClassDriftPlot.JointDisplacementDetail> GetJointDisplacement(List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointDisplacementsTable, 
            string outputCase, string reactionDirection)
        {

            List<ClassDriftPlot.JointDisplacementDetail> jointDisplacementDetailList = new List<ClassDriftPlot.JointDisplacementDetail>();

            foreach (HelperEtabs.Point3D joint in jointList)
            {

                List<Dictionary<string, string>> tableInList = jointDisplacementsTable.TableInList;

                foreach (Dictionary<string, string> row in tableInList)
                {
                    if (row["UniqueName"] == joint.UniqueName && row["OutputCase"] == outputCase)
                    {
                        double displacement = Convert.ToDouble(row[reactionDirection]) * 1000; //m to mm

                        ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(joint.UniqueName, joint.ZCoordinate, displacement);
                        jointDisplacementDetailList.Add(jointDisplacementDetail);
                        break;
                    }
                }
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
                    jointDisplacementDetail.Drift = (jointDisplacementDetail.Displacement - sortedJointDisplacementDetailList[index - 1].Displacement) / (jointDisplacementDetail.ZCoordinate - sortedJointDisplacementDetailList[index - 1].ZCoordinate);
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
            bool firstZ = true;
            foreach (HelperEtabs.Point3D joint in jointList)
            {
                if (firstZ)
                {
                    minimumZ = joint.ZCoordinate;
                }
                else
                {
                    if (minimumZ > joint.ZCoordinate)
                    {
                        minimumZ = joint.ZCoordinate;
                    }
                }
            }

            foreach (HelperEtabs.Point3D joint in jointList)
            {
                int numerator = Convert.ToInt32(maxDisplacementFactor[0]);
                int denominator = Convert.ToInt32(maxDisplacementFactor[2]);

                double displacementLimit = (joint.ZCoordinate - minimumZ) * (numerator/ denominator) * 1000; //m to mm

                ClassDriftPlot.JointDisplacementDetail jointDisplacementDetail = new ClassDriftPlot.JointDisplacementDetail(joint.UniqueName, joint.ZCoordinate, displacementLimit);
                jointDisplacementDetail.Drift = Convert.ToDouble(maxDisplacementFactor);
                jointDisplacementDetailList.Add(jointDisplacementDetail);
            }

            //Get the drift
            //Sort from lowest Z
            List<ClassDriftPlot.JointDisplacementDetail> sortedJointDisplacementDetailList = jointDisplacementDetailList.OrderBy(o => o.ZCoordinate).ToList();

            return sortedJointDisplacementDetailList;
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

            return (zCoordinateList.ToArray(), displacementList.ToArray(), driftList.ToArray()) ;
        }

    }
}
