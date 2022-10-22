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

namespace WS_SG_ETABS_Plugin.src.copy_properties
{
    /// <summary>
    /// Interaction logic for WindowCopyPropResult.xaml
    /// </summary>
    public partial class WindowCopyPropResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowCopyPropResult(cSapModel SapModel, cPluginCallback Plugin, string filePathFrom)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            ShowDialog();
        }
    }
}
