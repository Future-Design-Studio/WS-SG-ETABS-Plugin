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
                bool isError = false;
                string endIname = "";
                string endJname = "";
                List<ClassReleaseSelectedNode.ReleaseApplication> applyreleaseList = new List<ClassReleaseSelectedNode.ReleaseApplication>();
                string groupNameFrame = release.GroupNameFramesToAssign;
                string groupNameJoint = release.GroupNameJoints;
                List<string> releaseType = release.ReleaseType;

                List<string> frameUniqueName = src.HelperEtabs.GetGroupFrameElement(SapModel, groupNameFrame);
                List<string> jointUniqueName = src.HelperEtabs.GetGroupJointElement(SapModel, groupNameJoint);

                foreach (string framename in frameUniqueName)
                {
                    (endIname, endJname, isError) = src.HelperEtabs.GetFramePointsUniqueName(SapModel, framename);

                    if (jointUniqueName.Contains(endIname) && jointUniqueName.Contains(endJname))
                    {
                        ClassReleaseSelectedNode.ReleaseApplication eachRelease = new ClassReleaseSelectedNode.ReleaseApplication(framename, releaseType, true, true);
                        applyreleaseList.Add(eachRelease);
                    }
                    else if (jointUniqueName.Contains(endIname))
                    {
                        ClassReleaseSelectedNode.ReleaseApplication eachRelease = new ClassReleaseSelectedNode.ReleaseApplication(framename, releaseType, true, false);
                        applyreleaseList.Add(eachRelease);
                    }
                    else if (jointUniqueName.Contains(endJname))
                    {
                        ClassReleaseSelectedNode.ReleaseApplication eachRelease = new ClassReleaseSelectedNode.ReleaseApplication(framename, releaseType, false, true);
                        applyreleaseList.Add(eachRelease);
                    }
                    else
                    {
                        ClassReleaseSelectedNode.ReleaseApplication eachRelease = new ClassReleaseSelectedNode.ReleaseApplication(framename, releaseType, false, false);
                        applyreleaseList.Add(eachRelease);
                    }
                }

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
