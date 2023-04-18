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
using static WS_SG_ETABS_Plugin.src.punching_shear_plot.ClassPunchingShrPlot;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    /// <summary>
    /// Interaction logic for WindowPunchShrPlotResult.xaml
    /// </summary>
    public partial class WindowPunchShrPlotResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<ClassPunchingShrPlot.GroupDefinition> groupDefinitions = new List<ClassPunchingShrPlot.GroupDefinition>();
        string plotTitleContainer;
        double forceFactorContainer;
        double bubbleScaleFactorContainer;
        int textSizeContainer;
        int titleSizeContainer;
        int axisTitleSizeContainer;
        double gridSpacingXContainer;
        double gridSpacingYContainer;

        List<string> groupNames = new List<string>();

        public WindowPunchShrPlotResult(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, double forceIncrement, string plotTitle, 
            double forceFactor, double bubbleScaleFactor, int textSize, int titleSize, int axisTitleSize, double gridSpacingX, double gridSpacingY, 
            List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            plotTitleContainer = plotTitle;
            forceFactorContainer = forceFactor;
            bubbleScaleFactorContainer = bubbleScaleFactor;
            textSizeContainer = textSize;
            titleSizeContainer = titleSize;
            axisTitleSizeContainer = axisTitleSize;
            gridSpacingXContainer = gridSpacingX;
            gridSpacingYContainer = gridSpacingY;

            //Check if any load case is choosen
            bool anyChoosenLoadCase = src.HelperUI.CheckLoadCaseListExist(_SapModel, listChoosenLoadCase);

            if (!anyChoosenLoadCase)
            {
                return;
            }

            InitializeComponent();

            bool isError = false;
            (isError, groupDefinitions) = src.punching_shear_plot.FunctionPunchingShrPlot.PunchingShearPlot(
                _SapModel, _Plugin, listChoosenLoadCase, choosenGroup, forceIncrement, plotTitle, forceFactor, bubbleScaleFactor, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, databaseTableList, ReactionPlot);

            //List of group to filter
            groupNames.Add("All");
            foreach(ClassPunchingShrPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                groupNames.Add(Convert.ToString(groupDefinition.GroupIndex));
            }

            comBoxGroupFilter.ItemsSource = groupNames;
            comBoxGroupFilter.SelectedIndex = 0;
            HelperPlot.ResetZoom(ReactionPlot);

            //Show sum per group
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Group Index";
            c1.Binding = new Binding("GroupIndex");
            c1.Width = 100;
            dataGridGroupDefinitions.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Color";
            c2.Width = 250;
            c2.Binding = new Binding("Color");
            dataGridGroupDefinitions.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Column Section Property";
            c3.Width = 200;
            c3.Binding = new Binding("SectProp");
            dataGridGroupDefinitions.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Min shear force (kN)";
            c4.Width = 200;
            c4.Binding = new Binding("MinForce");
            dataGridGroupDefinitions.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Max shear force (kN)";
            c5.Width = 200;
            c5.Binding = new Binding("MaxForce");
            dataGridGroupDefinitions.Columns.Add(c5);

            foreach (ClassPunchingShrPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                dataGridGroupDefinitions.Items.Add(groupDefinition);
            }

            if (isError)
            {
                return;
            }

            ShowDialog();


        }

        private void comBoxGroupFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedGroup = groupNames[comBoxGroupFilter.SelectedIndex];

            ScottPlot.AxisLimits axisLimits = ReactionPlot.Plot.GetAxisLimits();

            FunctionPunchingShrPlot.FilterPunchingShearPlot(groupDefinitions, plotTitleContainer, forceFactorContainer,
            bubbleScaleFactorContainer, textSizeContainer, titleSizeContainer, axisTitleSizeContainer, gridSpacingXContainer, gridSpacingYContainer, ReactionPlot, selectedGroup, axisLimits);
        }
    }
}
