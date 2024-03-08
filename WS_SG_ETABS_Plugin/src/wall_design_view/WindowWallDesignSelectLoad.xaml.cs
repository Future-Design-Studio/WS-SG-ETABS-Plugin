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

        string modelPath;
        bool firstPathAssigned = true;

        public WindowWallDesignSelectLoad(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Get model path as a starting directory
            modelPath = SapModel.GetModelFilepath();

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }

            //Fill in group list box
            src.HelperUI.PopulateListBoxAvailGroup(_SapModel, listBoxAvailGroup);

            //Check extract from etabs
            radioButtonExtractEtabs.IsChecked = true;

            ShowDialog();
        }

        
        private void radioButtonExtractExcel_Checked(object sender, RoutedEventArgs e)
        {
            buttonModifyPath.IsEnabled = true;
        }

        private void radioButtonExtractEtabs_Checked(object sender, RoutedEventArgs e)
        {
            buttonModifyPath.IsEnabled = false;
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

                //Check extraction data source
                bool? extractEtabs = radioButtonExtractEtabs.IsChecked;
                bool? extractExcel = radioButtonExtractExcel.IsChecked;
                string excelFilePath = filePath.Text;

                //Call up window
                WindowWallDesignLoading windowWallDesignLoading = new WindowWallDesignLoading(_SapModel, _Plugin, listChoosenGroup, extractEtabs, extractExcel, excelFilePath);
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

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Excel files|*.xls;*.xlsx;*.xlsm";

            //For first time path assignment, initial directory is where model is. After that use current path as a initial directory
            if (firstPathAssigned)
            {
                openFileDialog.InitialDirectory = modelPath;
            }
            else
            {
                openFileDialog.InitialDirectory = filePath.Text;
            }


            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = openFileDialog.FileName;
            }

            //After first time assigning path, set this flag to false
            firstPathAssigned = false;
        }

    }
}
