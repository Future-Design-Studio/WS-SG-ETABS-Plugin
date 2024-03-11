using ETABSv1;
using Microsoft.Office.Interop.Excel;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;
using WS_SG_ETABS_Plugin.src.punching_shear_plot;
using static WS_SG_ETABS_Plugin.src.pad_footing_plot.ClassPadFootPlot;
using static WS_SG_ETABS_Plugin.src.punching_shear_plot.ClassPunchingShrPlot;

namespace WS_SG_ETABS_Plugin.src.pad_footing_plot
{
    internal class FunctionPadFootPlot
    {
        public static (bool, List<pad_footing_plot.ClassPadFootPlot.GroupDefinition>) PadFootingPlot(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, double sizeIncrement, string plotTitle, double thickness, double density,
            double soilCapacity, double maxUtilizationRatio, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, WpfPlot ReactionPlot, string fileName, string filePath)
        {
            bool isError = false;
            List<string> errorList= new List<string>();
            List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitions = new List<ClassPadFootPlot.GroupDefinition>();

            //Get the table
            src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
            (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
            if (isError) { return (isError, groupDefinitions); }

            src.HelperEtabs.DatabaseTable jointReactionTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Reactions");
            if (isError) { return (isError, groupDefinitions); }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return (isError, groupDefinitions); }

            //Filter group to only include points, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Point");
            if (isError) { return (isError, groupDefinitions); }

            //Container
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();
            List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList = new List<ClassPadFootPlot.JointPadFootDetails>();

            //Get joint which is part of the choosen group
            jointList = HelperEtabs.GetJointListFromGroup(pointObjectConnectivityTable, groupAssignmentsTable, choosenGroup);
            GetJointReaction(jointPadFootDetailsList, jointList, jointReactionTable, listChoosenLoadCase, errorList);
            double soilCapacityReduced = CalculatePadFootingSize(jointPadFootDetailsList, thickness, density, soilCapacity, errorList, sizeIncrement, maxUtilizationRatio);
            groupDefinitions = GroupByArea(jointPadFootDetailsList, sizeIncrement, soilCapacityReduced);
            //PlotPadFooting(groupDefinitions, plotTitle, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, ReactionPlot);

            //Create excel
            var result = CompilePadFootDetailsToArray(jointPadFootDetailsList, choosenGroup);
            var resultGroupDef = CompileGroupDefinitionsToArray(groupDefinitions);
            WritePadFootDetailsToExcel(result, resultGroupDef, fileName, filePath);

            return (isError, groupDefinitions);
        }

        public static void GetJointReaction(List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList, List<src.HelperEtabs.Point3D> jointList, src.HelperEtabs.DatabaseTable jointReactionTable,
            List<string> listChoosenLoadCase, List<string> errorList)
        {
            bool isError = false;

            //Go through each joint
            foreach (src.HelperEtabs.Point3D joint in jointList)
            {
                ClassPadFootPlot.JointPadFootDetails jointPadFootDetails = new ClassPadFootPlot.JointPadFootDetails(joint.UniqueName, joint.XCoordinate, joint.YCoordinate, joint.ZCoordinate);

                bool addedToErrorNoReaction = false;
                bool addedToErrorTension = false;
                foreach (string loadCase in listChoosenLoadCase)
                {
                    HelperEtabs.DatabaseTable filteredJointReactionTable = new HelperEtabs.DatabaseTable();
                    (isError, filteredJointReactionTable) = HelperEtabs.FilterDatabaseTable(jointReactionTable, "UniqueName", joint.UniqueName);

                    //Second filter only when table is not empty
                    if (filteredJointReactionTable.TableInList.Count != 0)
                    {
                        (isError, filteredJointReactionTable) = HelperEtabs.FilterDatabaseTable(filteredJointReactionTable, "OutputCase", loadCase);
                    }

                    List<double> jointReactionInOneLoadCase = new List<double>();

                    //Check if there are reactions
                    if (filteredJointReactionTable.TableInList.Count == 0)
                    {
                        if (!addedToErrorNoReaction)
                        {
                            string errorMessage = $"Joint {joint.UniqueName} has no reaction from load case {loadCase}!";
                            MessageBox.Show(errorMessage);
                            errorList.Add(errorMessage);
                            addedToErrorNoReaction = true;
                            
                        }
                        jointPadFootDetails.IsNotValid = true;
                        jointPadFootDetails.ForceList.Add(loadCase, 0);
                    }
                    else
                    {
                        //List reaction, get max reaction for 1 load case and check if there is any tension
                        double maxReactionInOneLoadCase = 0;
                        bool firstLoop = true;
                        foreach (Dictionary<string, string> row in filteredJointReactionTable.TableInList)
                        {
                            double jointReaction = Convert.ToDouble(row["FZ"]);

                            if (firstLoop)
                            {
                                maxReactionInOneLoadCase = jointReaction;
                                firstLoop = false;
                            }
                            else
                            {
                                if (maxReactionInOneLoadCase < jointReaction)
                                {
                                    maxReactionInOneLoadCase = jointReaction;
                                }
                            }

                            //Check if any tension
                            if (jointReaction < 0 && !addedToErrorTension)
                            {
                                string errorMessage = $"Joint {joint.UniqueName} has tension reaction from load case {loadCase}!";
                                MessageBox.Show(errorMessage);
                                errorList.Add(errorMessage);
                                addedToErrorTension = true;

                                jointPadFootDetails.IsTension = true;
                            }

                            jointReactionInOneLoadCase.Add(jointReaction);
                        }

                        jointPadFootDetails.ForceList.Add(loadCase, maxReactionInOneLoadCase);
                    }
                }

                //Get max reaction for one joint
                double maxReactionInOneJoint = 0;
                string maxLoadCaseInOneJoint = "";
                bool firstLoopInOneJoint = true;
                foreach (KeyValuePair<string, double> force in jointPadFootDetails.ForceList)
                {
                    if (firstLoopInOneJoint)
                    {
                        maxLoadCaseInOneJoint = force.Key;
                        maxReactionInOneJoint = force.Value;
                        firstLoopInOneJoint = false;
                    }
                    else
                    {
                        if (maxReactionInOneJoint < force.Value)
                        {
                            maxLoadCaseInOneJoint = force.Key;
                            maxReactionInOneJoint = force.Value;
                        }
                    }
                }

                jointPadFootDetails.ForceMax = Math.Ceiling(maxReactionInOneJoint);
                jointPadFootDetails.LoadCaseMax = maxLoadCaseInOneJoint;

                jointPadFootDetailsList.Add(jointPadFootDetails);
            }
        }

