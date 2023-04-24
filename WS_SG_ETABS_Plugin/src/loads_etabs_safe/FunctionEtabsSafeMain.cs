using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using SAFEv1;
using System.Windows.Forms;
using Microsoft.Office.Core;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_safe
{
    internal class FunctionEtabsSafeMain
    {

        public static bool TransferLoadFromEtabsToSafe(ETABSv1.cSapModel SapModelETABS, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedLevel, List<ClassEtabsSafe.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable storydefinitionTable)
        {

            bool isError = false;
            List<string> pointUniqueName = new List<string>();


            src.HelperEtabs.DatabaseTable jointforcesFrameTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable jointforcesShellTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable jointforcesLinkTable = new HelperEtabs.DatabaseTable();

            List<string> listofTableKeys = new List<string>() { "Element Joint Forces - Frame", "Element Joint Forces - Shells", "Element Joint Forces - Links" };

            //if (databaseTableList.Contains("Element Joint Forces - Frame"))

            foreach (src.HelperEtabs.DatabaseTable table in databaseTableList)
            {
                try
                {
                    if (table.TableKey == "Element Joint Forces - Frame")
                    {
                        (isError, jointforcesFrameTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Joint Forces - Frame");
                        if (isError)
                        {
                            return isError;
                        }
                        listofTableKeys.Remove("Element Joint Forces - Frame");
                    }
                    if (table.TableKey == "Element Joint Forces - Shells")
                    {
                        (isError, jointforcesShellTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Joint Forces - Shells");
                        if (isError)
                        {
                            return isError;
                        }
                        listofTableKeys.Remove("Element Joint Forces - Shells");
                    }
                    if (table.TableKey == "Element Joint Forces - Links")
                    {
                        (isError, jointforcesLinkTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Joint Forces - Links");
                        if (isError)
                        {
                            return isError;
                        }
                        listofTableKeys.Remove("Element Joint Forces - Links");
                    }
                }
                catch
                {
                    continue;
                }
                
            }

            int storyindex = 0;
            List<string> storyLevelsList = new List<string>();
            foreach (Dictionary<string, string> row in storydefinitionTable.TableInList)
            {
                if (row["Story"] != selectedLevel[0])
                {
                    storyLevelsList.Add(row["Story"]);
                }
            }



            #region filter point unique names, jointreactiontable by selected level

            (isError, pointobjectconnectivityTable) = src.HelperEtabs.FilterDatabaseTable(pointobjectconnectivityTable, "Story", selectedLevel[0]);
            List<string> pointUniqueNameFromAll = new List<string>();
            List<string> pointUniqueNameToRemove = new List<string>();
            foreach (Dictionary<string, string> row in pointobjectconnectivityTable.TableInList)
            {
                pointUniqueName.Add(row["UniqueName"]);
                pointUniqueNameFromAll.Add(row["UniqueName"]);
                pointUniqueNameToRemove.Add(row["UniqueName"]);
            }


            List<string> pointUniqueNameReactions = new List<string>();
            List<Dictionary<string,string>> jointreactionDatabaseFiltered = new List<Dictionary<string,string>>();

            foreach (Dictionary<string, string> row in jointreactionTable.TableInList)
            {
                if (pointUniqueName.Contains(row["UniqueName"]))
                {
                    pointUniqueNameReactions.Add(row["UniqueName"]);
                    pointUniqueNameToRemove.Remove(row["UniqueName"]);
                    jointreactionDatabaseFiltered.Add(row);
                }
            }
            #endregion

            #region filter joint forces by above selected level
            List<Dictionary<string, string>> jointforcesFrameTableFiltered = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> jointforcesShellTableFiltered = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> jointforcesLinkTableFiltered = new List<Dictionary<string, string>>();

            try
            {
                foreach (Dictionary<string, string> row in jointforcesFrameTable.TableInList)
                {
                    if (storyLevelsList.Contains(row["Story"]) && pointUniqueNameToRemove.Contains(row["Joint"]))
                    {
                        jointforcesFrameTableFiltered.Add(row);
                    }
                }
            }
            catch
            {
                
            }
            

            try
            {
                foreach (Dictionary<string, string> row in jointforcesShellTable.TableInList)
                {
                    if (storyLevelsList.Contains(row["Story"]) && pointUniqueNameToRemove.Contains(row["Joint"]))
                    {
                        jointforcesShellTableFiltered.Add(row);
                    }
                }
            }
            catch
            {

            }
            
            try
            {
                foreach (Dictionary<string, string> row in jointforcesLinkTable.TableInList)
                {
                    if (storyLevelsList.Contains(row["Story"]) && pointUniqueNameToRemove.Contains(row["Joint"]))
                    {
                        jointforcesLinkTableFiltered.Add(row);
                    }
                }
            }
            catch 
            {

            }
            #endregion



            #region Point unique name matching
            src.HelperSafe.DatabaseTable pointobjectconnectivityTableTo = new src.HelperSafe.DatabaseTable();
            List<string> pointUniqueNameTo = new List<string>();

            (isError, pointobjectconnectivityTableTo) = src.HelperSafe.GetTableDisplay(SapModelSAFE, "Point Object Connectivity", false);
            if (isError)
            {
                return isError;
            }

            List<Dictionary<string, string>> pointobjectconnectivityTableToDict = pointobjectconnectivityTableTo.TableInList;

            foreach (Dictionary<string, string> rowfrom in pointobjectconnectivityTable.TableInList)
            {
                List<double> distanceList = new List<double>();
                foreach (Dictionary<string, string> rowto in pointobjectconnectivityTableToDict)
                {
                    double distance = Math.Sqrt(Math.Pow(Convert.ToDouble(rowfrom["X"]) - Convert.ToDouble(rowto["X"]), 2.0) + Math.Pow(Convert.ToDouble(rowfrom["Y"]) - Convert.ToDouble(rowto["Y"]), 2.0) + Math.Pow(Convert.ToDouble(rowfrom["Z"]) - Convert.ToDouble(rowto["Z"]), 2.0));
                    distanceList.Add(distance);
                    if (distance < 0.001)
                    {
                        break;
                    }

                }

                int indexofMinDistance = distanceList.IndexOf(distanceList.Min());
                pointUniqueNameTo.Add(pointobjectconnectivityTableToDict[indexofMinDistance]["UniqueName"]);
                pointobjectconnectivityTableToDict.RemoveAt(indexofMinDistance);
            }
            #endregion

            #region case & pattern matching as well as create new pattern
            List<string> caseFrom = new List<string>();
            List<string> patternTo = new List<string>();
            foreach (var casematch in casePatternList)
            {
                caseFrom.Add(casematch.LoadCaseFrom);
                patternTo.Add(casematch.LoadPatternTo);

                if (casematch.IsCreateNewPattern)
                {
                    isError = src.HelperSafe.AddLoadPattern(SapModelSAFE, casematch.LoadPatternTo, 0, "Other", false, true);
                }
            }
            #endregion

            #region create dictionary with updated joint unique names and load patterns
            List<Dictionary<string, string>> jointreactionTableFilteredLoads = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> jointforcesFrameTableFilteredLoads = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> jointforcesShellTableFilteredLoads = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> jointforcesLinkTableFilteredLoads = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in jointreactionDatabaseFiltered)
            {
                if (caseFrom.Contains(row["OutputCase"]))
                {
                    row["UniqueName"] = pointUniqueNameTo[pointUniqueNameFromAll.IndexOf(row["UniqueName"])];
                    row["OutputCase"] = patternTo[caseFrom.IndexOf(row["OutputCase"])];
                    jointreactionTableFilteredLoads.Add(row);
                }
            }

            try
            {
                foreach (Dictionary<string, string> row in jointforcesFrameTableFiltered)
                {
                    if (caseFrom.Contains(row["OutputCase"]))
                    {
                        row["Joint"] = pointUniqueNameTo[pointUniqueNameFromAll.IndexOf(row["Joint"])];
                        row["OutputCase"] = patternTo[caseFrom.IndexOf(row["OutputCase"])];
                        jointforcesFrameTableFilteredLoads.Add(row);
                    }
                }
            }
            catch { }

            try
            {
                foreach (Dictionary<string, string> row in jointforcesShellTableFiltered)
                {
                    if (caseFrom.Contains(row["OutputCase"]))
                    {
                        row["Joint"] = pointUniqueNameTo[pointUniqueNameFromAll.IndexOf(row["Joint"])];
                        row["OutputCase"] = patternTo[caseFrom.IndexOf(row["OutputCase"])];
                        jointforcesShellTableFilteredLoads.Add(row);
                    }
                }
            }
            catch { }

            try
            {
                foreach (Dictionary<string, string> row in jointforcesLinkTableFiltered)
                {
                    if (caseFrom.Contains(row["OutputCase"]))
                    {
                        row["Joint"] = pointUniqueNameTo[pointUniqueNameFromAll.IndexOf(row["Joint"])];
                        row["OutputCase"] = patternTo[caseFrom.IndexOf(row["OutputCase"])];
                        jointforcesLinkTableFilteredLoads.Add(row);
                    }
                }
            }
            catch { }
            
            #endregion

            #region create group and add points inside
            int ret = SapModelSAFE.GroupDef.Delete("Temp_GRP_Plugin_Joints_Identified");
            isError = src.HelperSafe.CreateNewGroup(SapModelSAFE, "Temp_GRP_Plugin_Joints_Identified");
            if (isError)
            {
                MessageBox.Show("Unable to create group, please check what happen");
                return isError;
            }
            isError = src.HelperSafe.AddPointsInsideGroup(SapModelSAFE, pointUniqueNameTo, "Temp_GRP_Plugin_Joints_Identified");
            if (isError)
            {
                MessageBox.Show("Unable to add points inside group, please check what happen");
                return isError;
            }
            else
            {
                MessageBox.Show("Successfully added all identified joints in which loads are assigned to group name: Temp_GRP_Plugin_Joints_Identified.");
            }
            #endregion


            #region update database table with loads assigned

            List<string> checkJointExist = new List<string>();
            src.HelperSafe.DatabaseTable databaseTableLoadsAssigned = new src.HelperSafe.DatabaseTable();
            databaseTableLoadsAssigned = src.HelperSafe.GetTableEdit(SapModelSAFE, "Joint Loads Assignments - Force");

            foreach (Dictionary<string, string> row in databaseTableLoadsAssigned.TableInList)
            {
                if (pointUniqueNameTo.Contains(row["UniqueName"]) && patternTo.Contains(row["LoadPattern"]))
                {
                    row["FX"] = "0.00";
                    row["FY"] = "0.00";
                    row["FZ"] = "0.00";
                    row["MX"] = "0.00";
                    row["MY"] = "0.00";
                    row["MZ"] = "0.00";
                    checkJointExist.Add(row["UniqueName"] + row["LoadPattern"]);
                }
            }
            


            foreach (Dictionary<string, string> row in jointreactionTableFilteredLoads)
            {
                if (checkJointExist.Contains(row["UniqueName"] + row["OutputCase"]))
                {
                    foreach (Dictionary<string, string> rowupdate in databaseTableLoadsAssigned.TableInList)
                    {
                        if (rowupdate["UniqueName"] == row["UniqueName"] && rowupdate["LoadPattern"] == row["OutputCase"])
                        {
                            rowupdate["FX"] = (Convert.ToDouble(rowupdate["FX"]) + ((Convert.ToDouble(row["FX"])) * (-1))).ToString();
                            rowupdate["FY"] = (Convert.ToDouble(rowupdate["FY"]) + ((Convert.ToDouble(row["FY"])) * (-1))).ToString();
                            rowupdate["FZ"] = (Convert.ToDouble(rowupdate["FZ"]) + ((Convert.ToDouble(row["FZ"])) * (-1))).ToString();
                            rowupdate["MX"] = (Convert.ToDouble(rowupdate["MX"]) + ((Convert.ToDouble(row["MX"])) * (-1))).ToString();
                            rowupdate["MY"] = (Convert.ToDouble(rowupdate["MY"]) + ((Convert.ToDouble(row["MY"])) * (-1))).ToString();
                            rowupdate["MZ"] = (Convert.ToDouble(rowupdate["MZ"]) + ((Convert.ToDouble(row["MZ"])) * (-1))).ToString();
                            break;
                        }
                    }
                }
                else
                {
                    Dictionary<string, string> newrow = new Dictionary<string, string>()
                    {
                        {"UniqueName", row["UniqueName"] },
                        {"LoadPattern", row["OutputCase"] },
                        {"FX", ((Convert.ToDouble(row["FX"]))*(-1)).ToString()},
                        {"FY", ((Convert.ToDouble(row["FY"]))*(-1)).ToString()},
                        {"FZ", ((Convert.ToDouble(row["FZ"]))*(-1)).ToString()},
                        {"MX", ((Convert.ToDouble(row["MX"]))*(-1)).ToString()},
                        {"MY", ((Convert.ToDouble(row["MY"]))*(-1)).ToString()},
                        {"MZ", ((Convert.ToDouble(row["MZ"]))*(-1)).ToString()},
                        {"XDim", "" },
                        {"YDim", "" },
                        {"GUID", "" }
                    };
                    checkJointExist.Add(row["UniqueName"] + row["OutputCase"]);
                    databaseTableLoadsAssigned.TableInList.Add(newrow);
                }
            }

            try
            {
                foreach (Dictionary<string, string> row in jointforcesFrameTableFilteredLoads)
                {
                    if (checkJointExist.Contains(row["Joint"] + row["OutputCase"]))
                    {
                        foreach (Dictionary<string, string> rowupdate in databaseTableLoadsAssigned.TableInList)
                        {
                            if (rowupdate["UniqueName"] == row["Joint"] && rowupdate["LoadPattern"] == row["OutputCase"])
                            {
                                rowupdate["FX"] = (Convert.ToDouble(rowupdate["FX"]) + ((Convert.ToDouble(row["F1"])) * (-1))).ToString();
                                rowupdate["FY"] = (Convert.ToDouble(rowupdate["FY"]) + ((Convert.ToDouble(row["F2"])) * (-1))).ToString();
                                rowupdate["FZ"] = (Convert.ToDouble(rowupdate["FZ"]) + ((Convert.ToDouble(row["F3"])) * (-1))).ToString();
                                rowupdate["MX"] = (Convert.ToDouble(rowupdate["MX"]) + ((Convert.ToDouble(row["M1"])) * (-1))).ToString();
                                rowupdate["MY"] = (Convert.ToDouble(rowupdate["MY"]) + ((Convert.ToDouble(row["M2"])) * (-1))).ToString();
                                rowupdate["MZ"] = (Convert.ToDouble(rowupdate["MZ"]) + ((Convert.ToDouble(row["M3"])) * (-1))).ToString();
                                break;
                            }
                        }
                    }
            
                    else
            
                    {
                        Dictionary<string, string> newrow = new Dictionary<string, string>()
                
                        {
                    
                            {"UniqueName", row["Joint"] },
                            {"LoadPattern", row["OutputCase"] },
                            {"FX", ((Convert.ToDouble(row["F1"]))*(-1)).ToString()},
                            {"FY", ((Convert.ToDouble(row["F2"]))*(-1)).ToString()},
                            {"FZ", ((Convert.ToDouble(row["F3"]))*(-1)).ToString()},
                            {"MX", ((Convert.ToDouble(row["M1"]))*(-1)).ToString()},
                            {"MY", ((Convert.ToDouble(row["M2"]))*(-1)).ToString()},
                            {"MZ", ((Convert.ToDouble(row["M3"]))*(-1)).ToString()},
                            {"XDim", "" },
                            {"YDim", "" },
                            {"GUID", "" }
                        };
                        checkJointExist.Add(row["Joint"] + row["OutputCase"]);
                        databaseTableLoadsAssigned.TableInList.Add(newrow);
                    }
                }
            }
            catch { }
            
            try
            {
                foreach (Dictionary<string, string> row in jointforcesShellTableFilteredLoads)
                {
                    if (checkJointExist.Contains(row["Joint"] + row["OutputCase"]))
                    {
                        foreach (Dictionary<string, string> rowupdate in databaseTableLoadsAssigned.TableInList)
                        {
                            if (rowupdate["UniqueName"] == row["Joint"] && rowupdate["LoadPattern"] == row["OutputCase"])
                            {
                                rowupdate["FX"] = (Convert.ToDouble(rowupdate["FX"]) + ((Convert.ToDouble(row["F1"])) * (-1))).ToString();
                                rowupdate["FY"] = (Convert.ToDouble(rowupdate["FY"]) + ((Convert.ToDouble(row["F2"])) * (-1))).ToString();
                                rowupdate["FZ"] = (Convert.ToDouble(rowupdate["FZ"]) + ((Convert.ToDouble(row["F3"])) * (-1))).ToString();
                                rowupdate["MX"] = (Convert.ToDouble(rowupdate["MX"]) + ((Convert.ToDouble(row["M1"])) * (-1))).ToString();
                                rowupdate["MY"] = (Convert.ToDouble(rowupdate["MY"]) + ((Convert.ToDouble(row["M2"])) * (-1))).ToString();
                                rowupdate["MZ"] = (Convert.ToDouble(rowupdate["MZ"]) + ((Convert.ToDouble(row["M3"])) * (-1))).ToString();
                                break;
                            }
                        }
                    }

                    else

                    {
                        Dictionary<string, string> newrow = new Dictionary<string, string>()

                    {

                        {"UniqueName", row["Joint"] },
                        {"LoadPattern", row["OutputCase"] },
                        {"FX", ((Convert.ToDouble(row["F1"]))*(-1)).ToString()},
                        {"FY", ((Convert.ToDouble(row["F2"]))*(-1)).ToString()},
                        {"FZ", ((Convert.ToDouble(row["F3"]))*(-1)).ToString()},
                        {"MX", ((Convert.ToDouble(row["M1"]))*(-1)).ToString()},
                        {"MY", ((Convert.ToDouble(row["M2"]))*(-1)).ToString()},
                        {"MZ", ((Convert.ToDouble(row["M3"]))*(-1)).ToString()},
                        {"XDim", "" },
                        {"YDim", "" },
                        {"GUID", "" }
                    };
                        checkJointExist.Add(row["Joint"] + row["OutputCase"]);
                        databaseTableLoadsAssigned.TableInList.Add(newrow);
                    }
                }
            }
            catch 
            {
                
            }
            
            try
            {
                foreach (Dictionary<string, string> row in jointforcesLinkTableFilteredLoads)
                {
                    if (checkJointExist.Contains(row["Joint"] + row["OutputCase"]))
                    {
                        foreach (Dictionary<string, string> rowupdate in databaseTableLoadsAssigned.TableInList)
                        {
                            if (rowupdate["UniqueName"] == row["Joint"] && rowupdate["LoadPattern"] == row["OutputCase"])
                            {
                                rowupdate["FX"] = (Convert.ToDouble(rowupdate["FX"]) + ((Convert.ToDouble(row["F1"])) * (-1))).ToString();
                                rowupdate["FY"] = (Convert.ToDouble(rowupdate["FY"]) + ((Convert.ToDouble(row["F2"])) * (-1))).ToString();
                                rowupdate["FZ"] = (Convert.ToDouble(rowupdate["FZ"]) + ((Convert.ToDouble(row["F3"])) * (-1))).ToString();
                                rowupdate["MX"] = (Convert.ToDouble(rowupdate["MX"]) + ((Convert.ToDouble(row["M1"])) * (-1))).ToString();
                                rowupdate["MY"] = (Convert.ToDouble(rowupdate["MY"]) + ((Convert.ToDouble(row["M2"])) * (-1))).ToString();
                                rowupdate["MZ"] = (Convert.ToDouble(rowupdate["MZ"]) + ((Convert.ToDouble(row["M3"])) * (-1))).ToString();
                                break;
                            }
                        }
                    }

                    else

                    {
                        Dictionary<string, string> newrow = new Dictionary<string, string>()

                    {

                        {"UniqueName", row["Joint"] },
                        {"LoadPattern", row["OutputCase"] },
                        {"FX", ((Convert.ToDouble(row["F1"]))*(-1)).ToString()},
                        {"FY", ((Convert.ToDouble(row["F2"]))*(-1)).ToString()},
                        {"FZ", ((Convert.ToDouble(row["F3"]))*(-1)).ToString()},
                        {"MX", ((Convert.ToDouble(row["M1"]))*(-1)).ToString()},
                        {"MY", ((Convert.ToDouble(row["M2"]))*(-1)).ToString()},
                        {"MZ", ((Convert.ToDouble(row["M3"]))*(-1)).ToString()},
                        {"XDim", "" },
                        {"YDim", "" },
                        {"GUID", "" }
                    };
                        checkJointExist.Add(row["Joint"] + row["OutputCase"]);
                        databaseTableLoadsAssigned.TableInList.Add(newrow);
                    }
                }
            }
            catch { }
            




            isError = src.HelperSafe.ApplyEtabsTable(SapModelSAFE, databaseTableLoadsAssigned);

            if (isError)
            {
                MessageBox.Show("Error updating database tables with loads assigned");
                return true;
            }
            #endregion

            return false;
        }
    }
}


