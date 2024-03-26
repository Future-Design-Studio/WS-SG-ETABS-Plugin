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
using WS_SG_ETABS_Plugin.src.plot_reaction;
using ETABSv1;
using ScottPlot.Drawing.Colormaps;

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    /// <summary>
    /// Interaction logic for WindowWallDesignLoading.xaml
    /// </summary>
    public partial class WindowWallDesignLoading : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        List<src.HelperEtabs.DatabaseTable> databaseTableList = new List<src.HelperEtabs.DatabaseTable>();

        bool? extractEtabsGlobal;
        bool? extractExcelGlobal;

        public WindowWallDesignLoading(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenGroup, bool? extractEtabs, bool? extractExcel, string excelFilePath)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;
            bool Ret;

            extractEtabsGlobal = extractEtabs;
            extractExcelGlobal = extractExcel;

            InitializeComponent();

            //Get currently showed load
            List<string> loadCaseDisplayed = new List<string>();
            List<string> loadCombinationDisplayed = new List<string>();
            (loadCaseDisplayed, loadCombinationDisplayed) = HelperEtabs.GetAllLoadCaseAndComboDisplayedDatabase(SapModel);

            List<string> originallyDisplayedLoadCaseAndComboList = new List<string>();
            originallyDisplayedLoadCaseAndComboList.AddRange(loadCaseDisplayed);
            originallyDisplayedLoadCaseAndComboList.AddRange(loadCombinationDisplayed);

            /*
            //Turn on all load pattern
            HelperEtabs.OnAllLoadPatternDatabase(SapModel);

            ///Turn off all load case and combo displayed
            HelperEtabs.OffAllLoadCaseAndComboDisplayedDatabase(SapModel);

            //Turn on selected load case and combo displayed
            HelperEtabs.OnLoadCaseAndComboDatabaseBasedOnList(SapModel, listChoosenLoadCase);
            */


            //Get database table
            List<string> tableKeys = new List<string> { "Wall Object Connectivity", "Group Assignments", "Area Assignments - Pier Labels", "Area Assignments - Spandrel Labels", "Point Object Connectivity" };

            //Combine into 1 group for faster database loading
            List<string> combinedUniqueNameList = new List<string>();
            foreach (string choosenGroup in listChoosenGroup)
            {
                List<string> uniqueNameList = HelperEtabs.GetGroupAreaElement(SapModel, choosenGroup);
                combinedUniqueNameList.AddRange(uniqueNameList);
            }

            //Create temporary group for all choosen group
            bool isError = HelperEtabs.CreateNewGroup(SapModel, "~temp");
            if (isError) { return; }
            isError = HelperEtabs.AddAreaInsideGroup(SapModel, combinedUniqueNameList, "~temp");
            if (isError) { return; }

            List<string> tempListChoosenGroup = new List<string>();
            tempListChoosenGroup.Add("~temp");

            //Use all load case when loading database
            List<string> listChoosenLoadCase = src.HelperEtabs.GetLoadCaseAndComboList(SapModel);

            loadDatabaseTable(SapModel, Plugin, tableKeys, databaseTableList, listChoosenGroup, tempListChoosenGroup, excelFilePath, extractEtabs, extractExcel);

            ShowDialog();
        }

        private static (List<Dictionary<string, string>>, List<Dictionary<string, string>>) GetWallDesignResultFromExcel(string excelFilePath)
        {
            List<Dictionary<string, string>> tableInListPierDesignResult = null;
            List<Dictionary<string, string>> tableInListSpandrelDesignResult = null;
            List<string> columnKeys = null;
            List<string> tableInList = null;

            List<string> excelOutput;
            bool isError = false;

            //Get pier design
            (isError, excelOutput) = HelperExcel.ReadExcel(excelFilePath, "Pier Dgn Sum", false);

            if (!isError)
            {
                columnKeys = excelOutput.GetRange(29, 29);
                tableInList = excelOutput.GetRange(87, excelOutput.Count - 87);

                tableInListPierDesignResult = HelperMethod.EtabsTableToDictionary(tableInList.ToArray(), columnKeys.ToArray());
            }

            //Get spandrel design
            (isError, excelOutput) = HelperExcel.ReadExcel(excelFilePath, "Span Dgn Sum", false);

            if (!isError)
            {
                columnKeys = excelOutput.GetRange(21, 21);
                tableInList = excelOutput.GetRange(63, excelOutput.Count - 63);

                tableInListSpandrelDesignResult = HelperMethod.EtabsTableToDictionary(tableInList.ToArray(), columnKeys.ToArray());
            }

            return (tableInListPierDesignResult, tableInListSpandrelDesignResult);
        }

        private async void loadDatabaseTable(cSapModel SapModel, cPluginCallback Plugin, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableList, 
            List<string> listChoosenGroup, List<string> tempListChoosenGroup, string excelFilePath, bool? extractEtabs, bool? extractExcel)
        {
            bool isError = false;
            int ret;

            List<Dictionary<string, string>> tableInListPierDesignResult = null;
            List<Dictionary<string, string>> tableInListSpandrelDesignResult = null;

            await Task.Run(() => {

                //Get wall design result

                if (extractEtabs == true)
                {
                    (tableInListPierDesignResult, tableInListSpandrelDesignResult) = HelperEtabs.GetWallDesignResult(SapModel);

                    if (tableInListPierDesignResult == null && tableInListSpandrelDesignResult == null)
                    {
                        isError = true;
                    }
                }
                else if (extractExcel == true)
                {
                    (tableInListPierDesignResult, tableInListSpandrelDesignResult) = GetWallDesignResultFromExcel(excelFilePath);

                    if (tableInListPierDesignResult == null && tableInListSpandrelDesignResult == null)
                    {

                        isError = true;
                    }
                }

                if (isError)
                {
                    MessageBox.Show("There is no wall design result! Please wall design!");
                    return;
                }
                else
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        checkBoxWallDesign.IsChecked = true;
                    }));
                }

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    //Extracting wall object connectivity or group assignments
                    if (tableKey == "Wall Object Connectivity" || tableKey == "Group Assignments" || tableKey == "Area Assignments - Pier Labels" || tableKey == "Area Assignments - Spandrel Labels")
                    {
                        (isError, databaseTable) = src.HelperEtabs.GetTableDisplaySpecificGroup(SapModel, tableKey, tempListChoosenGroup, false);
                    }
                    else if (tableKey == "Point Object Connectivity")
                    {
                        //Select points which is connected to the wall
                        //Delete temp group
                        ret = SapModel.GroupDef.Delete("~temp");
                        if (ret != 0) { isError = true; }

                        //Extracting point object connectivity only which are connected to the wall
                        src.HelperEtabs.DatabaseTable wallObjectConnectivityTable = new HelperEtabs.DatabaseTable();
                        (isError, wallObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Wall Object Connectivity");
                        if (isError) { return; }

                        List<HelperEtabs.Shell> shellList = HelperEtabs.GetShellsPoints(wallObjectConnectivityTable);

                        List<string> pointConnectedToShellList = new List<string>();
                        foreach (HelperEtabs.Shell shell in shellList)
                        {
                            foreach (string pointUniqueName in shell.Points)
                            {
                                pointConnectedToShellList.Add(pointUniqueName);
                            }
                        }

                        isError = HelperEtabs.CreateNewGroup(SapModel, "~temp");
                        if (isError) { return; }
                        isError = HelperEtabs.AddPointsInsideGroup(SapModel, pointConnectedToShellList, "~temp");
                        if (isError) { return; }

                        (isError, databaseTable) = src.HelperEtabs.GetTableDisplaySpecificGroup(SapModel, tableKey, tempListChoosenGroup, false);
                    }

                    //These 2 tables can be empty in case there is no spandrel or pier label
                    if (isError && (tableKey == "Area Assignments - Pier Labels" || tableKey == "Area Assignments - Spandrel Labels"))
                    {
                        isError = false;
                        databaseTable = new src.HelperEtabs.DatabaseTable();
                        databaseTable.TableKey = tableKey;
                    }

                    if (isError)
                    {
                        if (tableKey == "Point Object Connectivity")
                        {
                            MessageBox.Show("Error in retrieving point connectivity!");
                            return;
                        }
                        else if (tableKey == "Wall Object Connectivity")
                        {
                            MessageBox.Show("Error in retrieving wall connectivity!");
                            return;
                        }
                        else if (tableKey == "Area Assignments - Pier Labels")
                        {
                            //MessageBox.Show("Error in retrieving wall connectivity!");
                            //return;
                        }
                        else if (tableKey == "Area Assignments - Spandrel Labels")
                        {
                            //MessageBox.Show("Error in retrieving wall connectivity!");
                            //return;
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
                        else if (tableKey == "Wall Object Connectivity")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxWallObjectConnectivity.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Area Assignments - Pier Labels")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxPierLabels.IsChecked = true;
                            }));
                        }
                        else if (tableKey == "Area Assignments - Spandrel Labels")
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                checkBoxSpandrelLabels.IsChecked = true;
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
            ret = SapModel.GroupDef.Delete("~temp");
            if (ret != 0) { isError = true; }

            if (!isError)
            {
                //Call up window
                WindowWallDesignMain windowWallDesignMain = new WindowWallDesignMain(SapModel, Plugin, databaseTableList, listChoosenGroup, tableInListPierDesignResult, tableInListSpandrelDesignResult
                    , extractEtabsGlobal, extractExcelGlobal);
            }

        }
    }
}
