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

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up copy property result window
            src.loads_etabs_etabs.WindowEtabsEtabsResult windowEtabsEtabsResult = new src.loads_etabs_etabs.WindowEtabsEtabsResult(_SapModel, _Plugin, databaseTableList);
            IsEnabled = true;
        }

        private void comBoxLoadCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxLoadPattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxCreateNew_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxCreateNew_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxCasePatternMatching_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_ADD_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_REMOVE_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxUseAllJoints_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxUseAllJoints_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_ADD_Group_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_REMOVE_Group_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
