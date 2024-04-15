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
using System.Reflection;
using ETABSv1;
using System.IO;
using System.Data;

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
            string version = "1.2.0f";

            _Plugin = Plugin;
            _SapModel = SapModel;

            src.HelperEtabs.SetOutputUnitTo_kN_m_C(_SapModel);

            /*
            string strComputerName = Environment.MachineName.ToString();

            //Read license number
            try
            {
                StreamReader sr = new StreamReader(@"C:\Program Files\Computers and Structures\ETABS 20\LMHOST.ini");
                string licenseNumber = sr.ReadLine();
                sr.Close();

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
                MessageBox.Show("Illegal license. Your PC ID " + strComputerName + " has been reported to Ramboll IT as well as Singapore Police.");
            }
            */

            
            (bool licenseValid, string[] listofallowedPlugins) = src.HelperLicense.CheckLicense(version);
           
            if (!licenseValid)
            {
                Close();
            }
            else
            {
                InitializeComponent();

                labelVersion.Content = "version " + version;

                if (listofallowedPlugins[0] != "all")
                {
                    List<string> listofButtons = new List<string>
                    {
                        "buttonRCBeamSchedule", "buttonCopyProperties", "buttonPlotReaction", "buttonLoadCombo", "buttonPadFooting", "buttonPunchingShear", "buttonTedds", "buttonRCColSchedule", "buttonPlotDrift", 
                        "buttonPlotSumMassParticip", "buttonWallDesignViewer", "buttonShellUni", "buttonSelUniqueName", "buttonEtabsToRhino", "buttonWindCoeff", "buttonReleaseSelectedNode", "buttonTrfEtabsLoads", 
                        "buttonTrfEtabstoSafe", "buttonCarbon", "buttonTransferMidas", "buttonTrussGrouping", "buttonDeleteFrames", "buttonColOptimizer", "buttonSteelTrussOptimizer", "buttonCircCrack", 
                        "buttonPreWarning", "buttonAddLoadCombo", "buttonApplyModifier", "buttonJointForcesExtractor", "buttonApplySectProp", "buttonCopyElements"
                    };

                    foreach (string buttonName in listofButtons)
                    {
                        object buttonObject = FindName(buttonName);
                        if(buttonObject is Button)
                        {
                            Button buttonButton = (Button)buttonObject;
                            if (listofallowedPlugins.Contains(buttonName))
                            {
                                continue;
                            }
                            else
                            {
                                buttonButton.IsEnabled = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error in list of buttons code, please contact FDS to rectify.");
                            Close();
                        }
                    }
                }
                ShowDialog();

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
            src.plot_reaction.WindowPlotReactionSelectLoad windowPlotReactionSelectLoad = new src.plot_reaction.WindowPlotReactionSelectLoad(_SapModel, _Plugin);
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
            //IsEnabled = false;
            //Call up helper window
            string folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.Start($"{folder}\\src\\resources\\Tools.PDF");
            //IsEnabled = true;
        }

        private void buttonTrfEtabstoSafe_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up Etabs to etabs load transferring open file window
            src.loads_etabs_safe.WindowEtabsSafeOpenFile windowOpenFile = new src.loads_etabs_safe.WindowEtabsSafeOpenFile(_SapModel, _Plugin);
            IsEnabled = true;
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

        private void buttonColOptimizer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonTrfEtabsLoads_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up Etabs to etabs load transferring open file window
            src.loads_etabs_etabs.WindowEtabsEtabsOpenFile windowOpenFile = new src.loads_etabs_etabs.WindowEtabsEtabsOpenFile(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonTrussGrouping_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFeedback_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forms.office.com/e/qaiKf5e3a4");
        }

        private void buttonPunchingShear_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.punching_shear_plot.WindowPunchShrPlotLoading windowPunchShrPlotLoading = new src.punching_shear_plot.WindowPunchShrPlotLoading(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonPadFooting_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.pad_footing_plot.WindowPadFootLoading windowPadFootPlotLoading = new src.pad_footing_plot.WindowPadFootLoading(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonRCColSchedule_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.rc_col_schedule.WindowRCCSchedMain windowRCCSchedMain = new src.rc_col_schedule.WindowRCCSchedMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonSafeCopyProperties_Click(object sender, RoutedEventArgs e)
        {

        }
        private void buttonTrfTedds_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.tedds_main.WindowTeddsMain windowTeddsMain = new src.tedds_main.WindowTeddsMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonPlotDrift_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.plot_drift.WindowDriftPlotSelectLoad windowDriftPlotSelectLoad = new src.plot_drift.WindowDriftPlotSelectLoad(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonPlotSumMassParticip_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.plot_sum_mass_participation.WindowSumParticipPlotMain windowSumParticipPlotMain = new src.plot_sum_mass_participation.WindowSumParticipPlotMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonWallDesignViewer_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.wall_design_view.WindowWallDesignSelectLoad windowWallDesignSelectLoad = new src.wall_design_view.WindowWallDesignSelectLoad(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonAddLoadCombo_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.add_load_combo.WindowAddLoadComboMain windowAddLoadComboMain = new src.add_load_combo.WindowAddLoadComboMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonApplyModifier_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.apply_modifier.WindowApplyModifierMain windowApplyModifierMain = new src.apply_modifier.WindowApplyModifierMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonApplySectProp_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.apply_section_property.WindowApplySectPropMain windowApplySectPropMain = new src.apply_section_property.WindowApplySectPropMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonCopyElements_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.copy_elements.WindowCopyElementsMain windowCopyElementsMain = new src.copy_elements.WindowCopyElementsMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonJointForcesExtractor_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up window
            src.joint_forces_extractor.WindowJointForcesExtractorMain windowJointForcesExtractorMain = new src.joint_forces_extractor.WindowJointForcesExtractorMain(_SapModel, _Plugin);
            IsEnabled = true;
        }
    }
}
