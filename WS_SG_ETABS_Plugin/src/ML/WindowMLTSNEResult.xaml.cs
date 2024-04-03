using ScottPlot;
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

namespace WS_SG_ETABS_Plugin.src.ML
{ 
/// <summary>
/// Interaction logic for WindowRCBSchedTSNEResult.xaml
/// </summary>
    public partial class WindowMLTSNEResult : Window
    {
        public WindowMLTSNEResult(int numClusters, int[] labels, double[][] TSNEresults)
        {
            InitializeComponent();

            for (int cluster = 0; cluster < numClusters; cluster++)
            {
                List<double> xList = new List<double>();
                List<double> yList = new List<double>();

                for (int i = 0; i < labels.Count(); i++)
                {
                    if (labels[i] == cluster)
                    {
                        xList.Add(TSNEresults[i][0]);
                        yList.Add(TSNEresults[i][1]);
                    }
                }

                double[] xArray = xList.ToArray();
                double[] yArray = yList.ToArray();
                Plot.Plot.AddScatter(xArray, yArray, markerSize: 10, lineWidth: 0, label: $"Cluster {cluster}");
            }


            Plot.Plot.Legend();
            Plot.Plot.AxisScaleLock(true);
            Plot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            Plot.Refresh();

            ShowDialog();
        }
    }
}
