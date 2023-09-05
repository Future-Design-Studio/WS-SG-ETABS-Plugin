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

            var progress = new Progress<int>(x => pbStatus.Value = x);

            await Task.Run(() => src.etabs_to_rhino.FunctionEtabsToRhino.ExportEtabsToRhino(_SapModel, _filePath, _fileName, _selectedItems, progress));

            pbStatus.Value = 100;

            Thread.Sleep(500);

            Close();

            

            //Call up export result window
            src.etabs_to_rhino.WindowEtabsToRhinoResult windowEtabsToRhinoResult = new src.etabs_to_rhino.WindowEtabsToRhinoResult(_SapModel, _Plugin, _filePath, _fileName);

            

        }
    }
}
