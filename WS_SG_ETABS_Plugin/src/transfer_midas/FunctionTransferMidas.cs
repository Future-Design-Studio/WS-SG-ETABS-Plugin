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
            List<string> textMgt = new List<string>();

            //Text file to compiled warning
            List<string> textWarning = new List<string>();

            bool isError = false;
            List<string> tableKeysToSave = new List<string>();

            //Change MIDAS unit to kN M C
            changeUnitMidas(Sap, textMgt);

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
                                copyMaterialToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
                            }

                            if (databaseTableKey.Key == "frameSec")
                            {
                                copyFrameSecToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
                            }

                            if (databaseTableKey.Key == "areaSec")
                            {
                                copyAreaSecToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
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

            writeTextFile(Sap, textMgt, "test.mgt");
            writeTextFile(Sap, textWarning, "warning.txt");

            return isError;

        }

        //Change MGT unit to kN m and C
        private static void changeUnitMidas(cSapModel Sap,  List<string> textLineByLine)
        {
            textLineByLine.Add("*UNIT");
            textLineByLine.Add("KN, M, BTU, C");
        }

        //Method to copy material to MIDAS
        private static void copyMaterialToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
        {
            //MGTHeader
            textMgt.Add("*MATERIAL");
            textMgt.Add("; i, CONC/STEEL, (name), (spheat=0), (heatco=0), (plast=\"\"), (tunit), (bmass=no), (damp ratio), (2), " +
                "(modulus elasticity), (poisson), (thermal coeff), (density), (mass)");

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            if (isError)
            {
                return;
            }

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
                    //Maximum character for material name is 16
                    string materialName = rowTableGeneral["Material"];
                    if (materialName.Length > 16)
                    {
                        materialName = materialName.Substring(0, 16);
                    }

                    double unitWeight = 0;
                    double unitMass = 0;
                    double elasticModulus = 0;
                    double poissonRatio = 0;
                    double thermalCoefficient = 0;

                    //Going through basic mech prop to find density, modulus and possion ratio
                    foreach (Dictionary<string, string> rowTableMechProp in tableMechProp)
                    {
                        if (materialName == rowTableMechProp["Material"])
                        {
                            unitWeight = Convert.ToDouble(rowTableMechProp["UnitWeight"]);
                            unitMass = Convert.ToDouble(rowTableMechProp["UnitMass"]);
                            elasticModulus = Convert.ToDouble(rowTableMechProp["E1"]);
                            poissonRatio = Convert.ToDouble(rowTableMechProp["U12"]);
                            thermalCoefficient = Convert.ToDouble(rowTableMechProp["A1"]);
                            break;
                        }
                    }
                    
                    textMgt.Add($"{index}, CONC, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
                    index++;
                }

                //For steel type
                else if (rowTableGeneral["Type"] == "Steel")
                {
                    //Maximum character for material name is 16
                    string materialName = rowTableGeneral["Material"];
                    if (materialName.Length > 16)
                    {
                        materialName = materialName.Substring(0, 16);
                    }

                    double unitWeight = 0;
                    double unitMass = 0;
                    double elasticModulus = 0;
                    double poissonRatio = 0;
                    double thermalCoefficient = 0;

                    //Going through basic mech prop to find density, modulus and possion ratio
                    foreach (Dictionary<string, string> rowTableMechProp in tableMechProp)
                    {
                        if (materialName == rowTableMechProp["Material"])
                        {
                            unitWeight = Convert.ToDouble(rowTableMechProp["UnitWeight"]);
                            unitMass = Convert.ToDouble(rowTableMechProp["UnitMass"]);
                            elasticModulus = Convert.ToDouble(rowTableMechProp["E1"]);
                            poissonRatio = Convert.ToDouble(rowTableMechProp["U12"]);
                            thermalCoefficient = Convert.ToDouble(rowTableMechProp["A1"]);
                            break;
                        }
                    }

                    textMgt.Add($"{index}, STEEL, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
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
                    string warning = $"Have material type other than concrete and steel. {rowTableGeneral["Type"]} will be imported as concrete with _CHECK behind its name!";
                    MessageBox.Show(warning);
                    textWarning.Add(warning);

                    //Import as concrete
                    //Maximum character for material name is 16
                    string materialName = rowTableGeneral["Material"];
                    if (materialName.Length > 16-7)
                    {
                        materialName = materialName.Substring(0, 16-7);
                    }

                    materialName = materialName + "_CHECK!";
                    double unitWeight = 0;
                    double unitMass = 0;
                    double elasticModulus = 0.1;
                    double poissonRatio = 0;
                    double thermalCoefficient = 0;

                    textMgt.Add($"{index}, CONC, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");
                    index++;
                }
            }

        }

        //Method to copy frame section to MIDAS
        private static void copyFrameSecToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
        {
            //MGTHeader
            textMgt.Add("*SECTION");
            textMgt.Add("; i, (section type), (name), (offset=CC), (iCENT=0), (iREF=0), (iHORZ=0), (huser=0), (iVERT=0), (vuser=0), (consider shear deformation=\"YES\"), " +
                "(consider warping effect=\"NO\"), (shape type), 2, (dimension 1 to 10)");

            //List to save property modifier
            List<Dictionary<string, double>> propertyModifiers = new List<Dictionary<string,double>>();

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            if (isError)
            {
                return;
            }

            //Gothrough each table key to get the needed table
            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                
                if (databaseTable.TableKey == "Frame Section Property Definitions - Steel I/Wide Flange")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "H";
                        double depth = Convert.ToDouble(row["t3"]);
                        double widthTop = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThicknessTop = Convert.ToDouble(row["tf"]);
                        double widthBottom = Convert.ToDouble(row["t2b"]);
                        double flangeThicknessBottom = Convert.ToDouble(row["tfb"]);
                        double radiusInner = Convert.ToDouble(row["FilletRad"]);
                        double radiusOuter = 0;

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {widthTop}, {webThickness}, {flangeThicknessTop}," +
                            $"{widthBottom}, {flangeThicknessBottom}, {radiusInner}, {radiusOuter}, 0, 0");

                        //Save property modifier
                        propertyModifierSaver(propertyModifiers, index, Convert.ToDouble(row["AMod"]), Convert.ToDouble(row["A2Mod"]), Convert.ToDouble(row["A3Mod"]),
                            Convert.ToDouble(row["JMod"]), Convert.ToDouble(row["I2Mod"]), Convert.ToDouble(row["I3Mod"]), Convert.ToDouble(row["MMod"]), Convert.ToDouble(row["WMod"]));

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Channel")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "C";
                        double depth = Convert.ToDouble(row["t3"]);
                        double widthTop = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThicknessTop = Convert.ToDouble(row["tf"]);
                        double widthBottom = Convert.ToDouble(row["t2"]);
                        double flangeThicknessBottom = Convert.ToDouble(row["tf"]);
                        double radiusInner = Convert.ToDouble(row["FilletRad"]);
                        double radiusOuter = 0;

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {widthTop}, {webThickness}, {flangeThicknessTop}," +
                            $"{widthBottom}, {flangeThicknessBottom}, {radiusInner}, {radiusOuter}, 0, 0");

                        index++;
                    }

                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Tee")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "T";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThickness = Convert.ToDouble(row["tf"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {webThickness}, {flangeThickness}, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Angle")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }
