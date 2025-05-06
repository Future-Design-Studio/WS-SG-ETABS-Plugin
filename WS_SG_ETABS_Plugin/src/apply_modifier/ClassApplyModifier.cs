using Accord.MachineLearning.Boosting;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
//using static WS_SG_ETABS_Plugin.src.pad_footing_plot.ClassPadFootPlot;

namespace WS_SG_ETABS_Plugin.src.apply_modifier
{
    public class ClassApplyModifier
    {
        public class ModifierDefinition
        {
            public string GroupName { get; set; }
            public double FrameModA { get; set; }
            public double FrameModV2 { get; set; }
            public double FrameModV3 { get; set; }
            public double FrameModT { get; set; }
            public double FrameModM2 { get; set; }
            public double FrameModM3 { get; set; }
            public double FrameModMass { get; set; }
            public double FrameModWeight { get; set; }
            public double AreaModF11 { get; set; }
            public double AreaModF22 { get; set; }
            public double AreaModF12 { get; set; }
            public double AreaModM11 { get; set; }
            public double AreaModM22 { get; set; }
            public double AreaModM12 { get; set; }
            public double AreaModV13 { get; set; }
            public double AreaModV23 { get; set; }
            public double AreaModMass { get; set; }
            public double AreaModWeight { get; set; }


            public ModifierDefinition(string groupName, double frameModA, double frameModV2, double frameModV3, double frameModT, double frameModM2,
                double frameModM3, double frameModMass, double frameModWeight, double areaModF11,
                double areaModF22, double areaModF12, double areaModM11, double areaModM22,
                double areaModM12, double areaModV13, double areaModV23, double areaModMass,
                double areaModWeight)
            {
                GroupName = groupName;

                FrameModA = frameModA;
                FrameModV2 = frameModV2;
                FrameModV3 = frameModV3;
                FrameModT = frameModT;
                FrameModM2 = frameModM2;
                FrameModM3 = frameModM3;
                FrameModMass = frameModMass;
                FrameModWeight = frameModWeight;

                AreaModF11 = areaModF11;
                AreaModF22 = areaModF22;
                AreaModF12 = areaModF12;
                AreaModM11 = areaModM11;
                AreaModM22 = areaModM22;
                AreaModM12 = areaModM12;
                AreaModV13 = areaModV13;
                AreaModV23 = areaModV23;
                AreaModMass = areaModMass;
                AreaModWeight = areaModWeight;
            }
        }
    }
}



