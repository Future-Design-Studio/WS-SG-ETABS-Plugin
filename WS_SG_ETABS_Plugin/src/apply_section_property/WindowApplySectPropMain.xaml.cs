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
using WS_SG_ETABS_Plugin.src.apply_modifier;
using static WS_SG_ETABS_Plugin.src.apply_section_property.ClassApplySectProp;

namespace WS_SG_ETABS_Plugin.src.apply_section_property
{
    /// <summary>
    /// Interaction logic for WindowApplySectPropMain.xaml
    /// </summary>
    public partial class WindowApplySectPropMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;


        List<ClassApplySectProp.ApplySectPropDefinition> applySectPropDefinitionList = new List<ClassApplySectProp.ApplySectPropDefinition> { };

        public WindowApplySectPropMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in combo box
            HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            HelperUI.PopulateComboBoxAvailFrameSectProp(SapModel, comBoxFrameSectProp);
            HelperUI.PopulateComboBoxAvailAreaSectProp(SapModel, comBoxAreaSectProp);

            //Datagrid
            dataGridApplySectProp.ItemsSource = applySectPropDefinitionList;

            //Check radio button frame
            radioButtonFrame.IsChecked = true;

            ShowDialog();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string groupName = comBoxGroup.SelectedItem.ToString();
            string frameSectProp = null;
            string areaSectProp = null;

            if (radioButtonFrame.IsChecked == true)
            {
                frameSectProp = comBoxFrameSectProp.SelectedItem.ToString();
            }
            else if (radioButtonArea.IsChecked == true)
            {
                areaSectProp = comBoxAreaSectProp.SelectedItem.ToString();
            }

            ClassApplySectProp.ApplySectPropDefinition applySectPropDefinition = new ApplySectPropDefinition(groupName, frameSectProp, areaSectProp);

            applySectPropDefinitionList.Add(applySectPropDefinition);

            dataGridApplySectProp.Items.Refresh();
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = dataGridApplySectProp.SelectedItems;

            foreach (ClassApplySectProp.ApplySectPropDefinition item in selectedItems)
            {
                applySectPropDefinitionList.Remove(item);
            }

            dataGridApplySectProp.Items.Refresh();

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {


            IsEnabled = false;

            try
            {
                //Call up copy result window
                WindowApplySectPropResult windowApplySectPropResult = new WindowApplySectPropResult(_SapModel, _Plugin, applySectPropDefinitionList);
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            IsEnabled = true;
        }

        private void radioButtonFrame_Checked(object sender, RoutedEventArgs e)
        {
            comBoxAreaSectProp.SelectedIndex = -1;
            comBoxAreaSectProp.IsEnabled = false;

            comBoxFrameSectProp.SelectedIndex = 0;
            comBoxFrameSectProp.IsEnabled = true;
        }

        private void radioButtonArea_Checked(object sender, RoutedEventArgs e)
        {
            comBoxFrameSectProp.SelectedIndex = -1;
            comBoxFrameSectProp.IsEnabled = false;

            comBoxAreaSectProp.SelectedIndex = 0;
            comBoxAreaSectProp.IsEnabled = true;
        }
    }
}
