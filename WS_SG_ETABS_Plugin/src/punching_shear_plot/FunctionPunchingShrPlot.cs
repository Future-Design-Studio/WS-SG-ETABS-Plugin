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
using static WS_SG_ETABS_Plugin.src.plot_reaction.table.FunctionPlotReactionTable;
using static WS_SG_ETABS_Plugin.src.punching_shear_plot.ClassPunchingShrPlot;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    internal class FunctionPunchingShrPlot
    {
        public static bool PunchingShearPlot(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, double forceIncrement, string plotTitle, 
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

            src.HelperEtabs.DatabaseTable frameDefSummaryTable = new HelperEtabs.DatabaseTable();
            (isError, frameDefSummaryTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Frame Section Property Definitions - Summary");
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
            GetColumnFrameSection(jointPunchingShearDetailsList, frameAssignmentSectPropTable, frameDefSummaryTable);
            List<ClassPunchingShrPlot.GroupDefinition> groupDefinitions = GroupBasedOnForceAndSectProp(jointPunchingShearDetailsList, forceIncrement);

            //Create excel
            var result = CompilePunchingShearDetailsToArray(jointPunchingShearDetailsList, choosenGroup);
            var resultGroupDef = CompileGroupDefinitionsToArray(groupDefinitions);
            WritePunchingShearDetailsToExcel(result, resultGroupDef, "punching_shear", "C:\\Users\\STEFANO\\OneDrive - Ramboll\\Desktop\\");

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
                ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails = new ClassPunchingShrPlot.JointPunchingShearDetails(joint.UniqueName, joint.XCoordinate, joint.YCoordinate, joint.ZCoordinate);

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

                jointPunchingShearDetails.ForceMaxDifference = Math.Ceiling(maxForceDifferenceInOneJoint);
                jointPunchingShearDetails.LoadCaseMaxDifference = maxLoadCaseInOneJoint;

                jointPunchingShearDetailsList.Add(jointPunchingShearDetails);

            }

        }

        public static void GetColumnFrameSection(List<ClassPunchingShrPlot.JointPunchingShearDetails> jointPunchingShearDetailsList, HelperEtabs.DatabaseTable frameAssignmentSectPropTable, HelperEtabs.DatabaseTable frameDefSummaryTable)
        {
            foreach(ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in jointPunchingShearDetailsList)
            {
                bool isError = false;

                bool notFoundColumnI = jointPunchingShearDetails.NotFoundColumnI;
                bool notFoundColumnJ = jointPunchingShearDetails.NotFoundColumnJ;

                string columnIUniqueName;
                string columnJUniqueName;
                string columnISectProp;
                double columnISectArea;
                string columnJSectProp;
                double columnJSectArea;

                string columnSectProp = "";


                if (!notFoundColumnI && !notFoundColumnJ)
                {
                    columnIUniqueName = jointPunchingShearDetails.ColumnIUniqueName;
                    columnJUniqueName = jointPunchingShearDetails.ColumnJUniqueName;

                    HelperEtabs.DatabaseTable filteredFrameAssignmentSectPropTable = new HelperEtabs.DatabaseTable();
                    HelperEtabs.DatabaseTable filteredFrameDefSummaryTable = new HelperEtabs.DatabaseTable();
                    (isError, filteredFrameAssignmentSectPropTable) = HelperEtabs.FilterDatabaseTable(frameAssignmentSectPropTable, "UniqueName", columnIUniqueName);
                    columnISectProp = filteredFrameAssignmentSectPropTable.TableInList[0]["SectProp"];
                    (isError, filteredFrameDefSummaryTable) = HelperEtabs.FilterDatabaseTable(frameDefSummaryTable, "Name", columnISectProp);
                    columnISectArea = Convert.ToDouble(filteredFrameDefSummaryTable.TableInList[0]["Area"]);

                    (isError, filteredFrameAssignmentSectPropTable) = HelperEtabs.FilterDatabaseTable(frameAssignmentSectPropTable, "UniqueName", columnJUniqueName);
                    columnJSectProp = filteredFrameAssignmentSectPropTable.TableInList[0]["SectProp"];
                    (isError, filteredFrameDefSummaryTable) = HelperEtabs.FilterDatabaseTable(frameDefSummaryTable, "Name", columnJSectProp);
                    columnJSectArea = Convert.ToDouble(filteredFrameDefSummaryTable.TableInList[0]["Area"]);

                    //Take the bigger sect area to take the section property
                    if (columnISectArea > columnJSectArea)
                    {
                        columnSectProp = columnISectProp;
                    }
                    else
                    {
                        columnSectProp = columnJSectProp;
                    }


                }
                else if (!notFoundColumnI)
                {
                    columnIUniqueName = jointPunchingShearDetails.ColumnIUniqueName;

                    HelperEtabs.DatabaseTable filteredFrameAssignmentSectPropTable = new HelperEtabs.DatabaseTable();
                    (isError, filteredFrameAssignmentSectPropTable) = HelperEtabs.FilterDatabaseTable(frameAssignmentSectPropTable, "UniqueName", columnIUniqueName);
                    columnISectProp = filteredFrameAssignmentSectPropTable.TableInList[0]["SectProp"];

                    //Take the bigger sect area to take the section property
                    columnSectProp = columnISectProp;
                }
                else if (!notFoundColumnJ)
                {
                    columnJUniqueName = jointPunchingShearDetails.ColumnJUniqueName;

                    HelperEtabs.DatabaseTable filteredFrameAssignmentSectPropTable = new HelperEtabs.DatabaseTable();
                    (isError, filteredFrameAssignmentSectPropTable) = HelperEtabs.FilterDatabaseTable(frameAssignmentSectPropTable, "UniqueName", columnJUniqueName);
                    columnJSectProp = filteredFrameAssignmentSectPropTable.TableInList[0]["SectProp"];

                    columnSectProp = columnJSectProp;
                }

                jointPunchingShearDetails.SectProp = columnSectProp;
            }
        }

        public static object [,] CompilePunchingShearDetailsToArray(List<ClassPunchingShrPlot.JointPunchingShearDetails> jointPunchingShearDetails, string choosenGroup)
        {
            List<string> colTitles = new List<string>()
            {
                "Num","Joint Unique Name", "X (m)","Y (m)","Z (m)", "Group Name", "Punching Shear Force (kN)", "Governing Load Case", "Section Property", "Group"
            };

            int numOfRow = jointPunchingShearDetails.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1, numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetail in jointPunchingShearDetails)
            {
                result[i, 0] = i;
                result[i, 1] = jointPunchingShearDetail.UniqueName;
                result[i, 2] = jointPunchingShearDetail.XCoordinate;
                result[i, 3] = jointPunchingShearDetail.YCoordinate;
                result[i, 4] = jointPunchingShearDetail.ZCoordinate;
                result[i, 5] = choosenGroup;

                result[i, 6] = jointPunchingShearDetail.ForceMaxDifference;
                result[i, 7] = jointPunchingShearDetail.LoadCaseMaxDifference;

                result[i, 8] = jointPunchingShearDetail.SectProp;

                result[i, 9] = jointPunchingShearDetail.GroupIndex;

                i++;
            }

            return result;


        }

        public static object[,] CompileGroupDefinitionsToArray(List<ClassPunchingShrPlot.GroupDefinition> groupDefinitions)
        {
            List<string> colTitles = new List<string>()
            {
                "Group", "Section Property", "Min Force (kN)", "Max Force (kN)"
            };

            int numOfRow = groupDefinitions.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1, numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach (ClassPunchingShrPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                result[i, 0] = groupDefinition.GroupIndex;
                result[i, 1] = groupDefinition.SectProp;
                result[i, 2] = groupDefinition.MinForce;
                result[i, 3] = groupDefinition.MaxForce;

                i++;
            }

            return result;


        }

        public static List<ClassPunchingShrPlot.GroupDefinition> GroupBasedOnForceAndSectProp(List<ClassPunchingShrPlot.JointPunchingShearDetails> jointPunchingShearDetailsList, double forceIncrement)
        {
            //Sort punching shear details by force
            List<ClassPunchingShrPlot.JointPunchingShearDetails> sortedJointPunchingShearDetailsList = jointPunchingShearDetailsList.OrderBy(o=>o.ForceMaxDifference).ToList();

            //Container group definition list
            List<ClassPunchingShrPlot.GroupDefinition> groupDefinitions = new List<ClassPunchingShrPlot.GroupDefinition> ();

            //Get unique sect prop
            List<string> sectPropList = new List<string>();

            foreach(ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in sortedJointPunchingShearDetailsList)
            {
                //If have not yet added
                if (!sectPropList.Contains(jointPunchingShearDetails.SectProp))
                {
                    sectPropList.Add(jointPunchingShearDetails.SectProp);
                }
            }


            //Group by sect prop
            List<List<ClassPunchingShrPlot.JointPunchingShearDetails>> groupBySectProp = new List<List<ClassPunchingShrPlot.JointPunchingShearDetails>>();
            foreach (string sectProp in sectPropList)
            {
                List<ClassPunchingShrPlot.JointPunchingShearDetails> eachGroupBySectProp = new List<ClassPunchingShrPlot.JointPunchingShearDetails>();
                foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in sortedJointPunchingShearDetailsList)
                {
                    if (jointPunchingShearDetails.SectProp == sectProp)
                    {
                        eachGroupBySectProp.Add(jointPunchingShearDetails);
                    }
                }

                groupBySectProp.Add(eachGroupBySectProp);
            }


            //Group by force
            List<List<ClassPunchingShrPlot.JointPunchingShearDetails>> groupByForce = new List<List<ClassPunchingShrPlot.JointPunchingShearDetails>>();
            foreach (List<ClassPunchingShrPlot.JointPunchingShearDetails> eachGroupBySectProp in groupBySectProp)
            {
                foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in eachGroupBySectProp)
                {
                    double forceDividedByIncrement = Math.Floor(jointPunchingShearDetails.ForceMaxDifference / forceIncrement);
                    jointPunchingShearDetails.ForceDividedByIncrement = forceDividedByIncrement;
                }

                //Get unique force divided by increment
                List<double> forceDividedByIncrementList = new List<double>();
                foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in eachGroupBySectProp)
                {
                    //If have not yet added
                    if (!forceDividedByIncrementList.Contains(jointPunchingShearDetails.ForceDividedByIncrement))
                    {
                        forceDividedByIncrementList.Add(jointPunchingShearDetails.ForceDividedByIncrement);
                    }
                }

                //Group by force
                foreach (double forceDividedByIncrement in forceDividedByIncrementList)
                {
                    //Container
                    List<ClassPunchingShrPlot.JointPunchingShearDetails> eachGroupByForce = new List<ClassPunchingShrPlot.JointPunchingShearDetails>();

                    foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in eachGroupBySectProp)
                    {
                        if(jointPunchingShearDetails.ForceDividedByIncrement == forceDividedByIncrement)
                        {
                            eachGroupByForce.Add(jointPunchingShearDetails);
                        }
                    }
                    groupByForce.Add(eachGroupByForce);
                }
            }

            //Assign group index
            int groupIndex = 0;
            foreach(List<ClassPunchingShrPlot.JointPunchingShearDetails> eachGroupByForce in groupByForce)
            {
                bool firstLoop = true;
                string sectProp = "";
                double forceDividedByIncrement = 0;

                foreach (ClassPunchingShrPlot.JointPunchingShearDetails jointPunchingShearDetails in eachGroupByForce)
                {
                    if (firstLoop)
                    {
                        sectProp = jointPunchingShearDetails.SectProp;

                        forceDividedByIncrement = jointPunchingShearDetails.ForceDividedByIncrement;

                        firstLoop = false;
                    }

                    jointPunchingShearDetails.GroupIndex = groupIndex;
                    
                }

                ClassPunchingShrPlot.GroupDefinition groupDefinition = new ClassPunchingShrPlot.GroupDefinition(groupIndex, sectProp, forceDividedByIncrement * forceIncrement, (forceDividedByIncrement + 1) * forceIncrement);
                groupDefinitions.Add(groupDefinition);

                groupIndex++;
            }

            return groupDefinitions;
        }

        public static void WritePunchingShearDetailsToExcel(object[,] result, object[,] resultGroupDef, string fileName, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            //Create workbook
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //Add worksheets
            xlWorkBook.Sheets.Add(); //Fy

            //Worksheet
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "Punching shear result";

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

            //Worksheet
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            xlWorkSheet.Name = "Group definitions";

            numOfRow = resultGroupDef.GetLength(0);
            numOfCol = resultGroupDef.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = resultGroupDef;

            //Here saving the file in xlsx
            string excelPath = filePath + fileName + ".xlsx";

            try
            {
                xlWorkBook.SaveAs(excelPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
        }
    }
}
