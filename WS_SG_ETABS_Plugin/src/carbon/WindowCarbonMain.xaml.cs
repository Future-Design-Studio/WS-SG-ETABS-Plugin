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

namespace WS_SG_ETABS_Plugin.src.carbon
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WindowCarbonMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowCarbonMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

/*            //Check if table required for carbon calculation exist
            bool requiredTableExist = src.carbon.FunctionCarbon.CheckTableExistCarbon(_SapModel);

            if (!requiredTableExist)
            {
                return;
            }*/

            InitializeComponent();
            
            //Fill in available groups list box
            src.HelperUI.PopulateListBoxAvailGroup(_SapModel, listBoxAvailGroup);

            //Fill in combobox for column
            comBoxColumnConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
            comBoxColumnConcreteGrade.DisplayMemberPath = "ConcreteGrade";
            comBoxColumnConcreteGrade.SelectedValuePath = "Emission";
            comBoxColumnConcreteGrade.SelectedIndex = 0;

            //Fill in combobox for beam
            comBoxBeamConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
            comBoxBeamConcreteGrade.DisplayMemberPath = "ConcreteGrade";
            comBoxBeamConcreteGrade.SelectedValuePath = "Emission";
            comBoxBeamConcreteGrade.SelectedIndex = 0;

            //Fill in combobox for brace
            comBoxBraceConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
            comBoxBraceConcreteGrade.DisplayMemberPath = "ConcreteGrade";
            comBoxBraceConcreteGrade.SelectedValuePath = "Emission";
            comBoxBraceConcreteGrade.SelectedIndex = 0;

            //Fill in combobox for slab
            comBoxFloorConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
            comBoxFloorConcreteGrade.DisplayMemberPath = "ConcreteGrade";
            comBoxFloorConcreteGrade.SelectedValuePath = "Emission";
            comBoxFloorConcreteGrade.SelectedIndex = 0;

            //Fill in combobox for wall
            comBoxWallConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
            comBoxWallConcreteGrade.DisplayMemberPath = "ConcreteGrade";
            comBoxWallConcreteGrade.SelectedValuePath = "Emission";
            comBoxWallConcreteGrade.SelectedIndex = 0;

            //Put ShowDialog at last after completing constructor
            ShowDialog();
        }


        #region Button Method
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            //Compile input into 1 dictionary
            Dictionary<string, string> inputCarbon = new Dictionary<string, string>() {
                            { "Column Concrete Emission", comBoxColumnConcreteGrade.SelectedValue.ToString()},
                            { "Column Rebar Ratio", textBoxColumnRebar.Text.ToString() },
                            { "Beam Concrete Emission", comBoxBeamConcreteGrade.SelectedValue.ToString() },
                            { "Beam Rebar Ratio", textBoxBeamRebar.Text.ToString() },
                            { "Brace Concrete Emission", comBoxBraceConcreteGrade.SelectedValue.ToString() },
                            { "Brace Rebar Ratio", textBoxBraceRebar.Text.ToString() },
                            { "Floor Concrete Emission", comBoxFloorConcreteGrade.SelectedValue.ToString() },
                            { "Floor Rebar Ratio", textBoxFloorRebar.Text.ToString() },
                            { "Wall Concrete Emission", comBoxWallConcreteGrade.SelectedValue.ToString() },
                            { "Wall Rebar Ratio", textBoxWallRebar.Text.ToString() }
                        };

            /*            Dictionary<string, string> inputCarbon = new Dictionary<string, string>();
                        inputCarbon.Add("Column Concrete Emission", comBoxColumnConcreteGrade.SelectedValue.ToString());
                        inputCarbon.Add("Column Rebar Ratio", textBoxColumnRebar.Text.ToString());
                        inputCarbon.Add("Beam Concrete Emission", comBoxBeamConcreteGrade.SelectedValue.ToString());
                        inputCarbon.Add("Beam Rebar Ratio", textBoxBeamRebar.Text.ToString());
                        inputCarbon.Add("Brace Concrete Emission", comBoxBraceConcreteGrade.SelectedValue.ToString());
                        inputCarbon.Add("Brace Rebar Ratio", textBoxBraceRebar.ToString());
                        inputCarbon.Add("Slab Concrete Emission", comBoxSlabConcreteGrade.SelectedValue.ToString());
                        inputCarbon.Add("Slab Rebar Ratio", textBoxSlabRebar.Text.ToString());*/

            //Casting listbox content to list
            List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

            //Check if checkbox all is checked
            bool? includeAll = checkBoxAll.IsChecked;

            //Call up carbon result window
            src.carbon.WindowCarbonResult windowCarbonResult = new src.carbon.WindowCarbonResult(_SapModel, _Plugin, inputCarbon, listChoosenGroup, includeAll);
        }

        private void checkBoxAll_Checked(object sender, RoutedEventArgs e)
        {
            listBoxAvailGroup.IsEnabled = false;
            listBoxChoosenGroup.IsEnabled = false;
            buttonAdd.IsEnabled = false;
            buttonRemove.IsEnabled = false;
        }

        private void checkBoxAll_Unchecked(object sender, RoutedEventArgs e)
        {
            listBoxAvailGroup.IsEnabled = true;
            listBoxChoosenGroup.IsEnabled = true;
            buttonAdd.IsEnabled = true;
            buttonRemove.IsEnabled = true;
        }

        #endregion


    }
}
