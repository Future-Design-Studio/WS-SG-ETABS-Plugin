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

        List<ClassReleaseSelectedNode.ReleaseDetails> releaseList = new List<ClassReleaseSelectedNode.ReleaseDetails>();

        List<string> ReleaseType = new List<string>()
        {
            "Axial", "ShearMajor", "ShearMinor", "Torsion", "MomentMinor", "MomentMajor",
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

            //Fill in combobox frames for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroupsFrames);
            comBoxGroupsFrames.SelectedIndex = 0;

            //Fill in combobox joints for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroupsJoints);
            comBoxGroupsJoints.SelectedIndex = 0;

            ShowDialog();
        }

        private void Button_ADD_Releases_Click(object sender, RoutedEventArgs e)
        {
            string groupNameFrame = comBoxGroupsFrames.Text;
            string groupNameJoint = comBoxGroupsJoints.Text;
            List<string> releaseType = new List<string>();
            if (checkBoxAxial.IsChecked == true)
            {
                releaseType.Add(ReleaseType[0]);
            }
            if (checkBoxShearMajor.IsChecked == true)
            {
                releaseType.Add(ReleaseType[1]);
            }
            if (checkBoxShearMinor.IsChecked == true)
            {
                releaseType.Add(ReleaseType[2]);
            }
            if (checkBoxTorsion.IsChecked == true)
            {
                releaseType.Add(ReleaseType[3]);
            }
            if (checkBoxMomentMinor.IsChecked == true)
            {
                releaseType.Add(ReleaseType[4]);
            }
            if (checkBoxMomentMajor.IsChecked == true)
            {
                releaseType.Add(ReleaseType[5]);
            }
            
            //Check if at least 1 checkbox selected
            if (checkBoxAxial.IsChecked == true || checkBoxShearMajor.IsChecked == true || checkBoxShearMinor.IsChecked == true || checkBoxTorsion.IsChecked == true || checkBoxMomentMinor.IsChecked == true || checkBoxMomentMajor.IsChecked == true)
            {
                if (releaseType.Count == 1)
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }
                else if (releaseType.Count == 2)
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ", " + releaseType[1] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }
                else if (releaseType.Count == 3)
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ", " + releaseType[1] + ", " + releaseType[2] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }
                else if (releaseType.Count == 4)
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ", " + releaseType[1] + ", " + releaseType[2] + ", " + releaseType[3] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }
                else if (releaseType.Count == 5)
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ", " + releaseType[1] + ", " + releaseType[2] + ", " + releaseType[3] + ", " + releaseType[4] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }
                else
                {
                    listBoxChoosenReleases.Items.Add("Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + " for " + releaseType[0] + ", " + releaseType[1] + ", " + releaseType[2] + ", " + releaseType[3] + ", " + releaseType[4] + ", " + releaseType[5] + ".");
                    ClassReleaseSelectedNode.ReleaseDetails eachRelease = new ClassReleaseSelectedNode.ReleaseDetails(groupNameFrame, groupNameJoint, releaseType);
                    releaseList.Add(eachRelease);
                }


            }
            else
            {
                MessageBox.Show("Please select at least 1 type of release before adding.");
            }

        }

        private void Button_REMOVE_Releases_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenReleases.Items.Count != 0 && listBoxChoosenReleases.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenReleases.SelectedIndex;

                listBoxChoosenReleases.Items.RemoveAt(selectedIndex);

                releaseList.RemoveAt(selectedIndex);
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
            src.release_selected_node.WindowReleaseSelectedNodeResult windowReleaseSelectedResult = new src.release_selected_node.WindowReleaseSelectedNodeResult(_SapModel, _Plugin, releaseList);
            IsEnabled = true;
        }

        private void checkBoxAxial_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxShearMajor_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxShearMinor_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxTorsion_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxMomentMinor_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxMomentMajor_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxAxial_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxShearMajor_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxShearMinor_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxTorsion_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxMomentMinor_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxMomentMajor_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
