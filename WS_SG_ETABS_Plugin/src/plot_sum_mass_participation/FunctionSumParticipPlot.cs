using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using Microsoft.Office.Interop.Excel;
using Accord.Math;

namespace WS_SG_ETABS_Plugin.src.plot_sum_mass_participation
{
    internal class FunctionSumParticipPlot
    {
        public static void PlotSumParticipationFactor(cSapModel SapModel, cPluginCallback Plugin, int legendTextSize, int titleSize, int axisTitleSize, int tickLabelSize, bool showUz,
            string fileName, string filePath, WpfPlot Plot)
        {
            //Get database
            bool isError = false;
            HelperEtabs.DatabaseTable modalParticipationMassTable = new HelperEtabs.DatabaseTable();
            (isError, modalParticipationMassTable) = src.HelperEtabs.GetTableDisplay(SapModel, "Modal Participating Mass Ratios", false);

            List<Dictionary<string, string>> tableInList = modalParticipationMassTable.TableInList;

            List<double> modal = new List<double>();
            List<double> sumUx = new List<double>();
            List<double> sumUy = new List<double>();
            List<double> sumUz = new List<double>();
            List<double> limit = new List<double>();
            foreach (Dictionary<string, string> row in tableInList)
            {
                modal.Add(Convert.ToDouble(row["Mode"]));
                sumUx.Add(Convert.ToDouble(row["SumUX"]));
                sumUy.Add(Convert.ToDouble(row["SumUY"]));
                sumUz.Add(Convert.ToDouble(row["SumUZ"]));
                limit.Add(0.9);
            }

            Plot.Plot.AddScatterLines(modal.ToArray(), limit.ToArray(), System.Drawing.ColorTranslator.FromHtml("#FF0000"), 2, LineStyle.Dash, $"90%");
            Plot.Plot.AddScatterLines(modal.ToArray(), sumUx.ToArray(), null, 2, LineStyle.Solid, $"Ux");
            Plot.Plot.AddScatterLines(modal.ToArray(), sumUy.ToArray(), null, 2, LineStyle.Solid, $"Uy");
            if (showUz)
            {
                Plot.Plot.AddScatterLines(modal.ToArray(), sumUz.ToArray(), null, 2, LineStyle.Solid, $"Uz");
            }

            Plot.Plot.Title($"Modal Sum Participation Mass Ratio", true, null, titleSize, "Arial Narrow");
            Plot.Plot.YAxis.Label("Modal Sum Participation Mass Ratio", null, axisTitleSize, true, "Arial Narrow");
            Plot.Plot.XAxis.Label("Mode", null, axisTitleSize, true, "Arial Narrow");
            //Plot.Plot.YAxis.ManualTickSpacing(gridSpacingY);
            Plot.Plot.XAxis.ManualTickSpacing(2);
            Plot.Plot.YAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            Plot.Plot.XAxis.TickLabelStyle(null, null, tickLabelSize, null, null);
            var legend = Plot.Plot.Legend();
            legend.FontSize = legendTextSize;

            Plot.Refresh();
        }
    }
}
