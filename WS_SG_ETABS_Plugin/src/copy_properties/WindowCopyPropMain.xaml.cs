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
using Microsoft.Win32;

namespace WS_SG_ETABS_Plugin.src.copy_properties
{
    /// <summary>
    /// Interaction logic for WindowCopyPropMain.xaml
    /// </summary>
    public partial class WindowCopyPropMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        public WindowCopyPropMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            ShowDialog();
        }

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            filePath.Text = modelPath;
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            /*
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = filePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = dialog.SelectedPath;
            }
            */

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "ETABS files(*.edb)|*.edb";

            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = openFileDialog.FileName;
            }

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
            //Call up carbon result window
            src.copy_properties.WindowCopyPropResult windowCopyPropResult = new src.copy_properties.WindowCopyPropResult(_SapModel, _Plugin, filePathFrom);
            IsEnabled = true;
        }
    } 

}
