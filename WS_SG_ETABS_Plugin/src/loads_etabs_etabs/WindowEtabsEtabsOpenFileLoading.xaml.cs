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

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    /// <summary>
    /// Interaction logic for WindowEtabsEtabsOpenFileLoading.xaml
    /// </summary>
    public partial class WindowEtabsEtabsOpenFileLoading : Window
    {

        cSapModel SapModelFrom;
        cOAPI myETABSObjectFrom;
        bool isError = false;
        List<string> tableKeysToSave = new List<string>();
        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<src.HelperEtabs.DatabaseTable>();

        public WindowEtabsEtabsOpenFileLoading(cSapModel SapModel, cPluginCallback Plugin, string filePathFrom)
        {
            

            //Opening model
            (isError, SapModelFrom, myETABSObjectFrom) = src.HelperEtabs.OpenModel(filePathFrom);

            if (isError)
            {
                MessageBox.Show("Unable to open file! Please recheck path!");
                return;
            }

            //Get unit in EtabsTo
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();

            (forceUnit, lengthUnit, temperatureUnit) = src.HelperEtabs.GetPresentUnit(SapModel);

            //Set unit in EtabsFrom
            src.HelperEtabs.SetOutputUnit(SapModelFrom, forceUnit, lengthUnit, temperatureUnit);

            //Check if modelFrom is locked
            bool modelFromLocked = SapModelFrom.GetModelIsLocked();

            if (!modelFromLocked)
            {
                myETABSObjectFrom.ApplicationExit(false);
                MessageBox.Show("Reference ETABS file is unlocked! Please run analysis first!");
                isError = true;
                return;
            }

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModel);

            InitializeComponent();

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Reactions", "Group Assignments", "Group Definitions" };
            loadDatabaseTable(SapModelFrom, SapModel, Plugin, tableKeys, databaseTableList);

            ShowDialog();
        }

        private async void loadDatabaseTable(cSapModel SapModelFrom, cSapModel SapModel, cPluginCallback Plugin, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModelFrom, tableKey, false);

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
                        databaseTableList.Add(databaseTable);
                    }
                }
                myETABSObjectFrom.ApplicationExit(false);
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.loads_etabs_etabs.WindowEtabsEtabsMain windowEtabsEtabsMain = new src.loads_etabs_etabs.WindowEtabsEtabsMain(SapModel, Plugin, databaseTableList);
            }

        }
    }
}
