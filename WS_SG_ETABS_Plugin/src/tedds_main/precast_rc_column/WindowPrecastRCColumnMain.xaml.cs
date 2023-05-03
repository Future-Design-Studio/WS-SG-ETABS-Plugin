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
        public WindowPrecastRCColumnMain(cSapModel SapModelEtabs, cPluginCallback PluginEtabs, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            InitializeComponent();
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
            src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult windowPrecastRCColumnResult = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnResult();
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

        private void Button_ADD_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_REMOVE_Case_Pattern_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_ADD_Group_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_REMOVE_Group_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxCasePatternMatching_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxChoosenGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
