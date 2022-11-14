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

        public static bool PlotReaction(cSapModel Sap, List<string> listChoosenGroup, string outputCase, string reactionDirection, WpfPlot ReactionPlot, double bubbleScaleFactor, double textScaleFactor)
        {
            bool isError = false;
            bool showWarning = true;

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(Sap);

            //Get required table
            
            List<Dictionary<string, string>> pointConnectivityTable = new List<Dictionary<string, string>>();            
            (isError, pointConnectivityTable) = src.HelperEtabs.GetTableDisplay(Sap, "Point Object Connectivity", showWarning);

            int groupIndex = 0;
            foreach (string group in listChoosenGroup)
            {
                //Bubble color is between 0 and 1 based on how many groups there are
                double plotColor = (Convert.ToDouble(groupIndex)  + 1) / Convert.ToDouble(listChoosenGroup.Count());


                //Get joint reaction for 1 specific group
                List<Dictionary<string, string>> jointReactionTable = new List<Dictionary<string, string>>();

                List<string> groupList = new List<string>();
                groupList.Add(group);

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
                            }
                        }

                        //Plotting
                        double bubbleSize = Math.Abs(Convert.ToDouble(jointReactionRow[reactionDirection])) * bubbleScaleFactor;
                        System.Drawing.Color bubbleColor = ScottPlot.Drawing.Colormap.Jet.GetColor(plotColor, .5);

                        var myBubblePlot = ReactionPlot.Plot.AddBubblePlot();
                        myBubblePlot.Add(
                            x: xCoord,
                            y: yCoord,
                            radius: bubbleSize,
                            fillColor: bubbleColor,
                            edgeColor: System.Drawing.Color.Transparent,
                            edgeWidth: 1
                        );

                        var txt = ReactionPlot.Plot.AddText(jointReactionRow[reactionDirection], xCoord, yCoord);
                        //txt.FontBold = true;
                        txt.FontSize = Convert.ToSingle(12 * textScaleFactor);
                        txt.Color = System.Drawing.Color.Black;
                        txt.Alignment = Alignment.MiddleCenter;

                    }
                }

                groupIndex++;
            }

            ReactionPlot.Plot.AxisScaleLock(true);
            ReactionPlot.Plot.Title("Bubble Plot with Labels");
            ReactionPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            ReactionPlot.Refresh();

            return isError;
        }

    }
}
