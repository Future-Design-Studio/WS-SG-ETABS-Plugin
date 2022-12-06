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

        public WindowTransferMidasMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
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
                src.transfer_midas.WindowTransferMidasResult windowCopyPropResult = new src.transfer_midas.WindowTransferMidasResult(_SapModel, _Plugin, filePathFrom, selectedItems);
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */

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
            src.transfer_midas.WindowTransferMidasResult windowCopyPropResult = new src.transfer_midas.WindowTransferMidasResult(_SapModel, _Plugin, filePathFrom, selectedItems);
            IsEnabled = true;

        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {

        }


        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
