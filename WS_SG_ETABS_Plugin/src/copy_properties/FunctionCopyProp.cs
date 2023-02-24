using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.copy_properties
{
    internal class FunctionCopyProp
    {
        public static bool CopyPropertiesFromOtherEtabs(cSapModel Sap, string filePathFrom, Dictionary<string, bool?> selectedDatabaseTables)
        {
            cSapModel SapModelFrom;
            cOAPI myETABSObjectFrom;
            bool isError = false;
            List<string> tableKeysToSave = new List<string>();

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(Sap);

            Dictionary<string, List<string>> databaseTableKeys = new Dictionary<string, List<string>>()
            {
                { "material", new List<string>(){"Material Properties - Acceptance Criteria",
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
                                                     } },
                { "frameSec", new List<string>() {"Frame Section Property Definitions - Auto Select List",
                                                    "Frame Section Property Definitions - Buckling Restrained Brace",
                                                    "Frame Section Property Definitions - Cold Formed C",
                                                    "Frame Section Property Definitions - Cold Formed Hat",
                                                    "Frame Section Property Definitions - Cold Formed Z",
                                                    "Frame Section Property Definitions - Conc Encasement Rectangle",
                                                    "Frame Section Property Definitions - Concrete Beam Reinforcing",
                                                    "Frame Section Property Definitions - Concrete Box",
                                                    "Frame Section Property Definitions - Concrete Circle",
                                                    "Frame Section Property Definitions - Concrete Column Reinforcing",
                                                    "Frame Section Property Definitions - Concrete Cross",
                                                    "Frame Section Property Definitions - Concrete Encasement Circle",
                                                    "Frame Section Property Definitions - Concrete L",
                                                    "Frame Section Property Definitions - Concrete Pipe",
                                                    "Frame Section Property Definitions - Concrete Precast I",
                                                    "Frame Section Property Definitions - Concrete Rectangular",
                                                    "Frame Section Property Definitions - Concrete Tee",
                                                    "Frame Section Property Definitions - Concrete Trapezoid",
                                                    "Frame Section Property Definitions - CoreBrace BRB",
                                                    "Frame Section Property Definitions - Filled Steel Pipe",
                                                    "Frame Section Property Definitions - Filled Steel Tube",
                                                    "Frame Section Property Definitions - General",
                                                    "Frame Section Property Definitions - Nonprismatic",
                                                    "Frame Section Property Definitions - Section Designer",
                                                    "Frame Section Property Definitions - Steel Angle",
                                                    "Frame Section Property Definitions - Steel BU I Cover Plate",
                                                    "Frame Section Property Definitions - Steel Castellated",
                                                    "Frame Section Property Definitions - Steel Cellular",
                                                    "Frame Section Property Definitions - Steel Channel",
                                                    "Frame Section Property Definitions - Steel Double Angle",
                                                    "Frame Section Property Definitions - Steel Double Channel",
                                                    "Frame Section Property Definitions - Steel I/Wide Flange",
                                                    "Frame Section Property Definitions - Steel Joist",
                                                    "Frame Section Property Definitions - Steel Pipe",
                                                    "Frame Section Property Definitions - Steel Plate",
                                                    "Frame Section Property Definitions - Steel Rod",
                                                    "Frame Section Property Definitions - Steel Tee",
                                                    "Frame Section Property Definitions - Steel Tube",
                                                    } },
                {"areaSec", new List<string>() {"Slab Property Definitions",
                                                    "Slab Property Definitions - Layers",
                                                    "Deck Property Definitions",
                                                    "Wall Property Definitions - Auto Select List",
                                                    "Wall Property Definitions - Layers",
                                                    "Wall Property Definitions - Specified",
                                                     } },
                {"linkSec", new List<string>() {"Link Property Definitions - Acceptance Criteria - Displacement",
                                                    "Link Property Definitions - Acceptance Criteria - Force",
                                                    "Link Property Definitions - Damper - Bilinear",
                                                    "Link Property Definitions - Damper - Exponential",
                                                    "Link Property Definitions - Damper - Friction Spring",
                                                    "Link Property Definitions - Gap",
                                                    "Link Property Definitions - Hook",
                                                    "Link Property Definitions - Isolator - Friction",
                                                    "Link Property Definitions - Isolator - High Damping Rubber",
                                                    "Link Property Definitions - Isolator - Rubber",
                                                    "Link Property Definitions - Isolator - TC Friction",
                                                    "Link Property Definitions - Isolator - Triple Pendulum",
                                                    "Link Property Definitions - Linear",
                                                    "Link Property Definitions - Multilinear - Elastic",
                                                    "Link Property Definitions - Multilinear - Plastic",
                                                    "Link Property Definitions - Plastic Wen",
                                                     } },
                {"springSec", new List<string>() {"Spring Property Definitions - Area Springs",
                                                "Spring Property Definitions - Area Springs",
                                                "Spring Property Definitions - Isolated Column Footings",
                                                "Spring Property Definitions - Line Springs",
                                                "Spring Property Definitions - Point Springs",
                                                "Spring Property Definitions - Soil Profiles",
                                        } },
                {"massSource", new List<string>() {"Mass Source Definition" } },
                {"pDelta", new List<string>() { "P-Delta Option Definition" } },
                {"loadPattern", new List<string>() {"Load Pattern Definitions",
                                                    "Load Pattern Definitions - Auto Notional Loads",
                                                    "Load Pattern Definitions - Auto Seismic - AS 1170 2007",
                                                    "Load Pattern Definitions - Auto Seismic - ASCE 7-02",
                                                    "Load Pattern Definitions - Auto Seismic - ASCE 7-05",
                                                    "Load Pattern Definitions - Auto Seismic - ASCE 7-10",
                                                    "Load Pattern Definitions - Auto Seismic - ASCE 7-16",
                                                    "Load Pattern Definitions - Auto Seismic - Chinese 2010",
                                                    "Load Pattern Definitions - Auto Seismic - Dominican Rep R-001",
                                                    "Load Pattern Definitions - Auto Seismic - Eurocode8 2004",
                                                    "Load Pattern Definitions - Auto Seismic - IS 1893 2002",
                                                    "Load Pattern Definitions - Auto Seismic - IS 1893 2016",
                                                    "Load Pattern Definitions - Auto Seismic - Italian NTC 2008",
                                                    "Load Pattern Definitions - Auto Seismic - Italian NTC 2018",
                                                    "Load Pattern Definitions - Auto Seismic - KBC 2009",
                                                    "Load Pattern Definitions - Auto Seismic - KBC 2016",
                                                    "Load Pattern Definitions - Auto Seismic - KDS 41 17 00:2019",
                                                    "Load Pattern Definitions - Auto Seismic - NBCC 2005",
                                                    "Load Pattern Definitions - Auto Seismic - NBCC 2010",
                                                    "Load Pattern Definitions - Auto Seismic - NBCC 2015",
                                                    "Load Pattern Definitions - Auto Seismic - NBCC 95",
                                                    "Load Pattern Definitions - Auto Seismic - NEHRP 97",
                                                    "Load Pattern Definitions - Auto Seismic - NZS 1170 2004",
                                                    "Load Pattern Definitions - Auto Seismic - TCVN 9386 2012",
                                                    "Load Pattern Definitions - Auto Seismic - TSC 2007",
                                                    "Load Pattern Definitions - Auto Seismic - TSC 2018",
                                                    "Load Pattern Definitions - Auto Seismic - UBC 97",
                                                    "Load Pattern Definitions - Auto Seismic - User Coefficient",
                                                    "Load Pattern Definitions - Auto Seismic - User Loads",
                                                    "Load Pattern Definitions - Auto Wind - AS and NZS 1170 2 2002",
                                                    "Load Pattern Definitions - Auto Wind - AS and NZS 1170 2 2011",
                                                    "Load Pattern Definitions - Auto Wind - AS and NZS 1170 2 2021",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-02",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-05",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-10",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-16",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-88",
                                                    "Load Pattern Definitions - Auto Wind - ASCE 7-95",
                                                    "Load Pattern Definitions - Auto Wind - BS 6399-95",
                                                    "Load Pattern Definitions - Auto Wind - Chinese 2010",
                                                    "Load Pattern Definitions - Auto Wind - Eurocode1 2005",
                                                    "Load Pattern Definitions - Auto Wind - Indian IS 875 1987",
                                                    "Load Pattern Definitions - Auto Wind - Indian IS 875 2015",
                                                    "Load Pattern Definitions - Auto Wind - Italian NTC 2008",
                                                    "Load Pattern Definitions - Auto Wind - Italian NTC 2018",
                                                    "Load Pattern Definitions - Auto Wind - KBC 2016",
                                                    "Load Pattern Definitions - Auto Wind - Mexican",
                                                    "Load Pattern Definitions - Auto Wind - NBCC 2005",
                                                    "Load Pattern Definitions - Auto Wind - NBCC 2010",
                                                    "Load Pattern Definitions - Auto Wind - NBCC 2015",
                                                    "Load Pattern Definitions - Auto Wind - NBCC 95",
                                                    "Load Pattern Definitions - Auto Wind - TCVN 2737 1995",
                                                    "Load Pattern Definitions - Auto Wind - Turkish TS 498-97",
                                                    "Load Pattern Definitions - Auto Wind - User Loads",
                                                    "Shell Uniform Load Sets",
                                                    } },
                {"responseSpectrum", new List<string>() {"Functions - Response Spectrum - User Defined" } },
                {"loadCase", new List<string>() {   "Modal Case Definitions - Eigen",
                                                    "Modal Case Definitions - Ritz",
                                                    "Load Case Definitions - Buckling",
                                                    "Load Case Definitions - Damping - Modal",
                                                    "Load Case Definitions - Hyperstatic",
                                                    "Load Case Definitions - Linear Static",
                                                    "Load Case Definitions - Nonlinear Staged Construction",
                                                    "Load Case Definitions - Nonlinear Static",
                                                    "Load Case Definitions - Response Spectrum",
                                                    "Load Case Definitions - Time History - Linear Direct Integration",
                                                    "Load Case Definitions - Time History - Linear Modal",
                                                    "Load Case Definitions - Time History - Nonlin Direct Integration",
                                                    "Load Case Definitions - Time History - Nonlinear Modal",
                                                    } },
                {"loadCombination", new List<string>() {"Load Combination Definitions"} },
            };

            //List of table that needs to be filtered (from load case and load combination
            List<Dictionary<string, string>> databaseTablesToFilter = new List<Dictionary<string, string>>()
            
            {
                new Dictionary<string, string>() { { "tableKey", "Mass Source Definition" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "P-Delta Option Definition" }, {"columnToFilter", "LoadPattern" } },
                new Dictionary<string, string>() { { "tableKey", "Functions - Response Spectrum - User Defined" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Modal Case Definitions - Eigen" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Modal Case Definitions - Ritz" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Buckling" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Damping - Modal" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Hyperstatic" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Linear Static" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Nonlinear Staged Construction" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Nonlinear Static" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Response Spectrum" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Time History - Linear Direct Integration" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Time History - Linear Modal" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Time History - Nonlin Direct Integration" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Case Definitions - Time History - Nonlinear Modal" }, {"columnToFilter", "Name" } },
                new Dictionary<string, string>() { { "tableKey", "Load Combination Definitions" }, {"columnToFilter", "Name" } },
            };

            //Opening model
            (isError, SapModelFrom, myETABSObjectFrom) = src.HelperEtabs.OpenModel(filePathFrom);

            if (isError)
            {
                MessageBox.Show("Unable to open file! Please recheck path!");
                return isError;
            }

            //Check if modelFrom is locked
            bool modelFromLocked = SapModelFrom.GetModelIsLocked();

            if (modelFromLocked)
            {
                myETABSObjectFrom.ApplicationExit(false);
                MessageBox.Show("Reference ETABS file is locked! Please 'save as' the model to unlock it!");
                isError = true;
                return isError;
            }

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModelFrom);

            /*
            //Get database unit in EtabsTo
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();

            (forceUnit, lengthUnit, temperatureUnit) = src.HelperEtabs.GetDatabaseUnit(Sap);
            */

            //Get unit in EtabsTo
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();
            
            (forceUnit, lengthUnit, temperatureUnit) = src.HelperEtabs.GetPresentUnit(Sap);

            //Set unit in EtabsFrom
            src.HelperEtabs.SetOutputUnit(SapModelFrom, forceUnit, lengthUnit, temperatureUnit);

            //Going through each checkbox and take note of table keys to save
            foreach (KeyValuePair<string, bool?> selectedDatabaseTable in selectedDatabaseTables)
            {
                if (selectedDatabaseTable.Value == true)
                {
                    foreach (KeyValuePair<string, List<string>> databaseTableKey in databaseTableKeys)
                    {
                        if (selectedDatabaseTable.Key == databaseTableKey.Key)
                        {
                            //If response spectrum is selected, convert response spectrum function to user function first
                            if (selectedDatabaseTable.Key == "responseSpectrum")
                            {
                                src.HelperEtabs.ConvertAllFunctionToUser(SapModelFrom);
                            }

                            //Going through each table key and save it as a flat list
                            foreach (string tableKey in databaseTableKey.Value)
                            {
                                tableKeysToSave.Add(tableKey);
                            }
                            
                        }
                        
                    }
                }
            };

            //Copy multiple tables from reference etabs
            List<src.HelperEtabs.DatabaseTable> databaseTablesEtabsFrom = src.HelperEtabs.GetMultipleTableEdit(SapModelFrom, tableKeysToSave);
            myETABSObjectFrom.ApplicationExit(false);

            List<src.HelperEtabs.DatabaseTable> mergedDatabaseTablesEtabs = new List<src.HelperEtabs.DatabaseTable>();
            //Going through each saved table in EtabsFrom and open in EtabsTo
            foreach (src.HelperEtabs.DatabaseTable databaseTableEtabsFrom in databaseTablesEtabsFrom)
            {
                
                string TableKey = "";
                string GroupName = "";
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;
                List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();

                src.HelperEtabs.DatabaseTable databaseTableEtabsTo = new src.HelperEtabs.DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);
                src.HelperEtabs.DatabaseTable mergedDatabaseTableEtabs = new src.HelperEtabs.DatabaseTable(TableKey, GroupName, TableVersion, FieldsKeysIncluded, NumberRecords, tableInList);

                databaseTableEtabsTo = src.HelperEtabs.GetTableEdit(Sap, databaseTableEtabsFrom.TableKey);


                //Filtering load case and load combination from databaseTableEtabsTo so no duplicate load
                List<Dictionary<string, string>> removedRows = new List<Dictionary<string, string>>();

                //Check if table key is in filter list
                bool needFilter = false;
                string columnToFilter = "";
                foreach (Dictionary<string, string> databaseTableToFilter in databaseTablesToFilter)
                {
                    if (databaseTableToFilter["tableKey"] == databaseTableEtabsTo.TableKey)
                    {
                        needFilter = true;
                        columnToFilter = databaseTableToFilter["columnToFilter"];
                        break;
                    }
                };

                if (needFilter)
                {
                    //Going through each row and take note of rows to remove
                    foreach (Dictionary<string, string> rowEtabsFrom in databaseTableEtabsFrom.TableInList)
                    {
                        foreach (Dictionary<string, string> rowEtabsTo in databaseTableEtabsTo.TableInList)
                        {
                            //Filter by column name
                            if (rowEtabsFrom[columnToFilter] == rowEtabsTo[columnToFilter])
                            {
                                removedRows.Add(rowEtabsTo);
                            }
                        }
                    }

                    //Go through removal list and remove the row in rowEtabsTo
                    foreach (Dictionary<string, string> removedRow in removedRows)
                    {
                        databaseTableEtabsTo.TableInList.Remove(removedRow);
                    }
                }


                //Merging data from EtabsTo and EtabsFrom
                mergedDatabaseTableEtabs.TableInList.AddRange(databaseTableEtabsTo.TableInList);
                mergedDatabaseTableEtabs.TableInList.AddRange(databaseTableEtabsFrom.TableInList);

                //Update mergedDatabaseTableEtabs properties
                mergedDatabaseTableEtabs.TableKey = databaseTableEtabsTo.TableKey;
                mergedDatabaseTableEtabs.GroupName = databaseTableEtabsTo.GroupName;
                mergedDatabaseTableEtabs.TableVersion = databaseTableEtabsTo.TableVersion;
                mergedDatabaseTableEtabs.FieldsKeysIncluded = databaseTableEtabsTo.FieldsKeysIncluded;
                mergedDatabaseTableEtabs.NumberRecords = mergedDatabaseTableEtabs.TableInList.Count();

                //Save merged table in a list
                mergedDatabaseTablesEtabs.Add(mergedDatabaseTableEtabs);
               
            }

            //Applying original area assignments
            isError = src.HelperEtabs.ApplyMultipleEtabsTable(Sap, mergedDatabaseTablesEtabs);
            if (isError) { return isError; }


            return isError;
        }


    }
}
