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
        private double _thickness;

        public AreaGeometry(string name, eAreaDesignOrientation designOrientation, List<Point3d> boundaryPoints, cSapModel Sap)
        {
            _name = name;
            _designOrientation = designOrientation;
            _boundaryPoints = boundaryPoints;

            var result1 = Sap.AreaObj.GetProperty(name, ref _propName);
            var result2 = Sap.AreaObj.GetOpening(name, ref _isOpening);
            GetThickness(Sap);
        }

        private void GetThickness(cSapModel Sap)
        {
            if(_designOrientation.ToString() == "Floor")
            {
                string Name = _propName;
                eSlabType SlabType = eSlabType.Slab;
                eDeckType DeckType = eDeckType.Filled;
                eShellType ShellType = eShellType.ShellThick;
                string MatProp = "";
                double Thickness = 0;
                int color = 0;
                string notes = "";
                string GUID = "";
                
                var return_val = Sap.PropArea.GetSlab(Name,
                                                    ref SlabType,
                                                    ref ShellType,
                                                    ref MatProp,
                                                    ref Thickness,
                                                    ref color,
                                                    ref notes,
                                                    ref GUID);
                if(return_val != 0)
                {
                    var return_val1 = Sap.PropArea.GetDeck(Name,
                                                            ref DeckType,
                                                            ref ShellType,
                                                            ref MatProp,
                                                            ref Thickness,
                                                            ref color,
                                                            ref notes,
                                                            ref GUID);
                }

                _thickness = Thickness;
            }

            if(_designOrientation.ToString() == "Wall")
            {
                string Name = _propName;
                eWallPropType WallPropType = eWallPropType.Specified;
                eShellType ShellType = eShellType.ShellThick;
                string MatProp = "";
                double Thickness = 0;
                int color = 0;
                string notes = "";
                string GUID = "";

                var return_val = Sap.PropArea.GetWall(Name,
                                                        ref WallPropType,
                                                        ref ShellType,
                                                        ref MatProp,
                                                        ref Thickness,
                                                        ref color,
                                                        ref notes,
                                                        ref GUID);

                _thickness= Thickness;
            }
        }


        public string Name => _name;
        public string PropName => _propName;
        public string DesignOrientation => _designOrientation.ToString();
        public List<Point3d> BoundaryPoints => _boundaryPoints;
        public bool IsOpening => _isOpening;
        public double Thickness => _thickness;
    }
}
