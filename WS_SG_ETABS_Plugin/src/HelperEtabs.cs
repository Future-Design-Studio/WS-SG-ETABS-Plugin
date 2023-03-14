using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Contains all helper methods for ETABS API
    /// </summary>
    internal class HelperEtabs
    {
        private static int Ret;

        #region Class

        public class Point3D
        {
            public string UniqueName { get; set; }
            public double XCoordinate { get; set; }
            public double YCoordinate { get; set; }
            public double ZCoordinate { get; set; }

            public Point3D(string name, double x, double y, double z)
            {
                UniqueName = name;
                XCoordinate = x;
                YCoordinate = y;
                ZCoordinate = z;
            }
        }


        public class Shell
        {
            public string UniqueName { get; set; }
            public List<string> Points { get; set; }
            public Dictionary<string, double> NormalDirection { get; set; }

            public Shell(string name, List<string> pointsList)
            {
                UniqueName = name;
                Points = pointsList;
            }
        }

        public class DatabaseTable
        {
            public string TableKey { get; set; }
            public string GroupName { get; set; }
            public int TableVersion { get; set; }
            public string[] FieldsKeysIncluded { get; set; }
            public int NumberRecords { get; set; }
            public List<Dictionary<string, string>> TableInList {get; set; }

            public DatabaseTable()
            {

            }

            public DatabaseTable(string tableKey, string groupName, int tableVersion, string[] fieldsKeysIncluded, int numberRecords, List<Dictionary<string, string>> tableInList)
            {
                TableKey =  tableKey;
                GroupName = groupName;
                TableVersion = tableVersion;
                FieldsKeysIncluded = fieldsKeysIncluded;
                NumberRecords = numberRecords;
                TableInList = tableInList;
            }
        }


        #endregion

        #region ETABS Function Method

        /// <summary>
        /// Run Analysis
        /// </summary>
        /// <param name="Sap"></param>
        public static void RunAnalysis(cSapModel Sap)
        {
            Ret = Sap.Analyze.RunAnalysis();
        }

        /// <summary>
        /// Opening another etabs file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static (bool, cSapModel, cOAPI) OpenModel(string filePath)
        {
            bool isError = false;
            cSapModel Sap = null;
            
            //set the following flag to true to attach to an existing instance of the program 
            //otherwise a new instance of the program will be started 
            bool AttachToInstance;
            AttachToInstance = false;

            //set the following flag to true to manually specify the path to ETABS.exe
            //this allows for a connection to a version of ETABS other than the latest installation
            //otherwise the latest installed version of ETABS will be launched
            bool SpecifyPath;
            SpecifyPath = false;

            //if the above flag is set to true, specify the path to ETABS below
            string ProgramPath;
            ProgramPath = @"C:\Program Files\Computers and Structures\ETABS 19\ETABS.exe";

            /*
            //full path to the model 
            //set it to an already existing folder 
            string ModelDirectory = @"C:\CSi_ETABS_API_Example";
            try
            {
                System.IO.Directory.CreateDirectory(ModelDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create directory: " + ModelDirectory);
            }

            string ModelName = "ETABS_API_Example.edb";
            string ModelPath = ModelDirectory + System.IO.Path.DirectorySeparatorChar + ModelName;

            */

            //dimension the ETABS Object as cOAPI type
            ETABSv1.cOAPI myETABSObject = null;

            //Use ret to check if functions return successfully (ret = 0) or fail (ret = nonzero) 
            int ret = 0;

            //create API helper object
            ETABSv1.cHelper myHelper;
            try
            {
                myHelper = new ETABSv1.Helper();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot create an instance of the Helper object");
                isError = true;
                return (isError, Sap, myETABSObject);
            }

            if (AttachToInstance)
            {
                //attach to a running instance of ETABS 
                try
                {
                    //get the active ETABS object
                    myETABSObject = myHelper.GetObject("CSI.ETABS.API.ETABSObject");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No running instance of the program found or failed to attach.");
                    isError = true;
                    return (isError, Sap, myETABSObject);
                }
            }
            else
            {
                if (SpecifyPath)
                {
                    //'create an instance of the ETABS object from the specified path
                    try
                    {
                        //create ETABS object
                        myETABSObject = myHelper.CreateObject(ProgramPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot start a new instance of the program from " + ProgramPath);
                        isError = true;
                        return (isError, Sap, myETABSObject);
                    }
                }
                else
                {
                    //'create an instance of the ETABS object from the latest installed ETABS
                    try
                    {
                        //create ETABS object
                        myETABSObject = myHelper.CreateObjectProgID("CSI.ETABS.API.ETABSObject");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot start a new instance of the program.");
                        isError = true;
                        return (isError, Sap, myETABSObject);
                    }
                }
                //start ETABS application
                ret = myETABSObject.ApplicationStart();
            }

            //Get a reference to cSapModel to access all API classes and functions
            //Sap = default(ETABSv1.cSapModel);
            Sap = myETABSObject.SapModel;

            //initialize model
            try
            {
                ret = Sap.InitializeNewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot start a new ETABS. Possibly no license.");
                isError = true;
                return (isError, Sap, myETABSObject);
            }
            

            if (!AttachToInstance)
            {
                ret = Sap.File.OpenFile(filePath);

                if (ret == 1)
                {
                    MessageBox.Show("Error opening ETABS file!");
                    myETABSObject.ApplicationExit(false);
                    isError = true;
                }
            }

            return (isError, Sap, myETABSObject);
        }


        #endregion

        #region Area Method

        public static void GetAreaAssignments(cSapModel Sap)
        {
            List<Dictionary<string, string>> tableToAccess = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    {"tableOutputName", "sectProp" }, {"tableKey", "Area Assignments - Section Properties"}
                },
                new Dictionary<string, string>()
                {
                    {"tableOutputName", "stiffnessMod" }, {"tableKey", "Area Assignments - Stiffness Modifiers"}
                },


            };

            GetTableEditOnlyData(Sap, "Area Assignments - Section Properties");
        }

        public static void DeleteArea(cSapModel Sap, Shell shell)
        {
            Ret = Sap.AreaObj.Delete(shell.UniqueName, 0);
        }

        public static void RecreateExistingAreaByPoints(cSapModel Sap, Shell shell)
        {

            string[] points = shell.Points.ToArray();
            int numOfPoints = points.Count();
            string name = "";
            string userName = shell.UniqueName;

            Ret = Sap.AreaObj.AddByPoint(numOfPoints, ref points, ref name, "Default", userName);
        }

        /// <summary>
        /// Copy all area assignments inside ETABS file
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>multipleTableInList</returns>
        public static List<DatabaseTable> CopyAreaAssigments(cSapModel Sap)
        {

            //List of table names
            List<string> tableNameToCopy = new List<string>
            {
                "Area Assignments - Additional Mass",
                "Area Assignments - Area Springs",
                "Area Assignments - Auto Edge Constraints",
                "Area Assignments - Diaphragms",
                "Area Assignments - Edge Releases",
                "Area Assignments - Floor Auto Mesh Options",
                "Area Assignments - Floor Cracking",
                "Area Assignments - Insertion Point",
                "Area Assignments - Local Axes",
                "Area Assignments - Material Overwrites",
                "Area Assignments - Pier Labels",
                "Area Assignments - Section Properties",
                "Area Assignments - Slab Line Releases",
                "Area Assignments - Slab Rib Locations",
                "Area Assignments - Spandrel Labels",
                "Area Assignments - Stiffness Modifiers",
                "Area Assignments - Thickness Overwrites",
                "Area Assignments - Wall Auto Mesh Options",
                "Area Assignments - Wall Hinge - Auto Fiber PM3",
                "Area Assignments - Wall Hinge Reinforcement",
                "Area Assignments - Wall Hinges",
                "Area Assignments - Wall Rebar Ratio for Creep Analysis",
                "Area Load Assignments - Non-uniform",
                "Area Load Assignments - Temperature",
                "Area Load Assignments - Uniform",
                "Area Load Assignments - Uniform Load Sets",
                "Area Load Assignments - Wind Pressure Coefficients"
            };

            //List to save multiple tables
            List<DatabaseTable> multipleDatabaseTable = new List<DatabaseTable>();

            //Going to each table and save it if table exists
            foreach (string tableKey in tableNameToCopy)
            {
                string TableKey = "";
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                List<Dictionary<string, string>> tableInList = null;

                DatabaseTable singleTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);
                singleTable = GetTableEdit(Sap, tableKey);

                //Only save table if its not empty
                if (singleTable.NumberRecords != 0)
                {
                    multipleDatabaseTable.Add(singleTable);
                }
                
            }

            return multipleDatabaseTable;
        }


        public static void ApplyAreaAssigments(cSapModel Sap, List<DatabaseTable> multipleTableInList)
        {

            foreach(DatabaseTable singleTable in multipleTableInList)
            {
                string TableKey = singleTable.TableKey;
                int TableVersion = singleTable.TableVersion;
                string[] FieldsKeysIncluded = singleTable.FieldsKeysIncluded;
                int NumberRecords = singleTable.NumberRecords;
                List<Dictionary<string, string>> TableInList = singleTable.TableInList;

            }

        }


        #endregion

        #region Database Method

        /// <summary>
        /// Get all table keys for database operation
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static string[] GetAllTable(cSapModel Sap)
        {
            int NumberTables = 0;
            string[] TableKey = null;
            string[] TableName = null;
            int[] ImportType = null;
            bool[] IsEmpty = null;

            Ret = Sap.DatabaseTables.GetAllTables(ref NumberTables, ref TableKey, ref TableName, ref ImportType, ref IsEmpty);

            return TableKey;

        }

        public static void SetOutputUnitTo_kN_m_C(cSapModel Sap)
        {
            Ret = Sap.SetPresentUnits_2(eForce.kN, eLength.m, eTemperature.C);
        }

        /// <summary>
        /// Set output unit to specific unit
        /// </summary>
        /// <param name="Sap"></param>
        public static void SetOutputUnit(cSapModel Sap, eForce forceUnit, eLength lengthUnit, eTemperature temperatureUnit)
        {
            Ret = Sap.SetPresentUnits_2(forceUnit, lengthUnit, temperatureUnit);
        }

        /// <summary>
        /// Get database unit
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static (eForce forceUnit, eLength lengthUnit, eTemperature temperatureUnit) GetDatabaseUnit(cSapModel Sap)
        {
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();

            Ret = Sap.GetDatabaseUnits_2(ref forceUnit, ref lengthUnit, ref temperatureUnit);

            return (forceUnit, lengthUnit, temperatureUnit);
        }

        /// <summary>
        /// Get present unit
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static (eForce forceUnit, eLength lengthUnit, eTemperature temperatureUnit) GetPresentUnit(cSapModel Sap)
        {
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();

            Ret = Sap.GetPresentUnits_2(ref forceUnit, ref lengthUnit, ref temperatureUnit);

            return (forceUnit, lengthUnit, temperatureUnit);
        }

        /// <summary>
        /// Show message box if tableKey does not exist in ETABS
        /// </summary>
        /// <param name="tableKey"></param>
        public static void WarningTableDoesNotExist(string tableKey)
        {
            System.Diagnostics.Debug.WriteLine("Table '" + tableKey + "' does not exist!");
            MessageBox.Show("Table '" + tableKey + "' does not exist!");
        }

        public static void WarningApplyingTable(string ImportLog)
        {
            System.Diagnostics.Debug.WriteLine(ImportLog);
            MessageBox.Show("Plugin ERROR! DO NOT save your model and close immediately!" + ImportLog);
        }

        /// <summary>
        /// Access ETABS database for editing and return it as list of dictionary
        /// Each dictionary in the list is 1 row
        /// Dictionary key is column
        /// </summary>
        /// <param name="Sap">Refer to specified SAP model</param>
        /// <param name="tableKey">Table key that we want to extract</param>
        /// <returns>
        /// Return tupple (tableInList, TableVersion, FieldKeysIncluded, NumberRecords)
        /// </returns>
        public static DatabaseTable GetTableEdit(cSapModel Sap, string tableKey)
        {
            string TableKey = tableKey;
            string GroupName = "";
            int TableVersion = 0;
            string[] FieldsKeysIncluded = null;
            int NumberRecords = 0;
            string[] TableData = null;

            Ret = Sap.DatabaseTables.GetTableForEditingArray(TableKey, GroupName, ref TableVersion, ref FieldsKeysIncluded,
                    ref NumberRecords, ref TableData);

            List<Dictionary<string, string>> tableInList = HelperMethod.EtabsTableToDictionary(TableData, FieldsKeysIncluded);

            DatabaseTable databaseTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded,
                    NumberRecords, tableInList);

            return databaseTable;

        }
        
        /// <summary>
        /// Access multiple ETABS database for editing and return it as list of DatabaseTable
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="tableKeys"></param>
        /// <returns></returns>
        public static List<DatabaseTable> GetMultipleTableEdit(cSapModel Sap, List<string> tableKeys)
        {
            List<DatabaseTable> multipleDatabaseTable = new List<DatabaseTable>();

            //Going to each table and save it if table exists
            foreach (string tableKey in tableKeys)
            {
                string TableKey = "";
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                List<Dictionary<string, string>> tableInList = null;

                DatabaseTable singleTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);
                singleTable = GetTableEdit(Sap, tableKey);

                //Only save table if its not empty
                if (singleTable.NumberRecords != 0)
                {
                    multipleDatabaseTable.Add(singleTable);
                }
            }

            return multipleDatabaseTable;
        }

        /// <summary>
        /// Access ETABS database for editing but only return the data
        /// Each dictionary in the list is 1 row
        /// Dictionary key is column
        /// </summary>
        /// <param name="Sap">Refer to specified SAP model</param>
        /// <param name="tableKey">Table key that we want to extract</param>
        /// <returns>
        /// Return tupple (tableInList, TableVersion, FieldKeysIncluded, NumberRecords)
        /// </returns>
        public static List<Dictionary<string, string>> GetTableEditOnlyData(cSapModel Sap, string tableKey)
        {
            string TableKey = tableKey;
            string GroupName = "";
            int TableVersion = 0;
            string[] FieldsKeysIncluded = null;
            int NumberRecords = 0;
            string[] TableData = null;

            Ret = Sap.DatabaseTables.GetTableForEditingArray(TableKey, GroupName, ref TableVersion, ref FieldsKeysIncluded,
                    ref NumberRecords, ref TableData);

            List<Dictionary<string, string>> tableInList = HelperMethod.EtabsTableToDictionary(TableData, FieldsKeysIncluded);
            return tableInList;

        }

        /// <summary>
        /// Access ETABS database for display and return it as DatabaseTable
        /// </summary>
        /// <param name="Sap">Refer to specified SAP model</param>
        /// <param name="tableKey">Table key that we want to extract</param>
        /// <returns>
        /// Return tupple (isError, tableInList)
        /// </returns>
        public static (bool, DatabaseTable) GetTableDisplay(cSapModel Sap, string tableKey, bool showWarning)
        {
            string TableKey = tableKey;
            string[] FieldKeyList = null;
            string GroupName = "";
            int TableVersion = 0;
            string[] FieldsKeysIncluded = null;
            int NumberRecords = 0;
            string[] TableData = null;
            bool isError = false;

            Ret = Sap.DatabaseTables.GetTableForDisplayArray(TableKey, ref FieldKeyList, GroupName, ref TableVersion,
                ref FieldsKeysIncluded, ref NumberRecords, ref TableData);

            //If table doesnt exist, ETABS return first element of FieldKeysIncluded = null
            if (FieldsKeysIncluded[0] == null)
            {
                if (showWarning)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKey);
                }

                isError = true;
                return (isError, null);
            }

            List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

            tableInList = HelperMethod.EtabsTableToDictionary(TableData, FieldsKeysIncluded);

            DatabaseTable databaseTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded,
                    NumberRecords, tableInList);

            return (isError, databaseTable);
        }

        /// <summary>
        /// Access single ETABS database for display but onyl for specified groups and return it as list of DatabaseTable
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="tableKey"></param>
        /// <param name="groupList"></param>
        /// <param name="showWarning"></param>
        /// <returns></returns>
        public static (bool, DatabaseTable) GetTableDisplaySpecificGroup(cSapModel Sap, string tableKey, List<string> groupList, bool showWarning)
        {
            List<string> compiledTableData = new List<string>();
            string[] columnLabel = new string[] { null }; //to save FieldsKeysIncluded, simulate data structure of FieldsKeysIncluded
            bool isError = false;

            string TableKey = tableKey;
            string[] FieldKeyList = null;
            string GroupName = null;
            int TableVersion = 0;
            string[] FieldsKeysIncluded = null;
            int NumberRecords = 0;
            string[] TableData = null;

            foreach (string group in groupList)
            {
                TableKey = tableKey;
                FieldKeyList = null;
                GroupName = group;
                TableVersion = 0;
                FieldsKeysIncluded = null;
                NumberRecords = 0;
                TableData = null;

                Ret = Sap.DatabaseTables.GetTableForDisplayArray(TableKey, ref FieldKeyList, GroupName, ref TableVersion,
                    ref FieldsKeysIncluded, ref NumberRecords, ref TableData);

                //if table exist, then add it to compileTableData and update columnLabel
                if (FieldsKeysIncluded[0] != null)
                {
                    compiledTableData.AddRange(TableData);
                    columnLabel = FieldsKeysIncluded;
                }
            }

            //If table doesnt exist, ETABS return first element of FieldKeysIncluded = null
            if (columnLabel[0] == null)
            {
                if (showWarning)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKey);
                }

                isError = true;
                return (isError, null);
            }

            //convert compiled table data back to array to match original TableData type
            string[] compiledTableDataInList = compiledTableData.ToArray();

            List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

            tableInList = HelperMethod.EtabsTableToDictionary(compiledTableDataInList, columnLabel);

            DatabaseTable databaseTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded,
                    NumberRecords, tableInList);

            return (isError, databaseTable);
        }


        /// <summary>
        /// Access multiple ETABS database for display and return it as list of DatabaseTable
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="tableKeys"></param>
        /// <returns></returns>
        public static (bool, List<DatabaseTable>) GetMultipleTableDisplay(cSapModel Sap, List<string> tableKeys, bool showWarning)
        {
            List<DatabaseTable> multipleDatabaseTable = new List<DatabaseTable>();
            bool isError = false;

            //Going to each table and save it if table exists
            foreach (string tableKey in tableKeys)
            {
                isError = false;
                string TableKey = "";
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                List<Dictionary<string, string>> tableInList = null;

                DatabaseTable singleTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);
                (isError, singleTable) = GetTableDisplay(Sap, tableKey, showWarning);

                //Only save table if its not empty
                if (isError == false)
                {
                    multipleDatabaseTable.Add(singleTable);
                }
            }

            return (isError, multipleDatabaseTable);
        }

        /// <summary>
        /// Access ETABS database for display but only return the data
        /// Each dictionary in the list is 1 row
        /// Dictionary key is column
        /// </summary>
        /// <param name="Sap">Refer to specified SAP model</param>
        /// <param name="tableKey">Table key that we want to extract</param>
        /// <returns>
        /// Return tupple (tableInList, TableVersion, FieldKeysIncluded, NumberRecords)
        /// </returns>
        public static (bool, List<Dictionary<string, string>>) GetTableDisplayOnlyData(cSapModel Sap, string tableKey, bool showWarning)
        {
            string TableKey = tableKey;
            string[] FieldKeyList = null;
            string GroupName = "";
            int TableVersion = 0;
            string[] FieldsKeysIncluded = null;
            int NumberRecords = 0;
            string[] TableData = null;
            bool isError = false;

            Ret = Sap.DatabaseTables.GetTableForDisplayArray(TableKey, ref FieldKeyList, GroupName, ref TableVersion,
                ref FieldsKeysIncluded, ref NumberRecords, ref TableData);

            //If table doesnt exist, ETABS return first element of FieldKeysIncluded = null
            if (FieldsKeysIncluded[0] == null)
            {
                if (showWarning)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKey);
                }

                isError = true;
                return (isError, null);
            }

            List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

            return (isError, tableInList);
        }

        /// <summary>
        /// Check if tables exist for display
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="tableKey">Input is table list to check</param>
        /// <returns>Return list of tableKey that does not exist</returns>
        public static List<string> CheckIfTableExistForEdit(cSapModel Sap, List<string> tableKeys)
        {
            List<string> tableKeysUnavailable = new List<string>();

            foreach (string tableKey in tableKeys)
            {
                string TableKey = tableKey;
                string[] FieldKeyList = null;
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                string[] TableData = null;

                Ret = Sap.DatabaseTables.GetTableForDisplayArray(TableKey, ref FieldKeyList, GroupName, ref TableVersion,
                    ref FieldsKeysIncluded, ref NumberRecords, ref TableData);

                //If table doesnt exist, ETABS return first element of FieldKeysIncluded = null
                if (FieldsKeysIncluded[0] == null)
                {
                    tableKeysUnavailable.Add(tableKey);
                }
            }
            return tableKeysUnavailable;
        }

        /// <summary>
        /// Check if tables exist for display
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="tableKey">Input is table list to check</param>
        /// <returns>Return list of tableKey that does not exist</returns>
        public static List<string> CheckIfTableExistForDisplay(cSapModel Sap, List<string> tableKeys)
        {
            List<string> tableKeysUnavailable = new List<string>();

            foreach (string tableKey in tableKeys)
            {
                string TableKey = tableKey;
                string[] FieldKeyList = null;
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                string[] TableData = null;

                Ret = Sap.DatabaseTables.GetTableForDisplayArray(TableKey, ref FieldKeyList, GroupName, ref TableVersion,
                    ref FieldsKeysIncluded, ref NumberRecords, ref TableData);

                //If table doesnt exist, ETABS return first element of FieldKeysIncluded = null
                if (FieldsKeysIncluded[0] == null)
                {
                    tableKeysUnavailable.Add(tableKey);
                }
            }
            return tableKeysUnavailable;
        }

        /// <summary>
        /// Edit table (in dictionary format) return edited table (in dictionary format)
        /// </summary>
        /// <param name="table"></param>
        /// <param name="refColumn">Column to refer</param>
        /// <param name="refValue">If find this value in refColumn, then row is edited</param>
        /// <param name="editColumn">Column to edit</param>
        /// <param name="editValue">Assign this value to editColumn in edited row</param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> EditTable(List<Dictionary<string, string>> table, string refColumn, string refValue, string editColumn, string editValue)
        {
            List<Dictionary<string, string>> editedTable = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in table)
            {
                if (row[refColumn] == refValue)
                {
                    row[editColumn] = editValue;
                }
            }

            return table;
        }


        public static bool ApplyEtabsTable(cSapModel Sap, DatabaseTable databaseTable)
        {

            string TableKey = databaseTable.TableKey;
            int TableVersion = databaseTable.TableVersion;
            string [] FieldsKeysIncluded = databaseTable.FieldsKeysIncluded;
            int NumberRecords = databaseTable.NumberRecords;
            List<Dictionary<string, string>> tableInList = databaseTable.TableInList;

            string[] tableArray = HelperMethod.DictionaryTableToEtabs(tableInList);

            Ret = Sap.DatabaseTables.SetTableForEditingArray(TableKey, ref TableVersion, ref FieldsKeysIncluded, NumberRecords, ref tableArray);

            bool FillImportLog = true;
            int NumFatalErrors = 0;
            int NumErrorMsgs = 0;
            int NumWarnMsgs = 0;
            int NumInfoMsgs = 0;
            string ImportLog = "";

            Ret = Sap.DatabaseTables.ApplyEditedTables(FillImportLog, ref NumFatalErrors, ref NumErrorMsgs, ref NumWarnMsgs, ref NumInfoMsgs, ref ImportLog);

            bool isError = false;
            if (Ret != 0)
            {
                isError = true;
                src.HelperEtabs.WarningApplyingTable(ImportLog);
            }

            return isError;

        }

        public static bool ApplyMultipleEtabsTable(cSapModel Sap, List<DatabaseTable> multipleDatabaseTable)
        {

            foreach(DatabaseTable databaseTable in multipleDatabaseTable)

            {
                string TableKey = databaseTable.TableKey;
                int TableVersion = databaseTable.TableVersion;
                string[] FieldsKeysIncluded = databaseTable.FieldsKeysIncluded;
                int NumberRecords = databaseTable.NumberRecords;
                List<Dictionary<string, string>> tableInList = databaseTable.TableInList;

                string[] tableArray = HelperMethod.DictionaryTableToEtabs(tableInList);

                //string[] tableArray = null;

                Ret = Sap.DatabaseTables.SetTableForEditingArray(TableKey, ref TableVersion, ref FieldsKeysIncluded, NumberRecords, ref tableArray);

                /*bool FillImportLog = true;
                int NumFatalErrors = 0;
                int NumErrorMsgs = 0;
                int NumWarnMsgs = 0;
                int NumInfoMsgs = 0;
                string ImportLog = "";

                Ret = Sap.DatabaseTables.ApplyEditedTables(FillImportLog, ref NumFatalErrors, ref NumErrorMsgs, ref NumWarnMsgs, ref NumInfoMsgs, ref ImportLog);

                if (Ret != 0)
                {
                    isError = true;
                    src.HelperEtabs.WarningApplyingTable(ImportLog);
                }*/
          

            }


            bool FillImportLog = true;
            int NumFatalErrors = 0;
            int NumErrorMsgs = 0;
            int NumWarnMsgs = 0;
            int NumInfoMsgs = 0;
            string ImportLog = "";

            Ret = Sap.DatabaseTables.ApplyEditedTables(FillImportLog, ref NumFatalErrors, ref NumErrorMsgs, ref NumWarnMsgs, ref NumInfoMsgs, ref ImportLog);

            bool isError = false;
            if (Ret != 0 || NumFatalErrors > 0 || NumErrorMsgs > 0)
            {
                isError = true;
                src.HelperEtabs.WarningApplyingTable(ImportLog);
            }

            return isError;
        }

        public static void OnAllLoadDatabase(cSapModel Sap)
        {
            //Load Pattern
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadPatterns.GetNameList(ref NumberNames, ref MyName);
            Ret = Sap.DatabaseTables.SetLoadPatternsSelectedForDisplay(ref MyName);

            //Load Case
            Sap.LoadCases.GetNameList(ref NumberNames, ref MyName);
            Ret = Sap.DatabaseTables.SetLoadCasesSelectedForDisplay(ref MyName);

            //Load Combination
            Sap.RespCombo.GetNameList(ref NumberNames, ref MyName);
            Ret = Sap.DatabaseTables.SetLoadCombinationsSelectedForDisplay(ref MyName);
        }

        #endregion

        #region Group Method

        /// <summary>
        /// Return list of groups inside model
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static string[] GetGroupList(cSapModel Sap)
        {
            int NumberNames = 0;
            string[] MyName = null;

            Ret = Sap.GroupDef.GetNameList(ref NumberNames, ref MyName);

            return MyName;
        }

        /// <summary>
        /// Select group inside groupList. Only for frame and area type.
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupList"></param>
        public static void SelectGroupAllObject(cSapModel Sap, List<string> groupList)
        {

            //Deselect everything
            Ret = Sap.FrameObj.SetSelected("All", false, eItemType.Group);
            Ret = Sap.AreaObj.SetSelected("All", false, eItemType.Group);

            foreach (string group in groupList)
            {
                Ret = Sap.FrameObj.SetSelected(group, true, eItemType.Group);
                Ret = Sap.AreaObj.SetSelected(group, true, eItemType.Group);
            }

        }

        public static void CheckDuplicateObjectInGroup(cSapModel Sap, List<string> groupList)
        {
            List<string> frameListCompiled = new List<string>();
            List<string> areaListCompiled = new List<string>();

            //Get member of each group and compile it
            foreach (string group in groupList)
            {
                List<string> frameList = new List<string>();
                List<string> areaList = new List<string>();

                frameList = GetGroupFrameElement(Sap, group);
                areaList = GetGroupAreaElement(Sap, group);

                frameListCompiled.AddRange(frameList);
                areaListCompiled.AddRange(areaList);
            }

            //Find duplicates
            List<string> frameDuplicated = frameListCompiled.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            List<string> areaDuplicated = areaListCompiled.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (frameDuplicated.Count() > 0)
            {
                System.Diagnostics.Debug.WriteLine("Warning! Frame duplicate: '" + String.Join(", ", frameDuplicated));
                MessageBox.Show("Warning! Frame duplicate: " + String.Join(", ", frameDuplicated));
            }

            if (areaDuplicated.Count() > 0)
            {
                System.Diagnostics.Debug.WriteLine("Warning! Area duplicate: '" + String.Join(", ", areaDuplicated));
                MessageBox.Show("Warning! Area duplicate: " + String.Join(", ", areaDuplicated));
            }


        }

        /// <summary>
        /// Get list of unique name of frame element which is member of groupName
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupName">Group to inspect</param>
        /// <returns></returns>
        public static List<string> GetGroupFrameElement(cSapModel Sap, string groupName)
        {
            int NumberItems = 0;
            int[] ObjectType = null;
            string[] ObjectName = null;
            List<string> frameList = new List<string>();

            Ret = Sap.GroupDef.GetAssignments(groupName, ref NumberItems, ref ObjectType, ref ObjectName);

            int index = 0;
            foreach (int singleObjectType in ObjectType)
            {
                //ObjectType = 2 is frame
                if (singleObjectType == 2)
                {
                    frameList.Add(ObjectName[index]);
                }
                index++;
            }
            return frameList;
        }

        /// <summary>
        /// Get list of unique name of area element which is member of groupName
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupName">Group to inspect</param>
        /// <returns></returns>
        public static List<string> GetGroupAreaElement(cSapModel Sap, string groupName)
        {
            int NumberItems = 0;
            int[] ObjectType = null;
            string[] ObjectName = null;
            List<string> areaList = new List<string>();

            Ret = Sap.GroupDef.GetAssignments(groupName, ref NumberItems, ref ObjectType, ref ObjectName);

            int index = 0;
            foreach (int singleObjectType in ObjectType)
            {
                //ObjectType = 5 is area
                if (singleObjectType == 5)
                {
                    areaList.Add(ObjectName[index]);
                }
                index++;
            }
            return areaList;
        }


        #endregion

        #region Point Method

        /// <summary>
        /// Converting ETABS database "Point Object Connectivity to Point3D class
        /// </summary>
        /// <param name="pointObjConnectivity"></param>
        /// <returns></returns>
        public static List<Point3D> ConvertPointDatabaseToPoint3D(DatabaseTable pointObjConnectivity)
        {
            List<Point3D> point3DList = new List<Point3D>();

            foreach (Dictionary<string, string> singlePoint in pointObjConnectivity.TableInList)
            {
                string uniqueName = singlePoint["UniqueName"];
                double xCoordinate = Convert.ToDouble(singlePoint["X"]);
                double yCoordinate = Convert.ToDouble(singlePoint["Y"]);
                double zCoordinate = Convert.ToDouble(singlePoint["Z"]);
                Point3D point = new Point3D(uniqueName, xCoordinate, yCoordinate, zCoordinate);

                point3DList.Add(point);
            }

            return point3DList;
        }


        #endregion

        #region Material Method

        /// <summary>
        /// Copy all material properties inside ETABS file
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>multipleTableInList</returns>
        public static List<DatabaseTable> CopyMaterialProperties(cSapModel Sap)
        {

            //List of table names
            List<string> tableNameToCopy = new List<string>
            {
                "Material Properties - Acceptance Criteria",
                "Material Properties - Aluminum Data",
                "Material Properties - Basic Mechanical Properties",
                "Material Properties - Coldformed Data",
                "Material Properties - Concrete Data",
                "Material Properties - Damping Parameters",
                "Material Properties - General",
                "Material Properties - Hysteresis Parameters - BRB Hardening",
                "Material Properties - Hysteresis Parameters - Concrete",
                "Material Properties - Hysteresis Parameters - Degrading",
                "Material Properties - Hysteresis Parameters - Pivot",
                "Material Properties - Masonry Data",
                "Material Properties - Other Data",
                "Material Properties - Rebar Data",
                "Material Properties - Steel Data",
                "Material Properties - Tendon Data",
                "Material Properties - Time Dependence - ACI 209R-92",
                "Material Properties - Time Dependence - AS 3600-2009",
                "Material Properties - Time Dependence - AS 3600-2018",
                "Material Properties - Time Dependence - CEB-FIP 2010",
                "Material Properties - Time Dependence - CEB-FIP 90",
                "Material Properties - Time Dependence - Eurocode 2-2004",
                "Material Properties - Time Dependence - GL2000",
                "Material Properties - Time Dependence - NZS 3101-2006",
                "Material Properties - Time Dependence - User Creep",
                "Material Properties - Time Dependence - User Shrinkage",
                "Material Properties - Time Dependence - User Stiffness",
                "Material Properties - User Stress-Strain Curves",
            };

            //List to save multiple tables
            List<DatabaseTable> multipleDatabaseTable = new List<DatabaseTable>();

            //Going to each table and save it if table exists
            foreach (string tableKey in tableNameToCopy)
            {
                string TableKey = "";
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                List<Dictionary<string, string>> tableInList = null;

                DatabaseTable singleTable = new DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);
                singleTable = GetTableEdit(Sap, tableKey);

                //Only save table if its not empty
                if (singleTable.NumberRecords != 0)
                {
                    multipleDatabaseTable.Add(singleTable);
                }

            }

            return multipleDatabaseTable;
        }

        #endregion

        #region Response Spectrum Method

        public static void ConvertAllFunctionToUser(cSapModel Sap)
        {
            int NumberNames = 0;
            string[] MyName = null;

            Ret = Sap.Func.GetNameList(ref NumberNames, ref MyName);

            foreach (string name in MyName)
            {
                Ret = Sap.Func.ConvertToUser(name);
            }
        }


        #endregion

        #region Load Method

        /// <summary>
        /// Get list of all loads (patterns, cases and combo)
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static List<string> GetAllLoadList(cSapModel Sap)
        {
            List<string> outputCasesList = new List<string>();

            //Load Pattern
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadPatterns.GetNameList(ref NumberNames, ref MyName);

            outputCasesList.AddRange(MyName);

            //Load Case
            Sap.LoadCases.GetNameList(ref NumberNames, ref MyName);
            outputCasesList.AddRange(MyName);

            //Load Combination
            Sap.RespCombo.GetNameList(ref NumberNames, ref MyName);
            outputCasesList.AddRange(MyName);

            return outputCasesList;
        }

        /// <summary>
        /// Get list of all loads patterns
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static List<string> GetAllLoadPattern(cSapModel Sap)
        {
            List<string> outputPatternsList = new List<string>();

            //Load Pattern
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadPatterns.GetNameList(ref NumberNames, ref MyName);

            outputPatternsList.AddRange(MyName);

            return outputPatternsList;
        }

        /// <summary>
        /// Get list of load cases and combo
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static List<string> GetLoadCaseAndComboList(cSapModel Sap)
        {
            List<string> outputCasesList = new List<string>();

            //Load Case
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadCases.GetNameList(ref NumberNames, ref MyName);
            outputCasesList.AddRange(MyName);

            //Load Combination
            Sap.RespCombo.GetNameList(ref NumberNames, ref MyName);
            outputCasesList.AddRange(MyName);

            return outputCasesList;
        }

        #endregion

        #region shell method

        /// <summary>
        /// Parsing shell edge points from ETABS database
        /// </summary>
        /// <param name="shellObjConnectivity"></param>
        /// <returns></returns>
        public static List<Shell> GetShellsPoints(DatabaseTable shellObjConnectivity)
        {
            int index = 0;
            List<Shell> shellList = new List<Shell>();

            if (shellObjConnectivity != null && shellObjConnectivity.NumberRecords != 0)
            {
                while (index < shellObjConnectivity.TableInList.Count)
                {
                    List<string> pointList = new List<string>();

                    //Assume that row below is same shell so can enter while loop
                    bool rowBelowIsSameShell = true;

                    while (rowBelowIsSameShell)
                    {
                        if (shellObjConnectivity.TableInList[index]["UniquePt1"] != null)
                        {
                            //Only add if no duplicate points
                            if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt1"]))
                            {
                                pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt1"]);
                            }

                        }
                        if (shellObjConnectivity.TableInList[index]["UniquePt2"] != null)
                        {
                            //Only add if no duplicate points
                            if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt2"]))
                            {
                                pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt2"]);
                            }
                        }
                        if (shellObjConnectivity.TableInList[index]["UniquePt3"] != null)
                        {
                            //Only add if no duplicate points
                            if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt3"]))
                            {
                                pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt3"]);
                            }
                        }

                        //Check if UniquePt4 key exist, if exist take UniquePt4 else skip
                        bool keyExists = shellObjConnectivity.TableInList[index].ContainsKey("UniquePt4");
                        if (keyExists)
                        {
                            if (shellObjConnectivity.TableInList[index]["UniquePt4"] != null)
                            {
                                //Only add if no duplicate points
                                if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt4"]))
                                {
                                    pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt4"]);
                                }
                            }
                        }

                        //If this is not last row and below this row still the same shell, go to next row, loop again to get more points
                        if (index < shellObjConnectivity.TableInList.Count - 1 && shellObjConnectivity.TableInList[index]["UniqueName"] == shellObjConnectivity.TableInList[index + 1]["UniqueName"])
                        {
                            index++;
                        }
                        //else go out the loop and save points
                        else
                        {
                            rowBelowIsSameShell = false;
                        }
                    }
                    Shell shell = new Shell(shellObjConnectivity.TableInList[index]["UniqueName"], pointList);
                    shellList.Add(shell);

                    index++;
                }
            }

            return shellList;
        }

        #endregion

        #region Selected method

        public static int CountSelected(cSapModel Sap)
        {
            int NumberItems = 0;
            int[] ObjectType = null;
            string[] ObjectName = null;

            Ret = Sap.SelectObj.GetSelected(ref NumberItems, ref ObjectType, ref ObjectName);

            return NumberItems;
        }

        #endregion


        #region Add load pattern

        /// <summary>
        /// Add load pattern
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
        public static bool AddLoadPattern(cSapModel Sap, string name, double selfWeightMultipiler, string type, bool isExisting, bool addAnalysisCase)
        {

            string Name = name;
            double SelfWTMultiplier = selfWeightMultipiler;
            bool AddAnalysisCase = addAnalysisCase;

            if (type == "Dead" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Dead;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "SuperDead" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.SuperDead;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Live" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Live;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Wind" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Wind;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Notional" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Notional;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Temperature" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Temperature;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Seismic" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Quake;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Accidental" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Other;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "Uplift" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Other;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            else if (type == "EHF" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Notional;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            if (Ret ==0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
             
        #endregion

        #region Update load case
        public static bool UpdateLoadCase(cSapModel Sap, string name, int numberLoads, string[] Loadtype, string[] Loadname, double[] Scalefactor)
        {
            Ret = Sap.LoadCases.StaticLinear.SetLoads(name, numberLoads, ref Loadtype, ref Loadname, ref Scalefactor);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Add new load case
        /// <summary>
        /// Add new load case to etabs
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddNewLoadCase(cSapModel Sap, string name)
        {
            Ret = Sap.LoadCases.StaticLinear.SetCase(name);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Assign wind pressure coefficients
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupName"></param>
        /// <param name="loadPattern"></param>
        /// <param name="direction"></param>
        /// <param name="cP"></param>
        /// <returns></returns>
        public static bool AssignWindLoads(cSapModel Sap, string groupName, string loadPattern, int direction, double cP)
        {
            eItemType ItemType = eItemType.Group;

            Ret = Sap.AreaObj.SetLoadWindPressure(groupName, loadPattern, direction, cP, ItemType);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion


        #region Add new load combination Linear
        /// <summary>
        /// Adds a new linear additive load combination
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddNewLoadCombiLinear(cSapModel Sap, string nameLoadCombi)
        {

            Ret = Sap.RespCombo.Add(nameLoadCombi, 0);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Add new load combination envelope
        /// <summary>
        /// Adds a new envelope load combination
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddNewLoadCombiEnvelope(cSapModel Sap, string nameLoadCombi)
        {

            Ret = Sap.RespCombo.Add(nameLoadCombi, 1);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Update load combination
        /// <summary>
        /// Adds a new envelope load combination
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool UpdateLoadCombi(cSapModel Sap, string nameLoadCombi, int caseType, string nameLoadCaseOrCombi, double scaleFactor)
        {
            eCNameType ItemType = new eCNameType();
            if (caseType ==0)
            {
                ItemType = eCNameType.LoadCase;
            }
            else
            {
                ItemType = eCNameType.LoadCase;
            }

            Ret = Sap.RespCombo.SetCaseList(nameLoadCombi, ref ItemType, nameLoadCaseOrCombi, scaleFactor);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        #endregion

    }

}

