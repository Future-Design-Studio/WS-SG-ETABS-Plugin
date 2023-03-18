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
        public WindowChooseLoadCombiToGenerate(cSapModel SapModel, cPluginCallback Plugin, bool accidentalYesOrNo, bool seismicYesOrNo, bool upliftYesOrNo)
        {
            InitializeComponent();

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

        bool createULSB = false;
        bool createULSC = false;
        bool createAccidental = false;
        bool createSeismic = false;
        bool createSLS = false;
        bool createSLSSteel = false;
        bool createUPLC1 = false;
        bool createUPLC2 = false;

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
    }
}
