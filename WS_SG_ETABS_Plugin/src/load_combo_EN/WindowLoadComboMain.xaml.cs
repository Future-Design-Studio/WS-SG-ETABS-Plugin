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

        #region Data source

        public static List<string> wind4directionList = new List<string>()
        {
            "WX", "WY"
        };

        #endregion

        #region Data source

        public static List<string> wind8directionList = new List<string>()
        {
            "W0", "W90", "W45", "W135"
        };

        #endregion

        #region Data source

        public static List<string> windvertdirectionList = new List<string>()
        {
            "WZ"
        };

        #endregion

        #region Data source

        public static List<string> wind48noplusminusdirectionList = new List<string>()
        {
            "WX", "WXN", "WY", "WYN", "W0", "W180", "W90", "W270", "W45", "W225", "W135", "W315"
        };

        #endregion

        #region Data source

        public static List<string> windvertnoplusminusdirectionList = new List<string>()
        {
            "WZ", "WZN"
        };

        #endregion

        public WindowLoadComboMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            checkBoxExistingDL.IsChecked = true;
            checkBoxExistingSDL.IsChecked = true;
            checkBoxExistingLL.IsChecked = true;
            checkBoxExistingWIND.IsChecked = true;
            checkBoxExistingWINDVERTICAL.IsChecked = true;
            checkBoxExistingTEMP.IsChecked = true;
            checkBoxExistingSEISMIC.IsChecked = true;
            checkBoxExistingAL.IsChecked = true;
            checkBoxExistingUPLIFT.IsChecked = true;

            //Fill in combobox for existing DL case
            List<string> outputCasesList = src.HelperEtabs.GetAllLoadPattern(_SapModel);
            comBoxExistingDL.ItemsSource = outputCasesList;
            comBoxExistingDL.SelectedIndex = 0;

            comBoxExistingSDL.ItemsSource = outputCasesList;
            comBoxExistingSDL.SelectedIndex = 0;

            comBoxExistingLL.ItemsSource = outputCasesList;
            comBoxExistingLL.SelectedIndex = 0;

            comBoxExistingWIND.ItemsSource = outputCasesList;
            comBoxExistingWIND.SelectedIndex = 0;

            comBoxExistingWINDVERTICAL.ItemsSource = outputCasesList;
            comBoxExistingWINDVERTICAL.SelectedIndex = 0;

            comBoxExistingTEMP.ItemsSource = outputCasesList;
            comBoxExistingTEMP.SelectedIndex = 0;

            comBoxExistingSEISMIC.ItemsSource = outputCasesList;
            comBoxExistingSEISMIC.SelectedIndex = 0;

            comBoxExistingAL.ItemsSource = outputCasesList;
            comBoxExistingAL.SelectedIndex = 0;

            comBoxExistingUPLIFT.ItemsSource = outputCasesList;
            comBoxExistingUPLIFT.SelectedIndex = 0;

            comBoxWINDDIRECTION.ItemsSource = windLoadDirectionList;
            comBoxWINDDIRECTION.SelectedIndex = 0;

            comBoxLL.ItemsSource = quasiPermanentCategoryList;
            comBoxLL.SelectedIndex = 0;

            comBoxWIND.ItemsSource = quasiPermanentCategoryList;
            comBoxWIND.SelectedIndex = 0;

            comBoxWINDVERTICAL.ItemsSource = quasiPermanentCategoryList;
            comBoxWINDVERTICAL.SelectedIndex = 0;

            comBoxTEMP.ItemsSource = quasiPermanentCategoryList;
            comBoxTEMP.SelectedIndex = 0;

            comBoxNameWINDVERTICAL.ItemsSource = windvertdirectionList;
            comBoxNameWINDVERTICAL.SelectedIndex = 0;

            ShowDialog();
        }

        private void checkBoxExistingDL_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingDL.IsEnabled = true;
            textBoxNameDL.IsEnabled = false;
        }
        private void checkBoxExistingDL_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingDL.IsEnabled = false;
            textBoxNameDL.IsEnabled=true;
        }

        private void Button_ADD_DL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingDL.IsChecked == true)
            {
                string selectedString = comBoxExistingDL.Text;
                listBoxChoosenDL.Items.Add(selectedString);
            }
            else
            {
                string selectedString = textBoxNameDL.Text;
                listBoxChoosenDL.Items.Add(selectedString);
            }
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

        private void Button_REMOVE_DL_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenDL.Items.Count != 0 && listBoxChoosenDL.SelectedIndex != -1)
            {
                listBoxChoosenDL.Items.RemoveAt(listBoxChoosenDL.SelectedIndex);
            }
        }

        private void checkBoxExistingSDL_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingSDL.IsEnabled = true;
            textBoxNameSDL.IsEnabled = false;
        }

        private void checkBoxExistingSDL_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingSDL.IsEnabled = false;
            textBoxNameSDL.IsEnabled = true;
        }

        private void comBoxExistingSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxNameSDL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_ADD_SDL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingSDL.IsChecked == true)
            {
                string selectedString = comBoxExistingSDL.Text;
                listBoxChoosenSDL.Items.Add(selectedString);
            }
            else
            {
                string selectedString = textBoxNameDL.Text;
                listBoxChoosenSDL.Items.Add(selectedString);
            }
        }

        private void listBoxChoosenSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_SDL_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenSDL.Items.Count != 0 && listBoxChoosenSDL.SelectedIndex != -1)
            {
                listBoxChoosenSDL.Items.RemoveAt(listBoxChoosenSDL.SelectedIndex);
            }
        }

        private void checkBoxExistingLL_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingLL.IsEnabled = true;
            textBoxNameLL.IsEnabled = false;
        }

        private void checkBoxExistingLL_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingLL.IsEnabled = false;
            textBoxNameLL.IsEnabled = true;
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

        private void Button_ADD_LL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingLL.IsChecked == true)
            {
                string selectedString = comBoxExistingLL.Text + ", " + comBoxLL.Text;
                listBoxChoosenLL.Items.Add(selectedString);
            }
            else
            {
                string selectedString = textBoxNameLL.Text + ", " + comBoxLL.Text;
                listBoxChoosenLL.Items.Add(selectedString);
            }
        }

        private void listBoxChoosenLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_LL_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenLL.Items.Count != 0 && listBoxChoosenLL.SelectedIndex != -1)
            {
                listBoxChoosenLL.Items.RemoveAt(listBoxChoosenLL.SelectedIndex);
            }
        }

        private void checkBoxExistingWIND_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingWIND.IsEnabled = true;
            comBoxNameWIND.IsEnabled = false;
            checkBoxWIND.IsEnabled = false;
            comBoxWINDDIRECTION.IsEnabled = false;
        }

        private void checkBoxExistingWIND_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingWIND.IsEnabled = false;
            comBoxNameWIND.IsEnabled = true;
            checkBoxWIND.IsEnabled = true;
            comBoxWINDDIRECTION.IsEnabled = true;
        }

        private void checkBoxExistingWINDVERTICAL_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingWINDVERTICAL.IsEnabled = true;
            comBoxNameWINDVERTICAL.IsEnabled = false;
            checkBoxWINDVERTICAL.IsEnabled = false;
        }

        private void checkBoxExistingWINDVERTICAL_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingWINDVERTICAL.IsEnabled = false;
            comBoxNameWINDVERTICAL.IsEnabled = true;
            checkBoxWINDVERTICAL.IsEnabled = true;
        }

        private void comBoxExistingWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxExistingWINDVERTICAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxNameWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comBoxNameWINDVERTICAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            if (comBoxWINDDIRECTION.SelectedIndex == 0)
            {
                comBoxNameWIND.ItemsSource = wind4directionList;
                comBoxNameWIND.SelectedIndex = 0;
            }    
            else
            {
                comBoxNameWIND.ItemsSource = wind8directionList;
                comBoxNameWIND.SelectedIndex = 0;
            }
        }

        private void Button_ADD_WIND_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingWIND.IsChecked == true)
            {
                string selectedString = comBoxExistingWIND.Text + ", " + comBoxWIND.Text;
                listBoxChoosenWIND.Items.Add(selectedString);
            }
            else
            {
                if (checkBoxWIND.IsChecked == true)
                {
                    string selectedString = comBoxNameWIND.Text + ", " + comBoxWIND.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);
                }
                else
                {
                    if (comBoxWINDDIRECTION.SelectedIndex == 0)
                    {
                        if (comBoxNameWIND.SelectedIndex == 0)
                        {
                            string selectedString = wind48noplusminusdirectionList[0] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[1] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }
                        else
                        {
                            string selectedString = wind48noplusminusdirectionList[2] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[3] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }                        
                    }
                    else
                    {
                        if (comBoxNameWIND.SelectedIndex == 0)
                        {
                            string selectedString = wind48noplusminusdirectionList[4] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[5] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 1)
                        {
                            string selectedString = wind48noplusminusdirectionList[6] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[7] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 2)
                        {
                            string selectedString = wind48noplusminusdirectionList[8] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[9] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 3)
                        {
                            string selectedString = wind48noplusminusdirectionList[10] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[11] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);
                        }
                    }
                        
                }
            }
        }

        private void Button_ADD_WINDVERTICAL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingWINDVERTICAL.IsChecked == true)
            {
                string selectedString = comBoxExistingWINDVERTICAL.Text + ", " + comBoxWINDVERTICAL.Text;
                listBoxChoosenWIND.Items.Add(selectedString);
            }
            else
            {
                if (checkBoxWINDVERTICAL.IsChecked ==true)
                {
                    string selectedString = comBoxNameWINDVERTICAL.Text + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);
                }
                else
                {
                    string selectedString = windvertnoplusminusdirectionList[0] + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);
                    string selectedString2 = windvertnoplusminusdirectionList[1] + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString2);
                }
            }
        }

        private void listBoxChoosenWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_WINDS_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenWIND.Items.Count != 0 && listBoxChoosenWIND.SelectedIndex != -1)
            {
                listBoxChoosenWIND.Items.RemoveAt(listBoxChoosenWIND.SelectedIndex);
            }
        }

        private void textBoxNameNOTIONALFACTOR_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxChoosenNOTIONAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxExistingTEMP_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingTEMP.IsEnabled = true;
            textBoxNameTEMP.IsEnabled = false;
        }

        private void checkBoxExistingTEMP_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingTEMP.IsEnabled = false;
            textBoxNameTEMP.IsEnabled = true;
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
            comBoxExistingSEISMIC.IsEnabled = true;
        }

        private void checkBoxExistingSEISMIC_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingSEISMIC.IsEnabled = false;
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
            comBoxExistingAL.IsEnabled = true;
            textBoxNameAL.IsEnabled = false;
            checkBoxAL.IsEnabled = false;
        }

        private void checkBoxExistingAL_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingAL.IsEnabled = false;
            textBoxNameAL.IsEnabled = true;
            checkBoxAL.IsEnabled = true;
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
            comBoxExistingUPLIFT.IsEnabled = true;
        }

        private void checkBoxExistingUPLIFT_Unchecked(object sender, RoutedEventArgs e)
        {
            comBoxExistingUPLIFT.IsEnabled = false;
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
    
