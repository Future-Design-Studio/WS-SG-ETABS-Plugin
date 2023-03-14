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

namespace WS_SG_ETABS_Plugin.src.plot_reaction.graph
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionGraphResult.xaml
    /// </summary>
    public partial class WindowPlotReactionGraphResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowPlotReactionGraphResult(cSapModel SapModel, cPluginCallback Plugin, List<Dictionary<string, string>> groupAndColor, string outputCase,
            string reactionDirection, double forceFactor, double bubbleScaleFactor, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY, string title, bool splitX, bool splitY,
            int divisionX, int divisionY, double offsetX, double offsetY, int sumTextSizeX, int sumTextSizeY, double separationX, double separationY, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen
            List<string> listChoosenGroupName = new List<string>();
            foreach (Dictionary<string, string> group in groupAndColor)
            {
                listChoosenGroupName.Add(group["groupName"]);
            }


            //Check if any group is choosen
            bool anyGroupChoosen = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroupName);

            if (!anyGroupChoosen)
            {
                return;
            }

            InitializeComponent();

            //Main logic
            List<WpfPlot> reactionPlotList = new List<WpfPlot>();
            WpfPlot reactionPlot1 = new WpfPlot();
            reactionPlotList.Add(reactionPlot1);

            bool isError = false;
            double baseReaction = 0;
            double totalReactionInPlot = 0;

            (isError, baseReaction, totalReactionInPlot) = src.plot_reaction.graph.FunctionPlotReactionGraph.PlotReaction(_SapModel, groupAndColor,
            outputCase, reactionDirection, ReactionPlot, forceFactor, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, title, splitX, splitY, divisionX, divisionY, offsetX, offsetY,
            sumTextSizeX, sumTextSizeY, separationX, separationY, databaseTableList);

            //Show comparison of base reaction to total reaction in plot
            IsEnabled = false;
            src.plot_reaction.graph.WindowPlotReactionGraphBaseReaction windowPlotReactionBaseReaction = new src.plot_reaction.graph.WindowPlotReactionGraphBaseReaction(baseReaction, totalReactionInPlot);
            IsEnabled = true;

            //ReactionPlot.Content = reactionPlotList[0];

            if (isError)
            {
                return;
            }

            ShowDialog();

        }
    }
}
