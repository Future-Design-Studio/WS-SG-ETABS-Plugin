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
using Microsoft.Win32;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    /// <summary>
    /// Interaction logic for WindowEtabsEtabsOpenFile.xaml
    /// </summary>
    public partial class WindowEtabsEtabsOpenFile : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;
        bool firstPathAssigned = true;
        public WindowEtabsEtabsOpenFile(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (modelToLocked)
            {
                MessageBox.Show("Current ETABS file is locked! Please unlock the model!");
                return;
            }

            //Get model path as a starting directory
            modelPath = SapModel.GetModelFilepath();
            InitializeComponent();
            ShowDialog();
        }

        private void filePath_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "ETABS files(*.edb)|*.edb";

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

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {

            //Casting file path
            string filePathFrom = filePath.Text;

            IsEnabled = false;
            //Call up loading window
            src.loads_etabs_etabs.WindowEtabsEtabsOpenFileLoading windowOpenFileLoading = new src.loads_etabs_etabs.WindowEtabsEtabsOpenFileLoading(_SapModel, _Plugin, filePathFrom);
            IsEnabled = true;
        }
    }
}
