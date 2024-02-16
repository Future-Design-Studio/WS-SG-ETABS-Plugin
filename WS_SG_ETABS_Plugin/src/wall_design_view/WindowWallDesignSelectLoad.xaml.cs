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
using WS_SG_ETABS_Plugin.src.plot_reaction;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    /// <summary>
    /// Interaction logic for WindowWallDesignSelectLoad.xaml
    /// </summary>
    public partial class WindowWallDesignSelectLoad : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowWallDesignSelectLoad(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }

            InitializeComponent();

            //Fill in group list box
            src.HelperUI.PopulateListBoxAvailGroup(_SapModel, listBoxAvailGroup);

            ShowDialog();
        }

        private void buttonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
        }

        private void buttonRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                //Casting listbox content to list
                List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

                //Call up window
                WindowWallDesignLoading windowWallDesignLoading = new WindowWallDesignLoading(_SapModel, _Plugin, listChoosenGroup);
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
