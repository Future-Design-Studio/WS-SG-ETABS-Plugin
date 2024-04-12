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
        List<string> ListChoosenLoadCase = new List<string>();
        List<string> ListChoosenGroup = new List<string>();
        string FilePath;
        string FileName;
        public WindowJointForceExtractorLoading(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, List<string> listChoosenGroup, string filePath, string fileName)
        {
            ListChoosenLoadCase = listChoosenLoadCase;
            ListChoosenGroup = listChoosenGroup;
            FileName = fileName;
            FilePath = filePath;

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

            ///Turn off all load case and combo displayed
            HelperEtabs.OffAllLoadCaseAndComboDisplayedDatabase(SapModel);

            //Turn on load case and combo based on list
            src.HelperEtabs.OnLoadCaseAndComboDatabaseBasedOnList(SapModel, ListChoosenLoadCase);

            List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<HelperEtabs.DatabaseTable>();

            InitializeComponent();

            //Get database table
            List<string> tableKeys = new List<string> {"Group Assignments", "Beam Object Connectivity", "Column Object Connectivity", "Brace Object Connectivity", "Element Forces - Columns", "Element Forces - Beams", "Element Forces - Braces"};
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
                    else if (tableKey == "Beam Object Connectivity")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxBeamConnectivity.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Column Object Connectivity")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxColumnConnectivity.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Brace Object Connectivity")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxBraceConnectivity.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Forces - Columns")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxColumnForces.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Forces - Beams")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxBeamForces.IsChecked = true;
                        }));
                    }
                    else if (tableKey == "Element Forces - Braces")
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxBraceForces.IsChecked = true;
                        }));
                    }
                    databaseTableList.Add(databaseTable);
                }
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.joint_forces_extractor.WindowJointForcesExtractorResult windowJointForcesExtractorResult = new src.joint_forces_extractor.WindowJointForcesExtractorResult(SapModel, Plugin, databaseTableList, ListChoosenLoadCase, ListChoosenGroup, FilePath, FileName);
            }

        }
    }
}
