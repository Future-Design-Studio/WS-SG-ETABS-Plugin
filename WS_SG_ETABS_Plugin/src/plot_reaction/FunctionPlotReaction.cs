using ETABSv1;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;

namespace WS_SG_ETABS_Plugin.src.plot_reaction
{
    internal class FunctionPlotReaction
    {
        private static int Ret;

        #region Data source

        public static List<string> reactionDirectionList = new List<string>()
        {
            "FX", "FY", "FZ", "MX", "MY", "MZ"
        };

        #endregion

        public static bool PlotReaction(cSapModel Sap, List<string> listChoosenGroup, string outputCase, string reactionDirection, 
            WpfPlot ReactionPlot, double forceFactor, double bubbleScaleFactor, int textSize, string title, bool splitX, bool splitY,
            int divisionX, int divisionY, double offsetX, double offsetY)
        {
            bool isError = false;
            bool showWarning = true;
            List<Dictionary<string, object>> compiledReactionData = new List<Dictionary<string, object>>();

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(Sap);

            //Get required table
            List<Dictionary<string, string>> pointConnectivityTable = new List<Dictionary<string, string>>();            
            (isError, pointConnectivityTable) = src.HelperEtabs.GetTableDisplay(Sap, "Point Object Connectivity", showWarning);

            //Casting List<string> content to List<dictionary>
            List<object> listChoosenGroupParsed = new List<object>();
            foreach (string group in listChoosenGroup)
            {
                //Split the string using comma ','
                string[] splitted = group.Split(',');

                Dictionary<string, string> groupDict = new Dictionary<string, string>()
                    {
                        {"groupName", splitted[0]},
                        {"color", splitted[1]},
                    };

                listChoosenGroupParsed.Add(groupDict);
            }

            int groupIndex = 0;
            foreach (Dictionary<string, string> group in listChoosenGroupParsed)
            {
                string groupName = group["groupName"];
                string color = group["color"];

                //Bubble color is either decided by user or auto (between 0 and 1 based on how many groups there are)
                System.Drawing.Color bubbleColor = new System.Drawing.Color();
                if (color == "Auto")
                {

                    if (listChoosenGroup.Count > 1)
                    {
                        double colorMapFraction = Convert.ToDouble(groupIndex) / (Convert.ToDouble(listChoosenGroup.Count) - 1);
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


                //Get joint reaction for 1 specific group
                List<Dictionary<string, string>> jointReactionTable = new List<Dictionary<string, string>>();

                List<string> groupList = new List<string>();
                groupList.Add(groupName);

                (isError, jointReactionTable)  = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, "Joint Reactions", groupList, showWarning);

                if (isError)
                {
                    return isError;
                }


                foreach(Dictionary<string, string>  jointReactionRow in jointReactionTable)
                {
                    if (jointReactionRow["OutputCase"] == outputCase)
                    {
                        double xCoord = new double();
                        double yCoord = new double();

                        //Find coordinate of the joint
                        foreach (Dictionary<string, string> pointRow in pointConnectivityTable)
                        {
                            if(pointRow["UniqueName"] == jointReactionRow["UniqueName"])
                            {
                                xCoord = Convert.ToDouble(pointRow["X"]);
                                yCoord = Convert.ToDouble(pointRow["Y"]);
                                break;
                            }
                        }

                        //Bubble size and text
                        double reaction = Convert.ToDouble(jointReactionRow[reactionDirection]);
                        double bubbleSize = Math.Abs(reaction) * bubbleScaleFactor * forceFactor;
                        string bubbleText = Convert.ToString(Convert.ToInt32(reaction * forceFactor));

                        //Compile the data inside dictionary for sum
                        Dictionary<string, object> reactionData = new Dictionary<string, object>()
                        {
                            {"uniqueName", jointReactionRow["UniqueName"]},
                            {"groupName", groupName},
                            {"reaction",  reaction * forceFactor},
                            {"xCoord", xCoord},
                            {"yCoord", yCoord}
                        };
                        compiledReactionData.Add(reactionData);

                        src.HelperPlot.BubblePlotter(ReactionPlot, bubbleSize, bubbleText, textSize, bubbleColor, xCoord, yCoord);

                    }

                }

                groupIndex++;
            }

            //Summing up reaction
            if (splitX || splitY)
            {

                (double minXCoord, double maxXCoord) = src.HelperMethod.GetMinMaxDictionary(compiledReactionData, "xCoord");
                (double minYCoord, double maxYCoord) = src.HelperMethod.GetMinMaxDictionary(compiledReactionData, "yCoord");

                if (splitX)
                {
                    //For y only split into 2. Start from minYCoord
                    double lowY = minYCoord;
                    double highY = minYCoord;

                    for (int divisionYIndex = 0; divisionYIndex < 2; divisionYIndex++)
                    {
                        lowY = highY;
                        highY = highY + (maxYCoord - minYCoord) / 2;

                        //Start from minXCoord
                        double lowX = minXCoord;
                        double highX = minXCoord;

                        for (int divisionXIndex = 0; divisionXIndex < divisionX; divisionXIndex++)
                        {
                            lowX = highX;
                            highX = highX + (maxXCoord - minXCoord) / divisionX;

                            //Inside each divison total reaction for each group
                            int groupSumIndex = 0;
                            foreach (Dictionary<string, string> group in listChoosenGroupParsed)
                            {
                                string groupName = group["groupName"];
                                string color = group["color"];

                                //Text color is either decided by user or auto (between 0 and 1 based on how many groups there are)
                                System.Drawing.Color textColor = new System.Drawing.Color();
                                if (color == "Auto")
                                {
                                    if (listChoosenGroup.Count > 1)
                                    {
                                        double colorMapFraction = Convert.ToDouble(groupSumIndex) / (Convert.ToDouble(listChoosenGroup.Count) - 1);
                                        textColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .5);
                                    }
                                    else
                                    {
                                        double colorMapFraction = 0;
                                        textColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .5);
                                    }
                                }
                                else
                                {
                                    string colorHex = color;
                                    textColor = System.Drawing.ColorTranslator.FromHtml(colorHex);
                                }

                                double reactionSum = new double();

                                //Inside each group go through each point
                                foreach (Dictionary<string, object> reactionData in compiledReactionData)
                                {
                                    double yCoord = Convert.ToDouble(reactionData["yCoord"]);
                                    double xCoord = Convert.ToDouble(reactionData["xCoord"]);

                                    //If the point is within grid, then add into reactionSum
                                    if ((yCoord >= lowY) && (yCoord < highY) && (xCoord >= lowX) && (xCoord < highX) 
                                        && (Convert.ToString(reactionData["groupName"]) == groupName))
                                    {
                                        reactionSum += Convert.ToDouble(reactionData["reaction"]);
                                    }
                                }

                                //Put the sum inside plot
                                if (divisionYIndex == 0)
                                {
                                    double textXCoord = (lowX + highX) / 2;
                                    double textYCoord = (lowY + highY) / 2 - offsetY - groupSumIndex * 6;
                                    ReactionPlot.Plot.AddText(Convert.ToString(Convert.ToInt32(reactionSum)), textXCoord, textYCoord, 20, textColor);
                                }
                                else
                                {
                                    double textXCoord = (lowX + highX) / 2;
                                    double textYCoord = (lowY + highY) / 2 + offsetY + groupSumIndex * 6;
                                    ReactionPlot.Plot.AddText(Convert.ToString(Convert.ToInt32(reactionSum)), textXCoord, textYCoord, 20, textColor);
                                }

                                groupSumIndex++;

                            }


                        }



                    }




                }


            }


            ReactionPlot.Plot.AxisScaleLock(true);
            ReactionPlot.Plot.Title(title, true, null, 36, "Arial Narrow");
            ReactionPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles

            //Axis
            ReactionPlot.Plot.YAxis.Label("Y coordinate (m)", null, 28, true, "Arial Narrow");
            ReactionPlot.Plot.XAxis.Label("X coordinate (m)", null, 28, true, "Arial Narrow");
            ReactionPlot.Plot.YAxis.MinimumTickSpacing(50);
            ReactionPlot.Plot.XAxis.MinimumTickSpacing(50);

            ReactionPlot.Refresh();

            return isError;
        }
        


    }
}
