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

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    /// <summary>
    /// Interaction logic for WindowLoadComboResult.xaml
    /// </summary>
    public partial class WindowLoadComboResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowLoadComboResult(cSapModel SapModel, cPluginCallback Plugin, List<src.load_combo_EN.ClassLoadCombo.DeadLoad> deadLoadList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Mainlogic
            bool isError = src.load_combo_EN.FunctionLoadCombo.CreateLoadPatternAndCombo(_SapModel, _Plugin, deadLoadList);

            if (isError)
            {
                return;
            }

            InitializeComponent();

            ShowDialog();
        }
    }
}
