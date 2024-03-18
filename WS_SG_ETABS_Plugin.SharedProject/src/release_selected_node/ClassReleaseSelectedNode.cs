using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.release_selected_node
{
    public class ClassReleaseSelectedNode
    {
        public class ReleaseDetails
        {
            public string GroupNameFramesToAssign { get; set; }

            public string GroupNameJoints { get; set; }

            public List<string> ReleaseType { get; set; }

            public ReleaseDetails(string groupNameFramesToAssign, string groupNameJoints, List<string> releaseType)
            {
                GroupNameFramesToAssign = groupNameFramesToAssign;
                GroupNameJoints = groupNameJoints;
                ReleaseType = releaseType;
            }
        }

        public class ReleaseApplication
        {
            public string UniqueName { get; set; }

            public List<string> ReleaseType { get; set; }

            public bool endirelease { get; set; }

            public bool endjrelease { get; set; }

            public ReleaseApplication(string uniqueName, List<string> releaseType, bool endirelease, bool endjrelease)
            {
                UniqueName = uniqueName;
                ReleaseType = releaseType;
                this.endirelease = endirelease;
                this.endjrelease = endjrelease;
            }
        }

    }
}
