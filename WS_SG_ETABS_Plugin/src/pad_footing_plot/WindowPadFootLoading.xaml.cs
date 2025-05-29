using ETABSv1;
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

namespace WS_SG_ETABS_Plugin.src.pad_footing_plot
{
    /// <summary>
    /// Interaction logic for WindowPadFootLoading.xaml
    /// </summary>
    public partial class WindowPadFootLoading : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<src.HelperEtabs.DatabaseTable>();

        public WindowPadFootLoading(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            //Check if model is locked
            bool modelToLocked = _SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }

            InitializeComponent();

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModel);

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Reactions", "Group Assignments"};
            loadDatabaseTable(SapModel, Plugin, tableKeys, databaseTableList);

            ShowDialog();
        }

        private async void loadDatabaseTable(cSapModel SapModel, cPluginCallback Plugin, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModel, tableKey, false);

                    if (isError)
                    {
                        MessageBox.Show("Error in retreiving joint reactions. Please make sure the model is run!");
                        return;
                    }
                    else
                    {
                        //Update loading UI
                        if (tableKey == "Point Object Connectivity")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxPointObjectConnectivity.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Joint Reactions")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxJointReaction.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Group Assignments")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxGroupAssignments.IsChecked = true;
                            }));
                        }

                        databaseTableList.Add(databaseTable);
                    }
                }
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.pad_footing_plot.WindowPadFootPlotMain windowPadFootPlotMain = new src.pad_footing_plot.WindowPadFootPlotMain(SapModel, Plugin, databaseTableList);
            }

        }
    }
}
