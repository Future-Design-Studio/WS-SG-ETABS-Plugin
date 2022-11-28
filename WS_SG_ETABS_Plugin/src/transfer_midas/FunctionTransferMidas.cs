using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.IO;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.transfer_midas
{
    internal class FunctionTransferMidas
    {
        public static bool TransferToMidas(cSapModel Sap, string filePathFrom, Dictionary<string, bool?> selectedDatabaseTables)
        {
            //Text file line by line to be converted to MGT
            List<string> textLineByLine = new List<string>();

            bool isError = false;
            List<string> tableKeysToSave = new List<string>();

            //Change MIDAS unit to kN M C
            changeUnitMidas(Sap, textLineByLine);

            //Change ETABS unit to kN M C
            src.HelperEtabs.SetOutputUnit(Sap, eForce.kN, eLength.m, eTemperature.C);

            //List of database table keys
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

            //Going through each checkbox and take note of table keys to save
            foreach (KeyValuePair<string, bool?> selectedDatabaseTable in selectedDatabaseTables)
            {
                if (selectedDatabaseTable.Value == true)
                {
                    foreach (KeyValuePair<string, List<string>> databaseTableKey in databaseTableKeys)
                    {
                        if (selectedDatabaseTable.Key == databaseTableKey.Key)
                        {
                            //Going through each category to copy
                            //Copying material
                            if (databaseTableKey.Key == "material")
                            {
                                copyMaterialToMidas(Sap, databaseTableKey.Value, textLineByLine);
                            }

                            if (databaseTableKey.Key == "frameSec")
                            {
                                copyFrameSecToMidas(Sap, databaseTableKey.Value, textLineByLine);
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

            /*
            //Copy multiple tables from reference etabs
            List<src.HelperEtabs.DatabaseTable> databaseTablesEtabsFrom = new List<src.HelperEtabs.DatabaseTable>();
            (isError, databaseTablesEtabsFrom) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeysToSave, true);
            */

            writeMgtFile(Sap, textLineByLine);

            return isError;

        }

        //Change MGT unit to kN m and C
        private static void changeUnitMidas(cSapModel Sap,  List<string> textLineByLine)
        {
            textLineByLine.Add("*UNIT");
            textLineByLine.Add("KN, M, BTU, C");
        }

        //Method to copy material to MIDAS
        private static void copyMaterialToMidas(cSapModel Sap, List<string> tableKeys, List<string> textLineByLine)
        {
            //MGTHeader
            textLineByLine.Add("*MATERIAL");

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //Placeholder for each table
            List<Dictionary<string, string>> tableGeneral = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> tableMechProp = new List<Dictionary<string, string>>();

            //Gothrough each table key to get the needed table
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Material Properties - General")
                {
                    tableGeneral = databaseTable.TableInList;
                }
                else if (databaseTable.TableKey == "Material Properties - Basic Mechanical Properties")
                {
                    tableMechProp = databaseTable.TableInList;
                }

            }

            int index = 1;
            foreach (Dictionary<string, string> rowTableGeneral in tableGeneral)
            {
                //For concrete type
                if (rowTableGeneral["Type"] == "Concrete")
                {
                    string materialName = rowTableGeneral["Material"];
                    string unitWeight = "";
                    string unitMass = "";
                    string elasticModulus = "";
                    string poissonRatio = "";
                    string thermalCoefficient = "";

                    //Going through basic mech prop to find density, modulus and possion ratio
                    foreach (Dictionary<string, string> rowTableMechProp in tableMechProp)
                    {
                        if (materialName == rowTableMechProp["Material"])
                        {
                            unitWeight = rowTableMechProp["UnitWeight"];
                            unitMass = rowTableMechProp["UnitMass"];
                            elasticModulus = rowTableMechProp["E1"];
                            poissonRatio = rowTableMechProp["U12"];
                            thermalCoefficient = rowTableMechProp["A1"];
                            break;
                        }
                    }
                    
                    textLineByLine.Add($"{index}, CONC, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
                    index++;
                }

                //For steel type
                else if (rowTableGeneral["Type"] == "Steel")
                {
                    string materialName = rowTableGeneral["Material"];
                    string unitWeight = "";
                    string unitMass = "";
                    string elasticModulus = "";
                    string poissonRatio = "";
                    string thermalCoefficient = "";

                    //Going through basic mech prop to find density, modulus and possion ratio
                    foreach (Dictionary<string, string> rowTableMechProp in tableMechProp)
                    {
                        if (materialName == rowTableMechProp["Material"])
                        {
                            unitWeight = rowTableMechProp["UnitWeight"];
                            unitMass = rowTableMechProp["UnitMass"];
                            elasticModulus = rowTableMechProp["E1"];
                            poissonRatio = rowTableMechProp["U12"];
                            thermalCoefficient = rowTableMechProp["A1"];
                            break;
                        }
                    }

                    textLineByLine.Add($"{index}, STEEL, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
                    index++;
                }

                /*
                //For rebar type
                else if (rowTableGeneral["Type"] == "Rebar")
                {
                    string materialName = rowTableGeneral["Material"];
                    string unitWeight = "";
                    string unitMass = "";
                    string elasticModulus = "";
                    string poissonRatio = "";
                    string thermalCoefficient = "";

                    //Going through basic mech prop to find density, modulus and possion ratio
                    foreach (Dictionary<string, string> rowTableMechProp in tableMechProp)
                    {
                        if (materialName == rowTableMechProp["Material"])
                        {
                            unitWeight = rowTableMechProp["UnitWeight"];
                            unitMass = rowTableMechProp["UnitMass"];
                            elasticModulus = rowTableMechProp["E1"];
                            poissonRatio = Convert.ToString(0.05);
                            thermalCoefficient = rowTableMechProp["A1"];
                            break;
                        }
                    }

                    textLineByLine.Add($"{index}, USER, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
                    index++;
                }
                */

                else
                {
                    MessageBox.Show($"Have material type other than concrete and steel. {rowTableGeneral["Type"]} will NOT be imported!");
                }
            }

        }

        //Method to copy material to MIDAS
        private static void copyFrameSecToMidas(cSapModel Sap, List<string> tableKeys, List<string> textLineByLine)
        {

        }

        private static void writeMgtFile(cSapModel Sap, List<string> textLineByLine)
        {            
            //Text file properties
            string modelPath = Sap.GetModelFilepath() + "test.mgt";
            //string fileName = @"C:\Temp\Mahesh.txt";
            FileStream stream = null;
            stream = new FileStream(modelPath, FileMode.OpenOrCreate);

            Encoding utf8WithoutBom = new UTF8Encoding(false);

            using (StreamWriter writer = new StreamWriter(stream, utf8WithoutBom))
            {
                foreach (string line in textLineByLine)
                {
                    writer.WriteLine(line);
                }

            }

        }

    }



}
