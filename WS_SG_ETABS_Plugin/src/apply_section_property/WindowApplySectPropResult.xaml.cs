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

namespace WS_SG_ETABS_Plugin.src.apply_section_property
{
    /// <summary>
    /// Interaction logic for WindowApplySectPropResult.xaml
    /// </summary>
    public partial class WindowApplySectPropResult : Window
    {
        public WindowApplySectPropResult(cSapModel SapModel, cPluginCallback Plugin, List<ClassApplySectProp.ApplySectPropDefinition> applySectPropDefinitionList)
        {
            InitializeComponent();

            bool isError = FunctionApplySectProp.ApplySectProp(SapModel, Plugin, applySectPropDefinitionList);

            if (isError)
            {
                return;
            }
            else
            {
                ShowDialog();
            }
            
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
