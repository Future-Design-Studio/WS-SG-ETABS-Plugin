using ETABSv1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WS_SG_ETABS_Plugin.src.plot_reaction
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionMain.xaml
    /// </summary>
    public partial class WindowPlotReactionMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowPlotReactionMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in title
            textBoxTitle.Text = "Reaction Plot";

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            //Fill in combobox for output case
            List<string> outputCasesList = src.HelperEtabs.GetLoadCaseAndComboList(_SapModel);
            comBoxOutputCase.ItemsSource = outputCasesList;
            comBoxOutputCase.SelectedIndex = 0;

            //Fill in combobox for direction
            comBoxReactionDirection.ItemsSource = src.plot_reaction.FunctionPlotReaction.reactionDirectionList;
            comBoxReactionDirection.SelectedIndex = 2;

            ShowDialog();
        }

        #region Button Method

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            //src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
            if (checkBoxDefault.IsChecked == true)
            {
                string selectedString = comBoxGroup.Text + "," + "Auto";
                listBoxChoosenGroup.Items.Add(selectedString);
            }
            else
            {
                string selectedColor = ClrPcker.SelectedColor.ToString();

                string selectedString = comBoxGroup.Text + "," + selectedColor;
                listBoxChoosenGroup.Items.Add(selectedString);
            }
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenGroup.Items.Count != 0)
            {
                listBoxChoosenGroup.Items.RemoveAt(listBoxChoosenGroup.SelectedIndex);
            }
            //src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
            
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
                //Casting listbox content to list
                List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

                /*
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

                string title = textBoxTitle.Text;
                string outputCase = comBoxOutputCase.SelectedValue.ToString();
                string reactionDirection = comBoxReactionDirection.SelectedValue.ToString();
                double forceFactor = Convert.ToDouble(textBoxForceFactor.Text);
                double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
                int textSize = Convert.ToInt32(textBoxTextSize.Text);

                //Splitting x or y
                bool splitX = new bool();
                bool splitY = new bool();
                int divisionX = new int();
                int divisionY = new int();
                double offsetX = new double();
                double offsetY = new double();
                if (radioButtonSplitX.IsChecked == true)
                {
                    splitX = true;
                    splitY = false;
                    divisionX = Convert.ToInt32(textBoxDivisionX.Text);
                    divisionY = 0;
                    offsetX = 0;
                    offsetY = Convert.ToDouble(textBoxOffsetY.Text);
                }
                else if (radioButtonSplitY.IsChecked == true)
                {
                    splitX = false;
                    splitY = true;
                    divisionX = 0;
                    divisionY = Convert.ToInt32(textBoxDivisionX.Text);
                    offsetX = Convert.ToDouble(textBoxOffsetX.Text);
                    offsetY = 0;
                }
                else
                {
                    splitX = false;
                    splitY = false;
                    divisionX = 0;
                    divisionY = 0;
                    offsetX = 0;
                    offsetY = 0;
                }
                

                IsEnabled = false;
                //Call up copy property result window
                src.plot_reaction.WindowPlotReactionResult windowPlotReactionResult = new src.plot_reaction.WindowPlotReactionResult(_SapModel, _Plugin, listChoosenGroup,
                    outputCase, reactionDirection, forceFactor, bubbleScaleFactor, textSize, title, splitX, splitY, divisionX, divisionY, offsetX, offsetY);
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
            textBoxDivisionX.IsEnabled = true;
            textBoxOffsetY.IsEnabled = true;
        }

        private void radioButtonSplitX_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionX.IsEnabled = false;
            textBoxOffsetY.IsEnabled = false;
        }

        private void radioButtonSplitY_Checked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionY.IsEnabled = true;
            textBoxOffsetX.IsEnabled = true;
        }

        private void radioButtonSplitY_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxDivisionY.IsEnabled = false;
            textBoxOffsetX.IsEnabled = false;
        }




        #endregion


    }
}
