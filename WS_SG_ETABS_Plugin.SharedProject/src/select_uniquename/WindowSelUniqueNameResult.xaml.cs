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

namespace WS_SG_ETABS_Plugin.src.select_uniquename
{
    /// <summary>
    /// Interaction logic for WindowSelUniqueNameResult.xaml
    /// </summary>
    public partial class WindowSelUniqueNameResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        public WindowSelUniqueNameResult(cSapModel SapModel, cPluginCallback Plugin, string pointUniqueName, string frameUniqueName, string areaUniqueName)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Main logic
            (bool isError, string errorMessage) = src.select_uniquename.FunctionSelUniqueName.SelectByUniqueName(_SapModel, pointUniqueName, frameUniqueName, areaUniqueName);

            if (isError)
            {
                return;
            }

            InitializeComponent();

            //Fill in number of warning
            labelError.Text = errorMessage;

            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
