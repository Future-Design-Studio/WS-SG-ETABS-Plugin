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

namespace WS_SG_ETABS_Plugin.src.carbon
{
    internal class FunctionCarbon
    {

        private static int Ret;

        #region Class

        public class concreteEmissionData
        {
            public string ConcreteGrade { get; set; }

            //Emission in kgCO2e/kg
            public double Emission { get; set; }

            public concreteEmissionData(string concreteGrade, double emission)
            {
                ConcreteGrade = concreteGrade;
                Emission = emission;
            }
        }

        #endregion

        #region Data source

        public static List<concreteEmissionData> concreteEmissionDataList = new List<concreteEmissionData>()
        {
            new concreteEmissionData("C20/25 standard", 0.121),
            new concreteEmissionData("C20/25 15%PFA", 0.111),
            new concreteEmissionData("C20/25 30%PFA", 0.099)
        };

        public static double rebarEmission = 2.620;
        public static double steelSectionsEmission = 2.630;


        #endregion

        #region Method

        /// <summary>
        /// Check if the required tables exist
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>Return true if all required tables exist</returns>
        public static bool CheckTableExistCarbon(cSapModel Sap)
        {
            //Check if the table exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add("Material List by Section Property");
            tableKeys.Add("Frame Section Property Definitions - Summary");
            tableKeys.Add("Area Section Property Definitions - Summary");
            tableKeys.Add("Material Properties - General");

            List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForDisplay(Sap, tableKeys);

            if (tableKeysNotExist.Count > 0)
            {
                foreach (string tableKeyNotExist in tableKeysNotExist)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKeyNotExist);
                }

                return false;
            }
            /*
            test.Test.PrintTableDisplay(Sap, "Frame Section Property Definitions - Summary");
            test.Test.PrintTableDisplay(Sap, "Material Properties - General");*/

