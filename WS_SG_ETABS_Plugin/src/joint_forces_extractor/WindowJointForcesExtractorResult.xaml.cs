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

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    /// <summary>
    /// Interaction logic for WindowJointForcesExtractorResult.xaml
    /// </summary>
    public partial class WindowJointForcesExtractorResult : Window
    {
        public WindowJointForcesExtractorResult(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase, List<string> listChoosenGroup, string filePath)
        {

            //Main logic
            bool isError = src.joint_forces_extractor.FunctionJointForcesExtractorMain.ExtractJointForcesExcel(SapModel, Plugin, databaseTableList, listChoosenLoadCase, listChoosenGroup, filePath);

            if (isError)
            {
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
