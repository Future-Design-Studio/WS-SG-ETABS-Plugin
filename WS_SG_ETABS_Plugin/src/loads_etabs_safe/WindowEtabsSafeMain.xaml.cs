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
using SAFEv1;
using WS_SG_ETABS_Plugin.src.loads_etabs_etabs;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_safe
{
    /// <summary>
    /// Interaction logic for WindowEtabsSafeMain.xaml
    /// </summary>
    public partial class WindowEtabsSafeMain : Window
    {
        ETABSv1.cSapModel _SapModelETABS;
        SAFEv1.cSapModel _SapModelSAFE;
        ETABSv1.cPluginCallback _PluginETABS;
        List<src.HelperEtabs.DatabaseTable> databaseTableListETABS;
        src.HelperEtabs.DatabaseTable pointobjectconnectivityTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable jointreactionTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable storydefinitionTable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable jointforcesFrametable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable jointforcesShelltable = new HelperEtabs.DatabaseTable();
        src.HelperEtabs.DatabaseTable jointforcesLinktable = new HelperEtabs.DatabaseTable();
        List<string> loadcasefromList = new List<string>();
        List<string> jointsindefinedgroupList = new List<string>();
        List<string> groupnameList = new List<string>();
        bool isError = false;
        public WindowEtabsSafeMain(ETABSv1.cSapModel SapModelETABS, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginETABS, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS)
        {
            _SapModelETABS = SapModelETABS;
            _SapModelSAFE = SapModelSAFE;
            _PluginETABS = PluginETABS;
            databaseTableListETABS = DatabaseTableListETABS;
            InitializeComponent();

            //Fill in combobox for existing DL case
            List<string> outputCasesList = src.HelperSafe.GetAllLoadPattern(_SapModelSAFE);

            comBoxLoadPattern.ItemsSource = outputCasesList;
            comBoxLoadPattern.SelectedIndex = 0;

            (isError, pointobjectconnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableListETABS, "Point Object Connectivity");
            if (isError)
            {
                return;
            }

            (isError, storydefinitionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableListETABS, "Story Definitions");
            if (isError)
            {
                return;
            }

            string storyabove = "";
            int storeyloop = 0;
            foreach (Dictionary<string, string> row in storydefinitionTable.TableInList)
            {
                if (row["Name"])
            }

            (isError, jointreactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableListETABS, "Joint Reactions");
            if (isError)
            {
                return;
            }
            else
            {
                foreach (Dictionary<string, string> load in jointreactionTable.TableInList)
                {
                    if (load["StepNumber"] != null)
                    {
                        if (!loadcasefromList.Contains(load["OutputCase"] + "_Step_" + load["StepNumber"]))
                        {
                            loadcasefromList.Add(load["OutputCase"] + "_Step_" + load["StepNumber"]);
                        }
                    }
                    else
                    {
                        if (!loadcasefromList.Contains(load["OutputCase"]))
                        {
                            loadcasefromList.Add(load["OutputCase"]);
                        }
                    }
        
                    
                }

                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;
            }

            ShowDialog();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        List<string> selectedGroup = new List<string>();
        List<ClassEtabsSafe.CaseandPatternMatch> casePatternList = new List<ClassEtabsSafe.CaseandPatternMatch>();

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
                ClassEtabsSafe.CaseandPatternMatch eachCasePattern = new ClassEtabsSafe.CaseandPatternMatch(comBoxLoadCase.Text, comBoxLoadCase.Text + "_transferred", true);

                casePatternList.Add(eachCasePattern);
                listBoxCasePatternMatching.Items.Add("Load from " + comBoxLoadCase.Text + " assigned to " + comBoxLoadCase.Text + "_transferred.");

                loadcasefromList.RemoveAt(comBoxLoadCase.SelectedIndex);

                comBoxLoadCase.ClearValue(ItemsControl.ItemsSourceProperty);
                comBoxLoadCase.ItemsSource = loadcasefromList;
                comBoxLoadCase.SelectedIndex = 0;
            }
            else
            {
                ClassEtabsSafe.CaseandPatternMatch eachCasePattern = new ClassEtabsSafe.CaseandPatternMatch(comBoxLoadCase.Text, comBoxLoadPattern.Text, false);

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
            listBoxChoosenGroup.Items.Add(comBoxGroups.Text);   
            selectedGroup.Add(comBoxGroups.Text);
            Button_ADD_Group.IsEnabled = false;
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
            if (selectedGroup.Count == 0)
            {
                MessageBox.Show("Please choose at least a group or click add to use all groups of joints to transfer reactions.");
            }
            else if (casePatternList.Count == 0)
            {
                MessageBox.Show("Please choose at least 1 load case to transfer reactions.");
            }
            else
            {
                IsEnabled = false;
                //Call up copy property result window
                src.loads_etabs_safe.WindowEtabsSafeResult windowEtabsSafeResult = new src.loads_etabs_safe.WindowEtabsSafeResult(_SapModelETABS, _SapModelSAFE, _PluginETABS, databaseTableListETABS, selectedGroup, casePatternList, pointobjectconnectivityTable, jointreactionTable, storydefinitionTable, jointforcesFrametable, jointforcesShelltable, jointforcesLinktable);
                IsEnabled = true;
            }
        }
    }
}

