using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.wind_coeff
{
    internal class FunctionWindCoeff
    {
        public static bool AssignWindLoads(cSapModel SapModel, cPluginCallback Plugin, List<src.wind_coeff.ClassWindCoeff.WindLoad> windLoadList)
        {
            bool isError = false;

            foreach (var windLoad in windLoadList)
            {
                isError = src.HelperEtabs.Assignwindloads(SapModel, windLoad.GroupName, windLoad.LoadPattern, windLoad.Direction, windLoad.CP);
                if (isError)
                {
                    MessageBox.Show("Error, please let Mani know what happened");
                    return true;
                }
            }

            return false;
        }
    }
}
