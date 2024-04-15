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

namespace WS_SG_ETABS_Plugin.src.transfer_midas
{
    /// <summary>
    /// Interaction logic for WindowTransferMidasProgress.xaml
    /// </summary>
    public partial class WindowTransferMidasProgress : Window
    {
        public WindowTransferMidasProgress(string title)
        {
            InitializeComponent();

            labelProgressTitle.Content = title;
            pbStatus.Value = 0;

            ShowDialog();
        }

        public void UpdateProgress(double newProgress)
        {
            pbStatus.Value = newProgress;
        }


    }
}
