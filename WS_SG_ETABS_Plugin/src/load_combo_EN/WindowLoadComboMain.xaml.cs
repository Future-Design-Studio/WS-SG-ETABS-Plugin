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

        #region Data source quasi permenant stuff

        List<string> quasiPermanentCategoryList = new List<string>()
        {
            "A: dometic,residential areas", "B: office areas", "C: congregation areas", "D: shopping areas", "E: storage areas", "F: traffic area, vw<30kN", "G: traffic area, 30kN<vw<160kN", "H: roofs"
        };

        #endregion

        #region Data source wind

        List<string> windLoadDirectionList = new List<string>()
        {
            "4", "8"
        };

        List<string> wind4directionList = new List<string>()
        {
            "WX", "WY"
        };

        List<string> wind8directionList = new List<string>()
        {
            "W0", "W90", "W45", "W135"
        };

        List<string> windvertdirectionList = new List<string>()
        {
            "WZ"
        };

        List<string> wind48noplusminusdirectionList = new List<string>()
        {
            "WX", "WX_N", "WY", "WY_N", "W0", "W180", "W90", "W270", "W45", "W225", "W135", "W315"
        };

        List<string> windvertnoplusminusdirectionList = new List<string>()
        {
            "WZ", "WZ_N"
        };

        #endregion

        #region Data source notional

        List<string> notionalxyList = new List<string>()
        {
            "Nx (", "Ny ("
        };

        #endregion

        #region Data source seismic

        List<string> seismicList = new List<string>()
        {
            "RSA", "EQx", "EQy"
        };

        #endregion

        #region Data source EHF

        List<string> ehfList = new List<string>()
        {
            "EHFx", "EHFy"
        };

        #endregion

        #region Data source loadlists

        List<ClassLoadCombo.DeadLoad> deadLoadList = new List<ClassLoadCombo.DeadLoad>();
        List<ClassLoadCombo.SDLLoad> SDLoadList = new List<ClassLoadCombo.SDLLoad>();
        List<ClassLoadCombo.LiveLoad> LiveLoadList = new List<ClassLoadCombo.LiveLoad>();
        List<ClassLoadCombo.WindLoad> WindLoadList = new List<ClassLoadCombo.WindLoad>();
        List<ClassLoadCombo.WindLoad> WindVertLoadList = new List<ClassLoadCombo.WindLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalDXLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalDYLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalSDXLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.NotionalLoad> NotionalSDYLoadList = new List<ClassLoadCombo.NotionalLoad>();
        List<ClassLoadCombo.TemperatureLoad> TemperatureLoadList = new List<ClassLoadCombo.TemperatureLoad>();
        List<ClassLoadCombo.SeismicLoad> SeismicLoadList = new List<ClassLoadCombo.SeismicLoad>();
        List<ClassLoadCombo.AccidentalLoad> AccidentalLoadList = new List<ClassLoadCombo.AccidentalLoad>();
        List<ClassLoadCombo.UpliftLoad> UpliftLoadList = new List<ClassLoadCombo.UpliftLoad>();
        List<ClassLoadCombo.EHFLoad> EHFDLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFDLYLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFSDLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFSDLYLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFLLXLoadList = new List<ClassLoadCombo.EHFLoad>();
        List<ClassLoadCombo.EHFLoad> EHFLLYLoadList = new List<ClassLoadCombo.EHFLoad>();

        #endregion

        public WindowLoadComboMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            
            InitializeComponent();

            checkBoxExistingDL.IsChecked = true;
            checkBoxExistingSDL.IsChecked = true;
            checkBoxExistingLL.IsChecked = true;
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

            comBoxExistingAL.ItemsSource = outputCasesList;
            comBoxExistingAL.SelectedIndex = 0;

            comBoxExistingUPLIFT.ItemsSource = outputCasesList;
            comBoxExistingUPLIFT.SelectedIndex = 0;

            comBoxWINDDIRECTION.ItemsSource = windLoadDirectionList;
            comBoxWINDDIRECTION.SelectedIndex = 0;

            comBoxLL.ItemsSource = quasiPermanentCategoryList;
            comBoxLL.SelectedIndex = 0;

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
                string selectedNLXString = notionalxyList[0] + comBoxExistingDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + comBoxExistingDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLYString);

                ClassLoadCombo.DeadLoad eachDeadLoad = new ClassLoadCombo.DeadLoad(selectedString, true);
                deadLoadList.Add(eachDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + comBoxExistingDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), comBoxExistingDL.Text);
                NotionalDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + comBoxExistingDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), comBoxExistingDL.Text);
                NotionalDYLoadList.Add(eachnotionaly);

                string selectedStringehfxdl = ehfList[0] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFDL.Items.Add(selectedStringehfxdl);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFDLXLoadList.Add(eachehfLoad);

                string selectedStringehfydl = ehfList[1] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFDL.Items.Add(selectedStringehfydl);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFDLYLoadList.Add(eachehfLoad1);
            }
            else if (!string.IsNullOrEmpty(textBoxNameDL.Text))
            {
                string selectedString = textBoxNameDL.Text;
                listBoxChoosenDL.Items.Add(selectedString);
                string selectedNLXString = notionalxyList[0] + textBoxNameDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + textBoxNameDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLYString);

                ClassLoadCombo.DeadLoad eachDeadLoad = new ClassLoadCombo.DeadLoad(selectedString, false);
                deadLoadList.Add(eachDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + textBoxNameDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), textBoxNameDL.Text);
                NotionalDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + textBoxNameDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), textBoxNameDL.Text);
                NotionalDYLoadList.Add(eachnotionaly);

                string selectedStringehfxdl = ehfList[0] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFDL.Items.Add(selectedStringehfxdl);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFDLXLoadList.Add(eachehfLoad);

                string selectedStringehfydl = ehfList[1] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFDL.Items.Add(selectedStringehfydl);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFDLYLoadList.Add(eachehfLoad1);
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
                int selectedIndex = listBoxChoosenDL.SelectedIndex;
                
                listBoxChoosenNOTIONALDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenNOTIONALDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenDL.Items.RemoveAt(selectedIndex);
                listBoxChoosenEHFDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenEHFDL.Items.RemoveAt(selectedIndex * 2);

                deadLoadList.RemoveAt(selectedIndex);

                NotionalDXLoadList.RemoveAt(selectedIndex);
                NotionalDYLoadList.RemoveAt(selectedIndex);

                EHFDLXLoadList.RemoveAt(selectedIndex);
                EHFDLYLoadList.RemoveAt(selectedIndex);
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
                string selectedNLXString = notionalxyList[0] + comBoxExistingSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + comBoxExistingSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLYString);

                ClassLoadCombo.SDLLoad eachSDeadLoad = new ClassLoadCombo.SDLLoad(selectedString, true);
                SDLoadList.Add(eachSDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + comBoxExistingSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), comBoxExistingSDL.Text);
                NotionalSDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + comBoxExistingSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), comBoxExistingSDL.Text);
                NotionalSDYLoadList.Add(eachnotionaly);

                string selectedStringehfxsdl = ehfList[0] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFSDL.Items.Add(selectedStringehfxsdl);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFSDLXLoadList.Add(eachehfLoad);

                string selectedStringehfysdl = ehfList[1] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFSDL.Items.Add(selectedStringehfysdl);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFSDLYLoadList.Add(eachehfLoad1);
            }
            else if (!string.IsNullOrEmpty(textBoxNameSDL.Text))
            {
                string selectedString = textBoxNameSDL.Text;
                listBoxChoosenSDL.Items.Add(selectedString);
                string selectedNLXString = notionalxyList[0] + textBoxNameSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + textBoxNameSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLYString);

                ClassLoadCombo.SDLLoad eachSDeadLoad = new ClassLoadCombo.SDLLoad(selectedString, false);
                SDLoadList.Add(eachSDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + textBoxNameSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), textBoxNameSDL.Text);
                NotionalSDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + textBoxNameSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text), textBoxNameSDL.Text);
                NotionalSDYLoadList.Add(eachnotionaly);

                string selectedStringehfxsdl = ehfList[0] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFSDL.Items.Add(selectedStringehfxsdl);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFSDLXLoadList.Add(eachehfLoad);

                string selectedStringehfysdl = ehfList[1] + " (" + selectedString + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFSDL.Items.Add(selectedStringehfysdl);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + selectedString + ")", double.Parse(textBoxNameEHF.Text), selectedString, "");
                EHFSDLYLoadList.Add(eachehfLoad1);
            }
        }

        private void listBoxChoosenSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_SDL_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenSDL.Items.Count != 0 && listBoxChoosenSDL.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenSDL.SelectedIndex;
                
                listBoxChoosenNOTIONALSDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenNOTIONALSDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenSDL.Items.RemoveAt(selectedIndex);
                listBoxChoosenEHFSDL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenEHFSDL.Items.RemoveAt(selectedIndex * 2);

                SDLoadList.RemoveAt(selectedIndex);

                NotionalSDXLoadList.RemoveAt(selectedIndex);
                NotionalSDYLoadList.RemoveAt(selectedIndex);

                EHFSDLXLoadList.RemoveAt(selectedIndex);
                EHFSDLYLoadList.RemoveAt(selectedIndex);
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

                ClassLoadCombo.LiveLoad eachLiveLoad = new ClassLoadCombo.LiveLoad(comBoxExistingLL.Text, true, comBoxLL.Text);
                LiveLoadList.Add(eachLiveLoad);

                string selectedStringehfxll = ehfList[0] + " (" + comBoxExistingLL.Text + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFLL.Items.Add(selectedStringehfxll);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + comBoxExistingLL.Text + ")", double.Parse(textBoxNameEHF.Text), comBoxExistingLL.Text, comBoxLL.Text);
                EHFLLXLoadList.Add(eachehfLoad);

                string selectedStringehfyll = ehfList[1] + " (" + comBoxExistingLL.Text + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFLL.Items.Add(selectedStringehfyll);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + comBoxExistingLL.Text + ")", double.Parse(textBoxNameEHF.Text), comBoxExistingLL.Text, comBoxLL.Text);
                EHFLLYLoadList.Add(eachehfLoad1);
            }
            else if (!string.IsNullOrEmpty(textBoxNameLL.Text))
            {
                string selectedString = textBoxNameLL.Text + ", " + comBoxLL.Text;
                listBoxChoosenLL.Items.Add(selectedString);

                ClassLoadCombo.LiveLoad eachLiveLoad = new ClassLoadCombo.LiveLoad(textBoxNameLL.Text, false, comBoxLL.Text);
                LiveLoadList.Add(eachLiveLoad);

                string selectedStringehfxll = ehfList[0] + " (" + textBoxNameLL.Text + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFLL.Items.Add(selectedStringehfxll);
                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfList[0] + " (" + textBoxNameLL.Text + ")", double.Parse(textBoxNameEHF.Text), textBoxNameLL.Text, comBoxLL.Text);
                EHFLLXLoadList.Add(eachehfLoad);

                string selectedStringehfyll = ehfList[1] + " (" + textBoxNameLL.Text + ")" + textBoxNameEHF.Text;
                listBoxChoosenEHFLL.Items.Add(selectedStringehfyll);
                ClassLoadCombo.EHFLoad eachehfLoad1 = new ClassLoadCombo.EHFLoad(ehfList[1] + " (" + textBoxNameLL.Text + ")", double.Parse(textBoxNameEHF.Text), textBoxNameLL.Text, comBoxLL.Text);
                EHFLLYLoadList.Add(eachehfLoad1);
            }
        }

        private void listBoxChoosenLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_LL_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenLL.Items.Count != 0 && listBoxChoosenLL.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenLL.SelectedIndex;

                listBoxChoosenLL.Items.RemoveAt(selectedIndex);
                listBoxChoosenEHFLL.Items.RemoveAt(selectedIndex * 2);
                listBoxChoosenEHFLL.Items.RemoveAt(selectedIndex * 2);

                LiveLoadList.RemoveAt(selectedIndex);

                EHFLLXLoadList.RemoveAt(selectedIndex);
                EHFLLYLoadList.RemoveAt(selectedIndex);
            }
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


        int notionaldirectionFactor = 2;
        private void comBoxWINDDIRECTION_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comBoxWINDDIRECTION.SelectedIndex == 0)
            {
                listBoxChoosenWIND.Items.Clear();
                WindLoadList.Clear();
                notionaldirectionFactor = 2;
            }    
            else
            {
                listBoxChoosenWIND.Items.Clear();
                WindLoadList.Clear();
                notionaldirectionFactor = 4;
            }
        }

        private void Button_ADD_WIND_Click(object sender, RoutedEventArgs e)
        {

            if (checkBoxWIND.IsChecked == true && comBoxWINDDIRECTION.SelectedIndex == 0)
            {
                listBoxChoosenWIND.Items.Add(wind4directionList[0]);
                listBoxChoosenWIND.Items.Add(wind4directionList[1]);

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(wind4directionList[0]);
                WindLoadList.Add(eachWindLoad);
                ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind4directionList[1]);
                WindLoadList.Add(eachWindLoad1);

                Button_ADD_WIND.IsEnabled = false;
            }
            else if (checkBoxWIND.IsChecked == true && comBoxWINDDIRECTION.SelectedIndex == 1)
            {
                listBoxChoosenWIND.Items.Add(wind8directionList[0]);
                listBoxChoosenWIND.Items.Add(wind8directionList[1]);
                listBoxChoosenWIND.Items.Add(wind8directionList[2]);
                listBoxChoosenWIND.Items.Add(wind8directionList[3]);

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(wind8directionList[0]);
                WindLoadList.Add(eachWindLoad);
                ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind8directionList[1]);
                WindLoadList.Add(eachWindLoad1);
                ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind8directionList[2]);
                WindLoadList.Add(eachWindLoad2);
                ClassLoadCombo.WindLoad eachWindLoad3 = new ClassLoadCombo.WindLoad(wind8directionList[3]);
                WindLoadList.Add(eachWindLoad3);

                Button_ADD_WIND.IsEnabled = false;
            }
            else if (checkBoxWIND.IsChecked == false && comBoxWINDDIRECTION.SelectedIndex == 0)
            {
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[0]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[1]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[2]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[3]);

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[0]);
                WindLoadList.Add(eachWindLoad);
                ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[1]);
                WindLoadList.Add(eachWindLoad1);
                ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[2]);
                WindLoadList.Add(eachWindLoad2);
                ClassLoadCombo.WindLoad eachWindLoad3 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[3]);
                WindLoadList.Add(eachWindLoad3);

                Button_ADD_WIND.IsEnabled = false;
            }
            else if (checkBoxWIND.IsChecked == false && comBoxWINDDIRECTION.SelectedIndex == 1)
            {
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[4]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[5]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[6]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[7]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[8]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[9]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[10]);
                listBoxChoosenWIND.Items.Add(wind48noplusminusdirectionList[11]);

                ClassLoadCombo.WindLoad eachWindLoad4 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[4]);
                WindLoadList.Add(eachWindLoad4);
                ClassLoadCombo.WindLoad eachWindLoad5 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[5]);
                WindLoadList.Add(eachWindLoad5);
                ClassLoadCombo.WindLoad eachWindLoad6 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[6]);
                WindLoadList.Add(eachWindLoad6);
                ClassLoadCombo.WindLoad eachWindLoad7 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[7]);
                WindLoadList.Add(eachWindLoad7);
                ClassLoadCombo.WindLoad eachWindLoad8 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[8]);
                WindLoadList.Add(eachWindLoad8);
                ClassLoadCombo.WindLoad eachWindLoad9 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[9]);
                WindLoadList.Add(eachWindLoad9);
                ClassLoadCombo.WindLoad eachWindLoad10 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[10]);
                WindLoadList.Add(eachWindLoad10);
                ClassLoadCombo.WindLoad eachWindLoad11 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[11]);
                WindLoadList.Add(eachWindLoad11);

                Button_ADD_WIND.IsEnabled = false;
            }
        }

        private void Button_ADD_WINDVERTICAL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxWINDVERTICAL.IsChecked ==true)
            {
                listBoxChoosenWIND.Items.Add(windvertdirectionList[0]);

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(windvertdirectionList[0]);
                WindVertLoadList.Add(eachWindLoad);

                Button_ADD_WINDVERTICAL.IsEnabled = false;
            }
            else
            {
                listBoxChoosenWIND.Items.Add(windvertnoplusminusdirectionList[0]);
                listBoxChoosenWIND.Items.Add(windvertnoplusminusdirectionList[1]);

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(windvertnoplusminusdirectionList[0]);
                WindVertLoadList.Add(eachWindLoad);
                ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(windvertnoplusminusdirectionList[1]);
                WindVertLoadList.Add(eachWindLoad1);

                Button_ADD_WINDVERTICAL.IsEnabled = false;
            }
            
        }

        private void listBoxChoosenWIND_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_WINDS_Click(object sender, RoutedEventArgs e)
        {
            
            listBoxChoosenWIND.Items.Clear();

            WindLoadList.Clear();
            WindVertLoadList.Clear();

            Button_ADD_WIND.IsEnabled = true;
            Button_ADD_WINDVERTICAL.IsEnabled = true;
            
        }

        private void textBoxNameNOTIONALFACTOR_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxChoosenNOTIONALDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxChoosenNOTIONALSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxConsiderplusminus_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxConsiderplusminus_Unchecked(object sender, RoutedEventArgs e)
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

        private void Button_ADD_TEMP_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxConsiderplusminus.IsChecked == true)
            {
                string selectedString = textBoxNameTEMP.Text;
                listBoxChoosenTEMP.Items.Add(selectedString);

                ClassLoadCombo.TemperatureLoad eachTempLoad = new ClassLoadCombo.TemperatureLoad(selectedString, false);
                TemperatureLoadList.Add(eachTempLoad);

                textBoxNameTEMP.IsEnabled = false;
                Button_ADD_TEMP.IsEnabled = false;
            }
            else if (!string.IsNullOrEmpty(textBoxNameTEMP.Text))
            {
                string selectedString = textBoxNameTEMP.Text;
                listBoxChoosenTEMP.Items.Add(selectedString);

                ClassLoadCombo.TemperatureLoad eachTempLoad = new ClassLoadCombo.TemperatureLoad(selectedString, false);
                TemperatureLoadList.Add(eachTempLoad);

                string selectedString1 = textBoxNameTEMP.Text + "_N";
                listBoxChoosenTEMP.Items.Add(selectedString1);

                ClassLoadCombo.TemperatureLoad eachTempLoad1 = new ClassLoadCombo.TemperatureLoad(selectedString1, false);
                TemperatureLoadList.Add(eachTempLoad1);

                textBoxNameTEMP.IsEnabled = false;
                Button_ADD_TEMP.IsEnabled = false;
            }
        }

        private void listBoxChoosenTEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_TEMP_Click(object sender, RoutedEventArgs e)
        {
            
            textBoxNameTEMP.IsEnabled = true;
            Button_ADD_TEMP.IsEnabled = true;
            listBoxChoosenTEMP.Items.Clear();

            TemperatureLoadList.Clear();
            
        }

        private void Button_ADD_SEISMIC_Click(object sender, RoutedEventArgs e)
        {
            
            string selectedString = seismicList[0];
            listBoxChoosenSEISMIC.Items.Add(selectedString);
            string selectedString1 = seismicList[1];
            listBoxChoosenSEISMIC.Items.Add(selectedString1);
            string selectedString2 = seismicList[2];
            listBoxChoosenSEISMIC.Items.Add(selectedString2);

            ClassLoadCombo.SeismicLoad eachSeismicLoad = new ClassLoadCombo.SeismicLoad(selectedString, false);
            SeismicLoadList.Add(eachSeismicLoad);
            ClassLoadCombo.SeismicLoad eachSeismicLoad1 = new ClassLoadCombo.SeismicLoad(selectedString1, false);
            SeismicLoadList.Add(eachSeismicLoad1);
            ClassLoadCombo.SeismicLoad eachSeismicLoad2 = new ClassLoadCombo.SeismicLoad(selectedString2, false);
            SeismicLoadList.Add(eachSeismicLoad2);

            Button_ADD_SEISMIC.IsEnabled = false;


        }

        private void listBoxChoosenSEISMIC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_SEISMIC_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenSEISMIC.Items.Count != 0)
            {
                listBoxChoosenSEISMIC.Items.Clear();

                SeismicLoadList.Clear();


                Button_ADD_SEISMIC.IsEnabled = true;
            }
        }

        private void checkBoxExistingAL_Checked(object sender, RoutedEventArgs e)
        {
            comBoxExistingAL.IsEnabled = true;
            textBoxNameAL.IsEnabled = false;
            checkBoxAL.IsEnabled = false;
            checkBoxAL.IsChecked = false;
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

        int ALneedtimes2 = 1;
        private void checkBoxAL_Checked(object sender, RoutedEventArgs e)
        {
            ALneedtimes2 = 2;
        }

        private void checkBoxAL_Unchecked(object sender, RoutedEventArgs e)
        {
            ALneedtimes2 = 1;
        }

        private void Button_ADD_AL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingAL.IsChecked == true)
            {
                string selectedString = comBoxExistingAL.Text;
                listBoxChoosenAL.Items.Add(selectedString);

                ClassLoadCombo.AccidentalLoad eachAccidentalLoad = new ClassLoadCombo.AccidentalLoad(comBoxExistingAL.Text, true);
                AccidentalLoadList.Add(eachAccidentalLoad);
            }
            else if (!string.IsNullOrEmpty(textBoxNameAL.Text))
            {
                if (checkBoxAL.IsChecked == true)
                {
                    string selectedString = textBoxNameAL.Text;
                    listBoxChoosenAL.Items.Add(selectedString);

                    ClassLoadCombo.AccidentalLoad eachAccidentalLoad = new ClassLoadCombo.AccidentalLoad(selectedString, false);
                    AccidentalLoadList.Add(eachAccidentalLoad);
                }
                else
                {
                    string selectedString = textBoxNameAL.Text;
                    listBoxChoosenAL.Items.Add(selectedString);
                    string selectedString1 = textBoxNameAL.Text + "_N";
                    listBoxChoosenAL.Items.Add(selectedString1);

                    ClassLoadCombo.AccidentalLoad eachAccidentalLoad = new ClassLoadCombo.AccidentalLoad(selectedString, false);
                    AccidentalLoadList.Add(eachAccidentalLoad);
                    ClassLoadCombo.AccidentalLoad eachAccidentalLoad1 = new ClassLoadCombo.AccidentalLoad(selectedString1, false);
                    AccidentalLoadList.Add(eachAccidentalLoad1);
                }
            }
        }

        private void listBoxChoosenAL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_AL_Click(object sender, RoutedEventArgs e)
        {
            
            listBoxChoosenAL.Items.Clear();

            AccidentalLoadList.Clear();
            
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

        private void Button_ADD_UPLIFT_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingUPLIFT.IsChecked == true)
            {
                string selectedString = comBoxExistingUPLIFT.Text;
                listBoxChoosenUPLIFT.Items.Add(selectedString);

                ClassLoadCombo.UpliftLoad eachUpliftLoad = new ClassLoadCombo.UpliftLoad(selectedString, true);
                UpliftLoadList.Add(eachUpliftLoad);

                Button_ADD_UPLIFT.IsEnabled = false;
            }
            else
            {
                string selectedString = "UPLIFT";
                listBoxChoosenUPLIFT.Items.Add(selectedString);

                ClassLoadCombo.UpliftLoad eachUpliftLoad = new ClassLoadCombo.UpliftLoad(selectedString, false);
                UpliftLoadList.Add(eachUpliftLoad);

                Button_ADD_UPLIFT.IsEnabled = false;
            }
        }

        private void listBoxChoosenUPLIFT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_UPLIFT_Click(object sender, RoutedEventArgs e)
        {
            
            listBoxChoosenUPLIFT.Items.Clear();

            UpliftLoadList.Clear();

            Button_ADD_UPLIFT.IsEnabled = true;
            
        }

        private void textBoxNameEHF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxChoosenEHFDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void listBoxChoosenEHFSDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void listBoxChoosenEHFLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            bool accidentalYesOrNo = true;
            bool seismicYesOrNo = true;
            bool upliftYesOrNo = true;

            if (AccidentalLoadList.Count == 0)
            {
                accidentalYesOrNo = false;
            }
            if (SeismicLoadList.Count == 0)
            {
                seismicYesOrNo = false;
            }
            if (UpliftLoadList.Count == 0)
            {
                upliftYesOrNo = false;
            }

            Close();

            //Call up load combo choosing window
            src.load_combo_EN.WindowChooseLoadCombiToGenerate windowChooseLoadCombiToGenerate = new src.load_combo_EN.WindowChooseLoadCombiToGenerate(_SapModel, _Plugin, accidentalYesOrNo, seismicYesOrNo, upliftYesOrNo, deadLoadList, SDLoadList, LiveLoadList, WindLoadList, NotionalDXLoadList, NotionalDYLoadList, NotionalSDXLoadList, NotionalSDYLoadList, TemperatureLoadList, SeismicLoadList, AccidentalLoadList, UpliftLoadList, EHFDLXLoadList, EHFDLYLoadList, EHFSDLXLoadList, EHFSDLYLoadList, EHFLLXLoadList, EHFLLYLoadList, notionaldirectionFactor, WindVertLoadList, ALneedtimes2);
            
        }
    }
}
    
