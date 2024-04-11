using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_SG_ETABS_Plugin.src.loads_etabs_safe;

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    internal class FunctionJointForcesExtractorMain
    {

        public static bool ExtractJointForcesExcel(ETABSv1.cSapModel SapModelETABS, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedLevel, List<ClassEtabsSafe.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable storydefinitionTable)
        {

            return false;
        }
    }
}
