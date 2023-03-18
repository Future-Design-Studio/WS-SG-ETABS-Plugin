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
    /// Interaction logic for WindowChooseLoadCombiToGenerate.xaml
    /// </summary>
    public partial class WindowChooseLoadCombiToGenerate : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        #region Data source loadlists

        List<ClassLoadCombo.DeadLoad> deadLoadList = new List<ClassLoadCombo.DeadLoad>();
        List<ClassLoadCombo.SDLLoad> SDLoadList = new List<ClassLoadCombo.SDLLoad>();
        List<ClassLoadCombo.LiveLoad> LiveLoadList = new List<ClassLoadCombo.LiveLoad>();
        List<ClassLoadCombo.WindLoad> WindLoadList = new List<ClassLoadCombo.WindLoad>();
        List<ClassLoadCombo.WindLoad> WindVertLoadList = new List<ClassLoadCombo.WindLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalDXLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalDYLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalSDXLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalSDYLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.TemperatureLoad> TemperatureLoadList = new List<ClassLoadCombo.TemperatureLoad>();
        List<ClassLoadCombo.SeismicLoad> SeismicLoadList = new List<ClassLoadCombo.SeismicLoad>();
        List<ClassLoadCombo.AccidentalLoad> AccidentalLoadList = new List<ClassLoadCombo.AccidentalLoad>();
        List<ClassLoadCombo.UpliftLoad> UpliftLoadList = new List<ClassLoadCombo.UpliftLoad>();
        List<ClassLoadCombo.EHFLoad> EHFDLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFDLYLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFSDLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFSDLYLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFLLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFLLYLoadList = new List<ClassLoadCombo.EHFLoad>();
        int notionaldirectionFactor = new int();
        int ALneedtimes2 = new int();
        bool createULSB = false;
        bool createULSC = false;
        bool createAccidental = false;
        bool createSeismic = false;
        bool createSLS = false;
        bool createSLSSteel = false;
        bool createUPLC1 = false;
        bool createUPLC2 = false;

        #endregion

        public WindowChooseLoadCombiToGenerate(cSapModel SapModel, cPluginCallback Plugin, bool accidentalYesOrNo, bool seismicYesOrNo, bool upliftYesOrNo,
            List<src.load_combo_EN.ClassLoadCombo.DeadLoad> DeadLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SDLLoad> sDLoadList,
            List<src.load_combo_EN.ClassLoadCombo.LiveLoad> liveLoadList,
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> windLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> notionalDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> notionalDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> notionalSDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> notionalSDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.TemperatureLoad> temperatureLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SeismicLoad> seismicLoadList,
            List<src.load_combo_EN.ClassLoadCombo.AccidentalLoad> accidentalLoadList,
            List<src.load_combo_EN.ClassLoadCombo.UpliftLoad> upliftLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFSDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFSDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFLLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> eHFLLYLoadList,
            int NotionalDirectionFactor,
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> windVertLoadList,
            int aLneedtimes2)
        {
            InitializeComponent();

            deadLoadList = DeadLoadList;
            SDLoadList = sDLoadList;
            LiveLoadList = liveLoadList;
            WindLoadList = windLoadList;
            WindVertLoadList = windVertLoadList;
            NotionalDXLoadList = notionalDXLoadList;
            NotionalDYLoadList = notionalDYLoadList;
            NotionalSDXLoadList = notionalSDXLoadList;
            NotionalSDYLoadList = notionalSDYLoadList;
            TemperatureLoadList = temperatureLoadList;
            SeismicLoadList = seismicLoadList;
            AccidentalLoadList = accidentalLoadList;
            UpliftLoadList = upliftLoadList;
            EHFDLXLoadList = eHFDLXLoadList;
            EHFDLYLoadList = eHFDLYLoadList;
            EHFSDLXLoadList = eHFSDLXLoadList;
            EHFSDLYLoadList = eHFSDLYLoadList;
            EHFLLXLoadList = eHFLLXLoadList;
            EHFLLYLoadList = eHFLLYLoadList;
            notionaldirectionFactor = NotionalDirectionFactor;
            ALneedtimes2 = aLneedtimes2;

            if (accidentalYesOrNo == false)
            {
                checkboxAccidental.IsEnabled = false;
            }
            if (seismicYesOrNo == false)
            {
                checkboxSeismic.IsEnabled = false;
            }
            if (upliftYesOrNo == false)
            {
                checkboxUpliftC1.IsEnabled = false;
                checkboxUpliftC2.IsEnabled = false;
            }
        }

        

        private void checkboxULSComboB_Checked(object sender, RoutedEventArgs e)
        {
            createULSB = true;
        }

        private void checkboxULSComboC_Checked(object sender, RoutedEventArgs e)
        {
            createULSC = true;
        }

        private void checkboxAccidental_Checked(object sender, RoutedEventArgs e)
        {
            createAccidental = true;
        }

        private void checkboxSeismic_Checked(object sender, RoutedEventArgs e)
        {
            createSeismic = true;
        }

        private void checkboxService_Checked(object sender, RoutedEventArgs e)
        {
            createSLS = true;
        }

        private void checkboxServiceSteel_Checked(object sender, RoutedEventArgs e)
        {
            createSLSSteel=true;
        }

        private void checkboxUpliftC1_Checked(object sender, RoutedEventArgs e)
        {
            createUPLC1 = true; 
        }

        private void checkboxUpliftC2_Checked(object sender, RoutedEventArgs e)
        {
            createUPLC2 = true;
        }

        private void checkboxULSComboB_Unchecked(object sender, RoutedEventArgs e)
        {
            createULSB = false;
        }

        private void checkboxULSComboC_Unchecked(object sender, RoutedEventArgs e)
        {
            createULSC = false;
        }

        private void checkboxAccidental_Unchecked(object sender, RoutedEventArgs e)
        {
            createAccidental = false;
        }

        private void checkboxSeismic_Unchecked(object sender, RoutedEventArgs e)
        {
            createSeismic = false;
        }

        private void checkboxService_Unchecked(object sender, RoutedEventArgs e)
        {
            createSLS = false;
        }

        private void checkboxServiceSteel_Unchecked(object sender, RoutedEventArgs e)
        {
            createSLSSteel = false;
        }

        private void checkboxUpliftC1_Unchecked(object sender, RoutedEventArgs e)
        {
            createUPLC1 = false;
        }

        private void checkboxUpliftC2_Unchecked(object sender, RoutedEventArgs e)
        {
            createUPLC2 = false;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            if (!createULSB && !createULSC && !createAccidental && !createSeismic && !createSLS && !createSLSSteel && !createUPLC1 && !createUPLC2)
            {
                MessageBox.Show("Warning: No Load combinations have been selected. Only load patterns and cases will be generated.");
            }

            //Call up result window
            src.load_combo_EN.WindowLoadComboResult windowLoadComboResult = new src.load_combo_EN.WindowLoadComboResult(_SapModel, _Plugin, deadLoadList, SDLoadList, LiveLoadList, WindLoadList, NotionalDXLoadList, NotionalDYLoadList, NotionalSDXLoadList, NotionalSDYLoadList, TemperatureLoadList, SeismicLoadList, AccidentalLoadList, UpliftLoadList, EHFDLXLoadList, EHFDLYLoadList, EHFSDLXLoadList, EHFSDLYLoadList, EHFLLXLoadList, EHFLLYLoadList, notionaldirectionFactor, WindVertLoadList, ALneedtimes2, createULSB, createULSC, createAccidental, createSeismic, createSLS, createSLSSteel, createUPLC1, createUPLC2);
            IsEnabled = true;
        }
    }
}
