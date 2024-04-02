using ETABSv1;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.Office.Interop.Excel;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static Microsoft.ML.Trainers.KMeansTrainer;
using static WS_SG_ETABS_Plugin.src.rc_beam_schedule.ClassFunctionRCBSched;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using System.Windows.Markup;
using static WS_SG_ETABS_Plugin.src.HelperEtabs;
using static WS_SG_ETABS_Plugin.src.HelperSafe;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    internal class FunctionRCBSched
    {

        private static int Ret;

        public static bool GetRCBeamSchedule(cSapModel Sap, double leftLength, double midLength, double rightLength, double cantileverLeftLength, double cantileverRightLength,
            double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int maxNoLayer, int[] rebarDiameterList, int[] shearLinksDiameterList,
            List<Dictionary<string, string>> groupAndBeamType, bool? doML)
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

            bool tableExist = CheckTableExistRCBSched(Sap);
            
            if (!tableExist)
            {
                isError = true;
                return isError;
            }

            //Get RC beam design result
            src.HelperEtabs.DatabaseTable rcBeamDesignSummary = new src.HelperEtabs.DatabaseTable();
            (isError, rcBeamDesignSummary) = GetAllRcBeamDesignResult(Sap);

            if (isError) { return isError; }

            //Get group database table
            src.HelperEtabs.DatabaseTable groupDatabaseTable = new HelperEtabs.DatabaseTable();
            (isError, groupDatabaseTable) = src.HelperEtabs.GetTableDisplay(Sap, "Group Assignments", false);

            if (isError) { return isError; }

            //Get list of beams
            List<src.HelperEtabs.RcBeam> rcBeamList = new List<src.HelperEtabs.RcBeam>();
            (isError, rcBeamList) = GetRcBeamListSpecificGroup(Sap, rcBeamDesignSummary, groupAndBeamType, groupDatabaseTable);

            if (isError) {return isError;}

            //Get dimension
            GetRcBeamDimensionAndMaterial(Sap, rcBeamDesignSummary, rcBeamList, cover);

            //Get beam offset
            GetRcBeamOffset(Sap, rcBeamList);

            //Get rebar required from etabs design
            GetRcBeamRebarArea(Sap, rcBeamDesignSummary, rcBeamList, leftLength, midLength, rightLength, cantileverLeftLength, cantileverRightLength);

            //Get forces from etabs for self design
            //GetForcesEnvelope(Sap, rcBeamList, groupAndBeamType);

            //Convert area to rebar
            DesignRcBeamRebar(rcBeamList, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, rebarDiameterList, shearLinksDiameterList, maxNoLayer);

            //Check warning and error message from ETABS
            CheckMessageRcBeamRebar(rcBeamList, rcBeamDesignSummary);

            //Get d from rebar
            CalculateDFromRebar(rcBeamList);

            //Calculate As provided
            CalculateAsProv(rcBeamList);

            //Calculate Av provided
            CalculateAvProv(rcBeamList);

            //do kmeans
            if (doML == true)
            {
                RcBeamClusteringUsingKMeans(rcBeamList);
            }
            

            //Compile data to 2d array
            var result = CompileResutRcBeamToArray(rcBeamList);

            //Compile data self design to 2d array
            var result2 = CompileResutRcBeamSelfDesignToArray(rcBeamList);

            //Write to excel
            WriteResultRcBeamToExcel(result, result2);

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

        public static (bool, src.HelperEtabs.DatabaseTable) GetAllRcBeamDesignResult(cSapModel Sap)
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

        public static (bool, List<src.HelperEtabs.RcBeam>) GetRcBeamListSpecificGroup(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary, List<Dictionary<string, string>> groupAndBeamType, src.HelperEtabs.DatabaseTable groupDatabaseTable)
        {
            bool isError = false;

            //To return
            List<src.HelperEtabs.RcBeam> rcBeamList = new List<HelperEtabs.RcBeam>();

            //Filter group to only include line, to improve performance
            (isError, groupDatabaseTable) = src.HelperEtabs.FilterDatabaseTable(groupDatabaseTable, "ObjectType", "Line");
            if (isError) { return (isError, rcBeamList); }

            foreach (Dictionary<string, string> groupWithBeamType in groupAndBeamType)
            {
                string groupName = groupWithBeamType["groupName"];
                string beamType = groupWithBeamType["beamType"];

                src.HelperEtabs.DatabaseTable rcBeamDesignSummaryFiltered = new HelperEtabs.DatabaseTable();
                (isError, rcBeamDesignSummaryFiltered) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(rcBeamDesignSummary, groupDatabaseTable, groupName);

                if (isError)
                {
                    return (isError, rcBeamList);
                }

                List<src.HelperEtabs.RcBeam> rcBeamListEachGroup = GetRcBeamList(rcBeamDesignSummaryFiltered);

                foreach (src.HelperEtabs.RcBeam frame in rcBeamListEachGroup)
                {
                    frame.BeamType = beamType;
                }

                rcBeamList.AddRange(rcBeamListEachGroup);
            }

            return (isError, rcBeamList);
        }

        public static List<src.HelperEtabs.RcBeam> GetRcBeamList(src.HelperEtabs.DatabaseTable rcBeamDesignSummary)
        {
            //Get table
             List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            List<src.HelperEtabs.RcBeam> rcBeamList = new List<HelperEtabs.RcBeam>();

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
                rcBeamList.Add(new src.HelperEtabs.RcBeam(uniqueName));
            }

            return rcBeamList;
        }

        public static void GetRcBeamDimensionAndMaterial(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary ,List<src.HelperEtabs.RcBeam> rcBeamList, double cover)
        {
            //Get table
            List<Dictionary<string , string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            foreach(src.HelperEtabs.RcBeam rcBeam in rcBeamList)
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
                        string concreteMatProp = "";
                        string rebarMatProp = "";

                        (width, height, concreteMatProp) = src.HelperEtabs.GetRcBeamDimensionAndConcreteMaterial(Sap, sectionPropName);

                        rcBeam.Width = width;
                        rcBeam.Height = height;

                        (coverTop, coverBottom, rebarMatProp) = src.HelperEtabs.GetRcBeamCoverAndRebarMaterial(Sap, sectionPropName);

                        //In etabs cover is to rebar center
                        rcBeam.CoverTopEtabs = coverTop;
                        rcBeam.CoverBottomEtabs = coverBottom;
                        rcBeam.Cover = cover;

                        double d = height - Math.Max(coverTop, coverBottom);
                        rcBeam.d = d;

                        //Get fyk
                        double fyk = 0;
                        fyk = src.HelperEtabs.GetRebarFyk(Sap, rebarMatProp);
                        rcBeam.Fyk = fyk;

                        //Get fck
                        double fck = 0;
                        fck = src.HelperEtabs.GetConcreteFck(Sap, concreteMatProp);
                        rcBeam.Fck = fck;

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

            foreach(src.HelperEtabs.RcBeam rcBeam in rcBeamList)
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
            List<src.HelperEtabs.RcBeam> rcBeamList, double leftLength, double midLength, double rightLength, double cantileverLeftLength, double cantileverRightLength)
        {
            //Get table
            List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            //Getting length for each beam
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
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
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                double effectiveLength = rcBeam.Length - rcBeam.OffsetI - rcBeam.OffsetJ;

                double leftStation = effectiveLength * leftLength + rcBeam.OffsetI;
                double rightStation = effectiveLength * (1-rightLength) + rcBeam.OffsetI;

                double cantileverLeftStation = effectiveLength * cantileverLeftLength + rcBeam.OffsetI;

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

                //if mid is not found, means is short beam
                bool midNotFound = true;

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    //Single or multispan
                    if (row["UniqueName"] == rcBeam.UniqueName && rcBeam.BeamType == "Single/multi span")
                    {
                        //Saving station information
                        rcBeam.LeftStation = leftStation;
                        rcBeam.RightStation = rightStation;

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
                                midNotFound = false;
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
                    //Cantilever
                    else if (row["UniqueName"] == rcBeam.UniqueName && rcBeam.BeamType == "Cantilever")
                    {
                        //Saving station information
                        rcBeam.LeftStation = cantileverLeftStation;

                        if (Convert.ToDouble(row["Station"]) <= cantileverLeftStation)
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
                        else
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
                    }
                }

                //if short beam
                if (midNotFound && rcBeam.BeamType == "Single/multi span")
                {
                    rcBeam.LeftBottomAs = leftBottomAs;
                    rcBeam.LeftTopAs = leftTopAs;
                    rcBeam.MidBottomAs = Math.Max(leftBottomAs, rightBottomAs);
                    rcBeam.MidTopAs = Math.Max(leftTopAs, rightTopAs);
                    rcBeam.RightBottomAs = rightBottomAs;
                    rcBeam.RightTopAs = rightTopAs;

                    rcBeam.LeftAv = leftAv * 1000; //mm2/m
                    rcBeam.MidAv = Math.Max(leftAv, rightAv) * 1000; //mm2/m
                    rcBeam.RightAv = rightAv * 1000; //mm2/m
                }
                else
                {
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

                //Check againts minimum rebar
                CompareToMinimumRcBeamRebar(rcBeam);

            }

        }

        public static void DesignRcBeamRebar(List<src.HelperEtabs.RcBeam> rcBeamList, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, double minShearLinksSpacing, double shearLinksSpacingStep,
            int[] rebarDiameterList, int[] shearLinksDiameterList, int maxNoLayer)
        {
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                double cover = rcBeam.Cover;

                bool warningRebarDiameterTooBigShown = false;

                double leftTopAs = rcBeam.LeftTopAs;
                double leftTopAv = rcBeam.LeftAv;
                double width = rcBeam.Width;
                double height = rcBeam.Height;
                double leftTopNoRebarLayer = 0;
                List<int> leftTopNoRebarPerLayer = new List<int>();
                List<int> leftTopRebarDiameter = new List<int>();
                string leftTopAsEvaluationComment = "";
                double leftTopNoShearLinksLeg = 0;
                double leftTopshearLinksDiameter = 0;
                double leftTopShearLinksSpacing = 0;
                string leftTopAvEvaluationComment = "";
                (leftTopNoRebarLayer, leftTopNoRebarPerLayer, leftTopRebarDiameter, leftTopAsEvaluationComment, leftTopNoShearLinksLeg, leftTopshearLinksDiameter, leftTopShearLinksSpacing,
                    leftTopAvEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcBeamAreaToRebar(
                    leftTopAs, leftTopAv, width, height, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                    rebarDiameterList, maxNoLayer, rcBeam.UniqueName, warningRebarDiameterTooBigShown);

                rcBeam.LeftTopRebar = "";
                for (int layerIndex = 0; layerIndex < leftTopRebarDiameter.Count(); layerIndex++)
                {
                    rcBeam.LeftTopRebar += leftTopNoRebarPerLayer[layerIndex] + "H" + leftTopRebarDiameter[layerIndex] + "+";
                }
                rcBeam.LeftTopRebar = rcBeam.LeftTopRebar.Remove(rcBeam.LeftTopRebar.Length - 1, 1);
                rcBeam.LeftTopRebarDiameterList = leftTopRebarDiameter;
                rcBeam.LeftTopNoRebarPerLayer = leftTopNoRebarPerLayer;

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

                // Left shear links
                rcBeam.LeftShearLinks = "H" + leftTopshearLinksDiameter + "-" + leftTopShearLinksSpacing + " (Link)";
                if (leftTopNoShearLinksLeg > 2)
                {
                    rcBeam.LeftShearLinks += " + " + (leftTopNoShearLinksLeg - 2) + "H" + leftTopshearLinksDiameter + "-" + leftTopShearLinksSpacing + " (Hooks)";
                }
                rcBeam.LeftNoShearLinksLeg = leftTopNoShearLinksLeg;
                rcBeam.LeftShearLinksDiameter = leftTopshearLinksDiameter;
                rcBeam.LeftShearLinksSpacing = leftTopShearLinksSpacing;

                string midTopAsEvaluationComment = "";
                string midTopAvEvaluationComment = "";
                string midBottomAsEvaluationComment = "";
                string midBottomAvEvaluationComment = "";
                if (rcBeam.BeamType == "Single/multi span")
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
                    
                    //Mid shear links
                    rcBeam.MidShearLinks = "H" + midTopshearLinksDiameter + "-" + midTopShearLinksSpacing + " (Link)";
                    if (midTopNoShearLinksLeg > 2)
                    {
                        rcBeam.MidShearLinks += " + " + (midTopNoShearLinksLeg - 2) + "H" + midTopshearLinksDiameter + "-" + midTopShearLinksSpacing + " (Hooks)";
                    }
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

                // Right shear links
                rcBeam.RightShearLinks = "H" + rightTopshearLinksDiameter + "-" + rightTopShearLinksSpacing + " (Link)";
                if (rightTopNoShearLinksLeg > 2)
                {
                    rcBeam.RightShearLinks += " + " + (rightTopNoShearLinksLeg - 2) + "H" + rightTopshearLinksDiameter + "-" + rightTopShearLinksSpacing + " (Hooks)";
                }
                rcBeam.RightNoShearLinksLeg = rightTopNoShearLinksLeg;
                rcBeam.RightShearLinksDiameter = rightTopshearLinksDiameter;
                rcBeam.RightShearLinksSpacing = rightTopShearLinksSpacing;

                //Gathering evaluation comment
                if (leftTopAsEvaluationComment != "" || leftBottomAsEvaluationComment != "" || midTopAsEvaluationComment != "" || midBottomAsEvaluationComment != "" ||
                    rightTopAsEvaluationComment != "" || rightBottomAsEvaluationComment != "")
                {
                    rcBeam.AsEvaluationComment = "Num layer req > max num layer";
                }

                if (leftTopAvEvaluationComment != "" || leftBottomAvEvaluationComment != "" || midTopAvEvaluationComment != "" || midBottomAvEvaluationComment != "" ||
                    rightTopAvEvaluationComment != "" || rightBottomAvEvaluationComment != "")
                {
                    rcBeam.AvEvaluationComment = "Shear links spc < min shear links spc";
                }
            }
        }

        private static (double, List<int>, List<int>, string, double, double, double, string, bool) ConvertRcBeamAreaToRebar(double asReq, double avReq, double width, double height, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, 
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
            int minNoRebarPerLayer = Convert.ToInt32(Math.Round(0.0051 * width + 1.1118));

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

                        //Set no of rebar per layer to minimum
                        if (currentLayer == 0)
                        {
                            noRebarPerLayer[currentLayer] = minNoRebarPerLayer;
                        }
                        else
                        {
                            noRebarPerLayer[currentLayer] = 2;
                        }
                        

                        rebarDiameterPerLayer[currentLayer] = rebarDiameterList[rebarDiameterIndexCurrentLayer];
                        maxRebarDiameterInAllLayer = rebarDiameterPerLayer.Max();

                        //Calculating how many rebar per layer based on rebar spc preference
                        if (rebarSpcPreference == "clear")
                        {
                            maxNoRebarPerLayer = Convert.ToInt32(Math.Floor((width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer) / (rebarSpc + maxRebarDiameterInAllLayer) + 1));
                        }
                        else if (rebarSpcPreference == "ctoc")
                        {
                            maxNoRebarPerLayer = Convert.ToInt32(Math.Floor((width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer) / (rebarSpc) + 1));
                        }
                        else if (rebarSpcPreference == "auto")
                        {
                            rebarSpc = Math.Max(25, maxRebarDiameterInAllLayer);
                            maxNoRebarPerLayer = Convert.ToInt32(Math.Floor((width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer) / (rebarSpc + maxRebarDiameterInAllLayer) + 1));
                        }
                        else if (rebarSpcPreference == "autoRamboll")
                        {
                            maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(width/100));
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
                                noRebarCurrentLayer += 1;
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

        private static bool IsNoRebarLayerValid (List<int> noRebarPerLayer)
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
                    if (eachNoRebarPerLayer == 2 || (noRebarFirstLayer%2 == 0 && eachNoRebarPerLayer%2 == 0) || (noRebarFirstLayer % 2 != 0 && eachNoRebarPerLayer % 2 != 0))
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

        private static object[,] CompileResutRcBeamToArray(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "fyk (Mpa)", "fck (Mpa)", "d (ETABS) (mm)", "d calculated (mm)", "Beam type", "As Min(mm2)"
                , "Left Top As (mm2)", "Mid Top As (mm2)", "Right Top As (mm2)", "Left Bottom As (mm2)", "Mid Bottom As (mm2)", "Right Bottom As (mm2)"
                , "Left Av (mm2/m)", "Mid Av (mm2/m)", "Right Av (mm2/m)"
                , "Left Top Rebar", "Mid Top Rebar", "Right Top Rebar", "Left Bottom Rebar", "Mid Bottom Rebar", "Right Bottom Rebar"
                , "Left Shear Links", "Mid Shear Links", "Right Shear Links"
                , "Left Top As Prov (mm2)", "Mid Top As Prov (mm2)", "Right Top As Prov (mm2)", "Left Bottom As  Prov (mm2)", "Mid Bottom As Prov (mm2)", "Right Bottom As Prov (mm2)"
                , "Left Av Prov (mm2/m)", "Mid Av Prov (mm2/m)", "Right Av Prov (mm2/m)"
                , "Left Top As Ratio", "Mid Top As Ratio", "Right Top As Ratio", "Left Bottom As Ratio", "Mid Bottom As Ratio", "Right Bottom As Ratio"
                , "Left Av Ratio", "Mid Av Ratio", "Right Av Ratio"
                , "ETABS warning", "Rebar warning", "Shear links warning",
                "Cluster"
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
            foreach(src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                result[i, 0] = rcBeam.UniqueName;
                result[i, 1] = rcBeam.Width;
                result[i, 2] = rcBeam.Height;
                result[i, 3] = rcBeam.Fyk;
                result[i, 4] = rcBeam.Fck;
                result[i, 5] = rcBeam.d;
                result[i, 6] = rcBeam.dCalculated;
                result[i, 7] = rcBeam.BeamType;
                result[i, 8] = rcBeam.AsMin;

                result[i, 9] = rcBeam.LeftTopAs;
                result[i, 10] = rcBeam.MidTopAs;
                result[i, 11] = rcBeam.RightTopAs;
                result[i, 12] = rcBeam.LeftBottomAs;
                result[i, 13] = rcBeam.MidBottomAs;
                result[i, 14] = rcBeam.RightBottomAs;

                result[i, 15] = rcBeam.LeftAv;
                result[i, 16] = rcBeam.MidAv;
                result[i, 17] = rcBeam.RightAv;

                result[i, 18] = rcBeam.LeftTopRebar;
                result[i, 19] = rcBeam.MidTopRebar;
                result[i, 20] = rcBeam.RightTopRebar;
                result[i, 21] = rcBeam.LeftBottomRebar;
                result[i, 22] = rcBeam.MidBottomRebar;
                result[i, 23] = rcBeam.RightBottomRebar;

                result[i, 24] = rcBeam.LeftShearLinks;
                result[i, 25] = rcBeam.MidShearLinks;
                result[i, 26] = rcBeam.RightShearLinks;

                result[i, 27] = rcBeam.LeftTopAsProv;
                result[i, 28] = rcBeam.MidTopAsProv;
                result[i, 29] = rcBeam.RightTopAsProv;
                result[i, 30] = rcBeam.LeftBottomAsProv;
                result[i, 31] = rcBeam.MidBottomAsProv;
                result[i, 32] = rcBeam.RightBottomAsProv;

                result[i, 33] = rcBeam.LeftAvProv;
                result[i, 34] = rcBeam.MidAvProv;
                result[i, 35] = rcBeam.RightAvProv;

                result[i, 36] = rcBeam.RatioLeftTopAs;
                result[i, 37] = rcBeam.RatioMidTopAs;
                result[i, 38] = rcBeam.RatioRightTopAs;
                result[i, 39] = rcBeam.RatioLeftBottomAs;
                result[i, 40] = rcBeam.RatioMidBottomAs;
                result[i, 41] = rcBeam.RatioRightBottomAs;

                result[i, 42] = rcBeam.RatioLeftAv;
                result[i, 43] = rcBeam.RatioMidAv;
                result[i, 44] = rcBeam.RatioRightAv;

                result[i, 45] = rcBeam.EtabsComment;
                result[i, 46] = rcBeam.AsEvaluationComment;
                result[i, 47] = rcBeam.AvEvaluationComment;

                result[i, 48] = rcBeam.Cluster;

                i++;
            }

            return result;
        }

        private static object[,] CompileResutRcBeamSelfDesignToArray(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "fyk (Mpa)", "fck (Mpa)", "d (ETABS) (mm)", "d calculated (mm)", "Beam type", "As Min(mm2)", "Load case"
                , "Left Moment (-) (kNm)", "Mid Moment (-) (kNm)", "Right Moment (-) (kNm)", "Left Moment (+) (kNm)", "Mid Moment (+) (kNm)", "Right Moment (+) (kNm)"
                , "Left Shear (kN)", "Mid Shear (kN)", "Right Shear (kN)"
                , "Left Top As (mm2)", "Mid Top As (mm2)", "Right Top As (mm2)", "Left Bottom As (mm2)", "Mid Bottom As (mm2)", "Right Bottom As (mm2)"
                , "Left Av (mm2/m)", "Mid Av (mm2/m)", "Right Av (mm2/m)"
                , "Left Top Rebar", "Mid Top Rebar", "Right Top Rebar", "Left Bottom Rebar", "Mid Bottom Rebar", "Right Bottom Rebar"
                , "Left Shear Links", "Mid Shear Links", "Right Shear Links"
                , "Left Top As Prov (mm2)", "Mid Top As Prov (mm2)", "Right Top As Prov (mm2)", "Left Bottom As  Prov (mm2)", "Mid Bottom As Prov (mm2)", "Right Bottom As Prov (mm2)"
                , "Left Av Prov (mm2/m)", "Mid Av Prov (mm2/m)", "Right Av Prov (mm2/m)"
                , "Left Top As Ratio", "Mid Top As Ratio", "Right Top As Ratio", "Left Bottom As Ratio", "Mid Bottom As Ratio", "Right Bottom As Ratio"
                , "Left Av Ratio", "Mid Av Ratio", "Right Av Ratio"
                , "ETABS warning", "Rebar warning", "Shear links warning",
                "Cluster"
            };

            int numOfRow = rcBeamList.Count() * rcBeamList[0].FrameForceEachCaseList.Count();
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
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                foreach (src.HelperEtabs.FrameForceEachCase frameForceEachCase in rcBeam.FrameForceEachCaseList)
                {
                    result[i, 0] = rcBeam.UniqueName;
                    result[i, 1] = rcBeam.Width;
                    result[i, 2] = rcBeam.Height;
                    result[i, 3] = rcBeam.Fyk;
                    result[i, 4] = rcBeam.Fck;
                    result[i, 5] = rcBeam.d;
                    result[i, 6] = rcBeam.dCalculated;
                    result[i, 7] = rcBeam.BeamType;
                    result[i, 8] = rcBeam.AsMin;

                    result[i, 9] = frameForceEachCase.LoadCaseName;

                    result[i, 10] = frameForceEachCase.LeftMomentNeg;
                    result[i, 11] = frameForceEachCase.MidMomentNeg;
                    result[i, 12] = frameForceEachCase.RightMomentNeg;
                    result[i, 13] = frameForceEachCase.LeftMomentPos;
                    result[i, 14] = frameForceEachCase.MidMomentPos;
                    result[i, 15] = frameForceEachCase.RightMomentPos;

                    result[i, 16] = frameForceEachCase.LeftShear;
                    result[i, 17] = frameForceEachCase.MidShear;
                    result[i, 18] = frameForceEachCase.RightShear;

                    i++;
                    Console.WriteLine(i);
                }

            }

            return result;
        }

        public static void WriteResultRcBeamToExcel(object[,] result, object[,] result2)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            xlApp.Visible = true;

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //Add worksheets
            xlWorkBook.Sheets.Add(); //Self design

            //Sheet 1
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "ETABS design";

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

            //Sheet 2
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            xlWorkSheet.Name = "Force envelope";

            numOfRow = result2.GetLength(0);
            numOfCol = result2.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result2;

            /*
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
            */
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

            return Math.Ceiling(asMin);
        }

        public static void CompareToMinimumRcBeamRebar(src.HelperEtabs.RcBeam rcBeam)
        {
            double asMin = 0;
            asMin = GetMinimumRcBeamRebarEC(rcBeam.Fck, rcBeam.Fyk, rcBeam.Width, rcBeam.d);

            if (rcBeam.BeamType == "Single/multi span")
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

        public static void CheckMessageRcBeamRebar(List<src.HelperEtabs.RcBeam> rcBeamList, src.HelperEtabs.DatabaseTable rcBeamDesignSummary)
        {
            //Get table
            List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            bool popUpShown = false;

            //Getting message for each beam
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                bool messageTaken = false;
                double length = 0;

                rcBeam.EtabsComment = "";

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {

                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        if (row["WarnMsg"] != "No Message")
                        {
                            rcBeam.EtabsComment += row["WarnMsg"];
                            messageTaken = true;

                            //Show warning if ETABS has warning!
                            if (!popUpShown)
                            {
                                MessageBox.Show($"Warning! ETABS has warning message!");
                                popUpShown = true;
                            }
                        }

                        if (row["ErrMsg"] != "No Message")
                        {
                            rcBeam.EtabsComment += row["ErrMsg"];
                            messageTaken = true;

                            //Show warning if ETABS has warning!
                            if (!popUpShown)
                            {
                                MessageBox.Show($"Warning! ETABS has warning message!");
                                popUpShown = true;
                            }
                        }

                        if (messageTaken)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static void CalculateDFromRebar(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            foreach(src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                double coverToRebarCentroid = 0;
                double maxCoverToRebarCentroid = 0;
                double centroidFromFirstRebarCentroid = 0;

                //Left top
                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.LeftTopRebarDiameterList, rcBeam.LeftTopNoRebarPerLayer);
                coverToRebarCentroid = rcBeam.Cover + rcBeam.LeftShearLinksDiameter + 0.5 * rcBeam.LeftTopRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                //Left bottom
                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.LeftBottomRebarDiameterList, rcBeam.LeftBottomNoRebarPerLayer);
                coverToRebarCentroid = rcBeam.Cover + rcBeam.LeftShearLinksDiameter + 0.5 * rcBeam.LeftBottomRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                if (rcBeam.BeamType == "Single/multi span")
                {
                    //Mid top
                    centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.MidTopRebarDiameterList, rcBeam.MidTopNoRebarPerLayer);
                    coverToRebarCentroid = rcBeam.Cover + rcBeam.MidShearLinksDiameter + 0.5 * rcBeam.MidTopRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                    maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                    //Mid bottom
                    centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.MidBottomRebarDiameterList, rcBeam.MidBottomNoRebarPerLayer);
                    coverToRebarCentroid = rcBeam.Cover + rcBeam.MidShearLinksDiameter + 0.5 * rcBeam.MidBottomRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                    maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);
                }

                //Right top
                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.RightTopRebarDiameterList, rcBeam.RightTopNoRebarPerLayer);
                coverToRebarCentroid = rcBeam.Cover + rcBeam.RightShearLinksDiameter + 0.5 * rcBeam.RightTopRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                //Right bottom
                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcBeam.RightBottomRebarDiameterList, rcBeam.RightBottomNoRebarPerLayer);
                coverToRebarCentroid = rcBeam.Cover + rcBeam.RightShearLinksDiameter + 0.5 * rcBeam.RightBottomRebarDiameterList[0] + centroidFromFirstRebarCentroid;
                maxCoverToRebarCentroid = Math.Max(coverToRebarCentroid, maxCoverToRebarCentroid);

                //Save d calcualated
                rcBeam.dCalculated = Math.Floor(rcBeam.Height - maxCoverToRebarCentroid);

                //Give warning if d calculated < d ETABS
                if (rcBeam.dCalculated < rcBeam.d)
                {
                    if (rcBeam.AsEvaluationComment == null)
                    {
                        rcBeam.AsEvaluationComment += "d calculated < d ETABS";
                    }
                    else
                    {
                        rcBeam.AsEvaluationComment += ", d calculated < d ETABS";
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

                if (rcBeam.BeamType == "Single/multi span")
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

                if (rcBeam.BeamType == "Single/multi span")
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
                    sumArea += 0.25 * Math.PI * Math.Pow(rebarDiameter, 2) * noRebarPerLayer[index];

                    //Calculate centroid for next layer
                    if (index < rebarDiameterList.Count - 1)
                    {
                        eachLayerCentroid += 0.5 * rebarDiameter + Math.Max(25, Math.Max(rebarDiameterList[index + 1], rebarDiameter)) + 0.5 * rebarDiameterList[index + 1];
                    }

                    index++;
                }

                double centroid = sumAreaTimesCentroid / sumArea;
                return centroid;
            }

        }

        public static void GetForcesEnvelope(cSapModel Sap, List<HelperEtabs.RcBeam> rcBeamList, 
            List<Dictionary<string, string>> groupAndBeamType)
        {
            //List to save list of load combo used in rc design
            List<string> rcFrameDesignLoadComboList = new List<string>();

            //Extraction of load combo list using database
            src.HelperEtabs.DatabaseTable rcDesignLoadComboDatabase = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string tableKey = "Concrete Frame Design Load Combination Data";
            (isError, rcDesignLoadComboDatabase) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            List<Dictionary<string, string>> rcDesignLoadComboTable = rcDesignLoadComboDatabase.TableInList;

            foreach (Dictionary<string, string> row in rcDesignLoadComboTable)
            {
                if (row["ComboType"] == "Strength")
                {
                    rcFrameDesignLoadComboList.Add(row["ComboName"]);
                }
            }

            //Get group to use to filter database so not very long
            List<string> groupNameList = new List<string>();
            foreach (Dictionary<string, string> groupWithBeamType in groupAndBeamType)
            {
                string groupName = groupWithBeamType["groupName"];
                groupNameList.Add(groupName);
            }


            //Extraction of forces list using database
            src.HelperEtabs.DatabaseTable elementForceBeamDatabase = new src.HelperEtabs.DatabaseTable();

            tableKey = "Element Forces - Beams";
            (isError, elementForceBeamDatabase) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey, groupNameList, false);

            List<Dictionary<string, string>> elementForceBeamTable = elementForceBeamDatabase.TableInList;

            //Getting force envelope for each beam
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {

                //Getting force envelope for each combo
                foreach (string rcFrameDesignLoadCombo in rcFrameDesignLoadComboList)
                {
                    src.HelperEtabs.FrameForceEachCase frameForceEachCase = new HelperEtabs.FrameForceEachCase(rcFrameDesignLoadCombo);

                    double leftMomentNeg = 0;
                    double leftMomentPos = 0;
                    double midMomentNeg = 0;
                    double midMomentPos = 0;
                    double rightMomentNeg = 0;
                    double rightMomentPos = 0;

                    double leftShear = 0;
                    double midShear = 0;
                    double rightShear = 0;

                    bool firstLeft = true;
                    bool firstMid = true;
                    bool firstRight = true;

                    //if mid is not found, means is short beam
                    bool midNotFound = true;

                    foreach (Dictionary<string, string> row in elementForceBeamTable)
                    {
                        //Single or multispan
                        if (row["UniqueName"] == rcBeam.UniqueName && rcBeam.BeamType == "Single/multi span" && rcFrameDesignLoadCombo == row["OutputCase"])
                        {
                            if (Convert.ToDouble(row["Station"]) <= rcBeam.LeftStation)
                            {
                                if (firstLeft)
                                {
                                    leftMomentNeg = Convert.ToDouble(row["M3"]);
                                    leftMomentPos = Convert.ToDouble(row["M3"]);
                                    leftShear = Math.Abs(Convert.ToDouble(row["V2"]));

                                    firstLeft = false;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["M3"]) < leftMomentNeg)
                                    {
                                        leftMomentNeg = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Convert.ToDouble(row["M3"]) > leftMomentPos)
                                    {
                                        leftMomentPos = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Math.Abs(Convert.ToDouble(row["V2"])) > leftShear)
                                    {
                                        leftShear = Math.Abs(Convert.ToDouble(row["V2"]));
                                    }
                                }
                            }
                            else if (Convert.ToDouble(row["Station"]) >= rcBeam.RightStation)
                            {
                                if (firstRight)
                                {
                                    rightMomentNeg = Convert.ToDouble(row["M3"]);
                                    rightMomentPos = Convert.ToDouble(row["M3"]);
                                    rightShear = Math.Abs(Convert.ToDouble(row["V2"]));

                                    firstRight = false;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["M3"]) < rightMomentNeg)
                                    {
                                        rightMomentNeg = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Convert.ToDouble(row["M3"]) > rightMomentPos)
                                    {
                                        rightMomentPos = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Math.Abs(Convert.ToDouble(row["V2"])) > rightShear)
                                    {
                                        rightShear = Math.Abs(Convert.ToDouble(row["V2"]));
                                    }
                                }
                            }
                            else
                            {
                                if (firstMid)
                                {
                                    midMomentNeg = Convert.ToDouble(row["M3"]);
                                    midMomentPos = Convert.ToDouble(row["M3"]);
                                    midShear = Math.Abs(Convert.ToDouble(row["V2"]));

                                    firstMid = false;
                                    midNotFound = false;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["M3"]) < midMomentNeg)
                                    {
                                        midMomentNeg = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Convert.ToDouble(row["M3"]) > midMomentPos)
                                    {
                                        midMomentPos = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Math.Abs(Convert.ToDouble(row["V2"])) > midShear)
                                    {
                                        midShear = Math.Abs(Convert.ToDouble(row["V2"]));
                                    }
                                }

                            }
                        }
                        //Cantilever
                        else if (row["UniqueName"] == rcBeam.UniqueName && rcBeam.BeamType == "Cantilever" && rcFrameDesignLoadCombo == row["OutputCase"])
                        {

                            if (Convert.ToDouble(row["Station"]) <= rcBeam.LeftStation)
                            {
                                if (firstLeft)
                                {
                                    leftMomentNeg = Convert.ToDouble(row["M3"]);
                                    leftMomentPos = Convert.ToDouble(row["M3"]);
                                    leftShear = Math.Abs(Convert.ToDouble(row["V2"]));

                                    firstLeft = false;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["M3"]) < leftMomentNeg)
                                    {
                                        leftMomentNeg = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Convert.ToDouble(row["M3"]) > leftMomentPos)
                                    {
                                        leftMomentPos = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Math.Abs(Convert.ToDouble(row["V2"])) > leftShear)
                                    {
                                        leftShear = Math.Abs(Convert.ToDouble(row["V2"]));
                                    }
                                }
                            }
                            else
                            {
                                if (firstRight)
                                {
                                    rightMomentNeg = Convert.ToDouble(row["M3"]);
                                    rightMomentPos = Convert.ToDouble(row["M3"]);
                                    rightShear = Math.Abs(Convert.ToDouble(row["V2"]));

                                    firstRight = false;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["M3"]) < rightMomentNeg)
                                    {
                                        rightMomentNeg = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Convert.ToDouble(row["M3"]) > rightMomentPos)
                                    {
                                        rightMomentPos = Convert.ToDouble(row["M3"]);
                                    }

                                    if (Math.Abs(Convert.ToDouble(row["V2"])) > rightShear)
                                    {
                                        rightShear = Math.Abs(Convert.ToDouble(row["V2"]));
                                    }
                                }
                            }
                        }
                    }


                    //if short beam
                    if (midNotFound && rcBeam.BeamType == "Single/multi span")
                    {
                        frameForceEachCase.LeftMomentNeg = Math.Ceiling(leftMomentNeg * Math.Pow(10, -6)); //Nmm to kNm
                        frameForceEachCase.LeftMomentPos = Math.Ceiling(leftMomentPos * Math.Pow(10, -6));
                        frameForceEachCase.MidMomentNeg = Math.Ceiling(Math.Max(leftMomentNeg, rightMomentNeg) * Math.Pow(10, -6));
                        frameForceEachCase.MidMomentPos = Math.Ceiling(Math.Max(leftMomentPos, rightMomentPos) * Math.Pow(10, -6));
                        frameForceEachCase.RightMomentNeg = Math.Ceiling(rightMomentNeg * Math.Pow(10, -6));
                        frameForceEachCase.RightMomentPos = Math.Ceiling(rightMomentPos * Math.Pow(10, -6));

                        frameForceEachCase.LeftShear = Math.Ceiling(leftShear * Math.Pow(10, -3)); //N to kN
                        frameForceEachCase.MidShear = Math.Ceiling(Math.Max(leftShear, rightShear) * Math.Pow(10, -3));
                        frameForceEachCase.RightShear = Math.Ceiling(rightShear * Math.Pow(10, -3));
                    }
                    else
                    {
                        frameForceEachCase.LeftMomentNeg = Math.Ceiling(leftMomentNeg * Math.Pow(10, -6));
                        frameForceEachCase.LeftMomentPos = Math.Ceiling(leftMomentPos * Math.Pow(10, -6));
                        frameForceEachCase.MidMomentNeg = Math.Ceiling(midMomentNeg * Math.Pow(10, -6));
                        frameForceEachCase.MidMomentPos = Math.Ceiling(midMomentPos * Math.Pow(10, -6));
                        frameForceEachCase.RightMomentNeg = Math.Ceiling(rightMomentNeg * Math.Pow(10, -6));
                        frameForceEachCase.RightMomentPos = Math.Ceiling(rightMomentPos * Math.Pow(10, -6));

                        frameForceEachCase.LeftShear = Math.Ceiling(leftShear * Math.Pow(10, -3));
                        frameForceEachCase.MidShear = Math.Ceiling(midShear * Math.Pow(10, -3));
                        frameForceEachCase.RightShear = Math.Ceiling(rightShear * Math.Pow(10, -3));
                    }

                    //Save force each case to rcbeam
                    rcBeam.FrameForceEachCaseList.Add(frameForceEachCase);
                }
            }
        }

        public static void RcBeamClusteringUsingKMeans(List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            List<ClassFunctionRCBSched.RcBeamForMLInput> rcBeamForMLInputList = new List<ClassFunctionRCBSched.RcBeamForMLInput>();

            //Convert RcBeam to RcBeamForML
            foreach(src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                ClassFunctionRCBSched.RcBeamForMLInput rcBeamForML = new ClassFunctionRCBSched.RcBeamForMLInput(rcBeam.UniqueName, (float)rcBeam.Height, (float)rcBeam.Width, (float)rcBeam.LeftTopAs, (float)rcBeam.MidTopAs,
                    (float)rcBeam.RightTopAs, (float)rcBeam.LeftBottomAs, (float)rcBeam.MidBottomAs, (float)rcBeam.RightBottomAs, (float)rcBeam.LeftAv, (float)rcBeam.MidAv, (float)rcBeam.RightAv);
                rcBeamForMLInputList.Add(rcBeamForML);
            }

            //Normalize data
            NormalizeData(rcBeamForMLInputList);


            //Prepare data for ML
            double[][] observations = new double[rcBeamForMLInputList.Count()][];

            int index = 0;
            foreach(ClassFunctionRCBSched.RcBeamForMLInput rcBeamForMLInput in rcBeamForMLInputList)
            {
                observations[index] = new double[]
                {
                    rcBeamForMLInput.Width,
                    rcBeamForMLInput.Height,
                    rcBeamForMLInput.LeftTopAs,
                    rcBeamForMLInput.MidTopAs,
                    rcBeamForMLInput.RightTopAs,
                    rcBeamForMLInput.LeftBottomAs,
                    rcBeamForMLInput.MidBottomAs,
                    rcBeamForMLInput.RightBottomAs,
                    rcBeamForMLInput.LeftAv,
                    rcBeamForMLInput.MidAv,
                    rcBeamForMLInput.RightAv,
                };
                index++;
            }

            //Call up ML preference window
            WindowRCBSchedMLPrefence windowRCBSchedMLPrefence = new WindowRCBSchedMLPrefence(observations);
            int[] labels = windowRCBSchedMLPrefence.GetLabels();

            /*
            //TEST---------------------------------------------

            double[] x = new double[] { -3,-4,-3,-4,-5,-4,-10,-12,-9,-13,-10,-9,10,12,14,15,13,15,10,12,15,14,13,14 };
            double[] y = new double[] { 3,4,5,6,7,4,-10,-10,-8,-12,-9,-10,-3,-4,-7,-5,-4,-1,10,12,15,14,13,15 };

            observations = new double[x.Count()][];

            rcBeamForMLInputList = new List<ClassFunctionRCBSched.RcBeamForMLInput>();
            rcBeamList = new List<HelperEtabs.RcBeam>();

            index = 0;
            foreach (double xi in x)
            {
                observations[index] = new double[]
                {
                    x[index], y[index]
                };
                index++;
                ClassFunctionRCBSched.RcBeamForMLInput rcBeamForML = new ClassFunctionRCBSched.RcBeamForMLInput("A", 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0);
                rcBeamForMLInputList.Add(rcBeamForML);

            }

            //------------------------------------------------
            */

            //Get optimal num of clusters
            //HelperML.CalculateOptimalNumOfClusters(observations);

            /*
            //PCA to 2 axis

            MLContext mlContext = new MLContext();

            IDataView dataView = mlContext.Data.LoadFromEnumerable(rcBeamForMLInputList);

            string featuresColumnName = "Features";
            var pipelinePCA = mlContext.Transforms
                .Concatenate(featuresColumnName, "Width", "Height", "LeftTopAs", "MidTopAs", "RightTopAs", "LeftBottomAs", "MidBottomAs",
                "RightBottomAs", "LeftAv", "RightAv")
                .Append(mlContext.Transforms.ProjectToPrincipalComponents(outputColumnName:"PCAFeatures", featuresColumnName, rank: 2));

            var modelPCA = pipelinePCA.Fit(dataView);

            var transformedDataView = modelPCA.Transform(dataView);

            var PCAResults = mlContext.Data.CreateEnumerable<ClassFunctionRCBSched.PCAResult>(transformedDataView, reuseRowObject: false).ToList();

            int index = 0;
            foreach (var result in PCAResults)
            {
                rcBeamForMLInputList[index].PC1 = result.PCAFeatures[0];
                rcBeamForMLInputList[index].PC2 = result.PCAFeatures[1];
                index++;
            }
            */

            /*
            //PCA to 2 axis
            var method = PrincipalComponentMethod.Center;
            var pca = new KernelPrincipalComponentAnalysis(new Linear(), method);
            pca.Learn(observations);
            pca.NumberOfOutputs = 2;
            double[][] PCAresults = pca.Transform(observations);

            index = 0;
            foreach (var result in PCAresults)
            {
                rcBeamForMLInputList[index].PC1 = (float)result[0];
                rcBeamForMLInputList[index].PC2 = (float)result[1];
                index++;
            }
            */

            /*
            //Plot error chart
            int minNumCluster = 1;
            int maxNumCluster = 30;
            List<double> numClusterList = new List<double>();
            List<double> errorList = new List<double>();

            for(int numClusters = minNumCluster; numClusters < maxNumCluster; numClusters++)
            {

                KMeans kmeans = new KMeans(k: numClusters);
                var clusters = kmeans.Learn(observations);
                var error = kmeans.Error;

                numClusterList.Add(numClusters);
                errorList.Add(error);
            }
            

            ML.WindowRCBSchedKMeans windowRCBSchedKMeans = new ML.WindowRCBSchedKMeans(numClusterList, errorList);
            */


            //Parsing cluster back to rcBeamList
            index = 0;
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                rcBeam.Cluster =  Convert.ToString(labels[index]);
                index++;
            }
            

         
            /*
            //ML.NET KMeans
            int numClusters = 10;
            IDataView dataViewKMeans = mlContext.Data.LoadFromEnumerable(rcBeamForMLInputList);
            string featuresColumnNameKMeans = "Features";
            
            var pipelineKMeans = mlContext.Transforms
                .Concatenate(featuresColumnNameKMeans, "Width", "Height", "LeftTopAs", "MidTopAs", "RightTopAs", "LeftBottomAs", "MidBottomAs", 
                "RightBottomAs", "LeftAv", "RightAv")
                .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnNameKMeans, numberOfClusters: numClusters));

            
            //var pipelineKMeans = mlContext.Transforms
                //.Concatenate(featuresColumnName, "PC1", "PC2")
                //.Append(mlContext.Clustering.Trainers.KMeans(featuresColumnNameKMeans, numberOfClusters: numClusters));
            
            var modelKMeans = pipelineKMeans.Fit(dataViewKMeans);
            
            //Predict
            var predEngine = mlContext.Model.CreatePredictionEngine<RcBeamForMLInput, RcBeamForMLOutput>(modelKMeans);

            foreach (ClassFunctionRCBSched.RcBeamForMLInput rcBeamForMLInput in rcBeamForMLInputList)
            {
                RcBeamForMLOutput predictionResult = predEngine.Predict(rcBeamForMLInput);
                rcBeamForMLInput.Cluster = Convert.ToString(predictionResult.PredictedClusterId);
            }

            //Parsing cluster back to rcBeamList
            index = 0;
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                rcBeam.Cluster = rcBeamForMLInputList[index].Cluster;
                index++;
            }
            */

            //WindowRCBSchedPCAResult windowRCBSchedPCAResult = new WindowRCBSchedPCAResult(rcBeamForMLInputList, numClusters);
        }

        public static void NormalizeData(List<ClassFunctionRCBSched.RcBeamForMLInput> rcBeamForMLInputList)
        {
            string[] properties = { "Height", "Width",
            "LeftTopAs",
            "MidTopAs",
            "RightTopAs",
            "LeftBottomAs",
            "MidBottomAs",
            "RightBottomAs",
            "LeftAv",
            "MidAv",
            "RightAv" };

            foreach (string  property in properties)
            {

                //Search for max and min
                //bool firstLoop = true;
                //double max = 0;
                //double min = 0;
                List<double> listOfValues = new List<double>();

                foreach(ClassFunctionRCBSched.RcBeamForMLInput rcBeamForMLInput in rcBeamForMLInputList)
                {
                    //Do log transformation
                    double originalValue = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                    float logTransormedValue = (float) Math.Log(originalValue + 1);
                    rcBeamForMLInput.GetType().GetProperty(property).SetValue(rcBeamForMLInput, logTransormedValue);

                    listOfValues.Add(logTransormedValue);

                    /*
                    if (firstLoop)
                    {
                        max = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                        min = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                        firstLoop = false;
                    }
                    else
                    {
                        if (Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null)) > max)
                        {
                            max = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                        }
                        else if (Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null)) < min)
                        {
                            min = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                        }
                    }
                    */
                }

                //Caculate mean and std dev
                double mean = listOfValues.Average();
                double stdDev = HelperMethod.GetStandardDeviation(listOfValues);

                if (stdDev == 0)
                {
                    stdDev = 0.001;
                }

                //Conversion
                foreach (ClassFunctionRCBSched.RcBeamForMLInput rcBeamForMLInput in rcBeamForMLInputList)
                {
                    double originalValue = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                    float normalizedValue = (float)((originalValue - mean) / stdDev);

                    /*
                    double originalValue = Convert.ToDouble(rcBeamForMLInput.GetType().GetProperty(property).GetValue(rcBeamForMLInput, null));
                    float normalizedValue = (float) ((originalValue - min) / (max - min));
                    */

                    rcBeamForMLInput.GetType().GetProperty(property).SetValue(rcBeamForMLInput, normalizedValue);
                    
                }

            }

        }
    }
}
