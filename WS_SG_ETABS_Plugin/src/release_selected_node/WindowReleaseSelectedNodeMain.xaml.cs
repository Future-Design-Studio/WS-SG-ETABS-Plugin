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

namespace WS_SG_ETABS_Plugin.src.release_selected_node
{
    /// <summary>
    /// Interaction logic for WindowReleaseSelectedNodeMain.xaml
    /// </summary>
    public partial class WindowReleaseSelectedNodeMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        #region Data source


        List<string> directionList = new List<string>()
        {
            "Windward", "Leeward"
        };

        #endregion
        public WindowReleaseSelectedNodeMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            InitializeComponent();
            //Check if model is locked
            bool modelFromLocked = SapModel.GetModelIsLocked();

            if (modelFromLocked)
            {
                MessageBox.Show("ETABS file is locked! Please unlock the model!");
                return;
            }

            //Fill in combobox for existing patterns
            List<string> outputPatternsList = src.HelperEtabs.GetAllLoadPattern(_SapModel);

            comBoxLoadPattern.ItemsSource = outputPatternsList;
            comBoxLoadPattern.SelectedIndex = 0;

            comBoxDirection.ItemsSource = directionList;
            comBoxDirection.SelectedIndex = 0;

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroups);
            comBoxGroups.SelectedIndex = 0;

            ShowDialog();
        }

        private void Button_ADD_Wind_Click(object sender, RoutedEventArgs e)
        {
            string groupName = comBoxGroups.Text;
            string loadPattern = comBoxLoadPattern.Text;
            int direction = new int();
            if (comBoxDirection.SelectedIndex == 0)
            {
                direction = 1;
            }
            else
            {
                direction = 2;
            }

            double cp = 0;
            //Check if wind coefficient is filled
            if (textBoxCP.Text != "")
            {
                cp = double.Parse(textBoxCP.Text);

                listBoxChoosenWind.Items.Add(groupName + ", " + loadPattern + ", " + comBoxDirection.Text + ", " + textBoxCP.Text);
            }
            else
            {
                MessageBox.Show("Please input Cp!");
            }

        }

        private void Button_REMOVE_Wind_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenWind.Items.Count != 0 && listBoxChoosenWind.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenWind.SelectedIndex;

                listBoxChoosenWind.Items.RemoveAt(selectedIndex);
            }
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up result window
            src.release_selected_node.WindowReleaseSelectedNodeResult windowReleaseSelectedResult = new src.release_selected_node.WindowReleaseSelectedNodeResult(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void textBoxCP_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
