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

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    /// <summary>
    /// Interaction logic for WindowDriftPlotLoading.xaml
    /// </summary>
    public partial class WindowDriftPlotLoading : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<src.HelperEtabs.DatabaseTable>();

        public WindowDriftPlotLoading(cSapModel SapModel, cPluginCallback Plugin)
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
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Displacements", "Group Assignments" };
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
                        MessageBox.Show("Error in retrieving joint displacements. Please make sure the model is run!");
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
                        else if (tableKey == "Joint Displacements")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxJointDisplacements.IsChecked = true;
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
                src.plot_drift.WindowDriftPlotMain windowDriftPlotMain = new src.plot_drift.WindowDriftPlotMain(SapModel, Plugin, databaseTableList);
            }

        }
    }
}
