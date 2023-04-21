using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;
using static WS_SG_ETABS_Plugin.src.HelperEtabs;
using Rhino;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    internal class FunctionEtabsEtabsMain
    {
        
        public static bool TransferLoadFromEtabsToEtabs(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedGroup, List<ClassEtabsEtabs.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable groupdefinitionTable, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable groupassignmentTable)
        {

            bool isError = false;
            List<src.HelperEtabs.DatabaseTable> filteredDatabaseTable;
            List<string> pointUniqueName = new List<string>();



            if (selectedGroup[0] == "All")
            {
                foreach (Dictionary<string, string> row in pointobjectconnectivityTable.TableInList)
                {
                    pointUniqueName.Add(row["UniqueName"]);
                }

                List<Dictionary<string, string>> jointreactionTableFiltered = new List<Dictionary<string, string>>();
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


            }


            return false;
        }
    }
}


