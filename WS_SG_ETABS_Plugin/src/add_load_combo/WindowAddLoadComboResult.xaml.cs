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

namespace WS_SG_ETABS_Plugin.src.add_load_combo
{
    /// <summary>
    /// Interaction logic for WindowAddLoadComboResult.xaml
    /// </summary>
    public partial class WindowAddLoadComboResult : Window
    {
        public WindowAddLoadComboResult(cSapModel _SapModel, cPluginCallback _Plugin, int loadComboTypeInt, List<Dictionary<string, string>>  loadAndFactorList, 
            string loadComboName)
        {
            InitializeComponent();

            bool isError = FunctionAddLoadCombo.AddLoadCombo(_SapModel, _Plugin, loadComboTypeInt, loadAndFactorList, loadComboName);

            if (isError)
            {
                return;
            }
            else
            {
                ShowDialog();
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
