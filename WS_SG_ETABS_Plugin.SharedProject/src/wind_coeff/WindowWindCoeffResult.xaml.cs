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

namespace WS_SG_ETABS_Plugin.src.wind_coeff
{
    /// <summary>
    /// Interaction logic for WindowWindCoeffResult.xaml
    /// </summary>
    public partial class WindowWindCoeffResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        public WindowWindCoeffResult(cSapModel SapModel, cPluginCallback Plugin, 
            List<src.wind_coeff.ClassWindCoeff.WindLoad> windLoadList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Mainlogic
            bool isError = src.wind_coeff.FunctionWindCoeff.AssignWindLoads(_SapModel, _Plugin, windLoadList);

            /*
            if (isError)
            {
                MessageBox.Show("Error, please let Mani know what happened");
                return;
            }
            */

            InitializeComponent();

            ShowDialog();

        }
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
