using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WS_SG_ETABS_Plugin.src.punching_shear_plot.ClassPunchingShrPlot;

namespace WS_SG_ETABS_Plugin.src.pad_footing_plot
{
    internal class ClassPadFootPlot
    {
        public class JointPadFootDetails
        {
            public string UniqueName { get; set; }
            public double XCoordinate { get; set; }
            public double YCoordinate { get; set; }
            public double ZCoordinate { get; set; }

            public Dictionary<string, double> ForceList { get; set; }
            public double ForceMax { get; set; }
            public string LoadCaseMax { get; set; }
            public double GroupIndex { get; set; }
            public bool IsTension { get; set; }
            public bool IsNotValid { get; set; }
            public double RequiredArea { get; set; }
            public double WidthDividedByIncrement { get; set; }
            public double Width { get; set; }
            public double Area { get; set; }
            public double UtilizationRatio { get; set; }

            public JointPadFootDetails(string uniqueName, double xCoordinate, double yCoordinate, double zCoordinate)
            {
                UniqueName = uniqueName;
                XCoordinate = xCoordinate;
                YCoordinate = yCoordinate;
                ZCoordinate = zCoordinate;

                IsTension = false;
                IsNotValid = false;

                ForceList = new Dictionary<string, double>();
            }
        }

        public class GroupDefinition
        {
            public int GroupIndex { get; set; }
            public double Width { get; set; }
            public double Area { get; set; }
            public string Color { get; set; }
            public List<JointPadFootDetails> JointPunchingShearDetailsList { get; set; }

            public GroupDefinition(int groupIndex, double width, List<JointPadFootDetails> jointPunchingShearDetailsList)
            {
                GroupIndex = groupIndex;
                Width = width;
                Area = width * width;

                JointPunchingShearDetailsList = jointPunchingShearDetailsList;
            }
        }
    }
}
