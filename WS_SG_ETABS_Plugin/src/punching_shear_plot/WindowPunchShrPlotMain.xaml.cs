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

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    /// <summary>
    /// Interaction logic for WindowPunchShrPlotMain.xaml
    /// </summary>
    public partial class WindowPunchShrPlotMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        //List of tables to save joint reaction and grouping table
        List<src.HelperEtabs.DatabaseTable> databaseTableListGlobal = new List<HelperEtabs.DatabaseTable>();

        public WindowPunchShrPlotMain(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "punching_shear";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Save databaseTableList to global variable
            databaseTableListGlobal = databaseTableList;

            /*
            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModel);

            //Get joint reaction database table
            bool isError = false;
            List<string> tableKeys = new List<string> { "Joint Reactions", "Group Assignments", "Element Forces - Columns", "Frame Assignments - Section Properties"};
            (isError, databaseTableList) = src.HelperEtabs.GetMultipleTableDisplay(SapModel, tableKeys, false);

            if (isError)
            {
                MessageBox.Show("Error in retreiving column forces and joint reactions. Please make sure the model is run!");
                return;
            }
            */

            //Fill in load case list box
            src.HelperUI.PopulateListBoxAvailLoadCase(_SapModel, listBoxAvailLoadCase);

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            ShowDialog();
        }

        #region Button Method
        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailLoadCase, listBoxChoosenLoadCase);
        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenLoadCase, listBoxAvailLoadCase);
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                //Casting listbox content to list
                List<string> listChoosenLoadCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();

                string fileName = textBoxFileName.Text;
                string filePath = textBoxFilePath.Text;

                string choosenGroup = comBoxGroup.Text;

                double forceIncrement = Convert.ToDouble(textBoxForceIncrement.Text);

                string plotTitle = textBoxTitle.Text;
                double forceFactor = Convert.ToDouble(textBoxForceFactor.Text);
                double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
                int textSize = Convert.ToInt32(textBoxTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int gridSpacingX = Convert.ToInt32(textBoxGridSpacingX.Text);
                int gridSpacingY = Convert.ToInt32(textBoxGridSpacingY.Text);

                //Call up copy result window
                src.punching_shear_plot.WindowPunchShrPlotResult windowPlotReactionTableResult = new src.punching_shear_plot.WindowPunchShrPlotResult(
                    _SapModel, _Plugin, listChoosenLoadCase, choosenGroup, forceIncrement, plotTitle, forceFactor, bubbleScaleFactor, textSize, titleSize, 
                    axisTitleSize, gridSpacingX, gridSpacingY, databaseTableListGlobal, filePath, fileName);
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

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            textBoxFilePath.Text = modelPath;
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


        #endregion


    }
}
