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
using Tekla;
using Tekla.Structural.InteropAssemblies.TeddsCalc;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    /// <summary>
    /// Interaction logic for WindowPrecastRCColumnMain.xaml
    /// </summary>
    public partial class WindowPrecastRCColumnMain : Window
    {

        string modelPath;
        List<string> calcMethodList = new List<string>()
        {
            "All Columns within group", "Critical Column per group per storey"
        };

        cSapModel SapModel;
        cPluginCallback Plugin;
        List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS = new List<HelperEtabs.DatabaseTable>();
        src.HelperEtabs.DatabaseTable groupassignmentTable = new HelperEtabs.DatabaseTable();
        bool isError = false;
        List<string> groupnameList = new List<string>();



        public WindowPrecastRCColumnMain(cSapModel SapModelEtabs, cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            SapModel = SapModelEtabs;
            Plugin = PluginEtabs;
            DatabaseTableListETABS = databaseTableListETABS;

            InitializeComponent();

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            filePath.Text = modelPath;

            //Fill in combobox for groups
            string[] outputCasesList = src.HelperEtabs.GetGroupList(SapModelEtabs);
            outputCasesList = outputCasesList.Skip(1).ToArray();
            comBoxGroup.ItemsSource = outputCasesList;
            comBoxGroup.SelectedIndex = 0;

            //Fill in combobox for concrete grade
            comboxCalcMethod.ItemsSource = calcMethodList;
            comboxCalcMethod.SelectedIndex = 0;


            (isError, groupassignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableListETABS, "Group Assignments");
            if (isError)
            {
                MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                return;
            }
            else
            {
                //Filter group to only include points, to improve performance
                (isError, groupassignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupassignmentTable, "ObjectType", "Line");
                if (isError)
                {
                    MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                    return;
                }
                else
                {
                    foreach (Dictionary<string, string> group in groupassignmentTable.TableInList)
                    {
                        if (group["GroupName"] != "All" && !groupnameList.Contains(group["GroupName"]))
                        {
                            groupnameList.Add(group["GroupName"]);
                        }
                    }

                    comBoxGroup.ItemsSource = groupnameList;
                    comBoxGroup.SelectedIndex = 0;
                }
            }

            ShowDialog();
        }

        private void comBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textboxCover_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList = new List<ClassPrecastRCColumn.GroupandCoverandGradeMatch>();
        private void Button_ADD_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textboxCover.Text) && IsDigitsOnly(textboxCover.Text))
            {

                ClassPrecastRCColumn.GroupandCoverandGradeMatch eachgroupList = new ClassPrecastRCColumn.GroupandCoverandGradeMatch(comBoxGroup.Text, textboxCover.Text, comboxCalcMethod.SelectedIndex);
                groupList.Add(eachgroupList);
                listBoxGroups.Items.Add("Group of columns from group: " + comBoxGroup.Text + ", " + textboxCover.Text + "mm cover, with method: " + calcMethodList[comboxCalcMethod.SelectedIndex]);


            }
            else if (string.IsNullOrEmpty(textboxCover.Text))
            {
                MessageBox.Show("Please enter a value for Concrete Cover!");
            }
            else
            {
                MessageBox.Show("Please enter only numeric values for Concrete Cover!");
            }

        }

        private void Button_REMOVE_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxGroups.Items.Count != 0 && listBoxGroups.SelectedIndex != -1)
            {
                int selectedIndex = listBoxGroups.SelectedIndex;

                listBoxGroups.Items.RemoveAt(selectedIndex);
                groupList.RemoveAt(selectedIndex);
            }
        }

        private void comboxCalcMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                filePath.Text = dialog.SelectedPath + "\\";
            }
        }


        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxGroups.Items.Count != 0)
            {
                IsEnabled = false;
                //Call up copy property result window
                src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult windowPrecastRCColumnResult = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult(SapModel, Plugin, DatabaseTableListETABS, groupList, filePath.Text);
                IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please add at least one group containing columns to run Precast concrete column design (EN1992)!");
            }
        }



        bool IsDigitsOnly(string str)
        {
            int i = 0;
            foreach (char c in str)
            {
                if (c.ToString() == ".")
                {
                    i++;
                    if (i > 1)
                    {
                        return false;
                    }
                    continue;
                }
                bool value = char.IsDigit(c);
                if (value == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}