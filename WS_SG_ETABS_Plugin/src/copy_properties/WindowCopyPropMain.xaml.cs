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
using Microsoft.Win32;

namespace WS_SG_ETABS_Plugin.src.copy_properties
{
    /// <summary>
    /// Interaction logic for WindowCopyPropMain.xaml
    /// </summary>
    public partial class WindowCopyPropMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;
        bool firstPathAssigned = true;

        public WindowCopyPropMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (modelToLocked)
            {
                MessageBox.Show("ETABS file is locked! Please unlock the model!");
                return;
            }

            //Get model path as a starting directory
            modelPath = SapModel.GetModelFilepath();

            InitializeComponent();

            ShowDialog();
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "ETABS files(*.edb)|*.edb";

            //For first time path assignment, initial directory is where model is. After that use current path as a initial directory
            if (firstPathAssigned)
            {
                openFileDialog.InitialDirectory = modelPath;
            }
            else
            {
                openFileDialog.InitialDirectory = filePath.Text;
            }
            

            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = openFileDialog.FileName;
            }

            //After first time assigning path, set this flag to false
            firstPathAssigned = false;
        }

        #region Button Method

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
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
                {"pDelta", checkBoxPDelta.IsChecked },
                {"loadPattern", checkBoxLoadPattern.IsChecked },
                {"responseSpectrum", checkBoxResponseSpectrum.IsChecked },
                {"loadCase", checkBoxPDelta.IsChecked },
                {"loadCombination", checkBoxPDelta.IsChecked },
            };

            IsEnabled = false;
            //Call up copy property result window
            src.copy_properties.WindowCopyPropResult windowCopyPropResult = new src.copy_properties.WindowCopyPropResult(_SapModel, _Plugin, filePathFrom, selectedItems);
            IsEnabled = true;
        }

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



        #endregion

        private void checkBoxAreaSec_Checked(object sender, RoutedEventArgs e)
        {

        }
    } 

}
