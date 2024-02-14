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

namespace WS_SG_ETABS_Plugin.src.plot_reaction.graph
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionGraphMain.xaml
    /// </summary>
    public partial class WindowPlotReactionGraphMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<src.HelperEtabs.DatabaseTable> databaseTableListGlobal = new List<src.HelperEtabs.DatabaseTable>();
        List<Dictionary<string, string>> groupAndColorNotAll = new List<Dictionary<string, string>>();

        public WindowPlotReactionGraphMain(cSapModel SapModel, cPluginCallback Plugin, 
            List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase,
            List<string> listChoosenGroup)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in title
            textBoxTitle.Text = "Reaction Plot";

            //Fill in combobox for available groups
            //src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            comBoxGroup.ItemsSource = listChoosenGroup;
            comBoxGroup.SelectedIndex = 0;

            //Fill in combobox for output case
            //List<string> outputCasesList = src.HelperEtabs.GetLoadCaseAndComboList(_SapModel);
            comBoxOutputCase.ItemsSource = listChoosenLoadCase;
            comBoxOutputCase.SelectedIndex = 0;

            //Fill in combobox for direction
            comBoxReactionDirection.ItemsSource = src.HelperEtabs.reactionDirectionList;
            comBoxReactionDirection.SelectedIndex = 2;

            //Fill in combobox for max or min
            List<string> maxOrMinList = new List<string>() {"Max", "Min"};
            comBoxMaxOrMin.ItemsSource = maxOrMinList;
            comBoxMaxOrMin.SelectedIndex = 0;

            databaseTableListGlobal = databaseTableList;

            ShowDialog();
        }

        #region Button Method

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
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

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBoxChoosenGroup.SelectedIndex;

            if (listBoxChoosenGroup.Items.Count != 0 && selectedIndex != -1)
            {
                listBoxChoosenGroup.Items.RemoveAt(selectedIndex);
            }

            groupAndColorNotAll.RemoveAt(selectedIndex);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                /*
                //Casting listbox content to list
                List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

                
                //Casting listbox content to list of dictionary
                List<object> listChoosenGroup = new List<object>(); 
                
                
                foreach (string group in listBoxChoosenGroup.Items)
                {
                    //Split the string using comma ','
                    string[] splitted = group.Split(',');

                    Dictionary<string, string> groupDict = new Dictionary<string, string>()
                    {
                        {"groupName", splitted[0]},
                        {"color", splitted[1]},
                        {"opacity", splitted[2]},
                    };

                    listChoosenGroup.Add(groupDict);
                }
                */

                bool? includeAllJoints = checkBoxAllJoints.IsChecked;

                //groupAndColor container
                List<Dictionary<string, string>> groupAndColor = new List<Dictionary<string, string>>();

                //If include all joints, add one item inside the groupAndColor dictionary
                if (includeAllJoints == true)
                {
                    //groupAndColorAll container
                    List<Dictionary<string, string>> groupAndColorAll = new List<Dictionary<string, string>>();

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


                string title = textBoxTitle.Text;
                string outputCase = comBoxOutputCase.SelectedValue.ToString();
                string reactionDirection = comBoxReactionDirection.SelectedValue.ToString();
                double forceFactor = Convert.ToDouble(textBoxForceFactor.Text);
                string maxOrMin = comBoxMaxOrMin.SelectedValue.ToString();
                double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
                int textSize = Convert.ToInt32(textBoxTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int gridSpacingX = Convert.ToInt32(textBoxGridSpacingX.Text);
                int gridSpacingY = Convert.ToInt32(textBoxGridSpacingY.Text);

                //Splitting x or y
                bool splitX = new bool();
                bool splitY = new bool();
                bool autoSplit = new bool();
                int divisionX = Convert.ToInt32(textBoxDivisionX.Text);
                int divisionY = Convert.ToInt32(textBoxDivisionY.Text);
                double offsetX = Convert.ToDouble(textBoxOffsetX.Text);
                double offsetY = Convert.ToDouble(textBoxOffsetY.Text);
                int sumTextSizeX = Convert.ToInt32(textBoxSumTextSizeX.Text);
                int sumTextSizeY = Convert.ToInt32(textBoxSumTextSizeY.Text);
                double separationX = Convert.ToInt32(textBoxSeparationX.Text);
                double separationY = Convert.ToInt32(textBoxSeparationY.Text);
                List<double> splitCoord = new List<double>();

                if (radioButtonSplitX.IsChecked == true)
                {
                    splitX = true;
                    splitY = false;
                }
                else if (radioButtonSplitY.IsChecked == true)
                {
                    splitX = false;
                    splitY = true;
                }
                else
                {
                    splitX = false;
                    splitY = false;
                }

                if (radioButtonManualSplitX.IsChecked == true || radioButtonManualSplitY.IsChecked == true)
                {
                    autoSplit = false;
                    string coordListToSplit = "";
                    List<string> splitCoordString = new List<string>();

                    if (splitX)
                    {
                        coordListToSplit = textBoxDivisionXCoord.Text;
                    }
                    else if (splitY)
                    {
                        coordListToSplit = textBoxDivisionYCoord.Text;
                    }

                    splitCoordString = coordListToSplit.Split(',').ToList();

                    foreach (string coordString in splitCoordString)
                    {
                        splitCoord.Add(Convert.ToDouble(coordString));  
                    }
                }
                else
                {
                    autoSplit= true;
                }

                IsEnabled = false;
                //Call up copy property result window
                src.plot_reaction.graph.WindowPlotReactionGraphResult windowPlotReactionResult = new src.plot_reaction.graph.WindowPlotReactionGraphResult(_SapModel, _Plugin, groupAndColor,
                    outputCase, reactionDirection, forceFactor, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, title, splitX, splitY, divisionX, divisionY, offsetX, offsetY,
                    sumTextSizeX, sumTextSizeY, separationX, separationY, includeAllJoints, maxOrMin, autoSplit, splitCoord, databaseTableListGlobal);
                IsEnabled = true;
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            IsEnabled = true;
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

        private void checkBoxSum_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonSplitX.IsEnabled = true;
            radioButtonSplitY.IsEnabled = true;

            radioButtonSplitX.IsChecked = true;
        }

        private void checkBoxSum_Unchecked(object sender, RoutedEventArgs e)
        {
            radioButtonSplitX.IsChecked = false;
            radioButtonSplitY.IsChecked = false;

            radioButtonSplitX.IsEnabled = false;
            radioButtonSplitY.IsEnabled = false;
        }

        private void radioButtonSplitX_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonAutoSplitX.IsEnabled = true;
            radioButtonManualSplitX.IsEnabled = true;

            textBoxOffsetX.IsEnabled = true;
            textBoxSumTextSizeX.IsEnabled = true;
            textBoxSeparationX.IsEnabled = true;

            radioButtonAutoSplitX.IsChecked = true;

        }

        private void radioButtonSplitX_Unchecked(object sender, RoutedEventArgs e)
        {
            radioButtonAutoSplitX.IsChecked = false;
            radioButtonManualSplitX.IsChecked = false;

            radioButtonAutoSplitX.IsEnabled = false;
            radioButtonManualSplitX.IsEnabled = false;

            textBoxOffsetX.IsEnabled = false;
            textBoxSumTextSizeX.IsEnabled = false;
            textBoxSeparationX.IsEnabled = false;
        }

        private void radioButtonAutoSplitX_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionX.IsEnabled = true;
        }

        private void radioButtonAutoSplitX_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionX.IsEnabled = false;
        }

        private void radioButtonManualSplitX_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionXCoord.IsEnabled = true;
        }

        private void radioButtonManualSplitX_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionXCoord.IsEnabled = false;
        }

        private void radioButtonSplitY_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonAutoSplitY.IsEnabled = true;
            radioButtonManualSplitY.IsEnabled = true;

            textBoxOffsetY.IsEnabled = true;
            textBoxSumTextSizeY.IsEnabled = true;
            textBoxSeparationY.IsEnabled = true;

            radioButtonAutoSplitY.IsChecked = true;
        }

        private void radioButtonSplitY_Unchecked(object sender, RoutedEventArgs e)
        {
            radioButtonAutoSplitY.IsChecked = false;
            radioButtonManualSplitY.IsChecked = false;

            radioButtonAutoSplitY.IsEnabled = false;
            radioButtonManualSplitY.IsEnabled = false;

            textBoxOffsetY.IsEnabled = false;
            textBoxSumTextSizeY.IsEnabled = false;
            textBoxSeparationY.IsEnabled = false;
        }

        private void radioButtonAutoSplitY_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionY.IsEnabled = true;
        }

        private void radioButtonAutoSplitY_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionY.IsEnabled = false;
        }

        private void radioButtonManualSplitY_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionYCoord.IsEnabled = true;
        }

        private void radioButtonManualSplitY_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionYCoord.IsEnabled = false;
        }

        private void checkBoxAllJoints_Checked(object sender, RoutedEventArgs e)
        {
            comBoxGroup.IsEnabled = false;
            listBoxChoosenGroup.IsEnabled = false;
            buttonRemove.IsEnabled = false;
            buttonAdd.IsEnabled = false;
        }

        private void checkBoxAllJoints_UnChecked(object sender, RoutedEventArgs e)
        {
            comBoxGroup.IsEnabled = true;
            listBoxChoosenGroup.IsEnabled = true;
            buttonRemove.IsEnabled = true;
            buttonAdd.IsEnabled = true;
        }


        #endregion

    }
}
