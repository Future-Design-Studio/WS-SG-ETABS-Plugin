using System;
using System.Collections.Generic;
using System.IO;
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

namespace WS_SG_ETABS_Plugin.src.rc_col_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCCSchedResult.xaml
    /// </summary>
    public partial class WindowRCCSchedResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowRCCSchedResult(cSapModel SapModel, cPluginCallback Plugin, string fileName, string filePath, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, 
            double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int maxNoLayer, int[] rebarDiameterList, int[] shearLinksDiameterList,
            List<string> listChoosenGroupName)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen
            bool requiredGroupListExist = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroupName);

            if (!requiredGroupListExist)
            {
                return;
            }

            //Main logic
            bool isError = src.rc_col_schedule.FunctionRCCSched.GetRCColumnSchedule(_SapModel, _Plugin, fileName, filePath,
                    rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, maxNoLayer, rebarDiameterList, shearLinksDiameterList, listChoosenGroupName);

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
