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

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    /// <summary>
    /// Interaction logic for WindowPrecastRCColumnResult.xaml
    /// </summary>
    public partial class WindowPrecastRCColumnResult : Window
    {
        public WindowPrecastRCColumnResult(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList)
        {

            //Main logic
            bool isError = src.tedds_main.precast_rc_column.FunctionPrecastRCColumn.PrecastRCColumn(SapModel, Plugin, DatabaseTableListETABS, groupList);

            if (isError)
            {
                return;
            }

            InitializeComponent();
            ShowDialog();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
