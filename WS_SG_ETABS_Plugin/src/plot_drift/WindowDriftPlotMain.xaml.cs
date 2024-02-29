using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    /// <summary>
    /// Interaction logic for WindowDriftPlotMain.xaml
    /// </summary>
    public partial class WindowDriftPlotMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        //List of tables to save joint reaction and grouping table
        List<src.HelperEtabs.DatabaseTable> databaseTableListGlobal = new List<HelperEtabs.DatabaseTable>();

        public WindowDriftPlotMain(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase, List<string> listChoosenGroup)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "drift_plot";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            /*
            //Fill in combobox for output case
            comBoxOutputCase.ItemsSource = listChoosenLoadCase;
            comBoxOutputCase.SelectedIndex = 0;
            */

            //Fill in combobox for direction
            comBoxDirection.ItemsSource = src.HelperEtabs.lateralDeflectionDirectionWithSRSSList;
            comBoxDirection.SelectedIndex = 0;

            //Check the default factor max allowable disp
            radioButtonFactorMaxDisp.IsChecked = true;

            //Check the default
            radioButtonDispXAxisMaxAuto.IsChecked = true;
            radioButtonDriftXAxisMaxAuto.IsChecked = true;

            //Save databaseTableList to global variable
            databaseTableListGlobal = databaseTableList;

            /*
            //Fill in load case list box
            src.HelperUI.PopulateListBoxAvailLoadCase(_SapModel, listBoxAvailLoadCase);
            */
            foreach (string choosenLoadCase in listChoosenLoadCase)
            {
                listBoxAvailLoadCase.Items.Add(choosenLoadCase);
            }

            //Fill in combobox for available groups
            //src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            comBoxGroup.ItemsSource = listChoosenGroup;
            comBoxGroup.SelectedIndex = 0;

            ShowDialog();
        }

        #region Button Method

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            textBoxFilePath.Text = modelPath;
        }
        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailLoadCase, listBoxChoosenLoadCase);
        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenLoadCase, listBoxAvailLoadCase);
        }

        /*
        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailLoadCase, listBoxChoosenLoadCase);
        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenLoadCase, listBoxAvailLoadCase);
        }
        */

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                /*
                //Casting listbox content to list
                List<string> listChoosenLoadCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                */

                string fileName = textBoxFileName.Text;
                string filePath = textBoxFilePath.Text;

                //string outputCase = comBoxOutputCase.SelectedValue.ToString();
                List<string> listOutputCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                string choosenGroup = comBoxGroup.Text;
                string displacementDirection = comBoxDirection.SelectedValue.ToString();
                
                int legendTextSize = Convert.ToInt32(textBoxLegendTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int tickLabelSize = Convert.ToInt32(textBoxTickLabelSize.Text);
                int gridSpacingDisplacementX = Convert.ToInt32(textBoxGridSpacingDisplacementX.Text);
                double gridSpacingDriftX = Convert.ToDouble(textBoxGridSpacingDriftX.Text);
                int gridSpacingY = Convert.ToInt32(textBoxGridSpacingY.Text);

                //Allowable max displacement 
                string maxAllowableDispType = "";
                string maxAllowableDisplacementFactor = textBoxFactorMaxDisp.Text;
                if (radioButtonFactorMaxDisp.IsChecked == true)
                {
                    maxAllowableDispType = "factor";
                    
                }
                else if (radioButtonHideMaxDisp.IsChecked == true)
                {
                    maxAllowableDispType = "hide";
                }


                //Max displacement X Axis
                string displacementXAxisMaxType = "";
                string displacementXAxisMax = textBoxDispXAxisMaxManual.Text;
                if (radioButtonDispXAxisMaxAuto.IsChecked == true)
                {
                    displacementXAxisMaxType = "auto";

                }
                else if (radioButtonDriftXAxisMaxAuto.IsChecked == false)
                {
                    displacementXAxisMaxType = "manual";
                }


                //Max drift X Axis
                string driftXAxisMaxType = "";
                string driftXAxisMax = textBoxDriftXAxisMaxManual.Text;
                if (radioButtonDriftXAxisMaxAuto.IsChecked == true)
                {
                    driftXAxisMaxType = "auto";

                }
                else if (radioButtonDriftXAxisMaxAuto.IsChecked == false)
                {
                    driftXAxisMaxType = "manual";
                }

                //Show max displacement for each z
                bool showMaxDisp = new bool();
                if (checkBoxMaxDisp.IsChecked == true)
                {
                    showMaxDisp = true;
                }
                else
                {
                    showMaxDisp = false;
                }

                //Call up copy result window
                src.plot_drift.WindowDriftPlotResult windowdriftPlotResult = new src.plot_drift.WindowDriftPlotResult(
                    _SapModel, _Plugin, listOutputCase, displacementDirection, maxAllowableDispType, maxAllowableDisplacementFactor, choosenGroup, legendTextSize,
                    titleSize, axisTitleSize, tickLabelSize, gridSpacingDisplacementX, gridSpacingDriftX, gridSpacingY, showMaxDisp, databaseTableListGlobal, fileName, filePath, 
                    displacementXAxisMaxType, displacementXAxisMax, driftXAxisMaxType, driftXAxisMax);
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }


            IsEnabled = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void radioButtonFactorMaxDisp_Checked(object sender, RoutedEventArgs e)
        {
            textBoxFactorMaxDisp.IsEnabled = true;
        }

        private void radioButtonHideMaxDisp_Checked(object sender, RoutedEventArgs e)
        {
            textBoxFactorMaxDisp.IsEnabled = false;
        }



        #endregion

        private void radioButtonDispXAxisMaxAuto_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDispXAxisMaxManual.IsEnabled = false;
        }

        private void radioButtonDispXAxisMaxManual_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDispXAxisMaxManual.IsEnabled = true;
        }

        private void radioButtonDriftXAxisMaxAuto_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDriftXAxisMaxManual.IsEnabled = false;
        }

        private void radioButtonDriftXAxisMaxManual_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDriftXAxisMaxManual.IsEnabled = true;
        }
    }
}
