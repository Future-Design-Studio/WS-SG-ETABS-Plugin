using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WS_SG_ETABS_Plugin.src.plot_reaction
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionResult.xaml
    /// </summary>
    public partial class WindowPlotReactionResult : Window
    {
        public WindowPlotReactionResult()
        {
            InitializeComponent();

            /*
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            ReactionPlot.Plot.AddScatter(dataX, dataY);
            ReactionPlot.Refresh();
            */
            Random rand = new Random(0);
            int pointCount = 30;
            double[] xs = DataGen.Consecutive(pointCount);
            double[] ys = DataGen.Random(rand, pointCount, 10);
            string[] labels = ys.Select(x => x.ToString("N2")).ToArray();

            for (int i = 0; i < xs.Length; i++)
            {
                // give each bubble a random size and make smaller ones bluer
                double randomValue = rand.NextDouble();
                double colourValue = Convert.ToDouble(i) / Convert.ToDouble(xs.Length);
                double bubbleSize = randomValue * 30 + 10;
                System.Drawing.Color bubbleColor = ScottPlot.Drawing.Colormap.Jet.GetColor(colourValue, .5);

                var myBubblePlot = ReactionPlot.Plot.AddBubblePlot();
                myBubblePlot.Add(
                    x: xs[i],
                    y: ys[i],
                    radius: bubbleSize,
                    fillColor: bubbleColor,
                    edgeColor: System.Drawing.Color.Transparent,
                    edgeWidth: 1
                );

                var txt = ReactionPlot.Plot.AddText(labels[i], xs[i], ys[i]);
                txt.FontBold = true;
                txt.Color = System.Drawing.Color.Black;
                txt.Alignment = Alignment.MiddleCenter;

            }

            ReactionPlot.Plot.Title("Bubble Plot with Labels");
            ReactionPlot.Plot.AxisAuto(.2, .25); // zoom out to accommodate large bubbles
            ReactionPlot.Refresh();

            ShowDialog();

        }
    }
}
