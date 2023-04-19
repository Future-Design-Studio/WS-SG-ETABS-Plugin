using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    internal class ClassPunchingShrPlot
    {
        public class JointPunchingShearDetails
        {
            public string UniqueName { get; set; }
            public double XCoordinate { get; set; }
            public double YCoordinate { get; set; }
            public double ZCoordinate { get; set; }

            public string ColumnIUniqueName { get; set; }
            public string ColumnJUniqueName { get; set; }
            public bool NotFoundColumnI { get; set; }
            public bool NotFoundColumnJ { get; set; }
            public Dictionary<string, double> ForceList { get; set; }
            public double ForceMaxDifference { get; set; }
            public string LoadCaseMaxDifference { get; set; }

            public string SectProp { get; set; }

            public JointPunchingShearDetails(string uniqueName, double xCoordinate, double yCoordinate, double zCoordinate)
            {
                UniqueName = uniqueName;
                XCoordinate = xCoordinate;
                YCoordinate = yCoordinate;
                ZCoordinate = zCoordinate;

                ForceList = new Dictionary<string, double>();
            }
        }

    }
}
