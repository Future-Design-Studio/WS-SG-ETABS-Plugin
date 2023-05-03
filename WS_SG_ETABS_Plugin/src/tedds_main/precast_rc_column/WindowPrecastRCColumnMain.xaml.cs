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

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    /// <summary>
    /// Interaction logic for WindowPrecastRCColumnMain.xaml
    /// </summary>
    public partial class WindowPrecastRCColumnMain : Window
    {
        public WindowPrecastRCColumnMain(cSapModel SapModelEtabs, cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLevel.Count == 0)
            {
                MessageBox.Show("Please choose a storey level.");
            }
            else if (casePatternList.Count == 0)
            {
                MessageBox.Show("Please choose at least 1 load case to transfer loads.");
            }
            else
            {
                IsEnabled = false;
                //Call up copy property result window
                src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult windowPrecastRCColumnResult = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult();
                IsEnabled = true;
            }
        }
    }
}
