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

namespace WS_SG_ETABS_Plugin.src.plot_reaction.graph
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionGraphBaseReaction.xaml
    /// </summary>
    public partial class WindowPlotReactionGraphBaseReaction : Window
    {
        public WindowPlotReactionGraphBaseReaction(double baseReaction, double totalReactionInPlot)
        {
            InitializeComponent();

            string baseReactionFormatted = $"{baseReaction:n0}";
            string totalReactionInPlotFormatted = $"{totalReactionInPlot:n0}";
            string deltaFormatted = $"{baseReaction - totalReactionInPlot:n0}";

            labelBaseReaction.Content = baseReactionFormatted;
            labelReactionInPlot.Content = totalReactionInPlotFormatted;

            labelDeltaReaction.Content = deltaFormatted;

            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
