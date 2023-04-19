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
    /// Interaction logic for WindowEtabsEtabsMain.xaml
    /// </summary>
    public partial class WindowEtabsEtabsMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        List<src.HelperEtabs.DatabaseTable> databaseTableList;
        public WindowEtabsEtabsMain(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            databaseTableList = DatabaseTableList;
            InitializeComponent();
            ShowDialog();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up copy property result window
            src.loads_etabs_etabs.WindowEtabsEtabsResult windowEtabsEtabsResult = new src.loads_etabs_etabs.WindowEtabsEtabsResult(_SapModel, _Plugin, databaseTableList);
            IsEnabled = true;
        }
    }
}
