using ETABSv1;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            WpfPlot ReactionPlot, double forceFactor, double bubbleScaleFactor, double textSize, string title)
        {
            bool isError = false;
            bool showWarning = true;

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
                double colorMapFraction = new double();
                if (color == "Auto")
                {
                    if (listChoosenGroup.Count > 1)
                    {
                        colorMapFraction = Convert.ToDouble(groupIndex) / (Convert.ToDouble(listChoosenGroup.Count) - 1);
                    }
                    else
                    {
                        colorMapFraction = 0;
                    }
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

                        //Plotting
                        //Bubble size is from force factored
                        double bubbleSize = Math.Abs(Convert.ToDouble(jointReactionRow[reactionDirection])) * bubbleScaleFactor * forceFactor;

                        //Color is either auto from colormap or hexcode
                        System.Drawing.Color bubbleColor = new System.Drawing.Color();
                        if (color == "Auto")
                        {
                            bubbleColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colorMapFraction, .5);
                        }
                        else
                        {
                            bubbleColor = System.Drawing.ColorTranslator.FromHtml(color);
                        }

                        var myBubblePlot = ReactionPlot.Plot.AddBubblePlot();
                        myBubblePlot.Add(
                            x: xCoord,
                            y: yCoord,
                            radius: bubbleSize,
                            fillColor: bubbleColor,
                            edgeColor: System.Drawing.Color.Transparent,
                            edgeWidth: 1
                        );

                        var txt = ReactionPlot.Plot.AddText(Convert.ToString(Convert.ToInt32(Convert.ToDouble(jointReactionRow[reactionDirection]) * forceFactor)), xCoord, yCoord);
                        //txt.FontBold = true;
                        txt.FontSize = Convert.ToSingle(textSize);
                        txt.Color = System.Drawing.Color.Black;
                        txt.Alignment = Alignment.MiddleCenter;
                        txt.FontName = "Arial Narrow";

                    }
                }

                groupIndex++;
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
