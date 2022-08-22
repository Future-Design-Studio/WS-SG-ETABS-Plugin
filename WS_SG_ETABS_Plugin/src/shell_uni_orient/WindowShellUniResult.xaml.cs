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

namespace WS_SG_ETABS_Plugin.src.shell_uni_orient
{
    /// <summary>
    /// Interaction logic for ShellUniResult.xaml
    /// </summary>
    public partial class WindowShellUniResult : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowShellUniResult(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenGroup, string shellDir)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if any group is choosen for shell uniform orientation
            bool requiredGroupListExist = src.HelperUI.CheckGroupListExistCarbon(_SapModel, listChoosenGroup);

            if (!requiredGroupListExist)
            {
                return;
            }

            //Main logic
            bool isError = src.shell_uni_orient.FunctionShellUni.MakeShellOrientationUniform(_SapModel, listChoosenGroup, shellDir);

            if (isError)
            {
                return;
            }

            InitializeComponent();

            ShowDialog();

        }
    }
}
