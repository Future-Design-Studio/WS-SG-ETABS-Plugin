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

namespace WS_SG_ETABS_Plugin.src.apply_modifier
{
    /// <summary>
    /// Interaction logic for WindowApplyModifierResult.xaml
    /// </summary>
    public partial class WindowApplyModifierResult : Window
    {
        public WindowApplyModifierResult(cSapModel _SapModel, cPluginCallback _Plugin, List<ClassApplyModifier.ModifierDefinition> modifierDefinitionList)
        {
            InitializeComponent();

            bool isError = FunctionApplyModifier.ApplyModifier(_SapModel, _Plugin, modifierDefinitionList);

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
