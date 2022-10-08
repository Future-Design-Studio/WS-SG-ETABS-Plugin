using ETABSv1;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    internal class FunctionRCBSched
    {
        private static int Ret;

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

        public static bool GetRCBeamSchedule(cSapModel Sap, string fileName, string filePath)
        {
            bool isError = false;

            bool tableExist = CheckTableExistRCBSched(Sap);
            
            if (!tableExist)
            {
                isError = true;
                return isError;
            }

            //Get RC design result table
            List<Dictionary<string, string>> tableDesignSummary = new List<Dictionary<string, string>>();
            (isError, tableDesignSummary) = GetDesignResult(Sap);




            return isError;
        }

        public static (bool,  List<Dictionary<string, string>>) GetDesignResult(cSapModel Sap)
        {
            //Get RC design result table
            List<Dictionary<string, string>> tableInList = new List<Dictionary<string, string>>();
            bool isError;

            string designCode = "";
            Ret = Sap.DesignConcrete.GetCode(ref designCode);
            string tableKey = "Concrete Beam Design Summary - " + designCode;

            (isError, tableInList) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, false);

            return (isError, tableInList);
        }


    }
}
