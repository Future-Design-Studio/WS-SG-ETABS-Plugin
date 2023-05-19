using Accord.MachineLearning;
using System;
using System.Collections;
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

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule.ML
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedKMeans.xaml
    /// </summary>
    public partial class WindowRCBSchedKMeans : Window
    {
        public WindowRCBSchedKMeans(List<double> numClusterList, List<double> errorList)
        {
            InitializeComponent();

            double[] xArray = numClusterList.ToArray();
            double[] yArray = errorList.ToArray();

            Plot.Plot.AddScatter(xArray, yArray, markerSize: 10, lineWidth: 1);

            Plot.Plot.AxisScaleLock(true);
            Plot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            Plot.Refresh();

            ShowDialog();
    }
    }
}
