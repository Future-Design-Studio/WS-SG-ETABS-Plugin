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
            "A: dometic,residential areas", "B: office areas", "C: congregation areas", "D: shopping areas", "E: storage areas", "F: traffic area, vw<30kN", "G: traffic area, 30kN<vw<160kN", "H: roofs", "Wind load", "Temperature"
        };

        Dictionary<string, List<double>> quasiPermanentDict = new Dictionary<string, List<double>>()
       {
           { "A: dometic,residential areas", new List<double>(){0.7, 0.5, 0.3 } },
           { "B: office areas", new List<double>(){0.7, 0.5, 0.3 } },
           { "C: congregation areas", new List<double>(){0.7, 0.7, 0.6 } },
           { "D: shopping areas", new List<double>(){0.7, 0.7, 0.6 } },
           { "E: storage areas", new List<double>(){1.0, 0.9, 0.8 } },
           { "F: traffic area, vw<30kN", new List<double>(){0.7, 0.7, 0.6 } },
           { "G: traffic area, 30kN<vw<160kN", new List<double>(){0.7, 0.5, 0.3 } },
           { "H: roofs", new List<double>(){0.7, 0, 0 } },
           { "Wind load", new List<double>(){0.5, 0.2, 0 } },
           { "Temperature", new List<double>(){0.6, 0.5, 0 } },
       };

        
        Dictionary<string, double> quasiPermanentADict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.5}, 
            {"phi2", 0.3}
        };
        

        Dictionary<string, double> quasiPermanentBDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.5},
            {"phi2", 0.3}
        };

        Dictionary<string, double> quasiPermanentCDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.7},
            {"phi2", 0.6}
        };

        Dictionary<string, double> quasiPermanentDDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.7},
            {"phi2", 0.6}
        };

        Dictionary<string, double> quasiPermanentEDict = new Dictionary<string, double>
        {
            {"phi0", 1.0},
            {"phi1", 0.9},
            {"phi2", 0.8}
        };

        Dictionary<string, double> quasiPermanentFDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.7},
            {"phi2", 0.6}
        };

        Dictionary<string, double> quasiPermanentGDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0.5},
            {"phi2", 0.3}
        };

        Dictionary<string, double> quasiPermanentHDict = new Dictionary<string, double>
        {
            {"phi0", 0.7},
            {"phi1", 0},
            {"phi2", 0}
        };

        Dictionary<string, double> quasiPermanentWINDDict = new Dictionary<string, double>
        {
            {"phi0", 0.5},
            {"phi1", 0.2},
            {"phi2", 0}
        };

        Dictionary<string, double> quasiPermanentTEMPERATUREDict = new Dictionary<string, double>
        {
            {"phi0", 0.6},
            {"phi1", 0.5},
            {"phi2", 0}
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
            "WX", "WXN", "WY", "WYN", "W0", "W180", "W90", "W270", "W45", "W225", "W135", "W315"
        };

        List<string> windvertnoplusminusdirectionList = new List<string>()
        {
            "WZ", "WZN"
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

        List<string> ehfDLXList = new List<string>();
        List<string> ehfSDLXList = new List<string>();
        List<string> ehfLLXList = new List<string>();
        List<string> ehfDLYList = new List<string>();
        List<string> ehfSDLYList = new List<string>();
        List<string> ehfLLYList = new List<string>();

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
                string selectedNLXString = notionalxyList[0] + comBoxExistingDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + comBoxExistingDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLYString);

                ClassLoadCombo.DeadLoad eachDeadLoad = new ClassLoadCombo.DeadLoad(selectedString, true);
                deadLoadList.Add(eachDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + comBoxExistingDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + comBoxExistingDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalDYLoadList.Add(eachnotionaly);

                ehfDLXList.Add(ehfList[0] + " (" + selectedString + ")");
                ehfDLYList.Add(ehfList[1] + " (" + selectedString + ")");
            }
            else
            {
                string selectedString = textBoxNameDL.Text;
                listBoxChoosenDL.Items.Add(selectedString);
                string selectedNLXString = notionalxyList[0] + textBoxNameDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + textBoxNameDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALDL.Items.Add(selectedNLYString);

                ClassLoadCombo.DeadLoad eachDeadLoad = new ClassLoadCombo.DeadLoad(selectedString, false);
                deadLoadList.Add(eachDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + textBoxNameDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + textBoxNameDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalDYLoadList.Add(eachnotionaly);

                ehfDLXList.Add(ehfList[0] + " (" + selectedString + ")");
                ehfDLYList.Add(ehfList[1] + " (" + selectedString + ")");
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
                
                deadLoadList.RemoveAt(selectedIndex);

                NotionalDXLoadList.RemoveAt(selectedIndex);
                NotionalDYLoadList.RemoveAt(selectedIndex);

                ehfDLXList.RemoveAt(selectedIndex);
                ehfDLYList.RemoveAt(selectedIndex);
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

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + comBoxExistingSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalSDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + comBoxExistingSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalSDYLoadList.Add(eachnotionaly);

                ehfSDLXList.Add(ehfList[0] + " (" + selectedString + ")");
                ehfSDLYList.Add(ehfList[1] + " (" + selectedString + ")");
            }
            else
            {
                string selectedString = textBoxNameSDL.Text;
                listBoxChoosenSDL.Items.Add(selectedString);
                string selectedNLXString = notionalxyList[0] + textBoxNameSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLXString);
                string selectedNLYString = notionalxyList[1] + textBoxNameSDL.Text + "), " + textBoxNameNOTIONALFACTOR.Text;
                listBoxChoosenNOTIONALSDL.Items.Add(selectedNLYString);

                ClassLoadCombo.SDLLoad eachSDeadLoad = new ClassLoadCombo.SDLLoad(selectedString, false);
                SDLoadList.Add(eachSDeadLoad);

                ClassLoadCombo.NotionalLoad eachnotionalx = new ClassLoadCombo.NotionalLoad(notionalxyList[0] + textBoxNameSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalSDXLoadList.Add(eachnotionalx);
                ClassLoadCombo.NotionalLoad eachnotionaly = new ClassLoadCombo.NotionalLoad(notionalxyList[1] + textBoxNameSDL.Text + ")", double.Parse(textBoxNameNOTIONALFACTOR.Text));
                NotionalSDYLoadList.Add(eachnotionaly);

                ehfSDLXList.Add(ehfList[0] + " (" + selectedString + ")");
                ehfSDLYList.Add(ehfList[1] + " (" + selectedString + ")");
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
                
                SDLoadList.RemoveAt(selectedIndex);

                NotionalSDXLoadList.RemoveAt(selectedIndex);
                NotionalSDYLoadList.RemoveAt(selectedIndex);

                ehfSDLXList.RemoveAt(selectedIndex);
                ehfSDLYList.RemoveAt(selectedIndex);
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

                ehfLLXList.Add(ehfList[0] + " (" + comBoxExistingLL.Text + ")");
                ehfLLYList.Add(ehfList[1] + " (" + comBoxExistingLL.Text + ")");
            }
            else
            {
                string selectedString = textBoxNameLL.Text + ", " + comBoxLL.Text;
                listBoxChoosenLL.Items.Add(selectedString);

                ClassLoadCombo.LiveLoad eachLiveLoad = new ClassLoadCombo.LiveLoad(textBoxNameLL.Text, false, comBoxLL.Text);
                LiveLoadList.Add(eachLiveLoad);

                ehfLLXList.Add(ehfList[0] + " (" + textBoxNameLL.Text + ")");
                ehfLLYList.Add(ehfList[1] + " (" + textBoxNameLL.Text + ")");
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

                LiveLoadList.RemoveAt(selectedIndex);

                ehfLLXList.RemoveAt(selectedIndex);
                ehfLLYList.RemoveAt(selectedIndex);
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

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(comBoxExistingWIND.Text, true, comBoxWIND.Text);
                WindLoadList.Add(eachWindLoad);
            }
            else
            {
                if (checkBoxWIND.IsChecked == true)
                {
                    string selectedString = comBoxNameWIND.Text + ", " + comBoxWIND.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);

                    ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(comBoxNameWIND.Text, false, comBoxWIND.Text);
                    WindLoadList.Add(eachWindLoad);
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

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[0], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[1], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
                        }
                        else
                        {
                            string selectedString = wind48noplusminusdirectionList[2] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[3] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[2], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[3], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
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

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[4], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[5], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 1)
                        {
                            string selectedString = wind48noplusminusdirectionList[6] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[7] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[6], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[7], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 2)
                        {
                            string selectedString = wind48noplusminusdirectionList[8] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[9] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[8], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[9], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
                        }
                        if (comBoxNameWIND.SelectedIndex == 3)
                        {
                            string selectedString = wind48noplusminusdirectionList[10] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString);
                            string selectedString2 = wind48noplusminusdirectionList[11] + ", " + comBoxWIND.Text;
                            listBoxChoosenWIND.Items.Add(selectedString2);

                            ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[10], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad1);
                            ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(wind48noplusminusdirectionList[11], false, comBoxWIND.Text);
                            WindLoadList.Add(eachWindLoad2);
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

                ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(comBoxExistingWINDVERTICAL.Text, true, comBoxWINDVERTICAL.Text);
                WindLoadList.Add(eachWindLoad);
            }
            else
            {
                if (checkBoxWINDVERTICAL.IsChecked ==true)
                {
                    string selectedString = comBoxNameWINDVERTICAL.Text + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);

                    ClassLoadCombo.WindLoad eachWindLoad = new ClassLoadCombo.WindLoad(comBoxNameWINDVERTICAL.Text, false, comBoxWINDVERTICAL.Text);
                    WindLoadList.Add(eachWindLoad);
                }
                else
                {
                    string selectedString = windvertnoplusminusdirectionList[0] + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString);
                    string selectedString2 = windvertnoplusminusdirectionList[1] + ", " + comBoxWINDVERTICAL.Text;
                    listBoxChoosenWIND.Items.Add(selectedString2);

                    ClassLoadCombo.WindLoad eachWindLoad1 = new ClassLoadCombo.WindLoad(windvertnoplusminusdirectionList[0], false, comBoxWINDVERTICAL.Text);
                    WindLoadList.Add(eachWindLoad1);
                    ClassLoadCombo.WindLoad eachWindLoad2 = new ClassLoadCombo.WindLoad(windvertnoplusminusdirectionList[1], false, comBoxWINDVERTICAL.Text);
                    WindLoadList.Add(eachWindLoad2);
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
                int selectedIndex = listBoxChoosenWIND.SelectedIndex;

                listBoxChoosenWIND.Items.RemoveAt(selectedIndex);

                WindLoadList.RemoveAt(selectedIndex);
            }
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

        private void Button_ADD_TEMP_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingTEMP.IsChecked == true)
            {
                string selectedString = comBoxExistingTEMP.Text + ", " + comBoxTEMP.Text;
                listBoxChoosenTEMP.Items.Add(selectedString);

                ClassLoadCombo.TemperatureLoad eachTempLoad = new ClassLoadCombo.TemperatureLoad(comBoxExistingTEMP.Text, true, comBoxTEMP.Text);
                TemperatureLoadList.Add(eachTempLoad);
            }
            else
            {
                string selectedString = textBoxNameTEMP.Text + ", " + comBoxTEMP.Text;
                listBoxChoosenTEMP.Items.Add(selectedString);

                ClassLoadCombo.TemperatureLoad eachTempLoad = new ClassLoadCombo.TemperatureLoad(textBoxNameTEMP.Text, false, comBoxTEMP.Text);
                TemperatureLoadList.Add(eachTempLoad);
            }
        }

        private void listBoxChoosenTEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_TEMP_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenTEMP.Items.Count != 0 && listBoxChoosenTEMP.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenTEMP.SelectedIndex;

                listBoxChoosenTEMP.Items.RemoveAt(selectedIndex);

                TemperatureLoadList.RemoveAt(selectedIndex);
            }
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

        private void Button_ADD_SEISMIC_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingSEISMIC.IsChecked == true)
            {
                string selectedString = comBoxExistingSEISMIC.Text;
                listBoxChoosenSEISMIC.Items.Add(selectedString);

                ClassLoadCombo.SeismicLoad eachSeismicLoad = new ClassLoadCombo.SeismicLoad(comBoxExistingSEISMIC.Text, true);
                SeismicLoadList.Add(eachSeismicLoad);
            }
            else
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
            }
        }

        private void listBoxChoosenSEISMIC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_SEISMIC_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenSEISMIC.Items.Count != 0 && listBoxChoosenSEISMIC.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenSEISMIC.SelectedIndex;

                listBoxChoosenSEISMIC.Items.RemoveAt(selectedIndex);

                SeismicLoadList.RemoveAt(selectedIndex);
            }
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

        private void Button_ADD_AL_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxExistingAL.IsChecked == true)
            {
                string selectedString = comBoxExistingAL.Text;
                listBoxChoosenAL.Items.Add(selectedString);

                ClassLoadCombo.AccidentalLoad eachAccidentalLoad = new ClassLoadCombo.AccidentalLoad(comBoxExistingAL.Text, true);
                AccidentalLoadList.Add(eachAccidentalLoad);
            }
            else
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
                    string selectedString1 = textBoxNameAL.Text + "N";
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
            if (listBoxChoosenAL.Items.Count != 0 && listBoxChoosenAL.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenAL.SelectedIndex;

                listBoxChoosenAL.Items.RemoveAt(selectedIndex);

                AccidentalLoadList.RemoveAt(selectedIndex);
            }
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
            }
            else
            {
                string selectedString = "UPLIFT";
                listBoxChoosenUPLIFT.Items.Add(selectedString);

                ClassLoadCombo.UpliftLoad eachUpliftLoad = new ClassLoadCombo.UpliftLoad(selectedString, false);
                UpliftLoadList.Add(eachUpliftLoad);
            }
        }

        private void listBoxChoosenUPLIFT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_UPLIFT_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenUPLIFT.Items.Count != 0 && listBoxChoosenUPLIFT.SelectedIndex != -1)
            {
                int selectedIndex = listBoxChoosenUPLIFT.SelectedIndex;

                listBoxChoosenUPLIFT.Items.RemoveAt(selectedIndex);

                UpliftLoadList.RemoveAt(selectedIndex);
            }
        }

        private void textBoxNameEHF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_ADD_EHF_Click(object sender, RoutedEventArgs e)
        {
            foreach (string ehfdl in ehfDLXList)
            {
                string selectedString = ehfdl + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfdl, double.Parse(textBoxNameEHF.Text));
                EHFDLXLoadList.Add(eachehfLoad);
            }
            foreach (string ehfdl in ehfDLYList)
            {
                string selectedString = ehfdl + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfdl, double.Parse(textBoxNameEHF.Text));
                EHFDLYLoadList.Add(eachehfLoad);
            }
            foreach (string ehfsdl in ehfSDLXList)
            {
                string selectedString = ehfsdl + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfsdl, double.Parse(textBoxNameEHF.Text));
                EHFSDLXLoadList.Add(eachehfLoad);
            }
            foreach (string ehfsdl in ehfSDLYList)
            {
                string selectedString = ehfsdl + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfsdl, double.Parse(textBoxNameEHF.Text));
                EHFSDLYLoadList.Add(eachehfLoad);
            }
            foreach (string ehfll in ehfLLXList)
            {
                string selectedString = ehfll + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfll, double.Parse(textBoxNameEHF.Text));
                EHFLLXLoadList.Add(eachehfLoad);
            }
            foreach (string ehfll in ehfLLYList)
            {
                string selectedString = ehfll + textBoxNameEHF.Text;
                listBoxChoosenEHF.Items.Add(selectedString);

                ClassLoadCombo.EHFLoad eachehfLoad = new ClassLoadCombo.EHFLoad(ehfll, double.Parse(textBoxNameEHF.Text));
                EHFLLYLoadList.Add(eachehfLoad);
            }
        }

        private void listBoxChoosenEHF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_REMOVE_EHF_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChoosenEHF.Items.Count != 0 && listBoxChoosenEHF.SelectedIndex != -1)
            {
                listBoxChoosenEHF.Items.Clear();

                EHFDLXLoadList.Clear();
                EHFDLYLoadList.Clear();
                EHFSDLXLoadList.Clear();
                EHFSDLYLoadList.Clear();
                EHFLLXLoadList.Clear();
                EHFLLYLoadList.Clear();
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
            src.load_combo_EN.WindowLoadComboResult windowLoadComboResult = new src.load_combo_EN.WindowLoadComboResult(_SapModel, _Plugin, deadLoadList, SDLoadList, LiveLoadList, WindLoadList, NotionalDXLoadList, NotionalDYLoadList, NotionalSDXLoadList, NotionalSDYLoadList, TemperatureLoadList, SeismicLoadList, AccidentalLoadList, UpliftLoadList, EHFDLXLoadList, EHFDLYLoadList, EHFSDLXLoadList, EHFSDLYLoadList, EHFLLXLoadList, EHFLLYLoadList);
            IsEnabled = true;
        }
    }
}
    
