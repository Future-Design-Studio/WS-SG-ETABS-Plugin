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
using SAFEv1;
using ScottPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    /// <summary>
    /// Interaction logic for WindowDriftPlotResult.xaml
    /// </summary>
    public partial class WindowDriftPlotResult : Window
    {
        public WindowDriftPlotResult(ETABSv1.cSapModel SapModel, ETABSv1.cPluginCallback Plugin, List<string> listOutputCase, string displacementDirection, string maxAllowableDispType, string maxAllowableDisplacementFactor, string choosenGroup,
            int legendTextSize, int titleSize, int axisTitleSize, int tickLabelSize, double gridSpacingDisplacementX, double gridSpacingDriftX, double gridSpacingY, bool showMaxDisp,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath, string displacementXAxisMaxType, string displacementXAxisMax, string driftXAxisMaxType, string driftXAxisMax)

        {
            InitializeComponent();

            src.plot_drift.FunctionDriftPlot.PlotDrift(SapModel, Plugin, listOutputCase, displacementDirection, maxAllowableDispType, maxAllowableDisplacementFactor, choosenGroup, legendTextSize,
                    titleSize, axisTitleSize, tickLabelSize, gridSpacingDisplacementX, gridSpacingDriftX, gridSpacingY, showMaxDisp, databaseTableList, fileName, filePath, DriftPlot, DisplacementPlot,
                    displacementXAxisMaxType, displacementXAxisMax, driftXAxisMaxType, driftXAxisMax);

            ShowDialog();
        }
    }
}
