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

        public WindowDriftPlotLoading(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, List<string> listChoosenGroup)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Get currently showed load
            List<string> loadCaseDisplayed = new List<string>();
            List<string> loadCombinationDisplayed = new List<string>();
            (loadCaseDisplayed, loadCombinationDisplayed) = HelperEtabs.GetAllLoadCaseAndComboDisplayedDatabase(SapModel);

            List<string> originallyDisplayedLoadCaseAndComboList = new List<string>();
            originallyDisplayedLoadCaseAndComboList.AddRange(loadCaseDisplayed);
            originallyDisplayedLoadCaseAndComboList.AddRange(loadCombinationDisplayed);

            //Turn on all load pattern
            HelperEtabs.OnAllLoadPatternDatabase(SapModel);

            ///Turn off all load case and combo displayed
            HelperEtabs.OffAllLoadCaseAndComboDisplayedDatabase(SapModel);

            //Turn on selected load case and combo displayed
            HelperEtabs.OnLoadCaseAndComboDatabaseBasedOnList(SapModel, listChoosenLoadCase);

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Point Object Connectivity", "Joint Displacements", "Group Assignments" };

            //Combine into 1 group for faster database loading
            List<string> combinedUniqueNameList = new List<string>();
            foreach (string choosenGroup in listChoosenGroup)
            {
                List<string> uniqueNameList = HelperEtabs.GetGroupJointElement(SapModel, choosenGroup);
                combinedUniqueNameList.AddRange(uniqueNameList);
            }

            //Create temporary group for all choosen group
            bool isError = HelperEtabs.CreateNewGroup(SapModel, "~temp");
            if (isError) { return;  }
            isError = HelperEtabs.AddPointsInsideGroup(SapModel, combinedUniqueNameList, "~temp");
            if (isError) { return; }

            List<string> tempListChoosenGroup = new List<string>();
            tempListChoosenGroup.Add("~temp");

            loadDatabaseTable(SapModel, Plugin, tableKeys, databaseTableList, listChoosenLoadCase, listChoosenGroup, tempListChoosenGroup, originallyDisplayedLoadCaseAndComboList);

            ShowDialog();
        }

        private async void loadDatabaseTable(cSapModel SapModel, cPluginCallback Plugin, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableList, 
            List<string> listChoosenLoadCase, List<string> listChoosenGroup, List<string> tempListChoosenGroup, List<string> originallyDisplayedLoadCaseAndComboList)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplaySpecificGroup(SapModel, tableKey, tempListChoosenGroup, false);

                    //(isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModel, tableKey, false);

                    if (isError)
                    {
                        if (tableKey == "Joint Displacements")
                        {
                            MessageBox.Show("Error in retrieving joint displacements. Please make sure the model is run!");
                            return;
                        }
                        else if (tableKey == "Group Assignments")
                        {
                            MessageBox.Show("Error in retrieving group assignments. Please make sure you have group!");
                            return;
                        }

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

            //Delete temp group
            int ret = SapModel.GroupDef.Delete("~temp");
            if (ret != 0) { isError = true; }

            if (!isError)
            {
                //Turn on back load originally
                HelperEtabs.OnLoadCaseAndComboDatabaseBasedOnList(SapModel, originallyDisplayedLoadCaseAndComboList);

                //Call up window
                src.plot_drift.WindowDriftPlotMain windowDriftPlotMain = new src.plot_drift.WindowDriftPlotMain(SapModel, Plugin, databaseTableList, listChoosenLoadCase, listChoosenGroup);
            }

        }
    }
}
