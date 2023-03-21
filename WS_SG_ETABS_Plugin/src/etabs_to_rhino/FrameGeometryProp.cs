using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    internal class FrameGeometryProp
    {
        
        private string _name;
        private eFramePropType _propType;
        private double _t3;
        private double _t2;
        private double _tf;
        private double _tw;
        private double _t2b;
        private double _tfb;
        private double _area;

        public FrameGeometryProp(string name, eFramePropType propType, double t3, double t2, double tf, double tw, double t2b, double tfb, double area)
        {
            _name = name;
            _propType = propType;
            _t3 = t3;
            _t2 = t2;
            _tf = tf;
            _tw = tw;
            _t2b = t2b;
            _tfb = tfb;
            _area = area;
        }


        public string Name => _name;
        public string PropType => _propType.ToString();
        public double T3 => _t3;
        public double T2 => _t2;
        public double Tf => _tf;
        public double Tw => _tw;
        public double T2b => _t2b;
        public double Tfb => _tfb;
        public double Area => _area;

    }
}
