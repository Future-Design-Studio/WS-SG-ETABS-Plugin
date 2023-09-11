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
using System.Reflection;
using ETABSv1;
using System.IO;

namespace WS_SG_ETABS_Plugin.src.tedds_main
{
    /// <summary>
    /// Interaction logic for WindowTeddsMain.xaml
    /// </summary>
    public partial class WindowTeddsMain : Window
    {
        private cPluginCallback _Plugin;
        private cSapModel _SapModel;
        public WindowTeddsMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            string version = "0.0.1";

            _Plugin = Plugin;
            _SapModel = SapModel;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }
            else
            {
                InitializeComponent();

                labelVersion.Content = "version " + version;

                ShowDialog();
            }




        }

        private void buttonHelper_Click(object sender, RoutedEventArgs e)
        {
            //IsEnabled = false;
            //Call up helper window
            string folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.Start($"{folder}\\src\\resources\\TEDDS_API.pdf");
            //IsEnabled = true;
        }

        private void buttonFeedback_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forms.office.com/e/qaiKf5e3a4");
        }

        private void buttonPrecastSplice_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up Release at selected nodes window
            src.tedds_main.precast_rc_column.WindowPrecastRCColumnLoading windowPrecastRCColumnLoading = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnLoading(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonPadFooting_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
