using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.add_load_combo
{
    /// <summary>
    /// Interaction logic for WindowAddLoadComboMain.xaml
    /// </summary>
    public partial class WindowAddLoadComboMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        List<Dictionary<string, string>> loadAndFactorList = new List<Dictionary<string, string>>();

        public WindowAddLoadComboMain(cSapModel SapModel, cPluginCallback plugin)
        {
            _SapModel = SapModel;
            _Plugin = plugin;

            InitializeComponent();

            //Filling combo box
            comBoxLoadComboType.ItemsSource = src.HelperEtabs.typeLoadCombo;
            comBoxLoadComboType.SelectedIndex = 0;

            //Fill in load case list box
            src.HelperUI.PopulateListBoxAvailLoadCase(_SapModel, listBoxAvailLoadCase);

            ShowDialog();
        }

        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            //If load factor is empty then return
            string loadFactor = textBoxLoadCaseFactor.Text;
            if (loadFactor == "")
            {
                System.Windows.MessageBox.Show("Load factor cannot be empty!");
                return;
            }

            List<Object> selectedItems = new List<Object>();
            foreach (var item in listBoxAvailLoadCase.SelectedItems)
            {
                selectedItems.Add(item);
            }

            foreach (var item in selectedItems)
            {
                listBoxAvailLoadCase.Items.RemoveAt(listBoxAvailLoadCase.Items.IndexOf(item));

                string selectedString = item + ", " + loadFactor;
                listBoxChoosenLoadCase.Items.Add(selectedString);

                //Add into dictionary
                Dictionary<string, string> eachLoadAndFactor = new Dictionary<string, string>
                    {
                        {"loadCase", Convert.ToString(item)},
                        {"factor", loadFactor}
                    };

                loadAndFactorList.Add(eachLoadAndFactor);
            }


        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            List<Object> selectedItems = new List<Object>();
            foreach (var item in listBoxChoosenLoadCase.SelectedItems)
            {
                selectedItems.Add(item);
            }

            foreach (var item in selectedItems)
            {
                int index = listBoxChoosenLoadCase.Items.IndexOf(item);

                listBoxAvailLoadCase.Items.Add(loadAndFactorList[index]["loadCase"]);

                listBoxChoosenLoadCase.Items.RemoveAt(index);
                loadAndFactorList.RemoveAt(index);
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                //Translate load combo type for API
                string loadComboType = comBoxLoadComboType.Text;
                int loadComboTypeInt = 0;

                switch (loadComboType)
                {
                    case "Linear Additive":
                        loadComboTypeInt = 0;
                        break;
                    case "Envelope":
                        loadComboTypeInt = 1;
                        break;
                    case "Absolute Additive":
                        loadComboTypeInt = 2;
                        break;
                    case "SRSS":
                        loadComboTypeInt = 3;
                        break;
                    case "Range Additive":
                        loadComboTypeInt = 4;
                        break;
                }

                string loadComboName = textBoxLoadComboName.Text;

                //Call up copy result window
                src.add_load_combo.WindowAddLoadComboResult windowAddLoadComboResult = new src.add_load_combo.WindowAddLoadComboResult(
                    _SapModel, _Plugin, loadComboTypeInt, loadAndFactorList, loadComboName);
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }


            IsEnabled = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
