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

namespace WS_SG_ETABS_Plugin.src.transfer_midas
{
    /// <summary>
    /// Interaction logic for WindowTransferMidasResult.xaml
    /// </summary>
    public partial class WindowTransferMidasResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        public WindowTransferMidasResult(cSapModel SapModel, cPluginCallback Plugin, string filePathFrom, Dictionary<string, bool?> selectedDatabaseTables)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Main logic
            bool isError = src.transfer_midas.FunctionTransferMidas.TransferToMidas(_SapModel, filePathFrom, selectedDatabaseTables);

            if (isError)
            {
                return;
            }

            InitializeComponent();
        }
    }
}
