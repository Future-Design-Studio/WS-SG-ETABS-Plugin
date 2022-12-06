using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Reflection;
using System.Security.Policy;

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

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(Sap);

            //List of database table keys
            Dictionary<string, List<string>> databaseTableKeysDisplay = new Dictionary<string, List<string>>()
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
                                                    "Frame Section Property Definitions - Summary"
                                                    } },
                {"areaSec", new List<string>() {"Area Section Property Definitions - Summary",
                                                "Slab Property Definitions",
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
                                                    /*
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
                                                    */
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
                {"objectConnectivity", new List<string>() {"Beam Object Connectivity",
                                                            "Brace Object Connectivity",
                                                            "Column Object Connectivity",
                                                            "Floor Object Connectivity",
                                                            "Line Gauge Object Connectivity",
                                                            "Link Object Connectivity",
                                                            "Null Area Object Connectivity",
                                                            "Null Line Object Connectivity",
                                                            "Point Object Connectivity",
                                                            "Quad Gauge Object Connectivity",
                                                            "Strip Object Connectivity",
                                                            "Tendon Object Connectivity",
                                                            "Wall Object Connectivity",
                                                            "Beam Object Curve Data",
                                                            "Wall Object Curve Data",
                                                            "Floor Object Curved Edge Data",
                                                            "Slab Rebar Object Geometry",
                                                            "Dimension Line Object Geometry",
                                                            //Assignments needed in MIDAS for geometry creation
                                                            "Frame Assignments - Section Properties",
                                                            "Frame Assignments - Local Axes",
                                                            "Area Assignments - Section Properties",
                                                            "Area Assignments - Local Axes",
                                                            } },
                {"frameAssignments", new List<string>() {"Frame Assignments - Additional Mass",
                                                            "Frame Assignments - Advanced Nonprismatic Section Parameters",
                                                            "Frame Assignments - Column Splice Overwrites",
                                                            "Frame Assignments - Corebrace BRB Details",
                                                            "Frame Assignments - End Length Offsets",
                                                            "Frame Assignments - Floor Cracking",
                                                            "Frame Assignments - Frame Auto Mesh Options",
                                                            "Frame Assignments - Frame Floor Meshing Option",
                                                            "Frame Assignments - Hinge - ASCE 41-13 - Concrete Beam",
                                                            "Frame Assignments - Hinge - ASCE 41-13 - Concrete Column",
                                                            "Frame Assignments - Hinge - ASCE 41-13 - Steel Beam",
                                                            "Frame Assignments - Hinge - ASCE 41-13 - Steel Brace",
                                                            "Frame Assignments - Hinge - ASCE 41-13 - Steel Column",
                                                            "Frame Assignments - Hinge - ASCE 41-17 - Concrete Beam",
                                                            "Frame Assignments - Hinge - ASCE 41-17 - Concrete Column",
                                                            "Frame Assignments - Hinge - ASCE 41-17 - Steel Beam",
                                                            "Frame Assignments - Hinge - ASCE 41-17 - Steel Brace",
                                                            "Frame Assignments - Hinge - ASCE 41-17 - Steel Column",
                                                            "Frame Assignments - Hinge - BRB",
                                                            "Frame Assignments - Hinge - Overwrites",
                                                            "Frame Assignments - Hinge - Summary",
                                                            "Frame Assignments - Hinge - User",
                                                            "Frame Assignments - Insertion Point",
                                                            "Frame Assignments - Line Springs",
                                                            //"Frame Assignments - Local Axes", put inside object connectivity
                                                            "Frame Assignments - Material Overwrites",
                                                            "Frame Assignments - Moment Frame Beam Connection Type",
                                                            "Frame Assignments - Output Stations",
                                                            "Frame Assignments - Pier Labels",
                                                            "Frame Assignments - Property Modifiers",
                                                            "Frame Assignments - Rebar Ratio For Creep Analysis",
                                                            "Frame Assignments - Releases and Partial Fixity",
                                                            //"Frame Assignments - Section Properties", put inside object connectivity
                                                            "Frame Assignments - SidePlate Data",
                                                            "Frame Assignments - Spandrel Labels",
                                                            "Frame Assignments - Tension and Compression Limits",
                                                            } },
                {"jointAssignments", new List<string>() {"Joint Assignments - Additional Mass",
                                                        "Joint Assignments - Diaphragms",
                                                        "Joint Assignments - Floor Meshing Option",
                                                        "Joint Assignments - Panel Zone",
                                                        "Joint Assignments - Restraints",
                                                        "Joint Assignments - Springs",
                                                        } },
                {"areaAssignments", new List<string>() {"Area Assignments - Additional Mass",
                                                        "Area Assignments - Area Springs",
                                                        "Area Assignments - Auto Edge Constraints",
                                                        "Area Assignments - Diaphragms",
                                                        "Area Assignments - Edge Releases",
                                                        "Area Assignments - Floor Auto Mesh Options",
                                                        "Area Assignments - Floor Cracking",
                                                        "Area Assignments - Insertion Point",
                                                        //"Area Assignments - Local Axes", put inside object connectivity
                                                        "Area Assignments - Material Overwrites",
                                                        "Area Assignments - Pier Labels",
                                                        //"Area Assignments - Section Properties", put inside object connectivity
                                                        "Area Assignments - Slab Line Releases",
                                                        "Area Assignments - Slab Rib Locations",
                                                        "Area Assignments - Spandrel Labels",
                                                        "Area Assignments - Stiffness Modifiers",
                                                        "Area Assignments - Summary",
                                                        "Area Assignments - Thickness Overwrites",
                                                        "Area Assignments - Wall Auto Mesh Options",
                                                        "Area Assignments - Wall Hinge - Auto Fiber PM3",
                                                        "Area Assignments - Wall Hinge Reinforcement",
                                                        "Area Assignments - Wall Hinges",
                                                        "Area Assignments - Wall Rebar Ratio for Creep Analysis",
                                                        } },
                {"frameLoadsAssignments", new List<string>() {"Frame Loads Assignments - Distributed",
                                                            "Frame Loads Assignments - Open Structure Wind Parameters",
                                                            "Frame Loads Assignments - Point",
                                                            "Frame Loads Assignments - Temperature",
                                                            } },
                {"jointLoadsAssignments", new List<string>() {"Joint Loads Assignments - Force",
                                                            "Joint Loads Assignments - Ground Displacement",
                                                            "Joint Loads Assignments - Temperature",
                                                            } },
                {"areaLoadAssignments", new List<string>() {"Area Load Assignments - Non-uniform",
                                                            "Area Load Assignments - Temperature",
                                                            "Area Load Assignments - Uniform",
                                                            "Area Load Assignments - Uniform Load Sets",
                                                            "Area Load Assignments - Wind Pressure Coefficients",
                                                            } },
            };

            //add in temporary in selectedDatabaseTables
            selectedDatabaseTables.Add("objectConnectivity", true);
            selectedDatabaseTables.Add("frameAssignments", true);
            selectedDatabaseTables.Add("jointAssignments", true);
            selectedDatabaseTables.Add("areaAssignments", true);
            selectedDatabaseTables.Add("frameLoadsAssignments", true);
            selectedDatabaseTables.Add("jointLoadsAssignments", true);
            selectedDatabaseTables.Add("areaLoadAssignments", true);

            //To save material index
            Dictionary<string, string> nameIndexMaterial = new Dictionary<string, string>();

            //To save section properties material index
            Dictionary<string, string> nameIndexSecProp = new Dictionary<string, string>();
            Dictionary<string, string> nameMatIndexSecProp = new Dictionary<string, string>();

            //To save area properties material index
            Dictionary<string, string> nameIndexAreaProp = new Dictionary<string, string>();
            Dictionary<string, string> nameMatIndexAreaProp = new Dictionary<string, string>();
            Dictionary<string, string> nameThinThickAreaProp = new Dictionary<string, string>();
            
            //To load pattern
            Dictionary<string, string> loadPatternNameEtabsMidas = new Dictionary<string, string>();

            //To save node, frame and area index
            Dictionary<string, string> nameIndexNode = new Dictionary<string, string>();
            Dictionary<string, string> nameIndexFrame = new Dictionary<string, string>();
            Dictionary<string, string> nameIndexArea = new Dictionary<string, string>();

            //Going through each checkbox and take note of table keys to save
            foreach (KeyValuePair<string, bool?> selectedDatabaseTable in selectedDatabaseTables)
            {
                if (selectedDatabaseTable.Value == true)
                {
                    foreach (KeyValuePair<string, List<string>> databaseTableKey in databaseTableKeysDisplay)
                    {
                        if (selectedDatabaseTable.Key == databaseTableKey.Key)
                        {
                            //Going through each category to copy
                            //Copying material
                            if (databaseTableKey.Key == "material")
                            {
                                nameIndexMaterial = copyMaterialToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
                            }

                            if (databaseTableKey.Key == "frameSec")
                            {
                                (nameIndexSecProp, nameMatIndexSecProp) = copyFrameSecToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexMaterial);
                            }

                            if (databaseTableKey.Key == "areaSec")
                            {
                                (nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp) = copyAreaSecToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexMaterial);
                            }

                            if (databaseTableKey.Key == "loadPattern")
                            {
                                loadPatternNameEtabsMidas = copyLoadPatternToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
                            }

                            if (databaseTableKey.Key == "objectConnectivity")
                            {
                                (nameIndexNode, nameIndexFrame, nameIndexArea) = copyObjectConnectivityToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexSecProp, 
                                    nameMatIndexSecProp, nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp);
                            }

                            if (databaseTableKey.Key == "frameAssignments")
                            {
                                copyFrameAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexFrame);
                            }

                            if (databaseTableKey.Key == "jointAssignments")
                            {
                                copyJointAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode);
                            }

                            /*
                            if (databaseTableKey.Key == "areaAssignments")
                            {
                                copyAreaAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexAreaProp);
                            }
                            */

                            if (databaseTableKey.Key == "frameLoadsAssignments")
                            {
                                copyFrameLoadsAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexFrame, loadPatternNameEtabsMidas);
                            }

                            if (databaseTableKey.Key == "jointLoadsAssignments")
                            {
                                copyJointLoadsAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode, loadPatternNameEtabsMidas);
                            }

                            /*
                            nameIndexNode;
                            nameIndexFrame;
                            nameIndexArea
                            */


                            /*
                            //Going through each table key and save it as a flat list
                            foreach (string tableKey in databaseTableKey.Value)
                            {
                                tableKeysToSave.Add(tableKey);
                            }
                            */

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
        private static Dictionary<string, string> copyMaterialToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
        {
            //List to save name and index
            Dictionary<string, string> nameIndexMaterial = new Dictionary<string, string>();

            //MGTHeader
            textMgt.Add("*MATERIAL");
            textMgt.Add("; i, CONC/STEEL, (name), (spheat=0), (heatco=0), (plast=\"\"), (tunit), (bmass=no), (damp ratio), (2), " +
                "(modulus elasticity), (poisson), (thermal coeff), (density), (mass)");

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

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

                    //Save material name and index
                    Dictionary<string, string> nameIndexMaterialSingle = new Dictionary<string, string>() { };
                    nameIndexMaterial.Add(rowTableGeneral["Material"], Convert.ToString(index));

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

                    //Save material name and index
                    Dictionary<string, string> nameIndexMaterialSingle = new Dictionary<string, string>() { };
                    nameIndexMaterial.Add(rowTableGeneral["Material"], Convert.ToString(index));

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

                    //Save material name and index
                    Dictionary<string, string> nameIndexMaterialSingle = new Dictionary<string, string>() { };
                    nameIndexMaterial.Add(rowTableGeneral["Material"], Convert.ToString(index));

                    index++;
                }
            }

            return nameIndexMaterial;

        }

        //Method to copy frame section to MIDAS
        private static (Dictionary<string, string>, Dictionary<string, string>) copyFrameSecToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, 
            Dictionary<string, string> nameIndexMaterial)
        {
            //MGTHeader
            textMgt.Add("*SECTION");
            textMgt.Add("; i, (section type), (name), (offset=CC), (iCENT=0), (iREF=0), (iHORZ=0), (huser=0), (iVERT=0), (vuser=0), (consider shear deformation=\"YES\"), " +
                "(consider warping effect=\"NO\"), (shape type), 2, (dimension 1 to 10)");

            //List to save name and index
            Dictionary<string, string> nameIndexSecProp = new Dictionary<string, string>();

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

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

                        //Save property name and index
                        nameIndexSecProp.Add( row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

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

                        //Save property name and index
                        nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Auto Select List")
                {
                    //Auto select will not be bring over
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Beam Reinforcing")
                {
                    //Concrete beam reinforcing will not be bring over
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Concrete Column Reinforcing")
                {
                    //Concrete column reinforcing will not be bring over
                }
                else if (databaseTable.TableKey == "Frame Section Property Definitions - Summary")
                {
                    //Summary will not be bring over
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

                            //Save property name and index
                            nameIndexSecProp.Add(row["Name"], Convert.ToString(index));

                            index++;
                        }
                    }
                }
            }

            //List to save material index
            Dictionary<string, string> nameMatIndexSecProp = new Dictionary<string, string>();

            //MGTHeader for section property stiffness modifier
            textMgt.Add("*SECT-SCALE");
            textMgt.Add("; i, (area modifier), (shear minor modifier), (shear major modifier), (torsion modifier), (moment major modifier), (moment minor modifier)");

            //Save stiffness modifiers and material index
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                //Finding the summary table
                if (databaseTable.TableKey == "Frame Section Property Definitions - Summary")
                {
                    //Going through each row in the summary table and find the matching name
                    foreach (Dictionary<string, string> row in databaseTable.TableInList)
                    {
                        //Check if section property exist in nameIndexSecProp
                        if (nameIndexSecProp.ContainsKey(row["Name"]))
                        {
                            //Writing modifiers
                            if (row["Shape"] == "Auto Select" || row["Shape"] == "Joist Section" || row["Shape"] == "Nonprismatic")
                            {
                                //For this shape there is no option to add modifiers
                            }
                            else
                            {
                                //Access using key=name to access value=index
                                string secPropIndex = nameIndexSecProp[row["Name"]];

                                double areaMod = Convert.ToDouble(row["AMod"]);
                                double aYMod = Convert.ToDouble(row["A2Mod"]);
                                double aZMod = Convert.ToDouble(row["A3Mod"]);
                                double jMod = Convert.ToDouble(row["JMod"]);
                                double iYMod = Convert.ToDouble(row["I3Mod"]);
                                double iZMod = Convert.ToDouble(row["I2Mod"]);
                                double weightMod = 0;

                                if (Convert.ToDouble(row["MMod"]) != Convert.ToDouble(row["WMod"]))
                                {
                                    string warning = $"Different weight and mass modifier found in section property {row["Name"]}. Highest value will be used!";
                                    MessageBox.Show(warning);
                                    textWarning.Add(warning);

                                    if (Convert.ToDouble(row["MMod"]) > Convert.ToDouble(row["WMod"]))
                                    {
                                        weightMod = Convert.ToDouble(row["MMod"]);
                                    }
                                    else
                                    {
                                        weightMod = Convert.ToDouble(row["WMod"]);
                                    }
                                }
                                else
                                {
                                    weightMod = Convert.ToDouble(row["WMod"]);
                                }

                                if (areaMod == 1 && aYMod == 1 && aZMod == 1 && jMod == 1 && iYMod == 1 && iZMod == 1 && weightMod == 1)
                                {
                                    //if no modifier then do nothing
                                }
                                else
                                {
                                    textMgt.Add($"{secPropIndex}, {areaMod}, {aYMod}, {aZMod}, {jMod}, {iYMod}, {iZMod}, {weightMod}");
                                }
                            }

                            //Finding material index
                            if (row["Shape"] == "Auto Select" || row["Shape"] == "BU I Cover Plate" || row["Shape"] == "Nonprismatic")
                            {
                                //For this shape there is no option to add material thus put material index 0
                                nameMatIndexSecProp.Add(row["Name"], "0");
                            }
                            else
                            {
                                string materialIndex = nameIndexMaterial[row["Material"]];
                                nameMatIndexSecProp.Add(row["Name"], materialIndex);
                            }
                        }
                    }
                }
            }

            return (nameIndexSecProp, nameMatIndexSecProp);
        }

        //Method to copy area section to MIDAS
        private static (Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>) copyAreaSecToMidas(cSapModel Sap, List<string> tableKeys, 
            List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexMaterial)
        {
            //MGTHeader
            textMgt.Add("*THICKNESS");
            textMgt.Add("; i, (section type=VALUE), (name), (same thickness all around=YES), (thickness in plane), (thickness out of plane=0), (offset=NO), (offtype=0), (value=0)");

            //List to save name and index
            Dictionary<string, string> nameIndexAreaProp = new Dictionary<string, string>();

            //List to save thin thick
            Dictionary<string, string> nameThinThickAreaProp = new Dictionary<string, string>();

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

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

                            //Save property name and index
                            nameIndexAreaProp.Add(row["Name"], Convert.ToString(index));

                            index++;
                        }
                        //Ribbed and waffle type not supported in MIDAS
                        else if (row["PropType"] == "Ribbed" || row["PropType"] == "Waffle")
                        {
                            //Show membrane warning if havent shown
                            if (!membraneWarningShown && row["ModelType"] == "Membrane")
                            {
                                string warning = "Membrane type is not supported in MIDAS! Will import as normal (analysis type thin) slab!";
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

                            //Save property name and index
                            nameIndexAreaProp.Add(row["Name"], Convert.ToString(index));

                            //Save shell name and thin or thick
                            if(row["ModelType"] == "Shell-Thin" || row["ModelType"] == "Membrane")
                            {
                                nameThinThickAreaProp.Add(row["Name"], "thin");
                            }
                            else if(row["ModelType"] == "Shell-Thick")
                            {
                                nameThinThickAreaProp.Add(row["Name"], "thick");
                            }
                            
                            index++;
                        }
                        //Normal slab proceed as usual
                        else
                        {
                            //Show membrane warning if havent shown
                            if (!membraneWarningShown && row["ModelType"] == "Membrane")
                            {
                                string warning = "Membrane type is not supported in MIDAS! Will import as normal (analysis type thin) slab!";
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

                            //Save property name and index
                            nameIndexAreaProp.Add(row["Name"], Convert.ToString(index));

                            //Save shell name and thin or thick
                            if (row["ModelType"] == "Shell-Thin" || row["ModelType"] == "Membrane")
                            {
                                nameThinThickAreaProp.Add(row["Name"], "thin");
                            }
                            else if (row["ModelType"] == "Shell-Thick")
                            {
                                nameThinThickAreaProp.Add(row["Name"], "thick");
                            }

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

                        //Save property name and index
                        nameIndexAreaProp.Add(row["Name"], Convert.ToString(index));

                        //Save shell name and thin or thick
                        if (row["ModelType"] == "Shell-Thin" || row["ModelType"] == "Membrane")
                        {
                            nameThinThickAreaProp.Add(row["Name"], "thin");
                        }
                        else if (row["ModelType"] == "Shell-Thick")
                        {
                            nameThinThickAreaProp.Add(row["Name"], "thick");
                        }

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

            //List to save material index
            Dictionary<string, string> nameMatIndexAreaProp = new Dictionary<string, string>();

            //Save stiffness modifiers and material index
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                //Finding the summary table
                if (databaseTable.TableKey == "Area Section Property Definitions - Summary")
                {
                    //Going through each row in the summary table and find the matching name
                    foreach (Dictionary<string, string> row in databaseTable.TableInList)
                    {
                        //Check if section property exist in nameIndexSecProp
                        if (nameIndexAreaProp.ContainsKey(row["Name"]))
                        {
                            string materialIndex = nameIndexMaterial[row["Material"]];
                            nameMatIndexAreaProp.Add(row["Name"], materialIndex);
                        }
                    }
                }
            }
            return (nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp);
        }

        private static Dictionary<string, string> copyLoadPatternToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
        {
            //MGTHeader
            textMgt.Add("*STLDCASE");
            textMgt.Add("; (name), (load type), (desc)");

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Saving load pattern that contains selfweight
            Dictionary<string, double> selfWeightLoadPatterns = new Dictionary<string, double>();

            //To save load pattern name in etabs and in midas
            Dictionary<string, string> loadPatternNameEtabsMidas = new Dictionary<string, string>();

            //Gothrough each table key to get the needed table
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Shell Uniform Load Sets")
                {
                    if (databaseTable.NumberRecords != 0)
                    {
                        string warning = "Shell uniform load sets not support! Will NOT be transferred!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
                else if (databaseTable.TableKey == "Load Pattern Definitions")
                {
                    //Placeholder for each table
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string,string> row in table)
                    {
                        if (row["IsAuto"] == "No")
                        {
                            string name = row["Name"];
                            string typeEtabs = row["Type"];
                            double swMultiplier = Convert.ToDouble(row["SelfWtMult"]);

                            if (swMultiplier != 0)
                            {
                                selfWeightLoadPatterns[name] = swMultiplier;
                            }

                            //Change etabs type to midas type
                            string typeMidas = "";
                            if (typeEtabs == "Dead" || typeEtabs == "Super Dead")
                            {
                                typeMidas = "D";
                            }
                            else if (typeEtabs == "Live" || typeEtabs == "Reducible Live")
                            {
                                typeMidas = "L";
                            }
                            else if (typeEtabs == "Roof Live")
                            {
                                typeMidas = "LR";
                            }
                            else if (typeEtabs == "Wind")
                            {
                                typeMidas = "W";
                            }
                            else if (typeEtabs == "Snow")
                            {
                                typeMidas = "S";
                            }
                            else if (typeEtabs == "Temperature")
                            {
                                typeMidas = "T";
                            }
                            else if (typeEtabs == "Other")
                            {
                                typeMidas = "USER";
                            }
                            else
                            {
                                typeMidas = "USER";
                                name = name + "_CHECK";

                                string warning = $"Load pattern {typeEtabs} not supported in MIDAS. Will import as USER load with _CHECK behind its name!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);
                            }
                            textMgt.Add($"{name}, {typeMidas},");

                            //Saving load pattern name
                            loadPatternNameEtabsMidas.Add(row["Name"], name);
                        }
                    }

                    //For self weight
                    foreach (KeyValuePair<string, double> selfWeightLoadPattern in selfWeightLoadPatterns)
                    {
                        string loadPattern = selfWeightLoadPattern.Key;
                        double swMultiplier = selfWeightLoadPattern.Value;

                        textMgt.Add($"*USE-STLD, {loadPattern}");
                        textMgt.Add($"*SELFWEIGHT");
                        textMgt.Add($"0, 0, {-1 * swMultiplier},");

                    }
                }
            }

            return loadPatternNameEtabsMidas;
        }

        private static (Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>) copyObjectConnectivityToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, 
            List<string> textWarning, Dictionary<string, string> nameIndexSecProp, Dictionary<string, string> nameMatIndexSecProp, Dictionary<string, string> nameIndexAreaProp, 
            Dictionary<string, string> nameMatIndexAreaProp, Dictionary<string, string> nameThinThickAreaProp)
        {
            //List to save node name and index
            Dictionary<string, string> nameIndexNode = new Dictionary<string, string>();

            //List to save frame name and index
            Dictionary<string, string> nameIndexFrame = new Dictionary<string, string>();

            //List to save area name and index
            Dictionary<string, string> nameIndexArea = new Dictionary<string, string>();

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Nodes geometry
            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Point Object Connectivity")
                {
                    //MGTHeader
                    textMgt.Add("*NODE");
                    textMgt.Add("; i, (X), (Y), (Z)");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        double xCoord = Convert.ToDouble(row["X"]);
                        double yCoord = Convert.ToDouble(row["Y"]);
                        double zCoord = Convert.ToDouble(row["Z"]);

                        textMgt.Add($"{index}, {xCoord}, {yCoord}, {zCoord}");

                        //Save node name and index
                        nameIndexNode.Add(row["UniqueName"], Convert.ToString(index));

                        index++;
                    }
                }
            }

            //Getting property assigned to section and area
            Dictionary<string, string> secPropAssigned = new Dictionary<string, string>();
            Dictionary<string, string> areaPropAssigned = new Dictionary<string, string>();

            //Go through frame assignment to get section property for each frame
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Frame Assignments - Section Properties")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        secPropAssigned.Add(row["UniqueName"], row["SectProp"]);
                    }
                }
                else if (databaseTable.TableKey == "Area Assignments - Section Properties")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        areaPropAssigned.Add(row["UniqueName"], row["SectProp"]);
                    }
                }
            }

            //Creating frame and area geometry
            index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                //Frame geometry
                if (databaseTable.TableKey == "Beam Object Connectivity" || databaseTable.TableKey == "Column Object Connectivity" || databaseTable.TableKey == "Brace Object Connectivity")
                {
                    //MGTHeader
                    textMgt.Add("*ELEMENT");
                    textMgt.Add("; i, BEAM, (index material), (index property), (index joint i), (index joint j), (angle=0), (index subtype=0)");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Finding section property index
                        string secProp = secPropAssigned[row["UniqueName"]];
                        string iProp = nameIndexSecProp[secProp];

                        //Finding material index
                        string iMaterial = nameMatIndexSecProp[secProp];

                        //Finding node index
                        string ptI = row["UniquePtI"];
                        string indexPtI = nameIndexNode[ptI];

                        string ptJ = row["UniquePtJ"];
                        string indexPtJ = nameIndexNode[ptJ];

                        //Save node name and index
                        nameIndexFrame.Add(row["UniqueName"], Convert.ToString(index));

                        textMgt.Add($"{index}, BEAM, {iMaterial}, {iProp}, {indexPtI}, {indexPtJ}, 0, 0");

                        index++;
                    }
                }

                //Area geometry

                //Indicator if one way slab warning has been shown
                bool moreThan4NodesWarningShown = false;

                if (databaseTable.TableKey == "Wall Object Connectivity" || databaseTable.TableKey == "Floor Object Connectivity")
                {
                    //Getting area list parsed as Shell type
                    List<src.HelperEtabs.Shell> areaList = src.HelperEtabs.GetShellsPoints(databaseTable);

                    //MGTHeader
                    textMgt.Add("*ELEMENT");
                    textMgt.Add("; i, PLATE, (index material), (index property), (index joint 1), (index joint 2), (index joint 3), (index joint 4), (subtype thick=1 thin=2), (local axis)");

                    foreach (src.HelperEtabs.Shell area in areaList)
                    {
                        //Show warning if there are more than 4 points
                        if (area.Points.Count > 4)
                        {
                            string warning = "";

                            //Show warning once
                            if (!moreThan4NodesWarningShown)
                            {
                                warning = $"Area with > 4 nodes is found! Max node number in MIDAS is 4!";
                                MessageBox.Show(warning);
                                moreThan4NodesWarningShown = true;
                            }

                            warning = $"Area {area.UniqueName} (in ETABS) or {index} (in MIDAS) has {area.Points.Count} nodes! Max node number in MIDAS is 4!";
                            //MessageBox.Show(warning);
                            textWarning.Add(warning);
                        }

                        //Finding section property index
                        string areaProp = areaPropAssigned[area.UniqueName];
                        string iProp = nameIndexAreaProp[areaProp];

                        //Finding material index
                        string iMaterial = nameMatIndexAreaProp[areaProp];

                        //Finding thin or thick
                        string thinThick = nameThinThickAreaProp[areaProp];
                        string indexThinThick = "";
                        if (thinThick == "thick")
                        {
                            indexThinThick = "1";
                        }
                        else if (thinThick == "thin")
                        {
                            indexThinThick = "2";
                        }

                        //Finding node index
                        string pt1 = area.Points[0];
                        string indexPt1 = nameIndexNode[pt1];

                        string pt2 = area.Points[1];
                        string indexPt2 = nameIndexNode[pt2];

                        string pt3 = area.Points[2];
                        string indexPt3 = nameIndexNode[pt3];

                        string indexPt4 = "0";
                        if (area.Points.Count > 3)
                        {
                            string pt4 = area.Points[3];
                            indexPt4 = nameIndexNode[pt4];
                        };

                        //Save node name and index
                        nameIndexArea.Add(area.UniqueName, Convert.ToString(index));

                        textMgt.Add($"{index}, PLATE, {iMaterial}, {iProp}, {indexPt1}, {indexPt2}, {indexPt3}, {indexPt4}, {indexThinThick}, 0");

                        index++;
                    }
                }
            }

            return (nameIndexNode, nameIndexFrame, nameIndexArea);
        }

        private static void copyFrameAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexFrame)
        {
            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Frame Assignments - Releases and Partial Fixity" && databaseTable.NumberRecords != 0)
                {
                    //Frame releases
                    //MGTHeader
                    textMgt.Add("*FRAME-RLS");
                    textMgt.Add("; (index element), (insert value=yes), (release condition-i 000000), (spring Fxi), (spring Fyi), (spring Fzi), (spring Mxi), (spring Myi), (spring Mzi) - 1ST LINE");
                    textMgt.Add(";                                      (release condition-j 000000), (spring Fxj), (spring Fyj), (spring Fzj), (spring Mxj), (spring Myj), (spring Mzj) - 2ND LINE");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string indexFrame = nameIndexFrame[row["UniqueName"]];

                        int releaseFxI = parseYesNoToBinary(row["PI"]);
                        int releaseFyI = parseYesNoToBinary(row["V2I"]);
                        int releaseFzI = parseYesNoToBinary(row["V3I"]);
                        int releaseMxI = parseYesNoToBinary(row["TI"]);
                        int releaseMyI = parseYesNoToBinary(row["M3I"]);
                        int releaseMzI = parseYesNoToBinary(row["M2I"]);

                        double springFxI = 0;
                        double springFyI = 0;
                        double springFzI = 0;
                        double springMxI = 0;
                        double springMyI = 0;
                        double springMzI = 0;
                        if (row.ContainsKey("PISpring"))
                        {
                            if (row["PISpring"] != "")
                            {
                                springFxI = Convert.ToDouble(row["PISpring"]);
                            }
                            if (row["V2ISpring"] != "")
                            {
                                springFyI = Convert.ToDouble(row["V2ISpring"]);
                            }
                            if (row["V3ISpring"] != "")
                            {
                                springFzI = Convert.ToDouble(row["V3ISpring"]);
                            }
                            if (row["TISpring"] != "")
                            {
                                springMxI = Convert.ToDouble(row["TISpring"]);
                            }
                            if (row["M3ISpring"] != "")
                            {
                                springMyI = Convert.ToDouble(row["M3ISpring"]);
                            }
                            if (row["M2ISpring"] != "")
                            {
                                springMyI = Convert.ToDouble(row["M2ISpring"]);
                            }
                        };

                        string releaseBinaryI = releaseFxI.ToString() + releaseFyI.ToString() + releaseFzI.ToString() +
                            releaseMxI.ToString() + releaseMyI.ToString() + releaseMzI.ToString();

                        int releaseFxJ = parseYesNoToBinary(row["PJ"]);
                        int releaseFyJ = parseYesNoToBinary(row["V2J"]);
                        int releaseFzJ = parseYesNoToBinary(row["V3J"]);
                        int releaseMxJ = parseYesNoToBinary(row["TJ"]);
                        int releaseMyJ = parseYesNoToBinary(row["M3J"]);
                        int releaseMzJ = parseYesNoToBinary(row["M2J"]);

                        double springFxJ = 0;
                        double springFyJ = 0;
                        double springFzJ = 0;
                        double springMxJ = 0;
                        double springMyJ = 0;
                        double springMzJ = 0;
                        if (row.ContainsKey("PJSpring"))
                        {
                            if (row["PJSpring"] != "")
                            {
                                springFxJ = Convert.ToDouble(row["PJSpring"]);
                            }
                            if (row["V2JSpring"] != "")
                            {
                                springFyJ = Convert.ToDouble(row["V2JSpring"]);
                            }
                            if (row["V3JSpring"] != "")
                            {
                                springFzJ = Convert.ToDouble(row["V3JSpring"]);
                            }
                            if (row["TJSpring"] != "")
                            {
                                springMxJ = Convert.ToDouble(row["TJSpring"]);
                            }
                            if (row["M3JSpring"] != "")
                            {
                                springMyJ = Convert.ToDouble(row["M3JSpring"]);
                            }
                            if (row["M2JSpring"] != "")
                            {
                                springMyJ = Convert.ToDouble(row["M2JSpring"]);
                            }
                        }

                        string releaseBinaryJ = releaseFxJ.ToString() + releaseFyJ.ToString() + releaseFzJ.ToString() +
                            releaseMxJ.ToString() + releaseMyJ.ToString() + releaseMzJ.ToString();

                        textMgt.Add($"{indexFrame}, YES, {releaseBinaryI}, {springFxI}, {springFyI}, {springFzI}, {springMxI}, {springMyI}, {springMzI}");
                        textMgt.Add($"{releaseBinaryJ}, {springFxJ}, {springFyJ}, {springFzJ}, {springMxJ}, {springMyJ}, {springMzJ}");
                    }
                }
                else if (databaseTable.TableKey == "Frame Assignments - Property Modifiers" && databaseTable.NumberRecords != 0)
                {
                    //Frame property modifiers
                    //MGTHeader
                    textMgt.Add("*ELEM-SRF    ; Element Stiffness Scale Factor");
                    textMgt.Add("; i, (area modifier), (shear minor modifier), (shear major modifier), (torsion modifier), (moment major modifier), (moment minor modifier), (weight modifier), (group=''), (iPart=1)");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Access using key=name to access value=index
                        string indexFrame = nameIndexFrame[row["UniqueName"]];

                        double areaMod = Convert.ToDouble(row["AMod"]);
                        double aYMod = Convert.ToDouble(row["A2Mod"]);
                        double aZMod = Convert.ToDouble(row["A3Mod"]);
                        double jMod = Convert.ToDouble(row["JMod"]);
                        double iYMod = Convert.ToDouble(row["I3Mod"]);
                        double iZMod = Convert.ToDouble(row["I2Mod"]);
                        double weightMod = 0;

                        if (Convert.ToDouble(row["MMod"]) != Convert.ToDouble(row["WMod"]))
                        {
                            string warning = $"Different weight and mass modifier found in element {row["UniqueName"]} (in ETABS). Highest value will be used!";
                            //MessageBox.Show(warning);
                            textWarning.Add(warning);

                            if (Convert.ToDouble(row["MMod"]) > Convert.ToDouble(row["WMod"]))
                            {
                                weightMod = Convert.ToDouble(row["MMod"]);
                            }
                            else
                            {
                                weightMod = Convert.ToDouble(row["WMod"]);
                            }
                        }
                        else
                        {
                            weightMod = Convert.ToDouble(row["WMod"]);
                        }

                        if (areaMod == 1 && aYMod == 1 && aZMod == 1 && jMod == 1 && iYMod == 1 && iZMod == 1 && weightMod == 1)
                        {
                            //if no modifier then do nothing
                        }
                        else
                        {
                            textMgt.Add($"{indexFrame}, {areaMod}, {aYMod}, {aZMod}, {jMod}, {iYMod}, {iZMod}, {weightMod}, ,1");
                        }
                    }
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will NOT be imported!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
            }
        }

        private static void copyFrameLoadsAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexFrame, 
            Dictionary<string, string>  loadPatternNameEtabsMidas)
        {
            
            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Distributed load UDL
            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Frame Loads Assignments - Distributed")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];

                        string indexFrame = nameIndexFrame[row["UniqueName"]];

                        string directionEtabs = row["Dir"];
                        (string directionMidas, string projected, double loadFactor) = parseDireactionEtabsToMidas(directionEtabs);

                        string loadTypeEtabs = row["LoadType"];
                        string loadTypeMidas = "";
                        double force1 = 0;
                        double force2 = 0;
                        if (loadTypeEtabs == "Force")
                        {
                            loadTypeMidas = "UNILOAD";
                            force1 = Convert.ToDouble(row["ForceA"]) * loadFactor;
                            force2 = Convert.ToDouble(row["ForceB"]) * loadFactor;
                        }
                        else if(loadTypeEtabs == "Moment")
                        {
                            loadTypeMidas = "UNIMOMENT";
                            force1 = Convert.ToDouble(row["MomentA"]) * loadFactor;
                            force2 = Convert.ToDouble(row["MomentB"]) * loadFactor;
                        }

                        double location1 = Convert.ToDouble(row["RelDistA"]);
                        double location2 = Convert.ToDouble(row["RelDistB"]);

                        //MGTHeader
                        textMgt.Add($"*USE-STLD, {loadPattern}");                      
                        textMgt.Add("*BEAMLOAD");
                        textMgt.Add("; (index element), (load classificiation=BEAM), (loadtype=UNILOAD), (direction), (projected), (bEccen=NO), (eccenDir=aDir[1]), (i-end=''), (j-end=''), (bj-end=''), " +
                            "(location relative 1), (force1), (location relative 2), (force2), (location relative 3=0), (force3=0), (location relative 4=0), (force4=0)");
                        textMgt.Add($"{indexFrame}, BEAM, {loadTypeMidas}, {directionMidas}, {projected}, NO, aDir[1], , , , {location1}, {force1}, {location2}, {force2}, 0, 0, 0, 0");
                    }
                }
                else if (databaseTable.TableKey == "Frame Loads Assignments - Point")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];

                        string indexFrame = nameIndexFrame[row["UniqueName"]];

                        string directionEtabs = row["Dir"];
                        (string directionMidas, string projected, double loadFactor) = parseDireactionEtabsToMidas(directionEtabs);

                        string loadTypeEtabs = row["LoadType"];
                        string loadTypeMidas = "";
                        double force = 0;
                        if (loadTypeEtabs == "Force")
                        {
                            loadTypeMidas = "CONLOAD";
                            force = Convert.ToDouble(row["Force"]) * loadFactor;
                        }
                        else if (loadTypeEtabs == "Moment")
                        {
                            loadTypeMidas = "CONMOMENT";
                            force = Convert.ToDouble(row["Moment"]) * loadFactor;
                        }

                        double location = Convert.ToDouble(row["RelDist"]);

                        //MGTHeader
                        textMgt.Add($"*USE-STLD, {loadPattern}");
                        textMgt.Add("*BEAMLOAD");
                        textMgt.Add("; (index element), (load classificiation=BEAM), (loadtype=UNILOAD), (direction), (projected), (bEccen=NO), (eccenDir=aDir[1]), (i-end=''), (j-end=''), (bj-end=''), " +
                            "(location relative 1), (force1), (location relative 2=0), (force2=0), (location relative 3=0), (force3=0), (location relative 4=0), (force4=0)");
                        textMgt.Add($"{indexFrame}, BEAM, {loadTypeMidas}, {directionMidas}, {projected}, NO, aDir[1], , , , {location}, {force}, 0, 0, 0, 0, 0, 0");
                    }
                }
                else if (databaseTable.TableKey == "Frame Loads Assignments - Temperature")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];
                        string indexFrame = nameIndexFrame[row["UniqueName"]];
                        double temperature = Convert.ToDouble(row["Temp"]);

                        //MGTHeader
                        textMgt.Add($"*USE-STLD, {loadPattern}");
                        textMgt.Add("*ELTEMPER");
                        textMgt.Add("; (index element), (temperature), (group)");
                        textMgt.Add($"{indexFrame}, {temperature}, ");
                    }
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will NOT be imported!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
            }
        }

        private static void copyJointAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexNode)
        {
            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Joint Assignments - Restraints")
                {
                    //Joint restraint
                    //MGTHeader
                    textMgt.Add("*CONSTRAINT ; Restraints");
                    textMgt.Add("; (index node), (resrtaint condition-i 000000 Dx,Dy,Dz,Rx,Ry,Rz), (groups='')");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string indexNode = nameIndexNode[row["UniqueName"]];

                        int dX = parseYesNoToBinary(row["UX"]);
                        int dY = parseYesNoToBinary(row["UY"]);
                        int dZ = parseYesNoToBinary(row["UZ"]);
                        int rX = parseYesNoToBinary(row["RX"]);
                        int rY = parseYesNoToBinary(row["RY"]);
                        int rZ = parseYesNoToBinary(row["RZ"]);

                        string supportBinary = dX.ToString() + dY.ToString() + dZ.ToString() + rX.ToString() + rY.ToString() + rZ.ToString();

                        textMgt.Add($"{indexNode}, {supportBinary}, ");
                    }
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will NOT be imported!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
            }
        }

        private static void copyJointLoadsAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexNode,
                                                            Dictionary<string, string> loadPatternNameEtabsMidas)
        {

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Point load
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Joint Loads Assignments - Force")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];

                        string indexNode = nameIndexNode[row["UniqueName"]];

                        double fX = Convert.ToDouble(row["FX"]);
                        double fY = Convert.ToDouble(row["FY"]);
                        double fZ = Convert.ToDouble(row["FZ"]);
                        double mX = Convert.ToDouble(row["MX"]);
                        double mY = Convert.ToDouble(row["MY"]);
                        double mZ = Convert.ToDouble(row["MZ"]);

                        //MGTHeader
                        textMgt.Add($"*USE-STLD, {loadPattern}");
                        textMgt.Add("*CONLOAD");
                        textMgt.Add("; (index node), (FX), (FY), (FZ), (MX), (MY), (MZ), (group='') ");
                        textMgt.Add($"{indexNode}, {fX}, {fY}, {fZ}, {mX}, {mY}, {mZ}, ");
                    }
                }
                else if (databaseTable.TableKey == "Joint Loads Assignments - Temperature")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];
                        string indexNode = nameIndexNode[row["UniqueName"]];
                        double temperature = Convert.ToDouble(row["T"]);

                        //MGTHeader
                        textMgt.Add($"*USE-STLD, {loadPattern}");
                        textMgt.Add("*ELTEMPER");
                        textMgt.Add("; (index node), (temperature), (group)");
                        textMgt.Add($"{indexNode}, {temperature}, ");
                    }
                }
                else
                {
                    if (databaseTable.NumberRecords > 0)
                    {
                        //Show warning that the type cant be converted to midas
                        string warning = $"{databaseTable.TableKey} not supported! Will NOT be imported!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                }
            }
        }

        #region Other methods

        private static (string, string, double) parseDireactionEtabsToMidas (string directionEtabs)
        {
            string directionMidas = "";
            string projected = "";
            double loadFactor = 1;

            //Parsing direction Etabs to direction Midas
            if (directionEtabs == "Local-1")
            {
                directionMidas = "Lx";
                projected = "NO";
            }
            else if (directionEtabs == "Local-2")
            {
                directionMidas = "Lz";
                projected = "NO";
            }
            else if (directionEtabs == "Local-3")
            {
                directionMidas = "Ly";
                projected = "NO";
            }
            else if (directionEtabs == "Global-X")
            {
                directionMidas = "Gx";
                projected = "NO";
            }
            else if (directionEtabs == "Global-Y")
            {
                directionMidas = "Gy";
                projected = "NO";
            }
            else if (directionEtabs == "Gravity")
            {
                directionMidas = "Gz";
                projected = "NO";
                loadFactor = -1;
            }
            else if (directionEtabs == "Global-X Proj")
            {
                directionMidas = "Gx";
                projected = "YES";
            }
            else if (directionEtabs == "Global-Y Proj")
            {
                directionMidas = "Gy";
                projected = "YES";
            }
            else if (directionEtabs == "Gravity Proj")
            {
                directionMidas = "Gz";
                projected = "YES";
                loadFactor = -1;
            }
            return (directionMidas, projected, loadFactor);
        }

        private static int parseYesNoToBinary(string yesNo)
        {
            if (yesNo == "Yes")
            {
                return 1;
            }
            else if (yesNo == "No")
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        #endregion


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
