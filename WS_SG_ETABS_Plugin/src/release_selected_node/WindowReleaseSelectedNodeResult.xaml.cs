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

namespace WS_SG_ETABS_Plugin.src.release_selected_node
{
    /// <summary>
    /// Interaction logic for WindowReleaseSelectedNodeResult.xaml
    /// </summary>
    public partial class WindowReleaseSelectedNodeResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        public WindowReleaseSelectedNodeResult(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Mainlogic
            bool isError = src.release_selected_node.FunctionReleaseSelectedNode.ReleaseSelectedNodes(_SapModel, _Plugin);

            /*
            if (isError)
            {
                MessageBox.Show("Error, please let Mani know what happened");
                return;
            }
            */

            InitializeComponent();

            ShowDialog();
        }
    }
}
