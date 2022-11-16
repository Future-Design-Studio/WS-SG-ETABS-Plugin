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
            comBoxReactionDirection.SelectedIndex = 0;

            ShowDialog();
        }

        #region Button Method

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            //src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
            if (checkBoxDefault.IsChecked == true)
            {
                string selectedString = comBoxGroup.Text + "," + "Auto" + "," + "Auto";
                listBoxChoosenGroup.Items.Add(selectedString);
            }
            else
            {
                string selectedColor = ClrPcker.SelectedColor.ToString();
                string selectedOpacity = textBoxOpacity.Text;

                string selectedString = comBoxGroup.Text + "," + selectedColor + "," + selectedOpacity;
                listBoxChoosenGroup.Items.Add(selectedString);
            }
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            //src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
            listBoxChoosenGroup.Items.RemoveAt(listBoxChoosenGroup.SelectedIndex);
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
                //Casting listbox content to list of dictionary
                List<Dictionary<string, string>> listChoosenGroup = new List<Dictionary<string, string>>(); 
                
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



                //Casting listbox content to list
                //List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

                string outputCase = comBoxOutputCase.SelectedValue.ToString();
                string reactionDirection = comBoxReactionDirection.SelectedValue.ToString();
                double forceFactor = Convert.ToDouble(textBoxForceFactor.Text);
                double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
                double textSize = Convert.ToDouble(textBoxTextSize.Text);

                IsEnabled = false;
                //Call up copy property result window
                src.plot_reaction.WindowPlotReactionResult windowPlotReactionResult = new src.plot_reaction.WindowPlotReactionResult(_SapModel, _Plugin, listChoosenGroup,
                    outputCase, reactionDirection, forceFactor, bubbleScaleFactor, textSize);
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
            textBoxOpacity.IsEnabled = false;
        }

        private void checkBoxDefault_UnChecked(object sender, RoutedEventArgs e)
        {
            ClrPcker.IsEnabled = true;
            textBoxOpacity.IsEnabled = true;
        }

        private void ClrPcker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }


        #endregion



    }
}
