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
        public static List<Dictionary<string, string>> FilterEtabsDatabase(List<Dictionary<string, string>> tableInList, 
            string columnKey, string keywordInColumn)
        {
            List<Dictionary<string, string>> tableFiltered = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in tableInList)
            {
                if (row[columnKey] == keywordInColumn)
                {
                    tableFiltered.Add(row);
                }
            }

            return tableFiltered;          

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



        #endregion

    }





}
