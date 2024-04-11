using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accord.Math.Distances;
using ETABSv1;
using Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    internal class FunctionJointForcesExtractorMain
    {

        public static bool ExtractJointForcesExcel(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase, List<string> listChoosenGroup, string filePath)
        {

            bool isError = false;
            src.HelperEtabs.DatabaseTable groupAssignmentTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable frameEndForcesTable = new HelperEtabs.DatabaseTable();

            foreach (src.HelperEtabs.DatabaseTable table in databaseTableList)
            {
                if (table.TableKey == "Group Assignments")
                {
                    (isError, groupAssignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Element Joint Forces - Frame")
                {
                    (isError, frameEndForcesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Joint Forces - Frame");
                    if (isError)
                    {
                        return isError;
                    }
                }
                else
                {
                    MessageBox.Show("Error, list of database tables do not match, please check that you have ");
                }
            }

            //Filter group by points only
            (isError, groupAssignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentTable, "ObjectType", "Point");

            //Filter group by selected group name
            List<Dictionary<string, string>> groupAssignmentTableFiltered = new List<Dictionary<string, string>>();
            List<string> pointUniqueNames = new List<string>();
            foreach (Dictionary<string, string> row in groupAssignmentTable.TableInList)
            {
                if (listChoosenGroup.Contains(row["GroupName"]))
                {
                    groupAssignmentTableFiltered.Add(row);
                    if (pointUniqueNames.Contains(row["UniqueName"]))
                    {
                        continue;
                    }
                    else
                    {
                        pointUniqueNames.Add(row["UniqueName"]);
                    }
                }
            }


            List<Dictionary<string, string>> frameEndForcesTableFiltered = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in frameEndForcesTable.TableInList)
            {
                if (pointUniqueNames.Contains(row["Joint"]) && listChoosenLoadCase.Contains(row["OutputCase"]))
                {
                    frameEndForcesTableFiltered.Add(row);
                }
            }


            List<Dictionary<string, string>> finalTableToExcel = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in frameEndForcesTableFiltered)
            {
                Dictionary<string, string> rowinternal = new Dictionary<string, string>
                {
                    {"Joint Unique Name", row["Joint"]},
                    {"Frame Unique Name", row["UniqueName"]},
                    {"Load Case/Combi", row["OutputCase"]},
                    {"Step Type", row["StepType"]},
                    {"Axial (kN)", row["F1"]},
                    {"Shear Minor (kN)", row["F2"]},
                    {"Shear Major (kN)", row["F3"]},
                    {"Moment Minor (kN)", row["M1"]},
                    {"Moment Major (kN)", row["M2"]},
                    {"Torsion (kN)", row["M3"]},
                };
                finalTableToExcel.Add(rowinternal);
            }



            

            //here to create object MANI

            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "fyk (Mpa)", "fck (Mpa)", "d (ETABS) (mm)", "d calculated (mm)", "Beam type", "As Min(mm2)", "Load case"
                , "Left Moment (-) (kNm)", "Mid Moment (-) (kNm)", "Right Moment (-) (kNm)", "Left Moment (+) (kNm)", "Mid Moment (+) (kNm)", "Right Moment (+) (kNm)"
                , "Left Shear (kN)", "Mid Shear (kN)", "Right Shear (kN)"
                , "Left Top As (mm2)", "Mid Top As (mm2)", "Right Top As (mm2)", "Left Bottom As (mm2)", "Mid Bottom As (mm2)", "Right Bottom As (mm2)"
                , "Left Av (mm2/m)", "Mid Av (mm2/m)", "Right Av (mm2/m)"
                , "Left Top Rebar", "Mid Top Rebar", "Right Top Rebar", "Left Bottom Rebar", "Mid Bottom Rebar", "Right Bottom Rebar"
                , "Left Shear Links", "Mid Shear Links", "Right Shear Links"
                , "Left Top As Prov (mm2)", "Mid Top As Prov (mm2)", "Right Top As Prov (mm2)", "Left Bottom As  Prov (mm2)", "Mid Bottom As Prov (mm2)", "Right Bottom As Prov (mm2)"
                , "Left Av Prov (mm2/m)", "Mid Av Prov (mm2/m)", "Right Av Prov (mm2/m)"
                , "Left Top As Ratio", "Mid Top As Ratio", "Right Top As Ratio", "Left Bottom As Ratio", "Mid Bottom As Ratio", "Right Bottom As Ratio"
                , "Left Av Ratio", "Mid Av Ratio", "Right Av Ratio"
                , "ETABS warning", "Rebar warning", "Shear links warning",
                "Cluster"
            };

            int numOfRow = rcBeamList.Count() * rcBeamList[0].FrameForceEachCaseList.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1, numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                foreach (src.HelperEtabs.FrameForceEachCase frameForceEachCase in rcBeam.FrameForceEachCaseList)
                {
                    result[i, 0] = rcBeam.UniqueName;
                    result[i, 1] = rcBeam.Width;
                    result[i, 2] = rcBeam.Height;
                    result[i, 3] = rcBeam.Fyk;
                    result[i, 4] = rcBeam.Fck;
                    result[i, 5] = rcBeam.d;
                    result[i, 6] = rcBeam.dCalculated;
                    result[i, 7] = rcBeam.BeamType;
                    result[i, 8] = rcBeam.AsMin;

                    result[i, 9] = frameForceEachCase.LoadCaseName;

                    result[i, 10] = frameForceEachCase.LeftMomentNeg;
                    result[i, 11] = frameForceEachCase.MidMomentNeg;
                    result[i, 12] = frameForceEachCase.RightMomentNeg;
                    result[i, 13] = frameForceEachCase.LeftMomentPos;
                    result[i, 14] = frameForceEachCase.MidMomentPos;
                    result[i, 15] = frameForceEachCase.RightMomentPos;

                    result[i, 16] = frameForceEachCase.LeftShear;
                    result[i, 17] = frameForceEachCase.MidShear;
                    result[i, 18] = frameForceEachCase.RightShear;

                    i++;
                    Console.WriteLine(i);
                }

            }

            return result;



            //HERE TO WRITE TO EXCEL MANI

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            xlApp.Visible = false;

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //Add worksheets
            xlWorkBook.Sheets.Add(); //Self design

            //Sheet 1
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Name = "ETABS design";

            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            string lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

            //Sheet 2
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            xlWorkSheet.Name = "Force envelope";

            numOfRow = result2.GetLength(0);
            numOfCol = result2.GetLength(1);

            alphabetOfCol = src.HelperExcel.GetExcelColumnName(numOfCol);
            lastCellRange = alphabetOfCol + Convert.ToString(numOfRow - 1 + 1);

            rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result2;

            
            //Here saving the file in xlsx
            string excelPath = filePath + fileName + ".xlsx";

            try
            {
                xlWorkBook.SaveAs(excelPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();




            





            return false;
        }
    }
}
