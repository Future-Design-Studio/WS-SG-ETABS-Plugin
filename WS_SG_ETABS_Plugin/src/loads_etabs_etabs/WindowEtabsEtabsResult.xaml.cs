using ETABSv1;
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

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    /// <summary>
    /// Interaction logic for WindowEtabsEtabsResult.xaml
    /// </summary>
    public partial class WindowEtabsEtabsResult : Window
    {
        public WindowEtabsEtabsResult(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {

            //Main logic
            bool isError = src.loads_etabs_etabs.FunctionEtabsEtabsMain.TransferLoadFromEtabsToEtabs(SapModel, Plugin, databaseTableList);

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
