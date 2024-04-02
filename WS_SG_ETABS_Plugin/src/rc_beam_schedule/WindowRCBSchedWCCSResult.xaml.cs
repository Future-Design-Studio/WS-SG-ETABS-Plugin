using ScottPlot;
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

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedWCCSResult.xaml
    /// </summary>
    public partial class WindowRCBSchedWCCSResult : Window
    {
        public WindowRCBSchedWCCSResult(List<int> numOfClustersList, List<double> sillouettheCoefficientList, List<double> WCCSList)
        {
            InitializeComponent();

            List<double> numOfClustersListDouble = numOfClustersList.ConvertAll(x => (double)x);

            double[] xArray = numOfClustersListDouble.ToArray();
            double[] sillouettheCoefficientArray = sillouettheCoefficientList.ToArray();
            double[] WCCSArray = WCCSList.ToArray();

            SillouettheScorePlot.Plot.AddScatter(xArray, sillouettheCoefficientArray, markerSize: 10, lineWidth: 1);
            WCCSPlot.Plot.AddScatter(xArray, WCCSArray, markerSize: 10, lineWidth: 1);

            SillouettheScorePlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            SillouettheScorePlot.Plot.Title("Sillouetthe Coefficient");
            SillouettheScorePlot.Plot.XLabel("Number of clusters");

            WCCSPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            WCCSPlot.Plot.Title("Within-Cluster Sum of Squares (WCSS)");
            WCCSPlot.Plot.XLabel("Number of clusters");

            SillouettheScorePlot.Refresh();
            WCCSPlot.Refresh();

            ShowDialog();
        }
    }
}
