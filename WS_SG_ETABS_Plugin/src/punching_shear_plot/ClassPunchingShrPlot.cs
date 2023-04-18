using ScottPlot;
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
            public double ForceDividedByIncrement { get; set; }
            public double GroupIndex { get; set; }

            public JointPunchingShearDetails(string uniqueName, double xCoordinate, double yCoordinate, double zCoordinate)
            {
                UniqueName = uniqueName;
                XCoordinate = xCoordinate;
                YCoordinate = yCoordinate;
                ZCoordinate = zCoordinate;

                ForceList = new Dictionary<string, double>();
            }
        }

        public class GroupDefinition
        {
            public int GroupIndex { get; set; }
            public string SectProp { get; set; }
            public double MinForce { get; set; }
            public double MaxForce { get; set; }
            public string Color { get; set; }
            public List<JointPunchingShearDetails> JointPunchingShearDetailsList { get; set; }

            public GroupDefinition(int groupIndex, string sectProp, double minForce, double maxForce, List<JointPunchingShearDetails> jointPunchingShearDetailsList)
            {
                GroupIndex = groupIndex;
                SectProp = sectProp;
                MinForce = minForce;
                MaxForce = maxForce;

                JointPunchingShearDetailsList = jointPunchingShearDetailsList;
            }
        }
    }



}
