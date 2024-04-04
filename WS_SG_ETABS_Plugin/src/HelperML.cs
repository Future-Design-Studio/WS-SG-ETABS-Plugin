using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_SG_ETABS_Plugin.src.rc_beam_schedule;
using static WS_SG_ETABS_Plugin.src.rc_beam_schedule.ClassFunctionRCBSched;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperML
    {
        public static (List<int>, List<double>, List<double>) CalculateOptimalNumOfClusters(double[][] observations, int minNumOfClusters, int maxNumOfClusters)
        {
            List<double> sillouettheCoefficientList = new List<double>();
            List<double> WCCSList = new List<double>();
            List<int> numOfClustersList = new List<int>();

            //Loop through all the possible cluster
            for (int numClusters = minNumOfClusters; numClusters < maxNumOfClusters + 1; numClusters++)
            {
                if (numClusters < 2)
                {
                    sillouettheCoefficientList.Add(0);
                }
                else
                {
                    int[] labels;
                    double[][] centroids;
                    double[][][] covariances;

                    Accord.Math.Random.Generator.Seed = 0;
                    (labels, centroids, covariances) = RunKMeans(observations, numClusters);

                    numOfClustersList.Add(numClusters);
                    //sillouettheCoefficientList.Add(GetSillouettheCoefficientMedoids(observations, labels, numClusters, centroids));
                    sillouettheCoefficientList.Add(GetSillouettheCoefficient(observations, labels, numClusters, centroids));
                    WCCSList.Add(GetWCCS(observations, labels, numClusters, centroids));
                }
            }

            return (numOfClustersList, sillouettheCoefficientList, WCCSList);
        }

        public static double GetSillouettheCoefficientMedoids(double[][] observations, int[] clusterLabels, int numClusters, double[][] clusterCentroids)
        {
            double[][] distanceBetweenClusters = GetDistanceBetweenClusters(observations, clusterLabels, numClusters, clusterCentroids);
            double sumSillouettheScore = 0;

            //Loop through all the clusters
            //CLuster label starts from 0
            for (int clusterSelf = 0; clusterSelf < numClusters; clusterSelf++)
            {
                //Loop through all data to split between cluster and not cluster
                List<double[]> dataClusterSelf = new List<double[]>();
                for (int i = 0; i < observations.Count(); i++)
                {
                    if (clusterLabels[i] == clusterSelf)
                    {
                        dataClusterSelf.Add(observations[i]);
                    }
                }

                //Pick any 1 point inside the cluster and calculate a and b
                //Calculate distance between points within cluster to its own centroid (cohession)
                if (dataClusterSelf.Count() > 0)
                {
                    //Finding medoids (point closest to cluster center to use as reference)
                    double minDistanceToCentroid = 0;
                    int minDistanceDataIndex = 0;
                    bool firstLoop = true;
                    for (int i = 1; i < dataClusterSelf.Count(); i++)
                    {
                        if (firstLoop)
                        {
                            minDistanceToCentroid = HelperMethod.GetEcludianDistance(dataClusterSelf[i], clusterCentroids[clusterSelf]);
                            minDistanceDataIndex = i;
                            firstLoop = false;
                        }
                        else
                        {
                            if (HelperMethod.GetEcludianDistance(dataClusterSelf[i], clusterCentroids[clusterSelf]) < minDistanceToCentroid)
                            {
                                minDistanceToCentroid = HelperMethod.GetEcludianDistance(dataClusterSelf[i], clusterCentroids[clusterSelf]);
                                minDistanceDataIndex = i;
                            };

                        }

                    }

                    double[] choosenDataClusterSelf = dataClusterSelf[minDistanceDataIndex];
                    double a = 0;
                    if (dataClusterSelf.Count() > 1)
                    {
                        double sumDistance = 0;
                        for (int i = 0; i < dataClusterSelf.Count(); i++)
                        {
                            sumDistance += HelperMethod.GetEcludianDistance(choosenDataClusterSelf, dataClusterSelf[i]);
                        }

                        //Because we use 1 point as a reference to total count is minus 1
                        a = sumDistance / (dataClusterSelf.Count() - 1);

                        //Go through other cluster to find the min avg distance
                        List<double> bList = new List<double>();
                        for (int clusterNotSelf = 0; clusterNotSelf < numClusters; clusterNotSelf++)
                        {
                            //Make sure cluster index is different
                            if (clusterNotSelf != clusterSelf)
                            {
                                List<double[]> dataClusterNotSelf = new List<double[]>();
                                for (int i = 0; i < observations.Count(); i++)
                                {
                                    if (clusterLabels[i] == clusterNotSelf)
                                    {
                                        dataClusterNotSelf.Add(observations[i]);
                                    }
                                }

                                //If no data is found with this clusterNotSelf index
                                if (dataClusterNotSelf.Count() > 0)
                                {
                                    //Calculate distance to selected point inside cluster self
                                    sumDistance = 0;
                                    for (int i = 0; i < dataClusterNotSelf.Count(); i++)
                                    {
                                        sumDistance += HelperMethod.GetEcludianDistance(choosenDataClusterSelf, dataClusterNotSelf[i]);
                                    }

                                    bList.Add(sumDistance / dataClusterNotSelf.Count());
                                }
                                else
                                {
                                    //do nothing
                                }
                            }
                        }
                        double b = bList.Min();

                        sumSillouettheScore += (b - a) / Math.Max(a, b);

                    }
                    else
                    {
                        sumSillouettheScore += 0;
                    }


                }

            }

            return sumSillouettheScore / numClusters;
        }

        public static double GetSillouettheCoefficient(double[][] observations, int[] clusterLabels, int numClusters, double[][] clusterCentroids)
        {
            //double[][] distanceBetweenClusters = GetDistanceBetweenClusters(observations, clusterLabels, numClusters, clusterCentroids);
            double sumSillouettheScore = 0;
            double counter = 0;

            //Loop through all the clusters
            //CLuster label starts from 0
            for (int clusterSelf = 0; clusterSelf < numClusters; clusterSelf++)
            {
                //Loop through all data to split between cluster and not cluster
                List<double[]> dataClusterSelf = new List<double[]>();
                for (int i = 0; i < observations.Count(); i++)
                {
                    if (clusterLabels[i] == clusterSelf)
                    {
                        dataClusterSelf.Add(observations[i]);
                    }
                }

                //Pick any 1 point inside the cluster and calculate a and b
                //Calculate distance between points within cluster to its own centroid (cohession)
                if (dataClusterSelf.Count() > 0)
                {
                    double a = 0;
                    if (dataClusterSelf.Count() > 1)
                    {
                        for (int i = 0; i < dataClusterSelf.Count(); i++)
                        {
                            double sumDistanceA = 0;
                            for (int j = 0; j < dataClusterSelf.Count(); j++)
                            {
                                if (i != j)
                                {
                                    sumDistanceA += HelperMethod.GetEcludianDistance(dataClusterSelf[i], dataClusterSelf[j]);
                                }
                            }

                            //Because we use 1 point as a reference to total count is minus 1
                            a = sumDistanceA / (dataClusterSelf.Count() - 1);

                            //Go through other cluster to find the min avg distance
                            List<double> bList = new List<double>();
                            for (int clusterNotSelf = 0; clusterNotSelf < numClusters; clusterNotSelf++)
                            {
                                //Make sure cluster index is different
                                if (clusterNotSelf != clusterSelf)
                                {
                                    List<double[]> dataClusterNotSelf = new List<double[]>();
                                    for (int iteratorDataNotSelf = 0; iteratorDataNotSelf < observations.Count(); iteratorDataNotSelf++)
                                    {
                                        if (clusterLabels[iteratorDataNotSelf] == clusterNotSelf)
                                        {
                                            dataClusterNotSelf.Add(observations[iteratorDataNotSelf]);
                                        }
                                    }

                                    //If no data is found with this clusterNotSelf index
                                    if (dataClusterNotSelf.Count() > 0)
                                    {
                                        //Calculate distance to selected point outside cluster
                                        double sumDistanceB = 0;
                                        for (int iteratorDataNotSelf = 0; iteratorDataNotSelf < dataClusterNotSelf.Count(); iteratorDataNotSelf++)
                                        {
                                            sumDistanceB += HelperMethod.GetEcludianDistance(dataClusterSelf[i], dataClusterNotSelf[iteratorDataNotSelf]);
                                        }

                                        bList.Add(sumDistanceB / dataClusterNotSelf.Count());
                                    }
                                    else
                                    {
                                        //do nothing
                                    }
                                }
                            }
                            double b = bList.Min();

                            sumSillouettheScore += (b - a) / Math.Max(a, b);
                            counter++;
                        }

                    }
                    else
                    {
                        sumSillouettheScore += 0;
                        counter++;
                    }
                }

            }

            return sumSillouettheScore / counter;
        }

        public static double GetWCCS(double[][] observations, int[] clusterLabels, int numClusters, double[][] clusterCentroids)
        {
            double WCCS = 0;

            for (int clusterIndex = 0; clusterIndex < numClusters; clusterIndex++)
            {
                for (int i = 0; i < observations.Count(); i++)
                {
                    if (clusterLabels[i] == clusterIndex)
                    {
                        WCCS += Math.Pow(HelperMethod.GetEcludianDistance(observations[i], clusterCentroids[clusterIndex]), 2);
                    }

                }
            }
            return WCCS;
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



        public static (int[], double[][], double[][][]) RunKMeans(double[][] observations, int numClusters)
        {
            //Accord KMeans
            KMeans kmeans = new KMeans(k: numClusters);
            var clusters = kmeans.Learn(observations);
            int[] labels = clusters.Decide(observations);
            double[][] centroids = clusters.Centroids;
            double[][][] covariance = clusters.Covariances;

            return (labels, centroids, covariance);
        }

        public static double[][] StandardizeData(double[][] observations)
        {
            List<double[]> standardizedObservations = new List<double[]>();

            List<double> meanList = new List<double>();
            List<double> stdDevList = new List<double>();

            //Get how many dimensions
            int numOfDimension = observations[0].Count();

            for (int dimensionIndex = 0; dimensionIndex < numOfDimension; dimensionIndex++)
            {
                List<double> listOfValues = new List<double>();

                for (int observationIndex = 0; observationIndex < observations.Count(); observationIndex++)
                {
                    double originalValue = observations[observationIndex][dimensionIndex];
                    float logTransformedValue = (float)Math.Log(originalValue + 1);
                    listOfValues.Add(logTransformedValue);
                }

                //Caculate mean and std dev
                double mean = listOfValues.Average();
                double stdDev = HelperMethod.GetStandardDeviation(listOfValues);

                if (stdDev == 0)
                {
                    stdDev = 0.001;
                }

                meanList.Add(mean);
                stdDevList.Add(stdDev);
            }

            for (int observationIndex = 0; observationIndex < observations.Count(); observationIndex++)
            {
                List<double> listOfStandardizedValues = new List<double>();
                for (int dimensionIndex = 0; dimensionIndex < numOfDimension; dimensionIndex++)
                {
                    double originalValue = observations[observationIndex][dimensionIndex];
                    float logTransformedValue = (float)Math.Log(originalValue + 1);
                    float standardizedValue = (float)((logTransformedValue - meanList[dimensionIndex]) / stdDevList[dimensionIndex]);

                    listOfStandardizedValues.Add(standardizedValue);
                }

                standardizedObservations.Add(listOfStandardizedValues.ToArray());
            }

            return standardizedObservations.ToArray();
        }
    }
}
