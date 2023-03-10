using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.test
{
    internal class Test
    {

        //Print all database table
        public static void PrintAllDatabaseTable(cSapModel Sap)
        {
            string[] tableKeys = src.HelperEtabs.GetAllTable(Sap);
            System.Diagnostics.Debug.WriteLine(string.Join(",", tableKeys));
        }

        //Print table for edit
        public static void PrintTableEdit(cSapModel Sap, string tableKey)
        {
            //Check if the table going to print exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add(tableKey);

            List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForEdit(Sap, tableKeys);

            if (tableKeysNotExist.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("table doesnt exist");
                return;
            }

            //Table exist then proceed to print

            src.HelperEtabs.DatabaseTable databaseTable = src.HelperEtabs.GetTableEdit(Sap, tableKey);

            List<Dictionary<string, string>> tableInList = databaseTable.TableInList;

            foreach (var table in tableInList)
            {
                foreach (var row in table)
                {
                    System.Diagnostics.Debug.WriteLine(row.Key + ": " + row.Value);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------");
            }

        }

        //Print table for display
        public static void PrintTableDisplay(cSapModel Sap, string tableKey)
        {
            bool isError = false;
            /*            //Check if the table going to print exist
                        List<string> tableKeys = new List<string>();
                        tableKeys.Add(tableKey);

                        List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForDisplay(Sap, tableKeys);

                        if (tableKeysNotExist.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Table " + tableKey + " doesnt exist");
                            return;
                        }*/

            //Table exist then proceed to print
            src.HelperEtabs.DatabaseTable databaseTable = null;
            (isError, databaseTable) = src.HelperEtabs.GetTableDisplay( Sap, tableKey, false);

            if (isError)
            {
                return;
            }

            foreach (var table in databaseTable.TableInList)
            {
                foreach (var row in table)
                {
                    System.Diagnostics.Debug.WriteLine(row.Key + ": " + row.Value);
                }
            }

            return;

        }

        public static void CrossProductTesting(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            (double x, double y, double z) = src.HelperMethod.CrossProduct(x1, y1, z1, x2, y2, z2);
            System.Diagnostics.Debug.WriteLine("x = " + x);
            System.Diagnostics.Debug.WriteLine("y = " + y);
            System.Diagnostics.Debug.WriteLine("z = " + z);
        }

    }
}
