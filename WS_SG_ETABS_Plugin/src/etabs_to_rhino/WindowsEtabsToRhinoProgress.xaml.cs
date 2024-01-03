using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    /// <summary>
    /// Interaction logic for WindowsEtabsToRhinoProgress.xaml
    /// </summary>
    public partial class WindowEtabsToRhinoProgress : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string _filePath;
        string _fileName;
        string _errorMessage;
        string _fullErrorMessage;
        Dictionary<string, bool?> _selectedItems;

        public WindowEtabsToRhinoProgress(cSapModel SapModel, cPluginCallback Plugin, string filePath, string fileName, Dictionary<string,bool?> selectedItems)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            _filePath = filePath;
            _fileName = fileName;
            _selectedItems = selectedItems;


            InitializeComponent();

            Show();
            

            ExecuteTaskWithProgress();
            

            
        }

        private async void ExecuteTaskWithProgress()
        {
            pbStatus.Value = 0;

            var progressStatus = new Progress<int>(x => pbStatus.Value = x);
            var progressDescription = new Progress<string>(x => pbDescription.Text = x);

            try
            {
                await Task.Run(() => src.etabs_to_rhino.FunctionEtabsToRhino.ExportEtabsToRhino(_SapModel, _filePath, _fileName, _selectedItems, progressStatus, progressDescription));
                textblockReceipt.Text = "Done!";
            }
            catch (Exception ex) 
            {
                _errorMessage = ex.Message;
                _fullErrorMessage = ex.StackTrace;
                buttonLog.IsEnabled = true;
                buttonLog.Opacity = 100;

                pbStatus.Value = 0;
                pbDescription.Text = _errorMessage;
            }

            if (pbStatus.Value == 0) 
            {
                pbStatus.Foreground = Brushes.Red;
                textblockReceipt.Text = "Error!";
            }

            pbStatus.Value = 100;
            

            Thread.Sleep(500);

            buttonOk.IsEnabled = true;
            buttonOk.Opacity = 100;

            //Close();

            

            //Call up export result window
            //src.etabs_to_rhino.WindowEtabsToRhinoResult windowEtabsToRhinoResult = new src.etabs_to_rhino.WindowEtabsToRhinoResult(_SapModel, _Plugin, _filePath, _fileName);

            

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void buttonLog_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_errorMessage);
            sb.AppendLine(_fullErrorMessage);
            File.AppendAllText(_filePath + _fileName + "_ErrorMessage.txt", sb.ToString());

            textblockReceipt.Text = "Error Message saved!";
        }
    }
}
