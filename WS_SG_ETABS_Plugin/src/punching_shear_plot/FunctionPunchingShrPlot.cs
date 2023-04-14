using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ETABSv1;
using ScottPlot.Renderable;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    internal class FunctionPunchingShrPlot
    {
        public static bool PunchingShearPlot(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, string plotTitle, 
            double bubbleScaleFactor, double textSize, double titleSize, double axisTitleSize, double gridSpacingX, double gridSpacingY, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            bool isError = false;
            List<string> errorList = new List<string>();

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable jointReactionTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Reactions");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable elementForcesColumnsTable = new HelperEtabs.DatabaseTable();
            (isError, elementForcesColumnsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Forces - Columns");
            if (isError) { return isError; }


            src.HelperEtabs.DatabaseTable columnObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, columnObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Column Object Connectivity");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable frameAssignmentSectPropTable = new HelperEtabs.DatabaseTable();
            (isError, frameAssignmentSectPropTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Frame Assignments - Section Properties");
            if (isError) { return isError; }

            //Filter group to only include points, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Point");
            if (isError) { return isError; }

            //Container
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();
            List<ClassPunchingShrPlot.JointPunchingShearDetails> jointPunchingShearDetailsList = new List<ClassPunchingShrPlot.JointPunchingShearDetails>();

            //Get joint which is part of the choosen group
            jointList = GetJointListFromGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);
            GetColumnForceConnectedToJoint(jointPunchingShearDetailsList, jointList, elementForcesColumnsTable, columnObjectConnectivityTable, jointReactionTable, listChoosenLoadCase, errorList);

            return false;
        }

        public static List<src.HelperEtabs.Point3D> GetJointListFromGroup(src.HelperEtabs.DatabaseTable pointObjectConnectivityTable, src.HelperEtabs.DatabaseTable groupAssignmentsTable, string choosenGroup)
        {
            bool isError = false;
            src.HelperEtabs.DatabaseTable filteredJointReactionTable = new HelperEtabs.DatabaseTable();
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();

            //Get table from database
            (isError, filteredJointReactionTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);
            List<Dictionary<string, string>> table = filteredJointReactionTable.TableInList;

            foreach(Dictionary<string, string> row in table)
            {
                src.HelperEtabs.Point3D joint = new HelperEtabs.Point3D(row["UniqueName"], Convert.ToDouble(row["X"]), Convert.ToDouble(row["Y"]), Convert.ToDouble(row["Z"]));
                jointList.Add(joint);
            }

            return jointList;
        }

        public static void GetColumnForceConnectedToJoint(List<ClassPunchingShrPlot.JointPunchingShearDetails> jointPunchingShearDetailsList, List<src.HelperEtabs.Point3D> jointList, HelperEtabs.DatabaseTable elementForcesColumnsTable, 
            HelperEtabs.DatabaseTable columnObjectConnectivityTable, HelperEtabs.DatabaseTable jointReactionTable, List<string> listChoosenLoadCase, List<string> errorList)
        {
            bool isError = false;

            foreach (HelperEtabs.Point3D joint in jointList)
            {
                ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails = new ClassPunchingShrPlot.JointPunchingShearDetails(joint.UniqueName);

                string errorMessage = "";

                List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

                HelperEtabs.DatabaseTable filteredColumnObjectConnectivityJointI = new HelperEtabs.DatabaseTable();
                (isError, filteredColumnObjectConnectivityJointI) = HelperEtabs.FilterDatabaseTable(columnObjectConnectivityTable, "UniquePtI", joint.UniqueName);

                tableInList = filteredColumnObjectConnectivityJointI.TableInList;
                //If find more than 1 column with joint I connected
                if (tableInList.Count > 1)
                {
                    MessageBox.Show($"Error! More than 1 column joint I connected to joint {joint.UniqueName}!");
                    isError = true;
                }
                else if (tableInList.Count == 0)
                {
                    jointPunchingShearDetails.NotFoundColumnI = true;
                }
                else
                {
                    jointPunchingShearDetails.ColumnIUniqueName = tableInList[0]["UniqueName"];
                }


                HelperEtabs.DatabaseTable filteredColumnObjectConnectivityJointJ = new HelperEtabs.DatabaseTable();
                (isError, filteredColumnObjectConnectivityJointJ) = HelperEtabs.FilterDatabaseTable(columnObjectConnectivityTable, "UniquePtJ", joint.UniqueName);

                tableInList = filteredColumnObjectConnectivityJointJ.TableInList;
                //If find more than 1 column with joint I connected
                if (tableInList.Count > 1)
                {
                    MessageBox.Show($"Error! More than 1 column joint J connected to joint {joint.UniqueName}!");
                    isError = true;
                }
                else if (tableInList.Count == 0)
                {
                    jointPunchingShearDetails.NotFoundColumnJ = true;
                }
                else
                {
                    jointPunchingShearDetails.ColumnJUniqueName = tableInList[0]["UniqueName"];
                }

                //Return if error
                if (isError) { return; }
                
                //Go through each load case
                foreach (string loadCase in listChoosenLoadCase)
                {
                    double minStation = 0;
                    double maxStation = 0;
                    double maxForceDifferenceInOneLoadCase = 0;

                    //Skip joint if not connected to any column joint I or joint J
                    if (jointPunchingShearDetails.NotFoundColumnI && jointPunchingShearDetails.NotFoundColumnJ)
                    {
                        errorMessage = $"Warning! Joint {joint.UniqueName} not connected to any column!";
                        errorList.Add( errorMessage );
                    }
                    else
                    {
                        List<double> forceFromIListInOneLoadCase = new List<double>();
                        List<double> forceFromJListInOneLoadCase = new List<double>();

                        //Column I
                        if (!jointPunchingShearDetails.NotFoundColumnI)
                        {
                            //If can find column, look for column force

                            HelperEtabs.DatabaseTable filteredElementForcesColumnsJointI = new HelperEtabs.DatabaseTable();
                            (isError, filteredElementForcesColumnsJointI) = HelperEtabs.FilterDatabaseTable(elementForcesColumnsTable, "UniqueName", jointPunchingShearDetails.ColumnIUniqueName);
                            (isError, filteredElementForcesColumnsJointI) = HelperEtabs.FilterDatabaseTable(filteredElementForcesColumnsJointI, "OutputCase", loadCase);

                            (isError, minStation) = HelperEtabs.FindMinValueInDatabaseTable(filteredElementForcesColumnsJointI, "Station");

                            (isError, filteredElementForcesColumnsJointI) = HelperEtabs.FilterDatabaseTable(filteredElementForcesColumnsJointI, "Station", Convert.ToString(minStation));

                            foreach(Dictionary<string, string> row in filteredElementForcesColumnsJointI.TableInList)
                            {
                                forceFromIListInOneLoadCase.Add(Convert.ToDouble(row["P"]));
                            }
                        }
                        else
                        {
                            //Else find joint reaction
                            HelperEtabs.DatabaseTable filteredJointReactionTableI = new HelperEtabs.DatabaseTable();
                            (isError, filteredJointReactionTableI) = HelperEtabs.FilterDatabaseTable(jointReactionTable, "UniqueName", joint.UniqueName);

                            if (filteredJointReactionTableI.TableInList.Count > 0)
                            {
                                (isError, filteredJointReactionTableI) = HelperEtabs.FilterDatabaseTable(filteredJointReactionTableI, "OutputCase", loadCase);

                                if (filteredJointReactionTableI.TableInList.Count > 0)
                                {
                                    foreach (Dictionary<string, string> row in filteredJointReactionTableI.TableInList)
                                    {
                                        forceFromIListInOneLoadCase.Add(Convert.ToDouble(row["FZ"]) * -1);
                                    }
                                }
                                //If cant find any reaction, then there is no column
                                else
                                {
                                    forceFromIListInOneLoadCase.Add(0);
                                }
                            }
                            else
                            {
                                forceFromIListInOneLoadCase.Add(0);
                            }

                        }

                        //Column J
                        if (!jointPunchingShearDetails.NotFoundColumnJ)
                        {
                            HelperEtabs.DatabaseTable filteredElementForcesColumnsJointJ = new HelperEtabs.DatabaseTable();
                            (isError, filteredElementForcesColumnsJointJ) = HelperEtabs.FilterDatabaseTable(elementForcesColumnsTable, "UniqueName", jointPunchingShearDetails.ColumnJUniqueName);
                            (isError, filteredElementForcesColumnsJointJ) = HelperEtabs.FilterDatabaseTable(filteredElementForcesColumnsJointJ, "OutputCase", loadCase);

                            (isError, maxStation) = HelperEtabs.FindMaxValueInDatabaseTable(filteredElementForcesColumnsJointJ, "Station");

                            (isError, filteredElementForcesColumnsJointJ) = HelperEtabs.FilterDatabaseTable(filteredElementForcesColumnsJointJ, "Station", Convert.ToString(maxStation));


                            foreach (Dictionary<string, string> row in filteredElementForcesColumnsJointJ.TableInList)
                            {
                                forceFromJListInOneLoadCase.Add(Convert.ToDouble(row["P"]));
                            }
                        }
                        else
                        {
                            //Else find joint reaction
                            HelperEtabs.DatabaseTable filteredJointReactionTableJ = new HelperEtabs.DatabaseTable();
                            (isError, filteredJointReactionTableJ) = HelperEtabs.FilterDatabaseTable(jointReactionTable, "UniqueName", joint.UniqueName);

                            if (filteredJointReactionTableJ.TableInList.Count > 0)
                            {
                                (isError, filteredJointReactionTableJ) = HelperEtabs.FilterDatabaseTable(filteredJointReactionTableJ, "OutputCase", loadCase);

                                if (filteredJointReactionTableJ.TableInList.Count > 0)
                                {
                                    foreach (Dictionary<string, string> row in filteredJointReactionTableJ.TableInList)
                                    {
                                        forceFromJListInOneLoadCase.Add(Convert.ToDouble(row["FZ"]) * -1);
                                    }
                                }
                                //If cant find any reaction, then there is no column
                                else
                                {
                                    forceFromJListInOneLoadCase.Add(0);
                                }
                            }
                            else
                            {
                                forceFromJListInOneLoadCase.Add(0);
                            }

                        }

                        //Looking for max difference in axial load in one load case
                        bool firstLoopInOneLoadCase = true;
                        foreach (double forceFromI in forceFromIListInOneLoadCase)
                        {
                            foreach(double forceFromJ in forceFromJListInOneLoadCase) 
                            { 
                                if (firstLoopInOneLoadCase)
                                {
                                    maxForceDifferenceInOneLoadCase = forceFromI - forceFromJ;
                                    firstLoopInOneLoadCase = false;
                                }
                                else
                                {
                                    double forceDifference = forceFromI - forceFromJ;

                                    if (Math.Abs(forceDifference) > Math.Abs(maxForceDifferenceInOneLoadCase))
                                    {
                                        maxForceDifferenceInOneLoadCase = forceDifference;
                                    }
                                }
                            }
                        }
                    }

                    jointPunchingShearDetails.ForceList.Add(loadCase, Math.Abs(maxForceDifferenceInOneLoadCase));
                }

                //Looping through all load case and return the governing
                bool firstLoopInOneJoint = true;
                double maxForceDifferenceInOneJoint = 0;
                string maxLoadCaseInOneJoint = "";
                foreach (KeyValuePair<string, double> force in jointPunchingShearDetails.ForceList)
                {
                    if (firstLoopInOneJoint)
                    {
                        maxForceDifferenceInOneJoint = force.Value;
                        maxLoadCaseInOneJoint = force.Key;
                        firstLoopInOneJoint = false;
                    }
                    else
                    {
                        if (Math.Abs(force.Value) > Math.Abs(maxForceDifferenceInOneJoint))
                        {
                            maxForceDifferenceInOneJoint = force.Value;
                            maxLoadCaseInOneJoint = force.Key;
                        }
                    }
                }

                jointPunchingShearDetails.ForceMaxDifference = maxForceDifferenceInOneJoint;
                jointPunchingShearDetails.LoadCaseMaxDifference = maxLoadCaseInOneJoint;

                jointPunchingShearDetailsList.Add(jointPunchingShearDetails);

            }



        }
    }
}