            return true;
        }




        /// <summary>
        /// Calculate carbon for each object type
        /// </summary>
        /// <param name="Sap"></param>
        /// <param name="inputCarbon">Dictionary with data for each data type</param>
        /// <returns>
        /// isError: bool if unable to data from table
        /// outputEachObjectTypeCarbon: dictionary with carbon data for each object type
        /// outputSumCarbon: sum total carbon
        /// </returns>
        public static (bool, Dictionary <string, double>, double) CalculateCarbon(cSapModel Sap, Dictionary<string, string> inputCarbon, 
            List<string> listChoosenGroup)
        {
            Dictionary <string, double> outputEachObjectTypeCarbon = new Dictionary<string, double>();
            double outputSumCarbon = 0;
            bool isError = false;
            bool showWarning = true;

            //Deconstruct input
            double columnConcreteEmission = Convert.ToDouble(inputCarbon["Column Concrete Emission"]);
            double columnRebarRatio = Convert.ToDouble(inputCarbon["Column Rebar Ratio"]);
            double beamConcreteEmission = Convert.ToDouble(inputCarbon["Beam Concrete Emission"]);
            double beamRebarRatio = Convert.ToDouble(inputCarbon["Beam Rebar Ratio"]);
            double braceConcreteEmission  = Convert.ToDouble(inputCarbon["Brace Concrete Emission"]);
            double braceRebarRatio = Convert.ToDouble(inputCarbon["Brace Rebar Ratio"]);
            double floorConcreteEmission = Convert.ToDouble(inputCarbon["Floor Concrete Emission"]);
            double floorRebarRatio = Convert.ToDouble(inputCarbon["Floor Rebar Ratio"]);
            double wallConcreteEmission = Convert.ToDouble(inputCarbon["Wall Concrete Emission"]);
            double wallRebarRatio = Convert.ToDouble(inputCarbon["Wall Rebar Ratio"]);

            //Open required tables
            src.HelperEtabs.DatabaseTable materialListBySectProp = null;
            src.HelperEtabs.DatabaseTable frameSectProp = null;
            src.HelperEtabs.DatabaseTable areaSectProp = null;
            src.HelperEtabs.DatabaseTable materialProp = null;

            string tableKey = "Material List by Section Property";
            (isError, materialListBySectProp) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey, 
                listChoosenGroup, showWarning);

            if (isError) { return (isError, null, 0); }

            tableKey = "Frame Section Property Definitions - Summary";
            (isError, frameSectProp) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            if (isError) { return (isError, null, 0); }

            tableKey = "Area Section Property Definitions - Summary";
            (isError, areaSectProp) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            if (isError) { return (isError, null, 0); }

            tableKey = "Material Properties - General";
            (isError, materialProp) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            if (isError) { return (isError, null, 0); }


            //Gather carbon emission for each object type
            double totalCarbonColumn = CalculateSingleObjectTypeCarbon(materialListBySectProp, frameSectProp, areaSectProp, materialProp,
                "Column", columnConcreteEmission, columnRebarRatio);

            outputSumCarbon += totalCarbonColumn;

            double totalCarbonBeam = CalculateSingleObjectTypeCarbon(materialListBySectProp, frameSectProp, areaSectProp, materialProp,
                "Beam", beamConcreteEmission, beamRebarRatio);

            outputSumCarbon += totalCarbonBeam;

            double totalCarbonBrace = CalculateSingleObjectTypeCarbon(materialListBySectProp, frameSectProp, areaSectProp, materialProp,
                "Brace", braceConcreteEmission, braceRebarRatio);

            outputSumCarbon += totalCarbonBrace;

            double totalCarbonFloor = CalculateSingleObjectTypeCarbon(materialListBySectProp, frameSectProp, areaSectProp, materialProp,
                "Floor", floorConcreteEmission, floorRebarRatio);

            outputSumCarbon += totalCarbonFloor;

            double totalCarbonWall = CalculateSingleObjectTypeCarbon(materialListBySectProp, frameSectProp, areaSectProp, materialProp,
                "Wall", wallConcreteEmission, wallRebarRatio);

            outputSumCarbon += totalCarbonWall;

            //Add output to dictionary
            outputEachObjectTypeCarbon.Add("Carbon From Column", totalCarbonColumn);
            outputEachObjectTypeCarbon.Add("Carbon From Beam", totalCarbonBeam);
            outputEachObjectTypeCarbon.Add("Carbon From Brace", totalCarbonBrace);
            outputEachObjectTypeCarbon.Add("Carbon From Floor", totalCarbonFloor);
            outputEachObjectTypeCarbon.Add("Carbon From Wall", totalCarbonWall);

            return (isError, outputEachObjectTypeCarbon, outputSumCarbon);


            /*            System.Diagnostics.Debug.WriteLine("col " + totalCarbonColumn);
                        System.Diagnostics.Debug.WriteLine("beam " + totalCarbonBeam);
                        System.Diagnostics.Debug.WriteLine("brace " + totalCarbonBrace);
                        System.Diagnostics.Debug.WriteLine("slab " + totalCarbonSlab);
                        System.Diagnostics.Debug.WriteLine("wall " + totalCarbonWall);*/


            //Material List by Section Property
            /*     Section: S300 - meshed
                    ObjectType: Floor
                    NumPieces: 
                    Length:
                    Weight: 2683042.99
                    NumStuds:*/

            //Frame Section Property Definitions - Summary
            /*  Name: C3 - 350x350
                Material: 4000Psi
                Shape: Concrete Rectangular
                Color: Red
                Area: 122500
                J: 2113380208.3
                I33: 1250520833.3
                I22: 1250520833.3
                As2: 102083.3
                As3: 102083.3
                S33Pos: 7145833.3
                S33Neg: 7145833.3
                S22Pos: 7145833.3
                S22Neg: 7145833.3
                Z33: 10718750
                Z22: 10718750
                R33: 101
                R22: 101
                CGOffset3: 0
                CGOffset2: 0
                PNAOffset3: 0
                PNAOffset2: 0
                AMod: 1
                A2Mod: 1
                A3Mod: 1
                JMod: 1
                I3Mod: 1
                I2Mod: 1
                MMod: 1
                WMod: 1*/



        }

        /// <summary>
        /// Calculate carbon for each object type
        /// </summary>
        /// <param name="materialListBySectProp"></param>
        /// <param name="frameSectProp"></param>
        /// <param name="areaSectProp"></param>
        /// <param name="materialProp"></param>
        /// <param name="objectType">Specified object type</param>
        /// <param name="concreteEmission"></param>
        /// <param name="rebarRatio"></param>
        /// <returns>Total carbon for one object type</returns>
        public static double CalculateSingleObjectTypeCarbon(src.HelperEtabs.DatabaseTable materialListBySectProp,
            src.HelperEtabs.DatabaseTable frameSectProp, src.HelperEtabs.DatabaseTable areaSectProp, 
            src.HelperEtabs.DatabaseTable materialProp, string objectType, double concreteEmission, double rebarRatio)
        {
            double totalCarbonSingle = 0;

            //Filter only specified object type to be evaluated
            src.HelperEtabs.DatabaseTable filteredMaterialListBySectProp = src.HelperMethod.
                FilterEtabsDatabase(materialListBySectProp, "ObjectType", objectType);

            //Go through each row after filtering table
            foreach (Dictionary<string, string> row in filteredMaterialListBySectProp.TableInList)
            {
                //Take weight in kg
                double weight = Convert.ToDouble(row["Weight"]) * 1000 / 9.81;

                //Take value of column section and use it to filter frame section property
                src.HelperEtabs.DatabaseTable filteredFrameSectProp = src.HelperMethod.
                    FilterEtabsDatabase(frameSectProp, "Name", row["Section"]);

                string section = "";
                string materialName = "";

                //Check if section property exist in frame section proerty
                if (filteredFrameSectProp.TableInList.Count > 0)
                {
                    //Take frame section and material name
                    section = filteredFrameSectProp.TableInList[0]["Name"];
                    materialName = filteredFrameSectProp.TableInList[0]["Material"];
                }
                //If property does not exist in frame section proerty, check in area section proerty
                else
                {
                    //Take value of column section and use it to filter frame section property
                    src.HelperEtabs.DatabaseTable filteredAreaSectProp = src.HelperMethod.
                        FilterEtabsDatabase(areaSectProp, "Name", row["Section"]);

                    //Take area section
                    section = filteredAreaSectProp.TableInList[0]["Name"];
                    materialName = filteredAreaSectProp.TableInList[0]["Material"];
                }


                //Take value of material (only 1 match) and use it to filter material property
                src.HelperEtabs.DatabaseTable filteredMaterialProp = src.HelperMethod.
                    FilterEtabsDatabase(materialProp, "Material", materialName);

                //Take material type
                string materialType = filteredMaterialProp.TableInList[0]["Type"];

                if (materialType == "Concrete")
                {
                    double concreteVolume = weight * 9.81 / 1000 / 25; //25 kN/m3
                    double carbonFromConcrete = weight * concreteEmission;
                    double carbonFromRebar = concreteVolume * rebarRatio * src.carbon.FunctionCarbon.rebarEmission;

                    totalCarbonSingle = totalCarbonSingle + carbonFromConcrete + carbonFromRebar;
                }
                else if (materialType == "Steel")
                {
                    double carbonFromSteelSections = weight * src.carbon.FunctionCarbon.steelSectionsEmission;
                    totalCarbonSingle += carbonFromSteelSections;
                }
                else
                {
                    MessageBox.Show("Unrecognized material type " + materialType + " from " + section + ". Result not valid");
                    totalCarbonSingle -= 9 * 10 ^ 99;
                }

            }


            return totalCarbonSingle;
        }

        #endregion
    }
}