        public static object[,] CompilePadFootDetailsToArray(List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetails, string choosenGroup)
        {
            List<string> colTitles = new List<string>()
            {
                "Num","Joint Unique Name", "X (m)","Y (m)","Z (m)", "Group Name", "Reaction Force (kN)", "Governing Load Case", "Width (m)", "Area (m2)", "Utilization ratio", "Group", "Invalid or in tension"
            };

            int numOfRow = jointPadFootDetails.Count();
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
            foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetail in jointPadFootDetails)
            {
                result[i, 0] = i;
                result[i, 1] = jointPadFootDetail.UniqueName;
                result[i, 2] = jointPadFootDetail.XCoordinate;
                result[i, 3] = jointPadFootDetail.YCoordinate;
                result[i, 4] = jointPadFootDetail.ZCoordinate;
                result[i, 5] = choosenGroup;

                result[i, 6] = jointPadFootDetail.ForceMax;
                result[i, 7] = jointPadFootDetail.LoadCaseMax;

                result[i, 8] = jointPadFootDetail.Width;
                result[i, 9] = jointPadFootDetail.Area;
                result[i, 10] = jointPadFootDetail.UtilizationRatio;

                result[i, 11] = jointPadFootDetail.GroupIndex;

                if (jointPadFootDetail.IsNotValid)
                {
                    result[i, 12] = "Invalid!";
                }
                else if (jointPadFootDetail.IsTension)
                {
                    result[i, 12] = "Tension!";
                }
                else
                {
                    result[i, 12] = "";
                }

                i++;
            }

            return result;


        }

        public static object[,] CompileGroupDefinitionsToArray(List<ClassPadFootPlot.GroupDefinition> groupDefinitions)
        {
            List<string> colTitles = new List<string>()
            {
                "Group Index","Width", "Area"
            };

            int numOfRow = groupDefinitions.Count();
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
            foreach (ClassPadFootPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                result[i, 0] = groupDefinition.GroupIndex;
                result[i, 1] = groupDefinition.Width;
                result[i, 2] = groupDefinition.Area;

                i++;
            }

            return result;


        }

        


        public static void WritePadFootDetailsToExcel(object[,] result, object[,] resultGroupDef, string fileName, string filePath)
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

            //Create workbook
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //Add worksheets
            xlWorkBook.Sheets.Add(); //Group def

            //Worksheet
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "Pad footing result";

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

            //Worksheet
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            xlWorkSheet.Name = "Group definitions";

            numOfRow = resultGroupDef.GetLength(0);
            numOfCol = resultGroupDef.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = resultGroupDef;

            //Here saving the file in xlsx
            string excelPath = filePath + fileName + ".xlsx";

