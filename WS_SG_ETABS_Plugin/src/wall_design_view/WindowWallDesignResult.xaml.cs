using Microsoft.ML.Data;
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

        public WindowWallDesignResult(List<string> listChoosenGroup,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel, List<ClassWallDesign.WallDesignDetail> wallDesignDetailList)
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
            comBoxViewType.ItemsSource = src.HelperEtabs.typeOfViewWallDesign;
            comBoxViewType.SelectedIndex = 0;

            string choosenGroup = comBoxGroup.SelectedValue.ToString();
            string viewType = comBoxViewType.SelectedValue.ToString();

            textBoxMinReinf.IsEnabled = false;
            textBoxMaxReinf.IsEnabled = false;

            bool isError = false;

            firstRunGlobal = false;

            bool? showOk = checkBoxShowOk.IsChecked;
            bool? showFail = checkBoxShowFail.IsChecked;
            bool? showNull = checkBoxShowNull.IsChecked;

            double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
            double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

            (isError, wallDesignDetailListGlobal, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal, countPassGlobal, countFailGlobal) = FunctionWallDesignView.PlotWallDesignResult(
                ResultPlot, PlanViewPlot, choosenGroup, viewType, titleSize, axisTitleSize,
                tickLabelSize, wallDesignDetailList, showOk, showFail, showNull, minReinfPercent, maxReinfPercent);

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


        private void textBoxMinReinf_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateReinfPercent();
        }

        private void textBoxMaxReinf_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateReinfPercent();
        }

        private void checkBoxShowPier_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlotBasedOnCheckBoxWallType();
        }

        private void checkBoxShowSpandrel_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlotBasedOnCheckBoxWallType();
        }

        private void UpdatePlotBasedOnCheckBox()
        {

            bool? showOk = checkBoxShowOk.IsChecked;
            bool? showFail = checkBoxShowFail.IsChecked;
            bool? showNull = checkBoxShowNull.IsChecked;

            bool? showPier = checkBoxShowPier.IsChecked;
            bool? showSpandrel = checkBoxShowSpandrel.IsChecked;

            string typeOfFailure = comBoxViewType.SelectedValue.ToString();

            double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
            double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

            FunctionWallDesignView.FilterWallDesignResultChangeViewType(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal,
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, minReinfPercent, maxReinfPercent, showPier, showSpandrel);

        }

        private void UpdatePlotBasedOnCheckBoxWallType()
        {
            bool? showOk = checkBoxShowOk.IsChecked;
            bool? showFail = checkBoxShowFail.IsChecked;
            bool? showNull = checkBoxShowNull.IsChecked;

            bool? showPier = checkBoxShowPier.IsChecked;
            bool? showSpandrel = checkBoxShowSpandrel.IsChecked;

            string typeOfFailure = comBoxViewType.SelectedValue.ToString();

            double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
            double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

            FunctionWallDesignView.FilterWallDesignResultChangeViewType(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, 
                                                    uniqueXGlobal, uniqueYGlobal,
                                                    titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, minReinfPercent, maxReinfPercent, showPier, showSpandrel);

        }

        private void UpdateReinfPercent()
        {
            if (!firstRunGlobal)
            {

                if (textBoxMinReinf.Text != "" && textBoxMaxReinf.Text != "")
                {
                    bool? showOk = checkBoxShowOk.IsChecked;
                    bool? showFail = checkBoxShowFail.IsChecked;
                    bool? showNull = checkBoxShowNull.IsChecked;

                    bool? showPier = checkBoxShowPier.IsChecked;
                    bool? showSpandrel = checkBoxShowSpandrel.IsChecked;

                    double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
                    double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

                    string typeOfFailure = comBoxViewType.SelectedItem.ToString();

                    if (minReinfPercent < maxReinfPercent)
                    {
                        FunctionWallDesignView.FilterWallDesignResultChangeViewType(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal,
                            titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, minReinfPercent, maxReinfPercent, showPier, showSpandrel);
                    }
                    else
                    {
                        MessageBox.Show("Min reinforcement must be smaller than max reinforcement!");
                        textBoxMinReinf.Text = Convert.ToString(Convert.ToDouble(textBoxMaxReinf.Text) - 0.1);
                    }

                }


            }
        }

        private void comBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!firstRunGlobal)
            {
                bool? showOk = checkBoxShowOk.IsChecked;
                bool? showFail = checkBoxShowFail.IsChecked;
                bool? showNull = checkBoxShowNull.IsChecked;

                string choosenGroup = comBoxGroup.SelectedItem.ToString();

                bool? showPier = checkBoxShowPier.IsChecked;
                bool? showSpandrel = checkBoxShowSpandrel.IsChecked;

                double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
                double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

                string typeOfFailure = comBoxViewType.SelectedItem.ToString();

                (wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal) = FunctionWallDesignView.FilterWallDesignResultChangeGroup(ResultPlot, PlanViewPlot, showOk, showFail, showNull, 
                    wallDesignDetailListGlobal, titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, choosenGroup, typeOfFailure, minReinfPercent, maxReinfPercent, showPier, showSpandrel);
            }
            
        }

        private void comBoxTypeOfFailure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!firstRunGlobal)
            {
                bool? showOk = checkBoxShowOk.IsChecked;
                bool? showFail = checkBoxShowFail.IsChecked;
                bool? showNull = checkBoxShowNull.IsChecked;

                bool? showPier = checkBoxShowPier.IsChecked;
                bool? showSpandrel = checkBoxShowSpandrel.IsChecked;

                double minReinfPercent = Convert.ToDouble(textBoxMinReinf.Text);
                double maxReinfPercent = Convert.ToDouble(textBoxMaxReinf.Text);

                string typeOfFailure = comBoxViewType.SelectedItem.ToString();

                FunctionWallDesignView.FilterWallDesignResultChangeViewType(ResultPlot, showOk, showFail, showNull, wallDesignDetailFilteredListGlobal, uniqueXGlobal, uniqueYGlobal,
                titleSizeGlobal, axisTitleSizeGlobal, tickLabelSizeGlobal, typeOfFailure, minReinfPercent, maxReinfPercent, showPier, showSpandrel);

                if (typeOfFailure != "Pier Req. Rebar %")
                {
                    checkBoxShowOk.IsEnabled = true;
                    checkBoxShowFail.IsEnabled = true;
                    checkBoxShowNull.IsEnabled = true;

                    textBoxMinReinf.IsEnabled = false;
                    textBoxMaxReinf.IsEnabled = false;
                }
                else
                {
                    checkBoxShowOk.IsEnabled = false;
                    checkBoxShowFail.IsEnabled = false;
                    checkBoxShowNull.IsEnabled = false;

                    textBoxMinReinf.IsEnabled = true;
                    textBoxMaxReinf.IsEnabled = true;
                }            
            }
        }

    }
}
