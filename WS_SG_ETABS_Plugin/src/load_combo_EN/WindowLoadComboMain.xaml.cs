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

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    /// <summary>
    /// Interaction logic for WindowLoadComboMain.xaml
    /// </summary>
    public partial class WindowLoadComboMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        #region Data source

        public static List<string> quasiPermanentCategoryList = new List<string>()
        {
            "A: dometic,residential areas", "B: office areas", "C: congregation areas", "D: shopping areas", "E: storage areas", "F: traffic area, vw<30kN", "G: traffic area, 30kN<vw<160kN", "H: roofs", "Wind load", "Temperature"
        };

        #endregion

        #region Data source

        public static List<string> windLoadDirectionList = new List<string>()
        {
            "4", "8"
        };

        #endregion

        public WindowLoadComboMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();


            //    //Fill in combobox for available groups
            //    src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            //    //Fill in combobox for output case
            //    List<string> outputCasesList = src.HelperEtabs.GetLoadCaseAndComboList(_SapModel);
            //    comBoxOutputCase.ItemsSource = outputCasesList;
            //    comBoxOutputCase.SelectedIndex = 0;

            //    //Fill in combobox for direction
            //    comBoxReactionDirection.ItemsSource = src.plot_reaction.FunctionPlotReaction.reactionDirectionList;
            //    comBoxReactionDirection.SelectedIndex = 2;

            ShowDialog();
        }

        private void checkBoxExistingDL_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void checkBoxExistingDL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBoxNameDL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comBoxExistingDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxChoosenGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingSDL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingSDL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameSDL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingLL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingLL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameLL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comBoxLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingWIND_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingWIND_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingWINDVERTICAL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingWINDVERTICAL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxExistingWINDVERTICAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameWIND_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBoxNameWINDVERTICAL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comBoxWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxWINDVERTICAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxWIND_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxWIND_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxWINDVERTICAL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxWINDVERTICAL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxWINDDIRECTION_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

        }

        private void textBoxNameNOTIONALFACTOR_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxChoosenNOTIONAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxExistingTEMP_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingTEMP_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingTEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameTEMP_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comBoxTEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenTEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingSEISMIC_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingSEISMIC_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingSEISMIC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenSEISMIC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingAL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingAL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameAL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void checkBoxAL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxAL_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingUPLIFT_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxExistingUPLIFT_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void comBoxExistingUPLIFT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenUPLIFT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {

        }

        private void textBoxNameEHF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxChoosenEHF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
    
