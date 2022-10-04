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

            InitializeComponent();

            //test.Test.PrintAllDatabaseTable(_SapModel);
            //test.Test.PrintTableDisplay(_SapModel, "Material List by Section Property");
            //test.Test.CrossProductTesting(-3, 1, 0, -1, 0, 3);

            ShowDialog();

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

        private void buttonRCBeamSchedule_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up shell uniform window
            src.rc_beam_schedule.WindowRCBSchedMain windowShellUniMain = new src.rc_beam_schedule.WindowRCBSchedMain(_SapModel, _Plugin);
            IsEnabled = true;
        }
    }
}