            /*
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

            xlApp.DisplayAlerts = false;
            xlWorkBook.SaveCopyAs(excelPath);
            xlWorkBook.Close();
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }

        public static double CalculatePadFootingSize(List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList, double thickness, double density, double soilCapacity, 
            List<string> errorList, double sizeIncrement, double maxUtilizationRatio)
        {
            //Reducing soil capacity due to pad footing SW
            double soilCapacityReduced = (soilCapacity - thickness / 1000 * density) * maxUtilizationRatio;

            foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in jointPadFootDetailsList)
            {
                double force = jointPadFootDetails.ForceMax;

                if (jointPadFootDetails.IsTension || jointPadFootDetails.IsNotValid)
                {
                    jointPadFootDetails.WidthDividedByIncrement = 0;
                }
                else
                {
                    double requiredArea = force / soilCapacityReduced;
                    jointPadFootDetails.RequiredArea = requiredArea;

                    double requiredWidth = Math.Pow(requiredArea, 0.5) * 1000; //in mm
                    jointPadFootDetails.WidthDividedByIncrement = Math.Ceiling(requiredWidth / sizeIncrement);
                }

            }
            double soilCapacityReducedWithoutFactor = soilCapacityReduced / maxUtilizationRatio;
            return soilCapacityReducedWithoutFactor;
        }

        public static List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> GroupByArea
            (List<ClassPadFootPlot.JointPadFootDetails> jointPadFootDetailsList, double sizeIncrement, double soilCapacityReduced)
        {

            //Sort pad footing details by force
            List<ClassPadFootPlot.JointPadFootDetails> sortedJointPadFootDetailsList = jointPadFootDetailsList.OrderBy(o => o.WidthDividedByIncrement).ToList();

            List<double> uniqueWidths = new List<double>();

            //Get unique width
            foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in sortedJointPadFootDetailsList)
            {
                if (!uniqueWidths.Contains(jointPadFootDetails.WidthDividedByIncrement))
                {
                    uniqueWidths.Add(jointPadFootDetails.WidthDividedByIncrement);
                }
            }

            //Group by unique width
            List<List<ClassPadFootPlot.JointPadFootDetails>> groupByWidth = new List<List<ClassPadFootPlot.JointPadFootDetails>>();
            foreach(double uniqueWidth in uniqueWidths)
            {
                List<ClassPadFootPlot.JointPadFootDetails> eachGroupByWidth = new List<ClassPadFootPlot.JointPadFootDetails>();

                foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in sortedJointPadFootDetailsList)
                {
                    if (jointPadFootDetails.WidthDividedByIncrement == uniqueWidth)
                    {
                        eachGroupByWidth.Add(jointPadFootDetails);
                    }
                }

                groupByWidth.Add(eachGroupByWidth);
            }

            //Assign group index
            int groupIndex = 1;
            List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitionList = new List<ClassPadFootPlot.GroupDefinition> ();
            foreach(List<ClassPadFootPlot.JointPadFootDetails> eachGroupByWidth in groupByWidth)
            {
                foreach(ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in eachGroupByWidth)
                {
                    jointPadFootDetails.GroupIndex = groupIndex;

                    //Get actual width
                    jointPadFootDetails.Width = jointPadFootDetails.WidthDividedByIncrement * sizeIncrement / 1000; //in m
                    jointPadFootDetails.Area = Math.Pow(jointPadFootDetails.Width, 2.0); // in m2

                    //Get utilization ratio
                    if (!jointPadFootDetails.IsNotValid && !jointPadFootDetails.IsTension)
                    {
                        jointPadFootDetails.UtilizationRatio = Math.Round(jointPadFootDetails.ForceMax / (jointPadFootDetails.Area * soilCapacityReduced), 3);
                    }
                    else
                    {
                        jointPadFootDetails.UtilizationRatio = 1;
                    }
                    
                }

                //Add to list of group definition
                pad_footing_plot.ClassPadFootPlot.GroupDefinition groupDefinition = new pad_footing_plot.ClassPadFootPlot.GroupDefinition(
                    groupIndex, eachGroupByWidth[0].Width, eachGroupByWidth);

                groupDefinitionList.Add(groupDefinition);

                groupIndex++;

            }
            return groupDefinitionList;
        }

        public static void PlotPadFooting(List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitions, string plotTitle, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY, WpfPlot ReactionPlot)
        {
            foreach (ClassPadFootPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                //Decide group color plot
                System.Drawing.Color color = new System.Drawing.Color();
                if (groupDefinitions.Count > 1)
                {
                    //groupDefinition.GroupIndex starts at 1 instead of 0, thus need to -1

                    double colorMapFraction = (groupDefinition.GroupIndex - 1) / (Convert.ToDouble(groupDefinitions.Count) - 1);
                    color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .6);
                    groupDefinition.Color = Convert.ToString(color);
                }
                else
                {
                    double colorMapFraction = 0;
                    color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .6);
                    groupDefinition.Color = Convert.ToString(color);
                }

                //Plot each column
                foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in groupDefinition.JointPadFootDetailsList)
                {
                    //Bubble size and text
                    //double reaction = Convert.ToDouble(jointReactionRow[reactionDirection]);
                    double xCenter = jointPadFootDetails.XCoordinate;
                    double yCenter = jointPadFootDetails.YCoordinate;
                    double width = groupDefinition.Width;

                    string text = Convert.ToString(
                                                        $"{jointPadFootDetails.UniqueName}"
                                                        + System.Environment.NewLine
                                                        + $"Width: {jointPadFootDetails.Width} m"
                                                        + System.Environment.NewLine
                                                        + $"Force: {jointPadFootDetails.ForceMax} kN"
                                                        + System.Environment.NewLine
                                                        + $"Case: {jointPadFootDetails.LoadCaseMax}"
                                                        + System.Environment.NewLine
                                                        + $"Group: {jointPadFootDetails.GroupIndex}"
                                                        );


                    src.HelperPlot.RectanglePlotter(ReactionPlot, xCenter, yCenter, text, textSize, color, width, width, 0, System.Drawing.ColorTranslator.FromHtml("black"));
                }

            }
            ReactionPlot.Plot.AxisScaleLock(true);
            ReactionPlot.Plot.Title(plotTitle, true, null, titleSize, "Arial Narrow");
            ReactionPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            //Axis
            ReactionPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ReactionPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ReactionPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            ReactionPlot.Plot.XAxis.ManualTickSpacing(gridSpacingX);

            ReactionPlot.Refresh();
        }

        public static void FilterPadFootingPlot(List<pad_footing_plot.ClassPadFootPlot.GroupDefinition> groupDefinitions, string plotTitle, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY, 
            WpfPlot ReactionPlot, List<string> selectedGroup, ScottPlot.AxisLimits axisLimits)
        {
            ReactionPlot.Reset();

            foreach (ClassPadFootPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                //Decide group color plot
                System.Drawing.Color color = new System.Drawing.Color();

                if (selectedGroup.Contains(Convert.ToString(groupDefinition.GroupIndex)))
                {
                    if (groupDefinitions.Count > 1)
                    {
                        double colorMapFraction = groupDefinition.GroupIndex / (Convert.ToDouble(groupDefinitions.Count) - 1);
                        color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .6);
                        groupDefinition.Color = Convert.ToString(color);
                    }
                    else
                    {
                        double colorMapFraction = 0;
                        color = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .6);
                        groupDefinition.Color = Convert.ToString(color);
                    }
                }
                else
                {
                    color = System.Drawing.Color.FromArgb(100, 200, 200, 200); ;
                }


                //Plot each column
                string text = "";
                foreach (ClassPadFootPlot.JointPadFootDetails jointPadFootDetails in groupDefinition.JointPadFootDetailsList)
                {
                    //Size and text
                    double xCenter = jointPadFootDetails.XCoordinate;
                    double yCenter = jointPadFootDetails.YCoordinate;
                    double width = groupDefinition.Width;

                    if (selectedGroup.Contains(Convert.ToString(jointPadFootDetails.GroupIndex)))
                    {
                        text = Convert.ToString($"{jointPadFootDetails.UniqueName}"
                                                    + System.Environment.NewLine
                                                    + $"Width: {jointPadFootDetails.Width} m"
                                                    + System.Environment.NewLine
                                                    + $"Force: {jointPadFootDetails.ForceMax} kN"
                                                    + System.Environment.NewLine
                                                    + $"Case: {jointPadFootDetails.LoadCaseMax}"
                                                    + System.Environment.NewLine
                                                    + $"Group: {jointPadFootDetails.GroupIndex}"
                                                    );
                    }
                    else
                    {
                        text = "";
                    }


                    src.HelperPlot.RectanglePlotter(ReactionPlot, xCenter, yCenter, text, textSize, color, width, width, 0, System.Drawing.ColorTranslator.FromHtml("black"));
                }


            }
            ReactionPlot.Plot.AxisScaleLock(true);
            ReactionPlot.Plot.Title(plotTitle, true, null, titleSize, "Arial Narrow");
            //ReactionPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            ReactionPlot.Plot.SetAxisLimits(axisLimits);

            //Axis
            ReactionPlot.Plot.YAxis.Label("Y coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ReactionPlot.Plot.XAxis.Label("X coordinate (m)", null, axisTitleSize, true, "Arial Narrow");
            ReactionPlot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            ReactionPlot.Plot.XAxis.ManualTickSpacing(gridSpacingX);

            ReactionPlot.Refresh();
        }
    }
}

