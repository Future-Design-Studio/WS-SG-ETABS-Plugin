using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WS_SG_ETABS_Plugin.src.apply_modifier
{
    /// <summary>
    /// Interaction logic for WindowApplyModifierMain.xaml
    /// </summary>
    public partial class WindowApplyModifierMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<ClassApplyModifier.ModifierDefinition> modifierDefinitionList = new List<ClassApplyModifier.ModifierDefinition>();

        public WindowApplyModifierMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in combobox for available groups
            src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);

            //Datagrid
            dataGridModifier.ItemsSource = modifierDefinitionList;

            

            ShowDialog();

            dataGridModifier.Columns[0].Header = "Group";
            dataGridModifier.Columns[0].Header = "Group";

        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string groupName = comBoxGroup.SelectedItem.ToString();

            double frameModA = Convert.ToDouble(textBoxFrameModA.Text);
            double frameModV2 = Convert.ToDouble(textBoxFrameModV2.Text);
            double frameModV3 = Convert.ToDouble(textBoxFrameModV3.Text);
            double frameModT = Convert.ToDouble(textBoxFrameModT.Text);
            double frameModM2 = Convert.ToDouble(textBoxFrameModM2.Text);
            double frameModM3 = Convert.ToDouble(textBoxFrameModM3.Text);
            double frameModMass = Convert.ToDouble(textBoxFrameModMass.Text);
            double frameModWeight = Convert.ToDouble(textBoxFrameModWeight.Text);

            double areaModF11 = Convert.ToDouble(textBoxAreaModF11.Text);
            double areaModF22 = Convert.ToDouble(textBoxAreaModF22.Text);
            double areaModF12 = Convert.ToDouble(textBoxAreaModF12.Text);
            double areaModM11 = Convert.ToDouble(textBoxAreaModM11.Text);
            double areaModM22 = Convert.ToDouble(textBoxAreaModM22.Text);
            double areaModM12 = Convert.ToDouble(textBoxAreaModM12.Text);
            double areaModV13 = Convert.ToDouble(textBoxAreaModV13.Text);
            double areaModV23 = Convert.ToDouble(textBoxAreaModV23.Text);
            double areaModMass = Convert.ToDouble(textBoxAreaModMass.Text);
            double areaModWeight = Convert.ToDouble(textBoxAreaModWeight.Text);

            ClassApplyModifier.ModifierDefinition modifierDefinition = new ClassApplyModifier.ModifierDefinition(groupName, frameModA, frameModV2, frameModV3, frameModT, frameModM2,
                                                                            frameModM3, frameModMass, frameModWeight, areaModF11,
                                                                            areaModF22, areaModF12, areaModM11, areaModM22,
                                                                            areaModM12, areaModV13, areaModV23, areaModMass,
                                                                            areaModWeight);

            modifierDefinitionList.Add(modifierDefinition);
            dataGridModifier.Items.Refresh();
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = dataGridModifier.SelectedItems;

            foreach(ClassApplyModifier.ModifierDefinition item in selectedItems)
            {
                modifierDefinitionList.Remove(item);
            }

            dataGridModifier.Items.Refresh();

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                //Call up copy result window
                WindowApplyModifierResult windowApplyModifierResult = new WindowApplyModifierResult(
                    _SapModel, _Plugin, modifierDefinitionList);
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
