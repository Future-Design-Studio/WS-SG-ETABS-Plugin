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

        #region Data source concrete grade

        List<string> concreteGradeList = new List<string>()
        {
            "C12/15", "C16/20", "C20/25", "C25/30", "C28/35", "C30/37", "C32/40", "C35/45", "C40/50", "C45/55", "C50/60", "C55/67", "C60/75", "C70/85", "C80/95", "C90/105"
        };

        #endregion

        cSapModel SapModel;
        cPluginCallback Plugin;
        List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS = new List<HelperEtabs.DatabaseTable> ();



        public WindowPrecastRCColumnMain(cSapModel SapModelEtabs, cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            SapModel = SapModelEtabs;
            Plugin = PluginEtabs;
            DatabaseTableListETABS = databaseTableListETABS;

            InitializeComponent();

            //Fill in combobox for groups
            string[] outputCasesList = src.HelperEtabs.GetGroupList(SapModelEtabs);
            outputCasesList = outputCasesList.Skip(1).ToArray();
            comBoxGroup.ItemsSource = outputCasesList;
            comBoxGroup.SelectedIndex = 0;

            //Fill in combobox for concrete grade
            comboxConcreteGrade.ItemsSource = concreteGradeList;
            comboxConcreteGrade.SelectedIndex = 0;

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
            if (!string.IsNullOrEmpty(textboxCover.Text))
            {
                ClassPrecastRCColumn.GroupandCoverandGradeMatch eachgroupList = new ClassPrecastRCColumn.GroupandCoverandGradeMatch(comBoxGroup.Text, textboxCover.Text, comboxConcreteGrade.Text);
                groupList.Add(eachgroupList);
                listBoxGroups.Items.Add("Group of columns from group: " + comBoxGroup.Text + ", " + textboxCover.Text + "mm cover, concrete grade: " + comboxConcreteGrade.Text);
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

        private void comboxConcreteGrade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Call up copy property result window
            src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult windowPrecastRCColumnResult = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult(SapModel, Plugin, DatabaseTableListETABS, groupList);
            IsEnabled = true;
        }
    }
}
