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
            int divisionX, int divisionY, double offsetX, double offsetY, int sumTextSizeX, int sumTextSizeY, double separationX, double separationY, 
            bool? includeAllJoints, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen
            bool anyGroupChoosen = true;
            if (includeAllJoints == false)
            {
                List<string> listChoosenGroupName = new List<string>();
                foreach (Dictionary<string, string> group in groupAndColor)
                {
                    listChoosenGroupName.Add(group["groupName"]);
                }

                anyGroupChoosen = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroupName);
            }

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
            List<src.plot_reaction.graph.ClassPlotGraph.SumReactionPerGroup> sumReactionPerGroupList = new List<src.plot_reaction.graph.ClassPlotGraph.SumReactionPerGroup>();

            (isError, baseReaction, totalReactionInPlot, sumReactionPerGroupList) = src.plot_reaction.graph.FunctionPlotReactionGraph.PlotReaction(_SapModel, groupAndColor,
            outputCase, reactionDirection, ReactionPlot, forceFactor, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, title, splitX, splitY, divisionX, divisionY, offsetX, offsetY,
            sumTextSizeX, sumTextSizeY, separationX, separationY, includeAllJoints, databaseTableList);

            //Show sum per group
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Name";
            c1.Binding = new Binding("Name");
            c1.Width = 110;
            dataGridSumPerGroup.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Color";
            c2.Width = 110;
            c2.Binding = new Binding("Color");
            dataGridSumPerGroup.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Total reaction (kN)";
            c3.Width = 110;
            c3.Binding = new Binding("SumReaction");
            dataGridSumPerGroup.Columns.Add(c3);

            foreach (src.plot_reaction.graph.ClassPlotGraph.SumReactionPerGroup sumReactionPerGroup in sumReactionPerGroupList)
            {
                dataGridSumPerGroup.Items.Add(sumReactionPerGroup);
            }


            //If base reaction and total reaction not same, show warning
            if (Math.Abs(baseReaction - totalReactionInPlot) > 1)
            {
                //Show comparison of base reaction to total reaction in plot
                IsEnabled = false;
                src.plot_reaction.graph.WindowPlotReactionGraphBaseReaction windowPlotReactionBaseReaction = new src.plot_reaction.graph.WindowPlotReactionGraphBaseReaction(baseReaction, totalReactionInPlot);
                IsEnabled = true;
            }

            if (isError)
            {
                return;
            }

            

            ShowDialog();


        }
    }
}
