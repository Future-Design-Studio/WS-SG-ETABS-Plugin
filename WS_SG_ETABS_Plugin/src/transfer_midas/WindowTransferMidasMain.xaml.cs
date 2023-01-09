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

namespace WS_SG_ETABS_Plugin.src.transfer_midas
{
    /// <summary>
    /// Interaction logic for WindowTransferMidasMain.xaml
    /// </summary>
    public partial class WindowTransferMidasMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        string modelPath = "";
        string modelName = "";

        public WindowTransferMidasMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Fill in model name (default path)
            modelName = SapModel.GetModelFilename(false);
            textBoxFileName.Text = modelName;

            ShowDialog();
        }


        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = textBoxFilePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFilePath.Text = dialog.SelectedPath;
            }

        }

        private void buttonResetPath_Click(object sender, RoutedEventArgs e)
        {
            //Fill in model path (default path)
            textBoxFilePath.Text = modelPath;

            //Fill in model name (default path)
            textBoxFileName.Text = modelName;

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                //Casting file path
                string filePath = textBoxFilePath.Text;
                string fileName = textBoxFileName.Text;

                //Collect which boxes are selected
                Dictionary<string, bool?> selectedItems = new Dictionary<string, bool?>()
                {
                    {"material", checkBoxMaterial.IsChecked },
                    {"frameSec", checkBoxFrameSec.IsChecked },
                    {"areaSec", checkBoxAreaSec.IsChecked },
                    //{"massSource", checkBoxMassSource.IsChecked },
                    {"loadPattern", checkBoxLoadPattern.IsChecked },
                    {"pDelta", checkBoxPDelta.IsChecked },
                    //{"responseSpectrum", checkBoxResponseSpectrum.IsChecked },
                    {"loadCase", checkBoxLoadCase.IsChecked },
                    {"loadCombination", checkBoxLoadCombination.IsChecked },
                    {"objectConnectivity", checkBoxObjectConnectivity.IsChecked },
                    {"jointAssignments", checkBoxJointAssignments.IsChecked },
                    {"frameAssignments", checkBoxFrameAssignments.IsChecked },
                    {"areaAssignments", checkBoxAreaAssignments.IsChecked },
                    {"jointLoadsAssignments", checkBoxJointLoadsAssignments.IsChecked },
                    {"frameLoadsAssignments", checkBoxFrameLoadsAssignments.IsChecked },
                    {"areaLoadsAssignments", checkBoxAreaLoadsAssignments.IsChecked },
                    {"links", checkBoxLinks.IsChecked },
                    {"groups", checkBoxGroups.IsChecked },
                    {"story", checkBoxStory.IsChecked },
                };


                IsEnabled = false;
                //Call up result window
                src.transfer_midas.WindowTransferMidasResult windowCopyPropResult = new src.transfer_midas.WindowTransferMidasResult(_SapModel, _Plugin, selectedItems, filePath, fileName);
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            
            /*
            //Casting file path
            string filePathFrom = filePath.Text;

            //Collect which boxes are selected
            Dictionary<string, bool?> selectedItems = new Dictionary<string, bool?>()
                {
                    {"material", checkBoxMaterial.IsChecked },
                    {"frameSec", checkBoxFrameSec.IsChecked },
                    {"areaSec", checkBoxAreaSec.IsChecked },
                    {"linkSec", checkBoxLinkSec.IsChecked },
                    {"massSource", checkBoxMassSource.IsChecked },
                    {"loadPattern", checkBoxLoadPattern.IsChecked },
                    {"pDelta", checkBoxPDelta.IsChecked },
                    {"responseSpectrum", checkBoxResponseSpectrum.IsChecked },
                    {"loadCase", checkBoxPDelta.IsChecked },
                    {"loadCombination", checkBoxPDelta.IsChecked },
                };

            IsEnabled = false;
            //Call up copy property result window
            src.transfer_midas.WindowTransferMidasResult windowCopyPropResult = new src.transfer_midas.WindowTransferMidasResult(_SapModel, _Plugin, filePathFrom, selectedItems);
            IsEnabled = true;
            */
            
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*

        private void checkBoxMaterial_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxFrameSec.IsEnabled = true;
            checkBoxAreaSec.IsEnabled = true;
        }

        private void checkBoxMaterial_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxFrameSec.IsEnabled = false;
            checkBoxFrameSec.IsChecked = false;

            checkBoxAreaSec.IsEnabled = false;
            checkBoxAreaSec.IsChecked = false;
        }

        private void checkBoxLoadCase_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxLoadCombination.IsEnabled = true;
        }

        private void checkBoxLoadCase_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxLoadCombination.IsEnabled = false;
            checkBoxLoadCombination.IsChecked = false;
        }

        private void buttonUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            checkBoxMaterial.IsChecked = false;
            checkBoxFrameSec.IsChecked = false;
            checkBoxAreaSec.IsChecked = false;
            checkBoxLinkSec.IsChecked = false;

            checkBoxMassSource.IsChecked = false;
            checkBoxPDelta.IsChecked = false;

            checkBoxLoadPattern.IsChecked = false;
            checkBoxResponseSpectrum.IsChecked = false;

            checkBoxLoadCase.IsChecked = false;
            checkBoxLoadCombination.IsChecked = false;
        }

        private void buttonCheckAll_Click(object sender, RoutedEventArgs e)
        {
            checkBoxMaterial.IsChecked = true;
            checkBoxFrameSec.IsChecked = true;
            checkBoxAreaSec.IsChecked = true;
            checkBoxLinkSec.IsChecked = true;

            checkBoxMassSource.IsChecked = true;
            checkBoxPDelta.IsChecked = true;

            checkBoxLoadPattern.IsChecked = true;
            checkBoxResponseSpectrum.IsChecked = true;

            checkBoxLoadCase.IsChecked = true;
            checkBoxLoadCombination.IsChecked = true;
        }

        private void checkBoxLoadPattern_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxLoadCase.IsEnabled = true;
        }

        private void checkBoxLoadPattern_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxLoadCase.IsEnabled = false;
            checkBoxLoadCase.IsChecked = false;
        }
        */
    }
}
