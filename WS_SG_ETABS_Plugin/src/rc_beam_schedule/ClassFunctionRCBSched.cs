using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    public class ClassFunctionRCBSched
    {

        public class RcBeamForMLInput
        {
            public string UniqueName { get; set; }
            public float Height { get; set; }
            public float Width { get; set; }
            public float LeftTopAs { get; set; }
            public float MidTopAs { get; set; }
            public float RightTopAs { get; set; }
            public float LeftBottomAs { get; set; }
            public float MidBottomAs { get; set; }
            public float RightBottomAs { get; set; }
            public float LeftAv { get; set; }
            public float RightAv { get; set; }

            public string Cluster { get; set; }
            public float PC1 { get; set; }
            public float PC2 { get; set; }

            public RcBeamForMLInput(string uniqueName, float height, float width, float leftTopAs, float midTopAs,
                float rightTopAs, float leftBottomAs, float midBottomAs, float rightBottomAs, float leftAv, float rightAv)
            {
                UniqueName = uniqueName;
                Height = height;
                Width = width;
                LeftTopAs = leftTopAs;
                MidTopAs = midTopAs;
                RightTopAs = rightTopAs;
                LeftBottomAs = leftBottomAs;
                MidBottomAs = midBottomAs;
                RightBottomAs = rightBottomAs;
                LeftAv = leftAv;
                RightAv = rightAv;
            }


        }

        public class RcBeamForMLOutput
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedClusterId;

            [ColumnName("Score")]
            public float[] Distances;
        }

        public class PCAResult
        {
            public float[] PCAFeatures { get; set; }
        }
    }
}
