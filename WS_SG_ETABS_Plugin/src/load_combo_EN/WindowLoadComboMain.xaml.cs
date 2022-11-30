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
    /// Interaction logic for WindowLoadComboMain.xaml
    /// </summary>
    public partial class WindowLoadComboMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        #region Data source

        public static List<string> quasiPermanentCategoryList = new List<string>()
        {
            "A: dometic,residential areas", "B: office areas", "C: congregation areas", "D: shopping areas", "E: storage areas", "F: traffic area, vw<30kN", "G: traffic area, 30kN<vw<160kN", "H: roofs", "Wind load", "Temperature"
        };

        #endregion

        #region Data source

        public static List<string> windLoadDirectionList = new List<string>()
        {
            "4", "8"
        };

        #endregion

        public WindowLoadComboMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();
        }

        private void comBoxSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
