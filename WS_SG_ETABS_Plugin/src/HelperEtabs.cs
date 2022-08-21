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
            MessageBox.Show("Error! DO NOT save your model and close immediately!" + ImportLog);
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
        public static (List<Dictionary<string, string>>, int, string[], int) GetTableEdit(cSapModel Sap, string tableKey)
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
            return (tableInList, TableVersion, FieldsKeysIncluded, NumberRecords);
            
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
        /// Access ETABS database for display and return it as list of dictionary
        /// Each dictionary in the list is 1 row
        /// Dictionary key is column
        /// </summary>
        /// <param name="Sap">Refer to specified SAP model</param>
        /// <param name="tableKey">Table key that we want to extract</param>
        /// <returns>
        /// Return tupple (tableInList, TableVersion, FieldKeysIncluded, NumberRecords)
        /// </returns>
        public static (bool, List<Dictionary<string, string>>) GetTableDisplay(cSapModel Sap, string tableKey)
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
                src.HelperEtabs.WarningTableDoesNotExist(tableKey);
                isError = true;
                return (isError, null);
            }

            List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

            tableInList = HelperMethod.EtabsTableToDictionary(TableData, FieldsKeysIncluded);
            return (isError, tableInList);
        }

        public static (bool, List<Dictionary<string, string>>) GetTableDisplaySpecificGroup(cSapModel Sap, string tableKey, List<string> groupList, bool showWarning)
        {
            List<string> compiledTableData = new List<string>();
            string[] columnLabel = new string[] { null }; //to save FieldsKeysIncluded, simulate data structure of FieldsKeysIncluded
            bool isError = false;

            foreach (string group in groupList)
            {
                string TableKey = tableKey;
                string[] FieldKeyList = null;
                string GroupName = group;
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                string[] TableData = null;

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
        public static List<Dictionary<string, string>> EditTable (List<Dictionary<string, string>> table, string refColumn, string refValue, string editColumn, string editValue)
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


        public static bool ApplyEtabsTable(cSapModel Sap, string TableKey, List<Dictionary<string,string>> table, 
            int TableVersion, string[]  FieldsKeysIncluded, int NumberRecords)
        {
            string[] tableArray = HelperMethod.DictionaryTableToEtabs(table);

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
        public static List<Point3D> ConvertPointDatabaseToPoint3D (List<Dictionary<string, string>> pointObjConnectivity)
        {
            List<Point3D> point3DList = new List<Point3D>();

            foreach (Dictionary<string, string> singlePoint in pointObjConnectivity)
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

    }

}

