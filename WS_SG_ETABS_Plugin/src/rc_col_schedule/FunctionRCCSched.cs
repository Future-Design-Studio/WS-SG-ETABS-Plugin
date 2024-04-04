  using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WS_SG_ETABS_Plugin.src.ML;
using WS_SG_ETABS_Plugin.src.rc_beam_schedule;

namespace WS_SG_ETABS_Plugin.src.rc_col_schedule
{
    internal class FunctionRCCSched
    {

        private static int Ret;

        public static bool GetRCColumnSchedule(cSapModel Sap, cPluginCallback Plugin, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing,
            double minShearLinksSpacing, double shearLinksSpacingStep, double minimumRebarPercentage, double cover, int maxNoLayer, int[] rebarDiameterList, int[] shearLinksDiameterList,
            List<string> listChoosenGroupName, bool? doCluster)
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
            GetRcColumnRebarArea(Sap, rcColumnDesignSummary, rcColumnList, minimumRebarPercentage);
                        
            //Convert area to rebar
            DesignRcColumnRebar(rcColumnList, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, rebarDiameterList, shearLinksDiameterList, maxNoLayer);

            //Check etabs message
            CheckMessageRcColRebar(rcColumnList, rcColumnDesignSummary);

            //Get d from rebar
            CalculateDFromRebar(rcColumnList);

            //Calculate As provided
            CalculateAsProv(rcColumnList);

            //Calculate Av provided
            CalculateAvProv(rcColumnList);

            //Do ML
            if (doCluster == true)
            {
                //RcColClusteringUsingKMeans(rcColumnList);
                RcColClustering(rcColumnList);
            }

            //Compile data to 2d array
            var result = CompileResutRcBeamToArray(rcColumnList);
            
            //Write to excel
            WriteResultRcColumnToExcel(result);
           
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

                        //Calculate d
                        double d = 0;
                        switch (shape)
                        {
                            case "Rectangular":
                                d = height - coverEtabs - shearLinksDiameterEtabs - 0.5 * rebarDiameterEtabs;
                                rcColumn.d = d;
                                break;
                            case "Circle":
                                d = diameter - coverEtabs - shearLinksDiameterEtabs - 0.5 * rebarDiameterEtabs;
                                rcColumn.d = d;
                                break;
                        }

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

        public static void GetRcColumnRebarArea(cSapModel Sap, src.HelperEtabs.DatabaseTable rcColumnDesignSummary, List<src.HelperEtabs.RcColumn> rcColumnList, double minimumRebarPercentage)
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

            //Getting rebar area for each column
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
                            As = (Convert.ToDouble(row["As"]));//mm2
                            avMajor = (Convert.ToDouble(row["VMajRebar"])) * 1000; //mm2/mm convert to mm2/m
                            avMinor = (Convert.ToDouble(row["VMinRebar"])) * 1000; //mm2/mm convert to mm2/m

