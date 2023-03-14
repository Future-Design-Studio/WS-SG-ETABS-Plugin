using ETABSv1;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    internal class AreaGeometry
    {
        
        private string _name;
        private string _propName;
        private eAreaDesignOrientation _designOrientation;
        private List<Point3d> _boundaryPoints;
        private bool _isOpening;

        public AreaGeometry(string name, eAreaDesignOrientation designOrientation, List<Point3d> boundaryPoints, cSapModel Sap)
        {
            _name = name;
            _designOrientation = designOrientation;
            _boundaryPoints = boundaryPoints;

            var result1 = Sap.AreaObj.GetProperty(name, ref _propName);
            var result2 = Sap.AreaObj.GetOpening(name, ref _isOpening);
        }


        public string Name => _name;
        public string PropName => _propName;
        public string DesignOrientation => _designOrientation.ToString();
        public List<Point3d> BoundaryPoints => _boundaryPoints;
        public bool IsOpening => _isOpening;
    }
}
