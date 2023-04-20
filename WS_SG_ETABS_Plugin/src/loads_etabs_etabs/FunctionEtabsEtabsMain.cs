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
        List<src.HelperEtabs.DatabaseTable> filteredDatabaseTable;
        public static bool TransferLoadFromEtabsToEtabs(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedGroup, List<ClassEtabsEtabs.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable groupdefinitionTable, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable groupassignmentTable)
        {
            bool isError = false;

            if (selectedGroup[0] == "All")
            {

            }


            return false;
        }
    }
}


