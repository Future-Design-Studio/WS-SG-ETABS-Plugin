using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Reflection;

namespace WS_SG_ETABS_Plugin
{
    /// <summary>
    /// Interaction logic for HelperOpenPDF.xaml
    /// </summary>
    public partial class WindowOpenPdf : Window
    {
        public WindowOpenPdf()
        {
            InitializeComponent();


            string folder = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            System.Uri uri = new System.Uri($"{folder}\\src\\resources\\Tools.PDF");

            pdfViewer.Source = uri;

            ShowDialog();
        }
    }
}
