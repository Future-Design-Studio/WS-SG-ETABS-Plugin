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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.carbon
{
    /// <summary>
    /// Interaction logic for WindowCarbonResult.xaml
    /// </summary>
    public partial class WindowCarbonResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowCarbonResult(cSapModel SapModel, cPluginCallback Plugin, Dictionary<string, string> inputCarbon, List<string> listChoosenGroup, bool? includeAll)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //If checkbox all elements is choosen
            if (includeAll == true)
            {
                listChoosenGroup = new List<string> { "All" };
            }
            else
            {
                //Check if any group is choosen for carbon calculation
                bool requiredGroupListExist = src.HelperUI.CheckGroupListExistCarbon(_SapModel, listChoosenGroup);

                if (!requiredGroupListExist)
                {
                    return;
                }
                //Check for duplicate frame and area
                src.HelperEtabs.CheckDuplicateObjectInGroup(_SapModel, listChoosenGroup);
            }

            //Getting data
            (bool isError, Dictionary<string, double> outputEachObjectTypeCarbon, double outputSumCarbon) = src.carbon.FunctionCarbon.CalculateCarbon(_SapModel, inputCarbon, listChoosenGroup);

            if (isError)
            {
                return;
            }

            InitializeComponent();

            //Put constructor here
            double carbonColumn = Math.Round(outputEachObjectTypeCarbon["Carbon From Column"]);
            double carbonBeam= Math.Round(outputEachObjectTypeCarbon["Carbon From Beam"]);
            double carbonBrace = Math.Round(outputEachObjectTypeCarbon["Carbon From Brace"]);
            double carbonFloor = Math.Round(outputEachObjectTypeCarbon["Carbon From Floor"]);
            double carbonWall = Math.Round(outputEachObjectTypeCarbon["Carbon From Wall"]);
            double sumCarbon = Math.Round(outputSumCarbon);

            //Fill in result label
            labelCarbonColumn.Content = carbonColumn;
            labelCarbonBeam.Content = carbonBeam;
            labelCarbonBrace.Content = carbonBrace;
            labelCarbonFloor.Content = carbonFloor;
            labelCarbonWall.Content = carbonWall;
            labelCarbonSum.Content = sumCarbon;
            
            //Creating chart
            SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Column",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(carbonColumn) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Beam",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(carbonBeam) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Brace",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(carbonBrace) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Floor",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(carbonFloor) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Wall",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(carbonWall) },
                    DataLabels = true
                }

            };

            DataContext = this;

            //Put ShowDialog at last after completing constructor
            ShowDialog();
        }
        public SeriesCollection SeriesCollection { get; set;}
    }
}
