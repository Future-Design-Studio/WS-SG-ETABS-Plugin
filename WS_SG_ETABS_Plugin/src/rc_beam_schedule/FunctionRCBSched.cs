using ETABSv1;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    internal class FunctionRCBSched
    {

        private static int Ret;

        public static bool GetRCBeamSchedule(cSapModel Sap, string fileName, string filePath, double leftLength, double midLength, double rightLength)
        {
            //Get current unit
            eForce forceUnits = new eForce();
            eLength lengthUnits = new eLength();
            eTemperature temperatureUnits = new eTemperature();
            Ret = Sap.GetPresentUnits_2(ref forceUnits, ref lengthUnits, ref temperatureUnits);

            //Set unit to mm
            src.HelperEtabs.SetOutputUnitTo_kN_mm_C(Sap);

            bool isError = false;

            bool tableExist = CheckTableExistRCBSched(Sap);
            
            if (!tableExist)
            {
                isError = true;
                return isError;
            }

            //Get RC design result table
            src.HelperEtabs.DatabaseTable rcBeamDesignSummary = new src.HelperEtabs.DatabaseTable();
            (isError, rcBeamDesignSummary) = GetRcBeamDesignResult(Sap);

            if (isError)
            {
                return isError;
            }

            //Get list of beams
            List<src.HelperEtabs.Frame> rcBeamList = new List<src.HelperEtabs.Frame>();
            rcBeamList = GetRcBeamList(rcBeamDesignSummary);

            //Get dimension
            GetRcBeamDimension(Sap, rcBeamDesignSummary, rcBeamList);

            //Get rebar required from etabs design
            GetRcBeamRebarArea(Sap, rcBeamDesignSummary, rcBeamList, leftLength, midLength, rightLength);

            //Convert area to rebar

            //Compile data to 2d array
            var result = CompileResutRcBeamToArray(rcBeamList);

            //Write to excel
            WriteResultRcBeamToExcel(result, fileName, filePath);

            //Set back to original units
            src.HelperEtabs.SetOutputUnit(Sap, forceUnits, lengthUnits, temperatureUnits);

            return isError;
        }

        /// <summary>
        /// Check if the required tables exist
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>Return true if all required tables exist</returns>
        public static bool CheckTableExistRCBSched(cSapModel Sap)
        {
            string designCode = "";

            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            //Check if the table exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add("Concrete Beam Design Summary - " + designCode);

            List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForDisplay(Sap, tableKeys);

            if (tableKeysNotExist.Count > 0)
            {
                foreach (string tableKeyNotExist in tableKeysNotExist)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKeyNotExist);
                }

                return false;
            }

            return true;
        }



        public static (bool, src.HelperEtabs.DatabaseTable) GetRcBeamDesignResult(cSapModel Sap)
        {
            //Get RC design result table
            src.HelperEtabs.DatabaseTable rcBeamDesignSummary = new src.HelperEtabs.DatabaseTable();
            bool isError;

            string designCode = "";
            Ret = Sap.DesignConcrete.GetCode(ref designCode);

            if (designCode != "Eurocode 2-2004")
            {
                isError = true;
                MessageBox.Show("Only support Eurocode 2-2004!");
                return (isError, rcBeamDesignSummary);
            }

            string tableKey = "Concrete Beam Design Summary - " + designCode;

            (isError, rcBeamDesignSummary) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            return (isError, rcBeamDesignSummary);
        }

        public static List<src.HelperEtabs.Frame> GetRcBeamList(src.HelperEtabs.DatabaseTable rcBeamDesignSummary)
        {
            //Get table
             List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            List<src.HelperEtabs.Frame> rcBeamList = new List<HelperEtabs.Frame>();

            //Get list of unique name
            List<string> uniqueNameAllList = new List<string>();

            foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
            {
                uniqueNameAllList.Add(row["UniqueName"]);
            }
            List<string> uniqueNameList = uniqueNameAllList.Distinct().ToList();

            //Create frame list
            foreach (string uniqueName in uniqueNameList)
            {
                rcBeamList.Add(new src.HelperEtabs.Frame(uniqueName));
            }

            return rcBeamList;

        }

        public static void GetRcBeamDimension(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary ,List<src.HelperEtabs.Frame> rcBeamList)
        {
            //Get table
            List<Dictionary<string , string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                foreach(Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        string sectionPropName = row["DesignSect"];
                        double width = 0;
                        double height = 0;
                        double coverTop = 0;
                        double coverBottom = 0;

                        (width, height) = src.HelperEtabs.GetDimensionRectangle(Sap, sectionPropName);

                        rcBeam.Width = width;
                        rcBeam.Height = height;

                        (coverTop, coverBottom) = src.HelperEtabs.GetRcBeamCover(Sap, sectionPropName);

                        double d = height - Math.Max(coverTop, coverBottom);
                        rcBeam.d = d;

                        break;
                    }
                }
            }
        }

        public static void GetRcBeamRebarArea(cSapModel Sap, src.HelperEtabs.DatabaseTable rcBeamDesignSummary, 
            List<src.HelperEtabs.Frame> rcBeamList, double leftLength, double midLength, double rightLength)
        {
            //Get table
            List<Dictionary<string, string>> rcBeamDesignSummaryTable = rcBeamDesignSummary.TableInList;

            //Getting length for each beam
            foreach (src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                bool firstRun = true;
                double length = 0;

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        if (firstRun)
                        {
                            length = Convert.ToDouble(row["Station"]);
                            firstRun = false;
                        }
                        else
                        {
                            if (Convert.ToDouble(row["Station"]) > length)
                            {
                                length = Convert.ToDouble(row["Station"]);
                            }
                        }
                    }
                }
                rcBeam.Length = length;
            }

            //Getting rebar area for each beam
            foreach (src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                double leftStation = rcBeam.Length * leftLength;
                double rightStation = rcBeam.Length * (1-rightLength);

                double leftTopAs = 0;
                double leftBottomAs = 0;
                double midTopAs = 0;
                double midBottomAs = 0;
                double rightTopAs = 0;
                double rightBottomAs = 0;

                double leftAv = 0;
                double midAv = 0;
                double rightAv = 0;

                bool firstLeft = true;
                bool firstMid = true;
                bool firstRight = true;

                foreach (Dictionary<string, string> row in rcBeamDesignSummaryTable)
                {
                    if (row["UniqueName"] == rcBeam.UniqueName)
                    {
                        if (Convert.ToDouble(row["Station"]) <= leftStation)
                        {
                            if (firstLeft)
                            {
                                leftTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                leftBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                leftAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstLeft = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > leftTopAs)
                                {
                                    leftTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > leftBottomAs)
                                {
                                    leftBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > leftAv)
                                {
                                    leftAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }
                        }
                        else if (Convert.ToDouble(row["Station"]) >= rightStation)
                        {
                            if (firstRight)
                            {
                                rightTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                rightBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                rightAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstRight = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > rightTopAs)
                                {
                                    rightTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > rightBottomAs)
                                {
                                    rightBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > rightAv)
                                {
                                    rightAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }
                        }
                        else
                        {
                            if (firstMid)
                            {
                                midTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                midBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                midAv = (Convert.ToDouble(row["totTrnRebar"]));

                                firstMid = false;
                            }
                            else
                            {
                                if (Convert.ToDouble(row["totTopRebar"]) > midTopAs)
                                {
                                    midTopAs = (Convert.ToDouble(row["totTopRebar"]));
                                }

                                if (Convert.ToDouble(row["totBotRebar"]) > midBottomAs)
                                {
                                    midBottomAs = (Convert.ToDouble(row["totBotRebar"]));
                                }

                                if (Convert.ToDouble(row["totTrnRebar"]) > midAv)
                                {
                                    midAv = (Convert.ToDouble(row["totTrnRebar"]));
                                }
                            }

                        }
                    }
                }

                rcBeam.LeftBottomAs = leftBottomAs;
                rcBeam.LeftTopAs = leftTopAs;
                rcBeam.MidBottomAs = midBottomAs;
                rcBeam.MidTopAs = midTopAs;
                rcBeam.RightBottomAs = rightBottomAs;
                rcBeam.RightTopAs = rightTopAs;

                rcBeam.LeftAv = leftAv * 1000; //mm2/m
                rcBeam.MidAv = midAv * 1000; //mm2/m
                rcBeam.RightAv = rightAv * 1000; //mm2/m
            }

        }

        private static object[,] CompileResutRcBeamToArray(List<src.HelperEtabs.Frame> rcBeamList)
        {
            List<string> colTitles = new List<string>()
            {
                "UniqueName", "Width (mm)", "Height (mm)", "d (mm)", "Left Top As (mm2)", "Mid Top As (mm2)", "Right Top As (mm2)"
                , "Left Bottom As (mm2)", "Mid Bottom As (mm2)", "Right Bottom As (mm2)", "Left Av (mm2/m)", "Mid Av (mm2/m)", "Right Av (mm2/m)"
            };

            int numOfRow = rcBeamList.Count();
            int numOfCol = colTitles.Count();

            object[,] result = new object[numOfRow + 1,numOfCol];

            //First row
            int j = 0;
            foreach (string colTitle in colTitles)
            {
                result[0, j] = colTitle;
                j++;
            }

            int i = 1;
            foreach(src.HelperEtabs.Frame rcBeam in rcBeamList)
            {
                result[i, 0] = rcBeam.UniqueName;
                result[i, 1] = rcBeam.Width;
                result[i, 2] = rcBeam.Height;
                result[i, 3] = rcBeam.d;
                result[i, 4] = rcBeam.LeftTopAs;
                result[i, 5] = rcBeam.MidTopAs;
                result[i, 6] = rcBeam.RightTopAs;
                result[i, 7] = rcBeam.LeftBottomAs;
                result[i, 8] = rcBeam.MidBottomAs;
                result[i, 9] = rcBeam.RightBottomAs;
                result[i, 10] = rcBeam.LeftAv;
                result[i, 11] = rcBeam.MidAv;
                result[i, 12] = rcBeam.RightAv;

                i++;
            }


            return result;
        }

        public static void WriteResultRcBeamToExcel(object[,] result, string fileName, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);


            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


            int numOfRow = result.GetLength(0);
            int numOfCol = result.GetLength(1);

            string lastCellRange = alphabet[numOfCol - 1] + Convert.ToString(numOfRow - 1 + 1);

            Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.UsedRange.Range["A1", lastCellRange];

            rng.Value2 = result;

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
        }


    }
}
