using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for WindowRCBSchedMLPrefence.xaml
    /// </summary>
    public partial class WindowRCBSchedMLPrefence : Window
    {
        double[][] _observations;
        int[] _labels;

        public WindowRCBSchedMLPrefence(double[][] observations)
        {
            InitializeComponent();

            _observations = observations;
            _labels = new int[observations.Count()];

            textBoxMinNumClusters.Text = "2";
            textBoxMaxNumClusters.Text = (observations.Count() - 1).ToString();
            
            ShowDialog();
        }

        private void buttonPlotWCCS_Click(object sender, RoutedEventArgs e)
        {
            int minNumClusters = Convert.ToInt32(textBoxMinNumClusters.Text);
            int maxNumClusters = Convert.ToInt32(textBoxMaxNumClusters.Text);

            //Get optimal num of clusters
            List<int> numOfClustersList = new List<int>();
            List<double> sillouettheCoefficientList = new List<double>();
            List<double> WCCSList = new List<double>();
            (numOfClustersList, sillouettheCoefficientList, WCCSList) = HelperML.CalculateOptimalNumOfClusters(_observations, minNumClusters, maxNumClusters);

            IsEnabled = false;
            WindowRCBSchedWCSSResult windowRCBSchedWCCSResult = new WindowRCBSchedWCSSResult(numOfClustersList, sillouettheCoefficientList, WCCSList);
            IsEnabled = true;
        }

        private void buttonPlotClusters_Click(object sender, RoutedEventArgs e)
        {

            int numClusters = Convert.ToInt32(textBoxNumClusters.Text);
            double perplexity = Convert.ToDouble(textBoxPerplexity.Text);

            //TSNE to 2 axis
            Accord.Math.Random.Generator.Seed = 0;
            Accord.MachineLearning.Clustering.TSNE tSNE = new Accord.MachineLearning.Clustering.TSNE()
            {
                NumberOfOutputs = 2,
                //Perplexity = Math.Floor((double) rcBeamForMLInputList.Count() / 3)
                Perplexity = perplexity
            };

            double[][] TSNEresults = null;
            try
            {
                TSNEresults = tSNE.Transform(_observations);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            //Accord KMeans
            Accord.Math.Random.Generator.Seed = 0;
            KMeans kmeans = new KMeans(k: numClusters);
            var clusters = kmeans.Learn(_observations);
            int[] labels = clusters.Decide(_observations);

            IsEnabled = false;
            WindowRCBSchedTSNEResult WindowRCBSchedTSNEResult = new WindowRCBSchedTSNEResult(numClusters, labels, TSNEresults);
            IsEnabled = true;

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            int numClusters = Convert.ToInt32(textBoxNumClusters.Text);
            double perplexity = Convert.ToDouble(textBoxPerplexity.Text);

            //TSNE to 2 axis
            Accord.Math.Random.Generator.Seed = 0;
            Accord.MachineLearning.Clustering.TSNE tSNE = new Accord.MachineLearning.Clustering.TSNE()
            {
                NumberOfOutputs = 2,
                //Perplexity = Math.Floor((double) rcBeamForMLInputList.Count() / 3)
                Perplexity = perplexity
            };

            double[][] TSNEresults = null;
            try
            {
                TSNEresults = tSNE.Transform(_observations);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //Accord KMeans
            Accord.Math.Random.Generator.Seed = 0;
            KMeans kmeans = new KMeans(k: numClusters);
            var clusters = kmeans.Learn(_observations);
            int[] labels = clusters.Decide(_observations);

            IsEnabled = false;
            WindowRCBSchedTSNEResult WindowRCBSchedTSNEResult = new WindowRCBSchedTSNEResult(numClusters, labels, TSNEresults);
            IsEnabled = true;

            _labels = labels;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public int[] GetLabels()
        {
            return _labels;
        }
    }
}
