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

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    /// <summary>
    /// Interaction logic for WindowEtabsToRhinoMain.xaml
    /// </summary>
    public partial class WindowEtabsToRhinoMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        string modelPath = "";
        string modelName = "";

        

        public WindowEtabsToRhinoMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            //Fill in model name (default path)
            
            modelName = SapModel.GetModelFilename(false);
            modelName = System.IO.Path.GetFileNameWithoutExtension(modelName);
            textBoxFileName.Text = modelName;

            ShowDialog();
        }

        #region Button Method

        private void checkBoxFrameExtrusion_Checked(object sender, RoutedEventArgs e)
        {
            frameExtrusionAuto.IsEnabled = true;
            frameExtrusionManual.IsEnabled = true;
        }

        private void checkBoxFrameExtrusion_Unchecked(object sender, RoutedEventArgs e)
        {
            frameExtrusionAuto.IsEnabled = false;
            frameExtrusionManual.IsEnabled = false;
        }

        private void checkBoxAreaExtrusion_Checked(object sender, RoutedEventArgs e)
        {
            areaExtrusionAuto.IsEnabled = true;
            areaExtrusionManual.IsEnabled = true;
        }

        private void checkBoxAreaExtrusion_Unchecked(object sender, RoutedEventArgs e)
        {
            areaExtrusionAuto.IsEnabled = false;
            areaExtrusionManual.IsEnabled = false;
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

        private void buttonResetPath_Click(object sender, RoutedEventArgs e)
        {
            //Fill in model path (default path)
            textBoxFilePath.Text = modelPath;

            //Fill in model name (default path)
            textBoxFileName.Text = modelName;

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {

            //Casting file path
            string filePath = textBoxFilePath.Text;
            string fileName = textBoxFileName.Text;

            //Collect which boxes are selected
            Dictionary<string, bool?> selectedItems = new Dictionary<string, bool?>()
            {
                {"extrusionFrameModel", checkBoxFrameExtrusion.IsChecked },
                {"extrusionAreaModel", checkBoxAreaExtrusion.IsChecked },
                {"frameExtrusionAuto", frameExtrusionAuto.IsChecked },
                {"frameExtrusionManual", frameExtrusionManual.IsChecked },
                {"areaExtrusionAuto", areaExtrusionAuto.IsChecked },
                {"areaExtrusionManual", areaExtrusionManual.IsChecked },
            };

            IsEnabled = false;

            //Call up progress result window
            src.etabs_to_rhino.WindowEtabsToRhinoProgress windowEtabsToRhinoProgress = new src.etabs_to_rhino.WindowEtabsToRhinoProgress(_SapModel, _Plugin, filePath, fileName, selectedItems);

            IsEnabled = true;

            Close();
        }


        #endregion

        private void frameExtrusionAuto_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
