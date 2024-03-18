using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WS_SG_ETABS_Plugin.src.pad_footing_plot.ClassPadFootPlot;

namespace WS_SG_ETABS_Plugin.src.plot_drift
{
    internal class ClassDriftPlot
    {
        public class JointDisplacementDetail
        {
            public string UniqueName { get; set; }
            public double ZCoordinate { get; set; }
            public double Displacement { get; set; }
            public double Drift { get; set; }

            public JointDisplacementDetail(string uniqueName, double zCoord, double displacement)
            {
                UniqueName = uniqueName;
                ZCoordinate = zCoord;
                Displacement = displacement;
            }
        }
    }
}
