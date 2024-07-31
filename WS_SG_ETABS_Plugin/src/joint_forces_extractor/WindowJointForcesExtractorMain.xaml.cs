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

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    /// <summary>
    /// Interaction logic for WindowJointForcesExtractorMain.xaml
    /// </summary>
    public partial class WindowJointForcesExtractorMain : Window
    {
        string modelPath;
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowJointForcesExtractorMain(cSapModel SapModel, cPluginCallback Plugin)
        {   
            InitializeComponent();
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Fill in default excel result name
            textBoxFileName.Text = "Element_Joint_Forces_Extracted";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Fill in group list box
            src.HelperUI.PopulateListBoxAvailGroup(SapModel, listBoxAvailGroup);

            //Fill in load case list box
            src.HelperUI.PopulateListBoxAvailLoadCase(SapModel, listBoxAvailLoadCase);


            ShowDialog();
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
            src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
        }

        private void buttonRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
        }

        #region Excel path method
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
                textBoxFilePath.Text = dialog.SelectedPath + @"\";
            }

        }
        #endregion

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                if (listBoxChoosenGroup.Items.Count > 0 && listBoxChoosenLoadCase.Items.Count > 0 && textBoxFileName.Text.Length > 0 && textBoxFilePath.Text.Length > 0)
                {
                    //Casting listbox content to list
                    List<string> listChoosenLoadCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                    List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();
                    //Casting file path
                    string filePath = textBoxFilePath.Text;
                    //Casting file name
                    string fileName = textBoxFileName.Text;

                    //Call up window
                    src.joint_forces_extractor.WindowJointForceExtractorLoading windowJointForceExtractorLoading = new src.joint_forces_extractor.WindowJointForceExtractorLoading(_SapModel, _Plugin, listChoosenLoadCase, listChoosenGroup, filePath, fileName);
                }
                else
                {
                    MessageBox.Show("Please select at least 1 group, 1 load case/combination, input file name and input file path.");
                }  
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            IsEnabled = true;
        }
    }
}
