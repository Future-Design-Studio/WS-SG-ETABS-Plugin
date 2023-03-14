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

        public WindowLoadComboResult(cSapModel SapModel, cPluginCallback Plugin, 
            List<src.load_combo_EN.ClassLoadCombo.DeadLoad> deadLoadList, 
            List<src.load_combo_EN.ClassLoadCombo.SDLLoad> SDLoadList, 
            List<src.load_combo_EN.ClassLoadCombo.LiveLoad> LiveLoadList, 
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> WindLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalSDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalSDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.TemperatureLoad> TemperatureLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SeismicLoad> SeismicLoadList,
            List<src.load_combo_EN.ClassLoadCombo.AccidentalLoad> AccidentalLoadList,
            List<src.load_combo_EN.ClassLoadCombo.UpliftLoad> UpliftLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFSDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFSDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLLYLoadList,
            int notionaldirectionFactor, int windVertical)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Mainlogic
            bool isError = src.load_combo_EN.FunctionLoadCombo.CreateLoadPatternAndCase(_SapModel, _Plugin, deadLoadList, SDLoadList, LiveLoadList, WindLoadList, NotionalDXLoadList, NotionalDYLoadList, NotionalSDXLoadList, NotionalSDYLoadList, TemperatureLoadList, SeismicLoadList, AccidentalLoadList, UpliftLoadList, EHFDLXLoadList, EHFDLYLoadList, EHFSDLXLoadList, EHFSDLYLoadList, EHFLLXLoadList, EHFLLYLoadList, notionaldirectionFactor, windVertical);

            if (isError)
            {
                MessageBox.Show("Error creating patterns and cases, please let Mani know what happened");
                return;
            }

            InitializeComponent();

            ShowDialog();

        }
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

