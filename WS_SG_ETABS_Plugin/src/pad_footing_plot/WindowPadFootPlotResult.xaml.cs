using ETABSv1;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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

        List<ClassPadFootPlot.GroupDefinition> groupDefinitions = new List<ClassPadFootPlot.GroupDefinition>();
        string plotTitleGlobal;
        int textSizeGlobal;
        int titleSizeGlobal;
        int axisTitleSizeGlobal;
        double gridSpacingXGlobal;
        double gridSpacingYGlobal;

        List<string> groupNames = new List<string>();

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
            (isError, groupDefinitions) = src.pad_footing_plot.FunctionPadFootPlot.PadFootingPlot(
                _SapModel, _Plugin, listChoosenLoadCase, choosenGroup, sizeIncrement, plotTitle, thickness, density, soilCapacity, maxUtilizationRatio, textSize, titleSize, axisTitleSize, gridSpacingX, gridSpacingY, databaseTableList, ReactionPlot, fileName, filePath);

            //List of group to filter
            //groupNames.Add("All");
            foreach (ClassPadFootPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                groupNames.Add(Convert.ToString(groupDefinition.GroupIndex));
            }

            listBoxShownGroup.ItemsSource = groupNames;
            selectedGroupChanged();
            HelperPlot.ResetZoom(ReactionPlot);

            listBoxShownGroup.ItemsSource = groupNames;
            listBoxShownGroup.SelectAll();

            //Show group
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Group Index";
            c1.Binding = new System.Windows.Data.Binding("GroupIndex");
            c1.Width = 100;
            dataGridGroupDefinitions.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Color";
            c2.Width = 250;
            c2.Binding = new System.Windows.Data.Binding("Color");
            dataGridGroupDefinitions.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Width (m)";
            c3.Width = 200;
            c3.Binding = new System.Windows.Data.Binding("Width");
            dataGridGroupDefinitions.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn();

            foreach (ClassPadFootPlot.GroupDefinition groupDefinition in groupDefinitions)
            {
                dataGridGroupDefinitions.Items.Add(groupDefinition);
            }

            if (isError)
            {
                return;
            }

            ShowDialog();

        }

        private void selectedGroupChanged()
        {
            List<string> selectedGroup = listBoxShownGroup.Items.OfType<string>().ToList();

            ScottPlot.AxisLimits axisLimits = ReactionPlot.Plot.GetAxisLimits();

            FunctionPadFootPlot.FilterPadFootingPlot(groupDefinitions, plotTitleGlobal, textSizeGlobal, titleSizeGlobal, axisTitleSizeGlobal, 
                gridSpacingXGlobal, gridSpacingYGlobal, ReactionPlot, selectedGroup, axisLimits);
        }


        private void buttonShowAllGroup_Click(object sender, RoutedEventArgs e)
        {
            listBoxShownGroup.SelectAll();
        }

        private void hideAllGroup_Click(object sender, RoutedEventArgs e)
        {
            listBoxShownGroup.UnselectAll();
        }

        private void listBoxShownGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> selectedGroup = listBoxShownGroup.SelectedItems.OfType<string>().ToList();

            ScottPlot.AxisLimits axisLimits = ReactionPlot.Plot.GetAxisLimits();

            FunctionPadFootPlot.FilterPadFootingPlot(groupDefinitions, plotTitleGlobal, textSizeGlobal, titleSizeGlobal, axisTitleSizeGlobal,
                gridSpacingXGlobal, gridSpacingYGlobal, ReactionPlot, selectedGroup, axisLimits);
        }
    }
}
