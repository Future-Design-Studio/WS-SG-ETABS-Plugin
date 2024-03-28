using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperML
    {
        public static void CalculateOptimalNumOfClusters(double[][] observations)
        {
            List<double> sillouettheCoefficientList = new List<double>();

            //Loop through all the possible cluster
            for (int numClusters = 0; numClusters < observations.Count(); numClusters++)
            {
                if (numClusters < 2)
                {
                    sillouettheCoefficientList.Add(0);
                }
                else
                {
                    int[] result;
                    double[][] centroids;

                    (result, centroids) = RunKMeans(observations, numClusters);
                    sillouettheCoefficientList.Add(GetSillouettheCoefficient(observations, result, numClusters, centroids));
                }
            }

        }

        public static double GetSillouettheCoefficient(double[][] observations, int[] clusterLabels, int numClusters, double[][] clusterCentroids)
        {
            double[][] distanceBetweenClusters = GetDistanceBetweenClusters(observations, clusterLabels, numClusters, clusterCentroids);
            double sumSillouettheScore = 0;

            //Loop through all the clusters
            //CLuster label starts from 0
            for (int cluster = 0; cluster < numClusters; cluster++)
            {
                //Get closest cluster index
                double[] distanceBetweenClustersForSpecific = distanceBetweenClusters[cluster];

                bool firstLoop = true;
                int minIndex = 0;
                double minDistance = 0;
                int index = 0;
                foreach(double distance in distanceBetweenClustersForSpecific)
                {
                    if (distance != 0)
                    {
                        if (firstLoop)
                        {
                            minIndex = index;
                            minDistance = distance;
                            firstLoop = false;
                        }

                        if (distance < minDistance)
                        {
                            minIndex = index;
                            minDistance = distance;
                        }

                    }
                    index++;

                }

                int closestCluster = minIndex;

                //Loop through all data
                List<double[]> dataClusterSelf = new List<double[]>();
                List<double[]> dataClusterClosest = new List<double[]>();
                for (int i = 0; i < observations.Count(); i++)
                {
                    if (clusterLabels[i] == cluster)
                    {
                        dataClusterSelf.Add(observations[i]);
                    }

                    if (clusterLabels[i] == closestCluster)
                    {
                        dataClusterClosest.Add(observations[i]);
                    }
                }

                //Calculate distance between points within cluster to its own centroid (cohession)
                double[] centroid = clusterCentroids[cluster];
                double sumDistance = 0;

                //If only contain 1 points
                if (dataClusterClosest.Count() == 1)
                {
                    sumDistance = 0.001;
                }
                else
                {
                    foreach (double[] data in dataClusterSelf)
                    {
                        sumDistance += HelperMethod.GetEcludianDistance(centroid, data);
                    }
                }

                double a = sumDistance / dataClusterSelf.Count();

                //Go through other cluster to find the min avg distance

                //Calculate distance between points the closets cluster to centroid of above cluster
                sumDistance = 0;

                foreach (double[] data in dataClusterClosest)
                {
                    sumDistance += HelperMethod.GetEcludianDistance(centroid, data);
                }

                double b = sumDistance / dataClusterClosest.Count();

                sumSillouettheScore += (b - a) / Math.Max(a, b);
            }

            return sumSillouettheScore / numClusters;
        }

        public static double[][] GetDistanceBetweenClusters(double[][] observations, int[] clusterLabels, int numClusters, double[][] clusterCentroids)
        {
            double[][] distanceBetweenClusters = new double[numClusters][];

            //Loop through all the clusters
            //Cluster label starts from 0
            for (int clusterAIndex = 0; clusterAIndex < numClusters; clusterAIndex++)
            {
                double[] clusterACentroid = clusterCentroids[clusterAIndex];
                List<double> distanceFromClusterAToBList = new List<double>();

                //Loop through all the clusters
                //Cluster label starts from 0
                for (int clusterBIndex = 0; clusterBIndex < numClusters; clusterBIndex++)
                {
                    if (clusterAIndex == clusterBIndex)
                    {
                        distanceFromClusterAToBList.Add(0);
                    }
                    else
                    {
                        double[] clusterBCentroid = clusterCentroids[clusterBIndex];

                        double distanceFromClusterAToB = HelperMethod.GetEcludianDistance(clusterACentroid, clusterBCentroid);
                        distanceFromClusterAToBList.Add(distanceFromClusterAToB);
                    }
                }

                distanceBetweenClusters[clusterAIndex] = distanceFromClusterAToBList.ToArray();
            }

            return distanceBetweenClusters;
        }



        public static (int[], double[][]) RunKMeans(double[][] observations, int numClusters)
        {
            //Accord KMeans
            KMeans kmeans = new KMeans(k: numClusters);
            var clusters = kmeans.Learn(observations);
            int[] labels = clusters.Decide(observations);
            double[][] centroids = clusters.Centroids;

            return (labels, centroids);
        }
    }
}
