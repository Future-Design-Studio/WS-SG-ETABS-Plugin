using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperPlot
    {
        /// <summary>
        /// Bubble plot chart using ScottPlot
        /// </summary>
        /// <param name="Plot"></param>
        /// <param name="bubbleSize"></param>
        /// <param name="bubbleText"></param>
        /// <param name="textSize"></param>
        /// <param name="colorUseHex"></param>
        /// <param name="colorHex"></param>
        /// <param name="colorMapFraction"></param>
        /// <param name="xCoord"></param>
        /// <param name="yCoord"></param>
        public static void BubblePlotter(WpfPlot Plot, double bubbleSize, string bubbleText, int textSize, System.Drawing.Color bubbleColor, double xCoord, double yCoord)
        {


            var myBubblePlot = Plot.Plot.AddBubblePlot();
            myBubblePlot.Add(
                x: xCoord,
                y: yCoord,
                radius: bubbleSize,
                fillColor: bubbleColor,
                edgeColor: System.Drawing.Color.Transparent,
                edgeWidth: 1
            );

            //Text for each bubble in kN
            var txt = Plot.Plot.AddText(bubbleText, xCoord, yCoord);
            //txt.FontBold = true;
            txt.FontSize = Convert.ToSingle(textSize);
            txt.Color = System.Drawing.Color.Black;
            txt.Alignment = Alignment.MiddleCenter;
            txt.FontName = "Arial Narrow";

        }

    }
}
