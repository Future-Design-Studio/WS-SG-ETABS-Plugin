using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    internal class FrameGeometry
    {

        private string _name;
        private string _propName;
        private string _storyName;
        private Point3d _pt0;
        private Point3d _pt1;
        private double _angle;
        private int _cardinal;

        public FrameGeometry(string name, string propName, string storyName, Point3d pt0, Point3d pt1, double angle, int cardinal)
        {
            _name = name;
            _propName = propName;
            _storyName = storyName;
            _pt0 = pt0;
            _pt1 = pt1;
            _angle = angle;
            _cardinal = cardinal;
        }

        private string GetCardinalText() 
        {
            Dictionary<int, string> cardinalTextDict = new Dictionary<int, string>();
            cardinalTextDict.Add(1, "BottomLeft");
            cardinalTextDict.Add(2, "BottomCenter");
            cardinalTextDict.Add(3, "BottomRight");
            cardinalTextDict.Add(4, "MiddleLeft");
            cardinalTextDict.Add(5, "MiddleCenter");
            cardinalTextDict.Add(6, "MiddleRight");
            cardinalTextDict.Add(7, "TopLeft");
            cardinalTextDict.Add(8, "TopCenter");
            cardinalTextDict.Add(9, "TopRight");
            cardinalTextDict.Add(10, "Centroid");
            cardinalTextDict.Add(11, "ShearCenter");

            string cardinalText = cardinalTextDict[_cardinal];
            return cardinalText;
        }

        public string Name => _name;
        public string PropName => _propName;
        public string StoryName => _storyName;
        public Point3d Pt0 => _pt0;
        public Point3d Pt1 => _pt1;
        public double Angle => _angle;
        public string Cardinal => GetCardinalText();
       
    }
}
