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

namespace WS_SG_ETABS_Plugin.src.shell_uni_orient
{
    /// <summary>
    /// Interaction logic for ShellUniMain.xaml
    /// </summary>
    public partial class WindowShellUniMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowShellUniMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in available groups list box
            src.HelperUI.PopulateListBoxAvailGroup(_SapModel, listBoxAvailGroup);

            //Fill in combobox for direction
            comBoxShellDirection.ItemsSource = src.shell_uni_orient.FunctionShellUni.shellDirectionList;
/*            comBoxShellDirection.DisplayMemberPath = "ConcreteGrade";
            comBoxShellDirection.SelectedValuePath = "Emission";*/
            comBoxShellDirection.SelectedIndex = 0;

            ShowDialog();

        }

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
            //Casting listbox content to list
            List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

            string shellDir =  comBoxShellDirection.SelectedValue.ToString();

            //Call up carbon result window
            src.shell_uni_orient.WindowShellUniResult windowShellUniResult = new src.shell_uni_orient
                .WindowShellUniResult(_SapModel, _Plugin, listChoosenGroup, shellDir);
        }

        
    }
}
