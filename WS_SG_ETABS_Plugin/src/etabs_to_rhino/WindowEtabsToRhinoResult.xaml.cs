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
using Rhino.FileIO;
using Rhino.Geometry;

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    /// <summary>
    /// Interaction logic for WindowEtabsToRhinoResult.xaml
    /// </summary>
    public partial class WindowEtabsToRhinoResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowEtabsToRhinoResult(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            src.etabs_to_rhino.FunctionEtabsToRhino.ExportEtabsToRhino(_SapModel);

            InitializeComponent();

            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    
}
