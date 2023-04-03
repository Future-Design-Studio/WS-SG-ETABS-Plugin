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

namespace WS_SG_ETABS_Plugin.src.plot_reaction.table
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionTableResult.xaml
    /// </summary>
    public partial class WindowPlotReactionTableResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowPlotReactionTableResult(cSapModel SapModel, cPluginCallback Plugin, List<Dictionary<string, string>> groupAndColor, List<string> listBoxChoosenLoadCase, string plotTitle, 
            double bubbleScaleFactor, int textSize, int titleSize, int axisTitleSize, int gridSpacingX, int gridSpacingY, bool? showRenumber,
            bool? includeAllJoints, List<src.HelperEtabs.DatabaseTable> databaseTableList, string fileName, string filePath)
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
            bool anyChoosenGroup = src.HelperUI.CheckGroupListExist(_SapModel, listChoosenGroupName);

            if (!anyChoosenGroup)
            {
                return;
            }

            //Check if any load case is choosen
            bool anyChoosenLoadCase = src.HelperUI.CheckLoadCaseListExist(_SapModel, listBoxChoosenLoadCase);

            if (!anyChoosenLoadCase)
            {
                return;
            }


            InitializeComponent();

            bool isError = false;

            isError = src.plot_reaction.table.FunctionPlotReactionTable.CreateExcelAndPlotPosition(_SapModel, _Plugin, ReactionPlot, groupAndColor, listBoxChoosenLoadCase, plotTitle,
            bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, showRenumber, includeAllJoints, databaseTableList, fileName, filePath);

            if (isError)
            {
                return;
            }

            ShowDialog();
        }
    }
}
