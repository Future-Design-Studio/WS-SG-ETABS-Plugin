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

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedPCAResult.xaml
    /// </summary>
    public partial class WindowRCBSchedPCAResult : Window
    {
        public WindowRCBSchedPCAResult(List<ClassFunctionRCBSched.RcBeamForMLInput> rcBeamForMLInputList, int numClusters)
        {
            InitializeComponent();

            for (int cluster = 0; cluster < numClusters; cluster++)
            {
                List<double> xList = new List<double>();
                List<double> yList = new List<double>();

                foreach (ClassFunctionRCBSched.RcBeamForMLInput rcBeamForMLInput in rcBeamForMLInputList)
                {
                    //rcBeamForMLInput.Cluster is from 1 to num of cluster
                    if (rcBeamForMLInput.Cluster == Convert.ToString(cluster))
                    {
                        xList.Add(rcBeamForMLInput.PC1);
                        yList.Add(rcBeamForMLInput.PC2);
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
