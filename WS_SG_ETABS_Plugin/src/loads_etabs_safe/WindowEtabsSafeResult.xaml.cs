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

namespace WS_SG_ETABS_Plugin.src.loads_etabs_safe
{
    /// <summary>
    /// Interaction logic for WindowEtabsSafeResult.xaml
    /// </summary>
    public partial class WindowEtabsSafeResult : Window
    {
        public WindowEtabsSafeResult(ETABSv1.cSapModel SapModelETABS, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> selectedGroup, List<ClassEtabsSafe.CaseandPatternMatch> casePatternList, src.HelperEtabs.DatabaseTable groupdefinitionTable, src.HelperEtabs.DatabaseTable pointobjectconnectivityTable, src.HelperEtabs.DatabaseTable jointreactionTable, src.HelperEtabs.DatabaseTable groupassignmentTable)
        {

            //Main logic
            bool isError = src.loads_etabs_safe.FunctionEtabsSafeMain.TransferLoadFromEtabsToSafe(SapModelETABS, SapModelSAFE, PluginEtabs, databaseTableList, selectedGroup, casePatternList, groupdefinitionTable, pointobjectconnectivityTable, jointreactionTable, groupassignmentTable);

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