;
                        string sectionShape = "L";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThickness = Convert.ToDouble(row["tf"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {webThickness}, {flangeThickness}, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Double Angle")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "2L";
                        double depth = Convert.ToDouble(row["t3"]);
                        double widthSingleAngle = Convert.ToDouble(row["SingleAngW"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThickness = Convert.ToDouble(row["tf"]);
                        double gap = Convert.ToDouble(row["dis"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {widthSingleAngle}, {webThickness}, {flangeThickness}, " +
                            $"{gap}, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Double Channel")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "2C";
                        double depth = Convert.ToDouble(row["t3"]);
                        double widthSingleChannel = Convert.ToDouble(row["SingleChanW"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThickness = Convert.ToDouble(row["tf"]);
                        double gap = Convert.ToDouble(row["dis"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {widthSingleChannel}, {webThickness}, {flangeThickness}, " +
                            $"{gap}, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Tube")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "B";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThicknessTop = Convert.ToDouble(row["tf"]);
                        double centerToCenter = 0;
                        double flangeThicknessBottom = Convert.ToDouble(row["tf"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {webThickness}, {flangeThicknessTop}, " +
                            $"{centerToCenter}, {flangeThicknessBottom}, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Pipe")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "P";
                        double diameter = Convert.ToDouble(row["t3"]);
                        double thickness = Convert.ToDouble(row["tw"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {diameter}, {thickness}, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Plate")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "SB";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Steel Rod")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "SR";
                        double diameter = Convert.ToDouble(row["t3"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {diameter}, 0, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Rectangular")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "SB";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Circle")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "SR";
                        double diameter = Convert.ToDouble(row["t3"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {diameter}, 0, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Box")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "B";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double webThickness = Convert.ToDouble(row["tw"]);
                        double flangeThicknessTop = Convert.ToDouble(row["tf"]);
                        double centerToCenter = 0;
                        double flangeThicknessBottom = Convert.ToDouble(row["tf"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {webThickness}, {flangeThicknessTop}, " +
                            $"{centerToCenter}, {flangeThicknessBottom}, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Pipe")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "P";
                        double diameter = Convert.ToDouble(row["t3"]);
                        double thickness = Convert.ToDouble(row["tw"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {diameter}, {thickness}, 0, 0, 0, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Cold Formed C")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "CC";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double thickness = Convert.ToDouble(row["tw"]);
                        double radius = Convert.ToDouble(row["Radius"]);
                        double lip = Convert.ToDouble(row["LipDepth"]) + Convert.ToDouble(row["tw"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {thickness}, {radius}," +
                            $"{lip}, 0, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Cold Formed Z")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for material name is 16
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        string sectionShape = "Z";
                        double depth = Convert.ToDouble(row["t3"]);
                        double width = Convert.ToDouble(row["t2"]);
                        double thickness = Convert.ToDouble(row["tw"]);
                        double radius = Convert.ToDouble(row["Radius"]);
                        double lip = Convert.ToDouble(row["LipDepth"]) + Convert.ToDouble(row["tw"]);
                        double lipAngle = Convert.ToDouble(row["AngleOfLip"]);

                        textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, {thickness}, {radius}," +
                            $"{lip}, {lipAngle}, 0, 0, 0, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Auto Select List")
                {
                    //Auto select will not be bring over
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will import as a rectangular section with _CHECK behind its name!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);

                        //Import as a rectangular section
                        //Placeholder for each table
                        List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                        table = databaseTable.TableInList;

                        foreach (Dictionary<string, string> row in table)
                        {
                            //Maximum character for material name is 16
                            string name = row["Name"];
                            if (name.Length > 28 - 7)
                            {
                                name = name.Substring(0, 28 - 7);
                            }

                            name = name + "_CHECK!";
                            string sectionShape = "SB";
                            double depth = 1;
                            double width = 1;

                            textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {depth}, {width}, 0, 0, 0, 0, 0, 0, 0, 0");

                            index++;
                        }
                    }
                }
            }
        }

        //Method to save property modifier
        private static void propertyModifierSaver(List<Dictionary<string,double>> propertyModifiers, int index, double areaMod, double a2Mod, double a3Mod, double jMod, double i2Mod, 
            double i3Mod, double mMod, double wMod)
        {
            Dictionary<string, double> propertyModifier = new Dictionary<string, double>()
            {
                {"areaMod", areaMod},
                {"aYMod", a2Mod},
                {"aZMod", a3Mod},
                {"torsionalMod", jMod},
                {"iYMod", i2Mod},
                {"iZMod", i3Mod},
                {"weightMod", mMod},
            };

            propertyModifiers.Add(propertyModifier);

        }

        //Method to copy area section to MIDAS
        private static void copyAreaSecToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
        {
            //MGTHeader
            textMgt.Add("*THICKNESS");
            textMgt.Add("; i, (section type=VALUE), (name), (same thickness all around=YES), (thickness in plane), (thickness out of plane=0), (offset=NO), (offtype=0), (value=0)");

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            if (isError)
            {
                return;
            }

            //Gothrough each table key to get the needed table
            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Area Section Property Definitions - Summary")
                {
                    //do nothing for the summary
                }
                else if (databaseTable.TableKey == "Slab Property Definitions")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    //Indicator if one way slab warning has been shown
                    bool layeredWarningShown = false;
                    bool membraneWarningShown = false;
                    bool oneWayWarningShown = false;
                    bool ribbedWarningShown = false;
                    bool waffleWarningShown = false;
                    bool dropWarningShown = false;
                    bool stiffWarningShown = false;
                    bool matWarningShown = false;
                    bool footingWarningShown = false;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Layered type does not show any thickness. Thus need to seperate
                        if (row["ModelType"] == "Layered")
                        {
                            //Show warning if havent show
                            if (!layeredWarningShown)
                            {
                                string warning = "Layered type is not supported in MIDAS! Will import as normal slab with 0.1 thickness with _CHECK behind its name!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                layeredWarningShown = true;
                            }

                            //Maximum character for area property name is 28
                            string name = row["Name"];
                            if (name.Length > 28 - 7)
                            {
                                name = name.Substring(0, 28 - 7);
                            }

                            name = name + "_CHECK!";
                            double thickness = 0.1;

                            textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                            index++;
                        }
                        //Ribbed and waffle type not supported in MIDAS
                        else if (row["PropType"] == "Ribbed" || row["PropType"] == "Waffle")
                        {
                            //Show membrane warning if havent shown
                            if (!membraneWarningShown && row["ModelType"] == "Membrane")
                            {
                                string warning = "Membrane type is not supported in MIDAS! Will import as normal slab!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                membraneWarningShown = true;

                                //Show one way slab warning if havent shown
                                if (!oneWayWarningShown && row["OneWayLoad"] == "Yes")
                                {
                                    warning = "One way type is not supported in MIDAS! Will import as normal slab!";
                                    MessageBox.Show(warning);
                                    textWarning.Add(warning);
                                    oneWayWarningShown = true;
                                }
                            }

                            if (row["PropType"] == "Ribbed")
                            {
                                //Show warning if havent show
                                if (!ribbedWarningShown)
                                {
                                    string warning = "Ribbed type is not supported in MIDAS! Will import as normal slab with _CHECK behind its name!";
                                    MessageBox.Show(warning);
                                    textWarning.Add(warning);
                                    ribbedWarningShown = true;
                                }
                            }
                            else if (row["PropType"] == "Waffle")
                            {
                                //Show warning if havent show
                                if (!waffleWarningShown)
                                {
                                    string warning = "Waffle type is not supported in MIDAS! Will import as normal slab with _CHECK behind its name!";
                                    MessageBox.Show(warning);
                                    textWarning.Add(warning);
                                    waffleWarningShown = true;
                                }
                            }

                            //Maximum character for area property name is 28
                            string name = row["Name"];
                            if (name.Length > 28 - 7)
                            {
                                name = name.Substring(0, 28 - 7);
                            }

                            name = name + "_CHECK!";
                            double thickness = Convert.ToDouble(row["Thickness"]);

                            textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                            index++;
                        }
                        //Normal slab proceed as usual
                        else
                        {
                            //Show membrane warning if havent shown
                            if (!membraneWarningShown && row["ModelType"] == "Membrane")
                            {
                                string warning = "Membrane type is not supported in MIDAS! Will import as normal slab!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                membraneWarningShown = true;

                                //Show one way slab warning if havent shown
                                if (!oneWayWarningShown && row["OneWayLoad"] == "Yes")
                                {
                                    warning = "One way type is not supported in MIDAS! Will import as normal slab!";
                                    MessageBox.Show(warning);
                                    textWarning.Add(warning);
                                    oneWayWarningShown = true;
                                }
                            }

                            //Show drop warning if havent shown
                            if (!dropWarningShown && row["PropType"] == "Drop")
                            {
                                string warning = "Drop type is not supported in MIDAS! Will import as normal slab! Might cause area overlap!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                dropWarningShown = true;
                            }

                            //Show stiff warning if havent shown
                            if (!stiffWarningShown && row["PropType"] == "Stiff")
                            {
                                string warning = "Stiff type is not supported in MIDAS! Will import as normal slab!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                stiffWarningShown = true;
                            }

                            //Show drop warning if havent shown
                            if (!matWarningShown && row["PropType"] == "Mat")
                            {
                                string warning = "Mat type is not supported in MIDAS! Will import as normal slab!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                matWarningShown = true;
                            }
                            //Show footing warning if havent shown
                            if (!footingWarningShown && row["PropType"] == "Footing")
                            {
                                string warning = "Footing type is not supported in MIDAS! Will import as normal slab!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                                footingWarningShown = true;
                            }

                            //Maximum character for area property name is 28
                            string name = row["Name"];
                            if (name.Length > 28)
                            {
                                name = name.Substring(0, 28);
                            }

                            double thickness = Convert.ToDouble(row["Thickness"]);

                            textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                            index++;
                        }
                    }
                }
                else if (databaseTable.TableKey == "Wall Property Definitions - Specified")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Maximum character for area property name is 28
                        string name = row["Name"];
                        if (name.Length > 28)
                        {
                            name = name.Substring(0, 28);
                        }

                        double thickness = Convert.ToDouble(row["Thickness"]);

                        textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                        index++;
                    }
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will NOT be imported!";
                        //MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
            }
        }

        private static void writeTextFile(cSapModel Sap, List<string> textLineByLine, string filename)
        {            
            //Text file properties
            string modelPath = Sap.GetModelFilepath() + filename;
            //string fileName = @"C:\Temp\Mahesh.txt";
            FileStream stream = null;
            stream = new FileStream(modelPath, FileMode.Create);

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
