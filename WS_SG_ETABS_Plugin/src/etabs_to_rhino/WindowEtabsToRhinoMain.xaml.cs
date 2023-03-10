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

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    /// <summary>
    /// Interaction logic for WindowEtabsToRhinoMain.xaml
    /// </summary>
    public partial class WindowEtabsToRhinoMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowEtabsToRhinoMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            ShowDialog();
        }

        #region Button Method

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            //Call up export result window
            src.etabs_to_rhino.WindowEtabsToRhinoResult windowEtabsToRhinoResult = new src.etabs_to_rhino.WindowEtabsToRhinoResult(_SapModel, _Plugin);
            
            IsEnabled = true;
        }

        #endregion
    }
}
