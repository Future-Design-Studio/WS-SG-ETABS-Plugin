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
        List<ClassWallDesign.WallDesignDetail> wallDesignDetailFilteredListGlobal = new List<ClassWallDesign.WallDesignDetail>();
        List<Dictionary<string, string>> tableInListPierDesignResultGlobal = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> tableInListSpandrelDesignResultGlobal = new List<Dictionary<string, string>>();
        List<double> uniqueXGlobal = new List<double>();
        List<double> uniqueYGlobal = new List<double>();
        double countPassGlobal = 0;
        double countFailGlobal = 0;

        int titleSizeGlobal = 0;
        int axisTitleSizeGlobal = 0;
        int tickLabelSizeGlobal = 0;

        string choosenGroupGlobal;

        bool? extractEtabsGlobal;
        bool? extractExcelGlobal;

        bool firstRunGlobal = true;

        public WindowWallDesignResult(List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList, List<double> uniqueX, List<double> uniqueY)
        {
            titleSizeGlobal = titleSize;
            axisTitleSizeGlobal = axisTitleSize;
            tickLabelSizeGlobal = tickLabelSize;

            tableInListPierDesignResultGlobal = tableInListPierDesignResult;
            tableInListSpandrelDesignResultGlobal = tableInListSpandrelDesignResult;

            extractEtabsGlobal = extractEtabs;
            extractExcelGlobal = extractExcel;

            InitializeComponent();

            //Fill in combobox for available groups
            //src.HelperUI.PopulateComboBoxAvailGroup(_SapModel, comBoxGroup);
            comBoxGroup.ItemsSource = listChoosenGroup;
            comBoxGroup.SelectedIndex = 0;

            //Fill in combobox for type of failure
            comBoxTypeOfFailure.ItemsSource = src.HelperEtabs.typeOfFailureList;
            comBoxTypeOfFailure.SelectedIndex = 0;

            string choosenGroup = comBoxGroup.SelectedValue.ToString();
            string typeOfFailure = comBoxTypeOfFailure.SelectedValue.ToString();

            bool isError = false;

            firstRunGlobal = false;

            (isError, wallDesignDetailListGlobal, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal, countPassGlobal, countFailGlobal) = FunctionWallDesignView.PlotWallDesignResult(ResultPlot, databaseTableList,
                listChoosenGroup, choosenGroup, typeOfFailure, tableInListPierDesignResult, tableInListSpandrelDesignResult, titleSize, axisTitleSize,
                tickLabelSize, extractEtabs, extractExcel, wallDesignDetailList, uniqueX, uniqueY);

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

            string typeOfFailure = comBoxTypeOfFailure.SelectedValue.ToString();

            FunctionWallDesignView.FilterWallDesignResult(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal,
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, countPassGlobal, countFailGlobal);
        }

        private void comBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!firstRunGlobal)
            {
                bool? showOk = checkBoxShowOk.IsChecked;
                bool? showFail = checkBoxShowFail.IsChecked;
                bool? showNull = checkBoxShowNull.IsChecked;

                string choosenGroup = comBoxGroup.SelectedItem.ToString();

                string typeOfFailure = comBoxTypeOfFailure.SelectedItem.ToString();

                wallDesignDetailFilteredListGlobal = FunctionWallDesignView.FilterWallDesignResultChangeGroup(ResultPlot, showOk, showFail, showNull, wallDesignDetailListGlobal,
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, choosenGroup, typeOfFailure);
            }
            
        }

        private void comBoxTypeOfFailure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!firstRunGlobal)
            {
                bool? showOk = checkBoxShowOk.IsChecked;
                bool? showFail = checkBoxShowFail.IsChecked;
                bool? showNull = checkBoxShowNull.IsChecked;

                string typeOfFailure = comBoxTypeOfFailure.SelectedItem.ToString();

                FunctionWallDesignView.FilterWallDesignResultChangeTypeOfFailure(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal,
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, tableInListPierDesignResultGlobal, tableInListSpandrelDesignResultGlobal,
                extractEtabsGlobal, extractExcelGlobal);
            }
        }
    }
}
