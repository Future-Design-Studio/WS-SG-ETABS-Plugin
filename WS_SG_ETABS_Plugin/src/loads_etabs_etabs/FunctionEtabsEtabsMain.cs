using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

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

                List<Dictionary<string, string>> pointobjectconnectivityTableFiltered = new List<Dictionary<string, string>>();
                foreach (string eachUniqueName in pointUniqueName)
                {
                    foreach (Dictionary<string, string> row in pointobjectconnectivityTable.TableInList)
                    {
                        if (row["UniqueName"] == eachUniqueName)
                        {
                            pointobjectconnectivityTableFiltered.Add(row);
                        }
                    }
                }




            }


            return false;
        }
    }
}


