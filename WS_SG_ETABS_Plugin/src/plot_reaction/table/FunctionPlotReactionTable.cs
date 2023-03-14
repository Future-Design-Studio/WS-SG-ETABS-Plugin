using ETABSv1;
using LiveCharts.Wpf.Charts.Base;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.plot_reaction.table
{
    internal class FunctionPlotReactionTable
    {
        #region Class
        public class jointReaction
        {
            public string UniqueName { get; set; }
            public double XCoordinate { get; set; }
            public double YCoordinate { get; set; }
            public double ZCoordinate { get; set; }
            public List<ReactionForce> ReactionForceList { get; set; }
            public int Label { get; set; }

            public jointReaction(string uniqueName, double x, double y, double z, int label, List<ReactionForce> reactionForceList)
            {
                UniqueName = uniqueName;
                XCoordinate = x;
                YCoordinate = y;
                ZCoordinate = z;
                Label = label;
                ReactionForceList = reactionForceList;
            }
        }

        public class ReactionForce
        {
            public string LoadCase { get; set; }
            public double FX { get; set; }
            public double FY { get; set; }
            public double FZ { get; set; }
            public double MX { get; set; }
            public double MY { get; set; }
            public double MZ { get; set; }

            public ReactionForce(string loadCase, double fX, double fY, double fZ, double mX, double mY, double mZ)
            {
                LoadCase = loadCase;
                FX = fX;
                FY = fY;
                FZ = fZ;

                MX = mX;
                MY = mY;
                MZ = mZ;
            }
        }

        #endregion


        public static bool CreateExcelAndPlotPosition (cSapModel Sap, cPluginCallback Plugin, WpfPlot ReactionPlot, List<Dictionary<string, string>> groupAndColor, List<string> listBoxChoosenLoadCase, string plotTitle,
            double bubbleScaleFactor, int textSize, int titleSize, int axisTitleSize, int gridSpacingX, int gridSpacingY, bool? showRenumber, List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath)
        {
            int Ret;

            bool isError = false;
            bool showWarning = true;
            List<Dictionary<string, object>> compiledReactionData = new List<Dictionary<string, object>>();

            //casting bool? to bool
            bool showRenumberCasting = showRenumber.HasValue && showRenumber.Value;

            //Get required table
            //List<Dictionary<string, string>> pointConnectivityTable = new List<Dictionary<string, string>>();
            //(isError, pointConnectivityTable) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, "Point Object Connectivity", listChoosenGroup, showWarning);

            //Get the table
            src.HelperEtabs.DatabaseTable jointReactionTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Reactions");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable groupAssignmentTable = new HelperEtabs.DatabaseTable();
            (isError, groupAssignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable baseReactionsTable = new HelperEtabs.DatabaseTable();
            (isError, baseReactionsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Base Reactions");
            if (isError) { return isError; }

            //To save all the reaction
            List<jointReaction> jointReactionList = new List<jointReaction>();

            int groupIndex = 0;
            foreach (Dictionary<string, string> group in groupAndColor)
            {
                string groupName = group["groupName"];
                string color = group["color"];

                //Bubble color is either decided by user or auto (between 0 and 1 based on how many groups there are)
                System.Drawing.Color bubbleColor = new System.Drawing.Color();
                if (color == "Auto")
                {

                    if (groupAndColor.Count > 1)
                    {
                        double colorMapFraction = Convert.ToDouble(groupIndex) / (Convert.ToDouble(groupAndColor.Count) - 1);
                        bubbleColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .5);
                    }
                    else
                    {
                        double colorMapFraction = 0;
                        bubbleColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .5);
                    }
                }
                else
                {
                    string colorHex = color;
                    bubbleColor = System.Drawing.ColorTranslator.FromHtml(colorHex);
                }

                
                //Filter joint reactions based on group
                src.HelperEtabs.DatabaseTable jointReactionTableFiltered = new HelperEtabs.DatabaseTable();
                (isError, jointReactionTableFiltered) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(jointReactionTable, groupAssignmentTable, groupName);
                if (isError) { return isError; }

                //To save unique joint that has been plotted
                List<string> uniqueJointRecorded = new List<string>();

                

                foreach (Dictionary<string, string> jointReactionRow in jointReactionTableFiltered.TableInList)
                {
                    foreach(string loadCase in listBoxChoosenLoadCase)
                    {
                        //If have been recorded before
                        if (jointReactionRow["OutputCase"] == loadCase && uniqueJointRecorded.Contains(jointReactionRow["UniqueName"]))
                        {
                            //Go through jointReactionList and find the joint
                            foreach (jointReaction jointReaction in jointReactionList)
                            {
                                if (jointReactionRow["UniqueName"] == jointReaction.UniqueName)
                                {
                                    double fX = Convert.ToDouble(jointReactionRow["FX"]);
                                    double fY = Convert.ToDouble(jointReactionRow["FY"]);
                                    double fZ = Convert.ToDouble(jointReactionRow["FZ"]);
                                    double mX = Convert.ToDouble(jointReactionRow["MX"]);
                                    double mY = Convert.ToDouble(jointReactionRow["MY"]);
                                    double mZ = Convert.ToDouble(jointReactionRow["MZ"]);

                                    ReactionForce eachReactionForce = new ReactionForce(loadCase, fX, fY, fZ, mX, mY, mZ);

                                    jointReaction.ReactionForceList.Add(eachReactionForce);
                                }                                
                            }
                        }

                        //If have not been recorded before
                        else if (jointReactionRow["OutputCase"] == loadCase && !uniqueJointRecorded.Contains(jointReactionRow["UniqueName"]))
                        {
                            string uniqueName = jointReactionRow["UniqueName"];

                            uniqueJointRecorded.Add(uniqueName);

                            int label = uniqueJointRecorded.IndexOf(jointReactionRow["UniqueName"]); //index inside the list
                            double xCoord = new double();
                            double yCoord = new double();
                            double zCoord = new double();

                            //Find coordinate of the joint
                            Ret = Sap.PointObj.GetCoordCartesian(jointReactionRow["UniqueName"], ref xCoord, ref yCoord, ref zCoord);

                            //Create new instance of jointReaction
                            double fX = Convert.ToDouble(jointReactionRow["FX"]);
                            double fY = Convert.ToDouble(jointReactionRow["FY"]);
                            double fZ = Convert.ToDouble(jointReactionRow["FZ"]);
                            double mX = Convert.ToDouble(jointReactionRow["MX"]);
                            double mY = Convert.ToDouble(jointReactionRow["MY"]);
                            double mZ = Convert.ToDouble(jointReactionRow["MZ"]);

                            List<ReactionForce> reactionForceList = new List<ReactionForce>();
                            ReactionForce eachReactionForce = new ReactionForce(loadCase, fX, fY, fZ, mX, mY, mZ);
                            reactionForceList.Add(eachReactionForce);

                            jointReaction jointReaction = new jointReaction(uniqueName, xCoord, yCoord, zCoord, label, reactionForceList);
                            jointReactionList.Add(jointReaction);

                            //Plotting
                            //Bubble size and text
                            //double reaction = Convert.ToDouble(jointReactionRow[reactionDirection]);
                            double reactionPlot = 1;
                            double bubbleSize = Math.Abs(reactionPlot) * bubbleScaleFactor;

                            string bubbleText = "";
                            if (showRenumberCasting)
                            {
                                //Use the index in uniqueJoint
                                bubbleText = Convert.ToString(label);
                            }
                            else
                            {
                                //Use the unique name
                                bubbleText = Convert.ToString(uniqueName);
                            }

                            src.HelperPlot.BubblePlotter(ReactionPlot, bubbleSize, bubbleText, textSize, bubbleColor, xCoord, yCoord);
                        }
                    }
                }  
                groupIndex++;
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


            //Create excel
            var resultFx = CompileReactionToArrayFx(jointReactionList);
            var resultFy = CompileReactionToArrayFy(jointReactionList);
            var resultFz = CompileReactionToArrayFz(jointReactionList);


            WriteReactionToExcel(resultFx, resultFy, resultFz, fileName, filePath);

            return isError;
        }

        private static object[,] CompileReactionToArrayFx(List<jointReaction> jointReactionList)
        {
            List<string> colTitles = new List<string>()
            {
                "Label","UniqueName","X (m)","Y (m)","Z (m)"
            };

            //Take the first jointReaction and create the column
            jointReaction jointReaction = jointReactionList[0];
            foreach (ReactionForce reactionForce in jointReaction.ReactionForceList)
            {
                colTitles.Add($"{reactionForce.LoadCase} (kN)");
            }

            int numOfRow = jointReactionList.Count();
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
            foreach (jointReaction eachJointReaction in jointReactionList)
            {
                result[i, 0] = eachJointReaction.Label;
                result[i, 1] = eachJointReaction.UniqueName;
                result[i, 2] = eachJointReaction.XCoordinate;
                result[i, 3] = eachJointReaction.YCoordinate;
                result[i, 4] = eachJointReaction.ZCoordinate;

                int loadCaseIndex = 5;
                foreach (ReactionForce reactionForce in eachJointReaction.ReactionForceList)
                {
                    result[i, loadCaseIndex] = reactionForce.FX;
                    loadCaseIndex++;
                }

                i++;
            }

            return result;
        }

        private static object[,] CompileReactionToArrayFy(List<jointReaction> jointReactionList)
        {
            List<string> colTitles = new List<string>()
            {
                "Label","UniqueName","X (m)","Y (m)","Z (m)"
            };

            //Take the first jointReaction and create the column
            jointReaction jointReaction = jointReactionList[0];
            foreach (ReactionForce reactionForce in jointReaction.ReactionForceList)
            {
                colTitles.Add($"{reactionForce.LoadCase} (kN)");
            }

            int numOfRow = jointReactionList.Count();
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
            foreach (jointReaction eachJointReaction in jointReactionList)
            {
                result[i, 0] = eachJointReaction.Label;
                result[i, 1] = eachJointReaction.UniqueName;
                result[i, 2] = eachJointReaction.XCoordinate;
                result[i, 3] = eachJointReaction.YCoordinate;
                result[i, 4] = eachJointReaction.ZCoordinate;

                int loadCaseIndex = 5;
                foreach (ReactionForce reactionForce in eachJointReaction.ReactionForceList)
                {
                    result[i, loadCaseIndex] = reactionForce.FY;
                    loadCaseIndex++;
                }

                i++;
            }

            return result;
        }

        private static object[,] CompileReactionToArrayFz(List<jointReaction> jointReactionList)
        {
            List<string> colTitles = new List<string>()
            {
                "Label","UniqueName","X (m)","Y (m)","Z (m)"
            };

            //Take the first jointReaction and create the column
            jointReaction jointReaction = jointReactionList[0];
            foreach (ReactionForce reactionForce in jointReaction.ReactionForceList)
            {
                colTitles.Add($"{reactionForce.LoadCase} (kN)");
            }

            int numOfRow = jointReactionList.Count();
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
            foreach (jointReaction eachJointReaction in jointReactionList)
            {
                result[i, 0] = eachJointReaction.Label;
                result[i, 1] = eachJointReaction.UniqueName;
                result[i, 2] = eachJointReaction.XCoordinate;
                result[i, 3] = eachJointReaction.YCoordinate;
                result[i, 4] = eachJointReaction.ZCoordinate;

                int loadCaseIndex = 5;
                foreach (ReactionForce reactionForce in eachJointReaction.ReactionForceList)
                {
                    result[i, loadCaseIndex] = reactionForce.FZ;
                    loadCaseIndex++;
                }

                i++;
            }

            return result;
        }

        public static void WriteReactionToExcel(object[,] resultFx, object[,] resultFy, object[,] resultFz, string fileName, string filePath)
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
            xlWorkBook.Sheets.Add();
            xlWorkBook.Sheets.Add();

            //Fx
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "FX";

            int numOfRow = resultFx.GetLength(0);
            int numOfCol = resultFx.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = resultFx;

            
            //Fy
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            xlWorkSheet.Name = "FY";

            numOfRow = resultFy.GetLength(0);
            numOfCol = resultFy.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = resultFy;

            
            //Fz
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            xlWorkSheet.Name = "FZ";

            numOfRow = resultFz.GetLength(0);
            numOfCol = resultFz.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = resultFz;
            

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
