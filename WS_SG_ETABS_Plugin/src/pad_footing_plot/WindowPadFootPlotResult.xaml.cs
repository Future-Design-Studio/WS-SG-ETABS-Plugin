using ETABSv1;
using ScottPlot;
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
using WS_SG_ETABS_Plugin.src.punching_shear_plot;

namespace WS_SG_ETABS_Plugin.src.pad_footing_plot
{
    /// <summary>
    /// Interaction logic for WindowPadFootPlotResult.xaml
    /// </summary>
    public partial class WindowPadFootPlotResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<ClassPunchingShrPlot.GroupDefinition> groupDefinitions = new List<ClassPunchingShrPlot.GroupDefinition>();
        string plotTitleGlobal;
        int textSizeGlobal;
        int titleSizeGlobal;
        int axisTitleSizeGlobal;
        double gridSpacingXGlobal;
        double gridSpacingYGlobal;

        public WindowPadFootPlotResult(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, double sizeIncrement, string plotTitle, double thickness, double density,
            double soilCapacity, double maxUtilizationRatio, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY,
            List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            plotTitleGlobal= plotTitle;
            textSizeGlobal = textSize;
            titleSizeGlobal = titleSize;
            axisTitleSizeGlobal = axisTitleSize;
            gridSpacingXGlobal = gridSpacingX;
            gridSpacingYGlobal = gridSpacingY;

            //Check if any load case is choosen
            bool anyChoosenLoadCase = src.HelperUI.CheckLoadCaseListExist(_SapModel, listChoosenLoadCase);

            if (!anyChoosenLoadCase)
            {
                return;
            }

            InitializeComponent();

            bool isError = false;
            isError = src.pad_footing_plot.FunctionPadFootPlot.PadFootingPlot(
                _SapModel, _Plugin, listChoosenLoadCase, choosenGroup, sizeIncrement, plotTitle, thickness, density, soilCapacity, maxUtilizationRatio, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, databaseTableList, ReactionPlot, fileName, filePath);



        }

        private void comBoxGroupFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
