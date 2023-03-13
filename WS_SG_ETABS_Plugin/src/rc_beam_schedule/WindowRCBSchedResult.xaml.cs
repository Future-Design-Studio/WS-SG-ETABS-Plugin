using ETABSv1;
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

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedResult.xaml
    /// </summary>
    public partial class WindowRCBSchedResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowRCBSchedResult(cSapModel SapModel, cPluginCallback Plugin, string fileName, string filePath, double leftLength, double midLength, double rightLength, double cantileverLeftLength, double cantileverRightLength,
            double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int[] rebarDiameterList, int[] shearLinksDiameterList, List<string> listChoosenGroup)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen
            bool requiredGroupListExist = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroup);

            if (!requiredGroupListExist)
            {
                return;
            }

            //Main logic
            bool isError = src.rc_beam_schedule.FunctionRCBSched.GetRCBeamSchedule(_SapModel, fileName, filePath, leftLength, midLength, rightLength, cantileverLeftLength, cantileverRightLength, 
                rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, rebarDiameterList, shearLinksDiameterList, listChoosenGroup);

            if (isError)
            {
                return;
            }

            InitializeComponent();

            //Put ShowDialog at last after completing constructor
            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
