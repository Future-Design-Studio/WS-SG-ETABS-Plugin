using ETABSv1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace WS_SG_ETABS_Plugin.src.plot_reaction
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionMain.xaml
    /// </summary>
    public partial class WindowPlotReactionMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        //List of tables to save joint reaction and grouping table
        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<HelperEtabs.DatabaseTable>();

        public WindowPlotReactionMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();


            ShowDialog();
        }

        private void buttonPlotReaction_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            src.plot_reaction.graph.WindowPlotReactionGraphMain windowPlotReactionGraphMain = new graph.WindowPlotReactionGraphMain(_SapModel, _Plugin);
            IsEnabled = true;
        }

        private void buttonCreateExcel_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            src.plot_reaction.table.WindowPlotReactionTableMain windowPlotReactionTableMain = new table.WindowPlotReactionTableMain(_SapModel, _Plugin);
            IsEnabled = true;
        }
    }
}
