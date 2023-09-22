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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.plot_sum_mass_participation
{
    /// <summary>
    /// Interaction logic for WindowSumParticipPlotResult.xaml
    /// </summary>
    public partial class WindowSumParticipPlotResult : Window
    {
        public WindowSumParticipPlotResult(cSapModel _SapModel, cPluginCallback _Plugin, int legendTextSize, int titleSize, int axisTitleSize, int tickLabelSize, bool showUz,
            string fileName, string filePath)
        {
            InitializeComponent();

            FunctionSumParticipPlot.PlotSumParticipationFactor(_SapModel, _Plugin, legendTextSize, titleSize, axisTitleSize, tickLabelSize, showUz, fileName, filePath, Plot);

            ShowDialog();
        }
    }
}
