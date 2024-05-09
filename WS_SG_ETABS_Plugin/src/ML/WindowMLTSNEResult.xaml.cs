using Accord.MachineLearning;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using WS_SG_ETABS_Plugin.src.pad_footing_plot;

namespace WS_SG_ETABS_Plugin.src.ML
{ 
/// <summary>
/// Interaction logic for WindowRCBSchedTSNEResult.xaml
/// </summary>
    public partial class WindowMLTSNEResult : Window
    {
        int _numClusters;
        int[] _labels;
        double[][] _observations;

        public WindowMLTSNEResult(int numClusters, int[] labels, double[][] TSNEresults, double[][] observations)
        {
            _numClusters = numClusters;
            _labels = labels;
            _observations = observations;

            InitializeComponent();

            List<string> clusterNameList = new List<string>();

            for (int cluster = 0; cluster < numClusters; cluster++)
            {
                string clusterName = $"Cluster {cluster}";
                clusterNameList.Add(clusterName);

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

                //Decide group color plot
                System.Drawing.Color color = new System.Drawing.Color();
                double colorMapFraction = Convert.ToDouble(cluster) / (Convert.ToDouble(numClusters - 1));
                color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, 1);

                double[] xArray = xList.ToArray();
                double[] yArray = yList.ToArray();
                TSNEPlot.Plot.AddScatter(xArray, yArray, markerSize: 10, lineWidth: 0, label: clusterName, color: color);

                //Count num of dimensions (features)
                int numOfDimensions = observations[0].Count();

                List<double> xListFeaturesPlot = new List<double>();
                
                //xAxis
                for(double dimensionIndex = 0;  dimensionIndex < numOfDimensions; dimensionIndex++)
                {
                    xListFeaturesPlot.Add(dimensionIndex / (numOfDimensions - 1) * 10);
                }

                for (int dataIndex = 0; dataIndex < labels.Count(); dataIndex++)
                {
                    List<double> yListFeaturesPlot = new List<double>();
                    if (labels[dataIndex] == cluster)
                    {
                        for (int dimensionIndex = 0; dimensionIndex < numOfDimensions; dimensionIndex++)
                        {
                            yListFeaturesPlot.Add(observations[dataIndex][dimensionIndex]);
                        }

                        double[] xArrayFeaturesPlot = xListFeaturesPlot.ToArray();
                        double[] yArrayFeaturesPlot = yListFeaturesPlot.ToArray();
                        FeaturesPlot.Plot.AddScatter(xArrayFeaturesPlot, yArrayFeaturesPlot, markerSize: 10, lineWidth: 1, color: color, markerShape:MarkerShape.none);
                    }
                }

            }

            TSNEPlot.Plot.Title("TSNE Plot");
            TSNEPlot.Plot.Legend();
            TSNEPlot.Plot.AxisScaleLock(true);
            TSNEPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            TSNEPlot.Refresh();

            FeaturesPlot.Plot.Title("Features Plot");
            FeaturesPlot.Refresh();

            listBoxCluster.ItemsSource = clusterNameList;
            //listBoxCluster.SelectAll();

            ShowDialog();
        }

        private void listBoxCluster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> selectedClusters = listBoxCluster.SelectedItems.OfType<string>().ToList();

            ScottPlot.AxisLimits axisLimits = FeaturesPlot.Plot.GetAxisLimits();

            FilterFeaturesPlot(_numClusters, _labels, _observations, selectedClusters, axisLimits, FeaturesPlot);
        }

        private static void FilterFeaturesPlot(int numClusters, int[] labels, double[][] observations, List<string> selectedClusters, ScottPlot.AxisLimits axisLimits, WpfPlot FeaturesPlot)
        {
            FeaturesPlot.Reset();

            foreach (string selectedCluster in selectedClusters)
            {
                //Take the second half so cluster become index only
                int clusterName = Convert.ToInt32(selectedCluster.Split(' ')[1]);

                for (int cluster = 0; cluster < numClusters; cluster++)
                {
                    if (clusterName == cluster)
                    {
                        //Decide group color plot
                        System.Drawing.Color color = new System.Drawing.Color();
                        double colorMapFraction = Convert.ToDouble(cluster) / (Convert.ToDouble(numClusters - 1));
                        color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, 1);

                        //Count num of dimensions (features)
                        int numOfDimensions = observations[0].Count();

                        List<double> xListFeaturesPlot = new List<double>();

                        //xAxis
                        for (double dimensionIndex = 0; dimensionIndex < numOfDimensions; dimensionIndex++)
                        {
                            xListFeaturesPlot.Add(dimensionIndex / (numOfDimensions - 1) * 10);
                        }

                        for (int dataIndex = 0; dataIndex < labels.Count(); dataIndex++)
                        {
                            List<double> yListFeaturesPlot = new List<double>();
                            if (labels[dataIndex] == cluster)
                            {
                                for (int dimensionIndex = 0; dimensionIndex < numOfDimensions; dimensionIndex++)
                                {
                                    yListFeaturesPlot.Add(observations[dataIndex][dimensionIndex]);
                                }

                                double[] xArrayFeaturesPlot = xListFeaturesPlot.ToArray();
                                double[] yArrayFeaturesPlot = yListFeaturesPlot.ToArray();
                                FeaturesPlot.Plot.AddScatter(xArrayFeaturesPlot, yArrayFeaturesPlot, markerSize: 10, lineWidth: 1, color: color, markerShape: MarkerShape.none);
                            }
                        }
                    }

                }
            }

            FeaturesPlot.Plot.SetAxisLimits(axisLimits);
            FeaturesPlot.Plot.Title("Features Plot");
            FeaturesPlot.Refresh();

        }
    }

}
