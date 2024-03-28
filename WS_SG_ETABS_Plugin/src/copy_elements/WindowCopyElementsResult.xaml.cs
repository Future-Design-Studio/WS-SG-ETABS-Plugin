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

namespace WS_SG_ETABS_Plugin.src.copy_elements
{
    /// <summary>
    /// Interaction logic for WindowCopyElementsResult.xaml
    /// </summary>
    public partial class WindowCopyElementsResult : Window
    {

        public WindowCopyElementsResult()
        {
            InitializeComponent();
            ShowDialog();

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
