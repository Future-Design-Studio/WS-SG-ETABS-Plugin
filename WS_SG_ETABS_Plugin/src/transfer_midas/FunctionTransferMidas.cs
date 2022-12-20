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
                                                    "Load Case Definitions - Summary",
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
                {"links", new List<string>() {"Link Assignments - Link Properties",
                                                "Link Assignments - Local Axes",
                                                //"Link Assignments - Summary",
                                                "Link Object Connectivity",
                                                //"Link Property Definitions - Acceptance Criteria - Displacement",
                                                //"Link Property Definitions - Acceptance Criteria - Force",
                                                //"Link Property Definitions - Damper - Bilinear",
                                                //"Link Property Definitions - Damper - Exponential",
                                                //"Link Property Definitions - Damper - Friction Spring",
                                                //"Link Property Definitions - Gap",
                                                //"Link Property Definitions - Hook",
                                                //"Link Property Definitions - Isolator - Friction",
                                                //"Link Property Definitions - Isolator - High Damping Rubber",
                                                //"Link Property Definitions - Isolator - Rubber",
                                                //"Link Property Definitions - Isolator - TC Friction",
                                                //"Link Property Definitions - Isolator - Triple Pendulum",
                                                "Link Property Definitions - Linear",
                                                //"Link Property Definitions - Multilinear - Elastic",
                                                //"Link Property Definitions - Multilinear - Plastic",
                                                //"Link Property Definitions - Plastic Wen",
                                                //"Link Property Definitions - Summary",
                                                } },
                {"groups", new List<string>() {"Group Assignments"
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
            selectedDatabaseTables.Add("links", true);
            selectedDatabaseTables.Add("groups", true);

            //To save material index
            Dictionary<string, string> nameIndexMaterial = new Dictionary<string, string>();

            //To save section properties material index
            Dictionary<string, string> nameIndexSecProp = new Dictionary<string, string>();
            Dictionary<string, string> nameMatIndexSecProp = new Dictionary<string, string>();

            //To save area properties material index
            Dictionary<string, string> nameIndexAreaProp = new Dictionary<string, string>();
            Dictionary<string, string> nameMatIndexAreaProp = new Dictionary<string, string>();
            Dictionary<string, string> nameThinThickAreaProp = new Dictionary<string, string>();

            //To save area property assigned to each area
            Dictionary<string, string> areaPropAssigned = new Dictionary<string, string>();

            //To save area section modifier from area properties
            List<Dictionary<string, string>> nameModifierAreaProp = new List<Dictionary<string, string>>();

            //To load pattern
            Dictionary<string, string> loadPatternNameEtabsMidas = new Dictionary<string, string>();

            //To save local axis
            Dictionary<string, string> nameLocalAxisFrame = new Dictionary<string, string>();
            Dictionary<string, string> nameLocalAxisArea = new Dictionary<string, string>();

            //To save node, frame and area index
            Dictionary<string, string> nameIndexNode = new Dictionary<string, string>();
            Dictionary<string, string> nameIndexFrame = new Dictionary<string, string>();
            Dictionary<string, string> nameIndexArea = new Dictionary<string, string>();
            List<string> skippedAreas = new List<string>();

            //To save shell uniform load sets
            Dictionary<string, Dictionary<string, string>> uniformLoadSets = new Dictionary<string, Dictionary<string, string>>();

            //To save load case name
            Dictionary<string, string> loadCaseNameEtabsMidas = new Dictionary<string, string>();

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
                                (nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp, nameModifierAreaProp) = copyAreaSecToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexMaterial);
                            }

                            if (databaseTableKey.Key == "loadPattern")
                            {
                                (loadPatternNameEtabsMidas, uniformLoadSets) = copyLoadPatternToMidas(Sap, databaseTableKey.Value, textMgt, textWarning);
                            }

                            if (databaseTableKey.Key == "objectConnectivity")
                            {
                                nameLocalAxisFrame = copyFrameAssignmentsLocalAxis(Sap);
                                nameLocalAxisArea = copyAreaAssignmentsLocalAxis(Sap);
                                (nameIndexNode, nameIndexFrame, nameIndexArea, skippedAreas, areaPropAssigned) = copyObjectConnectivityToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexSecProp, 
                                    nameMatIndexSecProp, nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp, nameLocalAxisFrame, nameLocalAxisArea);
                            }

                            if (databaseTableKey.Key == "frameAssignments")
                            {
                                copyFrameAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexFrame);
                            }

                            if (databaseTableKey.Key == "jointAssignments")
                            {
                                copyJointAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode);
                            }
                            
                            if (databaseTableKey.Key == "areaAssignments")
                            {
                                copyAreaAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexArea, nameModifierAreaProp, areaPropAssigned);
                            }
                            
                            if (databaseTableKey.Key == "frameLoadsAssignments")
                            {
                                copyFrameLoadsAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexFrame, loadPatternNameEtabsMidas);
                            }

                            if (databaseTableKey.Key == "jointLoadsAssignments")
                            {
                                copyJointLoadsAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode, loadPatternNameEtabsMidas);
                            }

                            if (databaseTableKey.Key == "areaLoadAssignments")
                            {
                                copyAreaLoadAssignmentsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexArea, loadPatternNameEtabsMidas, skippedAreas, uniformLoadSets);
                            }

                            if (databaseTableKey.Key == "links")
                            {
                                copyLinksToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode);
                            }

                            if (databaseTableKey.Key == "loadCase")
                            {
                                loadCaseNameEtabsMidas = copyLoadCaseToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, loadPatternNameEtabsMidas);
                            }

                            if (databaseTableKey.Key == "loadCombination")
                            {
                                copyLoadCombinationToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, loadCaseNameEtabsMidas);
                            }

                            if (databaseTableKey.Key == "groups")
                            {
                                copyGroupsToMidas(Sap, databaseTableKey.Value, textMgt, textWarning, nameIndexNode, nameIndexFrame, nameIndexArea, skippedAreas);
                            }

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

            //CreateExcelComparingEtabsMidas(Sap, nameIndexNode, nameIndexFrame, nameIndexArea);
            CreateCsvComparingEtabsMidas(Sap, nameIndexNode, nameIndexFrame, nameIndexArea);

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
                //For index = 1, create null material for null frame and null area
                if (index == 1)
                {
                    string materialName = "Null material";

                    double unitWeight = 0;
                    double unitMass = 0;
                    double elasticModulus = 1;
                    double poissonRatio = 0;
                    double thermalCoefficient = 0;

                    textMgt.Add($"{index}, CONC, {materialName}, 0, 0, ,C, NO, 0.05, 2, {elasticModulus}, {poissonRatio}, {thermalCoefficient}, {unitWeight}, {unitMass}");

                    //Save material name and index
                    Dictionary<string, string> nameIndexMaterialSingle = new Dictionary<string, string>() { };
                    nameIndexMaterial.Add(materialName, Convert.ToString(index));

                    index++;
                }

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
                    string warning = $"Have material type other than concrete and steel. {rowTableGeneral["Material"]} with type {rowTableGeneral["Type"]} " +
                        $"will be imported as concrete with _CHECK behind its name!";
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
                //For first loop, will add in NULL FRAME
                if (index == 1)
                {
                    string name = "Null frame";

                    string sectionShape = "SR";
                    double diameter = Convert.ToDouble(0.01);

                    textMgt.Add($"{index}, DBUSER, {name}, CC, 0, 0, 0, 0, 0, 0, YES, NO, {sectionShape}, 2, {diameter}, 0, 0, 0, 0, 0, 0, 0, 0, 0");

                    //Save property name and index
                    nameIndexSecProp.Add(name, Convert.ToString(index));

                    index++;
                }

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

            //Save material index for NULL FRAME
            string materialIndex = nameIndexMaterial["Null material"];
            nameMatIndexSecProp.Add("Null frame", materialIndex);

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
                                materialIndex = nameIndexMaterial[row["Material"]];
                                nameMatIndexSecProp.Add(row["Name"], materialIndex);
                            }
                        }
                    }
                }
            }

            return (nameIndexSecProp, nameMatIndexSecProp);
        }

        //Method to copy area section to MIDAS
        private static (Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, List<Dictionary<string, string>>) copyAreaSecToMidas(cSapModel Sap, List<string> tableKeys, 
            List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexMaterial)
        {
            //MGTHeader
            textMgt.Add("*THICKNESS");
            textMgt.Add("; i, (section type=VALUE), (name), (same thickness all around=YES), (thickness in plane), (thickness out of plane=0), (offset=NO), (offtype=0), (value=0)");

            //List to save name and index
            Dictionary<string, string> nameIndexAreaProp = new Dictionary<string, string>();

            //List to save thin thick
            Dictionary<string, string> nameThinThickAreaProp = new Dictionary<string, string>();

            //List to save modifier in area prop
            List<Dictionary<string, string>> nameModifierAreaProp = new List<Dictionary<string, string>>();

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Gothrough each table key to get the needed table
            int index = 1;
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                //For index = 1 create null area
                if (index == 1)
                {
                    string name = "Null area";
                    double thickness = 0.01;

                    textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                    //Save property name and index
                    nameIndexAreaProp.Add(name, Convert.ToString(index));

                    //Save shell name and thin or thick
                    nameThinThickAreaProp.Add(name, "thin");

                    index++;
                }
                
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
                        string name = row["Name"];

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
                            //string name = row["Name"];
                            if (name.Length > 28 - 7)
                            {
                                name = name.Substring(0, 28 - 7);
                            }

                            name = name + "_CHECK!";
                            double thickness = 0.1;

                            textMgt.Add($"{index}, VALUE, {name}, YES, {thickness}, 0, NO, 0, 0");

                            //Save property name and index
                            nameIndexAreaProp.Add(row["Name"], Convert.ToString(index));

                            //Save shell name and thin or thick
                            nameThinThickAreaProp.Add(row["Name"], "thin");

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
                            //string name = row["Name"];
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
                            if(row["ModelType"] == "Shell-Thin" || row["ModelType"] == "Membrane") //Membrane also save as thin
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
                            //string name = row["Name"];
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

                        //Save shell modifier in area prop
                        Dictionary<string, string> nameModifierAreaPropSingle = new Dictionary<string, string>();

                        double fxxMod = Convert.ToDouble(row["f11Mod"]);
                        double fyyMod = Convert.ToDouble(row["f22Mod"]);
                        double fxyMod = Convert.ToDouble(row["f12Mod"]);
                        double mxxMod = Convert.ToDouble(row["m22Mod"]);
                        double myyMod = Convert.ToDouble(row["m11Mod"]);
                        double mxyMod = Convert.ToDouble(row["m12Mod"]);
                        double vxxMod = Convert.ToDouble(row["v13Mod"]);
                        double vyyMod = Convert.ToDouble(row["v23Mod"]);
                        double weightMod = 0;

                        if (Convert.ToDouble(row["MMod"]) != Convert.ToDouble(row["WMod"]))
                        {
                            string warning = $"Different weight and mass modifier found in area property {row["Name"]}. Highest value will be used!";
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

                        if (fxxMod == 1 && fyyMod == 1 && fxyMod == 1 && mxxMod == 1 && myyMod == 1 && mxyMod == 1 && vxxMod == 1 && vyyMod == 1 && weightMod == 1)
                        {
                            //if no modifier then do nothing
                        }
                        else
                        {
                            nameModifierAreaPropSingle.Add("nameAreaProp", name.ToString());
                            nameModifierAreaPropSingle.Add("fxx", fxxMod.ToString());
                            nameModifierAreaPropSingle.Add("fyy", fyyMod.ToString());
                            nameModifierAreaPropSingle.Add("fxy", fxyMod.ToString());
                            nameModifierAreaPropSingle.Add("mxx", mxxMod.ToString());
                            nameModifierAreaPropSingle.Add("myy", myyMod.ToString());
                            nameModifierAreaPropSingle.Add("mxy", mxyMod.ToString());
                            nameModifierAreaPropSingle.Add("vxx", vxxMod.ToString());
                            nameModifierAreaPropSingle.Add("vyy", vyyMod.ToString());
                            nameModifierAreaPropSingle.Add("weightMod", weightMod.ToString());

                            nameModifierAreaProp.Add(nameModifierAreaPropSingle);
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


                        //Save shell modifier
                        Dictionary<string, string> nameModifierAreaPropSingle = new Dictionary<string, string>();

                        double fxxMod = Convert.ToDouble(row["f11Mod"]);
                        double fyyMod = Convert.ToDouble(row["f22Mod"]);
                        double fxyMod = Convert.ToDouble(row["f12Mod"]);
                        double mxxMod = Convert.ToDouble(row["m22Mod"]);
                        double myyMod = Convert.ToDouble(row["m11Mod"]);
                        double mxyMod = Convert.ToDouble(row["m12Mod"]);
                        double vxxMod = Convert.ToDouble(row["v13Mod"]);
                        double vyyMod = Convert.ToDouble(row["v23Mod"]);
                        double weightMod = 0;

                        if (Convert.ToDouble(row["MMod"]) != Convert.ToDouble(row["WMod"]))
                        {
                            string warning = $"Different weight and mass modifier found in area property {row["Name"]}. Highest value will be used!";
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

                        if (fxxMod == 1 && fyyMod == 1 && fxyMod == 1 && mxxMod == 1 && myyMod == 1 && mxyMod == 1 && vxxMod == 1 && vyyMod == 1 && weightMod == 1)
                        {
                            //if no modifier then do nothing
                        }
                        else
                        {
                            nameModifierAreaPropSingle.Add("nameAreaProp", name.ToString());
                            nameModifierAreaPropSingle.Add("fxx", fxxMod.ToString());
                            nameModifierAreaPropSingle.Add("fyy", fyyMod.ToString());
                            nameModifierAreaPropSingle.Add("fxy", fxyMod.ToString());
                            nameModifierAreaPropSingle.Add("mxx", mxxMod.ToString());
                            nameModifierAreaPropSingle.Add("myy", myyMod.ToString());
                            nameModifierAreaPropSingle.Add("mxy", mxyMod.ToString());
                            nameModifierAreaPropSingle.Add("vxx", vxxMod.ToString());
                            nameModifierAreaPropSingle.Add("vyy", vyyMod.ToString());
                            nameModifierAreaPropSingle.Add("weightMod", weightMod.ToString());

                            nameModifierAreaProp.Add(nameModifierAreaPropSingle);
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

            //Save material index for NULL AREA
            string materialIndex = nameIndexMaterial["Null material"];
            nameMatIndexAreaProp.Add("Null area", materialIndex);

            //Save area material index
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                //Finding the summary table
                if (databaseTable.TableKey == "Area Section Property Definitions - Summary")
                {
                    //Going through each row in the summary table and find the matching name
                    foreach (Dictionary<string, string> row in databaseTable.TableInList)
                    {
                        //Check if section property exist in nameIndexAreaProp
                        if (nameIndexAreaProp.ContainsKey(row["Name"]))
                        {
                            materialIndex = nameIndexMaterial[row["Material"]];
                            nameMatIndexAreaProp.Add(row["Name"], materialIndex);
                        }
                    }
                }
            }
            return (nameIndexAreaProp, nameMatIndexAreaProp, nameThinThickAreaProp, nameModifierAreaProp);
        }

        private static (Dictionary<string, string>, Dictionary<string, Dictionary<string, string>>) copyLoadPatternToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning)
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

            //To save shell uniform load sets
            Dictionary<string, Dictionary<string, string>> uniformLoadSets = new Dictionary<string, Dictionary<string, string>>();

            //Gothrough each table key to get the needed table
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Shell Uniform Load Sets")
                {
                    /*
                    if (databaseTable.NumberRecords != 0)
                    {
                        string warning = "Shell uniform load sets not support! Will NOT be transferred!";
                        MessageBox.Show(warning);
                        textWarning.Add(warning);
                    }
                    */

                    foreach (Dictionary<string, string> row in databaseTable.TableInList)
                    {
                        string uniformLoadSetName = row["Name"];
                        Dictionary<string, string> uniformLoadSetLoadPattern = new Dictionary<string, string>();

                        //if the load set havent been recorded previously
                        if (!uniformLoadSets.ContainsKey(uniformLoadSetName))
                        {
                            //Go through each row 1 by 1 to find the load set
                            foreach (Dictionary<string, string> row_inside in databaseTable.TableInList)
                            {
                                if (uniformLoadSetName == row_inside["Name"])
                                {
                                    uniformLoadSetLoadPattern.Add(row_inside["LoadPattern"], row_inside["LoadValue"]);
                                }

                            }
                            uniformLoadSets.Add(uniformLoadSetName, uniformLoadSetLoadPattern);
                        }
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

                                string warning = $"Load pattern {name} with type {typeEtabs} not supported in MIDAS. Will import as USER load with _CHECK behind its name!";
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

            return (loadPatternNameEtabsMidas, uniformLoadSets);
        }

        private static Dictionary<string, string> copyFrameAssignmentsLocalAxis(cSapModel Sap)
        {
            Dictionary<string, string>  nameLocalAxisFrame = new Dictionary<string, string>();

            bool showWarning = false;
            bool isError = false;

            //Display table
            src.HelperEtabs.DatabaseTable databaseTable = null;
            (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(Sap, "Frame Assignments - Local Axes", showWarning);

            if (databaseTable != null)
            {
                foreach (Dictionary<string, string> row in databaseTable.TableInList)
                {
                    nameLocalAxisFrame.Add(row["UniqueName"], row["Angle"]);
                }
            }

            return nameLocalAxisFrame;
        }

        private static Dictionary<string, string> copyAreaAssignmentsLocalAxis(cSapModel Sap)
        {
            Dictionary<string, string> nameLocalAxisArea = new Dictionary<string, string>();

            bool showWarning = false;
            bool isError = false;

            //Display table
            src.HelperEtabs.DatabaseTable databaseTable = null;
            (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(Sap, "Area Assignments - Local Axes", showWarning);

            if (databaseTable != null)
            {
                foreach (Dictionary<string, string> row in databaseTable.TableInList)
                {
                    nameLocalAxisArea.Add(row["UniqueName"], row["Angle"]);
                }
            }

            return nameLocalAxisArea;
        }

        private static (Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, List<string>, Dictionary<string, string>) copyObjectConnectivityToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, 
            List<string> textWarning, Dictionary<string, string> nameIndexSecProp, Dictionary<string, string> nameMatIndexSecProp, Dictionary<string, string> nameIndexAreaProp, 
            Dictionary<string, string> nameMatIndexAreaProp, Dictionary<string, string> nameThinThickAreaProp, Dictionary<string, string> nameLocalAxisFrame, Dictionary<string, string>  nameLocalAxisArea)
        {
            //List to save node name and index
            Dictionary<string, string> nameIndexNode = new Dictionary<string, string>();

            //List to save frame name and index
            Dictionary<string, string> nameIndexFrame = new Dictionary<string, string>();

            //List to save area name and index
            Dictionary<string, string> nameIndexArea = new Dictionary<string, string>();

            //Saving area unique name that have more than 4 nodes to be skipped
            List<string> skippedAreas = new List<string>();

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
            Dictionary<string, string> nameSecPropAssigned = new Dictionary<string, string>();
            Dictionary<string, string> nameAreaPropAssigned = new Dictionary<string, string>();

            //Go through frame assignment to get section property for each frame
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Frame Assignments - Section Properties")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        nameSecPropAssigned.Add(row["UniqueName"], row["SectProp"]);
                    }
                }
                else if (databaseTable.TableKey == "Area Assignments - Section Properties")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        nameAreaPropAssigned.Add(row["UniqueName"], row["SectProp"]);
                    }
                }
            }

            //Creating frame geometry
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
                        string secProp = nameSecPropAssigned[row["UniqueName"]];
                        string iProp = nameIndexSecProp[secProp];

                        //Finding material index
                        string iMaterial = nameMatIndexSecProp[secProp];

                        //Finding node index
                        string ptI = row["UniquePtI"];
                        string indexPtI = nameIndexNode[ptI];

                        string ptJ = row["UniquePtJ"];
                        string indexPtJ = nameIndexNode[ptJ];

                        //Finding local axis angle
                        string localAxis = "0";
                        if (nameLocalAxisFrame.ContainsKey(row["UniqueName"]))
                        {
                            localAxis = nameLocalAxisFrame[row["UniqueName"]];
                        }

                        //Save node name and index
                        nameIndexFrame.Add(row["UniqueName"], Convert.ToString(index));

                        textMgt.Add($"{index}, BEAM, {iMaterial}, {iProp}, {indexPtI}, {indexPtJ}, {localAxis}, 0");

                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Null Line Object Connectivity")
                {
                    //MGTHeader
                    textMgt.Add("*ELEMENT");
                    textMgt.Add("; i, BEAM, (index material), (index property), (index joint i), (index joint j), (angle=0), (index subtype=0)");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Finding section property index
                        string secProp = "Null frame";
                        string iProp = nameIndexSecProp[secProp];

                        //Finding material index
                        string iMaterial = nameMatIndexSecProp[secProp];

                        //Finding node index
                        string ptI = row["UniquePtI"];
                        string indexPtI = nameIndexNode[ptI];

                        string ptJ = row["UniquePtJ"];
                        string indexPtJ = nameIndexNode[ptJ];

                        //Finding local axis angle
                        string localAxis = "0";
                        if (nameLocalAxisFrame.ContainsKey(row["UniqueName"]))
                        {
                            localAxis = nameLocalAxisFrame[row["UniqueName"]];
                        }

                        //Save node name and index
                        nameIndexFrame.Add(row["UniqueName"], Convert.ToString(index));

                        textMgt.Add($"{index}, BEAM, {iMaterial}, {iProp}, {indexPtI}, {indexPtJ}, {localAxis}, 0");

                        index++;
                    }
                }

                //Area geometry

                //Indicator if one way slab warning has been shown
                bool moreThan4NodesWarningShown = false;

                //Get node coordinate table for area local axis adjustment
                List<Dictionary<string, string>> tableNodeCoordinate = null;
                foreach (src.HelperEtabs.DatabaseTable databaseTableForPointConnectivity in databaseTables)
                {
                    if (databaseTableForPointConnectivity.TableKey == "Point Object Connectivity")
                    {
                        tableNodeCoordinate = databaseTableForPointConnectivity.TableInList;
                    }
                }

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
                        string areaProp = nameAreaPropAssigned[area.UniqueName];
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

                        //Finding local axis angle
                        double localAxis = 0;
                        //For area need to adjust MIDAS local axis to ETABS local axis
                        localAxis = adjustAreaLocalAxisBasedOnEtabs(area, tableNodeCoordinate);

                        if (nameLocalAxisArea.ContainsKey(area.UniqueName))
                        {
                            localAxis = localAxis + Convert.ToDouble(nameLocalAxisArea[area.UniqueName]);
                        }

                        //Finding node index
                        string indexPt1 = "0";
                        string indexPt2 = "0";
                        string indexPt3 = "0";
                        string indexPt4 = "0";
                        if (area.Points.Count == 3 || area.Points.Count == 4)
                        {
                            string pt1 = area.Points[0];
                            indexPt1 = nameIndexNode[pt1];

                            string pt2 = area.Points[1];
                            indexPt2 = nameIndexNode[pt2];

                            string pt3 = area.Points[2];
                            indexPt3 = nameIndexNode[pt3];

                            if (area.Points.Count == 4)
                            {
                                string pt4 = area.Points[3];
                                indexPt4 = nameIndexNode[pt4];
                            };

                            //Save area name and index
                            nameIndexArea.Add(area.UniqueName, Convert.ToString(index));

                            textMgt.Add($"{index}, PLATE, {iMaterial}, {iProp}, {indexPt1}, {indexPt2}, {indexPt3}, {indexPt4}, {indexThinThick}, {localAxis}");
                        }
                        else
                        {
                            skippedAreas.Add(area.UniqueName);
                        }
                        index++;
                    }
                }
                else if (databaseTable.TableKey == "Null Area Object Connectivity")
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
                        string areaProp = "Null area";
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

                        //Finding local axis angle
                        double localAxis = 0;
                        //For area need to adjust MIDAS local axis to ETABS local axis
                        localAxis = adjustAreaLocalAxisBasedOnEtabs(area, tableNodeCoordinate);

                        if (nameLocalAxisArea.ContainsKey(area.UniqueName))
                        {
                            localAxis = localAxis + Convert.ToDouble(nameLocalAxisArea[area.UniqueName]);
                        }

                        //Finding node index
                        string indexPt1 = "0";
                        string indexPt2 = "0";
                        string indexPt3 = "0";
                        string indexPt4 = "0";
                        if (area.Points.Count == 3 || area.Points.Count == 4)
                        {
                            string pt1 = area.Points[0];
                            indexPt1 = nameIndexNode[pt1];

                            string pt2 = area.Points[1];
                            indexPt2 = nameIndexNode[pt2];

                            string pt3 = area.Points[2];
                            indexPt3 = nameIndexNode[pt3];

                            if (area.Points.Count == 4)
                            {
                                string pt4 = area.Points[3];
                                indexPt4 = nameIndexNode[pt4];
                            };

                            //Save area name and index
                            nameIndexArea.Add(area.UniqueName, Convert.ToString(index));

                            textMgt.Add($"{index}, PLATE, {iMaterial}, {iProp}, {indexPt1}, {indexPt2}, {indexPt3}, {indexPt4}, {indexThinThick}, {localAxis}");
                        }
                        else
                        {
                            skippedAreas.Add(area.UniqueName);
                        }
                        index++;
                    }
                }
            }

            return (nameIndexNode, nameIndexFrame, nameIndexArea, skippedAreas, nameAreaPropAssigned);
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
                if (databaseTable.TableKey == "Frame Assignments - Releases and Partial Fixity")// && databaseTable.NumberRecords != 0)
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
                else if (databaseTable.TableKey == "Frame Assignments - Property Modifiers") //&& databaseTable.NumberRecords != 0)
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
                            string warning = $"Different weight and mass modifier found in element {row["UniqueName"]} (in ETABS) or {indexFrame} (in MIDAS). Highest value will be used!";
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
                else if (databaseTable.TableKey == "Frame Assignments - Local Axes")
                {
                    //Dont show warning because already captured in copyFrameAssignmentsLocalAxis
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
                        (string directionMidas, string projected, double loadFactor) = parseLoadFrameDirectionEtabsToMidas(directionEtabs);

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

                        //Check if location 1 and location 2 different. Else will put warning and ignore the load
                        if (location1 != location2)
                        {
                            //MGTHeader
                            textMgt.Add($"*USE-STLD, {loadPattern}");
                            textMgt.Add("*BEAMLOAD");
                            textMgt.Add("; (index element), (load classificiation=BEAM), (loadtype=UNILOAD), (direction), (projected), (bEccen=NO), (eccenDir=aDir[1]), (i-end=''), (j-end=''), (bj-end=''), " +
                                "(location relative 1), (force1), (location relative 2), (force2), (location relative 3=0), (force3=0), (location relative 4=0), (force4=0)");
                            textMgt.Add($"{indexFrame}, BEAM, {loadTypeMidas}, {directionMidas}, {projected}, NO, aDir[1], , , , {location1}, {force1}, {location2}, {force2}, 0, 0, 0, 0");
                        }
                        else
                        {
                            string warning = $"Frame load distributed have same start and end in element {row["UniqueName"]} (in ETABS) or {indexFrame} (in MIDAS). Will ignore load!";
                            //MessageBox.Show(warning);
                            textWarning.Add(warning);
                        }
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
                        (string directionMidas, string projected, double loadFactor) = parseLoadFrameDirectionEtabsToMidas(directionEtabs);

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

                    //Total up load in each node because midas can only have one load in each node
                    List<Dictionary<string, string>> nodeLoad = new List<Dictionary<string, string>>();

                    foreach (Dictionary<string, string> row in table)
                    {
                        string uniqueName = row["UniqueName"];
                        string loadPattern = row["LoadPattern"];

                        double fX = Convert.ToDouble(row["FX"]);
                        double fY = Convert.ToDouble(row["FY"]);
                        double fZ = Convert.ToDouble(row["FZ"]);
                        double mX = Convert.ToDouble(row["MX"]);
                        double mY = Convert.ToDouble(row["MY"]);
                        double mZ = Convert.ToDouble(row["MZ"]);

                        bool nodeNameAndLoadPatternExist = false;

                        //Going through nodeLoad list and see if already have same node with same load pattern
                        foreach(Dictionary<string, string> nodeLoadSingle in nodeLoad)
                        {
                            if(nodeLoadSingle["uniqueName"] == uniqueName)
                            {
                                if(nodeLoadSingle["loadPattern"] == loadPattern)
                                {
                                    //Found unique name and load pattern already exist
                                    nodeNameAndLoadPatternExist = true;

                                    nodeLoadSingle["fX"] = (Convert.ToDouble(nodeLoadSingle["fX"]) + fX).ToString();
                                    nodeLoadSingle["fY"] = (Convert.ToDouble(nodeLoadSingle["fY"]) + fY).ToString();
                                    nodeLoadSingle["fZ"] = (Convert.ToDouble(nodeLoadSingle["fZ"]) + fZ).ToString();
                                    nodeLoadSingle["mX"] = (Convert.ToDouble(nodeLoadSingle["mX"]) + mX).ToString();
                                    nodeLoadSingle["mY"] = (Convert.ToDouble(nodeLoadSingle["mY"]) + mY).ToString();
                                    nodeLoadSingle["mZ"] = (Convert.ToDouble(nodeLoadSingle["mZ"]) + mZ).ToString();

                                    break;
                                }
                            }
                        }

                        if (!nodeNameAndLoadPatternExist)
                        {
                            Dictionary<string, string> nodeLoadSingle = new Dictionary<string, string>();

                            nodeLoadSingle["uniqueName"] = uniqueName;
                            nodeLoadSingle["loadPattern"] = loadPattern;

                            nodeLoadSingle["fX"] = fX.ToString();
                            nodeLoadSingle["fY"] = fY.ToString();
                            nodeLoadSingle["fZ"] = fZ.ToString();
                            nodeLoadSingle["mX"] = mX.ToString();
                            nodeLoadSingle["mY"] = mY.ToString();
                            nodeLoadSingle["mZ"] = mZ.ToString();

                            nodeLoad.Add(nodeLoadSingle);
                        }

                    }

                    //Filtering node that has 0 load
                    List<Dictionary<string, string>> nodeLoadToRemove = new List<Dictionary<string, string>>();
                    foreach (Dictionary<string, string> nodeLoadSingle in nodeLoad)
                    {
                        double fX = Convert.ToDouble(nodeLoadSingle["fX"]);
                        double fY = Convert.ToDouble(nodeLoadSingle["fY"]);
                        double fZ = Convert.ToDouble(nodeLoadSingle["fZ"]);
                        double mX = Convert.ToDouble(nodeLoadSingle["mX"]);
                        double mY = Convert.ToDouble(nodeLoadSingle["mY"]);
                        double mZ = Convert.ToDouble(nodeLoadSingle["mZ"]);

                        if (fX == 0 && fY == 0 && fZ == 0 && mX == 0 && mY == 0 && mZ == 0)
                        {
                            nodeLoadToRemove.Add(nodeLoadSingle);
                        }

                    }
                    foreach (Dictionary<string, string> nodeLoadSingleToRemove in nodeLoadToRemove)
                    {
                        nodeLoad.Remove(nodeLoadSingleToRemove);
                    }

                    foreach (Dictionary<string, string> nodeLoadSingle in nodeLoad)
                    {
                        string loadPattern = nodeLoadSingle["loadPattern"];

                        string indexNode = nameIndexNode[nodeLoadSingle["uniqueName"]];

                        double fX = Convert.ToDouble(nodeLoadSingle["fX"]);
                        double fY = Convert.ToDouble(nodeLoadSingle["fY"]);
                        double fZ = Convert.ToDouble(nodeLoadSingle["fZ"]);
                        double mX = Convert.ToDouble(nodeLoadSingle["mX"]);
                        double mY = Convert.ToDouble(nodeLoadSingle["mY"]);
                        double mZ = Convert.ToDouble(nodeLoadSingle["mZ"]);

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

        private static void copyAreaLoadAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexArea,
                                                    Dictionary<string, string> loadPatternNameEtabsMidas, List<string> skippedAreas, Dictionary<string, Dictionary<string, string>> uniformLoadSets)
        {

            bool showWarning = false;
            bool isError = false;

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Area load unfiorm
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Area Load Assignments - Uniform")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        bool inSkippedAreas = false;
                        foreach (string skippedArea in skippedAreas)
                        {
                            if (row["UniqueName"] == skippedArea)
                            {
                                inSkippedAreas = true;
                                break;
                            }
                        }

                        if (!inSkippedAreas)
                        {

                            string loadPattern = loadPatternNameEtabsMidas[row["LoadPattern"]];

                            string indexArea = nameIndexArea[row["UniqueName"]];

                            string directionEtabs = row["Dir"];
                            (string directionMidas, string projected, double loadFactor) = parseLoadAreaDirectionEtabsToMidas(directionEtabs);

                            double force = Convert.ToDouble(row["Load"]) * loadFactor;

                            //MGTHeader
                            textMgt.Add($"*USE-STLD, {loadPattern}");
                            textMgt.Add("*PRESSURE   ;Pressure Loads");
                            textMgt.Add("; (index element), (load classificiation=PRES), (elementtype=PLATE), (loadtype=FACE), (direction), (Vx=0), (Vy=0), (Vz=0)," +
                                "(projected yes/no), (load uniform), (P1=0), (P2=0), (P3=0), (P4=0), (group=''), (psltkey=0)");
                            textMgt.Add($"{indexArea}, PRES, PLATE, FACE, {directionMidas}, 0, 0, 0, {projected}, {force}, 0, 0, 0, 0, ,0");
                        }

                    }
                }
                else if (databaseTable.TableKey == "Area Load Assignments - Uniform Load Sets")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        bool inSkippedAreas = false;
                        foreach (string skippedArea in skippedAreas)
                        {
                            if (row["UniqueName"] == skippedArea)
                            {
                                inSkippedAreas = true;
                                break;
                            }
                        }

                        if (!inSkippedAreas)
                        {
                            string indexArea = nameIndexArea[row["UniqueName"]];

                            string uniformLoadSetName = row["LoadSet"];

                            //Adding each uniform load in uniform load sets into the shell
                            Dictionary<string, string> uniformLoadsInLoadSet = uniformLoadSets[uniformLoadSetName];
                            foreach (KeyValuePair<string, string> uniformLoad in uniformLoadsInLoadSet)
                            {
                                string loadPattern = uniformLoad.Key;
                                double force = Convert.ToDouble(uniformLoad.Value) * -1;
                                string directionMidas = "Gz";
                                string projected = "NO";

                                //MGTHeader
                                textMgt.Add($"*USE-STLD, {loadPattern}");
                                textMgt.Add("*PRESSURE   ;Pressure Loads");
                                textMgt.Add("; (index element), (load classificiation=PRES), (elementtype=PLATE), (loadtype=FACE), (direction), (Vx=0), (Vy=0), (Vz=0)," +
                                    "(projected yes/no), (load uniform), (P1=0), (P2=0), (P3=0), (P4=0), (group=''), (psltkey=0)");
                                textMgt.Add($"{indexArea}, PRES, PLATE, FACE, {directionMidas}, 0, 0, 0, {projected}, {force}, 0, 0, 0, 0, ,0");
                            }
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

        private static void copyAreaAssignmentsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexArea,
            List<Dictionary<string, string>> nameModifierAreaProp, Dictionary<string, string> nameAreaPropAssigned)
        {
            bool showWarning = false;
            bool isError = false;

            //List to save modifier in area assign
            List<Dictionary<string, string>> nameModifierAreaAssign = new List<Dictionary<string, string>>();

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Area Assignments - Stiffness Modifiers")
                {
                    
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {

                        //Save shell modifier in area assignment
                        Dictionary<string, string> nameModifierAreaAssignSingle = new Dictionary<string, string>();

                        double fxxMod = Convert.ToDouble(row["f11Mod"]);
                        double fyyMod = Convert.ToDouble(row["f22Mod"]);
                        double fxyMod = Convert.ToDouble(row["f12Mod"]);
                        double mxxMod = Convert.ToDouble(row["m22Mod"]);
                        double myyMod = Convert.ToDouble(row["m11Mod"]);
                        double mxyMod = Convert.ToDouble(row["m12Mod"]);
                        double vxxMod = Convert.ToDouble(row["v13Mod"]);
                        double vyyMod = Convert.ToDouble(row["v23Mod"]);
                        double weightMod = 0;

                        if (Convert.ToDouble(row["MMod"]) != Convert.ToDouble(row["WMod"]))
                        {
                            string warning = $"Different weight and mass modifier found in area {row["UniqueName"]} (in ETABS). Highest value will be used!";
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

                        if (fxxMod == 1 && fyyMod == 1 && fxyMod == 1 && mxxMod == 1 && myyMod == 1 && mxyMod == 1 && vxxMod == 1 && vyyMod == 1 && weightMod == 1)
                        {
                            //if no modifier then do nothing
                        }
                        else
                        {
                            nameModifierAreaAssignSingle.Add("uniqueName",row["UniqueName"]);
                            nameModifierAreaAssignSingle.Add("fxx", fxxMod.ToString());
                            nameModifierAreaAssignSingle.Add("fyy", fyyMod.ToString());
                            nameModifierAreaAssignSingle.Add("fxy", fxyMod.ToString());
                            nameModifierAreaAssignSingle.Add("mxx", mxxMod.ToString());
                            nameModifierAreaAssignSingle.Add("myy", myyMod.ToString());
                            nameModifierAreaAssignSingle.Add("mxy", mxyMod.ToString());
                            nameModifierAreaAssignSingle.Add("vxx", vxxMod.ToString());
                            nameModifierAreaAssignSingle.Add("vyy", myyMod.ToString());
                            nameModifierAreaAssignSingle.Add("weightMod", myyMod.ToString());

                            nameModifierAreaAssign.Add(nameModifierAreaAssignSingle);
                        }
                    }
                }
                else if (databaseTable.TableKey == "Area Assignments - Edge Releases")
                {
                    //MGT Header
                    textMgt.Add("*PLATE-RLS    ; Plate End Release");
                    textMgt.Add("; ELEM_LIST, N1, N2, N3, N4, GROUP");

                    //To save list of uniquename
                    List<string> areasWithEdgeRelease = new List<string>();

                    //To save releases
                    Dictionary<string, List<string>> areasEdgeReleaseEdgeNumber = new Dictionary<string, List<string>>();

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    //Getting uniqueName list of shell that 
                    foreach (Dictionary<string, string> row in table)
                    {
                        if (!areasWithEdgeRelease.Contains(row["UniqueName"]))
                        {
                            areasWithEdgeRelease.Add(row["UniqueName"]);
                        }
                    }

                    //Getting list of releases
                    foreach (string areaWithEdgeRelease in areasWithEdgeRelease)
                    {
                        List<string> edgeReleaseEdgeNumber = new List<string>();

                        foreach (Dictionary<string, string> row in table)
                        {
                            if (row["UniqueName"] == areaWithEdgeRelease)
                            {
                                edgeReleaseEdgeNumber.Add(row["EdgeNumber"]);
                            }
                        }
                        areasEdgeReleaseEdgeNumber.Add(areaWithEdgeRelease, edgeReleaseEdgeNumber);
                    }

                    foreach (KeyValuePair<string, List<string>> areaEndReleaseEdgeNumber in areasEdgeReleaseEdgeNumber)
                    {
                        //Check if have more than for releases
                        if (areaEndReleaseEdgeNumber.Value.Count > 4)
                        {

                            //not valid!
                        }
                        else
                        {
                            string releaseN1 = "00000";
                            string releaseN2 = "00000";
                            string releaseN3 = "00000";
                            string releaseN4 = "00000";

                            string areaUniqueName = areaEndReleaseEdgeNumber.Key;

                            foreach (Dictionary<string, string> row in table)
                            {
                                
                                if (areaUniqueName == row["UniqueName"])
                                {
                                    //Indicator if valid release for midas or not
                                    bool isValidPlateRelase = true;

                                    string releaseInPlaneShear = row["RInShear"];
                                    string releaseInDirectForce = row["RDirForce"];
                                    string releaseOutPlaneShear = row["ROutShear"];
                                    string releaseBending = row["RBending"];
                                    string releaseTwisting = row["RTwisting"];

                                    string springInPlaneShear = row["SInShear"];
                                    string springInDirectForce = row["SDirForce"];
                                    string springOutPlaneShear = row["SOutShear"];
                                    string springBending = row["SBending"];
                                    string springTwisting = row["STwisting"];

                                    //Check if valid
                                    if (releaseInPlaneShear != releaseInDirectForce || releaseBending != releaseTwisting)
                                    {
                                        isValidPlateRelase = false;
                                    }

                                    //Check if have spring
                                    if (springInPlaneShear != "0" || springInDirectForce != "0" || springOutPlaneShear != null || springBending != "0" || springTwisting != "0")
                                    {
                                        isValidPlateRelase = false;
                                    }

                                    //If valid then continue
                                    if (isValidPlateRelase)
                                    {
                                        string releaseFx = "0";
                                        string releaseFy = "0";
                                        string releaseFz = "0";
                                        string releaseMx = "0";
                                        string releaseMy = "0";

                                        if (releaseInPlaneShear == "Yes" && releaseInDirectForce == "Yes")
                                        {
                                            releaseFx = "1";
                                            releaseFy = "1";
                                        }
                                        if (releaseOutPlaneShear == "Yes")
                                        {
                                            releaseFz = "1";
                                        }
                                        if (releaseBending == "Yes" && releaseTwisting == "Yes")
                                        {
                                            releaseMx = "1";
                                            releaseMy = "1";
                                        }

                                        //Releasing edge 1
                                        //N4 and N1
                                        if (row["EdgeNumber"] == "1")
                                        {
                                            string updatedReleaseN12 = releaseFx + releaseFy + releaseFz + releaseMx + releaseMy;

                                            releaseN1 = checkIfPlateAlreadyRelease(updatedReleaseN12, releaseN1);
                                            releaseN2 = checkIfPlateAlreadyRelease(updatedReleaseN12, releaseN2);
                                        }
                                        else if (row["EdgeNumber"] == "2")
                                        {
                                            string updatedReleaseN23 = releaseFx + releaseFy + releaseFz + releaseMx + releaseMy;

                                            releaseN2 = checkIfPlateAlreadyRelease(updatedReleaseN23, releaseN2);
                                            releaseN3 = checkIfPlateAlreadyRelease(updatedReleaseN23, releaseN3);
                                        }
                                        else if (row["EdgeNumber"] == "3")
                                        {
                                            string updatedReleaseN34 = releaseFx + releaseFy + releaseFz + releaseMx + releaseMy;

                                            releaseN3 = checkIfPlateAlreadyRelease(updatedReleaseN34, releaseN3);
                                            releaseN4 = checkIfPlateAlreadyRelease(updatedReleaseN34, releaseN4);
                                        }
                                        else if (row["EdgeNumber"] == "4")
                                        {
                                            string updatedReleaseN41 = releaseFx + releaseFy + releaseFz + releaseMx + releaseMy;

                                            releaseN4 = checkIfPlateAlreadyRelease(updatedReleaseN41, releaseN4);
                                            releaseN1 = checkIfPlateAlreadyRelease(updatedReleaseN41, releaseN1);
                                        }
                                    }
                                }
                            }

                            //Getting area index in MIDAS
                            string areaIndex = nameIndexArea[areaUniqueName];

                            //MGT
                            textMgt.Add($"{areaIndex}, {releaseN1}, {releaseN2}, {releaseN3}, {releaseN4}, ");
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

            //Going through each area to combine area modifier applied in area prop and area assignment
            //MGTHeader for area assignment stiffness modifier
            textMgt.Add("*PLATE-SSRF    ;*Plate Stiffness Scale Factor");
            textMgt.Add("; i, (axial Fxx modifier), (axial Fyy modifier), (axial Fxy modifier), (moment Mxx modifier), (moment Myy modifier), (moment Mxy modifier), (moment Vxx modifier), (moment Vyy modifier), (group='')");
            foreach (KeyValuePair<string, string> area in nameIndexArea)
            {
                string uniqueName = area.Key;
                string areaIndex = area.Value;
                string areaProp = nameAreaPropAssigned[uniqueName];

                //Go through area modifier which are defined in area property
                double fxxModFromAreaProp = 1;
                double fyyModFromAreaProp = 1;
                double fxyModFromAreaProp = 1;
                double mxxModFromAreaProp = 1;
                double myyModFromAreaProp = 1;
                double mxyModFromAreaProp = 1;
                double vxxModFromAreaProp = 1;
                double vyyModFromAreaProp = 1;
                double weightModFromAreaProp = 1;

                foreach (Dictionary<string, string> nameModifierAreaPropSingle in nameModifierAreaProp)
                {
                    if (nameModifierAreaPropSingle["nameAreaProp"] == areaProp)
                    {
                        //If have area modifier defined in area property
                        fxxModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["fxx"]);
                        fyyModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["fyy"]);
                        fxyModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["fxy"]);
                        mxxModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["mxx"]);
                        myyModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["myy"]);
                        mxyModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["mxy"]);
                        vxxModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["vxx"]);
                        vyyModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["vyy"]);
                        weightModFromAreaProp = Convert.ToDouble(nameModifierAreaPropSingle["weightMod"]);

                        break;
                    }
                }

                //Go through area modifier which are defined in area assign
                double fxxModFromAreaAssign = 1;
                double fyyModFromAreaAssign = 1;
                double fxyModFromAreaAssign = 1;
                double mxxModFromAreaAssign = 1;
                double myyModFromAreaAssign = 1;
                double mxyModFromAreaAssign = 1;
                double vxxModFromAreaAssign = 1;
                double vyyModFromAreaAssign = 1;
                double weightModFromAreaAssign = 1;

                foreach (Dictionary<string, string> nameModifierAreaAssignSingle in nameModifierAreaAssign)
                {
                    if (nameModifierAreaAssignSingle["uniqueName"] == uniqueName)
                    {
                        //If have area modifier defined in area property
                        fxxModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["fxx"]);
                        fyyModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["fyy"]);
                        fxyModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["fxy"]);
                        mxxModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["mxx"]);
                        myyModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["myy"]);
                        mxyModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["mxy"]);
                        vxxModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["vxx"]);
                        vyyModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["vyy"]);
                        weightModFromAreaAssign = Convert.ToDouble(nameModifierAreaAssignSingle["weightMod"]);

                        break;
                    }
                }

                //Multiply both
                double fxxMod = fxxModFromAreaProp * fxxModFromAreaAssign;
                double fyyMod = fyyModFromAreaProp * fyyModFromAreaAssign;
                double fxyMod = fxyModFromAreaProp * fxyModFromAreaAssign;
                double mxxMod = mxxModFromAreaProp * mxxModFromAreaAssign;
                double myyMod = myyModFromAreaProp * myyModFromAreaAssign;
                double mxyMod = mxyModFromAreaProp * mxyModFromAreaAssign;
                double vxxMod = vxxModFromAreaProp * vxxModFromAreaAssign;
                double vyyMod = vyyModFromAreaProp * vyyModFromAreaAssign;
                double weightMod = weightModFromAreaProp * weightModFromAreaAssign;

                //Write in MGT
                if (fxxMod == 1 && fyyMod == 1 && fxyMod == 1 && mxxMod == 1 && myyMod == 1 && mxyMod == 1 && vxxMod == 1 && vyyMod == 1 && weightMod == 1)
                {
                    //if no modifier then do nothing
                }
                else
                {
                    textMgt.Add($"{areaIndex}, {fxxMod}, {fyyMod}, {fxyMod}, {mxxMod}, {myyMod}, {mxyMod}, {vxxMod}, {vyyMod}, ");
                }


                //MIDAS cant have weight modifier, put warning if weight modifier is not 1
                if (weightMod != 1)
                {
                    string warning = $"Weight or mass modifier found in area assignment {uniqueName} (in ETABS). MIDAS cant have weight modifier!";
                    //MessageBox.Show(warning);
                    textWarning.Add(warning);
                }

            }

        }
        private static void copyLinksToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexNode)
        {
            bool showWarning = false;
            bool isError = false;

            //List to save link assigned property
            Dictionary<string, string> nameLinkProperty = new Dictionary<string, string>();
            Dictionary<string, string> nameLinkLocalAxis = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> linkProperties = new Dictionary<string, Dictionary<string, string>>();

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Link Assignments - Link Properties")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        nameLinkProperty.Add(row["Name"], row["LinkProp"]);
                    }
                }
                else if (databaseTable.TableKey == "Link Assignments - Local Axes")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        nameLinkLocalAxis.Add(row["Name"], row["Angle"]);
                    }
                }
                else if (databaseTable.TableKey == "Link Property Definitions - Linear")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        Dictionary<string, string> linkPropertiesSingle = new Dictionary<string, string>();

                        string name = row["Name"];
                        string statusDx = row["U1Status"];
                        string statusDz = row["U2Status"];
                        string statusDy = row["U3Status"];
                        string statusRx = row["R1Status"];
                        string statusRz = row["R2Status"];
                        string statusRy = row["R3Status"];

                        string rigidDx = "NO";
                        string rigidDz = "NO";
                        string rigidDy = "NO";
                        string rigidRx = "NO";
                        string rigidRz = "NO";
                        string rigidRy = "NO";

                        double stiffnessDx = 0;
                        double stiffnessDz = 0;
                        double stiffnessDy = 0;
                        double stiffnessRx = 0;
                        double stiffnessRz = 0;
                        double stiffnessRy = 0;

                        if (statusDx == "Fixed")
                        {
                            rigidDx = "YES";
                        }
                        else if (statusDx == "Inactive")
                        {
                            rigidDx = "NO";
                        }
                        else if (statusDx == "Active")
                        {
                            rigidDx = "NO";
                            stiffnessDx = Convert.ToDouble(row["StiffU1"]);
                        }

                        if (statusDz == "Fixed")
                        {
                            rigidDz = "YES";
                        }
                        else if (statusDz == "Inactive")
                        {
                            rigidDz = "NO";
                        }
                        else if (statusDz == "Active")
                        {
                            rigidDz = "NO";
                            stiffnessDz = Convert.ToDouble(row["StiffU2"]);
                        }


                        if (statusDy == "Fixed")
                        {
                            rigidDy = "YES";
                        }
                        else if (statusDy == "Inactive")
                        {
                            rigidDy = "NO";
                        }
                        else if (statusDy == "Active")
                        {
                            rigidDy = "NO";
                            stiffnessDy = Convert.ToDouble(row["StiffU3"]);
                        }

                        if (statusRx == "Fixed")
                        {
                            rigidRx = "YES";
                        }
                        else if (statusRx == "Inactive")
                        {
                            rigidRx = "NO";
                        }
                        else if (statusRx == "Active")
                        {
                            rigidRx = "NO";
                            stiffnessRx = Convert.ToDouble(row["StiffU4"]);
                        }

                        if (statusRz == "Fixed")
                        {
                            rigidRz = "YES";
                        }
                        else if (statusRz == "Inactive")
                        {
                            rigidRz = "NO";
                        }
                        else if (statusRz == "Active")
                        {
                            rigidRz = "NO";
                            stiffnessRz = Convert.ToDouble(row["StiffU5"]);
                        }

                        if (statusRy == "Fixed")
                        {
                            rigidRy = "YES";
                        }
                        else if (statusRy == "Inactive")
                        {
                            rigidRy = "NO";
                        }
                        else if (statusRy == "Active")
                        {
                            rigidRy = "NO";
                            stiffnessRy = Convert.ToDouble(row["StiffU6"]);
                        }

                        linkPropertiesSingle.Add("rigidDx", rigidDx);
                        linkPropertiesSingle.Add("rigidDz", rigidDz);
                        linkPropertiesSingle.Add("rigidDy", rigidDy);
                        linkPropertiesSingle.Add("rigidRx", rigidRx);
                        linkPropertiesSingle.Add("rigidRz", rigidRz);
                        linkPropertiesSingle.Add("rigidRy", rigidRy);

                        linkPropertiesSingle.Add("stiffnessDx", stiffnessDx.ToString());
                        linkPropertiesSingle.Add("stiffnessDz", stiffnessDz.ToString());
                        linkPropertiesSingle.Add("stiffnessDy", stiffnessDy.ToString());
                        linkPropertiesSingle.Add("stiffnessRx", stiffnessRx.ToString());
                        linkPropertiesSingle.Add("stiffnessRz", stiffnessRz.ToString());
                        linkPropertiesSingle.Add("stiffnessRy", stiffnessRy.ToString());

                        linkProperties.Add(name, linkPropertiesSingle);
                    }
                }
            }

            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Link Object Connectivity")
                {
                    //MGTHeader for area assignment stiffness modifier
                    textMgt.Add("*ELASTICLINK    ; Elastic Link");
                    textMgt.Add("; i, (index node 1), (index node 2), (link=GEN), (angle), (rigid Dx), (rigid Dy), (rigid Dz), (rigid Rx), (rigid Ry), (rigid Rz), (stiffness Dx), (stiffness Dy), (stiffness Dz)" +
                        ", (stiffness Rx), (stiffness Ry), (stiffness Rz), (bShear=NO), (DRy=0.5), (DRz=0.5), (group='')");

                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    int index = 1;

                    foreach (Dictionary<string, string> row in table)
                    {
                        string pt1 = row["UniquePt1"];
                        string indexPt1 = nameIndexNode[pt1];

                        string pt2 = row["UniquePt2"];
                        string indexPt2 = nameIndexNode[pt2];

                        string linkName = row["Name"];
                        string linkProperty = nameLinkProperty[linkName];
                        string linkLocalAxis = nameLinkLocalAxis[linkName];

                        string rigidDx = linkProperties[linkProperty]["rigidDx"];
                        string rigidDz = linkProperties[linkProperty]["rigidDz"];
                        string rigidDy = linkProperties[linkProperty]["rigidDy"];
                        string rigidRx = linkProperties[linkProperty]["rigidRx"];
                        string rigidRz = linkProperties[linkProperty]["rigidRz"];
                        string rigidRy = linkProperties[linkProperty]["rigidRy"];

                        string stiffnessDx = linkProperties[linkProperty]["stiffnessDx"];
                        string stiffnessDz = linkProperties[linkProperty]["stiffnessDz"];
                        string stiffnessDy = linkProperties[linkProperty]["stiffnessDy"];
                        string stiffnessRx = linkProperties[linkProperty]["stiffnessRx"];
                        string stiffnessRz = linkProperties[linkProperty]["stiffnessRz"];
                        string stiffnessRy = linkProperties[linkProperty]["stiffnessRy"];

                        textMgt.Add($"{index}, {indexPt1}, {indexPt2}, GEN, {linkLocalAxis}, {rigidDx}, {rigidDy}, {rigidDz}, {rigidRx}, {rigidRy}, {rigidRz}, {stiffnessDx}, {stiffnessDy}, {stiffnessDz}, " +
                            $"{stiffnessRx}, {stiffnessRy}, {stiffnessRz}, NO, 0.5, 0.5, ");

                        index++;
                    }
                }
            }
        }

        private static Dictionary<string, string> copyLoadCaseToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> loadPatternNameEtabsMidas)
        {
            bool showWarning = false;
            bool isError = false;

            //List to save load case name etabs and midas
            Dictionary<string, string> loadCaseNameEtabsMidas = new Dictionary<string, string>();

            //List to save load case name
            List<string> loadCasesName = new List<string>();

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);


            //if (isError){return;}

            //Getting all the load cases name first
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Load Case Definitions - Summary")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        loadCasesName.Add(row["Name"]);
                    };
                }
            }

            //MGT Header
            textMgt.Add("*LOADCOMB    ; Combinations");
            textMgt.Add("; (NAME=name), (kind=GEN), (active=ACTIVE), (bES=0), (iType=0), (desc=''), (iSERVE-TYPE=0), (nLCOMTYPE=0), (nSEISTYPE=0)");
            textMgt.Add("; (load type=ST), (LCNAME1), (FACTOR)");

            //Go through each load case
            foreach (string loadCaseName in loadCasesName)
            {
                foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
                {
                    if (databaseTable.TableKey == "Load Case Definitions - Linear Static")
                    {
                        bool foundInsideStatic = false;

                        List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                        table = databaseTable.TableInList;

                        //Look if the load case is inside this particular table
                        foreach (Dictionary<string, string> row in table)
                        {
                            if (loadCaseName == row["Name"])
                            {
                                foundInsideStatic = true;

                                //MGT
                                string loadCaseNameInMidas = loadCaseName;
                                if (loadCaseNameInMidas.Length > 20 - 4)
                                {
                                    loadCaseNameInMidas = "LCa_" + loadCaseNameInMidas.Substring(0, 20-4);
                                }
                                else
                                {
                                    loadCaseNameInMidas = "LCa_" + loadCaseNameInMidas;
                                }

                                textMgt.Add($"NAME={loadCaseNameInMidas}, GEN, ACTIVE, 0, 0, , 0, 0, 0");

                                loadCaseNameEtabsMidas.Add(loadCaseName, loadCaseNameInMidas);

                                break;
                            }
                        };

                        //If the load case is in this table
                        if (foundInsideStatic)
                        {
                            //Looking for the correct 
                            foreach (Dictionary<string, string> row in table)
                            {
                                if (loadCaseName == row["Name"])
                                {
                                    //Finding load pattern name in midas
                                    string loadPatternNameInMidas = loadPatternNameEtabsMidas[row["LoadName"]];

                                    textMgt.Add($"ST, {loadPatternNameInMidas}, {row["LoadSF"]}");
                                }
                            };
                            break;
                        }                            
                    }
                    else
                    {
                        //For other load case which is not static linear

                        bool foundInsideNonStatic = false;

                        if (databaseTable.NumberRecords > 0)
                        {
                            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                            table = databaseTable.TableInList;

                            //Look if the load case is inside this particular table
                            foreach (Dictionary<string, string> row in table)
                            {
                                if (loadCaseName == row["Name"])
                                {
                                    foundInsideNonStatic = true;

                                    //MGT
                                    string loadCaseNameInMidas = loadCaseName;
                                    if (loadCaseNameInMidas.Length > 20 - 10)
                                    {
                                        loadCaseNameInMidas = "LCa_" + loadCaseNameInMidas.Substring(0, 20 - 10) + "_CHECK";
                                    }
                                    else
                                    {
                                        loadCaseNameInMidas = "LCa_" + loadCaseNameInMidas + "_CHECK";
                                    }
                                    textMgt.Add($"NAME={loadCaseNameInMidas}, GEN, ACTIVE, 0, 0, , 0, 0, 0");

                                    loadCaseNameEtabsMidas.Add(loadCaseName, loadCaseNameInMidas);

                                    break;
                                }
                            };
                            
                            //If the load case is in this table
                            if (foundInsideNonStatic)
                            {
                                //Show warning that the type cant be converted to midas
                                string warning = $"{databaseTable.TableKey} not supported! Load case {loadCaseName} will be imported as as static load case with _CHECK behind its name and with load factor = 1!";
                                MessageBox.Show(warning);
                                textWarning.Add(warning);

                                //Give the first load pattern name in midas
                                string loadPatternNameInMidas = loadPatternNameEtabsMidas.FirstOrDefault().Value;

                                textMgt.Add($"ST, {loadPatternNameInMidas}, 1");

                                break;
                            }
                        }
                    }
                }
            }
            return loadCaseNameEtabsMidas;
        }

        private static void copyLoadCombinationToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> loadCaseNameEtabsMidas)
        {
            bool showWarning = false;
            bool isError = false;

            //List to save load combination name
            List<string> loadCombinationsName = new List<string>();

            //List to save load combination name that are envelope, envelope will not be included for now
            List<string> loadCombinationsEnvelopeName = new List<string>();

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //Getting all the load combination which are in envelope name
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Load Combination Definitions")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Envelope will not be included for now
                        if (!loadCombinationsEnvelopeName.Contains(row["Name"]) && row["Type"] == "Envelope")
                        {
                            loadCombinationsEnvelopeName.Add(row["Name"]);
                        }

                    }
                }
            }

            //Getting all the load combination name
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Load Combination Definitions")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        //Envelope will not be included for now
                        if (!loadCombinationsName.Contains(row["Name"]) && !loadCombinationsEnvelopeName.Contains(row["Name"]))
                        {
                            loadCombinationsName.Add(row["Name"]);
                        }
                    }
                }
            }

            //Writing MGT for each load combination name
            foreach(string loadCombinationName in loadCombinationsName)
            {
                //MGT
                string loadCombinationNameInMidas = loadCombinationName;
                if (loadCombinationNameInMidas.Length > 20 - 4)
                {
                    loadCombinationNameInMidas = "LCo_" + loadCombinationNameInMidas.Substring(0, 20 - 4);
                }
                else
                {
                    loadCombinationNameInMidas = "LCo_" + loadCombinationNameInMidas;
                }
                textMgt.Add($"NAME={loadCombinationNameInMidas}, GEN, ACTIVE, 0, 0, , 0, 0, 0");

                foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
                {
                    if (databaseTable.TableKey == "Load Combination Definitions")
                    {
                        List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                        table = databaseTable.TableInList;

                        foreach (Dictionary<string, string> row in table)
                        {
                            if (loadCombinationName == row["Name"])
                            {
                                //Finding load case name in midas
                                string loadCaseNameInMidas = loadCaseNameEtabsMidas[row["LoadName"]];

                                textMgt.Add($"CB, {loadCaseNameInMidas}, {row["SF"]}");
                            }
                        }
                    }
                }
            }
        }

        private static void copyGroupsToMidas(cSapModel Sap, List<string> tableKeys, List<string> textMgt, List<string> textWarning, Dictionary<string, string> nameIndexNode,
            Dictionary<string, string> nameIndexFrame, Dictionary<string, string> nameIndexArea, List<string> skippedAreas)
        {
            bool showWarning = false;
            bool isError = false;

            //List to save load combination name
            List<string> groupsName = new List<string>();

            List<src.HelperEtabs.DatabaseTable> databaseTables = new List<src.HelperEtabs.DatabaseTable>();

            (isError, databaseTables) = src.HelperEtabs.GetMultipleTableDisplay(Sap, tableKeys, showWarning);

            //if (isError){return;}

            //MGT Header
            textMgt.Add("*GROUP    ; Group");
            textMgt.Add("; (group name), (node list), (element list), (plane type = 0)");

            //Getting all the group name
            foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
            {
                if (databaseTable.TableKey == "Group Assignments")
                {
                    List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                    table = databaseTable.TableInList;

                    foreach (Dictionary<string, string> row in table)
                    {
                        if (!groupsName.Contains(row["GroupName"]))
                        {
                            groupsName.Add(row["GroupName"]);
                        }
                    }
                }
            }

            //Getting all element and node inside each group
            foreach (string groupName in groupsName)
            {
                //String for element
                string elements = "";

                //String for node
                string nodes = "";

                foreach (src.HelperEtabs.DatabaseTable databaseTable in databaseTables)
                {
                    if (databaseTable.TableKey == "Group Assignments")
                    {
                        List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

                        table = databaseTable.TableInList;

                        foreach (Dictionary<string, string> row in table)
                        {
                            if (groupName == row["GroupName"])
                            {
                                if (row["ObjectType"] == "Line")
                                {
                                    string elementIndex = nameIndexFrame[row["UniqueName"]];
                                    elements += elementIndex + " ";
                                }
                                else if (row["ObjectType"] == "Area")
                                {
                                    //Only if area not contained in skipped area
                                    if (!skippedAreas.Contains(row["UniqueName"]))
                                    {
                                        string elementIndex = nameIndexArea[row["UniqueName"]];
                                        elements += elementIndex + " ";
                                    }
                                }
                                else if (row["ObjectType"] == "Point")
                                {
                                    string nodeIndex = nameIndexNode[row["UniqueName"]];
                                    nodes += nodeIndex + " ";
                                }
                            }
                        }
                        //After going through all row for 1 group, add MGT line
                        textMgt.Add($"{groupName}, {nodes}, {elements}, 0");
                    }
                }
            }

        }

        #region Other methods

                private static (string, string, double) parseLoadFrameDirectionEtabsToMidas (string directionEtabs)
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

        private static (string, string, double) parseLoadAreaDirectionEtabsToMidas(string directionEtabs)
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
                directionMidas = "Ly";
                projected = "NO";
            }
            else if (directionEtabs == "Local-3")
            {
                directionMidas = "Lz";
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

        public static void CreateExcelComparingEtabsMidas(cSapModel Sap, Dictionary<string, string> nameIndexNode, Dictionary<string, string> nameIndexFrame, Dictionary<string, string> nameIndexArea)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetNode;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetFrame;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetArea;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheetNode = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheetFrame = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.Add();
            xlWorkSheetArea = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.Add();

            //Creating node list
            xlWorkSheetNode.Cells[1, 1] = "Node";

            xlWorkSheetNode.Cells[2, 1] = "ETABS Unique Name";
            xlWorkSheetNode.Cells[2, 2] = "MIDAS Index";

            int xlRow = 3;
            foreach (KeyValuePair<string, string> node in nameIndexNode)
            {
                string etabsUniqueName = node.Key;
                string midasIndex = node.Value;

                xlWorkSheetNode.Cells[xlRow, 1] = etabsUniqueName;
                xlWorkSheetNode.Cells[xlRow, 2] = midasIndex;

                xlRow++;
            }

            //Creating frame list
            xlWorkSheetFrame.Cells[1, 1] = "Frame";

            xlWorkSheetFrame.Cells[2, 1] = "ETABS Unique Name";
            xlWorkSheetFrame.Cells[2, 2] = "MIDAS Index";

            xlRow = 3;
            foreach (KeyValuePair<string, string> node in nameIndexFrame)
            {
                string etabsUniqueName = node.Key;
                string midasIndex = node.Value;

                xlWorkSheetFrame.Cells[xlRow, 1] = etabsUniqueName;
                xlWorkSheetFrame.Cells[xlRow, 2] = midasIndex;

                xlRow++;
            }

            //Creating frame list
            xlWorkSheetArea.Cells[1, 1] = "Area";

            xlWorkSheetArea.Cells[2, 1] = "ETABS Unique Name";
            xlWorkSheetArea.Cells[2, 2] = "MIDAS Index";

            xlRow = 3;
            foreach (KeyValuePair<string, string> node in nameIndexArea)
            {
                string etabsUniqueName = node.Key;
                string midasIndex = node.Value;

                xlWorkSheetArea.Cells[xlRow, 1] = etabsUniqueName;
                xlWorkSheetArea.Cells[xlRow, 2] = midasIndex;

                xlRow++;
            }

            //Here saving the file in xlsx
            string excelPath = Sap.GetModelFilepath() + "test.xlsx";
            xlWorkBook.SaveAs(excelPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

        }

        public static void CreateCsvComparingEtabsMidas(cSapModel Sap, Dictionary<string, string> nameIndexNode, Dictionary<string, string> nameIndexFrame, Dictionary<string, string> nameIndexArea)
        {
            List<string> csvNode = new List<string>();
            csvNode.Add("Node");
            csvNode.Add("ETABS Unique Name" + "," + "MIDAS Index");
            foreach (KeyValuePair<string, string> node in nameIndexNode)
            {
                string etabsUniqueName = node.Key;
                string midasIndex = node.Value;

                csvNode.Add(etabsUniqueName + "," + midasIndex);
            }
            writeTextFile(Sap, csvNode, "csvNode.csv");

            List<string> csvFrame = new List<string>();
            csvFrame.Add("Frame");
            csvFrame.Add("ETABS Unique Name" + "," + "MIDAS Index");
            foreach (KeyValuePair<string, string> frame in nameIndexFrame)
            {
                string etabsUniqueName = frame.Key;
                string midasIndex = frame.Value;

                csvFrame.Add(etabsUniqueName + "," + midasIndex);
            }
            writeTextFile(Sap, csvFrame, "csvFrame.csv");

            List<string> csvArea = new List<string>();
            csvArea.Add("Area");
            csvArea.Add("ETABS Unique Name" + "," + "MIDAS Index");
            foreach (KeyValuePair<string, string> area in nameIndexArea)
            {
                string etabsUniqueName = area.Key;
                string midasIndex = area.Value;

                csvArea.Add(etabsUniqueName + "," + midasIndex);
            }
            writeTextFile(Sap, csvArea, "csvArea.csv");
        }

        private static double adjustAreaLocalAxisBasedOnEtabs(src.HelperEtabs.Shell area, List<Dictionary<string, string>> tableNodeCoordinate)
        {
            double localAxis = 0;
            double angleFromGlobalX = 0;
            List<src.HelperEtabs.Point3D> point3DList = new List<src.HelperEtabs.Point3D>();

            foreach (string point in area.Points)
            {
                foreach(Dictionary<string, string> row in tableNodeCoordinate)
                {
                    if (row["UniqueName"] == point)
                    {
                        src.HelperEtabs.Point3D point3D = new src.HelperEtabs.Point3D(row["UniqueName"], Convert.ToDouble(row["X"]),
                            Convert.ToDouble(row["Y"]), Convert.ToDouble(row["Z"]));

                        point3DList.Add(point3D);
                    }
                }
            }

            //Check if area horizontal
            bool areaHorizontal = src.HelperMethod.IsAreaHorizontal(point3DList);

            if (areaHorizontal)
            {
                //ETABS area X axis is following global X axis if area is horizontal
                //For 3 nodes area, MIDAS area X axis is following vector of point 1 to point 2
                //For 4 nodes area, MIDAS area X axis is following bisecting angle diagonal connection point 1 to 3 and 4 to 2
                //Originally want to split for 3 or 4 nodes area, but dont understand yet. Current method seems ok
                if (point3DList.Count == 3 || point3DList.Count == 4)
                {
                    //If 3 nodes area, take vector from point 1 to point2
                    double point1X = point3DList[0].XCoordinate;
                    double point1Y = point3DList[0].YCoordinate;
                    double point2X = point3DList[1].XCoordinate;
                    double point2Y = point3DList[1].YCoordinate;


                    //If point 1 X and point 2 X is equal then angle = 0
                    if (point1X != point2X && point1Y != point2Y)
                    {
                        double angleFromHorizontal = Math.Atan((point2Y - point1Y) / (point2X - point1X)) * 180 / Math.PI;

                        if (point2Y > point1Y && point2X > point1X)
                        {
                            //In quadrant 1
                            angleFromGlobalX = angleFromHorizontal;
                        }
                        else if (point2Y > point1Y && point2X < point1X)
                        {
                            //In quadrant 2
                            angleFromGlobalX = 180 + angleFromHorizontal;
                        }
                        else if (point2Y < point1Y && point2X < point1X)
                        {
                            //In quadrant 3
                            angleFromGlobalX = 180 + angleFromHorizontal;
                        }
                        else if (point2Y < point1Y && point2X > point1X)
                        {
                            //In quadrant 4
                            angleFromGlobalX = 360 + angleFromHorizontal;
                        }
                    }
                    else if (point1X == point2X)
                    {
                        if (point2Y > point1Y)
                        {
                            angleFromGlobalX = 90;
                        }
                        else
                        {
                            angleFromGlobalX = 360 - 90;
                        }
                    }
                    else if (point1Y == point2Y)
                    {
                        if (point2X > point1X)
                        {
                            angleFromGlobalX = 0;
                        }
                        else
                        {
                            angleFromGlobalX = 180 - 0;
                        }
                    }
                }
                /* Cant understand MIDAS correctly yet
                else if (point3DList.Count == 4)
                {
                    //If 4 nodes area, MIDAS area X axis is following bisecting angle diagonal connection point 1 to 3 and 4 to 2
                    double point1X = point3DList[0].XCoordinate;
                    double point1Y = point3DList[0].YCoordinate;
                    double point2X = point3DList[1].XCoordinate;
                    double point2Y = point3DList[1].YCoordinate;
                    double point3X = point3DList[2].XCoordinate;
                    double point3Y = point3DList[2].YCoordinate;
                    double point4X = point3DList[3].XCoordinate;
                    double point4Y = point3DList[3].YCoordinate;

                    double angleFromGlobalXPoint13 = 0;
                    double angleFromGlobalXPoint42 = 0;

                    //Do for point 1 and 3
                    if (point1X != point3X && point1Y != point3Y)
                    {
                        double angleFromHorizontal = Math.Atan((point3Y - point1Y) / (point3X - point1X)) * 180 / Math.PI;

                        if (point3Y > point1Y && point3X > point1X)
                        {
                            //In quadrant 1
                            angleFromGlobalXPoint13 = angleFromHorizontal;
                        }
                        else if (point3Y > point1Y && point3X < point1X)
                        {
                            //In quadrant 2
                            angleFromGlobalXPoint13 = 180 + angleFromHorizontal;
                        }
                        else if (point3Y < point1Y && point3X < point1X)
                        {
                            //In quadrant 3
                            angleFromGlobalXPoint13 = 180 + angleFromHorizontal;
                        }
                        else if (point3Y < point1Y && point3X > point1X)
                        {
                            //In quadrant 4
                            angleFromGlobalXPoint13 = 360 + angleFromHorizontal;
                        }
                    }
                    else if (point1X == point3X)
                    {
                        if (point3Y > point1Y)
                        {
                            angleFromGlobalXPoint13 = 90;
                        }
                        else
                        {
                            angleFromGlobalXPoint13 = 360 - 90;
                        }
                    }
                    else if (point1Y == point3Y)
                    {
                        if (point3X > point1X)
                        {
                            angleFromGlobalXPoint13 = 0;
                        }
                        else
                        {
                            angleFromGlobalXPoint13 = 180 - 0;
                        }
                    }

                    //Do for point 4 and 2
                    if (point4X != point2X && point4Y != point2Y)
                    {
                        double angleFromHorizontal = Math.Atan((point2Y - point4Y) / (point2X - point4X)) * 180 / Math.PI;

                        if (point2Y > point4Y && point2X > point4X)
                        {
                            //In quadrant 1
                            angleFromGlobalXPoint42 = angleFromHorizontal;
                        }
                        else if (point2Y > point4Y && point2X < point4X)
                        {
                            //In quadrant 2
                            angleFromGlobalXPoint42 = 180 + angleFromHorizontal;
                        }
                        else if (point2Y < point4Y && point2X < point4X)
                        {
                            //In quadrant 3
                            angleFromGlobalXPoint42 = 180 + angleFromHorizontal;
                        }
                        else if (point2Y < point4Y && point2X > point4X)
                        {
                            //In quadrant 4
                            angleFromGlobalXPoint42 = 360 + angleFromHorizontal;
                        }
                    }
                    else if (point4X == point2X)
                    {
                        if (point2Y > point4Y)
                        {
                            angleFromGlobalXPoint42 = 90;
                        }
                        else
                        {
                            angleFromGlobalXPoint42 = 360 - 90;
                        }
                    }
                    else if (point4Y == point2Y)
                    {
                        if (point2X > point4X)
                        {
                            angleFromGlobalXPoint42 = 0;
                        }
                        else
                        {
                            angleFromGlobalXPoint42 = 180 - 0;
                        }
                    }

                    //Get the bisecting angle
                    if (angleFromGlobalXPoint13 > angleFromGlobalXPoint42)
                    {
                        double bisectingAngle = (angleFromGlobalXPoint13 - angleFromGlobalXPoint42)/2;
                        angleFromGlobalX = angleFromGlobalXPoint13 + bisectingAngle;
                    }
                    else
                    {
                        double bisectingAngle = (angleFromGlobalXPoint42 - angleFromGlobalXPoint13) / 2;
                        angleFromGlobalX = angleFromGlobalXPoint42 + bisectingAngle;
                    }
                }
                */

            }
            else
            {
                //Dont know yet if area is not horizontal
            }

            localAxis = angleFromGlobalX * -1; //Reverse of this
            return localAxis;
        }

        private static string checkIfPlateAlreadyRelease(string newPlateRelease, string oldPlateRelease)
        {
            string updatedNewPlateRelease = "";

            //For each one that has been released, need to be kept released
            int releaseIndex = 0;
            foreach (char release in oldPlateRelease)
            {
                if (release == '1')
                {
                    updatedNewPlateRelease += '1';
                }
                else
                {
                    updatedNewPlateRelease += newPlateRelease[releaseIndex];
                }
                releaseIndex++;
            }

            return updatedNewPlateRelease;
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
