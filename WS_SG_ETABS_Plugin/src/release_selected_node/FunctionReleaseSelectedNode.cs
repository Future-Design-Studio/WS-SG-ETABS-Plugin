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
        public static bool ReleaseSelectedNodes(cSapModel SapModel, cPluginCallback Plugin,
            List<src.release_selected_node.ClassReleaseSelectedNode.ReleaseDetails> releaseList)
        {
            foreach (var release in releaseList)
            {
                List<ClassReleaseSelectedNode.ReleaseApplication> applyreleaseList = new List<ClassReleaseSelectedNode.ReleaseApplication>();
                string groupNameFrame = release.GroupNameFramesToAssign;
                string groupNameJoint = release.GroupNameJoints;
                List<string> releaseType = release.ReleaseType;




                bool isError = false;
                foreach (var releaseapplication in applyreleaseList)
                {
                    isError = src.HelperEtabs.AssignFrameReleases(SapModel, releaseapplication.UniqueName, releaseapplication.ReleaseType, releaseapplication.endirelease, releaseapplication.endjrelease);
                    if (isError)
                    {
                        MessageBox.Show("Error, please try again after changing releases accordingly in Release group of frames " + groupNameFrame + " at group of nodes from " + groupNameJoint + ".");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
