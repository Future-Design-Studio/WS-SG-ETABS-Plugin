using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ETABSv1;
using Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.add_load_combo
{
    internal class FunctionAddLoadCombo
    {
        public static bool AddLoadCombo(cSapModel _SapModel, cPluginCallback _Plugin, int loadComboTypeInt, List<Dictionary<string, string>> loadAndFactorList,
            string loadComboName)
        {
            bool isError = false;
            int ret = 0;

            //Add new load combo
            ret = _SapModel.RespCombo.Add(loadComboName, loadComboTypeInt);

            if (ret != 0)
            {
                MessageBox.Show($"{loadComboName} already exist!");
                isError = true;
                return isError;
            }

            //Add load case inside combo
            foreach (Dictionary<string, string> loadAndFactor in loadAndFactorList)
            {
                string loadName = loadAndFactor["loadCase"];
                string factor = loadAndFactor["factor"];

                //List of load case
                int NumberNames = 0;
                string[] loadCaseList = null;
                ret = _SapModel.LoadCases.GetNameList(ref NumberNames, ref loadCaseList);

                //List of load combo
                NumberNames = 0;
                string[] loadComboList = null;
                ret = _SapModel.RespCombo.GetNameList(ref NumberNames, ref loadComboList);

                //Check if load is load case or load combo
                eCNameType CNameType = eCNameType.LoadCase;
                if (loadCaseList.Contains(loadName))
                {
                    CNameType = eCNameType.LoadCase;
                }
                else if (loadComboList.Contains(loadName))  
                {
                    CNameType = eCNameType.LoadCombo;
                }

                ret = _SapModel.RespCombo.SetCaseList(loadComboName, ref CNameType, loadName, Convert.ToInt64(factor));

                if (ret != 0)
                {
                    MessageBox.Show($"Error adding {loadName}!");
                    isError = true;
                    return isError;
                }
            }

            return isError;
        }
    }
}
