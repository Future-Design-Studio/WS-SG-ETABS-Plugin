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
using SAFEv1;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_safe
{
    /// <summary>
    /// Interaction logic for WindowEtabsSafeOpenFileLoading.xaml
    /// </summary>
    public partial class WindowEtabsSafeOpenFileLoading : Window
    {
        public WindowEtabsSafeOpenFileLoading(ETABSv1.cSapModel SapModelEtabs, ETABSv1.cPluginCallback PluginEtabs, string filePathFrom)
        {


            SAFEv1.cSapModel SapModelSAFE;
            SAFEv1.cOAPI mySAFEObject;
            bool isError = false;
            List<string> tableKeysToSave = new List<string>();
            List<src.HelperEtabs.DatabaseTable> databaseTableListETABS = new List<src.HelperEtabs.DatabaseTable>();

            //Opening model
            (isError, SapModelSAFE, mySAFEObject) = src.HelperSafe.OpenModel(filePathFrom);

            if (isError)
            {
                MessageBox.Show("Unable to open file! Please recheck path!");
                return;
            }

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
            //Set unit in SafeTo
            src.HelperSafe.SetOutputUnit(SapModelSAFE, forceUnitSAFE, lengthUnitSAFE, temperatureUnitSAFE);

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
            src.HelperEtabs.OnAllLoadDatabase(SapModelEtabs);

            InitializeComponent();

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Reactions", "Group Assignments", "Group Definitions" };
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
                        MessageBox.Show("Error in retreiving joint reactions and groupings. Please make sure the model is run!");
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
                        else if (tableKey == "Group Definitions")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxGroupDefinition.IsChecked = true;
                            }));
                        }
                        databaseTableListETABS.Add(databaseTable);
                    }
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
