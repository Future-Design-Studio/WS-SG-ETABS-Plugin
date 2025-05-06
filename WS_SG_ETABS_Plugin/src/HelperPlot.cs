using ScottPlot.WPF;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

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
            if (!string.IsNullOrWhiteSpace(bubbleText))
            {
                var txt = Plot.Plot.AddText(bubbleText, xCoord, yCoord);
                //txt.FontBold = true;
                txt.FontSize = Convert.ToSingle(textSize);
                txt.Color = System.Drawing.Color.Black;
                txt.Alignment = Alignment.MiddleCenter;
                txt.FontName = "Arial Narrow";
            }
        }

        public static void RectanglePlotter(WpfPlot Plot, double xCenter, double yCenter, string text, int textSize, System.Drawing.Color color, double xWidth, double yWidth, 
            double borderWidth, System.Drawing.Color borderColor)
        {

            double[] xs = { xCenter - xWidth / 2.0, xCenter + xWidth / 2.0, xCenter + xWidth / 2.0, xCenter - xWidth / 2.0 };
            double[] ys = { yCenter + yWidth / 2.0, yCenter + yWidth / 2.0, yCenter - yWidth / 2.0, yCenter - yWidth / 2.0 };
            Plot.Plot.AddPolygon(xs, ys, color, borderWidth, borderColor);

            //Text for each bubble in kN
            if (!string.IsNullOrWhiteSpace(text))
            {
                var txt = Plot.Plot.AddText(text, xCenter, yCenter);
                //txt.FontBold = true;
                txt.FontSize = Convert.ToSingle(textSize);
                txt.Color = System.Drawing.Color.Black;
                txt.Alignment = Alignment.MiddleCenter;
                txt.FontName = "Arial Narrow";
            }
        }

        /// <summary>
        /// Reset zoom level
        /// </summary>
        /// <param name="Plot"></param>
        public static void ResetZoom(WpfPlot Plot)
        {
            Plot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
        }

        public static void AddAnnotationToPlot(WpfPlot Plot, string annotationText, double textSize, double marginTopX, double marginTopY, double borderWidth, System.Drawing.Color textColor, 
            System.Drawing.Color backgroundColor, bool isTextBold = false)
        {
            var text = Plot.Plot.AddAnnotation(annotationText, marginTopX, marginTopY);
            text.Font.Bold = isTextBold;
            text.BorderWidth = Convert.ToSingle(borderWidth);
            text.Shadow = false;
            text.Font.Color = textColor;
            text.BackgroundColor = backgroundColor;
            text.Font.Size = Convert.ToSingle(textSize);
        }
    }
}
