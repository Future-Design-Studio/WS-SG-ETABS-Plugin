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
using System.IO;


namespace WS_SG_ETABS_Plugin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private cPluginCallback _Plugin;
        private cSapModel _SapModel;

        public WindowMain(ref cSapModel SapModel, ref cPluginCallback Plugin)
        {
            _Plugin = Plugin;
            _SapModel = SapModel;

            src.HelperEtabs.SetOutputUnitTo_kN_m_C(_SapModel);

            //Read license number
            try
            {
                StreamReader sr = new StreamReader(@"C:\Program Files\Computers and Structures\ETABS 20\LMHOST.ini");
                string licenseNumber = sr.ReadLine();

                if (licenseNumber == "rwcphlice002")
                {
                    InitializeComponent();

                    //test.Test.PrintAllDatabaseTable(_SapModel);
                    //test.Test.PrintTableDisplay(_SapModel, "Material List by Section Property");
                    //test.Test.CrossProductTesting(-3, 1, 0, -1, 0, 3);

                    ShowDialog();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Invalid license. You have been reported to Ramboll IT as well as Singapore Police.");
            }

        }

        private void WindowMain_Closed(object sender, EventArgs e)
        {
            // must include a call to finish()
            _Plugin.Finish(0);

        }

        //Button functionality
        private void buttonCarbon_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up carbon window
            src.carbon.WindowCarbonMain windowCarbonMain = new src.carbon.WindowCarbonMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonShellUni_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up shell uniform window
            src.shell_uni_orient.WindowShellUniMain windowShellUniMain = new src.shell_uni_orient.WindowShellUniMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonCopyProperties_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up shell uniform window
            src.copy_properties.WindowCopyPropMain WindowCopyPropMain = new src.copy_properties.WindowCopyPropMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonRCBeamSchedule_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up shell uniform window
            src.rc_beam_schedule.WindowRCBSchedMain windowShellUniMain = new src.rc_beam_schedule.WindowRCBSchedMain(_SapModel, _Plugin);
            IsEnabled = true;
        }


        private void buttonPlotReaction_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up shell uniform window
            src.plot_reaction.WindowPlotReactionMain windowPlotReactionMain = new src.plot_reaction.WindowPlotReactionMain(_SapModel, _Plugin);
            IsEnabled = true;

        }

        private void buttonTransferMidas_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up transfer midas window
            src.transfer_midas.WindowTransferMidasMain windowTransferMidasMain = new src.transfer_midas.WindowTransferMidasMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonSelUniqueName_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up transfer midas window
            src.select_uniquename.WindowSelUniqueNameMain windowSelUniqueName = new src.select_uniquename.WindowSelUniqueNameMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonEtabsToRhino_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up transfer midas window
            src.etabs_to_rhino.WindowEtabsToRhinoMain windowEtabsToRhinoMain = new src.etabs_to_rhino.WindowEtabsToRhinoMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonLoadCombo_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up load combo window
            src.load_combo_EN.WindowLoadComboMain windowLoadComboMain = new src.load_combo_EN.WindowLoadComboMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonWindCoeff_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up Wind Coefficient Application window
            src.wind_coeff.WindowWindCoeffMain windowWindCoeffMain = new src.wind_coeff.WindowWindCoeffMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonReleaseSelectedNode_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up Release at selected nodes window
            src.release_selected_node.WindowReleaseSelectedNodeMain windowReleaseSelectedNodeMain = new src.release_selected_node.WindowReleaseSelectedNodeMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonHelper_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPadFooting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPunchingShear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonTrfEtabstoSafe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonDeleteFrames_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSteelTrussOptimizer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCircCrack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPreWarning_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
