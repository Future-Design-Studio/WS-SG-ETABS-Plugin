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
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    /// <summary>
    /// Interaction logic for WindowPunchShrPlotLoading.xaml
    /// </summary>
    public partial class WindowPunchShrPlotLoading : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<src.HelperEtabs.DatabaseTable>();

        public WindowPunchShrPlotLoading(cSapModel SapModel, cPluginCallback Plugin)
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
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Reactions", "Group Assignments", "Element Forces - Columns", "Column Object Connectivity", "Frame Assignments - Section Properties" };
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
                        MessageBox.Show("Error in retreiving column forces and joint reactions. Please make sure the model is run!");
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
                        else if (tableKey == "Element Forces - Columns")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxElementForcesColumns.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Column Object Connectivity")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxColumnObjectConnectivity.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Frame Assignments - Section Properties")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxFrameAssignmentsSectionProperties.IsChecked = true;
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
                src.punching_shear_plot.WindowPunchShrPlotMain windowPunchShrPlotMain = new src.punching_shear_plot.WindowPunchShrPlotMain(SapModel, Plugin, databaseTableList);
            }

        }
    }
}
