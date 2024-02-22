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
        public WindowWallDesignResult(List<src.HelperEtabs.DatabaseTable> databaseTableList, string choosenGroup, string typeOfFailure,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult)
        {
            InitializeComponent();

            bool isError = FunctionWallDesignView.PlotWallDesignResult(ResultPlot, databaseTableList, choosenGroup, typeOfFailure, tableInListPierDesignResult, tableInListSpandrelDesignResult);
            if (isError) { return; }

            ShowDialog();
        }
    }
}
