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

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    /// <summary>
    /// Interaction logic for WindowPunchShrPlotResult.xaml
    /// </summary>
    public partial class WindowPunchShrPlotResult : Window
    {
        public WindowPunchShrPlotResult(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, string plotTitle, 
            double bubbleScaleFactor, double textSize, double titleSize, double axisTitleSize, double gridSpacingX, double gridSpacingY)
        {
            InitializeComponent();
        }
    }
}
