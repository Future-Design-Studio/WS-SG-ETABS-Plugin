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

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    /// <summary>
    /// Interaction logic for WindowWallDesignResult.xaml
    /// </summary>
    public partial class WindowWallDesignResult : Window
    {
        List<ClassWallDesign.WallDesignDetail> wallDesignDetailListGlobal = new List<ClassWallDesign.WallDesignDetail>();
        List<double> uniqueXGlobal = new List<double>();
        List<double> uniqueYGlobal = new List<double>();
        double countPassGlobal = 0;
        double countFailGlobal = 0;

        int titleSizeGlobal = 0;
        int axisTitleSizeGlobal = 0;
        int tickLabelSizeGlobal = 0;

        public WindowWallDesignResult(List<src.HelperEtabs.DatabaseTable> databaseTableList, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize)
        {
            titleSizeGlobal = titleSize;
            axisTitleSizeGlobal = axisTitleSize;
            tickLabelSizeGlobal = tickLabelSize;

            InitializeComponent();

            bool isError = false;

            (isError, wallDesignDetailListGlobal, uniqueXGlobal, uniqueYGlobal, countPassGlobal, countFailGlobal) = FunctionWallDesignView.PlotWallDesignResult(ResultPlot, databaseTableList, 
                choosenGroup, typeOfFailure, tableInListPierDesignResult, tableInListSpandrelDesignResult, titleSize, axisTitleSize,
                tickLabelSize);
            if (isError) { return; }

            ShowDialog();


        }

        private void checkBoxShowOk_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlotBasedOnCheckBox();
        }

        private void checkBoxShowFail_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlotBasedOnCheckBox();
        }

        private void checkBoxShowNull_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlotBasedOnCheckBox();
        }
        
        private void UpdatePlotBasedOnCheckBox()
        {
            bool? showOk = checkBoxShowOk.IsChecked;
            bool? showFail = checkBoxShowFail.IsChecked;
            bool? showNull = checkBoxShowNull.IsChecked;

            FunctionWallDesignView.FilterWallDesignResult(ResultPlot, showOk, showFail, showNull, wallDesignDetailListGlobal, uniqueXGlobal, uniqueYGlobal, countPassGlobal, countFailGlobal, 
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal);
        }

    }
}
