using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperMethod
    {

        #region Database Method


        /// <summary>
        /// Convert ETABS table which is in array to dictionary data structure
        /// </summary>
        /// <param name="tableArray">Table from ETABS</param>
        /// <param name="columnKeys">Column label</param>
        /// <returns>List of dictionary with key = column label, each dictionary is a row</returns>
        public static List<Dictionary<string, string>> EtabsTableToDictionary(string[] tableArray, string[] columnKeys)
        {
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            
            int rowIndex = 0;

            while (rowIndex < tableArray.Length)
            {
                Dictionary<string, string> dataRow = new Dictionary<string, string>();

                foreach (string column in columnKeys)
                {
                    dataRow[column] = tableArray[rowIndex];
                    rowIndex++;
                }
                table.Add(dataRow);
            }
            return table;

        }

        /// <summary>
        /// Convert ETABS table which is in array to dictionary data structure
        /// </summary>
        /// <param name="tableArray">Table from ETABS</param>
        /// <param name="columnKeys">Column label</param>
        /// <returns>List of dictionary with key = column label, each dictionary is a row</returns>
        public static List<Dictionary<string, string>> TransposedEtabsTableToDictionary(string[] tableArray, string[] columnKeys)
        {
            //Get number of items
            int numItems = tableArray.Length / columnKeys.Length;

            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

            int itemIndex = 0;
           
            while (itemIndex < numItems)
            {
                int columnIndex = 0;

                Dictionary<string, string> dataRow = new Dictionary<string, string>();
                foreach (string column in columnKeys)
                {
                    int rowIndex = itemIndex + columnIndex * numItems;
                    dataRow[column] = tableArray[rowIndex];
                    columnIndex++;
                }
                table.Add(dataRow);
                itemIndex++;
            }



            return table;

        }

        /// <summary>
        /// Convert ETABS table which is in array to dictionary data structure
        /// </summary>
        /// <param name="tableArray">Table from ETABS</param>
        /// <param name="columnKeys">Column label</param>
        /// <returns>List of dictionary with key = column label, each dictionary is a row</returns>
        public static string[] DictionaryTableToEtabs(List<Dictionary<string, string>> table)
        {
            List<string> tableList = new List<string>();


            foreach (Dictionary<string, string> row in table) 
            {
                foreach (KeyValuePair<string, string> data in row)
                {
                    tableList.Add(data.Value);
                }
            }

            string[] tableArray = tableList.ToArray();

            return tableArray;

        }

        /// <summary>
        /// Filted ETABS table on specific column key with specific keyword
        /// </summary>
        /// <param name="tableInList"></param>
        /// <param name="columnKey">Column key to filter from</param>
        /// <param name="keywordInColumn">Keyword to find</param>
        /// <returns>Filtered table with the same data structure as input</returns>
        public static src.HelperEtabs.DatabaseTable FilterEtabsDatabase(src.HelperEtabs.DatabaseTable databaseTable, 
            string columnKey, string keywordInColumn)
        {
            List<Dictionary<string, string>> tableFiltered = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in databaseTable.TableInList)
            {
                if (row[columnKey] == keywordInColumn)
                {
                    tableFiltered.Add(row);
                }
            }

            string TableKey = databaseTable.TableKey;
            string GroupName = databaseTable.GroupName;
            int TableVersion = databaseTable.TableVersion;
            string[] FieldsKeysIncluded = databaseTable.FieldsKeysIncluded;
            int NumberRecords = databaseTable.NumberRecords;
            src.HelperEtabs.DatabaseTable filteredDatabaseTable = new src.HelperEtabs.DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableFiltered);

            return filteredDatabaseTable;          

        }

        #endregion

        #region Math Method

        /// <summary>
        /// Caclulate cross product of 2 vectors
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="Z1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <param name="Z2"></param>
        /// <returns></returns>
        public static (double X, double Y, double Z) CrossProduct(double X1, double Y1, double Z1, double X2, double Y2, double Z2)
        {
            double X = new double();
            double Y = new double();
            double Z = new double();

            X = (Y1 * Z2 - Y2 * Z1);
            Y = -(X1 * Z2 - X2 * Z1);
            Z = (X1 * Y2 - X2 * Y1);

            return (X, Y, Z);
        }

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static double GetStandardDeviation(IEnumerable<double> sequence)
        {
            double result = 0;

            if (sequence.Any())
            {
                double average = sequence.Average();
                double sum = sequence.Sum(d => Math.Pow(d - average, 2));
                result = Math.Sqrt((sum) / sequence.Count());
            }
            return result;
        }

        public static double GetEcludianDistance(double[] dataA, double[] dataB)
        {
            if (dataA.Length != dataB.Length)
            {
                MessageBox.Show("Ecludian distance requires both data to have the same dimension!");
                return 0;
            }
            else
            {
                double sumDimension = 0;
                for (int indexData = 0;  indexData < dataA.Count(); indexData++)
                {
                    double dimension = Math.Pow(dataA[indexData] - dataB[indexData], 2);
                    sumDimension += dimension;
                }

                return Math.Sqrt(sumDimension);
            }
        }


        #endregion

        #region Geometry Method

        public static bool IsAreaHorizontal (List<src.HelperEtabs.Point3D> pointList)
        {
            bool isHorizontal = true;
            double zCoord = 0;
            bool firstRun = true;

            foreach (src.HelperEtabs.Point3D point in pointList)
            {
                if (firstRun)
                {
                    zCoord = point.ZCoordinate;
                    firstRun = false;
                }
                else
                {
                    if (point.ZCoordinate != zCoord)
                    {
                        isHorizontal = false;
                        return isHorizontal;
                    }
                }
            }

            return isHorizontal;
        }

        #endregion

        #region Dictionary Method

        /// <summary>
        /// Finding min and max value in list of dictionary based on given key. Value must be number
        /// </summary>
        /// <param name="listOfDictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static (double, double) GetMinMaxDictionary (List<Dictionary<string, object>> listOfDictionary, string key)
        {
            double min = 0;
            double max = 0;
            bool firstLoop = true;
            //Looking for minimum maximum x and y
            foreach (Dictionary<string, object> dictionary in listOfDictionary)
            {
                if (firstLoop)
                {
                    min = Convert.ToDouble(dictionary[key]);
                    max = Convert.ToDouble(dictionary[key]);
                    firstLoop = false;
                }
                else
                {
                    if (Convert.ToDouble(dictionary[key]) < min)
                    {
                        min = Convert.ToDouble(dictionary[key]);
                    }
                    if(Convert.ToDouble(dictionary[key]) > max)
                    {
                        max = Convert.ToDouble(dictionary[key]);
                    }
                }
            }
            return (min, max);
        }

        #endregion

        #region Array method

        public static string[] ConvertDoubleToStringArray(double[] array)
        {
            // converting int myArr to String conarr 
            String[] newArray = Array.ConvertAll(array, new Converter<double, String>(DoubleToString));

            return newArray;
        }

        // Defining the method 
        public static String DoubleToString(double pf)
        {
            return pf.ToString();
        }

        #endregion

        #region Rebar method

        public static double GetRebarBendingRadius (int rebarDiameter)
        {
            if (rebarDiameter < 20)
            {
                return rebarDiameter * 2;
            }
            else
            {
                return rebarDiameter * 3.5;
            }
        }


        #endregion

    }





}
