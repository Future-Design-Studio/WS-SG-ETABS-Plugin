using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    public class ClassWallDesign
    {
        public class WallDesignDetail
        {
            public string UniqueName { get; set; }
            public string Label { get; set; }
            public bool? IsFailAll { get; set; }
            public bool? IsFailPMN { get; set; }
            public bool? IsFailShear { get; set; }
            public List<HelperEtabs.Point3D> PointList { get; set; }
            public string StoryLabel { get; set; }
            public string PierLabel { get; set; }
            public string SpandrelLabel { get; set; }
            public string Group { get; set; }
            public double PierReinfPercent { get; set; }

            public WallDesignDetail(string uniqueName, string label, List<HelperEtabs.Point3D> pointList, string storyLabel, string pierLabel, string spandrelLabel, string group)
            {
                UniqueName = uniqueName;
                Label = label;
                PointList = pointList;
                StoryLabel = storyLabel;
                PierLabel = pierLabel;
                SpandrelLabel = spandrelLabel;
                Group = group;

                IsFailAll = null;
                IsFailPMN = null;
                IsFailShear = null;
                PierReinfPercent = -1;
            }
        }
    }
}
