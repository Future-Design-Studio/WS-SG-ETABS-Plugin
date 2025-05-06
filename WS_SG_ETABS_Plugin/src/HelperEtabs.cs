using Accord;
using ETABSv1;
using Microsoft.Office.Interop.Excel;
using Rhino.Collections;
//using SAFEv1;
//using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace WS_SG_ETABS_Plugin.src
{
    /// <summary>
    /// Contains all helper methods for ETABS API
    /// </summary>
    public class HelperEtabs
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
            public List<string> PointUniqueNameList { get; set; }
            public Dictionary<string, double> NormalDirection { get; set; }
            public List<Point3D> Points { get; set; } 
            public string Group { get; set; }

            public Shell(string name, List<string> pointUniqueNameList)
            {
                UniqueName = name;
                PointUniqueNameList = pointUniqueNameList;
            }

            public Shell(string name, List<Point3D> points)
            {
                UniqueName = name;
                Points = points;
            }
        }

        public class Frame
        {
            public string UniqueName { get; set; }
            public List<string> PointUniqueNameList { get; set; }
            public List<Point3D> Points { get; set; }
            public string Group { get; set; }
            public string PointIUniqueName { get; set; }
            public string PointJUniqueName { get; set; }
            public Point3D PointI { get; set; }
            public Point3D PointJ { get; set; }

            public Frame(string name, List<string> pointUniqueNameList)
            {
                UniqueName = name;
                PointUniqueNameList = pointUniqueNameList;
            }

            public Frame(string name, List<Point3D> points)
            {
                UniqueName = name;
                Points = points;
            }

            public Frame() { }
        }


        public class Beam : Frame
        {
            public double Length { get; set; }
        }

        public class RcBeam : Beam
        {
            public double Height { get; set; }
            public double Width { get; set; }
            public double CoverTopEtabs { get; set; }
            public double CoverBottomEtabs { get; set; }
            public double OffsetI { get; set; }
            public double OffsetJ { get; set; }
            public double LeftLength { get; set; }
            public double MidLength { get; set; }
            public double RightLength { get; set; }
            public double LeftTopAs { get; set; }
            public string LeftTopRebar { get; set; }
            public double MidTopAs { get; set; }
            public string MidTopRebar { get; set; }
            public double RightTopAs { get; set; }
            public string RightTopRebar { get; set; }
            public double LeftBottomAs { get; set; }
            public string LeftBottomRebar { get; set; }
            public double MidBottomAs { get; set; }
            public string MidBottomRebar { get; set; }
            public double RightBottomAs { get; set; }
            public string RightBottomRebar { get; set; }
            public double LeftAv { get; set; }
            public string LeftShearLinks { get; set; }
            public double MidAv { get; set; }
            public string MidShearLinks { get; set; }
            public double RightAv { get; set; }
            public string RightShearLinks { get; set; }
            public double d { get; set; }
            public double dCalculated { get; set; }
            public string BeamType { get; set; }
            public double Fyk { get; set; }
            public double Fck { get; set; }
            public double AsMin { get; set; }
            public List<int> LeftTopRebarDiameterList { get; set; }
            public List<int> LeftTopNoRebarPerLayer { get; set; }
            public List<int> MidTopRebarDiameterList { get; set; }
            public List<int> MidTopNoRebarPerLayer { get; set; }
            public List<int> RightTopRebarDiameterList { get; set; }
            public List<int> RightTopNoRebarPerLayer { get; set; }
            public List<int> LeftBottomRebarDiameterList { get; set; }
            public List<int> LeftBottomNoRebarPerLayer { get; set; }
            public List<int> MidBottomRebarDiameterList { get; set; }
            public List<int> MidBottomNoRebarPerLayer { get; set; }
            public List<int> RightBottomRebarDiameterList { get; set; }
            public List<int> RightBottomNoRebarPerLayer { get; set; }
            public double LeftTopAsProv { get; set; }
            public double MidTopAsProv { get; set; }
            public double RightTopAsProv { get; set; }
            public double LeftBottomAsProv { get; set; }
            public double MidBottomAsProv { get; set; }
            public double RightBottomAsProv { get; set; }
            public double LeftAvProv { get; set; }
            public double MidAvProv { get; set; }
            public double RightAvProv { get; set; }
            public double LeftShearLinksDiameter { get; set; }
            public double LeftNoShearLinksLeg { get; set; }
            public double LeftShearLinksSpacing { get; set; }
            public double MidShearLinksDiameter { get; set; }
            public double MidNoShearLinksLeg { get; set; }
            public double MidShearLinksSpacing { get; set; }
            public double RightShearLinksDiameter { get; set; }
            public double RightNoShearLinksLeg { get; set; }
            public double RightShearLinksSpacing { get; set; }
            public double Cover { get; set; }
            public string AsEvaluationComment { get; set; }
            public string AvEvaluationComment { get; set; }
            public string EtabsComment { get; set; }
            public double RatioLeftTopAs { get; set; }
            public double RatioLeftBottomAs { get; set; }
            public double RatioMidTopAs { get; set; }
            public double RatioMidBottomAs { get; set; }
            public double RatioRightTopAs { get; set; }
            public double RatioRightBottomAs { get; set; }
            public double RatioLeftAv { get; set; }
            public double RatioMidAv { get; set; }
            public double RatioRightAv { get; set; }
            public string Cluster { get; set; }
            public double LeftStation { get; set; }
            public double RightStation { get; set; }
            public List<FrameForceEachCase> FrameForceEachCaseList { get; set;}

            public RcBeam(string name)
            {
                UniqueName = name;
                FrameForceEachCaseList = new List<FrameForceEachCase>();
            }
        }

        public class FrameForceEachCase
        {
            public FrameForceEachCase(string name)
            {
                LoadCaseName = name;
            }

            public string LoadCaseName { get; set; }
            public double LeftMomentNeg { get; set; }
            public double LeftMomentPos { get; set; }
            public double LeftShear { get; set; }
            public double MidMomentNeg { get; set; }
            public double MidMomentPos { get; set; }
            public double MidShear { get; set; }
            public double RightMomentNeg { get; set; }
            public double RightMomentPos { get; set; }
            public double RightShear { get; set; }
        }

        public class Column : Frame
        {
            public double Length { get; set; }
        }

        public class ColumnPunchingShear : Column
        {

            public ColumnPunchingShear(string name)
            {
                UniqueName = name;
            }
        }

        public class RcColumn : Column
        {
            public double Height { get; set; }
            public double Width { get; set; }
            public double Diameter { get; set; }
            public string Shape { get; set; }
            public double As { get; set; }
            public string Rebar { get; set; }
            public double AvMajor { get; set; }
            public double AvMinor { get; set; }
            public string ShearLinksMajor { get; set; }
            public string ShearLinksMinor { get; set; }
            public string ShearLinks { get; set; }
            public double Fyk { get; set; }
            public double Fck { get; set; }
            public double AsMin { get; set; }
            public double d { get; set; }
            public double dCalculated { get; set; }
            public double AsProv { get; set; }
            public double AsProvPercentage { get; set; }
            public double AvMajorProv { get; set; }
            public double AvMinorProv { get; set; }
            public double RatioAs { get; set; }
            public double RatioAvMajor { get; set; }
            public double RatioAvMinor { get; set; }
            public double Cover { get; set; }
            public double CoverEtabs { get; set; }
            public List<int> RebarDiameterList { get; set; }
            public List<int> NoRebarPerLayer { get; set; }
            public double ShearLinksDiameter { get; set; }
            public double NumShearLinksLegMajor { get; set; }
            public double NumShearLinksLegMinor { get; set; }
            public double ShearLinksSpacing { get; set; }
            public string AsEvaluationComment { get; set; }
            public string AvEvaluationComment { get; set; }
            public string EtabsComment { get; set; }
            public string Cluster { get; set; }
            public int WidthNoRebar { get; set; }
            public int HeightNoRebar { get; set; }

            public RcColumn(string name)
            {
                UniqueName = name;
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

            string[] points = shell.PointUniqueNameList.ToArray();
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

        public static void SetOutputUnitTo_kN_mm_C(cSapModel Sap)
        {
            Ret = Sap.SetPresentUnits_2(eForce.kN, eLength.mm, eTemperature.C);
        }

        public static void SetOutputUnitTo_N_mm_C(cSapModel Sap)
        {
            Ret = Sap.SetPresentUnits_2(eForce.N, eLength.mm, eTemperature.C);
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
        /// Return tupple (isError, databaseTable)
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

        public static void OnAllLoadPatternDatabase(cSapModel Sap)
        {
            //Load Pattern
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadPatterns.GetNameList(ref NumberNames, ref MyName);
            Ret = Sap.DatabaseTables.SetLoadPatternsSelectedForDisplay(ref MyName);

        }

        public static void OnAllLoadDatabaseWithoutCombinations(cSapModel Sap)
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
            MyName = new string[1] {""};
            Ret = Sap.DatabaseTables.SetLoadCombinationsSelectedForDisplay(ref MyName);
        }

        public static void OnLoadCaseAndComboDatabaseBasedOnList(cSapModel Sap, List<string> loadCaseAndComboList)
        {
            //Load Case
            int NumberNames = 0;
            string[] MyName = null;
            Sap.LoadCases.GetNameList(ref NumberNames, ref MyName);

            List<string> loadToTurnOnList = new List<string>();
            foreach(string loadCaseAndCombo in loadCaseAndComboList)
            {
                foreach (string name in MyName)
                {
                    if (loadCaseAndCombo == name)
                    {
                        loadToTurnOnList.Add(name);
                    }
                }
            }
            string[] loadToTurnOnArray = loadToTurnOnList.ToArray();

            Ret = Sap.DatabaseTables.SetLoadCasesSelectedForDisplay(ref loadToTurnOnArray);

            //Load Combination
            Sap.RespCombo.GetNameList(ref NumberNames, ref MyName);

            loadToTurnOnList = new List<string>();
            foreach (string loadCaseAndCombo in loadCaseAndComboList)
            {
                foreach (string name in MyName)
                {
                    if (loadCaseAndCombo == name)
                    {
                        loadToTurnOnList.Add(name);
                    }
                }
            }
            loadToTurnOnArray = loadToTurnOnList.ToArray();

            Ret = Sap.DatabaseTables.SetLoadCombinationsSelectedForDisplay(ref loadToTurnOnArray);
        }

        public static (List<string>, List<string>) GetAllLoadCaseAndComboDisplayedDatabase(cSapModel Sap)
        {
            /*
            //Load Pattern
            int NumberSelectedLoadPatterns = 0;
            string[] LoadPatternList = null;
            Ret = Sap.DatabaseTables.GetLoadPatternsSelectedForDisplay(ref NumberSelectedLoadPatterns, ref LoadPatternList);
            */

            //Load Case
            int NumberSelectedLoadCases = 0;
            string[] LoadCaseList = null;
            Ret = Sap.DatabaseTables.GetLoadCasesSelectedForDisplay(ref NumberSelectedLoadCases, ref LoadCaseList);

            //Load Combination
            int NumberSelectedLoadCombinations = 0;
            string[] LoadCombinationList = null;
            Ret = Sap.DatabaseTables.GetLoadCombinationsSelectedForDisplay(ref NumberSelectedLoadCombinations, ref LoadCombinationList);

            return (LoadCaseList.ToList(), LoadCombinationList.ToList());
        }

        public static void OffAllLoadCaseAndComboDisplayedDatabase(cSapModel Sap)
        {
            //Load Case
            string[] MyName = new string[1] { "" };
            Ret = Sap.DatabaseTables.SetLoadCasesSelectedForDisplay(ref MyName);

            //Load Combination
            MyName = new string[1] { "" };
            Ret = Sap.DatabaseTables.SetLoadCombinationsSelectedForDisplay(ref MyName);
        }


        public static List<src.HelperEtabs.Point3D> GetJointListFromGroup(src.HelperEtabs.DatabaseTable pointObjectConnectivityTable, src.HelperEtabs.DatabaseTable groupAssignmentsTable, string choosenGroup)
        {
            bool isError = false;
            src.HelperEtabs.DatabaseTable filteredJointTable = new HelperEtabs.DatabaseTable();
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();

            //Get table from database
            (isError, filteredJointTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);
            List<Dictionary<string, string>> table = filteredJointTable.TableInList;

            foreach (Dictionary<string, string> row in table)
            {
                src.HelperEtabs.Point3D joint = new HelperEtabs.Point3D(row["UniqueName"], Convert.ToDouble(row["X"]), Convert.ToDouble(row["Y"]), Convert.ToDouble(row["Z"]));
                jointList.Add(joint);
            }

            return jointList;
        }

        public static (bool, DatabaseTable) FilterDatabaseTable(DatabaseTable databaseTableToFilter, string columnKeyToFilter, string valueToFind)
        {
            bool isError = false;
            DatabaseTable databaseTableFiltered = new DatabaseTable();

            //Check if table to filter contains column key
            //First row only
            List<Dictionary<string, string>> tableInListToFilter = databaseTableToFilter.TableInList;

            //Check if input table is not empty
            if (tableInListToFilter.Count == 0)
            {
                MessageBox.Show($"Table {databaseTableToFilter.TableKey} is empty!");
                DatabaseTable emptyDatabaseTable = new DatabaseTable();
                isError = true;
                return (isError, emptyDatabaseTable);
            }

            if (!tableInListToFilter[0].ContainsKey(columnKeyToFilter))
            {
                MessageBox.Show($"Table {databaseTableToFilter.TableKey} does not contain {columnKeyToFilter}!");
                DatabaseTable emptyDatabaseTable = new DatabaseTable();
                isError = true;
                return (isError, emptyDatabaseTable);
            }

            List<Dictionary<string, string>> tableInListFiltered = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in tableInListToFilter)
            {
                if (row[columnKeyToFilter] == valueToFind)
                {
                    tableInListFiltered.Add(row);
                }
            }

            //Save table in list to DatabaseTable
            databaseTableFiltered.TableInList = tableInListFiltered;

            return (isError, databaseTableFiltered);
        }

        public static (bool, double) FindMinValueInDatabaseTable(DatabaseTable databaseTableToFilter, string columnKey)
        {
            bool isError = false;
            DatabaseTable databaseTableFiltered = new DatabaseTable();

            //Check if table to filter contains column key
            //First row only
            List<Dictionary<string, string>> tableInListToFilter = databaseTableToFilter.TableInList;
            if (!tableInListToFilter[0].ContainsKey(columnKey))
            {
                MessageBox.Show($"Table {databaseTableToFilter.TableKey} does not contain {columnKey}!");
                DatabaseTable emptyDatabaseTable = new DatabaseTable();
                isError = true;
                return (isError, 0);
            }

            bool firstLoop = true;
            double minValue = 0;
            List<Dictionary<string, string>> tableInListFiltered = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in tableInListToFilter)
            {
                if (firstLoop)
                {
                    minValue = Convert.ToDouble(row[columnKey]);
                    firstLoop = false;
                }
                else
                {
                    if (minValue > Convert.ToDouble(row[columnKey]))
                    {
                        minValue = Convert.ToDouble(row[columnKey]);
                    }
                }
            }
            //Save table in list to DatabaseTable
            databaseTableFiltered.TableInList = tableInListFiltered;

            return (isError, minValue);
        }

        public static (bool, double) FindMaxValueInDatabaseTable(DatabaseTable databaseTableToFilter, string columnKey)
        {
            bool isError = false;
            DatabaseTable databaseTableFiltered = new DatabaseTable();

            //Check if table to filter contains column key
            //First row only
            List<Dictionary<string, string>> tableInListToFilter = databaseTableToFilter.TableInList;
            if (!tableInListToFilter[0].ContainsKey(columnKey))
            {
                MessageBox.Show($"Table {databaseTableToFilter.TableKey} does not contain {columnKey}!");
                DatabaseTable emptyDatabaseTable = new DatabaseTable();
                isError = true;
                return (isError, 0);
            }

            bool firstLoop = true;
            double maxValue = 0;
            List<Dictionary<string, string>> tableInListFiltered = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in tableInListToFilter)
            {
                if (firstLoop)
                {
                    maxValue = Convert.ToDouble(row[columnKey]);
                    firstLoop = false;
                }
                else
                {
                    if (maxValue < Convert.ToDouble(row[columnKey]))
                    {
                        maxValue = Convert.ToDouble(row[columnKey]);
                    }
                }
            }
            //Save table in list to DatabaseTable
            databaseTableFiltered.TableInList = tableInListFiltered;

            return (isError, maxValue);
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
        /// <summary>
        /// Create new group by defining group name
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool CreateNewGroup(cSapModel Sap, string groupName)
        {
            
            Ret = Sap.GroupDef.SetGroup_1(groupName, -1, false, false, false, false, false, false, false, false, false, false, false, false, false);

            if (Ret != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Takes in list of points and group name and assigns the list of points inside the group
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="listofPoints"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool AddPointsInsideGroup(cSapModel Sap, List<string> listofPoints, string groupName)
        {
            foreach (string points in listofPoints)
            {
                Ret = Sap.PointObj.SetGroupAssign(points, groupName);
                if (Ret != 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Takes in list of areas and group name and assigns the list of points inside the group
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="listofPoints"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool AddAreaInsideGroup(cSapModel Sap, List<string> listofAreas, string groupName)
        {
            foreach (string areas in listofAreas)
            {
                Ret = Sap.AreaObj.SetGroupAssign(areas, groupName);
                if (Ret != 0)
                {
                    return true;
                }
            }
            return false;
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
        /// Get list of unique name of joint element which is member of groupName
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="groupName">Group to inspect</param>
        /// <returns></returns>
        public static List<string> GetGroupJointElement(cSapModel Sap, string groupName)
        {
            int NumberItems = 0;
            int[] ObjectType = null;
            string[] ObjectName = null;
            List<string> jointList = new List<string>();

            Ret = Sap.GroupDef.GetAssignments(groupName, ref NumberItems, ref ObjectType, ref ObjectName);

            int index = 0;
            foreach (int singleObjectType in ObjectType)
            {
                //ObjectType = 1 is point
                if (singleObjectType == 1)
                {
                    jointList.Add(ObjectName[index]);
                }
                index++;
            }
            return jointList;
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

        /// <summary>
        /// Call database table based on given table key
        /// </summary>
        /// <param name="databaseTableList"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        public static (bool, DatabaseTable) CallDatabaseTableByTableKey(List<DatabaseTable> databaseTableList, string tableKey)
        {
            bool isError = false;

            foreach(DatabaseTable databaseTable in databaseTableList)
            {
                if (databaseTable.TableKey == tableKey)
                {
                    return (isError, databaseTable);
                }
            }

            DatabaseTable emptyDatabaseTable = new DatabaseTable();
            isError = true;

            MessageBox.Show($"Table {tableKey} not found!");

            return (isError, emptyDatabaseTable);
        }

        /// <summary>
        /// To filter database table based on group
        /// </summary>
        /// <param name="databaseTableToFilter"></param>
        /// <param name="groupDatabaseTable"></param>
        /// <param name="groupName"></param>
        /// <returns>
        /// Return (isError, databaseTableFiltered)
        /// </returns>
        public static (bool, DatabaseTable) FilterDatabaseTableUniqueNameByGroup(DatabaseTable databaseTableToFilter, DatabaseTable groupDatabaseTable, string groupName)
        {
            bool isError = false;
            List<string> uniqueNameInsideGroup = new List<string>();
            DatabaseTable databaseTableFiltered = new DatabaseTable();

            List<Dictionary<string, string>> tableInListGroup = groupDatabaseTable.TableInList;

            //Go through group database to get list of unique name which is part of the group
            foreach (Dictionary<string, string> row in tableInListGroup)
            {
                if (row["GroupName"] == groupName)
                {
                    uniqueNameInsideGroup.Add(row["UniqueName"]);
                }
            }

            List<Dictionary<string, string>> tableInListToFilter = databaseTableToFilter.TableInList;

            //Check if table to filter contains key UniqueName
            //First row only
            if (!tableInListToFilter[0].ContainsKey("UniqueName"))
            {
                MessageBox.Show($"Table {databaseTableToFilter.TableKey} does not contain UniqueName!");
                DatabaseTable emptyDatabaseTable = new DatabaseTable();
                isError = true;
                return (isError, emptyDatabaseTable);
            }

            List<Dictionary<string, string>> tableInListFiltered = new List<Dictionary<string, string>>();
            foreach (string eachUniqueName in uniqueNameInsideGroup)
            {
                foreach (Dictionary<string, string> row in tableInListToFilter)
                {
                    if (row["UniqueName"] == eachUniqueName)
                    {
                        tableInListFiltered.Add(row);
                    }
                }
            }

            //Save table in list to DatabaseTable
            databaseTableFiltered.TableInList = tableInListFiltered;

            //Return all the properties
            databaseTableFiltered.FieldsKeysIncluded = databaseTableToFilter.FieldsKeysIncluded;
            databaseTableFiltered.GroupName = databaseTableToFilter.GroupName;
            databaseTableFiltered.NumberRecords = databaseTableToFilter.NumberRecords;
            databaseTableFiltered.TableKey = databaseTableToFilter.TableKey;
            databaseTableFiltered.TableVersion = databaseTableToFilter.TableVersion;

            return (isError, databaseTableFiltered);
        }

        public static string GetElementGroupName(DatabaseTable groupDatabaseTable, string elementName, string elementType)
        {
            string groupName = null;

            foreach(Dictionary<string, string> row in groupDatabaseTable.TableInList)
            {
                if (row["UniqueName"] == elementName)
                {
                    if (row["ObjectType"] == elementType)
                    {
                        groupName = row["GroupName"];
                        return groupName;
                    }
                }
            }

            return groupName;
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

        public static List<Point3D> FilterPointBasedOnZCoord(List<Point3D> jointList, List<double> uniqueZCoordList)
        {
            List<Point3D> point3DList = new List<Point3D>();

            foreach (Point3D joint in jointList)
            {
                if (uniqueZCoordList.Contains(joint.ZCoordinate))
                {
                    point3DList.Add(joint);
                }

            }

            return point3DList;
        }

        public static (bool, Point3D) GetSpecificPoint3D (string uniqueName, List<Point3D> allPointList)
        {
            bool isError = false;

            foreach(Point3D point in allPointList)
            {
                if (point.UniqueName == uniqueName)
                {
                    return (isError, point);
                }
            }

            isError = true;
            return (isError, null);
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

        /// <summary>
        /// Get rebar fyk
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="rebarMatPropName"></param>
        /// <returns></returns>
        public static double GetRebarFyk(cSapModel Sap, string rebarMatPropName)
        {
            double Fy = 0;
            double Fu = 0;
            double EFy = 0;
            double EFu = 0;
            int SSType = 0;
            int SSHysType = 0;
            double StrainAtHardening = 0;
            double StrainUltimate = 0;
            double FinalSlope = 0;
            bool UseCaltransSSDefaults = false;

            Ret = Sap.PropMaterial.GetORebar_1(rebarMatPropName, ref Fy, ref Fu, ref EFy, ref EFu, ref SSType, ref SSHysType, ref StrainAtHardening, ref StrainUltimate, ref FinalSlope, ref UseCaltransSSDefaults);

            if (Ret != 0)
            {
                MessageBox.Show($"Unable to find rebar {rebarMatPropName}");
                return 0;
            }
            else
            {
                return Fy;
            }
                
        }

        /// <summary>
        /// Get concrete fck
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="rebarMatPropName"></param>
        /// <returns></returns>
        public static double GetConcreteFck(cSapModel Sap, string concreteMatPropName)
        {
            double Fc = 0;
            bool IsLightweight = false;
            double FcsFactor = 0;
            int SSType = 0;
            int SSHysType = 0;
            double StrainAtFc = 0;
            double StrainUltimate = 0;
            double FinalSlope = 0;
            double FrictionAngle = 0;
            double DilatationalAngle = 0;

            Ret = Sap.PropMaterial.GetOConcrete_1(concreteMatPropName, ref Fc, ref IsLightweight, ref FcsFactor, ref SSType, ref SSHysType, ref StrainAtFc, ref StrainUltimate, ref FinalSlope, ref FrictionAngle, ref DilatationalAngle);

            if (Ret != 0)
            {
                MessageBox.Show($"Unable to find concrete {concreteMatPropName}");
                return 0;
            }
            else
            {
                return Fc;
            }

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
        /// Delete specified list of patterns loads assigned from joints inside a group
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="patternnameList"></param>
        /// <param name="groupNameJoint"></param>
        /// <returns></returns>
        public static void DeleteExistingLoadsFromPoints(cSapModel Sap, List<string> patternnameList, string groupNameJoint)
        {
            eItemType ItemType = eItemType.Group;
            foreach (string patternName in patternnameList)
            {
                Ret = Sap.PointObj.DeleteLoadForce(groupNameJoint, patternName, ItemType);
                if (Ret == 0)
                {
                    MessageBox.Show(patternName + " loads were assigned to joints inside group of joints :" + groupNameJoint + " and these have been removed to prevent double counting old loads.");
                }
            }
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

            if (MyName != null)
            {
                outputCasesList.AddRange(MyName);
            }
           
            //Load Combination
            NumberNames = 0;
            MyName = null;
            Sap.RespCombo.GetNameList(ref NumberNames, ref MyName);

            if (MyName != null)
            {
                outputCasesList.AddRange(MyName);
            }

            return outputCasesList;
        }



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

            else if (type == "Other" && !isExisting)
            {
                eLoadPatternType MyType = eLoadPatternType.Other;
                Ret = Sap.LoadPatterns.Add(Name, MyType, SelfWTMultiplier, AddAnalysisCase);
            }

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

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

        /// <summary>
        /// Add new load case to etabs
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddNewLinearLoadCase(cSapModel Sap, string name)
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

        /// <summary>
        /// Add new load case to etabs
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddNewRSALoadCase(cSapModel Sap, string name)
        {
            Ret = Sap.LoadCases.ResponseSpectrum.SetCase(name);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

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

        /// <summary>
        /// Adds a new envelope load combination
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool UpdateLoadCombi(cSapModel Sap, string nameLoadCombi, int caseType, string nameLoadCaseOrCombi, double scaleFactor)
        {
            eCNameType ItemType = new eCNameType();
            if (caseType == 0)
            {
                ItemType = eCNameType.LoadCase;
            }
            else
            {
                ItemType = eCNameType.LoadCombo;
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

        #region Shell method

        /// <summary>
        /// Parsing shell edge points from ETABS database. Will return points unique name
        /// </summary>
        /// <param name="shellObjConnectivity"></param>
        /// <returns></returns>
        public static List<Shell> GetShellsPointsUniqueName(DatabaseTable shellObjConnectivity)
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

        /// <summary>
        /// Parsing shell edge points from ETABS database. Will return Point3D
        /// </summary>
        /// <param name="shellObjConnectivity"></param>
        /// <param name="pointObjConnectivity"></param>
        /// <returns></returns>
        public static (bool, List<Shell>) GetShellsPoints(DatabaseTable shellObjConnectivity, DatabaseTable pointObjConnectivity)
        {
            int index = 0;
            List<Shell> shellList = new List<Shell>();
            bool isError = false;

            if (shellObjConnectivity != null && shellObjConnectivity.NumberRecords != 0)
            {
                //Convert point connectivity database to Point3D
                List<Point3D> allPointList = ConvertPointDatabaseToPoint3D(pointObjConnectivity);

                while (index < shellObjConnectivity.TableInList.Count)
                {
                    List<string> pointList = new List<string>();
                    List<Point3D> point3DList = new List<Point3D>();
                    
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

                                Point3D point = null;
                                (isError, point) = GetSpecificPoint3D(shellObjConnectivity.TableInList[index]["UniquePt1"], allPointList);
                                if (!isError) { point3DList.Add(point); } else { return (isError, null); }
                            }

                        }
                        if (shellObjConnectivity.TableInList[index]["UniquePt2"] != null)
                        {
                            //Only add if no duplicate points
                            if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt2"]))
                            {
                                pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt2"]);

                                Point3D point = null;
                                (isError, point) = GetSpecificPoint3D(shellObjConnectivity.TableInList[index]["UniquePt2"], allPointList);
                                if (!isError) { point3DList.Add(point); } else { return (isError, null); }
                            }
                        }
                        if (shellObjConnectivity.TableInList[index]["UniquePt3"] != null)
                        {
                            //Only add if no duplicate points
                            if (!pointList.Contains(shellObjConnectivity.TableInList[index]["UniquePt3"]))
                            {
                                pointList.Add(shellObjConnectivity.TableInList[index]["UniquePt3"]);

                                Point3D point = null;
                                (isError, point) = GetSpecificPoint3D(shellObjConnectivity.TableInList[index]["UniquePt3"], allPointList);
                                if (!isError) { point3DList.Add(point); } else { return (isError, null); }
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

                                    Point3D point = null;
                                    (isError, point) = GetSpecificPoint3D(shellObjConnectivity.TableInList[index]["UniquePt4"], allPointList);
                                    if (!isError) { point3DList.Add(point); } else { return (isError, null); }
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
                    Shell shell = new Shell(shellObjConnectivity.TableInList[index]["UniqueName"], point3DList);
                    shellList.Add(shell);

                    index++;
                }
            }

            return (isError, shellList);
        }

        /// <summary>
        /// Get wall design result
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns></returns>
            public static (List<Dictionary<string, string>>, List<Dictionary<string, string>>) GetWallDesignResult(cSapModel Sap)
        {
            List<Dictionary<string, string>>  tableInListPierDesignResult = GetPierDesignResult(Sap);
            List<Dictionary<string, string>> tableInListSpandrelDesignResult = GetSpandrelDesignResult(Sap);

            //If both null means no wall design
            if (tableInListPierDesignResult == null && tableInListSpandrelDesignResult == null)
            {
                MessageBox.Show("No wall design is found! Please run wall design first!");
                return (null, null);
            }

            return (tableInListPierDesignResult, tableInListSpandrelDesignResult);

        }

        public static List<Dictionary<string, string>> GetPierDesignResult(cSapModel Sap)
        {
            //Get pier result
            string[] Story = null;
            string[] PierLabel = null;
            string[] Station = null;
            string[] DesignType = null;
            string[] PierSecType = null;
            string[] EdgeBar = null;
            string[] EndBar = null;
            double[] BarSpacing = null;
            double[] ReinfPercent = null;
            double[] CurrPercent = null;
            double[] DCRatio = null;
            string[] PierLeg = null;
            double[] LegX1 = null;
            double[] LegY1 = null;
            double[] LegX2 = null;
            double[] LegY2 = null;
            double[] EdgeLeft = null;
            double[] EdgeRight = null;
            double[] AsLeft = null;
            double[] AsRight = null;
            double[] ShearAv = null;
            double[] StressCompLeft = null;
            double[] StressCompRight = null;
            double[] StressLimitLeft = null;
            double[] StressLimitRight = null;
            double[] CDepthLeft = null;
            double[] CLimitLeft = null;
            double[] CDepthRight = null;
            double[] CLimitRight = null;
            double[] InelasticRotDemand = null;
            double[] InelasticRotCapacity = null;
            double[] NormCompStress = null;
            double[] NormCompStressLimit = null;
            double[] CDepth = null;
            double[] BZoneL = null;
            double[] BZoneR = null;
            double[] BZoneLength = null;
            string[] WarnMsg = null;
            string[] ErrMsg = null;

            Ret = Sap.DesignShearWall.GetPierSummaryResults(ref Story, ref PierLabel, ref Station, ref DesignType, ref PierSecType, ref EdgeBar, ref EndBar, ref BarSpacing,
                ref ReinfPercent, ref CurrPercent, ref DCRatio, ref PierLeg, ref LegX1, ref LegY1, ref LegX2, ref LegY2, ref EdgeLeft, ref EdgeRight, ref AsLeft, ref AsRight,
                ref ShearAv, ref StressCompLeft, ref StressCompRight, ref StressLimitLeft, ref StressLimitRight, ref CDepthLeft, ref CLimitLeft, ref CDepthRight, ref CLimitRight,
                ref InelasticRotDemand, ref InelasticRotCapacity, ref NormCompStress, ref NormCompStressLimit, ref CDepth, ref BZoneL, ref BZoneR, ref BZoneLength, ref WarnMsg,
                ref ErrMsg);

            //If error
            if (Ret == 1)
            {
                MessageBox.Show("No pier design is found!");
                return null;
            }
            else
            {
                //Convert to string array
                string[] BarSpacingString = HelperMethod.ConvertDoubleToStringArray(BarSpacing);
                string[] ReinfPercentString = HelperMethod.ConvertDoubleToStringArray(ReinfPercent);
                string[] CurrPercentString = HelperMethod.ConvertDoubleToStringArray(CurrPercent);
                string[] DCRatioString = HelperMethod.ConvertDoubleToStringArray(DCRatio);
                string[] LegX1String = HelperMethod.ConvertDoubleToStringArray(LegX1);
                string[] LegY1String = HelperMethod.ConvertDoubleToStringArray(LegY1);
                string[] LegX2String = HelperMethod.ConvertDoubleToStringArray(LegX2);
                string[] LegY2String = HelperMethod.ConvertDoubleToStringArray(LegY2);
                string[] EdgeLeftString = HelperMethod.ConvertDoubleToStringArray(EdgeLeft);
                string[] EdgeRightString = HelperMethod.ConvertDoubleToStringArray(EdgeRight);
                string[] AsLeftString = HelperMethod.ConvertDoubleToStringArray(AsLeft);
                string[] AsRightString = HelperMethod.ConvertDoubleToStringArray(AsRight);
                string[] ShearAvString = HelperMethod.ConvertDoubleToStringArray(ShearAv);
                string[] StressCompLeftString = HelperMethod.ConvertDoubleToStringArray(StressCompLeft);
                string[] StressCompRightString = HelperMethod.ConvertDoubleToStringArray(StressCompRight);
                string[] StressLimitLeftString = HelperMethod.ConvertDoubleToStringArray(StressLimitLeft);
                string[] StressLimitRightString = HelperMethod.ConvertDoubleToStringArray(StressLimitRight);
                string[] CDepthLeftString = HelperMethod.ConvertDoubleToStringArray(CDepthLeft);
                string[] CLimitLeftString = HelperMethod.ConvertDoubleToStringArray(CLimitLeft);
                string[] CDepthRightString = HelperMethod.ConvertDoubleToStringArray(CDepthRight);
                string[] CLimitRightString = HelperMethod.ConvertDoubleToStringArray(CLimitRight);
                string[] InelasticRotDemandString = HelperMethod.ConvertDoubleToStringArray(InelasticRotDemand);
                string[] InelasticRotCapacityString = HelperMethod.ConvertDoubleToStringArray(InelasticRotCapacity);
                string[] NormCompStressString = HelperMethod.ConvertDoubleToStringArray(NormCompStress);
                string[] NormCompStressLimitString = HelperMethod.ConvertDoubleToStringArray(NormCompStressLimit);
                string[] CDepthString = HelperMethod.ConvertDoubleToStringArray(CDepth);
                string[] BZoneLString = HelperMethod.ConvertDoubleToStringArray(BZoneL);
                string[] BZoneRString = HelperMethod.ConvertDoubleToStringArray(BZoneR);
                string[] BZoneLengthString = HelperMethod.ConvertDoubleToStringArray(BZoneLength);

                //Combine arrays into 1
                string[] combinedArray = Story.Concat(PierLabel).Concat(Station).Concat(DesignType).Concat(PierSecType).Concat(EdgeBar).Concat(EndBar).Concat(BarSpacingString).Concat(ReinfPercentString).Concat(CurrPercentString).Concat(DCRatioString).
                    Concat(PierLeg).Concat(LegX1String).Concat(LegY1String).Concat(LegX2String).Concat(LegY2String).Concat(EdgeLeftString).Concat(EdgeRightString).Concat(AsLeftString).Concat(AsRightString).Concat(ShearAvString).Concat(StressCompLeftString)
                    .Concat(StressCompRightString).Concat(StressLimitLeftString).Concat(StressLimitRightString).Concat(CDepthLeftString).Concat(CLimitLeftString).Concat(CDepthRightString).Concat(CLimitRightString).Concat(InelasticRotDemandString).Concat(InelasticRotCapacityString)
                    .Concat(NormCompStressString).Concat(NormCompStressLimitString).Concat(CDepthString).Concat(BZoneLString).Concat(BZoneRString).Concat(BZoneLengthString).Concat(WarnMsg).Concat(ErrMsg).ToArray();

                string[] columnKeys = { "Story", "PierLabel","Station","DesignType","PierSecType","EdgeBar","EndBar","BarSpacing","ReinfPercent","CurrPercent","DCRatio","PierLeg","LegX1","LegY1","LegX2","LegY2","EdgeLeft","EdgeRight","AsLeft","AsRight","ShearAv",
                "StressCompLeft","StressCompRight","StressLimitLeft","StressLimitRight","CDepthLeft","CLimitLeft","CDepthRight","CLimitRight","InelasticRotDemand","InelasticRotCapacity","NormCompStress","NormCompStressLimit","CDepth","BZoneL","BZoneR","BZoneLength","WarnMsg","ErrMsg" };

                List<Dictionary<string, string>> tableInListPierDesignResult = HelperMethod.TransposedEtabsTableToDictionary(combinedArray, columnKeys);

                return tableInListPierDesignResult;
            }


        }

        public static List<Dictionary<string, string>> GetSpandrelDesignResult(cSapModel Sap)
        {
            //Get spandrel result
            string[] Story = null;
            string[] Spandrel = null;
            string[] Station = null;
            double[] TopRebar = null;
            double[] TopRebarRatio = null;
            string[] TopRebarCombo = null;
            double[] MuTop = null;
            double[] BotRebar = null;
            double[] BotRebarRatio = null;
            string[] BotRebarCombo = null;
            double[] MuBot = null;
            double[] AVert = null;
            double[] AHorz = null;
            string[] ShearCombo = null;
            double[] Vu = null;
            double[] ADiag = null;
            string[] ShearDiagCombo = null;
            double[] VuDiag = null;
            string[] WarnMsg = null;
            string[] ErrMsg = null;

            Ret = Sap.DesignShearWall.GetSpandrelSummaryResults(ref Story, ref Spandrel, ref Station, ref TopRebar, ref TopRebarRatio, ref TopRebarCombo, ref MuTop, ref BotRebar,
            ref BotRebarRatio, ref BotRebarCombo, ref MuBot, ref AVert, ref AHorz, ref ShearCombo, ref Vu, ref ADiag, ref ShearDiagCombo, ref VuDiag, ref WarnMsg, ref ErrMsg);

            //If error
            if (Ret == 1)
            {
                MessageBox.Show("No spandrel design is found!");
                return null;
            }
            else
            {
                //Convert to string array
                string[] TopRebarString = HelperMethod.ConvertDoubleToStringArray(TopRebar);
                string[] TopRebarRatioString = HelperMethod.ConvertDoubleToStringArray(TopRebarRatio);
                string[] MuTopString = HelperMethod.ConvertDoubleToStringArray(MuTop);
                string[] BotRebarString = HelperMethod.ConvertDoubleToStringArray(BotRebar);
                string[] BotRebarRatioString = HelperMethod.ConvertDoubleToStringArray(BotRebarRatio);
                string[] MuBotString = HelperMethod.ConvertDoubleToStringArray(MuBot);
                string[] AVertString = HelperMethod.ConvertDoubleToStringArray(AVert);
                string[] AHorzString = HelperMethod.ConvertDoubleToStringArray(AHorz);
                string[] VuString = HelperMethod.ConvertDoubleToStringArray(Vu);
                string[] ADiagString = HelperMethod.ConvertDoubleToStringArray(ADiag);
                string[] VuDiagString = HelperMethod.ConvertDoubleToStringArray(VuDiag);

                //Combine arrays into 1
                string[] combinedArray = Story.Concat(Spandrel).Concat(Station).Concat(TopRebarString).Concat(TopRebarRatioString).Concat(TopRebarCombo).Concat(MuTopString).Concat(BotRebarString).Concat(BotRebarRatioString).Concat(BotRebarCombo).Concat(MuBotString).
                    Concat(AVertString).Concat(AHorzString).Concat(ShearCombo).Concat(VuString).Concat(ADiagString).Concat(ShearDiagCombo).Concat(VuDiagString).Concat(WarnMsg).Concat(ErrMsg).ToArray();

                string[] columnKeys = { "Story", "Spandrel", "Station", "TopRebar", "TopRebarRatio", "TopRebarCombo", "MuTop", "BotRebar", "BotRebarRatio", "BotRebarCombo", "MuBot", "AVert", "AHorz", "ShearCombo", "Vu", "ADiag", "ShearDiagCombo", "VuDiag", "WarnMsg", "ErrMsg" };

                List<Dictionary<string, string>> tableInListSpandrelDesignResult = HelperMethod.TransposedEtabsTableToDictionary(combinedArray, columnKeys);

                return tableInListSpandrelDesignResult;
            }


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

        #region Frame method
        /// <summary>
        /// Get (width, depth) of rectangle section property
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="sectionPropName"></param>
        /// <returns></returns>
        public static (double, double, string) GetRcBeamDimensionAndConcreteMaterial (cSapModel Sap, string sectionPropName)
        {
            //Get property type
            ETABSv1.eFramePropType propType = new ETABSv1.eFramePropType();
            string propTypeString = "";

            Ret = Sap.PropFrame.GetTypeOAPI(sectionPropName, ref propType);
            propTypeString = propType.ToString();

            if (propTypeString == "Rectangular")
            {
                string FileName = "";
                string MatProp = "";
                double T3 = 0;
                double T2 = 0;
                int Color = 0;
                string Notes = "";
                string GUID = "";

                Ret = Sap.PropFrame.GetRectangle(sectionPropName, ref FileName, ref MatProp, ref T3, ref T2, ref Color, ref Notes, ref GUID);

                if (Ret == 0)
                {
                    return (T2, T3, MatProp);
                }
                else
                {
                    return (0, 0, "");
                }
            }
            else
            {
                MessageBox.Show($"Error extracting {sectionPropName} property! Only rectangular shape is supported!");
                return (0, 0, "");
            }


        }

        /// <summary>
        /// Get (width, depth) of column circle or rectangle section property
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="sectionPropName"></param>
        /// <returns></returns>
        public static (double, double, double, string, string) GetRcColumnDimensionAndConcreteMaterial(ETABSv1.cSapModel Sap, string sectionPropName)
        {
            //Get property type
            ETABSv1.eFramePropType propType = new ETABSv1.eFramePropType();
            string propTypeString = "";

            Ret = Sap.PropFrame.GetTypeOAPI(sectionPropName, ref propType);
            propTypeString = propType.ToString();

            if (propTypeString == "Rectangular")
            {
                string FileName = "";
                string MatProp = "";
                double T3 = 0;
                double T2 = 0;
                int Color = 0;
                string Notes = "";
                string GUID = "";

                Ret = Sap.PropFrame.GetRectangle(sectionPropName, ref FileName, ref MatProp, ref T3, ref T2, ref Color, ref Notes, ref GUID);

                if (Ret == 0)
                {
                    return (T2, T3, 0, propTypeString, MatProp);
                }
                else
                {
                    return (0, 0, 0, "", "");
                }
            }
            else if (propTypeString == "Circle")
            {
                string FileName = "";
                string MatProp = "";
                double T3 = 0;
                int Color = 0;
                string Notes = "";
                string GUID = "";

                Ret = Sap.PropFrame.GetCircle(sectionPropName, ref FileName, ref MatProp, ref T3, ref Color, ref Notes, ref GUID);

                if (Ret == 0)
                {
                    return (0, 0, T3, propTypeString, MatProp);
                }
                else
                {
                    return (0, 0, 0, "", "");
                }
            }
            else
            {
                MessageBox.Show($"Error extracting {sectionPropName} property! Only rectangular or circle shape is supported!");
                return (0, 0, 0, "", "");
            }

        }

        /// <summary>
        /// Get (cover top, cover bottom) of beam section property
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="sectionPropName"></param>
        /// <returns></returns>
        public static (double, double, string) GetRcBeamCoverAndRebarMaterial(cSapModel Sap, string sectionPropName)
        {
            string MatPropLong = "";
            string MatPropConfine = "";
            double CoverTop = 0;
            double CoverBot = 0;
            double TopLeftArea = 0;
            double TopRightArea = 0;
            double BotLeftArea = 0;
            double BotRightArea = 0;

            Ret = Sap.PropFrame.GetRebarBeam(sectionPropName, ref MatPropLong, ref MatPropConfine, ref CoverTop, ref CoverBot, ref TopLeftArea, ref TopRightArea, ref BotLeftArea, ref BotRightArea);

            if (Ret == 0)
            {
                return (CoverTop, CoverBot, MatPropLong);
            }
            else
            {
                return (0, 0, "");
            }
        }


        /// <summary>
        /// Get (cover top, cover bottom) of column section property
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="sectionPropName"></param>
        /// <returns></returns>
        public static (double, double, double, string) GetRcColumnCoverAndRebarMaterial(cSapModel Sap, string sectionPropName)
        {
            string MatPropLong = "";
            string MatPropConfine = "";
            int Pattern = 0;
            int ConfineType = 1;
            double Cover = 0;
            int NumberCBars = 0;
            int NumberR3Bars = 0;
            int NumberR2Bars = 0;
            string RebarSize = "";
            string TieSize = "";
            double TieSpacingLongit = 0;
            int Number3DirTieBars = 0;
            int Number2DirTieBars = 0;
            bool ToBeDesigned = false;

            Ret = Sap.PropFrame.GetRebarColumn(sectionPropName, ref MatPropLong, ref MatPropConfine, ref Pattern, ref ConfineType, ref Cover, ref NumberCBars, ref NumberR3Bars, ref NumberR2Bars, 
                ref RebarSize, ref TieSize, ref TieSpacingLongit, ref Number3DirTieBars, ref Number2DirTieBars, ref ToBeDesigned);

            if (Ret == 0)
            {
                return (Cover, Convert.ToDouble(RebarSize), Convert.ToDouble(TieSize), MatPropLong);
            }
            else
            {
                return (0, 0, 0, "");
            }
        }

        /// <summary>
        /// Assign frame releases using unique name of frames
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="uniqueName"></param>
        /// <param name="releaseType"></param>
        /// <param name="endi"></param>
        /// <param name="endj"></param>
        /// <returns></returns>
        public static bool AssignFrameReleases(cSapModel Sap, string uniqueName, List<string> releaseType, bool endi, bool endj)
        {
            bool[] ii = new bool[6];
            bool[] jj = new bool[6];
            double[] startValue = new double[6];
            double[] endValue = new double[6];

            if (releaseType.Contains("Axial"))
            {
                if (endi)
                {
                    ii[0] = true;
                }
                if (endj)
                {
                    jj[0] = true;
                }
                if (endi && endj)
                {
                    MessageBox.Show("Error, Axial cannot be released at both ends.");
                    return true;
                }
            }

            if (releaseType.Contains("ShearMajor"))
            {
                if (endi)
                {
                    ii[1] = true;
                }
                if (endj)
                {
                    jj[1] = true;
                }
                if (endi && endj)
                {
                    MessageBox.Show("Error, Shear Major cannot be released at both ends.");
                    return true;
                }
            }

            if (releaseType.Contains("ShearMinor"))
            {
                if (endi)
                {
                    ii[2] = true;
                }
                if (endj)
                {
                    jj[2] = true;
                }
                if (endi && endj)
                {
                    MessageBox.Show("Error, Shear Minor cannot be released at both ends.");
                    return true;
                }
            }

            if (releaseType.Contains("Torsion"))
            {
                if (endi)
                {
                    ii[3] = true;
                }
                if (endj)
                {
                    jj[3] = true;
                }
                if (endi && endj)
                {
                    MessageBox.Show("Error, Torsion cannot be released at both ends.");
                    return true;
                }
            }

            if (releaseType.Contains("MomentMinor"))
            {
                if (endi)
                {
                    ii[4] = true;
                }
                if (endj)
                {
                    jj[4] = true;
                }
                if (endi && endj && releaseType.Contains("ShearMinor"))
                {
                    MessageBox.Show("Error, Moment Minor cannot be released at both ends if Shear Minor is released at either end.");
                    return true;
                }
            }

            if (releaseType.Contains("MomentMajor"))
            {
                if (endi)
                {
                    ii[5] = true;
                }
                if (endj)
                {
                    jj[5] = true;
                }
                if (endi && endj && releaseType.Contains("ShearMajor"))
                {
                    MessageBox.Show("Error, Moment Major cannot be released at both ends if Shear Major is released at either end.");
                    return true;
                }
            }

            Ret = Sap.FrameObj.SetReleases(uniqueName, ref ii, ref jj, ref startValue, ref endValue);

            if (Ret == 0)
            {
                return false;
            }
            else
            {
                MessageBox.Show("Unknown error, please let FDS know for debugging.");
                return true;
            }
        }

        /// <summary>
        /// Retrieve the unique names of end I and end J of frame
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="sectionPropName"></param>
        /// <returns></returns>
        public static (string, string, bool) GetFramePointsUniqueName(cSapModel Sap, string frameName)
        {
            string pointI = "";
            string pointJ = "";

            Ret = Sap.FrameObj.GetPoints(frameName, ref pointI, ref pointJ);

            if (Ret == 0)
            {
                return (pointI, pointJ, false);
            }
            else
            {
                return ("", "", true);
            }
        }

        public static (bool, List<Frame>) GetFramesPoints(DatabaseTable frameObjConnectivity, DatabaseTable pointObjConnectivity)
        {
            List<Frame> framelList = new List<Frame>();
            bool isError = false;

            if(frameObjConnectivity != null && frameObjConnectivity.NumberRecords != 0)
            {
                //Convert point connectivity database to Point3D
                List<Point3D> allPointList = ConvertPointDatabaseToPoint3D(pointObjConnectivity);

                foreach (Dictionary<string, string> row in frameObjConnectivity.TableInList)
                {
                    List<Point3D> point3DList = new List<Point3D>();

                    Point3D point = null;
                    (isError, point) = GetSpecificPoint3D(row["UniquePtI"], allPointList);
                    if (!isError) { point3DList.Add(point); } else { return (isError, null); }

                    (isError, point) = GetSpecificPoint3D(row["UniquePtJ"], allPointList);
                    if (!isError) { point3DList.Add(point); } else { return (isError, null); }

                    Frame frame = new Frame(row["UniqueName"], point3DList);
                    framelList.Add(frame);
                }
            }

            return (isError, framelList);
        }

        #endregion

        #region File method

        public static string GetFileName(cSapModel Sap)
        {
            string modelPath = Sap.GetModelFilepath();

            string[] splitModelPath = modelPath.Split('.');

            string fileName = splitModelPath[0];
            return fileName;
        }

        #endregion

        #region Data source

        public static List<string> reactionDirectionList = new List<string>()
        {
            "FX", "FY", "FZ", "MX", "MY", "MZ"
        };

        public static List<string> lateralDeflectionDirectionList = new List<string>()
        {
            "Ux", "Uy"
        };


        public static List<string> lateralDeflectionDirectionWithSRSSList = new List<string>()
        {
            "Ux", "Uy", "Ux + Uy (SRSS)", "Ux and Uy (in one)", "Uz"
        };

        public static List<string> typeOfViewWallDesign = new List<string>()
        {
            "All Failure", "PMN Failure", "Shear Failure", "Pier Req. Rebar %"
        };

        public static List<string> typeLoadCombo = new List<string>()
        {
            "Linear Additive", "Envelope", "Absolute Additive", "SRSS", "Range Additive"
        };

        public static List<string> ifAlreadyExist = new List<string>()
        {
            "Modify", "Overwrite"
        };

        #endregion



    }

}

