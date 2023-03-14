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

namespace WS_SG_ETABS_Plugin.src.plot_reaction.table
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionTableMain.xaml
    /// </summary>
    public partial class WindowPlotReactionTableMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        //List of tables to save joint reaction and grouping table
        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<HelperEtabs.DatabaseTable>();

        List<Dictionary<string, string>> groupAndColor = new List<Dictionary<string, string>>();

        public WindowPlotReactionTableMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "reaction";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            //Fill in load case list box
            src.HelperUI.PopulateListBoxAvailLoadCase(_SapModel, listBoxAvailLoadCase);

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModel);

            //Get joint reaction database table
            bool isError = false;
            List<string> tableKeys = new List<string> { "Joint Reactions", "Group Assignments", "Base Reactions" };
            (isError, databaseTableList) = src.HelperEtabs.GetMultipleTableDisplay(SapModel, tableKeys, true);

            ShowDialog();
        }

        #region Button Method

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

        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailLoadCase, listBoxChoosenLoadCase);
        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenLoadCase, listBoxAvailLoadCase);
        }

        private void buttonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            //src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
            if (checkBoxDefault.IsChecked == true)
            {
                string selectedString = comBoxGroup.Text + "," + "Auto";
                listBoxChoosenGroup.Items.Add(selectedString);

                //Add into dictionary
                Dictionary<string, string> eachGroupAndColor = new Dictionary<string, string>
                    {
                        {"groupName",  comBoxGroup.Text},
                        {"color", "Auto"}
                    };

                groupAndColor.Add(eachGroupAndColor);
            }
            else
            {
                string selectedColor = ClrPcker.SelectedColor.ToString();

                if (selectedColor == "")
                {
                    MessageBox.Show("Please pick a color");
                }
                else
                {
                    string selectedString = comBoxGroup.Text + "," + selectedColor;
                    listBoxChoosenGroup.Items.Add(selectedString);

                    //Add into dictionary
                    Dictionary<string, string> eachGroupAndColor = new Dictionary<string, string>
                    {
                        {"groupName",  comBoxGroup.Text},
                        {"color", selectedColor}
                    };

                    groupAndColor.Add(eachGroupAndColor);
                }

            }
        }

        private void buttonRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBoxChoosenGroup.SelectedIndex;

            if (listBoxChoosenGroup.Items.Count != 0 && selectedIndex != -1)
            {
                listBoxChoosenGroup.Items.RemoveAt(selectedIndex);
            }

            groupAndColor.RemoveAt(selectedIndex);
        }

        private void checkBoxDefault_Checked(object sender, RoutedEventArgs e)
        {
            ClrPcker.IsEnabled = false;
        }

        private void checkBoxDefault_UnChecked(object sender, RoutedEventArgs e)
        {
            ClrPcker.IsEnabled = true;
        }

        private void ClrPcker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            
            try
            {
                //Casting listbox content to list
                List<string> listChoosenLoadCase= listBoxChoosenLoadCase.Items.OfType<string>().ToList();

                string fileName = textBoxFileName.Text;
                string filePath = textBoxFilePath.Text;

                string plotTitle = textBoxPlotTitle.Text;
                double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
                int textSize = Convert.ToInt32(textBoxTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int gridSpacingX = Convert.ToInt32(textBoxGridSpacingX.Text);
                int gridSpacingY = Convert.ToInt32(textBoxGridSpacingY.Text);

                bool? showRenumber = radioButtonShowRenumber.IsChecked;

                //Call up copy result window
                src.plot_reaction.table.WindowPlotReactionTableResult windowPlotReactionTableResult = new src.plot_reaction.table.WindowPlotReactionTableResult(_SapModel, _Plugin, groupAndColor, listChoosenLoadCase,
                    plotTitle, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, showRenumber, databaseTableList, fileName, filePath);
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

        #endregion


    }
}
