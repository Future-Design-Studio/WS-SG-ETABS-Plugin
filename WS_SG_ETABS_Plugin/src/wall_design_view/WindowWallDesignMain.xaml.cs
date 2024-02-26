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

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    /// <summary>
    /// Interaction logic for WindowWallDesignMain.xaml
    /// </summary>
    public partial class WindowWallDesignMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        //List of tables to save joint reaction and grouping table
        List<src.HelperEtabs.DatabaseTable> databaseTableListGlobal = new List<HelperEtabs.DatabaseTable>();
        List<Dictionary<string, string>> tableInListPierDesignResultGlobal = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> tableInListSpandrelDesignResultGlobal = new List<Dictionary<string, string>>();

        public WindowWallDesignMain(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "wall_design_result";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            /*
            //Fill in combobox for output case
            comBoxOutputCase.ItemsSource = listChoosenLoadCase;
            comBoxOutputCase.SelectedIndex = 0;
            */

            //Fill in combobox for direction
            comBoxTypeOfFailure.ItemsSource = src.HelperEtabs.typeOfFailureList;
            comBoxTypeOfFailure.SelectedIndex = 0;

            //Save databaseTableList to global variable
            databaseTableListGlobal = databaseTableList;

            //Save tableInListPierDesignResult and tableInListSpandrelDesignResult to global variable
            tableInListPierDesignResultGlobal = tableInListPierDesignResult;
            tableInListSpandrelDesignResultGlobal = tableInListSpandrelDesignResult;

            //Fill in combobox for available groups
            //src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            comBoxGroup.ItemsSource = listChoosenGroup;
            comBoxGroup.SelectedIndex = 0;

            ShowDialog();
        }

        #region Button Method

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            textBoxFilePath.Text = modelPath;
        }

        /*
        private void buttonAddLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailLoadCase, listBoxChoosenLoadCase);
        }

        private void buttonRemoveLoadCase_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenLoadCase, listBoxAvailLoadCase);
        }
        */

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                /*
                //Casting listbox content to list
                List<string> listChoosenLoadCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                */

                string fileName = textBoxFileName.Text;
                string filePath = textBoxFilePath.Text;

                //string outputCase = comBoxOutputCase.SelectedValue.ToString();
                //List<string> listOutputCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                string choosenGroup = comBoxGroup.Text;
                string typeOfFailure = comBoxTypeOfFailure.SelectedValue.ToString();

                int legendTextSize = Convert.ToInt32(textBoxLegendTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int tickLabelSize = Convert.ToInt32(textBoxTickLabelSize.Text);

                //Call up copy result window
                WindowWallDesignResult windowWallDesignResult = new WindowWallDesignResult(databaseTableListGlobal, choosenGroup, typeOfFailure, tableInListPierDesignResultGlobal, 
                    tableInListSpandrelDesignResultGlobal, titleSize, axisTitleSize, tickLabelSize);
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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = textBoxFilePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFilePath.Text = dialog.SelectedPath;
            }
        }



        #endregion

    }
}
