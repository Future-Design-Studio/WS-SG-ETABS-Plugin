using ETABSv1;
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
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowPlotReactionResult(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenGroup, string outputCase, 
            string reactionDirection, double forceFactor, double bubbleScaleFactor, int textSize, string title, bool splitX, bool splitY, 
            int divisionX, int divisionY, double offsetX, double offsetY, int sumTextSizeX, int sumTextSizeY, double separationX, double separationY)
        {

            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen
            bool requiredGroupListExist = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroup);

            if (!requiredGroupListExist)
            {
                return;
            }

            InitializeComponent();

            //Main logic
            bool isError = src.plot_reaction.FunctionPlotReaction.PlotReaction(_SapModel, listChoosenGroup, 
                outputCase, reactionDirection, ReactionPlot, forceFactor, bubbleScaleFactor, textSize, title, splitX, splitY, divisionX, divisionY, offsetX, offsetY,
                sumTextSizeX, sumTextSizeY, separationX, separationY);

            if (isError)
            {
                return;
            }

            ShowDialog();

        }
    }
}
