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

        List<src.HelperEtabs.DatabaseTable> databaseTableListGlobal = new List<src.HelperEtabs.DatabaseTable>();
        List<Dictionary<string, string>> groupAndColorNotAll = new List<Dictionary<string, string>>();

        public WindowPlotReactionTableMain(cSapModel SapModel, cPluginCallback Plugin, 
            List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase,
            List<string> listChoosenGroup)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "Keyplan";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Fill in combobox for available groups
            //src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            comBoxGroup.ItemsSource = listChoosenGroup;
            comBoxGroup.SelectedIndex = 0;

            //Fill in load case list box
            //src.HelperUI.PopulateListBoxAvailLoadCase(_SapModel, listBoxAvailLoadCase);
            foreach (string choosenLoadCase in listChoosenLoadCase)
            {
                listBoxAvailLoadCase.Items.Add(choosenLoadCase);
            }

            databaseTableListGlobal = databaseTableList;

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
            //If there is no group, selectedIndex = -1, then return
            if (comBoxGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a group!");
                return;
            }

            //If group already selected
            foreach (Dictionary<string, string> eachGroupAndColorNotAll in groupAndColorNotAll)
            {
                if (eachGroupAndColorNotAll["groupName"] == comBoxGroup.Text)
                {
                    MessageBox.Show("Group is already selected!");
                    return;
                }
            }

            //src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
            if (checkBoxDefault.IsChecked == true)
            {
                //If there is no group, selectedIndex = -1, then return
                if (comBoxGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a group!");
                    return;
                }

                string selectedString = comBoxGroup.Text + "," + "Auto";
                listBoxChoosenGroup.Items.Add(selectedString);

                //Add into dictionary
                Dictionary<string, string> eachGroupAndColor = new Dictionary<string, string>
                    {
                        {"groupName",  comBoxGroup.Text},
                        {"color", "Auto"}
                    };

                groupAndColorNotAll.Add(eachGroupAndColor);
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

                    groupAndColorNotAll.Add(eachGroupAndColor);
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

            groupAndColorNotAll.RemoveAt(selectedIndex);
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

            //groupAndColor container
            List<Dictionary<string, string>> groupAndColor = new List<Dictionary<string, string>>();

            try
            {

                bool? includeAllJoints = checkBoxAllJoints.IsChecked;

                //groupAndColorAll container
                List<Dictionary<string, string>> groupAndColorAll = new List<Dictionary<string, string>>();

                //If include all joints, add one item inside the groupAndColor dictionary
                if (includeAllJoints == true)
                {
                    //src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
                    if (checkBoxDefault.IsChecked == true)
                    {
                        //Add into dictionary
                        Dictionary<string, string> eachGroupAndColor = new Dictionary<string, string>
                        {
                            {"groupName",  "All"},
                            {"color", "Auto"}
                        };

                        groupAndColorAll.Add(eachGroupAndColor);
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
                            //Add into dictionary
                            Dictionary<string, string> eachGroupAndColor = new Dictionary<string, string>
                            {
                                {"groupName",  "All"},
                                {"color", selectedColor}
                            };

                            groupAndColorAll.Add(eachGroupAndColor);
                        }
                    }

                    groupAndColor = groupAndColorAll;
                }
                else
                {
                    groupAndColor = groupAndColorNotAll;
                }

                
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
                    plotTitle, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, showRenumber, includeAllJoints, databaseTableListGlobal, fileName, filePath);
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

        private void checkBoxAllJoints_Checked(object sender, RoutedEventArgs e)
        {
            comBoxGroup.IsEnabled = false;
            listBoxChoosenGroup.IsEnabled = false;
            buttonRemoveGroup.IsEnabled = false;
            buttonAddGroup.IsEnabled = false;
        }

        private void checkBoxAllJoints_UnChecked(object sender, RoutedEventArgs e)
        {
            comBoxGroup.IsEnabled = true;
            listBoxChoosenGroup.IsEnabled = true;
            buttonRemoveGroup.IsEnabled = true;
            buttonAddGroup.IsEnabled = true;
        }


        #endregion

        private void textBoxFileName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
