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

namespace WS_SG_ETABS_Plugin.src.select_uniquename
{
    /// <summary>
    /// Interaction logic for WindowSelUniqueNameMain.xaml
    /// </summary>
    public partial class WindowSelUniqueNameMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowSelUniqueNameMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();
            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //Casting file path
                string pointUniqueName = textBoxPoint.Text;
                string frameUniqueName = textBoxFrame.Text;
                string areaUniqueName = textBoxArea.Text;

                IsEnabled = false;
                //Call up result window
                src.select_uniquename.WindowSelUniqueNameResult windowSelUniqueNameResult = new src.select_uniquename.WindowSelUniqueNameResult(_SapModel, _Plugin, pointUniqueName, frameUniqueName, areaUniqueName);
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
