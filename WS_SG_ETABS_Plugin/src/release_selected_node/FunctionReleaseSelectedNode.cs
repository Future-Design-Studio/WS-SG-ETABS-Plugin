using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.release_selected_node
{
    internal class FunctionReleaseSelectedNode
    {
        public static bool ReleaseSelectedNodes(cSapModel SapModel, cPluginCallback Plugin)
        {
            bool isError = false;

            foreach (var windLoad in windLoadList)
            {
                isError = src.HelperEtabs.AssignWindLoads(SapModel, windLoad.GroupName, windLoad.LoadPattern, windLoad.Direction, windLoad.CP);
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
