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

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    /// <summary>
    /// Interaction logic for WindowJointForceExtractorLoading.xaml
    /// </summary>
    public partial class WindowJointForceExtractorLoading : Window
    {
        public WindowJointForceExtractorLoading(cSapModel SapModel, cPluginCallback Plugin)
        {

            //Check if model is locked
            bool modelToLocked = SapModel.GetModelIsLocked();

            if (!modelToLocked)
            {
                MessageBox.Show("ETABS file is unlocked! Please run analysis first!");
                return;
            }

            //Set unit in Etabs
            ETABSv1.eForce forceUnit = ETABSv1.eForce.kN;
            ETABSv1.eLength lengthUnit = ETABSv1.eLength.m;
            ETABSv1.eTemperature temperatureUnit = ETABSv1.eTemperature.C;

            //Set unit in Etabs
            src.HelperEtabs.SetOutputUnit(SapModel, forceUnit, lengthUnit, temperatureUnit);

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModel);

            List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<HelperEtabs.DatabaseTable>();

            InitializeComponent();

            //Get database table
            List<string> tableKeys = new List<string> {"Group Assignments", "Element Joint Forces - Frame"};
            loadDatabaseTable(SapModel, Plugin, tableKeys, databaseTableList);

            ShowDialog();
        }
        private async void loadDatabaseTable(ETABSv1.cSapModel SapModel, ETABSv1.cPluginCallback Plugin, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModel, tableKey, false);

                    if (isError)
                    {
                        MessageBox.Show("Error in retreiving table for " + tableKey + ". It is possible this table doesn't exist or model is not run, please double check.");
                        isError = true;
                    }
                    //Update loading UI
                    if (tableKey == "Group Assignments")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxGroupAssignment.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Joint Forces - Frame")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxJointForcesFrames.IsChecked = true;
                        }));
                    }
                    databaseTableList.Add(databaseTable);
                }
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.joint_forces_extractor.WindowJointForcesExtractorMain windowJointForcesExtractorMain = new src.joint_forces_extractor.WindowJointForcesExtractorMain(SapModel, Plugin, databaseTableList);
            }

        }
    }
}
