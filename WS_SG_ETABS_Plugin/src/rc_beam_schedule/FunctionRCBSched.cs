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
            List<Dictionary<string, string>> tableDesignSummary = new List<Dictionary<string, string>>();
            (isError, tableDesignSummary) = GetDesignResult(Sap);

            //Get list of beams
            GetBeamList(tableDesignSummary);









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

        public static List<src.HelperEtabs.Frame> GetBeamList(List<Dictionary<string, string>> tableDesignSummary)
        {
            List<src.HelperEtabs.Frame> frameList = new List<HelperEtabs.Frame>();

            //Get list of unique name
            List<string> uniqueNameAllList = new List<string>();

            foreach (Dictionary<string, string> row in tableDesignSummary)
            {
                uniqueNameAllList.Add(row["UniqueName"]);
            }
            List<string> uniqueNameList = uniqueNameAllList.Distinct().ToList();

            //Create frame list
            foreach (string uniqueName in uniqueNameList)
            {
                frameList.Add(new src.HelperEtabs.Frame(uniqueName));
            }


            return frameList;

        }




    }
}
