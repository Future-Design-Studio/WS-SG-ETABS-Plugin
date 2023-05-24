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
        public static bool PrecastRCColumn(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList, string folderPathselected)
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
            string folderPath = folderPathselected + "TeddsFilesFromTeddsPowerAPP\\";
            bool exists = System.IO.Directory.Exists(folderPath);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            #endregion

            #region start main function

            foreach (ClassPrecastRCColumn.GroupandCoverandGradeMatch group in groupList)
            {
                #region method to output every single column inside group as long as no overstressed column within the grup
                if (group.Method == 0)
                {
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
                    if (columnUniqueNames.Count == 0)
                    {
                        continue;
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

                    if (designCode != "Eurocode 2-2004")
                    {
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

                        if (columnoverwriteTable.TableInList.Count == 0)
                        {
                            continue;
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
                        else
                        {
                            //check if have any overstressed column
                            foreach (Dictionary<string, string> row in columnpmmTable.TableInList)
                            {
                                if (row["RatioRebar"] == "O/S")
                                {
                                    MessageBox.Show("Warning: Column Unique Name " + row["UniqueName"] + " in group " + group.Group + " is overstressed. Please recheck design before running again. Skipping this group for TEDDS output.");
                                    isError = true;
                                    break;
                                }
                            }
                            if (isError)
                            {
                                continue;
                            }
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


                    foreach (string column in columnUniqueNames)
                    {
                        string storey = null;
                        double Ptop = new double();
                        double Pbot = new double();
                        double Pcrit = new double();
                        double Mmajortop = new double();
                        double Mmajorbot = new double();
                        double Mmajorcrit = new double();
                        double Mminortop = new double();
                        double Mminorbot = new double();
                        double Mminorcrit = new double();

                        #region get forces of the column in question
                        foreach (Dictionary<string,string> row in columnpmmTable.TableInList)
                        {
                            if (row["UniqueName"] == column && row["Location"] == "Top")
                            {
                                Ptop = Convert.ToDouble(row["P"]);
                                Mmajortop = Convert.ToDouble(row["MMajor"]);
                                Mminortop = Convert.ToDouble(row["MMinor"]);
                                storey = row["Story"];
                            }
                            else if (row["UniqueName"] == column && row["Location"] == "Bottom")
                            {
                                Pbot = Convert.ToDouble(row["P"]);
                                Mmajorbot = Convert.ToDouble(row["MMajor"]);
                                Mminorbot = Convert.ToDouble(row["MMinor"]);
                                break;
                            }
                        }
                        Pcrit = Math.Max(Math.Abs(Ptop), Math.Abs(Pbot));
                        Mmajorcrit = Math.Max(Math.Abs(Mmajortop), Math.Abs(Mmajorbot));
                        Mminorcrit = Math.Max(Math.Abs(Mminortop), Math.Abs(Mminorbot));
                        #endregion

                        #region get concrete fck, section size, section type, of the column in question

                        string sectionType = null;
                        string sectionProperty = null;
                        string materialProperty = null;
                        double concreteStrength = new double();
                        string concreteGrade = null;
                        double width = new double();
                        double height = new double();

                        foreach (Dictionary<string,string> row in sectionpropertyassignmentTable.TableInList)
                        {
                            if (row["UniqueName"] == column)
                            {
                                #region get shape of element
                                if (row["Shape"] == "Concrete Rectangular")
                                    sectionType = "Rectangular";
                                else if (row["Shape"] == "Concrete Circle")
                                {
                                    sectionType = "Circular";
                                }
                                else
                                {
                                    break;                               
                                }
                                #endregion

                                sectionProperty = row["SectProp"];
                                if (sectionType == "Rectangular")
                                {
                                    #region get height and width
                                    foreach (Dictionary<string,string> rowrect in rectanglecolumndefinitionTable.TableInList)
                                    {
                                        if (rowrect["Name"] == sectionProperty)
                                        {
                                            materialProperty = rowrect["Material"];
                                            height = Convert.ToDouble(rowrect["t3"]);
                                            width = Convert.ToDouble(rowrect["t2"]);
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region get concrete strength and grade
                                    foreach (Dictionary<string,string> rowmaterial in materialpropertiesTable.TableInList)
                                    {
                                        if (rowmaterial["Material"] == materialProperty)
                                        {
                                            concreteStrength = Convert.ToDouble(rowmaterial["Fc"]);

                                            if (concreteStrength < 16000)
                                            {
                                                concreteGrade = "C12/15";
                                            }
                                            else if (concreteStrength < 20000)
                                            {
                                                concreteGrade = "C16/20";
                                            }
                                            else if (concreteStrength < 25000)
                                            {
                                                concreteGrade = "C20/25";
                                            }
                                            else if (concreteStrength < 28000)
                                            {
                                                concreteGrade = "C25/30";
                                            }
                                            else if (concreteStrength < 30000)
                                            {
                                                concreteGrade = "C28/35";
                                            }
                                            else if (concreteStrength < 32000)
                                            {
                                                concreteGrade = "C30/37";
                                            }
                                            else if (concreteStrength < 35000)
                                            {
                                                concreteGrade = "C32/40";
                                            }
                                            else if (concreteStrength < 40000)
                                            {
                                                concreteGrade = "C35/45";
                                            }
                                            else if (concreteStrength < 45000)
                                            {
                                                concreteGrade = "C40/50";
                                            }
                                            else if (concreteStrength < 50000)
                                            {
                                                concreteGrade = "C45/55";
                                            }
                                            else if (concreteStrength < 55000)
                                            {
                                                concreteGrade = "C50/60";
                                            }
                                            else if (concreteStrength < 60000)
                                            {
                                                concreteGrade = "C55/67";
                                            }
                                            else if (concreteStrength < 70000)
                                            {
                                                concreteGrade = "C60/75";
                                            }
                                            else if (concreteStrength < 80000)
                                            {
                                                concreteGrade = "C70/85";
                                            }
                                            else if (concreteStrength < 90000)
                                            {
                                                concreteGrade = "C80/95";
                                            }
                                            else if (concreteStrength >= 90000)
                                            {
                                                concreteGrade = "C90/105";
                                            }
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                                else if (sectionType == "Circular")
                                {
                                    #region get height and width
                                    foreach (Dictionary<string, string> rowcirc in circlecolumndefinitionTable.TableInList)
                                    {
                                        if (rowcirc["Name"] == sectionProperty)
                                        {
                                            materialProperty = rowcirc["Material"];
                                            height = Convert.ToDouble(rowcirc["t3"]);
                                            width = Convert.ToDouble(rowcirc["t3"]);
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region get concrete strength and grade
                                    foreach (Dictionary<string, string> rowmaterial in materialpropertiesTable.TableInList)
                                    {
                                        if (rowmaterial["Material"] == materialProperty)
                                        {
                                            concreteStrength = Convert.ToDouble(rowmaterial["Fc"]);

                                            if (concreteStrength < 16000)
                                            {
                                                concreteGrade = "C12/15";
                                            }
                                            else if (concreteStrength < 20000)
                                            {
                                                concreteGrade = "C16/20";
                                            }
                                            else if (concreteStrength < 25000)
                                            {
                                                concreteGrade = "C20/25";
                                            }
                                            else if (concreteStrength < 28000)
                                            {
                                                concreteGrade = "C25/30";
                                            }
                                            else if (concreteStrength < 30000)
                                            {
                                                concreteGrade = "C28/35";
                                            }
                                            else if (concreteStrength < 32000)
                                            {
                                                concreteGrade = "C30/37";
                                            }
                                            else if (concreteStrength < 35000)
                                            {
                                                concreteGrade = "C32/40";
                                            }
                                            else if (concreteStrength < 40000)
                                            {
                                                concreteGrade = "C35/45";
                                            }
                                            else if (concreteStrength < 45000)
                                            {
                                                concreteGrade = "C40/50";
                                            }
                                            else if (concreteStrength < 50000)
                                            {
                                                concreteGrade = "C45/55";
                                            }
                                            else if (concreteStrength < 55000)
                                            {
                                                concreteGrade = "C50/60";
                                            }
                                            else if (concreteStrength < 60000)
                                            {
                                                concreteGrade = "C55/67";
                                            }
                                            else if (concreteStrength < 70000)
                                            {
                                                concreteGrade = "C60/75";
                                            }
                                            else if (concreteStrength < 80000)
                                            {
                                                concreteGrade = "C70/85";
                                            }
                                            else if (concreteStrength < 90000)
                                            {
                                                concreteGrade = "C80/95";
                                            }
                                            else if (concreteStrength >= 90000)
                                            {
                                                concreteGrade = "C90/105";
                                            }
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    MessageBox.Show("Unknown error with section type, please check code");
                                    return true;
                                }
                                break;
                            }
                        }
                        if (sectionType == null || sectionProperty == null || materialProperty == null || concreteStrength.ToString() == null || concreteGrade == null || width.ToString() == null || height.ToString() == null)
                        {
                            break;
                        }

                        #endregion

                        #region get effective length factors

                        double length = new double();
                        double majoreffectiveLength = new double();
                        double minoreffectiveLength = new double();
                        double majorFactor = new double();
                        double minorFactor = new double();
                        bool progDet = false;

                        foreach  (Dictionary<string,string> row in columnobjectconnectivityTable.TableInList)
                        {
                            if (row["UniqueName"] == column)
                            {
                                length = Convert.ToDouble(row["Length"]);
                                if (designCode != "Eurocode 2-2004")
                                {
                                    foreach (Dictionary<string,string> rowoverwrite in columnoverwriteTable.TableInList)
                                    {
                                        if (rowoverwrite["UniqueName"] == column)
                                        {
                                            majorFactor = Convert.ToDouble(rowoverwrite["LMajor"]);
                                            majoreffectiveLength = length * majorFactor;
                                            minorFactor = Convert.ToDouble(rowoverwrite["LMinor"]);
                                            minoreffectiveLength = length * minorFactor;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Ret = SapModel.DesignConcrete.Eurocode_2_2004.GetOverwrite(column, 3, ref majorFactor, ref progDet);
                                    Ret = SapModel.DesignConcrete.Eurocode_2_2004.GetOverwrite(column, 4, ref minorFactor, ref progDet);
                                    majoreffectiveLength = length * majorFactor;
                                    minoreffectiveLength = length * minorFactor;
                                }
                                break;
                            }
                        }
                        if (length.ToString() == null || majoreffectiveLength.ToString() == null || minoreffectiveLength.ToString() == null || majorFactor.ToString() == null || minorFactor.ToString() == null)
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
                        calculator.Functions.SetVar("c_{nom}", Convert.ToDouble(group.Cover), "mm");
                        calculator.Functions.SetVar("_CalcUI", 1);

                        //input from etabs required
                        calculator.Functions.SetVar("_rcc.ColumnGrade", concreteGrade);

                        calculator.Functions.SetVar("_rcc.Type", sectionType);
                        calculator.Functions.SetVar("b", width, "m");
                        calculator.Functions.SetVar("h", height, "m");

                        calculator.Functions.SetVar("l_{0y}", majoreffectiveLength, "m");
                        calculator.Functions.SetVar("l_{0z}", minoreffectiveLength, "m");

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

                        #region create, save tedds file
                        ITeddsDocuments doc1 = new Documents();
                        doc1.Add2("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                        ITeddsDocument doc2 = doc1[0];
                        //doc2.Calculate();
                        doc2.SaveAs(folderPath + "\\" + group.Group + "_" + storey +"_" + column + "_all_column_method.ted");
                        doc2.Close(false);
                        doc1.Close(false);
                        
                        

                        #endregion
                    }

                }
                #endregion

                #region method to output the most critical columns within the group, done by taking the max rebar % within the group, if more than 1 column have the same max rebar %, all the max rebar % columns will output tedds for user to choose which is most critical from the forces
                else
                {

                    #region filter group assignment table
                    (isError, groupassignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(DatabaseTableListETABS, "Group Assignments");
                    if (isError)
                    {
                        MessageBox.Show("Error reading group Assignment table, please ensure you have added in columns that you would like to check on TEDDS into Groups on ETABS.");
                        return true;
                    }
                    else
                    {
                        //Filter group to only include lines, to improve performance
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
                            foreach (Dictionary<string, string> rowcolumn in columnobjectconnectivityTable.TableInList)
                            {
                                if (!columnUniqueNames.Contains(row["UniqueName"]) && row["UniqueName"] == rowcolumn["UniqueName"])
                                {
                                    columnUniqueNames.Add(row["UniqueName"]);
                                }
                            }
                        }
                    }
                    if (columnUniqueNames.Count == 0)
                    {
                        continue;
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

                    if (columnpmmTable.TableInList.Count == 0)
                    {
                        continue;
                    }
                    #endregion

                    #region create list of stories
                    List<string> storyList = new List<string>();
                    foreach (Dictionary<string,string> row in columnpmmTable.TableInList)
                    {
                        if (!storyList.Contains(row["Story"]))
                        {
                            storyList.Add(row["Story"]);
                        }
                    }
                    #endregion

                    Dictionary<string,double> storyRebarMatching = new Dictionary<string,double>();
                    #region filter columnpmm by each story and get max pmm ratio per story
                    // Filter group to only include points, to improve performance
                    foreach (string individualStory in storyList)
                    {
                        src.HelperEtabs.DatabaseTable temporarypmmTable = new HelperEtabs.DatabaseTable();
                        (isError, temporarypmmTable) = src.HelperEtabs.FilterDatabaseTable(columnpmmTable, "Story", individualStory);
                        if (isError)
                        {
                            MessageBox.Show("Error filtering PMM table by storey. Check code");
                            return true;
                        }
                        else
                        {
                            storyRebarMatching.Add(individualStory, 0);
                            foreach (Dictionary<string,string> row in temporarypmmTable.TableInList)
                            {
                                if (row["RatioRebar"] == "O/S")
                                {
                                    MessageBox.Show("Warning: Column Unique Name " + row["UniqueName"] + " in group " + group.Group + " is overstressed. Please recheck design before running again. Skipping this group for TEDDS output.");
                                    isError = true;
                                    break;
                                }
                                else
                                {
                                    if (Convert.ToDouble(row["RatioRebar"].Substring(0, row["RatioRebar"].Length - 2)) > storyRebarMatching[individualStory])
                                    {
                                        storyRebarMatching[individualStory] = Convert.ToDouble(row["RatioRebar"].Substring(0, row["RatioRebar"].Length - 2));
                                    }
                                }
                            }
                            if (isError)
                            {
                                break; ;
                            }
                        }
                    }
                    if (isError)
                    {
                        continue;
                    }
                    #endregion

                    #region get unique name of columns with max pmm ratios
                    columnUniqueNames.Clear();
                    foreach (Dictionary<string,string> row in columnpmmTable.TableInList)
                    {
                        if ((Convert.ToDouble(row["RatioRebar"].Substring(0, row["RatioRebar"].Length - 2)) == storyRebarMatching[row["Story"]]) && (!columnUniqueNames.Contains(row["UniqueName"])))
                        {
                            columnUniqueNames.Add(row["UniqueName"]);
                        }
                    }
                    #endregion

                    #region redo group and pmm tables filtering
                    //filter group assignment database by names of columns only
                    List<Dictionary<string, string>> groupassignmentTableFiltered2 = new List<Dictionary<string, string>>();
                    foreach (Dictionary<string, string> row in groupassignmentTable.TableInList)
                    {
                        if (columnUniqueNames.Contains(row["UniqueName"]))
                        {
                            groupassignmentTableFiltered2.Add(row);
                        }
                    }
                    groupassignmentTable.TableInList = groupassignmentTableFiltered2;

                    //filter pmm envelope by new group
                    (isError, columnpmmTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(columnpmmTable, groupassignmentTable, group.Group);
                    if (isError)
                    {
                        MessageBox.Show("Error filtering Concrete Column PMM Envelope table based on selected group. Please check that selected group contains columns.");
                        return true;
                    }
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

                    if (designCode != "Eurocode 2-2004")
                    {
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


                    foreach (string column in columnUniqueNames)
                    {
                        string storey = null;
                        double Ptop = new double();
                        double Pbot = new double();
                        double Pcrit = new double();
                        double Mmajortop = new double();
                        double Mmajorbot = new double();
                        double Mmajorcrit = new double();
                        double Mminortop = new double();
                        double Mminorbot = new double();
                        double Mminorcrit = new double();

                        #region get forces of the column in question
                        foreach (Dictionary<string, string> row in columnpmmTable.TableInList)
                        {
                            if (row["UniqueName"] == column && row["Location"] == "Top")
                            {
                                Ptop = Convert.ToDouble(row["P"]);
                                Mmajortop = Convert.ToDouble(row["MMajor"]);
                                Mminortop = Convert.ToDouble(row["MMinor"]);
                                storey = row["Story"];
                            }
                            else if (row["UniqueName"] == column && row["Location"] == "Bottom")
                            {
                                Pbot = Convert.ToDouble(row["P"]);
                                Mmajorbot = Convert.ToDouble(row["MMajor"]);
                                Mminorbot = Convert.ToDouble(row["MMinor"]);
                                break;
                            }
                        }
                        Pcrit = Math.Max(Math.Abs(Ptop), Math.Abs(Pbot));
                        Mmajorcrit = Math.Max(Math.Abs(Mmajortop), Math.Abs(Mmajorbot));
                        Mminorcrit = Math.Max(Math.Abs(Mminortop), Math.Abs(Mminorbot));
                        #endregion

                        #region get concrete fck, section size, section type, of the column in question

                        string sectionType = null;
                        string sectionProperty = null;
                        string materialProperty = null;
                        double concreteStrength = new double();
                        string concreteGrade = null;
                        double width = new double();
                        double height = new double();

                        foreach (Dictionary<string, string> row in sectionpropertyassignmentTable.TableInList)
                        {
                            if (row["UniqueName"] == column)
                            {
                                #region get shape of element
                                if (row["Shape"] == "Concrete Rectangular")
                                    sectionType = "Rectangular";
                                else if (row["Shape"] == "Concrete Circle")
                                {
                                    sectionType = "Circular";
                                }
                                else
                                {
                                    break;
                                }
                                #endregion

                                sectionProperty = row["SectProp"];
                                if (sectionType == "Rectangular")
                                {
                                    #region get height and width
                                    foreach (Dictionary<string, string> rowrect in rectanglecolumndefinitionTable.TableInList)
                                    {
                                        if (rowrect["Name"] == sectionProperty)
                                        {
                                            materialProperty = rowrect["Material"];
                                            height = Convert.ToDouble(rowrect["t3"]);
                                            width = Convert.ToDouble(rowrect["t2"]);
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region get concrete strength and grade
                                    foreach (Dictionary<string, string> rowmaterial in materialpropertiesTable.TableInList)
                                    {
                                        if (rowmaterial["Material"] == materialProperty)
                                        {
                                            concreteStrength = Convert.ToDouble(rowmaterial["Fc"]);

                                            if (concreteStrength < 16000)
                                            {
                                                concreteGrade = "C12/15";
                                            }
                                            else if (concreteStrength < 20000)
                                            {
                                                concreteGrade = "C16/20";
                                            }
                                            else if (concreteStrength < 25000)
                                            {
                                                concreteGrade = "C20/25";
                                            }
                                            else if (concreteStrength < 28000)
                                            {
                                                concreteGrade = "C25/30";
                                            }
                                            else if (concreteStrength < 30000)
                                            {
                                                concreteGrade = "C28/35";
                                            }
                                            else if (concreteStrength < 32000)
                                            {
                                                concreteGrade = "C30/37";
                                            }
                                            else if (concreteStrength < 35000)
                                            {
                                                concreteGrade = "C32/40";
                                            }
                                            else if (concreteStrength < 40000)
                                            {
                                                concreteGrade = "C35/45";
                                            }
                                            else if (concreteStrength < 45000)
                                            {
                                                concreteGrade = "C40/50";
                                            }
                                            else if (concreteStrength < 50000)
                                            {
                                                concreteGrade = "C45/55";
                                            }
                                            else if (concreteStrength < 55000)
                                            {
                                                concreteGrade = "C50/60";
                                            }
                                            else if (concreteStrength < 60000)
                                            {
                                                concreteGrade = "C55/67";
                                            }
                                            else if (concreteStrength < 70000)
                                            {
                                                concreteGrade = "C60/75";
                                            }
                                            else if (concreteStrength < 80000)
                                            {
                                                concreteGrade = "C70/85";
                                            }
                                            else if (concreteStrength < 90000)
                                            {
                                                concreteGrade = "C80/95";
                                            }
                                            else if (concreteStrength >= 90000)
                                            {
                                                concreteGrade = "C90/105";
                                            }
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                                else if (sectionType == "Circular")
                                {
                                    #region get height and width
                                    foreach (Dictionary<string, string> rowcirc in circlecolumndefinitionTable.TableInList)
                                    {
                                        if (rowcirc["Name"] == sectionProperty)
                                        {
                                            materialProperty = rowcirc["Material"];
                                            height = Convert.ToDouble(rowcirc["t3"]);
                                            width = Convert.ToDouble(rowcirc["t3"]);
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region get concrete strength and grade
                                    foreach (Dictionary<string, string> rowmaterial in materialpropertiesTable.TableInList)
                                    {
                                        if (rowmaterial["Material"] == materialProperty)
                                        {
                                            concreteStrength = Convert.ToDouble(rowmaterial["Fc"]);

                                            if (concreteStrength < 16000)
                                            {
                                                concreteGrade = "C12/15";
                                            }
                                            else if (concreteStrength < 20000)
                                            {
                                                concreteGrade = "C16/20";
                                            }
                                            else if (concreteStrength < 25000)
                                            {
                                                concreteGrade = "C20/25";
                                            }
                                            else if (concreteStrength < 28000)
                                            {
                                                concreteGrade = "C25/30";
                                            }
                                            else if (concreteStrength < 30000)
                                            {
                                                concreteGrade = "C28/35";
                                            }
                                            else if (concreteStrength < 32000)
                                            {
                                                concreteGrade = "C30/37";
                                            }
                                            else if (concreteStrength < 35000)
                                            {
                                                concreteGrade = "C32/40";
                                            }
                                            else if (concreteStrength < 40000)
                                            {
                                                concreteGrade = "C35/45";
                                            }
                                            else if (concreteStrength < 45000)
                                            {
                                                concreteGrade = "C40/50";
                                            }
                                            else if (concreteStrength < 50000)
                                            {
                                                concreteGrade = "C45/55";
                                            }
                                            else if (concreteStrength < 55000)
                                            {
                                                concreteGrade = "C50/60";
                                            }
                                            else if (concreteStrength < 60000)
                                            {
                                                concreteGrade = "C55/67";
                                            }
                                            else if (concreteStrength < 70000)
                                            {
                                                concreteGrade = "C60/75";
                                            }
                                            else if (concreteStrength < 80000)
                                            {
                                                concreteGrade = "C70/85";
                                            }
                                            else if (concreteStrength < 90000)
                                            {
                                                concreteGrade = "C80/95";
                                            }
                                            else if (concreteStrength >= 90000)
                                            {
                                                concreteGrade = "C90/105";
                                            }
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    MessageBox.Show("Unknown error with section type, please check code");
                                    return true;
                                }
                                break;
                            }
                        }
                        if (sectionType == null || sectionProperty == null || materialProperty == null || concreteStrength.ToString() == null || concreteGrade == null || width.ToString() == null || height.ToString() == null)
                        {
                            break;
                        }

                        #endregion

                        #region get effective length factors

                        double length = new double();
                        double majoreffectiveLength = new double();
                        double minoreffectiveLength = new double();
                        double majorFactor = new double();
                        double minorFactor = new double();
                        bool progDet = false;

                        foreach (Dictionary<string, string> row in columnobjectconnectivityTable.TableInList)
                        {
                            if (row["UniqueName"] == column)
                            {
                                length = Convert.ToDouble(row["Length"]);
                                if (designCode != "Eurocode 2-2004")
                                {
                                    foreach (Dictionary<string, string> rowoverwrite in columnoverwriteTable.TableInList)
                                    {
                                        if (rowoverwrite["UniqueName"] == column)
                                        {
                                            majorFactor = Convert.ToDouble(rowoverwrite["LMajor"]);
                                            majoreffectiveLength = length * majorFactor;
                                            minorFactor = Convert.ToDouble(rowoverwrite["LMinor"]);
                                            minoreffectiveLength = length * minorFactor;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Ret = SapModel.DesignConcrete.Eurocode_2_2004.GetOverwrite(column, 3, ref majorFactor, ref progDet);
                                    Ret = SapModel.DesignConcrete.Eurocode_2_2004.GetOverwrite(column, 4, ref minorFactor, ref progDet);
                                    majoreffectiveLength = length * majorFactor;
                                    minoreffectiveLength = length * minorFactor;
                                }
                                break;
                            }
                        }
                        if (length.ToString() == null || majoreffectiveLength.ToString() == null || minoreffectiveLength.ToString() == null || majorFactor.ToString() == null || minorFactor.ToString() == null)
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
                        calculator.Functions.SetVar("c_{nom}", Convert.ToDouble(group.Cover), "mm");
                        calculator.Functions.SetVar("_CalcUI", 1);

                        //input from etabs required
                        calculator.Functions.SetVar("_rcc.ColumnGrade", concreteGrade);

                        calculator.Functions.SetVar("_rcc.Type", sectionType);
                        calculator.Functions.SetVar("b", width, "m");
                        calculator.Functions.SetVar("h", height, "m");

                        calculator.Functions.SetVar("l_{0y}", majoreffectiveLength, "m");
                        calculator.Functions.SetVar("l_{0z}", minoreffectiveLength, "m");

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

                        #region create, save tedds file
                        ITeddsDocuments doc1 = new Documents();
                        doc1.Add2("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                        ITeddsDocument doc2 = doc1[0];
                        //doc2.Calculate();
                        doc2.SaveAs(folderPath + "\\" + group.Group + "_" + storey + "_" + column + "_critical_column_method.ted");
                        doc2.Close(false);
                        doc1.Close(false);



                        #endregion
                    }
                }
                #endregion
            }
            #endregion

            return false;
        }
    }
}
