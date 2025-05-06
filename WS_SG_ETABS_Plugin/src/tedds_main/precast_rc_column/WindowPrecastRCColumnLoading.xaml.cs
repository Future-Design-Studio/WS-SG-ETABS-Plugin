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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;
//using ScottPlot.Drawing.Colormaps;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    /// <summary>
    /// Interaction logic for WindowPrecastRCColumnLoading.xaml
    /// </summary>
    public partial class WindowPrecastRCColumnLoading : Window
    {

        private static int Ret;
        public WindowPrecastRCColumnLoading(cSapModel SapModelEtabs, cPluginCallback PluginEtabs)
        {

            bool isError = false;

            List<string> tableKeysToSave = new List<string>();
            List<src.HelperEtabs.DatabaseTable> databaseTableListETABS = new List<src.HelperEtabs.DatabaseTable>();


            //Set unit in EtabsFrom
            ETABSv1.eForce forceUnitETABS = ETABSv1.eForce.kN;
            ETABSv1.eLength lengthUnitETABS = ETABSv1.eLength.m;
            ETABSv1.eTemperature temperatureUnitETABS = ETABSv1.eTemperature.C;

            //Set unit in EtabsFrom
            src.HelperEtabs.SetOutputUnit(SapModelEtabs, forceUnitETABS, lengthUnitETABS, temperatureUnitETABS);

            //Turn on all load
            src.HelperEtabs.OnAllLoadDatabase(SapModelEtabs);

            InitializeComponent();

            string designCode = "";
            Ret = SapModelEtabs.DesignConcrete.GetCode(ref designCode);

            //Run RC design if result is not available
            bool resultsAvailable = SapModelEtabs.DesignConcrete.GetResultsAvailable();
            if (!resultsAvailable)
            {
                Ret = SapModelEtabs.DesignConcrete.StartDesign();
                if (Ret == 1)
                {
                    isError = true;
                    MessageBox.Show("Cannot start RC design. Please run FEM analysis first!");
                    return;
                }
            }

            //Get joint reaction database table
            List<string> tableKeys = new List<string> { "Column Object Connectivity", "Concrete Column Overwrites - " + designCode, "Concrete Column PMM Envelope - " + designCode, "Material Properties - Concrete Data", "Frame Assignments - Section Properties", "Frame Section Property Definitions - Concrete Circle", "Frame Section Property Definitions - Concrete Rectangular", "Group Assignments" };
            loadDatabaseTable(SapModelEtabs, PluginEtabs, tableKeys, databaseTableListETABS);

            ShowDialog();
        }

        private async void loadDatabaseTable(cSapModel SapModelEtabs, cPluginCallback PluginEtabs, List<string> tableKeys, List<src.HelperEtabs.DatabaseTable> databaseTableListETABS)
        {
            bool isError = false;

            await Task.Run(() => {

                foreach (string tableKey in tableKeys)
                {
                    src.HelperEtabs.DatabaseTable databaseTable = new src.HelperEtabs.DatabaseTable();

                    (isError, databaseTable) = src.HelperEtabs.GetTableDisplay(SapModelEtabs, tableKey, false);

                    if (isError && (tableKey == tableKeys[5] || tableKey == tableKeys[6] || tableKey == tableKeys[1]))
                    {
                        MessageBox.Show("Error in retreiving table for " + tableKey + ". It is possible this table doesn't exist or model is not run.");
                        isError = false;
                        continue;
                    }
                    else if (isError)
                    {
                        MessageBox.Show("Error in retreiving table for " + tableKey + ". It is possible this table doesn't exist or model is not run. Please recheck.");
                        isError = true;
                        return;
                    }
                    //Update loading UI
                    if (tableKey == tableKeys[0])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxColumnObjectConnectivity.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[1])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxConcreteColumnOverwrites.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[2])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxSConcreteColumnPMMEnvelope.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[3])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxMaterialProperties.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[4])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxFrameAssignmentsSectionProperties.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[5])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxFrameSectionPropertyDefinitionsConcreteCircle.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[6])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxFrameSectionPropertyDefinitionsConcreteRectangular.IsChecked = true;
                        }));
                    }
                    else if (tableKey == tableKeys[7])
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            checkBoxGroupAssignments.IsChecked = true;
                        }));
                    }
                    databaseTableListETABS.Add(databaseTable);
                }
            });

            Close();

            if (!isError)
            {
                //Call up window
                src.tedds_main.precast_rc_column.WindowPrecastRCColumnMain windowPrecastRCColumnMain = new src.tedds_main.precast_rc_column.WindowPrecastRCColumnMain(SapModelEtabs, PluginEtabs, databaseTableListETABS);
            }

        }
    }
}
