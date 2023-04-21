using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_SG_ETABS_Plugin.src.loads_etabs_etabs;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_safe
{
    internal class FunctionEtabsSafeMain
    {

        public static bool TransferLoadFromEtabsToEtabs(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedGroup, List<ClassEtabsEtabs.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable groupdefinitionTable, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable groupassignmentTable)
        {

            bool isError = false;
            List<string> pointUniqueName = new List<string>();


            List<Dictionary<string, string>> jointreactionTableFiltered = new List<Dictionary<string, string>>();

            if (selectedGroup[0] == "All")
            {
                #region filter by selected groups
                foreach (Dictionary<string, string> row in pointobjectconnectivityTable.TableInList)
                {
                    pointUniqueName.Add(row["UniqueName"]);
                }

                foreach (string eachUniqueName in pointUniqueName)
                {
                    foreach (Dictionary<string, string> row in jointreactionTable.TableInList)
                    {
                        if (row["UniqueName"] == eachUniqueName)
                        {
                            jointreactionTableFiltered.Add(row);
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region filter by selected groups
                src.HelperEtabs.DatabaseTable groupassignmentTablePoints = new src.HelperEtabs.DatabaseTable();

                (isError, groupassignmentTablePoints) = src.HelperEtabs.FilterDatabaseTable(groupassignmentTable, "ObjectType", "Point");

                foreach (string group in selectedGroup)
                {
                    src.HelperEtabs.DatabaseTable databasetableIndiGroup = new src.HelperEtabs.DatabaseTable();
                    (isError, databasetableIndiGroup) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(jointreactionTable, groupassignmentTablePoints, group);
                    foreach (Dictionary<string, string> row in databasetableIndiGroup.TableInList)
                    {
                        jointreactionTableFiltered.Add(row);
                    }
                }

                #endregion
            }



            #region Point unique name matching
            pointUniqueName.Clear();

            foreach (Dictionary<string, string> row in jointreactionTableFiltered)
            {
                pointUniqueName.Add(row["UniqueName"]);
            }

            pointUniqueName = pointUniqueName.Distinct().ToList();

            List<Dictionary<string, string>> pointobjectconnectivityTableFiltered = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> pointobjectconnectivityTableFromDict = pointobjectconnectivityTable.TableInList;
            int i = 0;
            foreach (string eachUniqueName in pointUniqueName)
            {
                foreach (Dictionary<string, string> row in pointobjectconnectivityTableFromDict)
                {
                    if (row["UniqueName"] == eachUniqueName)
                    {
                        pointobjectconnectivityTableFiltered.Add(row);
                        break;
                    }
                    i++;
                }
                pointobjectconnectivityTableFromDict.RemoveAt(i);
                i = 0;
            }

            src.HelperEtabs.DatabaseTable pointobjectconnectivityTableTo = new src.HelperEtabs.DatabaseTable();
            List<string> pointUniqueNameTo = new List<string>();

            (isError, pointobjectconnectivityTableTo) = src.HelperEtabs.GetTableDisplay(SapModel, "Point Object Connectivity", false);
            if (isError)
            {
                return isError;
            }

            List<Dictionary<string, string>> pointobjectconnectivityTableToDict = pointobjectconnectivityTableTo.TableInList;

            foreach (Dictionary<string, string> rowfrom in pointobjectconnectivityTableFiltered)
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
                    isError = src.HelperEtabs.AddLoadPattern(SapModel, casematch.LoadPatternTo, 0, "Other", false, true);
                }
            }
            #endregion

            #region create dictionary with updated joint unique names and load patterns
            List<Dictionary<string, string>> jointreactionTableFilteredLoads = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in jointreactionTableFiltered)
            {
                if (caseFrom.Contains(row["OutputCase"]))
                {
                    row["UniqueName"] = pointUniqueNameTo[pointUniqueName.IndexOf(row["UniqueName"])];
                    row["OutputCase"] = patternTo[caseFrom.IndexOf(row["OutputCase"])];
                    jointreactionTableFilteredLoads.Add(row);
                }
            }
            #endregion

            #region create group and add points inside

            int ret = SapModel.GroupDef.Delete("Temp_GRP_Plugin_Joints_Identified");
            isError = src.HelperEtabs.CreateNewGroup(SapModel, "Temp_GRP_Plugin_Joints_Identified");
            if (isError)
            {
                MessageBox.Show("Unable to create group, please check what happen");
                return isError;
            }
            isError = src.HelperEtabs.AddPointsInsideGroup(SapModel, pointUniqueNameTo, "Temp_GRP_Plugin_Joints_Identified");
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
            src.HelperEtabs.DatabaseTable databaseTableLoadsAssigned = new src.HelperEtabs.DatabaseTable();
            databaseTableLoadsAssigned = src.HelperEtabs.GetTableEdit(SapModel, "Joint Loads Assignments - Force");

            //src.HelperEtabs.DeleteExistingLoadsFromPoints(SapModel, patternTo, "Temp_GRP_Plugin_Joints_Identified");

            foreach (Dictionary<string, string> row in databaseTableLoadsAssigned.TableInList)
            {
                if (pointUniqueNameTo.Contains(row["UniqueName"]) && patternTo.Contains(row["LoadPattern"]))
                {
                    MessageBox.Show("Warning: Some or all of the joints inside group: Temp_GRP_Plugin_Joints_Identified have joint loads already assigned which will add on to the loads being transferred by this plugin. Existing loads will not be deleted.");
                    break;
                }
            }

            foreach (Dictionary<string, string> row in jointreactionTableFilteredLoads)
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
                databaseTableLoadsAssigned.TableInList.Add(newrow);
            }
            isError = src.HelperEtabs.ApplyEtabsTable(SapModel, databaseTableLoadsAssigned);

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


