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
using WS_SG_ETABS_Plugin.src.wind_coeff;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    /// <summary>
    /// Interaction logic for WindowEtabsEtabsMain.xaml
    /// </summary>
    public partial class WindowEtabsEtabsMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        List<src.HelperEtabs.DatabaseTable> databaseTableList;
        src.HelperEtabs.DatabaseTable groupdefinitionTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable pointobjectconnectivityTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable jointreactionTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable groupassignmentTable = new HelperEtabs.DatabaseTable();
        List<string> loadcasefromList = new List<string>();
        List<string> jointsindefinedgroupList = new List<string>();
        List<string> groupnameList = new List<string>();
        bool isError = false;
        public WindowEtabsEtabsMain(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            databaseTableList = DatabaseTableList;
            InitializeComponent();

            //Fill in combobox for existing DL case
            List<string> outputCasesList = src.HelperEtabs.GetAllLoadPattern(_SapModel);

            comBoxLoadPattern.ItemsSource = outputCasesList;
            comBoxLoadPattern.SelectedIndex = 0;

            (isError, pointobjectconnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError)
            {
                return;
            }

            (isError, jointreactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Reactions");
            if (isError)
            {
                return;
            }
            else
            {
                foreach (Dictionary<string, string> load in jointreactionTable.TableInList)
                {
                    if (!loadcasefromList.Contains(load["OutputCase"]))
                    {
                        loadcasefromList.Add(load["OutputCase"]);
                    }
                }

                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;
            }

            (isError, groupassignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError)
            {
                comBoxGroups.IsEnabled = false;
                checkBoxUseAllJoints.IsChecked = true;
                checkBoxUseAllJoints.IsEnabled = false;
                listBoxChoosenGroup.Items.Add("All available Joint Reactions will be transferred.");
                Button_ADD_Group.IsEnabled = false;
                Button_REMOVE_Group.IsEnabled = false;
            }
            else
            {
                //Filter group to only include points, to improve performance
                (isError, groupassignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupassignmentTable, "ObjectType", "Point");
                if (isError)
                {
                    return;
                }

                (isError, groupdefinitionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Definitions");
                if (isError)
                {
                    return;
                }
                else
                {
                    foreach (Dictionary<string, string> group in groupdefinitionTable.TableInList)
                    {
                        if (group["Name"] != "All")
                        {
                            groupnameList.Add(group["Name"]);
                        }
                    }


                    comBoxGroups.ItemsSource = groupnameList;
                    comBoxGroups.SelectedIndex = 0;
                }
            }

            ShowDialog();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        List<string> selectedGroup = new List<string>();
        List<ClassEtabsEtabs.CaseandPatternMatch> casePatternList = new List<ClassEtabsEtabs.CaseandPatternMatch>();

        private void comBoxLoadCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxLoadPattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxCreateNew_Checked(object sender, RoutedEventArgs e)
        {
            comBoxLoadPattern.IsEnabled = false;
        }

        private void checkBoxCreateNew_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxLoadPattern.IsEnabled = true;
        }

        private void listBoxCasePatternMatching_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_ADD_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxCreateNew.IsChecked == true)
            {
                ClassEtabsEtabs.CaseandPatternMatch eachCasePattern = new ClassEtabsEtabs.CaseandPatternMatch(comBoxLoadCase.Text, comBoxLoadCase.Text + "_transferred", true);

                casePatternList.Add(eachCasePattern);
                listBoxCasePatternMatching.Items.Add("Load from " + comBoxLoadCase.Text + " assigned to " + comBoxLoadCase.Text + "_transferred.");

                loadcasefromList.RemoveAt(comBoxLoadCase.SelectedIndex);

                comBoxLoadCase.ClearValue(ItemsControl.ItemsSourceProperty);
                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;
            }
            else
            {
                ClassEtabsEtabs.CaseandPatternMatch eachCasePattern = new ClassEtabsEtabs.CaseandPatternMatch(comBoxLoadCase.Text, comBoxLoadPattern.Text, false);

                casePatternList.Add(eachCasePattern);
                listBoxCasePatternMatching.Items.Add("Load from " + comBoxLoadCase.Text + " assigned to " + comBoxLoadPattern.Text + ".");

                loadcasefromList.RemoveAt(comBoxLoadCase.SelectedIndex);

                comBoxLoadCase.ClearValue(ItemsControl.ItemsSourceProperty);
                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;
            }
            
        }

        private void Button_REMOVE_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCasePatternMatching.Items.Count != 0 && listBoxCasePatternMatching.SelectedIndex != -1)
            {
                int selectedIndex = listBoxCasePatternMatching.SelectedIndex;


                loadcasefromList.Add(casePatternList[selectedIndex].LoadCaseFrom);

                comBoxLoadCase.ClearValue(ItemsControl.ItemsSourceProperty);
                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;

                listBoxCasePatternMatching.Items.RemoveAt(selectedIndex);
                casePatternList.RemoveAt(selectedIndex);
            }
        }

        private void comBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxUseAllJoints_Checked(object sender, RoutedEventArgs e)
        {
            comBoxGroups.IsEnabled = false;
        }

        private void checkBoxUseAllJoints_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxGroups.IsEnabled = true;
        }

        private void listBoxChoosenGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_ADD_Group_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxUseAllJoints.IsChecked == true)
            {
                listBoxChoosenGroup.Items.Clear();
                selectedGroup.Clear();

                listBoxChoosenGroup.Items.Add("Transfer all available joint reactions.");
                selectedGroup.Add("All");

                Button_ADD_Group.IsEnabled = false;
            }
            else
            {
                listBoxChoosenGroup.Items.Add(comBoxGroups.Text);
                selectedGroup.Add(comBoxGroups.Text);
            }
        }

        private void Button_REMOVE_Group_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenGroup.Items.Count != 0 && listBoxChoosenGroup.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenGroup.SelectedIndex;

                listBoxChoosenGroup.Items.RemoveAt(selectedIndex);
                selectedGroup.RemoveAt(selectedIndex);

                Button_ADD_Group.IsEnabled = true;
            }
        }




        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up copy property result window
            src.loads_etabs_etabs.WindowEtabsEtabsResult windowEtabsEtabsResult = new src.loads_etabs_etabs.WindowEtabsEtabsResult(_SapModel, _Plugin, databaseTableList, selectedGroup, casePatternList, groupdefinitionTable, pointobjectconnectivityTable, jointreactionTable, groupassignmentTable);
            IsEnabled = true;
        }
    }
}
    
