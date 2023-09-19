using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;
using ScottPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    /// <summary>
    /// Interaction logic for WindowDriftPlotResult.xaml
    /// </summary>
    public partial class WindowDriftPlotResult : Window
    {
        public WindowDriftPlotResult(cSapModel SapModel, cPluginCallback Plugin, string outputCase, string displacementDirection, string maxDisplacementFactor, string choosenGroup, string plotTitle,
            int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath)

        {
            InitializeComponent();

            src.plot_drift.FunctionDriftPlot.PlotDrift(SapModel, Plugin, outputCase, displacementDirection, maxDisplacementFactor, choosenGroup, plotTitle,
            textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY,
            databaseTableList, fileName, filePath, DriftPlot, DisplacementPlot);

            ShowDialog();
        }
    }
}
