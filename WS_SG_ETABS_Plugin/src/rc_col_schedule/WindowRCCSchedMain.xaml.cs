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

namespace WS_SG_ETABS_Plugin.src.rc_col_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCCSchedMain.xaml
    /// </summary>
    public partial class WindowRCCSchedMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        List<string> groupList = new List<string>();

        public WindowRCCSchedMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }

            InitializeComponent();

            //Fill in default excel result name
            fileName.Text = "rc_column_schedule";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            filePath.Text = modelPath;

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            //Default check the rebar preference
            radioButtonSpcAuto.IsChecked = true;

            ShowDialog();
        }

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            filePath.Text = modelPath;
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = filePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = dialog.SelectedPath;
            }

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = fileName.Text;
                string path = filePath.Text;

                double cover = Convert.ToDouble(textBoxCover.Text);
                int maxNoLayer = Convert.ToInt32(textBoxMaxNoLayer.Text);
                double maxShearLinksSpacing = Convert.ToDouble(textBoxMaxShearLinksSpacing.Text);
                double minShearLinksSpacing = Convert.ToDouble(textBoxMinShearLinksSpacing.Text);
                double shearLinksSpacingStep = Convert.ToDouble(textBoxShearLinksSpacingStep.Text);

                string[] rebarDiameterListInString = textBoxRebarDiameterList.Text.Split(',');
                int[] rebarDiameterList = Array.ConvertAll(rebarDiameterListInString, int.Parse);

                string[] shearLinksDiameterListInString = textBoxShearLinksDiameterList.Text.Split(',');
                int[] shearLinksDiameterList = Array.ConvertAll(shearLinksDiameterListInString, int.Parse);

                //Getting rebar spc preference
                double rebarSpc = 0;
                string rebarSpcPreference = "";
                if (radioButtonSpcClearSpc.IsChecked == true)
                {
                    rebarSpc = Convert.ToDouble(textBoxSpcClearSpc.Text);
                    rebarSpcPreference = "clear";
                }
                else if (radioButtonSpcCToC.IsChecked == true)
                {
                    rebarSpc = Convert.ToDouble(textBoxSpcCToC.Text);
                    rebarSpcPreference = "ctoc";
                }
                else if (radioButtonSpcAuto.IsChecked == true)
                {
                    rebarSpc = 0;
                    rebarSpcPreference = "auto";
                }

                IsEnabled = false;
                //Call up carbon result window
                src.rc_col_schedule.WindowRCCSchedResult windowRCCSchedResult = new WindowRCCSchedResult(_SapModel, _Plugin, name, path,
                    rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, maxNoLayer, rebarDiameterList, shearLinksDiameterList, groupList);
                IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void radioButtonSpcClearSpc_Checked(object sender, RoutedEventArgs e)
        {
            textBoxSpcClearSpc.IsEnabled = true;
            textBoxSpcCToC.IsEnabled = false;
        }

        private void radioButtonSpcCToC_Checked(object sender, RoutedEventArgs e)
        {
            textBoxSpcCToC.IsEnabled = true;
            textBoxSpcClearSpc.IsEnabled = false;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            //If there is no group, selectedIndex = -1, then return
            if (comBoxGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a group!");
                return;
            }

            //Check if group already chosen
            foreach (string eachGroup in groupList)
            {
                if (eachGroup == comBoxGroup.Text)
                {
                    MessageBox.Show($"Group {comBoxGroup.Text} is already selected");
                    return;
                }
            }

            string selectedString = comBoxGroup.Text;
            listBoxChoosenGroup.Items.Add(selectedString);

            groupList.Add(comBoxGroup.Text);
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBoxChoosenGroup.SelectedIndex;

            if (listBoxChoosenGroup.Items.Count != 0 && selectedIndex != -1)
            {
                listBoxChoosenGroup.Items.RemoveAt(selectedIndex);
                groupList.RemoveAt(selectedIndex);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
