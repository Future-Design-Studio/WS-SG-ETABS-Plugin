using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperExcel
    {
        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public static (bool, List<string>) ReadExcel(string excelFilePath, string workSheetName, bool? findExactMatch)
        {
            List<string> excelOutput = new List<string>();

            bool isError = false;

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@excelFilePath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            //Get all worksheets name
            List<string> workSheetNameList = new List<string>();

            //Get the worksheet that user want
            if (findExactMatch == true)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[workSheetName];
            }
            else if (findExactMatch == false)
            {
                foreach (Worksheet workSheet in xlWorkBook.Worksheets)
                {
                    if (workSheet.Name.Contains(workSheetName))
                    {
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[workSheet.Name];
                    }
                }
            }

            //If xlWorkSheet still null return error
            if (xlWorkSheet == null)
            {
                return (true, null);
            }

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;

            object[,] values = range.Value2;

            for (rCnt = 1; rCnt <= rw; rCnt++)
            {
                for (cCnt = 1; cCnt <= cl; cCnt++)
                {
                    string str = Convert.ToString(values[rCnt, cCnt]);
                    excelOutput.Add(str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return (isError, excelOutput);

        }

    }
}
