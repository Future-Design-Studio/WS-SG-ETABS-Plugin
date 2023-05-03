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
    /// Interaction logic for WindowPrecastRCColumnLoading.xaml
    /// </summary>
    public partial class WindowPrecastRCColumnLoading : Window
    {
        public WindowPrecastRCColumnLoading(cSapModel SapModelEtabs, cPluginCallback PluginEtabs)
        {
            bool isError = false;

            List<string> tableKeysToSave = new List<string>();
            List<src.HelperEtabs.DatabaseTable> databaseTableListETABS = new List<src.HelperEtabs.DatabaseTable>();


            //Set unit in EtabsFrom
            ETABSv1.eForce forceUnitETABS = ETABSv1.eForce.kN;
            ETABSv1.eLength lengthUnitETABS = ETABSv1.eLength.m;
            ETABSv1.eTemperature temperatureUnitETABS = ETABSv1.eTemperature.C;

            //Set unit in SAFETo
            SAFEv1.eForce forceUnitSAFE = SAFEv1.eForce.kN;
            SAFEv1.eLength lengthUnitSAFE = SAFEv1.eLength.m;
            SAFEv1.eTemperature temperatureUnitSAFE = SAFEv1.eTemperature.C;

            //Set unit in EtabsFrom
            src.HelperEtabs.SetOutputUnit(SapModelEtabs, forceUnitETABS, lengthUnitETABS, temperatureUnitETABS);

            //Check if modelFrom is locked
            bool modelFromLocked = SapModelEtabs.GetModelIsLocked();

            if (!modelFromLocked)
            {
                mySAFEObject.ApplicationExit(false);
                MessageBox.Show("SAFE file is locked! Please unlock the model!");
                isError = true;
                return;
            }

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabaseWithoutCombinations(SapModelEtabs);

            InitializeComponent();

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Reactions", "Story Definitions", "Element Joint Forces - Frame", "Element Joint Forces - Shells", "Element Joint Forces - Links" };
            loadDatabaseTable(SapModelEtabs, SapModelSAFE, PluginEtabs, tableKeys, databaseTableListETABS);

            ShowDialog();
        }

        private async void loadDatabaseTable(ETABSv1.cSapModel SapModelEtabs, SAFEv1.cSapModel SapModelSAFE, ETABSv1.cPluginCallback PluginEtabs, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModelEtabs, tableKey, false);

                    if (isError)
                    {
                        MessageBox.Show("Error in retreiving table for " + tableKey + ". It is possible this table doesn't exist or model is not run, please double check.");
                        isError = false;
                    }
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
                    else if (tableKey == "Story Definitions")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxStorydefin.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Joint Forces - Frame")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxJointForcesFrame.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Joint Forces - Shells")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxJointForcesShell.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Joint Forces - Links")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxJointForcesLink.IsChecked = true;
                        }));
                    }
                    databaseTableListETABS.Add(databaseTable);
                }
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.loads_etabs_safe.WindowEtabsSafeMain windowEtabsSafeMain = new src.loads_etabs_safe.WindowEtabsSafeMain(SapModelEtabs, SapModelSAFE, PluginEtabs, databaseTableListETABS);
            }

        }
    }
}
