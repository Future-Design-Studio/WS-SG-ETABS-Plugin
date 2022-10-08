using ETABSv1;
using System;
using System.Collections.Generic;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedMain.xaml
    /// </summary>
    public partial class WindowRCBSchedMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        public WindowRCBSchedMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            fileName.Text = "rc_beam_schedule";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            filePath.Text = modelPath;

            /*
            //testing excel
            var excel = new Excel.Application();

            var workBooks = excel.Workbooks;
            var workBook = workBooks.Add();
            var workSheet = (Excel.Worksheet)excel.ActiveSheet;

            workSheet.Cells[1, "A"] = "Foo";
            workSheet.Cells[1, "B"] = "Bar";

            workBook.SaveAs(modelPath+"test", Excel.XlFileFormat.xlOpenXMLWorkbook);
            workBook.Close();
            */

            ShowDialog();
        }

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            filePath.Text = modelPath;
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = filePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = dialog.SelectedPath;
            }

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            string name = fileName.Text;
            string path = filePath.Text;

            IsEnabled = false;
            //Call up carbon result window
            src.rc_beam_schedule.WindowRCBSchedResult windowRCBSchedResult = new src.rc_beam_schedule.WindowRCBSchedResult(_SapModel, _Plugin, name, path);
            IsEnabled = true;
        }
    }
}
