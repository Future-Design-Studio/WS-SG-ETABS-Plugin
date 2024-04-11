using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;
using SAFEv1;
using WS_SG_ETABS_Plugin.src.loads_etabs_etabs;
using WS_SG_ETABS_Plugin.src.loads_etabs_safe;

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    /// <summary>
    /// Interaction logic for WindowJointForcesExtractorResult.xaml
    /// </summary>
    public partial class WindowJointForcesExtractorResult : Window
    {
        public WindowJointForcesExtractorResult(ETABSv1.cSapModel SapModelETABS, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedLevel, List<ClassEtabsSafe.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable storydefinitionTable)
        {

            //Main logic
            bool isError = src.joint_forces_extractor.FunctionJointForcesExtractorMain.ExtractJointForcesExcel(SapModelETABS, SapModelSAFE, PluginEtabs, databaseTableList, selectedLevel, casePatternList, pointobjectconnectivityTable, jointreactionTable, storydefinitionTable);

            if (isError)
            {
                return;
            }

            InitializeComponent();
            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