                            first = false;
                        }
                        else
                        {
                            if (Convert.ToDouble(row["As"]) > As)
                            {
                                As = (Convert.ToDouble(row["As"])); //mm2
                            }

                            if (Convert.ToDouble(row["VMajRebar"]) > avMajor)
                            {
                                avMajor = (Convert.ToDouble(row["VMajRebar"])) * 1000; //mm2/mm convert to mm2/m
                            }

                            if (Convert.ToDouble(row["VMinRebar"]) > avMinor)
                            {
                                avMinor = (Convert.ToDouble(row["VMinRebar"])) * 1000; //mm2/mm convert to mm2/m
                            }
                        }
                    }
                }

                rcColumn.As = As;
                rcColumn.AvMajor = avMajor;
                rcColumn.AvMinor = avMinor;

                
                //Check againts minimum rebar
                CompareToMinimumRcColumnRebar(rcColumn, minimumRebarPercentage);
                

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
                double diameter = rcColumn.Diameter;

                string shape = rcColumn.Shape;

                double numRebarLayer = 0;
                List<int> numRebarPerLayer = new List<int>();
                List<int> rebarDiameter = new List<int>();
                string asEvaluationComment = "";
                double numShearLinksLegMajor = 0;
                double numShearLinksLegMinor = 0;
                double shearLinksDiameter = 0;
                double shearLinksSpacing = 0;
                string avEvaluationComment = "";

                double avReqMajor = avMajor;
                double avReqMinor = avMinor;

                (numRebarLayer, numRebarPerLayer, rebarDiameter, asEvaluationComment, numShearLinksLegMajor, numShearLinksLegMinor, shearLinksDiameter, shearLinksSpacing,
                    avEvaluationComment, warningRebarDiameterTooBigShown) = ConvertRcColumnAreaToRebar(
                    As, avReqMajor, avMinor, width, height, diameter, shape, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover, shearLinksDiameterList,
                    rebarDiameterList, maxNoLayer, rcColumn.UniqueName, warningRebarDiameterTooBigShown);
                
                rcColumn.Rebar = "";
                for (int layerIndex = 0; layerIndex < rebarDiameter.Count(); layerIndex++)
                {
                    rcColumn.Rebar += numRebarPerLayer[layerIndex] + "H" + rebarDiameter[layerIndex] + "+";
                }
                rcColumn.Rebar = rcColumn.Rebar.Remove(rcColumn.Rebar.Length - 1, 1);
                rcColumn.RebarDiameterList = rebarDiameter;
                rcColumn.NoRebarPerLayer = numRebarPerLayer;

                rcColumn.ShearLinksMajor = "H" + shearLinksDiameter + "-" + shearLinksSpacing + " (Link)";
                if (numShearLinksLegMajor > 2)
                {
                    rcColumn.ShearLinksMajor += " + " + (numShearLinksLegMajor - 2) + "H" + shearLinksDiameter + "-" + shearLinksSpacing + " (Hooks)";
                }
                rcColumn.NumShearLinksLegMajor = numShearLinksLegMajor;

                rcColumn.ShearLinksMinor = "H" + shearLinksDiameter + "-" + shearLinksSpacing + " (Link)";
                if (numShearLinksLegMinor > 2)
                {
                    rcColumn.ShearLinksMinor += " + " + (numShearLinksLegMinor - 2) + "H" + shearLinksDiameter + "-" + shearLinksSpacing + " (Hooks)";
                }
                rcColumn.NumShearLinksLegMinor = numShearLinksLegMinor;

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
            }
        }
        
        private static (double, List<int>, List<int>, string, double, double, double, double, string, bool) ConvertRcColumnAreaToRebar(double asReq, double avReqMajor, double avReqMinor, double width, double height,
            double diameter, string shape, double rebarSpc, string rebarSpcPreference, double maxShearLinksSpacing, double minShearLinksSpacing, double shearLinksSpacingStep, double cover, int[] shearLinksDiameterList, 
            int[] rebarDiameterList, int maxNoLayer, string uniqueName, bool warningRebarDiameterTooBigShown)
        {
            int shearLinksDiameterIndex = 0;
            double shearLinksDiameter = 0;
            bool shearLinksMajorSatisfied = false;
            bool shearLinksMinorSatisfied = false;
            double noShearLinksLegMajor = 0;
            double noShearLinksLegMinor = 0;
            double shearLinksSpacing = 0;

            List<int> rebarDiameterIndexPerLayer = new List<int>();
            List<int> rebarDiameterPerLayer = new List<int>();
            List<int> noRebarPerLayer = new List<int>();
            bool rebarSatisfied = false;
            int noRebarLayer = 0;
            int maxRebarDiameterInAllLayer = 0;

            int maxNoRebarPerLayer = 0;
            int minNoRebarPerLayer = 0;

            double widthNoRebar = 0;
            double heightNoRebar = 0;
            double widthSpace = 0;
            double heightSpace = 0;

            double diameterSpace = 0;
            double circumferenceSpace = 0;

            //Keep looping if not both satisfied
            while (shearLinksDiameterIndex <= shearLinksDiameterList.Count() - 1 && !shearLinksMajorSatisfied && !shearLinksMinorSatisfied)
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
                        switch (shape)
                        {
                            case "Rectangular":

                                switch (rebarSpcPreference)
                                {
                                    case "clear":
                                        widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        widthNoRebar = Math.Floor(widthSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1);
                                        heightNoRebar = Math.Floor(heightSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1);
                                        maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar * 2 + heightNoRebar * 2 - 4);
                                        break;

                                    case "ctoc":
                                        widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        widthNoRebar = Math.Floor(widthSpace / (rebarSpc) + 1);
                                        heightNoRebar = Math.Floor(heightSpace / (rebarSpc) + 1);
                                        maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar * 2 + heightNoRebar * 2 - 4);
                                        break;

                                    case "auto":
                                        rebarSpc = Math.Max(25, maxRebarDiameterInAllLayer);
                                        widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        widthNoRebar = Math.Floor(widthSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1);
                                        heightNoRebar = Math.Floor(heightSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1);
                                        maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar * 2 + heightNoRebar * 2 - 4);
                                        break;

                                    case "autoRamboll":
                                        widthSpace = width - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        heightSpace = height - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        widthNoRebar = Math.Floor(width / 100);
                                        heightNoRebar = Math.Floor(height / 100);
                                        maxNoRebarPerLayer = Convert.ToInt32(widthNoRebar * 2 + heightNoRebar * 2 - 4);
                                        break;
                                }

                                break;

                            case "Circle":

                                switch (rebarSpcPreference)
                                {
                                    case "clear":
                                        diameterSpace = diameter - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        circumferenceSpace = Math.PI * diameterSpace;
                                        maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(circumferenceSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1));
                                        break;

                                    case "ctoc":
                                        diameterSpace = diameter - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        circumferenceSpace = Math.PI * diameterSpace;
                                        maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(circumferenceSpace / (rebarSpc) + 1));
                                        break;

                                    case "auto":
                                        rebarSpc = Math.Max(25, maxRebarDiameterInAllLayer);
                                        diameterSpace = diameter - cover * 2 - shearLinksDiameter * 2 - maxRebarDiameterInAllLayer;
                                        circumferenceSpace = Math.PI * diameterSpace;
                                        maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(circumferenceSpace / (rebarSpc + maxRebarDiameterInAllLayer) + 1));
                                        break;

                                    case "autoRamboll":
                                        circumferenceSpace = Math.PI * diameter;
                                        maxNoRebarPerLayer = Convert.ToInt32(Math.Floor(circumferenceSpace / 100));
                                        break;
                                }
                                break;
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
                                //Increase no of rebar in the specific layer
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
                            //Increase rebar diameter
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
                        //Add 1 more rebar layer
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

                switch (shape)
                {
                    case "Rectangular":

                        //Calculate minimum number of shear links leg based on 150mm max distance compression rebar from confined rebar
                        double minNoShearLinksLegWidth = Math.Ceiling(widthSpace / 300) - 1 + 2; //+2 for the link
                        double minNoShearLinksLegHeight = Math.Ceiling(heightSpace / 300) - 1 + 2; //+2 for the link

                        //Evaluate local axis based on width and height dimension
                        double noRebarMajor = 0;
                        double noRebarMinor = 0;
                        double minNoShearLinksLegMajor = 0;
                        double minNoShearLinksLegMinor = 0;
                        if (width > height)
                        {
                            noRebarMajor = heightNoRebar;
                            noRebarMinor = widthNoRebar;
                            minNoShearLinksLegMajor = minNoShearLinksLegHeight;
                            minNoShearLinksLegMinor = minNoShearLinksLegWidth;
                        }
                        else
                        {
                            noRebarMajor = widthNoRebar;
                            noRebarMinor = heightNoRebar;
                            minNoShearLinksLegMajor = minNoShearLinksLegWidth;
                            minNoShearLinksLegMinor = minNoShearLinksLegHeight;
                        }


                        noShearLinksLegMajor = minNoShearLinksLegMajor;
                        noShearLinksLegMinor = minNoShearLinksLegMinor;

                        //Shear links design
                        while ((noShearLinksLegMajor <= noRebarMajor && noShearLinksLegMinor <= noRebarMinor) && (!shearLinksMajorSatisfied || !shearLinksMinorSatisfied))
                        {
                            shearLinksSpacing = maxShearLinksSpacing;

                            while (shearLinksSpacing >= minShearLinksSpacing && (!shearLinksMajorSatisfied || !shearLinksMinorSatisfied))
                            {
                                double avMajorFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLegMajor * (1000 / shearLinksSpacing);
                                double avMinorFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLegMinor * (1000 / shearLinksSpacing);

                                if (avMajorFromCurrentShearLinks >= avReqMajor && avMinorFromCurrentShearLinks >= avReqMinor)
                                {
                                    shearLinksMajorSatisfied = true;
                                    shearLinksMinorSatisfied = true;
                                }

                                //Check
                                if (shearLinksMajorSatisfied && shearLinksMinorSatisfied)
                                {
                                    //do nothing
                                }
                                else if (shearLinksSpacing <= minShearLinksSpacing)
                                {
                                    break;
                                }
                                else
                                {
                                    //Reduce spacing
                                    shearLinksSpacing -= shearLinksSpacingStep;
                                }
                            }

                            //Check
                            if (shearLinksMajorSatisfied)
                            {
                                //do nothing
                            }
                            else if (noShearLinksLegMajor == noRebarMajor)
                            {
                                break;
                            }
                            else
                            {
                                //Increase no of leg
                                noShearLinksLegMajor += 1;
                            }

                            //Check
                            if (shearLinksMinorSatisfied)
                            {
                                //do nothing
                            }
                            else if (noShearLinksLegMinor == noRebarMinor)
                            {
                                break;
                            }
                            else
                            {
                                //Increase no of leg
                                noShearLinksLegMinor += 1;
                            }
                        }

                        //Check
                        if (shearLinksMajorSatisfied && shearLinksMinorSatisfied)
                        {
                            //do nothing
                        }
                        else if (shearLinksDiameterIndex == shearLinksDiameterList.Count() - 1)
                        {
                            break;
                        }
                        else
                        {
                            //Increase shear links diameter
                            shearLinksDiameterIndex += 1;
                        }

                        break;


                    case "Circle":

                        //Calculate minimum number of shear links leg based on 150mm max distance compression rebar from confined rebar
                        double minNoShearLinks = 2; //2 legs for circular, cannot add more
                        noShearLinksLegMajor = minNoShearLinks;
                        noShearLinksLegMinor = minNoShearLinks;

                        //Shear links design
                        while ((noShearLinksLegMajor <= 2 && noShearLinksLegMinor <= 2) && (!shearLinksMajorSatisfied || !shearLinksMajorSatisfied))
                        {
                            shearLinksSpacing = maxShearLinksSpacing;

                            while (shearLinksSpacing >= minShearLinksSpacing && (!shearLinksMajorSatisfied || !shearLinksMajorSatisfied))
                            {
                                double avMajorFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLegMajor * (1000 / shearLinksSpacing);
                                double avMinorFromCurrentShearLinks = 0.25 * Math.PI * Math.Pow(shearLinksDiameter, 2) * noShearLinksLegMinor * (1000 / shearLinksSpacing);

                                double avFromCurrentShearLinks = Math.Min(avMajorFromCurrentShearLinks, avMinorFromCurrentShearLinks);

                                if (avFromCurrentShearLinks >= avReqMajor && avFromCurrentShearLinks >= avReqMinor)
                                {
                                    shearLinksMajorSatisfied = true;
                                    shearLinksMinorSatisfied = true;
                                }

                                //Check
                                if (shearLinksMajorSatisfied && shearLinksMinorSatisfied)
                                {
                                    //do nothing
                                }
                                else if (shearLinksSpacing <= minShearLinksSpacing)
                                {
                                    break;
                                }
                                else
                                {
                                    //Reduce spacing
                                    shearLinksSpacing -= shearLinksSpacingStep;
                                }
                            }

                            //Check
                            if (shearLinksMajorSatisfied && shearLinksMinorSatisfied)
                            {
                                //do nothing
                            }
                            else if (noShearLinksLegMajor == 2 || noShearLinksLegMinor == 2)
                            {
                                break;
                            }
                            else
                            {
                                //Increase no of leg
                                noShearLinksLegMajor += 1;
                                noShearLinksLegMinor += 1;
                            }
                        }

                        //Check
                        if (shearLinksMajorSatisfied && shearLinksMinorSatisfied)
                        {
                            //do nothing
                        }
                        else if (shearLinksDiameterIndex == shearLinksDiameterList.Count() - 1)
                        {
                            break;
                        }
                        else
                        {
                            //Increase shear links diameter
                            shearLinksDiameterIndex += 1;
                        }

                        break;
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


            if (!shearLinksMajorSatisfied || !shearLinksMinorSatisfied)
            {
                noShearLinksLegMajor = 0;
                noShearLinksLegMinor = 0;
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

            return (noRebarLayer, noRebarPerLayer, rebarDiameterPerLayer, asEvaluationComment, noShearLinksLegMajor, noShearLinksLegMinor, shearLinksDiameter, shearLinksSpacing, avEvaluationComment, warningRebarDiameterTooBigShown);
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
                "UniqueName", "Shape", "Width (mm)", "Height (mm)", "Diameter (mm)", "fyk (Mpa)", "fck (Mpa)", "d (ETABS) (mm)", "d calculated (mm)", "As Min(mm2)"
                , "As (mm2)","Av Major (mm2/m)","Av Minor (mm2/m)"
                , "Rebar", "Shear Links Major", "Shear Links Minor"
                , "As Prov (mm2)", "As Prov (%)"
                , "Av Major Prov (mm2/m)"
                , "Av Minor Prov (mm2/m)"
                , "As Ratio"
                , "Av Major Ratio"
                , "Av Minor Ratio"
                , "ETABS warning", "Rebar warning", "Shear links warning"
                , "Cluster"
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
                result[i, 1] = rcColumn.Shape;

                result[i, 2] = rcColumn.Width;
                result[i, 3] = rcColumn.Height;
                result[i, 4] = rcColumn.Diameter;

                result[i, 5] = rcColumn.Fyk;
                result[i, 6] = rcColumn.Fck;
                result[i, 7] = rcColumn.d;
                result[i, 8] = rcColumn.dCalculated;
                result[i, 9] = rcColumn.AsMin;

                result[i, 10] = rcColumn.As;

                result[i, 11] = rcColumn.AvMajor;
                result[i, 12] = rcColumn.AvMinor;

                result[i, 13] = rcColumn.Rebar;
                result[i, 14] = rcColumn.ShearLinksMajor;
                result[i, 15] = rcColumn.ShearLinksMinor;

                result[i, 16] = rcColumn.AsProv;
                result[i, 17] = rcColumn.AsProvPercentage;
                result[i, 18] = rcColumn.AvMajorProv;
                result[i, 19] = rcColumn.AvMinorProv;
                result[i, 20] = rcColumn.RatioAs;
                result[i, 21] = rcColumn.RatioAvMajor;
                result[i, 22] = rcColumn.RatioAvMinor;

                result[i, 23] = rcColumn.EtabsComment;
                result[i, 24] = rcColumn.AsEvaluationComment;
                result[i, 25] = rcColumn.AvEvaluationComment;

                result[i, 26] = rcColumn.Cluster;

                i++;
            }

            return result;
        }

        public static void WriteResultRcColumnToExcel(object[,] result)
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
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            //char aplhabethOfColFromManual = alphabet[numOfCol - 1];

            //string lastCellRange = alphabet[numOfCol - 1] + Convert.ToString(numOfRow - 1 + 1);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

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

        public static double GetMinimumRcColumnRebarEC( double width, double height, double minimumRebarPercentage)
        {
            //Min As
            double asMin = minimumRebarPercentage * width * height;

            return asMin;
        }

        public static void CompareToMinimumRcColumnRebar(src.HelperEtabs.RcColumn rcColumn, double minimumRebarPercentage)
        {
            double asMin = 0;
            asMin = GetMinimumRcColumnRebarEC(rcColumn.Width, rcColumn.Height, minimumRebarPercentage);
            rcColumn.AsMin = Math.Ceiling(asMin);

            rcColumn.As = Math.Max(rcColumn.As, rcColumn.AsMin);
        }

        public static void CalculateDFromRebar(List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                double coverToRebarCentroid = 0;
                double centroidFromFirstRebarCentroid = 0;

                centroidFromFirstRebarCentroid = GetCentroidFromRebarDiameterList(rcColumn.RebarDiameterList, rcColumn.NoRebarPerLayer);
                coverToRebarCentroid = rcColumn.Cover + rcColumn.ShearLinksDiameter + 0.5 * rcColumn.RebarDiameterList[0] + centroidFromFirstRebarCentroid;

                //Save d calcualated
                switch (rcColumn.Shape)
                {
                    case ("Rectangular"):
                        rcColumn.dCalculated = Math.Floor(rcColumn.Height - coverToRebarCentroid);
                        break;
                    case ("Circle"):
                        rcColumn.dCalculated = Math.Floor(rcColumn.Diameter - coverToRebarCentroid);
                        break;
                }


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

        public static void CheckMessageRcColRebar(List<src.HelperEtabs.RcColumn> rcColumnList, src.HelperEtabs.DatabaseTable rcColumnDesignSummary)
        {
            //Get table
            List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcColumnDesignSummary.TableInList;

            bool popUpShown = false;

            //Getting message for each beam
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                bool messageTaken = false;
                double length = 0;

                rcColumn.EtabsComment = "";

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {

                    if (row["UniqueName"] == rcColumn.UniqueName)
                    {
                        if (row["WarnMsg"] != "No Message")
                        {
                            rcColumn.EtabsComment += row["WarnMsg"];
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
                            rcColumn.EtabsComment += row["ErrMsg"];
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

        public static void CalculateAsProv(List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                rcColumn.AsProv = CalculateAsProvFromRebarList(rcColumn.RebarDiameterList, rcColumn.NoRebarPerLayer);

                switch (rcColumn.Shape)
                {
                    case ("Rectangular"):
                        rcColumn.AsProvPercentage = Math.Round(rcColumn.AsProv / (rcColumn.Height * rcColumn.Width) * 100, 2); //in %
                        break;
                    case ("Circle"):
                        double concreteArea = 0.25 * Math.PI * Math.Pow(rcColumn.Diameter, 2);
                        rcColumn.AsProvPercentage = Math.Round(rcColumn.AsProv / concreteArea * 100, 2); //in %
                        break;
                }
                
                rcColumn.RatioAs = Math.Round(rcColumn.As / rcColumn.AsProv, 3);

                //Give warning if Asprov > 4%
                if (rcColumn.AsProvPercentage > 4)
                {
                    if (rcColumn.AsEvaluationComment == "")
                    {
                        rcColumn.AsEvaluationComment = "As provided > 4%";
                    }
                    else
                    {
                        rcColumn.AsEvaluationComment += ", As provided > 4%";
                    }
                }
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

        public static void CalculateAvProv(List<src.HelperEtabs.RcColumn> rcColumnList)
        {
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                //Major
                rcColumn.AvMajorProv = Math.Round(rcColumn.NumShearLinksLegMajor * 0.25 * Math.PI * Math.Pow(rcColumn.ShearLinksDiameter, 2) * (1000 / rcColumn.ShearLinksSpacing)); //mm2/m
                rcColumn.RatioAvMajor = Math.Round(rcColumn.AvMajor / rcColumn.AvMajorProv, 3);

                //Minor
                rcColumn.AvMinorProv = Math.Round(rcColumn.NumShearLinksLegMinor * 0.25 * Math.PI * Math.Pow(rcColumn.ShearLinksDiameter, 2) * (1000 / rcColumn.ShearLinksSpacing)); //mm2/m
                rcColumn.RatioAvMinor = Math.Round(rcColumn.AvMinor / rcColumn.AvMinorProv, 3);
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
                    sumAreaTimesCentroid += 0.25 * Math.PI * Math.Pow(rebarDiameter, 2) * eachLayerCentroid;
                    sumArea += 0.25 * Math.PI * Math.Pow(rebarDiameter, 2);

                    //Calculate centroid for next layer
                    if (index < rebarDiameterList.Count - 1)
                    {
                        eachLayerCentroid += 0.5 * rebarDiameter + 0.5 * rebarDiameterList[index + 1];
                    }

                    index++;
                }

                double centroid = sumAreaTimesCentroid / sumArea;
                return centroid;
            }

        }

        public static void RcColClusteringUsingKMeans(List<src.HelperEtabs.RcColumn> rcColumnList)
        {

            //Prepare data for ML
            double[][] observations = new double[rcColumnList.Count()][];

            int index = 0;
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                observations[index] = new double[]
                {
                    rcColumn.Width,
                    rcColumn.Height,
                    rcColumn.Diameter,
                    rcColumn.AsProv,
                    rcColumn.AvMajorProv,
                    rcColumn.AvMinorProv,
                };
                index++;
            }

            //Standardize data
            observations = HelperML.StandardizeData(observations);

            //Call up ML preference window
            src.ML.WindowMLPrefence windowMLPrefence = new WindowMLPrefence(observations);
            int[] labels = windowMLPrefence.GetLabels();

            //Parsing cluster back to rcBeamList
            index = 0;
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                rcColumn.Cluster = Convert.ToString(labels[index]);
                index++;
            }


        }

        public static void RcColClustering(List<src.HelperEtabs.RcColumn> rcColumnList)
        {

            //Prepare data for clustering
            double[][] observations = new double[rcColumnList.Count()][];

            int index = 0;
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                observations[index] = new double[]
                {
                    rcColumn.Width,
                    rcColumn.Height,
                    rcColumn.Diameter,
                    rcColumn.AsProv,
                    rcColumn.AvMajorProv,
                    rcColumn.AvMinorProv,
                };
                index++;
            }

            List<double[]> uniqueObservations = new List<double[]>();
            int indexUnique = 0;
            List<int> labels = new List<int>();
            bool firstLoop = true;
            foreach (double[] observation in observations)
            {
                if (firstLoop)
                {
                    uniqueObservations.Add(observation);
                    firstLoop = false;
                    labels.Add(indexUnique);
                    indexUnique++;
                }
                else
                {
                    bool isArrayExistInAlready = false;
                    int indexInUniqueObservations = 0;
                    (isArrayExistInAlready, indexInUniqueObservations) = IsArrayExistedInAlready(observation, uniqueObservations);

                    if (!isArrayExistInAlready)
                    {
                        uniqueObservations.Add(observation);
                        labels.Add(indexUnique);
                        indexUnique++;
                    }
                    else
                    {
                        labels.Add(indexInUniqueObservations);
                    }
                }
                
            }

            //Parsing cluster back to rcBeamList
            index = 0;
            foreach (src.HelperEtabs.RcColumn rcColumn in rcColumnList)
            {
                rcColumn.Cluster = Convert.ToString(labels[index]);
                index++;
            }



        }

        public static (bool, int) IsArrayExistedInAlready(double[] observationsA, List<double[]> uniqueObservations)
        {
            int indexUnique = 0;
            foreach (double[] uniqueObservation in uniqueObservations)
            {
                //Check if size same
                if (observationsA.Count() == uniqueObservation.Count())
                {
                    bool eachComponentMatch = true;
                    for (int i = 0; i < observationsA.Count(); i++)
                    {
                        if (observationsA[i] != uniqueObservation[i])
                        {
                            eachComponentMatch = false;
                            break;
                        }
                    }
                    if (eachComponentMatch)
                    {
                        return (true, indexUnique);
                    }
                }
                indexUnique++;
            }
            return (false, -1);
        }
    }
}
