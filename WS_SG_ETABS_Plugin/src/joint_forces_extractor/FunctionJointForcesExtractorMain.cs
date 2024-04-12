using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accord.Math.Distances;
using ETABSv1;
using Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    internal class FunctionJointForcesExtractorMain
    {

        public static bool ExtractJointForcesExcel(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase, List<string> listChoosenGroup, string filePath, string fileName)
        {

            bool isError = false;
            src.HelperEtabs.DatabaseTable groupAssignmentTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable beamConnectivityTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable columnConnectivityTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable braceConnectivityTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable columnForcesTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable beamForcesTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable braceForcesTable = new HelperEtabs.DatabaseTable();

            foreach (src.HelperEtabs.DatabaseTable table in databaseTableList)
            {
                if (table.TableKey == "Group Assignments")
                {
                    (isError, groupAssignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Beam Object Connectivity")
                {
                    (isError, beamConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Column Object Connectivity")
                {
                    (isError, columnConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Brace Object Connectivity")
                {
                    (isError, braceConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Element Forces - Columns")
                {
                    (isError, columnForcesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Element Forces - Beams")
                {
                    (isError, beamForcesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Element Forces - Braces")
                {
                    (isError, braceForcesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, table.TableKey);
                    if (isError)
                    {
                        return isError;
                    }
                }
                else
                {
                    MessageBox.Show("Error, list of database tables do not match. Please revert to FDS");
                }
            }

            //Filter group by points only
            (isError, groupAssignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentTable, "ObjectType", "Point");

            //Filter group by selected group name
            List<Dictionary<string, string>> groupAssignmentTableFiltered = new List<Dictionary<string, string>>();
            List<string> pointUniqueNames = new List<string>();
            foreach (Dictionary<string, string> row in groupAssignmentTable.TableInList)
            {
                if (listChoosenGroup.Contains(row["GroupName"]))
                {
                    groupAssignmentTableFiltered.Add(row);
                    if (pointUniqueNames.Contains(row["UniqueName"]))
                    {
                        continue;
                    }
                    else
                    {
                        pointUniqueNames.Add(row["UniqueName"]);
                    }
                }
            }

            //Filter column, beam and brace connectivity based on unique point in groups and which i or j
            List<Dictionary<string, string>> frameConnectivityTableFiltered = new List<Dictionary<string, string>>();

            //Seperate groups for which frames using which station
            List<string> frameUniqueNameI = new List<string>();
            List<string> frameUniqueNameJ = new List<string>();
            List<string> frameUniqueNameIJ = new List<string>();

            //create dictionary to match frame unique name with joint unique name
            Dictionary<string,string> jointUniqueNameI = new Dictionary<string,string>();
            Dictionary<string, string> jointUniqueNameJ = new Dictionary<string, string>();
            Dictionary<string, string> jointUniqueNameIJI = new Dictionary<string, string>();
            Dictionary<string, string> jointUniqueNameIJJ = new Dictionary<string, string>();


            foreach (Dictionary<string, string> row in beamConnectivityTable.TableInList)
            {
                if (pointUniqueNames.Contains(row["UniquePtI"]) && pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameIJ.Add(row["UniqueName"]);
                    jointUniqueNameIJI.Add(row["UniqueName"], row["UniquePtI"]);
                    jointUniqueNameIJJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtI"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameI.Add(row["UniqueName"]);
                    jointUniqueNameI.Add(row["UniqueName"], row["UniquePtI"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameJ.Add(row["UniqueName"]);
                    jointUniqueNameJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
            }
            foreach (Dictionary<string, string> row in columnConnectivityTable.TableInList)
            {
                if (pointUniqueNames.Contains(row["UniquePtI"]) && pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameIJ.Add(row["UniqueName"]);
                    jointUniqueNameIJI.Add(row["UniqueName"], row["UniquePtI"]);
                    jointUniqueNameIJJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtI"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameI.Add(row["UniqueName"]);
                    jointUniqueNameI.Add(row["UniqueName"], row["UniquePtI"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameJ.Add(row["UniqueName"]);
                    jointUniqueNameJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
            }
            foreach (Dictionary<string, string> row in braceConnectivityTable.TableInList)
            {
                if (pointUniqueNames.Contains(row["UniquePtI"]) && pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameIJ.Add(row["UniqueName"]);
                    jointUniqueNameIJI.Add(row["UniqueName"], row["UniquePtI"]);
                    jointUniqueNameIJJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtI"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameI.Add(row["UniqueName"]);
                    jointUniqueNameI.Add(row["UniqueName"], row["UniquePtI"]);
                }
                else if (pointUniqueNames.Contains(row["UniquePtJ"]))
                {
                    frameConnectivityTableFiltered.Add(row);
                    frameUniqueNameJ.Add(row["UniqueName"]);
                    jointUniqueNameJ.Add(row["UniqueName"], row["UniquePtJ"]);
                }
            }


            //Filter column, beam and brace forces based on unique point in groups and min/max
            List<Dictionary<string, string>> frameForcesTableFiltered = new List<Dictionary<string, string>>();

            #region Column forces
            //first for column forces
            foreach (Dictionary<string, string> row in columnForcesTable.TableInList)
            {
                #region I end
                //Check for frames with only I end first
                if (frameUniqueNameI.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(columnForcesTable, "UniqueName", row["UniqueName"]);

                    //find for minimum station value
                    double station = 1000.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) < station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));
                    
                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameI[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameI list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameI.Remove(row["UniqueName"]);
                }
                #endregion

                #region J end
                //Check for frames with only J end next
                else if (frameUniqueNameJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(columnForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameJ.Remove(row["UniqueName"]);
                }
                #endregion

                #region I & J end
                //Check for frames with both end finally
                else if (frameUniqueNameIJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(columnForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameIJJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }



                    //find for minimum station value
                    station = 1000.0;
                    foreach (Dictionary<string, string> row4 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row4["Station"]) < station)
                        {
                            station = Convert.ToDouble(row4["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row5 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row5.Add("JointUniqueName", jointUniqueNameIJI[row5["UniqueName"]]);
                        frameForcesTableFiltered.Add(row5);
                    }

                    //remove from frameUniqueNameIJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameIJ.Remove(row["UniqueName"]);
                }
                #endregion
            }
            #endregion

            #region Beam forces
            //first for column forces
            foreach (Dictionary<string, string> row in beamForcesTable.TableInList)
            {
                #region I end
                //Check for frames with only I end first
                if (frameUniqueNameI.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(beamForcesTable, "UniqueName", row["UniqueName"]);

                    //find for minimum station value
                    double station = 1000.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) < station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameI[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameI list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameI.Remove(row["UniqueName"]);
                }
                #endregion

                #region J end
                //Check for frames with only J end next
                else if (frameUniqueNameJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(beamForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameJ.Remove(row["UniqueName"]);
                }
                #endregion

                #region I & J end
                //Check for frames with both end finally
                else if (frameUniqueNameIJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(beamForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameIJJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }



                    //find for minimum station value
                    station = 1000.0;
                    foreach (Dictionary<string, string> row4 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row4["Station"]) < station)
                        {
                            station = Convert.ToDouble(row4["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row5 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row5.Add("JointUniqueName", jointUniqueNameIJI[row5["UniqueName"]]);
                        frameForcesTableFiltered.Add(row5);
                    }

                    //remove from frameUniqueNameIJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameIJ.Remove(row["UniqueName"]);
                }
                #endregion
            }
            #endregion

            #region Brace forces
            //first for column forces
            foreach (Dictionary<string, string> row in braceForcesTable.TableInList)
            {
                #region I end
                //Check for frames with only I end first
                if (frameUniqueNameI.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(braceForcesTable, "UniqueName", row["UniqueName"]);

                    //find for minimum station value
                    double station = 1000.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) < station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameI[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameI list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameI.Remove(row["UniqueName"]);
                }
                #endregion

                #region J end
                //Check for frames with only J end next
                else if (frameUniqueNameJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(braceForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }

                    //remove from frameUniqueNameJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameJ.Remove(row["UniqueName"]);
                }
                #endregion

                #region I & J end
                //Check for frames with both end finally
                else if (frameUniqueNameIJ.Contains(row["UniqueName"]))
                {
                    //Filter based on frame unique name
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTable = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMaximum = new HelperEtabs.DatabaseTable();
                    src.HelperEtabs.DatabaseTable temporaryFilteredForcesTableMinimum = new HelperEtabs.DatabaseTable();
                    (isError, temporaryFilteredForcesTable) = src.HelperEtabs.FilterDatabaseTable(braceForcesTable, "UniqueName", row["UniqueName"]);

                    //find for maximum station value
                    double station = 0.0;
                    foreach (Dictionary<string, string> row2 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row2["Station"]) > station)
                        {
                            station = Convert.ToDouble(row2["Station"]);
                        }
                    }

                    //filter based on maximum station value
                    (isError, temporaryFilteredForcesTableMaximum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row3 in temporaryFilteredForcesTableMaximum.TableInList)
                    {
                        row3.Add("JointUniqueName", jointUniqueNameIJJ[row3["UniqueName"]]);
                        frameForcesTableFiltered.Add(row3);
                    }



                    //find for minimum station value
                    station = 1000.0;
                    foreach (Dictionary<string, string> row4 in temporaryFilteredForcesTable.TableInList)
                    {
                        if (Convert.ToDouble(row4["Station"]) < station)
                        {
                            station = Convert.ToDouble(row4["Station"]);
                        }
                    }

                    //filter based on minimum station value
                    (isError, temporaryFilteredForcesTableMinimum) = src.HelperEtabs.FilterDatabaseTable(temporaryFilteredForcesTable, "Station", Convert.ToString(station));

                    //Add into frameforcestablefiltered
                    foreach (Dictionary<string, string> row5 in temporaryFilteredForcesTableMinimum.TableInList)
                    {
                        row5.Add("JointUniqueName", jointUniqueNameIJI[row5["UniqueName"]]);
                        frameForcesTableFiltered.Add(row5);
                    }

                    //remove from frameUniqueNameIJ list so that it doesnt go into multiple loops for the same frame
                    frameUniqueNameIJ.Remove(row["UniqueName"]);
                }
                #endregion
            }
            #endregion








            //Check if frameforces table is empty
            if (frameForcesTableFiltered.Count == 0)
            {
                MessageBox.Show("Error, please ensure the load case/combination you have selected have analysis results run.");
                return true;
            }

            List<string> colTitles = new List<string>()
            {
                "Joint Unique Name", "Frame Unique Name", "Load Case/Combi", "Step Type", "Axial (kN)", "Shear Major (kN)", "Shear Minor (kN)", "Torsion (kN)", "Moment Minor (kN)", "Moment Major (kN)"
            };
            List<Dictionary<string, string>> finalTableToExcel = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in frameForcesTableFiltered)
            {
                Dictionary<string, string> rowinternal = new Dictionary<string, string>
                {
                    {colTitles[0], row["JointUniqueName"]},
                    {colTitles[1], row["UniqueName"]},
                    {colTitles[2], row["OutputCase"]},
                    {colTitles[3], row["StepType"]},
                    {colTitles[4], row["P"]},
                    {colTitles[5], row["V2"]},
                    {colTitles[6], row["V3"]},
                    {colTitles[7], row["T"]},
                    {colTitles[8], row["M2"]},
                    {colTitles[9], row["M3"]},
                };
                finalTableToExcel.Add(rowinternal);
            }



            



            //Creating object for excel

            int numOfRow = finalTableToExcel.Count();
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
            foreach (Dictionary<string, string> row in finalTableToExcel)
            {
                result[i, 0] = row[colTitles[0]];
                result[i, 1] = row[colTitles[1]];
                result[i, 2] = row[colTitles[2]];
                result[i, 3] = row[colTitles[3]];
                result[i, 4] = row[colTitles[4]];
                result[i, 5] = row[colTitles[5]];
                result[i, 6] = row[colTitles[6]];
                result[i, 7] = row[colTitles[7]];
                result[i, 8] = row[colTitles[8]];
                result[i, 9] = row[colTitles[9]];
                i++;
            }

            
            //HERE TO WRITE TO EXCEL

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            xlApp.Visible = false;

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return true;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //Add worksheets
            xlWorkBook.Sheets.Add(); //Self design

            //Sheet 1
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "Output Etabs";

            numOfRow = result.GetLength(0);
            numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

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


            return false;
        }
    }
}
