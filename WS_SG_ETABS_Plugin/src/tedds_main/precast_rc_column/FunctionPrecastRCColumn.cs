using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tekla;
using Tekla.Structural.InteropAssemblies.Tedds;
using ETABSv1;
using System.Windows.Controls;
using Tekla.Structural.InteropAssemblies.TeddsCalc;
using Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    internal class FunctionPrecastRCColumn
    {
        public static bool PrecastRCColumn(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList)
        {

            src.HelperEtabs.DatabaseTable columnobjectconnectivityTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable columnoverwriteTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable columnpmmTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable materialpropertiesTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable sectionpropertyassignmentTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable circlecolumndefinitionTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable rectanglecolumndefinitionTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable groupassignmentTable = new HelperEtabs.DatabaseTable();
            bool isError = false;

            #region Data source concrete grade

            List<string> concreteGradeList = new List<string>()
        {
            "C12/15", "C16/20", "C20/25", "C25/30", "C28/35", "C30/37", "C32/40", "C35/45", "C40/50", "C45/55", "C50/60", "C55/67", "C60/75", "C70/85", "C80/95", "C90/105"
        };

            #endregion

            #region Create folder to store all tedds documents and folderPath to store path
            //Check if have folder to store tedds files, otherwise create new folder to store
            string modelPath = SapModel.GetModelFilepath();
            string folderPath = modelPath + "TeddsFilesFromTeddsPowerAPP\\";
            bool exists = System.IO.Directory.Exists(folderPath);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            MessageBox.Show("Tedds files created from this plugin will be saved in ETABS source folder at: TeddsFilesFromTeddsPowerAPP");
            #endregion

            #region start main function

            foreach (ClassPrecastRCColumn.GroupandCoverandGradeMatch group in groupList)
            {
                if (group.Method == 0)
                {
                    #region create folder path
                    string groupfolderPath = folderPath + group.Group;
                    exists = System.IO.Directory.Exists(groupfolderPath);

                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(groupfolderPath);
                    }
                    #endregion


                    #region filter group assignment table
                    (isError, groupassignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Group Assignments");
                    if (isError)
                    {
                        MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                        return true;
                    }
                    else
                    {
                        //Filter group to only include points, to improve performance
                        (isError, groupassignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupassignmentTable, "ObjectType", "Line");
                        if (isError)
                        {
                            MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                            return true;
                        }
                        else
                        {
                            //Filter group to only include selected group, to improve performance
                            (isError, groupassignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupassignmentTable, "GroupName", group.Group);
                            if (isError)
                            {
                                MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                                return true;
                            }
                        }
                    }
                    #endregion


                    #region create list of columns within selected group
                    List<string> columnUniqueNames = new List<string>();

                    foreach (Dictionary<string,string> row in groupassignmentTable.TableInList)
                    {
                        if (!columnUniqueNames.Contains(row["UniqueName"]);
                        {
                            columnUniqueNames.Add(row["UniqueName"]);
                        }
                    }

                    #endregion





















                    #region set variables to be taken into document
                    ICalculator calculator = new Calculator();
                    calculator.Initialize();
                    calculator.Functions.SetVar("_ColumnSpliceCheckBox", 1);
                    calculator.Functions.SetVar("_rcc.BracedY", 1);
                    calculator.Functions.SetVar("_rcc.BracedZ", 1);
                    calculator.Functions.SetVar("_isLiftingCheckIncluded", 0);

                    //input from plugin main required
                    calculator.Functions.SetVar("c_{nom}", 50, "mm");
                    calculator.Functions.SetVar("_CalcUI", 0);

                    //input from etabs required
                    calculator.Functions.SetVar("_rcc.ColumnGrade", "C90/105");

                    calculator.Functions.SetVar("_rcc.Type", "Rectangular");
                    calculator.Functions.SetVar("b", 500, "mm");
                    calculator.Functions.SetVar("h", 500, "mm");

                    calculator.Functions.SetVar("l_{0y}", 500, "mm");
                    calculator.Functions.SetVar("l_{0z}", 500, "mm");

                    calculator.Functions.SetVar("M_{btmy}", 5, "kNm");
                    calculator.Functions.SetVar("M_{btmz}", 5, "kNm");
                    calculator.Functions.SetVar("M_{topy}", 5, "kNm");
                    calculator.Functions.SetVar("M_{topz}", 5, "kNm");
                    calculator.Functions.SetVar("N_{Ed}", 5, "kN");

                    calculator.Functions.SetVar("M_{Fry.Splice1}", 6, "kNm");
                    calculator.Functions.SetVar("M_{Frz.Splice1}", 6, "kNm");


                    calculator.Functions.SetVar("N_{Ed}", 5, "kN");


                    string variables = calculator.GetVariables();
                    /*
                    calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                    string variables2 = calculator.GetVariables();
                    */
                    #endregion


                    #region create, calculate and save tedds file
                    ITeddsDocuments doc1 = new Documents();
                    doc1.Add2("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                    ITeddsDocument doc2 = doc1[0];
                    doc2.Calculate();
                    doc2.SaveAs(groupfolderPath + "\\" + group.Group + ".ted");
                    doc1.Close(false);
                    doc2.Close(false);
                    #endregion
                }
                else
                {
                    MessageBox.Show("Have not created for method to use critical column within group");
                    return true;
                }



            }
            #endregion

            //calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);




            return false;
        }
    }
}
