  using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.rc_col_schedule
{
    internal class FunctionRCCSched
    {

        private static int Ret;

        public static bool GetRCColumnSchedule(cSapModel Sap, cPluginCallback Plugin, string fileName, string filePath, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing,
            double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int maxNoLayer, int[] rebarDiameterList, int[] shearLinksDiameterList,
            List<string> listChoosenGroupName)
        {
            bool isError = false;

            //Sap.Analyze();

            //Run RC design if result is not available
            bool resultsAvailable = Sap.DesignConcrete.GetResultsAvailable();
            if (!resultsAvailable)
            {
                Ret = Sap.DesignConcrete.StartDesign();
                if (Ret == 1)
                {
                    isError = true;
                    MessageBox.Show("Cannot start RC design. Please run FEM analysis first!");
                    return isError;
                }
            }

            //Get current unit
            eForce forceUnits = new eForce();
            eLength lengthUnits = new eLength();
            eTemperature temperatureUnits = new eTemperature();
            Ret = Sap.GetPresentUnits_2(ref forceUnits, ref lengthUnits, ref temperatureUnits);

            //Set unit to N mm
            src.HelperEtabs.SetOutputUnitTo_N_mm_C(Sap);

            bool tableExist = CheckTableExistRCCSched(Sap);

            if (!tableExist)
            {
                isError = true;
                return isError;
            }

            //Get RC column design result
            src.HelperEtabs.DatabaseTable rcColumnDesignSummary = new src.HelperEtabs.DatabaseTable();
            (isError, rcColumnDesignSummary) = GetAllRcColumnDesignResult(Sap);

            if (isError) { return isError; }

            //Get list of columns
            List<src.HelperEtabs.RcColumn> rcColumnList = new List<src.HelperEtabs.RcColumn>();
            (isError, rcColumnList) = GetRcColumnListSpecificGroup(Sap, rcColumnDesignSummary, listChoosenGroupName);

            if (isError) { return isError; }

            //Get dimension
            GetRcColumnDimensionAndMaterial(Sap, rcColumnDesignSummary, rcColumnList, cover);

            //Get rebar required from etabs design
            GetRcColumnRebarArea(Sap, rcColumnDesignSummary, rcColumnList);
                        
            //Convert area to rebar
            DesignRcColumnRebar(rcColumnList, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, rebarDiameterList, shearLinksDiameterList, maxNoLayer);

            //Get d from rebar
            CalculateDFromRebar(rcColumnList);

            /*
            //Calculate As provided
            CalculateAsProv(rcBeamList);

            //Calculate Av provided
            CalculateAvProv(rcBeamList);
            */

            //Compile data to 2d array
            var result = CompileResutRcBeamToArray(rcColumnList);
            
            //Write to excel
            WriteResultRcBeamToExcel(result, fileName, filePath);
           
            //Set back to original units
            src.HelperEtabs.SetOutputUnit(Sap, forceUnits, lengthUnits, temperatureUnits);

            return isError;
        }

        /// <summary>
        /// Check if the required tables exist
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>Return true if all required tables exist</returns>
        public static bool CheckTableExistRCCSched(cSapModel Sap)
        {
            string designCode = "";

            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            //Check if the table exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add("Concrete Column Design Summary - " + designCode);

            List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForDisplay(Sap, tableKeys);

            if (tableKeysNotExist.Count > 0)
            {
                foreach (string tableKeyNotExist in tableKeysNotExist)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKeyNotExist);
                }

                return false;
            }

            return true;
        }

        public static (bool, src.HelperEtabs.DatabaseTable) GetAllRcColumnDesignResult(cSapModel Sap)
        {
            //Get RC design result table
            src.HelperEtabs.DatabaseTable rcColumnDesignSummary = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string designCode = "";
            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            if (designCode != "Eurocode 2-2004")
            {
                isError = true;
                MessageBox.Show("Only support Eurocode 2-2004!");
                return (isError, rcColumnDesignSummary);
            }

            string tableKey = "Concrete Column Design Summary - " + designCode;

            (isError, rcColumnDesignSummary) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            return (isError, rcColumnDesignSummary);
        }

        public static (bool, List<src.HelperEtabs.RcColumn>) GetRcColumnListSpecificGroup(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary, List<string> listChoosenGroupName)
        {
            bool isError = false;

            //To return
            List<src.HelperEtabs.RcColumn> rcColumnList = new List<HelperEtabs.RcColumn>();

            //Get group database table
            src.HelperEtabs.DatabaseTable groupDatabaseTable = new HelperEtabs.DatabaseTable();
            (isError, groupDatabaseTable) = src.HelperEtabs.GetTableDisplay(Sap, "Group Assignments", false);

            //Filter group to only include line, to improve performance
            (isError, groupDatabaseTable) = src.HelperEtabs.FilterDatabaseTable(groupDatabaseTable, "ObjectType", "Line");
            if (isError) { return (isError, rcColumnList); }

            foreach (string eachChoosenGroupName in listChoosenGroupName)
            {
                src.HelperEtabs.DatabaseTable rcColumnDesignSummaryFiltered = new HelperEtabs.DatabaseTable();
                (isError, rcColumnDesignSummaryFiltered) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(rcBeamDesignSummary, groupDatabaseTable, eachChoosenGroupName);

                if (isError)
                {
                    return (isError, rcColumnList);
                }

                List<src.HelperEtabs.RcColumn> rcColumnListEachGroup = GetRcColumnList(rcColumnDesignSummaryFiltered);

                rcColumnList.AddRange(rcColumnListEachGroup);
            }

            return (isError, rcColumnList);
        }

        public static List<src.HelperEtabs.RcColumn> GetRcColumnList(src.HelperEtabs.DatabaseTable rcColumnDesignSummary)
        {
            //Get table
            List<Dictionary<string, string>> rcColumnDesignSummaryTable = rcColumnDesignSummary.TableInList;

            List<src.HelperEtabs.RcColumn> rcColumnList = new List<HelperEtabs.RcColumn>();

            //Get list of unique name
            List<string> uniqueNameAllList = new List<string>();

            foreach (Dictionary<string, string> row in rcColumnDesignSummaryTable)
            {
                uniqueNameAllList.Add(row["UniqueName"]);
            }
            List<string> uniqueNameList = uniqueNameAllList.Distinct().ToList();

            //Create frame list
            foreach (string uniqueName in uniqueNameList)
            {
                rcColumnList.Add(new src.HelperEtabs.RcColumn(uniqueName));
            }

            return rcColumnList;
        }

        public static void GetRcColumnDimensionAndMaterial(cSapModel Sap, src.HelperEtabs.DatabaseTable rcColumnDesignSummary, List<src.HelperEtabs.RcColumn> rcColumnList, double cover)
        {
            //Get table
            List<Dictionary<string, string>> rcColumnDesignSummaryTable = rcColumnDesignSummary.TableInList;

            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                foreach (Dictionary<string, string> row in rcColumnDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcColumn.UniqueName)
                    {
                        string sectionPropName = row["DesignSect"];
                        double width = 0;
                        double height = 0;
                        double diameter = 0;
                        string shape = "";
                        double coverEtabs = 0;
                        string concreteMatProp = "";
                        string rebarMatProp = "";

                        double rebarDiameterEtabs = 0;
                        double shearLinksDiameterEtabs = 0;

                        (width, height, diameter, shape, concreteMatProp) = src.HelperEtabs.GetRcColumnDimensionAndConcreteMaterial(Sap, sectionPropName);

                        rcColumn.Shape = shape;
                        rcColumn.Width = width;
                        rcColumn.Height = height;
                        rcColumn.Diameter = diameter;

                        (coverEtabs, rebarDiameterEtabs, shearLinksDiameterEtabs, rebarMatProp) = src.HelperEtabs.GetRcColumnCoverAndRebarMaterial(Sap, sectionPropName);

                        //In etabs cover for column is to edge of shear links
                        rcColumn.CoverEtabs = coverEtabs;
                        rcColumn.Cover = cover;

                        double d = height - coverEtabs - shearLinksDiameterEtabs - 0.5 * rebarDiameterEtabs;
                        rcColumn.d = d;

                        //Get fyk
                        double fyk = 0;
                        fyk = src.HelperEtabs.GetRebarFyk(Sap, rebarMatProp);
                        rcColumn.Fyk = fyk;

                        //Get fck
                        double fck = 0;
                        fck = src.HelperEtabs.GetConcreteFck(Sap, concreteMatProp);
                        rcColumn.Fck = fck;

                        break;
                    }
                }
            }
        }

        public static void GetRcBeamOffset(cSapModel Sap, List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            src.HelperEtabs.DatabaseTable rcBeamOffset = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string tableKey = "Frame Assignments - End Length Offsets";
            (isError, rcBeamOffset) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            List<Dictionary<string, string>> rcBeamOffsetTable = rcBeamOffset.TableInList;

            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                //Going through table to find the same unique name
                foreach (Dictionary<string, string> row in rcBeamOffsetTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        rcBeam.OffsetI = Convert.ToDouble(row["OffsetI"]);
                        rcBeam.OffsetJ = Convert.ToDouble(row["OffsetJ"]);

                        break;
                    }
                }
            }
        }

        public static void GetRcColumnRebarArea(cSapModel Sap, src.HelperEtabs.DatabaseTable rcColumnDesignSummary, List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            //Get table
            List<Dictionary<string, string>> rcColumnDesignSummaryTable = rcColumnDesignSummary.TableInList;

            //Getting length for each column
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                bool firstRun = true;
                double length = 0;

                foreach (Dictionary<string, string> row in rcColumnDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcColumn.UniqueName)
                    {
                        if (firstRun)
                        {
                            length = Convert.ToDouble(row["Station"]);
                            firstRun = false;
                        }
                        else
                        {
                            if (Convert.ToDouble(row["Station"]) > length)
                            {
                                length = Convert.ToDouble(row["Station"]);
                            }
                        }
                    }
                }
                rcColumn.Length = length;
            }

            //Getting rebar area for each beam
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                double As = 0;
                double avMajor = 0;
                double avMinor = 0;

                bool first = true;

                foreach (Dictionary<string, string> row in rcColumnDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcColumn.UniqueName)
                    {
                        if (first)
                        {
                            As = (Convert.ToDouble(row["As"]));
                            avMajor = (Convert.ToDouble(row["VMajRebar"]));
                            avMinor = (Convert.ToDouble(row["VMinRebar"]));

                            first = false;
                        }
                        else
                        {
                            if (Convert.ToDouble(row["As"]) > As)
                            {
                                As = (Convert.ToDouble(row["As"]));
                            }

                            if (Convert.ToDouble(row["VMajRebar"]) > avMajor)
                            {
                                avMajor = (Convert.ToDouble(row["VMajRebar"]));
                            }

                            if (Convert.ToDouble(row["VMinRebar"]) > avMinor)
                            {
                                avMinor = (Convert.ToDouble(row["VMinRebar"]));
                            }
                        }
                    }
                }

                rcColumn.As = As;
                rcColumn.AvMajor = avMajor;
                rcColumn.AvMinor = avMinor;

                /*
                //Check againts minimum rebar
                CompareToMinimumRcBeamRebar(rcBeam);
                */

            }
        }
        
        public static void DesignRcColumnRebar(List<src.HelperEtabs.RcColumn> rcColumnList, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, double minShearLinksSpacing, double shearLinksSpacingStep,
            int[] rebarDiameterList, int[] shearLinksDiameterList, int maxNoLayer)
        {
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                double cover = rcColumn.Cover;

                bool warningRebarDiameterTooBigShown = false;

                double As = rcColumn.As;
                double avMajor = rcColumn.AvMajor;
                double avMinor = rcColumn.AvMinor;
                double width = rcColumn.Width;
                double height = rcColumn.Height;

                double numRebarLayer = 0;
                List<int> numRebarPerLayer = new List<int>();
                List<int> rebarDiameter = new List<int>();
                string asEvaluationComment = "";
                double numShearLinksLeg = 0;
                double shearLinksDiameter = 0;
                double shearLinksSpacing = 0;
                string avEvaluationComment = "";

                double avReq = Math.Max(avMajor, avMinor);

                (numRebarLayer, numRebarPerLayer, rebarDiameter, asEvaluationComment, numShearLinksLeg, shearLinksDiameter, shearLinksSpacing,
                    avEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcColumnAreaToRebar(
                    As, avReq, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                    rebarDiameterList, maxNoLayer, rcColumn.UniqueName, warningRebarDiameterTooBigShown);
                
                rcColumn.Rebar = "";
                for (int layerIndex = 0; layerIndex < rebarDiameter.Count(); layerIndex++)
                {
                    rcColumn.Rebar += numRebarPerLayer[layerIndex] + "H" + rebarDiameter[layerIndex] + "+";
                }
                rcColumn.Rebar = rcColumn.Rebar.Remove(rcColumn.Rebar.Length - 1, 1);
                rcColumn.RebarDiameterList = rebarDiameter;
                rcColumn.NoRebarPerLayer = numRebarPerLayer;

                rcColumn.ShearLinks = numShearLinksLeg + "H" + shearLinksDiameter + "-" + shearLinksSpacing;
                rcColumn.NumShearLinksLeg = numShearLinksLeg;
                rcColumn.ShearLinksDiameter = shearLinksDiameter;
                rcColumn.ShearLinksSpacing = shearLinksSpacing;

                //Gathering evaluation comment
                if (asEvaluationComment != "")
                {
                    rcColumn.AsEvaluationComment = "Num layer req > max num layer";
                }

                if (avEvaluationComment != "")
                {
                    rcColumn.AvEvaluationComment = "Shear links spc < min shear links spc";
                }

                /*
                double leftBottomAs = rcBeam.LeftBottomAs;
                double leftBottomAv = rcBeam.LeftAv;
                width = rcBeam.Width;
                height = rcBeam.Height;
                double leftBottomNoRebarLayer = 0;
                List<int> leftBottomNoRebarPerLayer = new List<int>();
                List<int> leftBottomRebarDiameter = new List<int>();
                string leftBottomAsEvaluationComment = "";
                double leftBottomNoShearLinksLeg = 0;
                double leftBottomshearLinksDiameter = 0;
                double leftBottomShearLinksSpacing = 0;
                string leftBottomAvEvaluationComment = "";
                (leftBottomNoRebarLayer, leftBottomNoRebarPerLayer, leftBottomRebarDiameter, leftBottomAsEvaluationComment, leftBottomNoShearLinksLeg, leftBottomshearLinksDiameter, leftBottomShearLinksSpacing,
                    leftBottomAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                    leftBottomAs, leftBottomAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                    rebarDiameterList, maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);

                rcBeam.LeftBottomRebar = "";
                for (int layerIndex = 0; layerIndex < leftBottomRebarDiameter.Count(); layerIndex++)
                {
                    rcBeam.LeftBottomRebar += leftBottomNoRebarPerLayer[layerIndex] + "H" + leftBottomRebarDiameter[layerIndex] + "+";
                }
                rcBeam.LeftBottomRebar = rcBeam.LeftBottomRebar.Remove(rcBeam.LeftBottomRebar.Length - 1, 1);
                rcBeam.LeftBottomRebarDiameterList = leftBottomRebarDiameter;
                rcBeam.LeftBottomNoRebarPerLayer = leftBottomNoRebarPerLayer;

                rcBeam.LeftShearLinks = leftTopNoShearLinksLeg + "H" + leftTopshearLinksDiameter + "-" + leftTopShearLinksSpacing;
                rcBeam.LeftNoShearLinksLeg = leftTopNoShearLinksLeg;
                rcBeam.LeftShearLinksDiameter = leftTopshearLinksDiameter;
                rcBeam.LeftShearLinksSpacing = leftTopShearLinksSpacing;

                string midTopAsEvaluationComment = "";
                string midTopAvEvaluationComment = "";
                string midBottomAsEvaluationComment = "";
                string midBottomAvEvaluationComment = "";
                if (rcBeam.BeamType == "Simply supported")
                {
                    double midTopAs = rcBeam.MidTopAs;
                    double midTopAv = rcBeam.MidAv;
                    width = rcBeam.Width;
                    height = rcBeam.Height;
                    double midTopNoRebarLayer = 0;
                    List<int> midTopNoRebarPerLayer = new List<int>();
                    List<int> midTopRebarDiameter = new List<int>();
                    double midTopNoShearLinksLeg = 0;
                    double midTopshearLinksDiameter = 0;
                    double midTopShearLinksSpacing = 0;
                    (midTopNoRebarLayer, midTopNoRebarPerLayer, midTopRebarDiameter, midTopAsEvaluationComment, midTopNoShearLinksLeg, midTopshearLinksDiameter, midTopShearLinksSpacing,
                        midTopAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                        midTopAs, midTopAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                        rebarDiameterList, maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);

                    rcBeam.MidTopRebar = "";
                    for (int layerIndex = 0; layerIndex < midTopRebarDiameter.Count(); layerIndex++)
                    {
                        rcBeam.MidTopRebar += midTopNoRebarPerLayer[layerIndex] + "H" + midTopRebarDiameter[layerIndex] + "+";
                    }
                    rcBeam.MidTopRebar = rcBeam.MidTopRebar.Remove(rcBeam.MidTopRebar.Length - 1, 1);
                    rcBeam.MidTopRebarDiameterList = midTopRebarDiameter;
                    rcBeam.MidTopNoRebarPerLayer = midTopNoRebarPerLayer;

                    double midBottomAs = rcBeam.MidBottomAs;
                    double midBottomAv = rcBeam.MidAv;
                    width = rcBeam.Width;
                    height = rcBeam.Height;
                    double midBottomNoRebarLayer = 0;
                    List<int> midBottomNoRebarPerLayer = new List<int>();
                    List<int> midBottomRebarDiameter = new List<int>();
                    double midBottomNoShearLinksLeg = 0;
                    double midBottomshearLinksDiameter = 0;
                    double midBottomShearLinksSpacing = 0;
                    (midBottomNoRebarLayer, midBottomNoRebarPerLayer, midBottomRebarDiameter, midBottomAsEvaluationComment, midBottomNoShearLinksLeg, midBottomshearLinksDiameter, midBottomShearLinksSpacing,
                        midBottomAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                        midBottomAs, midBottomAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                        rebarDiameterList, maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);

                    rcBeam.MidBottomRebar = "";
                    for (int layerIndex = 0; layerIndex < midBottomRebarDiameter.Count(); layerIndex++)
                    {
                        rcBeam.MidBottomRebar += midBottomNoRebarPerLayer[layerIndex] + "H" + midBottomRebarDiameter[layerIndex] + "+";
                    }
                    rcBeam.MidBottomRebar = rcBeam.MidBottomRebar.Remove(rcBeam.MidBottomRebar.Length - 1, 1);
                    rcBeam.MidBottomRebarDiameterList = midBottomRebarDiameter;
                    rcBeam.MidBottomNoRebarPerLayer = midBottomNoRebarPerLayer;

                    rcBeam.MidShearLinks = midTopNoShearLinksLeg + "H" + midTopshearLinksDiameter + "-" + midTopShearLinksSpacing;
                    rcBeam.MidNoShearLinksLeg = midTopNoShearLinksLeg;
                    rcBeam.MidShearLinksDiameter = midTopshearLinksDiameter;
                    rcBeam.MidShearLinksSpacing = midTopShearLinksSpacing;
                }
                else
                {
                    rcBeam.MidTopRebar = "-";
                    rcBeam.MidBottomRebar = "-";
                    rcBeam.MidShearLinks = "-";
                }

                double rightTopAs = rcBeam.RightTopAs;
                double rightTopAv = rcBeam.RightAv;
                width = rcBeam.Width;
                height = rcBeam.Height;
                double rightTopNoRebarLayer = 0;
                List<int> rightTopNoRebarPerLayer = new List<int>();
                List<int> rightTopRebarDiameter = new List<int>();
                string rightTopAsEvaluationComment = "";
                double rightTopNoShearLinksLeg = 0;
                double rightTopshearLinksDiameter = 0;
                double rightTopShearLinksSpacing = 0;
                string rightTopAvEvaluationComment = "";
                (rightTopNoRebarLayer, rightTopNoRebarPerLayer, rightTopRebarDiameter, rightTopAsEvaluationComment, rightTopNoShearLinksLeg, rightTopshearLinksDiameter, rightTopShearLinksSpacing,
                    rightTopAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                    rightTopAs, rightTopAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList, rebarDiameterList,
                    maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);

                rcBeam.RightTopRebar = "";
                for (int layerIndex = 0; layerIndex < rightTopRebarDiameter.Count(); layerIndex++)
                {
                    rcBeam.RightTopRebar += rightTopNoRebarPerLayer[layerIndex] + "H" + rightTopRebarDiameter[layerIndex] + "+";
                }
                rcBeam.RightTopRebar = rcBeam.RightTopRebar.Remove(rcBeam.RightTopRebar.Length - 1, 1);
                rcBeam.RightTopRebarDiameterList = rightTopRebarDiameter;
                rcBeam.RightTopNoRebarPerLayer = rightTopNoRebarPerLayer;

                double rightBottomAs = rcBeam.RightBottomAs;
                double rightBottomAv = rcBeam.RightAv;
                width = rcBeam.Width;
                height = rcBeam.Height;
                double rightBottomNoRebarLayer = 0;
                List<int> rightBottomNoRebarPerLayer = new List<int>();
                List<int> rightBottomRebarDiameter = new List<int>();
                string rightBottomAsEvaluationComment = "";
                double rightBottomNoShearLinksLeg = 0;
                double rightBottomshearLinksDiameter = 0;
                double rightBottomShearLinksSpacing = 0;
                string rightBottomAvEvaluationComment = "";
                (rightBottomNoRebarLayer, rightBottomNoRebarPerLayer, rightBottomRebarDiameter, rightBottomAsEvaluationComment, rightBottomNoShearLinksLeg, rightBottomshearLinksDiameter, rightBottomShearLinksSpacing,
                    rightBottomAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                    rightBottomAs, rightBottomAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                    rebarDiameterList, maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);
                

                rcBeam.RightBottomRebar = "";
                for (int layerIndex = 0; layerIndex < rightBottomRebarDiameter.Count(); layerIndex++)
                {
                    rcBeam.RightBottomRebar += rightBottomNoRebarPerLayer[layerIndex] + "H" + rightBottomRebarDiameter[layerIndex] + "+";
                }
                rcBeam.RightBottomRebar = rcBeam.RightBottomRebar.Remove(rcBeam.RightBottomRebar.Length - 1, 1);
                rcBeam.RightBottomRebarDiameterList = rightBottomRebarDiameter;
                rcBeam.RightBottomNoRebarPerLayer = rightBottomNoRebarPerLayer;

                rcBeam.RightShearLinks = rightTopNoShearLinksLeg + "H" + rightTopshearLinksDiameter + "-" + rightTopShearLinksSpacing;
                rcBeam.RightNoShearLinksLeg = rightTopNoShearLinksLeg;
                rcBeam.RightShearLinksDiameter = rightTopshearLinksDiameter;
                rcBeam.RightShearLinksSpacing = rightTopShearLinksSpacing;
                */



            }
        }
        
        private static (double, List<int>, List<int>, string, double, double, double, string, bool) ConvertRcColumnAreaToRebar(double asReq, double avReq, double width, double height, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing,
            double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int[] shearLinksDiameterList, int[] rebarDiameterList, int maxNoLayer, string uniqueName, bool warningRebarDiameterTooBigShown)
        {
            int shearLinksDiameterIndex = 0;
            double shearLinksDiameter = 0;
            bool shearLinksSatisfied = false;
            int noShearLinksLeg = 0;
            double shearLinksSpacing = 0;

            List<int> rebarDiameterIndexPerLayer = new List<int>();
            List<int> rebarDiameterPerLayer = new List<int>();
            List<int> noRebarPerLayer = new List<int>();
            bool rebarSatisfied = false;
            int noRebarLayer = 0;
            int maxRebarDiameterInAllLayer = 0;

            int maxNoRebarPerLayer = 0;
            int minNoRebarPerLayer = 0;

            //Keep looping if not both satisfied
            while (shearLinksDiameterIndex <= shearLinksDiameterList.Count() - 1 && !shearLinksSatisfied)
            {
                rebarDiameterIndexPerLayer = new List<int>();
                rebarDiameterPerLayer = new List<int>();
                noRebarPerLayer = new List<int>();

                rebarSatisfied = false;
                noRebarLayer = 1;

                shearLinksDiameter = shearLinksDiameterList[shearLinksDiameterIndex];

                //Rebar design
                while (noRebarLayer <= maxNoLayer && !rebarSatisfied)
                {
                    rebarDiameterIndexPerLayer.Add(0);
                    rebarDiameterPerLayer.Add(0);
                    noRebarPerLayer.Add(0);

                    int currentLayer = noRebarLayer - 1;
                    int rebarDiameterIndexCurrentLayer = rebarDiameterIndexPerLayer[currentLayer];

                    while (rebarDiameterIndexCurrentLayer <= rebarDiameterList.Count() - 1 && !rebarSatisfied)
                    {

                        rebarDiameterPerLayer[currentLayer] = rebarDiameterList[rebarDiameterIndexCurrentLayer];
                        maxRebarDiameterInAllLayer = rebarDiameterPerLayer.Max();

                        //Calculating how many rebar per layer based on rebar spc preference
                        if (rebarSpcPreference == "clear")
                        {
                            double widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double widthNoRebar = Math.Floor(widthSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1) * 2;
                            double heightNoRebar = Math.Floor(heightSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1) * 2;
                            maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar + heightNoRebar - 4);
                        }
                        else if (rebarSpcPreference == "ctoc")
                        {
                            double widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double widthNoRebar = Math.Floor(widthSpace / (rebarSpc) + 1) * 2;
                            double heightNoRebar = Math.Floor(heightSpace / (rebarSpc) + 1) * 2;
                            maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar + heightNoRebar - 4);
                        }
                        else if (rebarSpcPreference == "auto")
                        {
                            rebarSpc = Math.Max(25, maxRebarDiameterInAllLayer);
                            double widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                            double widthNoRebar = Math.Floor(widthSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1) * 2;
                            double heightNoRebar = Math.Floor(heightSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1) * 2;
                            maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar + heightNoRebar - 4);
                        }
                        else if (rebarSpcPreference == "autoRamboll")
                        {
                            rebarSpc = Math.Max(25, maxRebarDiameterInAllLayer);
                            maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(width/100 * 2) + Math.Floor(height /100 * 2) - 4);
                        }

                        //Set no of rebar per layer to minimum
                        //For column number of max and min rebar per layer is set to be same
                        minNoRebarPerLayer = maxNoRebarPerLayer;
                        if (currentLayer == 0)
                        {
                            noRebarPerLayer[currentLayer] = minNoRebarPerLayer;
                        }
                        else
                        {
                            noRebarPerLayer[currentLayer] = Math.Max(0, minNoRebarPerLayer - 4);
                        }

                        //Show warning if number of rebar per layer can only fit 1, means the diameter is too big
                        if (maxNoRebarPerLayer == 1 && !warningRebarDiameterTooBigShown)
                        {
                            MessageBox.Show($"Warning! Beam width can only fits 1 rebar per layer for rebar! Rebar diameter {maxRebarDiameterInAllLayer} mm is too big for beam unique name {uniqueName}, reduce rebar size!");
                            warningRebarDiameterTooBigShown = true;
                        }

                        int noRebarCurrentLayer = noRebarPerLayer[currentLayer];

                        while (noRebarCurrentLayer <= maxNoRebarPerLayer && !rebarSatisfied)
                        {
                            //Evaluate if asReq is satisfied
                            double asFromCurrentRebar = 0;

                            //Gothrough each layer
                            for (int layerIndex = 0; layerIndex < rebarDiameterPerLayer.Count(); layerIndex++)
                            {
                                asFromCurrentRebar += 0.25 * Math.PI * Math.Pow(rebarDiameterPerLayer[layerIndex], 2) * noRebarPerLayer[layerIndex];
                            }

                            if (asFromCurrentRebar >= asReq && IsNoRebarLayerValid(noRebarPerLayer))
                            {
                                rebarSatisfied = true;
                            }

                            //Check
                            if (rebarSatisfied)
                            {
                                //do nothing
                            }
                            else if (noRebarCurrentLayer == maxNoRebarPerLayer)
                            {
                                break;
                            }
                            else
                            {
                                noRebarCurrentLayer += 2;
                                noRebarPerLayer[currentLayer] = noRebarCurrentLayer;
                            }
                        }

                        //Check
                        if (rebarSatisfied)
                        {
                            //do nothing
                        }
                        else if (rebarDiameterIndexCurrentLayer == rebarDiameterList.Count() - 1)
                        {
                            break;
                        }
                        else
                        {
                            rebarDiameterIndexCurrentLayer += 1;
                            rebarDiameterIndexPerLayer[currentLayer] = rebarDiameterIndexCurrentLayer;
                        }
                    }

                    //Check
                    if (rebarSatisfied)
                    {
                        //do nothing   
                    }
                    else if (noRebarLayer == maxNoLayer)
                    {
                        break;
                    }
                    else
                    {
                        noRebarLayer += 1;
                    }
                }
                /*
                //Calculate minimum number of shear links leg based on 150mm max distance compression rebar from confined rebar
                maxRebarDiameterInAllLayer = rebarDiameterPerLayer.Max();
                double cToCOuterMostRebar = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                double cToCRebar = cToCOuterMostRebar / (maxNoRebarPerLayer - 1);

                double distanceFromConfinedRebar = 0;
                double distanceFromConfinedRebarStart = 0;
                double distanceFromConfinedRebarEnd = cToCOuterMostRebar;

                int additionalShearLinksLeg = 0;
                for (int rebarHorizontalIndex = 0; rebarHorizontalIndex < maxNoRebarPerLayer; rebarHorizontalIndex++)
                {
                    distanceFromConfinedRebar = Math.Min(distanceFromConfinedRebarStart, distanceFromConfinedRebarEnd);

                    //Ignore the first rebar
                    if (rebarHorizontalIndex != 0)
                    {
                        if (distanceFromConfinedRebar <= 150)
                        {
                            //do nothing
                        }
                        else
                        {
                            additionalShearLinksLeg += 1;
                            distanceFromConfinedRebarStart = 0;
                        }
                    }

                    distanceFromConfinedRebarStart += cToCRebar;
                    distanceFromConfinedRebarEnd -= cToCRebar;
                }
                noShearLinksLeg = 2 + additionalShearLinksLeg;
                */
                noShearLinksLeg = 2;

                //Shear links design
                while (noShearLinksLeg <= maxNoRebarPerLayer && !shearLinksSatisfied)
                {
                    shearLinksSpacing = maxShearLinksSpacing;

                    while (shearLinksSpacing >= minShearLinksSpacing && !shearLinksSatisfied)
                    {
                        double avFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLeg * (1000 / shearLinksSpacing);

                        if (avFromCurrentShearLinks >= avReq)
                        {
                            shearLinksSatisfied = true;
                        }

                        //Check
                        if (shearLinksSatisfied)
                        {
                            //do nothing
                        }
                        else if (shearLinksSpacing <= minShearLinksSpacing)
                        {
                            break;
                        }
                        else
                        {
                            shearLinksSpacing -= shearLinksSpacingStep;
                        }
                    }

                    //Check
                    if (shearLinksSatisfied)
                    {
                        //do nothing
                    }
                    else if (noShearLinksLeg == maxNoRebarPerLayer)
                    {
                        break;
                    }
                    else
                    {
                        noShearLinksLeg += 1;
                    }

                }

                //Check
                if (shearLinksSatisfied)
                {
                    //do nothing
                }
                else if (shearLinksDiameterIndex == shearLinksDiameterList.Count() - 1)
                {
                    break;
                }
                else
                {
                    shearLinksDiameterIndex += 1;
                }
            }

            string asEvaluationComment = "";
            string avEvaluationComment = "";

            //Evaluating
            if (!rebarSatisfied)
            {
                noRebarLayer = 0;
                noRebarPerLayer = new List<int>() { 0 };
                rebarDiameterPerLayer = new List<int>() { 0 };
                asEvaluationComment = "Rebar not satisfied!";
            }


            if (!shearLinksSatisfied)
            {
                noShearLinksLeg = 0;
                shearLinksDiameter = 0;
                shearLinksSpacing = 0;
                avEvaluationComment = "Shear links not satisfied!";
            }

            /*
            //Calculate d
            double d = height - cover - shearLinksDiameter;
            for (int layer = 0; layer < rebarDiameterPerLayer.Count(); layer++)
            {
                d -= rebarDiameterPerLayer[layer];

                //For last layer
                if (layer == rebarDiameterPerLayer.Count() - 1)
                    d -= Math.Max(25, rebarDiameterPerLayer[layer]);
                else
                {
                    d -= 
                }
            }
            */

            return (noRebarLayer, noRebarPerLayer, rebarDiameterPerLayer, asEvaluationComment, noShearLinksLeg, shearLinksDiameter, shearLinksSpacing, avEvaluationComment, warningRebarDiameterTooBigShown);
        }

        private static bool IsNoRebarLayerValid(List<int> noRebarPerLayer)
        {
            //If there is only 1 layer
            if (noRebarPerLayer.Count == 1)
            {
                return true;
            }
            else
            {
                int noRebarFirstLayer = noRebarPerLayer[0];

                //Go through each layer
                foreach (int eachNoRebarPerLayer in noRebarPerLayer)
                {
                    if (eachNoRebarPerLayer == 2 || (noRebarFirstLayer % 2 == 0 && eachNoRebarPerLayer % 2 == 0) || (noRebarFirstLayer % 2 != 0 && eachNoRebarPerLayer % 2 != 0))
                    {
                        //do nothing
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static object[,] CompileResutRcBeamToArray(List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "fyk (Mpa)", "fck (Mpa)", "d (ETABS) (mm)", "d calculated (mm)", "As Min(mm2)"
                , "As (mm2)","Av Major (mm2/m)","Av Minor (mm2/m)"
                , "Rebar", "Shear Links"
                , "Left Top As Prov (mm2)", "Mid Top As Prov (mm2)", "Right Top As Prov (mm2)", "Left Bottom As  Prov (mm2)", "Mid Bottom As Prov (mm2)", "Right Bottom As Prov (mm2)"
                , "Left Av Prov (mm2/m)", "Mid Av Prov (mm2/m)", "Right Av Prov (mm2/m)"
                , "Left Top As Ratio", "Mid Top As Ratio", "Right Top As Ratio", "Left Bottom As Ratio", "Mid Bottom As Ratio", "Right Bottom As Ratio"
                , "Left Av Ratio", "Mid Av Ratio", "Right Av Ratio"
                , "Rebar warning", "Shear links warning"
            };

            int numOfRow = rcColumnList.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1, numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                result[i, 0] = rcColumn.UniqueName;
                result[i, 1] = rcColumn.Width;
                result[i, 2] = rcColumn.Height;
                result[i, 3] = rcColumn.Fyk;
                result[i, 4] = rcColumn.Fck;
                result[i, 5] = rcColumn.d;
                result[i, 6] = rcColumn.dCalculated;
                result[i, 7] = rcColumn.AsMin;

                result[i, 8] = rcColumn.As;

                result[i, 9] = rcColumn.AvMajor;
                result[i, 10] = rcColumn.AvMinor;

                result[i, 11] = rcColumn.Rebar;

                result[i, 12] = rcColumn.ShearLinks;

                result[i, 13] = rcColumn.AsEvaluationComment;
                result[i, 14] = rcColumn.AvEvaluationComment;

                i++;
            }

            return result;
        }

        public static void WriteResultRcBeamToExcel(object[,] result, string fileName, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            //char aplhabethOfColFromManual = alphabet[numOfCol - 1];

            //string lastCellRange = alphabet[numOfCol - 1] + Convert.ToString(numOfRow - 1 + 1);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

            //Here saving the file in xlsx
            string excelPath = filePath + fileName + ".xlsx";

            try
            {
                xlWorkBook.SaveAs(excelPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
        }

        public static double GetMinimumRcBeamRebarEC(double fck, double fyk, double width, double effDepth)
        {
            //Calculate fctm
            double fctm = 0;

            if (fck <= 50)
            {
                fctm = 0.3 * Math.Pow(fck, 2.0 / 3.0);
            }
            else
            {
                double fcm = fck + 8;
                fctm = 2.12 * Math.Log(1 + fcm / 10);
            }

            //Min rebar percentage
            double minRebarPercentage = 0.26 * fctm / fyk;
            minRebarPercentage = Math.Max(minRebarPercentage, 0.13 / 100);

            //Min As
            double asMin = minRebarPercentage * width * effDepth;

            return asMin;
        }

        public static void CompareToMinimumRcBeamRebar(src.HelperEtabs.RcBeam rcBeam)
        {
            double asMin = 0;
            asMin = GetMinimumRcBeamRebarEC(rcBeam.Fck, rcBeam.Fyk, rcBeam.Width, rcBeam.d);
            rcBeam.AsMin = Math.Ceiling(asMin);

            if (rcBeam.BeamType == "Simply supported")
            {
                rcBeam.LeftTopAs = Math.Max(rcBeam.LeftTopAs, asMin);
                rcBeam.MidTopAs = Math.Max(rcBeam.MidTopAs, asMin);
                rcBeam.RightTopAs = Math.Max(rcBeam.RightTopAs, asMin);
                rcBeam.LeftBottomAs = Math.Max(rcBeam.LeftBottomAs, asMin);
                rcBeam.MidBottomAs = Math.Max(rcBeam.MidBottomAs, asMin);
                rcBeam.RightBottomAs = Math.Max(rcBeam.RightBottomAs, asMin);
            }
            else if (rcBeam.BeamType == "Cantilever")
            {
                rcBeam.LeftTopAs = Math.Max(rcBeam.LeftTopAs, asMin);
                rcBeam.RightTopAs = Math.Max(rcBeam.RightTopAs, asMin);
                rcBeam.LeftBottomAs = Math.Max(rcBeam.LeftBottomAs, asMin);
                rcBeam.RightBottomAs = Math.Max(rcBeam.RightBottomAs, asMin);
            }
        }

        public static void CalculateDFromRebar(List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                double coverToRebarCentroid = 0;
                double maxCoverToRebarCentroid = 0;
                double centroidFromFirstRebarCentroid = 0;

                //Left top
                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcColumn.RebarDiameterList, rcColumn.NoRebarPerLayer);
                coverToRebarCentroid = rcColumn.Cover + rcColumn.ShearLinksDiameter + 0.5 * rcColumn.RebarDiameterList[0] + centroidFromFirstRebarCentroid;
                maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                //Save d calcualated
                rcColumn.dCalculated = Math.Floor(rcColumn.Height - maxCoverToRebarCentroid);

                //Give warning if d calculated < d ETABS
                if (rcColumn.dCalculated < rcColumn.d)
                {
                    if (rcColumn.AsEvaluationComment == null)
                    {
                        rcColumn.AsEvaluationComment = "d calculated < d ETABS";
                    }
                    else
                    {
                        rcColumn.AsEvaluationComment += ", d calculated < d ETABS";
                    }
                }
            }
        }

        public static void CalculateAsProv(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                //Left top
                rcBeam.LeftTopAsProv = CalculateAsProvFromRebarList(rcBeam.LeftTopRebarDiameterList, rcBeam.LeftTopNoRebarPerLayer);
                rcBeam.RatioLeftTopAs = Math.Round(rcBeam.LeftTopAs / rcBeam.LeftTopAsProv, 3);

                //Left bottom
                rcBeam.LeftBottomAsProv = CalculateAsProvFromRebarList(rcBeam.LeftBottomRebarDiameterList, rcBeam.LeftBottomNoRebarPerLayer);
                rcBeam.RatioLeftBottomAs = Math.Round(rcBeam.LeftBottomAs / rcBeam.LeftBottomAsProv, 3);

                if (rcBeam.BeamType == "Simply supported")
                {
                    //Mid top
                    rcBeam.MidTopAsProv = CalculateAsProvFromRebarList(rcBeam.MidTopRebarDiameterList, rcBeam.MidTopNoRebarPerLayer);
                    rcBeam.RatioMidTopAs = Math.Round(rcBeam.MidTopAs / rcBeam.MidTopAsProv, 3);

                    //Mid bottom
                    rcBeam.MidBottomAsProv = CalculateAsProvFromRebarList(rcBeam.MidBottomRebarDiameterList, rcBeam.MidBottomNoRebarPerLayer);
                    rcBeam.RatioMidBottomAs = Math.Round(rcBeam.MidBottomAs / rcBeam.MidBottomAsProv, 3);
                }
                else
                {
                    rcBeam.MidTopAsProv = 0;
                    rcBeam.RatioMidTopAs = 0;

                    rcBeam.MidBottomAsProv = 0;
                    rcBeam.RatioMidBottomAs = 0;
                }

                //Right top
                rcBeam.RightTopAsProv = CalculateAsProvFromRebarList(rcBeam.RightTopRebarDiameterList, rcBeam.RightTopNoRebarPerLayer);
                rcBeam.RatioRightTopAs = Math.Round(rcBeam.RightTopAs / rcBeam.RightTopAsProv, 3);

                //Right bottom
                rcBeam.RightBottomAsProv = CalculateAsProvFromRebarList(rcBeam.RightBottomRebarDiameterList, rcBeam.RightBottomNoRebarPerLayer);
                rcBeam.RatioRightBottomAs = Math.Round(rcBeam.RightBottomAs / rcBeam.RightBottomAsProv, 3);
            }
        }

        public static double CalculateAsProvFromRebarList(List<int> rebarDiameterList, List<int> noRebarPerLayer)
        {
            double rebarArea = 0;

            for (int layerIndex = 0; layerIndex < rebarDiameterList.Count; layerIndex++)
            {
                double diameter = rebarDiameterList[layerIndex];
                int noRebar = noRebarPerLayer[layerIndex];

                rebarArea += Math.Round(0.25 * Math.PI * Math.Pow(diameter, 2) * noRebar);
            }

            return rebarArea;
        }

        public static void CalculateAvProv(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                //Left
                rcBeam.LeftAvProv = Math.Round(rcBeam.LeftNoShearLinksLeg * 0.25 * Math.PI * Math.Pow(rcBeam.LeftShearLinksDiameter, 2) * (1000 / rcBeam.LeftShearLinksSpacing)); //mm2/m
                rcBeam.RatioLeftAv = Math.Round(rcBeam.LeftAv / rcBeam.LeftAvProv, 3);

                if (rcBeam.BeamType == "Simply supported")
                {
                    //Mid
                    rcBeam.MidAvProv = Math.Round(rcBeam.MidNoShearLinksLeg * 0.25 * Math.PI * Math.Pow(rcBeam.MidShearLinksDiameter, 2) * (1000 / rcBeam.MidShearLinksSpacing)); //mm2/m
                    rcBeam.RatioMidAv = Math.Round(rcBeam.MidAv / rcBeam.MidAvProv, 3);
                }
                else
                {
                    rcBeam.MidAvProv = 0;
                    rcBeam.RatioMidAv = 0;
                }

                //Right
                rcBeam.RightAvProv = Math.Round(rcBeam.RightNoShearLinksLeg * 0.25 * Math.PI * Math.Pow(rcBeam.RightShearLinksDiameter, 2) * (1000 / rcBeam.RightShearLinksSpacing)); //mm2/m
                rcBeam.RatioRightAv = Math.Round(rcBeam.RightAv / rcBeam.RightAvProv, 3);
            }
        }

        public static double GetCentroidFromRebarDiameterList(List<int> rebarDiameterList, List<int> noRebarPerLayer)
        {
            //For 1 layer only
            if (rebarDiameterList.Count == 1)
            {
                return 0;
            }
            else
            {
                double sumAreaTimesCentroid = 0;
                double sumArea = 0;
                double eachLayerCentroid = 0;
                int index = 0;
                foreach (int rebarDiameter in rebarDiameterList)
                {
                    sumAreaTimesCentroid += 0.25 * Math.PI * Math.Pow(rebarDiameter, 2) * eachLayerCentroid * noRebarPerLayer[index];
                    sumArea += 0.25 * Math.PI * Math.Pow(rebarDiameter, 2);

                    //Calculate centroid for next layer. For RC column assumed rebar to be bundled
                    if (index < rebarDiameterList.Count - 1)
                    {
                        eachLayerCentroid += 0.5 * rebarDiameter + 0.5 * rebarDiameterList[index + 1];
                        //eachLayerCentroid += 0.5 * rebarDiameter + Math.Max(25, rebarDiameter) + 0.5 * rebarDiameterList[index + 1];
                    }

                    index++;
                }

                double centroid = sumAreaTimesCentroid / sumArea;
                return centroid;
            }

        }
    }
}
