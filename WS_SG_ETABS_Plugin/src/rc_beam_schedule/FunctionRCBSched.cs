using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    internal class FunctionRCBSched
    {

        private static int Ret;

        public static bool GetRCBeamSchedule(cSapModel Sap, string fileName, string filePath, double leftLength, double midLength, double rightLength, double minCToCRebar, double maxShearLinksSpacing, double minShearLinksSpacing, double cover)
        {
            //Get current unit
            eForce forceUnits = new eForce();
            eLength lengthUnits = new eLength();
            eTemperature temperatureUnits = new eTemperature();
            Ret = Sap.GetPresentUnits_2(ref forceUnits, ref lengthUnits, ref temperatureUnits);

            //Set unit to mm
            src.HelperEtabs.SetOutputUnitTo_kN_mm_C(Sap);

            bool isError = false;

            bool tableExist = CheckTableExistRCBSched(Sap);
            
            if (!tableExist)
            {
                isError = true;
                return isError;
            }

            //Get RC design result table
            src.HelperEtabs.DatabaseTable rcBeamDesignSummary = new src.HelperEtabs.DatabaseTable();
            (isError, rcBeamDesignSummary) = GetRcBeamDesignResult(Sap);

            if (isError)
            {
                return isError;
            }

            //Get list of beams
            List<src.HelperEtabs.Frame> rcBeamList = new List<src.HelperEtabs.Frame>();
            rcBeamList = GetRcBeamList(rcBeamDesignSummary);

            //Get dimension
            GetRcBeamDimension(Sap, rcBeamDesignSummary, rcBeamList, cover);

            //Get beam offset
            GetRcBeamOffset(Sap, rcBeamList);

            //Get rebar required from etabs design
            GetRcBeamRebarArea(Sap, rcBeamDesignSummary, rcBeamList, leftLength, midLength, rightLength);

            //Convert area to rebar
            DesignRcBeamRebar(rcBeamList, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing);

            //Compile data to 2d array
            var result = CompileResutRcBeamToArray(rcBeamList);

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
        public static bool CheckTableExistRCBSched(cSapModel Sap)
        {
            string designCode = "";

            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            //Check if the table exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add("Concrete Beam Design Summary - " + designCode);

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

        public static (bool, src.HelperEtabs.DatabaseTable) GetRcBeamDesignResult(cSapModel Sap)
        {
            //Get RC design result table
            src.HelperEtabs.DatabaseTable rcBeamDesignSummary = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string designCode = "";
            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            if (designCode != "Eurocode 2-2004")
            {
                isError = true;
                MessageBox.Show("Only support Eurocode 2-2004!");
                return (isError, rcBeamDesignSummary);
            }

            string tableKey = "Concrete Beam Design Summary - " + designCode;

            (isError, rcBeamDesignSummary) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            return (isError, rcBeamDesignSummary);
        }

        public static List<src.HelperEtabs.Frame> GetRcBeamList(src.HelperEtabs.DatabaseTable rcBeamDesignSummary)
        {
            //Get table
             List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            List<src.HelperEtabs.Frame> rcBeamList = new List<HelperEtabs.Frame>();

            //Get list of unique name
            List<string> uniqueNameAllList = new List<string>();

            foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
            {
                uniqueNameAllList.Add(row["UniqueName"]);
            }
            List<string> uniqueNameList = uniqueNameAllList.Distinct().ToList();

            //Create frame list
            foreach (string uniqueName in uniqueNameList)
            {
                rcBeamList.Add(new src.HelperEtabs.Frame(uniqueName));
            }

            return rcBeamList;

        }

        public static void GetRcBeamDimension(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary ,List<src.HelperEtabs.Frame> rcBeamList, double cover)
        {
            //Get table
            List<Dictionary<string , string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                foreach(Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        string sectionPropName = row["DesignSect"];
                        double width = 0;
                        double height = 0;
                        double coverTop = 0;
                        double coverBottom = 0;

                        (width, height) = src.HelperEtabs.GetDimensionRectangle(Sap, sectionPropName);

                        rcBeam.Width = width;
                        rcBeam.Height = height;

                        //(coverTop, coverBottom) = src.HelperEtabs.GetRcBeamCover(Sap, sectionPropName);

                        //rcBeam.CoverTop = coverTop;
                        //rcBeam.CoverBottom = coverBottom;

                        rcBeam.CoverTop = cover;
                        rcBeam.CoverBottom = cover;

                        double d = height - Math.Max(coverTop, coverBottom);
                        rcBeam.d = d;

                        break;
                    }
                }
            }
        }

        public static void GetRcBeamOffset(cSapModel Sap, List<src.HelperEtabs.Frame> rcBeamList)
        {
            src.HelperEtabs.DatabaseTable rcBeamOffset = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string tableKey = "Frame Assignments - End Length Offsets";
            (isError, rcBeamOffset) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            List<Dictionary<string, string>> rcBeamOffsetTable = rcBeamOffset.TableInList;

            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
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

        public static void GetRcBeamRebarArea(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary, 
            List<src.HelperEtabs.Frame> rcBeamList, double leftLength, double midLength, double rightLength)
        {
            //Get table
            List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            //Getting length for each beam
            foreach (src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                bool firstRun = true;
                double length = 0;

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
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
                rcBeam.Length = length;
            }

            //Getting rebar area for each beam
            foreach (src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                double effectiveLength = rcBeam.Length - rcBeam.OffsetI - rcBeam.OffsetJ;

                double leftStation = effectiveLength * leftLength + rcBeam.OffsetI;
                double rightStation = effectiveLength * (1-rightLength) + rcBeam.OffsetI;

                double leftTopAs = 0;
                double leftBottomAs = 0;
                double midTopAs = 0;
                double midBottomAs = 0;
                double rightTopAs = 0;
                double rightBottomAs = 0;

                double leftAv = 0;
                double midAv = 0;
                double rightAv = 0;

                bool firstLeft = true;
                bool firstMid = true;
                bool firstRight = true;

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        if (Convert.ToDouble(row["Station"]) <= leftStation)
                        {
                            if (firstLeft)
                            {
                                leftTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                leftBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                leftAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstLeft = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > leftTopAs)
                                {
                                    leftTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > leftBottomAs)
                                {
                                    leftBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > leftAv)
                                {
                                    leftAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }
                        }
                        else if (Convert.ToDouble(row["Station"]) >= rightStation)
                        {
                            if (firstRight)
                            {
                                rightTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                rightBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                rightAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstRight = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > rightTopAs)
                                {
                                    rightTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > rightBottomAs)
                                {
                                    rightBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > rightAv)
                                {
                                    rightAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }
                        }
                        else
                        {
                            if (firstMid)
                            {
                                midTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                midBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                midAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstMid = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > midTopAs)
                                {
                                    midTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > midBottomAs)
                                {
                                    midBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > midAv)
                                {
                                    midAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }

                        }
                    }
                }

                rcBeam.LeftBottomAs = leftBottomAs;
                rcBeam.LeftTopAs = leftTopAs;
                rcBeam.MidBottomAs = midBottomAs;
                rcBeam.MidTopAs = midTopAs;
                rcBeam.RightBottomAs = rightBottomAs;
                rcBeam.RightTopAs = rightTopAs;

                rcBeam.LeftAv = leftAv * 1000; //mm2/m
                rcBeam.MidAv = midAv * 1000; //mm2/m
                rcBeam.RightAv = rightAv * 1000; //mm2/m
            }

        }

        public static void DesignRcBeamRebar(List<src.HelperEtabs.Frame> rcBeamList, double minCToCRebar, double maxShearLinksSpacing, double minShearLinksSpacing)
        {
            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                double cover = Math.Max(rcBeam.CoverTop, rcBeam.CoverBottom);

                List<double> shearLinksDiameterList = new List<double>{ 10, 12, 16 };
                List<double> rebarDiameterList = new List<double> { 16, 20, 25, 32 };

                int maxNoLayer = 3;

                double leftTopAs = rcBeam.LeftTopAs;
                double leftTopAv = rcBeam.LeftAv;
                double width = rcBeam.Width;
                double leftTopNoRebarLayer = 0;
                double leftTopNoRebarPerLayer = 0;
                double leftTopRebarDiameter = 0;
                string leftTopAsEvaluationComment = "";
                double leftTopNoShearLinksLeg = 0;
                double leftTopshearLinksDiameter = 0;
                double leftTopShearLinksSpacing = 0;
                string leftTopAvEvaluationComment = "";
                (leftTopNoRebarLayer, leftTopNoRebarPerLayer, leftTopRebarDiameter, leftTopAsEvaluationComment, leftTopNoShearLinksLeg, leftTopshearLinksDiameter, leftTopShearLinksSpacing, leftTopAvEvaluationComment)  = ConvertRcBeamAreaToRebar(
                    leftTopAs, leftTopAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (leftTopNoRebarLayer == 1)
                {
                    rcBeam.LeftTopRebar = leftTopNoRebarPerLayer + "H" + leftTopRebarDiameter;
                }
                else
                {
                    rcBeam.LeftTopRebar = leftTopNoRebarLayer + "x" + leftTopNoRebarPerLayer + "H" + leftTopRebarDiameter;
                }

                double leftBottomAs = rcBeam.LeftBottomAs;
                double leftBottomAv = rcBeam.LeftAv;
                width = rcBeam.Width;
                double leftBottomNoRebarLayer = 0;
                double leftBottomNoRebarPerLayer = 0;
                double leftBottomRebarDiameter = 0;
                string leftBottomAsEvaluationComment = "";
                double leftBottomNoShearLinksLeg = 0;
                double leftBottomshearLinksDiameter = 0;
                double leftBottomShearLinksSpacing = 0;
                string leftBottomAvEvaluationComment = "";
                (leftBottomNoRebarLayer, leftBottomNoRebarPerLayer, leftBottomRebarDiameter, leftBottomAsEvaluationComment, leftBottomNoShearLinksLeg, leftBottomshearLinksDiameter, leftBottomShearLinksSpacing, leftBottomAvEvaluationComment) = ConvertRcBeamAreaToRebar(
                    leftBottomAs, leftBottomAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (leftBottomNoRebarLayer == 1)
                {
                    rcBeam.LeftBottomRebar = leftBottomNoRebarPerLayer + "H" + leftBottomRebarDiameter;
                }
                else
                {
                    rcBeam.LeftBottomRebar = leftBottomNoRebarLayer + "x" + leftBottomNoRebarPerLayer + "H" + leftBottomRebarDiameter;
                }

                rcBeam.LeftShearLinks = leftTopNoShearLinksLeg + "H" + leftTopshearLinksDiameter + "-" + leftTopShearLinksSpacing;


                double midTopAs = rcBeam.MidTopAs;
                double midTopAv = rcBeam.MidAv;
                width = rcBeam.Width;
                double midTopNoRebarLayer = 0;
                double midTopNoRebarPerLayer = 0;
                double midTopRebarDiameter = 0;
                string midTopAsEvaluationComment = "";
                double midTopNoShearLinksLeg = 0;
                double midTopshearLinksDiameter = 0;
                double midTopShearLinksSpacing = 0;
                string midTopAvEvaluationComment = "";
                (midTopNoRebarLayer, midTopNoRebarPerLayer, midTopRebarDiameter, midTopAsEvaluationComment, midTopNoShearLinksLeg, midTopshearLinksDiameter, midTopShearLinksSpacing, midTopAvEvaluationComment) = ConvertRcBeamAreaToRebar(
                    midTopAs, midTopAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (midTopNoRebarLayer == 1)
                {
                    rcBeam.MidTopRebar = midTopNoRebarPerLayer + "H" + midTopRebarDiameter;
                }
                else
                {
                    rcBeam.MidTopRebar = midTopNoRebarLayer + "x" + midTopNoRebarPerLayer + "H" + midTopRebarDiameter;
                }

                double midBottomAs = rcBeam.MidBottomAs;
                double midBottomAv = rcBeam.MidAv;
                width = rcBeam.Width;
                double midBottomNoRebarLayer = 0;
                double midBottomNoRebarPerLayer = 0;
                double midBottomRebarDiameter = 0;
                string midBottomAsEvaluationComment = "";
                double midBottomNoShearLinksLeg = 0;
                double midBottomshearLinksDiameter = 0;
                double midBottomShearLinksSpacing = 0;
                string midBottomAvEvaluationComment = "";
                (midBottomNoRebarLayer, midBottomNoRebarPerLayer, midBottomRebarDiameter, midBottomAsEvaluationComment, midBottomNoShearLinksLeg, midBottomshearLinksDiameter, midBottomShearLinksSpacing, midBottomAvEvaluationComment) = ConvertRcBeamAreaToRebar(
                    midBottomAs, midBottomAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (midBottomNoRebarLayer == 1)
                {
                    rcBeam.MidBottomRebar = midBottomNoRebarPerLayer + "H" + midBottomRebarDiameter;
                }
                else
                {
                    rcBeam.MidBottomRebar = midBottomNoRebarLayer + "x" + midBottomNoRebarPerLayer + "H" + midBottomRebarDiameter;
                }

                rcBeam.MidShearLinks = midTopNoShearLinksLeg + "H" + midTopshearLinksDiameter + "-" + midTopShearLinksSpacing;

                double rightTopAs = rcBeam.RightTopAs;
                double rightTopAv = rcBeam.RightAv;
                width = rcBeam.Width;
                double rightTopNoRebarLayer = 0;
                double rightTopNoRebarPerLayer = 0;
                double rightTopRebarDiameter = 0;
                string rightTopAsEvaluationComment = "";
                double rightTopNoShearLinksLeg = 0;
                double rightTopshearLinksDiameter = 0;
                double rightTopShearLinksSpacing = 0;
                string rightTopAvEvaluationComment = "";
                (rightTopNoRebarLayer, rightTopNoRebarPerLayer, rightTopRebarDiameter, rightTopAsEvaluationComment, rightTopNoShearLinksLeg, rightTopshearLinksDiameter, rightTopShearLinksSpacing, rightTopAvEvaluationComment) = ConvertRcBeamAreaToRebar(
                    rightTopAs, rightTopAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (rightTopNoRebarLayer == 1)
                {
                    rcBeam.RightTopRebar = rightTopNoRebarPerLayer + "H" + rightTopRebarDiameter;
                }
                else
                {
                    rcBeam.RightTopRebar = rightTopNoRebarLayer + "x" + rightTopNoRebarPerLayer + "H" + rightTopRebarDiameter;
                }

                double rightBottomAs = rcBeam.RightBottomAs;
                double rightBottomAv = rcBeam.RightAv;
                width = rcBeam.Width;
                double rightBottomNoRebarLayer = 0;
                double rightBottomNoRebarPerLayer = 0;
                double rightBottomRebarDiameter = 0;
                string rightBottomAsEvaluationComment = "";
                double rightBottomNoShearLinksLeg = 0;
                double rightBottomshearLinksDiameter = 0;
                double rightBottomShearLinksSpacing = 0;
                string rightBottomAvEvaluationComment = "";
                (rightBottomNoRebarLayer, rightBottomNoRebarPerLayer, rightBottomRebarDiameter, rightBottomAsEvaluationComment, rightBottomNoShearLinksLeg, rightBottomshearLinksDiameter, rightBottomShearLinksSpacing, rightBottomAvEvaluationComment) = ConvertRcBeamAreaToRebar(
                    rightBottomAs, rightBottomAv, width, minCToCRebar, maxShearLinksSpacing, minShearLinksSpacing, cover, shearLinksDiameterList, rebarDiameterList, maxNoLayer);

                if (rightBottomNoRebarLayer == 1)
                {
                    rcBeam.RightBottomRebar = rightBottomNoRebarPerLayer + "H" + rightBottomRebarDiameter;
                }
                else
                {
                    rcBeam.RightBottomRebar = rightBottomNoRebarLayer + "x" + rightBottomNoRebarPerLayer + "H" + rightBottomRebarDiameter;
                }

                rcBeam.RightShearLinks = rightTopNoShearLinksLeg + "H" + rightTopshearLinksDiameter + "-" + rightTopShearLinksSpacing;

            }
        }

        private static (double, double, double, string, double, double, double, string) ConvertRcBeamAreaToRebar(double asReq, double avReq, double width, double minCToCRebar, double maxShearLinksSpacing, double minShearLinksSpacing,
            double cover, List<double> shearLinksDiameterList, List<double> rebarDiameterList, int maxNoLayer)
        {
            int shearLinksDiameterIndex = 0;
            double shearLinksDiameter = 0;
            bool shearLinksSatisfied = false;
            int noShearLinksLeg = 2;
            double shearLinksSpacing = maxShearLinksSpacing;

            int rebarDiameterIndex = 0;
            double rebarDiameter = 0;
            bool rebarSatisfied = false;
            int noRebarLayer = 1;
            double noRebarPerLayer = 0;

            //Keep looping if not both satisfied
            while (shearLinksDiameterIndex <= shearLinksDiameterList.Count - 1 && !shearLinksSatisfied)
            {
                rebarDiameterIndex = 0;
                rebarSatisfied = false;
                noRebarLayer = 1;
                noRebarPerLayer = 0;

                shearLinksDiameter = shearLinksDiameterList[shearLinksDiameterIndex];
                noShearLinksLeg = 2;
                shearLinksSpacing = maxShearLinksSpacing;

                //Rebar design
                while (noRebarLayer <= maxNoLayer && !rebarSatisfied)
                {
                    rebarDiameterIndex = 0;

                    while (rebarDiameterIndex <= rebarDiameterList.Count - 1 && !rebarSatisfied)
                    {
                        rebarDiameter = rebarDiameterList[rebarDiameterIndex];

                        noRebarPerLayer = Math.Floor((width - cover * 2 - shearLinksDiameter * 2 - rebarDiameter) / minCToCRebar + 1);

                        double asFromCurrentRebar = 0.25 * Math.PI * Math.Pow(rebarDiameter, 2) * noRebarPerLayer * noRebarLayer;

                        if (asFromCurrentRebar >= asReq)
                        {
                            rebarSatisfied = true;
                        }

                        if (!rebarSatisfied)
                        {
                            rebarDiameterIndex += 1;
                        }
                    }
                    if (!rebarSatisfied)
                    {
                        noRebarLayer += 1;
                    }
                }

                //Shear links design
                while ((noShearLinksLeg <= noRebarPerLayer || noShearLinksLeg == 2) && !shearLinksSatisfied)
                {
                    while (shearLinksSpacing >= minShearLinksSpacing && !shearLinksSatisfied)
                    {
                        double avFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLeg * (1000 / shearLinksSpacing);

                        if (avFromCurrentShearLinks >= avReq)
                        {
                            shearLinksSatisfied = true;
                        }

                        if (!shearLinksSatisfied)
                        {
                            shearLinksSpacing -= 100;
                        }
                        
                    }

                    if (!shearLinksSatisfied)
                    {
                        noShearLinksLeg += 1;
                    }

                }
                if (!shearLinksSatisfied)
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
                noRebarPerLayer = 0;
                rebarDiameter = 0;
                asEvaluationComment = "Rebar not satisfied!";
            }


            if (!shearLinksSatisfied)
            {
                noShearLinksLeg = 0;
                shearLinksDiameter = 0;
                shearLinksSpacing = 0;
                avEvaluationComment = "Shear links not satisfied!";
            }

            return (noRebarLayer, noRebarPerLayer, rebarDiameter, asEvaluationComment, noShearLinksLeg, shearLinksDiameter, shearLinksSpacing, avEvaluationComment);
        }

        private static object[,] CompileResutRcBeamToArray(List<src.HelperEtabs.Frame> rcBeamList)
        {
            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "d (mm)", "Left Top As (mm2)", "Mid Top As (mm2)", "Right Top As (mm2)"
                , "Left Bottom As (mm2)", "Mid Bottom As (mm2)", "Right Bottom As (mm2)", "Left Av (mm2/m)", "Mid Av (mm2/m)", "Right Av (mm2/m)"
                , "Left Top Rebar", "Mid Top Rebar", "Right Top Rebar", "Left Bottom Rebar", "Mid Bottom Rebar", "Right Bottom Rebar"
                , "Left Shear Links", "Mid Shear Links", "Right Shear Links"
            };

            int numOfRow = rcBeamList.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1,numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                result[i, 0] = rcBeam.UniqueName;
                result[i, 1] = rcBeam.Width;
                result[i, 2] = rcBeam.Height;
                result[i, 3] = rcBeam.d;
                result[i, 4] = rcBeam.LeftTopAs;
                result[i, 5] = rcBeam.MidTopAs;
                result[i, 6] = rcBeam.RightTopAs;
                result[i, 7] = rcBeam.LeftBottomAs;
                result[i, 8] = rcBeam.MidBottomAs;
                result[i, 9] = rcBeam.RightBottomAs;
                result[i, 10] = rcBeam.LeftAv;
                result[i, 11] = rcBeam.MidAv;
                result[i, 12] = rcBeam.RightAv;
                result[i, 13] = rcBeam.LeftTopRebar;
                result[i, 14] = rcBeam.MidTopRebar;
                result[i, 15] = rcBeam.RightTopRebar;
                result[i, 16] = rcBeam.LeftBottomRebar;
                result[i, 17] = rcBeam.MidBottomRebar;
                result[i, 18] = rcBeam.RightBottomRebar;
                result[i, 19] = rcBeam.LeftShearLinks;
                result[i, 20] = rcBeam.MidShearLinks;
                result[i, 21] = rcBeam.RightShearLinks;

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


            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string lastCellRange = alphabet[numOfCol - 1] + Convert.ToString(numOfRow - 1 + 1);

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


    }
}
