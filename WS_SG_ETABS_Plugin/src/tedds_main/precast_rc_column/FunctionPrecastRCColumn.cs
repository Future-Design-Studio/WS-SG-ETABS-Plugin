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
using ScottPlot.Drawing.Colormaps;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    internal class FunctionPrecastRCColumn
    {
        private static int Ret;
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
            string designCode = "";
            Ret = SapModel.DesignConcrete.GetCode(ref designCode);

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

                    //create a list of unique names of columns within group to filter
                    List<string> columnUniqueNames = new List<string>();
                    (isError, columnobjectconnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Column Object Connectivity");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Column Object Connectivity table.");
                        return true;
                    }
                    else
                    {

                        foreach (Dictionary<string, string> row in groupassignmentTable.TableInList)
                        {
                            foreach (Dictionary<string,string> rowcolumn in columnobjectconnectivityTable.TableInList)
                            {
                                if (!columnUniqueNames.Contains(row["UniqueName"]) && row["UniqueName"] == rowcolumn["UniqueName"])
                                {
                                    columnUniqueNames.Add(row["UniqueName"]);
                                }
                            }
                        }
                    }

                    //filter group assignment database by names of columns only
                    List<Dictionary<string, string>> groupassignmentTableFiltered = new List<Dictionary<string, string>>();
                    foreach (Dictionary<string, string> row in groupassignmentTable.TableInList)
                    {
                        if (columnUniqueNames.Contains(row["UniqueName"]))
                        {
                            groupassignmentTableFiltered.Add(row);
                        }
                    }
                    groupassignmentTable.TableInList = groupassignmentTableFiltered;
                    #endregion

                    #region filter column object connectivity table by group
                    (isError, columnobjectconnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Column Object Connectivity");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Column Object Connectivity table.");
                        return true;
                    }
                    else
                    {
                        (isError, columnobjectconnectivityTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(columnobjectconnectivityTable, groupassignmentTable, group.Group);
                        if (isError)
                        {
                            MessageBox.Show("Error filtering Column Object Connectivity table based on selected group. Please check that selected group contains columns.");
                            return true;
                        }
                    }
                    #endregion

                    #region filter column overwrite table by group
                    (isError, columnoverwriteTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Concrete Column Overwrites - " + designCode);
                    if (isError)
                    {
                        MessageBox.Show("Error reading Concrete Column Overwrites.");
                        return true;
                    }
                    else
                    {
                        (isError, columnoverwriteTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(columnoverwriteTable, groupassignmentTable, group.Group);
                        if (isError)
                        {
                            MessageBox.Show("Error filtering Concrete Column Overwrites table based on selected group. Please check that selected group contains columns.");
                            return true;
                        }
                    }
                    #endregion

                    #region filter column pmm table by group
                    (isError, columnpmmTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Concrete Column PMM Envelope - " + designCode);
                    if (isError)
                    {
                        MessageBox.Show("Error reading Concrete Column PMM Envelope.");
                        return true;
                    }
                    else
                    {
                        (isError, columnpmmTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(columnpmmTable, groupassignmentTable, group.Group);
                        if (isError)
                        {
                            MessageBox.Show("Error filtering Concrete Column PMM Envelope table based on selected group. Please check that selected group contains columns.");
                            return true;
                        }
                    }
                    #endregion

                    #region get material property table
                    (isError, materialpropertiesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Material Properties - Concrete Data");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Material Properties - Concrete Data.");
                        return true;
                    }
                    #endregion

                    #region filter section property assignment table by group
                    (isError, sectionpropertyassignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Frame Assignments - Section Properties");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Frame Assignments - Section Properties.");
                        return true;
                    }
                    else
                    {
                        (isError, sectionpropertyassignmentTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(sectionpropertyassignmentTable, groupassignmentTable, group.Group);
                        if (isError)
                        {
                            MessageBox.Show("Error filtering Frame Assignments - Section Properties table based on selected group. Please check that selected group contains columns.");
                            return true;
                        }
                    }
                    #endregion

                    #region get circle column definition table
                    (isError, circlecolumndefinitionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Frame Section Property Definitions - Concrete Circle");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Frame Section Property Definitions - Concrete Circle.");
                        return true;
                    }
                    #endregion

                    #region get rectangle column definition table
                    (isError, rectanglecolumndefinitionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Frame Section Property Definitions - Concrete Rectangular");
                    if (isError)
                    {
                        MessageBox.Show("Error reading Frame Section Property Definitions - Concrete Rectangular.");
                        return true;
                    }
                    #endregion

                    double Ptop = new double();
                    double Pbot = new double();
                    double Pcrit = new double();
                    double Mmajortop = new double();
                    double Mmajorbot = new double();
                    double Mmajorcrit = new double();
                    double Mminortop = new double();
                    double Mminorbot = new double();
                    double Mminorcrit = new double();

                    foreach (string column in columnUniqueNames)
                    {
                        #region get forces of the column in question
                        foreach (Dictionary<string,string> row in columnpmmTable.TableInList)
                        {
                            if (row["UniqueName"] == column && row["Location"] == "Top")
                            {
                                Ptop = Convert.ToDouble(row["P"]);
                                Mmajortop = Convert.ToDouble(row["P"]);
                                Mminortop = Convert.ToDouble(row["P"]);
                            }
                            else if (row["UniqueName"] == column && row["Location"] == "Bottom")
                            {
                                Pbot = Convert.ToDouble(row["P"]);
                                Mmajorbot = Convert.ToDouble(row["P"]);
                                Mminorbot = Convert.ToDouble(row["P"]);
                                break;
                            }
                        }
                        Pcrit = Math.Max(Math.Abs(Ptop), Math.Abs(Pbot));
                        Mmajorcrit = Math.Max(Math.Abs(Mmajortop), Math.Abs(Mmajorbot));
                        Mminorcrit = Math.Max(Math.Abs(Mminortop), Math.Abs(Mminorbot));
                        #endregion

                        #region get concrete fck, section size, section type, of the column in question

                        string sectionType = null;

                        foreach (Dictionary<string,string> row in sectionpropertyassignmentTable.TableInList)
                        {
                            if (row["UniqueName"] == column)
                            {
                                if (row["Shape"] == "Concrete Rectangular")
                                    sectionType = "Rectangular";
                                else if (row["Shape"] == "Concrete Circular")
                                {
                                    sectionType = "Circular";
                                }
                                else
                                {
                                    break;                               
                                }
                            }
                        }
                        if (sectionType == null)
                        {
                            break;
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
                        calculator.Functions.SetVar("c_{nom}", group.Cover, "mm");
                        calculator.Functions.SetVar("_CalcUI", 0);

                        //input from etabs required
                        //calculator.Functions.SetVar("_rcc.ColumnGrade", "C90/105");

                        //calculator.Functions.SetVar("_rcc.Type", sectionType);
                        //calculator.Functions.SetVar("b", 500, "mm");
                        //calculator.Functions.SetVar("h", 500, "mm");

                        //calculator.Functions.SetVar("l_{0y}", 500, "mm");
                        //calculator.Functions.SetVar("l_{0z}", 500, "mm");

                        calculator.Functions.SetVar("M_{btmy}", Mmajorbot, "kNm");
                        calculator.Functions.SetVar("M_{btmz}", Mminorbot, "kNm");
                        calculator.Functions.SetVar("M_{topy}", Mmajortop, "kNm");
                        calculator.Functions.SetVar("M_{topz}", Mminortop, "kNm");
                        calculator.Functions.SetVar("N_{Ed}", Pcrit, "kN");

                        calculator.Functions.SetVar("M_{Fry.Splice1}", Mmajorcrit, "kNm");
                        calculator.Functions.SetVar("M_{Frz.Splice1}", Mminorcrit, "kNm");

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
