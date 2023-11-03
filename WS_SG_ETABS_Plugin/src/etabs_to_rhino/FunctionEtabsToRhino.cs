using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml;
using ETABSv1;
using Rhino.FileIO;
using Rhino.Geometry;


namespace WS_SG_ETABS_Plugin.src.etabs_to_rhino
{
    internal class FunctionEtabsToRhino
    {

        #region Input Data

        

        #endregion

        public static List<AreaGeometry> GetAllAreasWithProperties(cSapModel Sap)
        {
            //pull area data from etabs
            int NumberNames = 0;
            string[] MyName = null;
            eAreaDesignOrientation[] DesignOrientaion = null;
            int NumberBoundaryPts = 0;
            int[] PointDelimiter = null;
            string[] PointNames = null;
            double[] PointX = null;
            double[] PointY = null;
            double[] PointZ = null;

            int returnValue = Sap.AreaObj.GetAllAreas(ref NumberNames,
                                                            ref MyName,
                                                            ref DesignOrientaion,
                                                            ref NumberBoundaryPts,
                                                            ref PointDelimiter,
                                                            ref PointNames,
                                                            ref PointX,
                                                            ref PointY,
                                                            ref PointZ);


            //check if result is empty
            if (MyName == null)
            {
                return null;
            }

            //new variables
            int counter = 0;
            List<Point3d> ptsPerArea = new List<Point3d>();
            List<List<Point3d>> ptsGroup = new List<List<Point3d>>();

            var allAreas = new List<AreaGeometry>();

            //get list of point3d
            var allPoints = new List<Point3d>();
            for (var i=0; i<PointX.Length; i++)
            {
                allPoints.Add(new Point3d(PointX[i], PointY[i], PointZ[i]));
            }


            for (int i = 0; i < PointX.Length; i++)
            {
                int delimeter = PointDelimiter[counter];
                var newPt = allPoints[i];

                if (i != delimeter)
                {
                    ptsPerArea.Add(newPt);
                }
                else 
                {
                    ptsPerArea.Add(newPt);

                    var ptsList = ptsPerArea.ConvertAll(a => a);
                    ptsGroup.Add(ptsList);

                    counter++;
                    ptsPerArea.Clear();
                }
            }

            //get area cardinal
            bool isError = false;
            HelperEtabs.DatabaseTable databaseTable = new HelperEtabs.DatabaseTable();
            (isError,databaseTable) = HelperEtabs.GetTableDisplay(Sap, "Area Assignments - Insertion Point", false);

            var cardinaldict = new Dictionary<string, string>();
            foreach (Dictionary<string,string> row in databaseTable.TableInList)
            {
                var name = row["UniqueName"];
                var cardinal = row["CardinalPt"];
                cardinaldict[name] = cardinal;
            }


            for (var i = 0; i < NumberNames; i++)
            {
                var area = new AreaGeometry(MyName[i], DesignOrientaion[i], ptsGroup[i], cardinaldict[MyName[i]], Sap);
                if (area.DesignOrientation != "Null") allAreas.Add(area);
            }


            return allAreas;
        }
        public static List<FrameGeometry> GetAllFrames(cSapModel Sap)
        {
            //pull frame data from etabs
            int NumberNames = 0;
            string[] MyName = null;
            string[] PropName = null;  
            string[] StoryName = null;
            string[] PointName1 = null;
            string[] PointName2 = null;
            double[] Point1X = null;
            double[] Point1Y = null;
            double[] Point1Z = null;
            double[] Point2X = null;
            double[] Point2Y = null;
            double[] Point2Z = null;
            double[] Angle = null;
            double[] Offset1X = null;
            double[] Offset2X = null;
            double[] Offset1Y = null;
            double[] Offset2Y = null;
            double[] Offset1Z = null;
            double[] Offset2Z = null;
            int[] CardinalPoint = null;
            string csys = "Global";

            int returnValue = Sap.FrameObj.GetAllFrames(ref NumberNames,
                                                            ref MyName,
                                                            ref PropName,
                                                            ref StoryName,
                                                            ref PointName1,
                                                            ref PointName2,
                                                            ref Point1X,
                                                            ref Point1Y,
                                                            ref Point1Z,
                                                            ref Point2X,
                                                            ref Point2Y,
                                                            ref Point2Z,
                                                            ref Angle,
                                                            ref Offset1X,
                                                            ref Offset2X,
                                                            ref Offset1Y,
                                                            ref Offset2Y,
                                                            ref Offset1Z,
                                                            ref Offset2Z,
                                                            ref CardinalPoint,
                                                            csys);
            
            //check if result is empty
            if (MyName == null)
            {
                return null;
            }
            
            
            //new variables
            List<Point3d> pt0 = new List<Point3d>();
            List<Point3d> pt1 = new List<Point3d>();
            //List<Point3d> offset0 = new List<Point3d>();
            //List<Point3d> offset1 = new List<Point3d>();
            
            //start and end points of frames
            for (int i=0; i<Point1X.Length; i++)
            {
                double pt0_x = Point1X[i];
                double pt0_y = Point1Y[i];
                double pt0_z = Point1Z[i];
                pt0.Add(new Point3d(pt0_x, pt0_y, pt0_z));

                double pt1_x = Point2X[i];
                double pt1_y = Point2Y[i];
                double pt1_z = Point2Z[i];
                pt1.Add(new Point3d(pt1_x, pt1_y, pt1_z));
            }

            //offsets
            //for (int i = 0; i < Point1X.Length; i++)
            //{
            //    double pt0_x = Offset1X[i];
            //    double pt0_y = Offset1Y[i];
            //    double pt0_z = Offset1Z[i];
            //    pt0.Add(new Point3d(pt0_x, pt0_y, pt0_z));

            //    double pt1_x = Offset2X[i];
            //    double pt1_y = Offset2Y[i];
            //    double pt1_z = Offset2Z[i];
            //    pt1.Add(new Point3d(pt1_x, pt1_y, pt1_z));
            //}

            
            var allFrame = new List<FrameGeometry>();

            for (var i = 0; i<NumberNames; i++)
            {
                var frame = new FrameGeometry(MyName[i], PropName[i], StoryName[i], pt0[i], pt1[i], Angle[i], CardinalPoint[i], Sap);
                if (frame.DesignOrientation != "Null") allFrame.Add(frame);
            }

            return allFrame;
        }
        public static Dictionary<string,FrameGeometryProp> GetAllFrameProperties(cSapModel Sap)
        {
            //pull frame properties data from etabs
            int NumberNames = 0;
            string[] MyName = null;
            eFramePropType[] PropType = null;
            double[] t3 = null;
            double[] t2 = null;
            double[] tf = null;
            double[] tw = null;
            double[] t2b = null;
            double[] tfb = null;
            double[] Area = null;

            int returnValue = Sap.PropFrame.GetAllFrameProperties_2(ref NumberNames,
                                                                        ref MyName,
                                                                        ref PropType,
                                                                        ref t3,
                                                                        ref t2,
                                                                        ref tf,
                                                                        ref tw,
                                                                        ref t2b,
                                                                        ref tfb,
                                                                        ref Area);

            //check if result is empty
            if (MyName == null)
            {
                return null;
            }

            //var allFrameProp = new List<FrameGeometryProp>();
            var allFrameProp = new Dictionary<string,FrameGeometryProp>();

            for (var i = 0; i < NumberNames; i++)
            {
                var frameProp = new FrameGeometryProp(MyName[i], PropType[i], t3[i], t2[i], tf[i], tw[i], t2b[i], tfb[i], Area[i]);
                allFrameProp.Add(MyName[i], frameProp);
            }


            return allFrameProp;
        }
        public static void WriteRhinoFile(Rhino.FileIO.File3dm file3dm ,string path)
        {
            Rhino.FileIO.File3dmWriteOptions options = new Rhino.FileIO.File3dmWriteOptions();
            options.Version = 0;
            options.SaveUserData = true;
            bool bake = file3dm.Write(path,options);
            if (bake)
            {
                Console.WriteLine("Export Completed Successfully");
            }
            else
            {
                Console.WriteLine("Export Unsuccess");
            }
            
        }

        #region WIP - EXTRUSION MODEL - DEPLOYED WITH LIMITED CAPABILITIES

        public static List<Curve> CreateFrameProfile(FrameGeometryProp frameProp)
        {
            var output = new List<Curve>();

            if (frameProp.PropType == "Circle")
            {
                double diameter = Convert.ToDouble(frameProp.T3);

                var outerCircle = new Circle(diameter / 2);

                output.Add(outerCircle.ToNurbsCurve());
            }

            else if (frameProp.PropType == "Pipe")
            {
                double diameter = Convert.ToDouble(frameProp.T3);
                double thickness = Convert.ToDouble(frameProp.Tw);

                var outerCircle = new Circle(diameter / 2);
                var innerCircle = new Circle(diameter / 2 - thickness);

                output.Add(outerCircle.ToNurbsCurve());
                output.Add(innerCircle.ToNurbsCurve());
            }

            else if (frameProp.PropType == "Rectangular")
            {
                double depth = Convert.ToDouble(frameProp.T3);
                double width = Convert.ToDouble(frameProp.T2);

                var pts = new List<Point3d>();
                pts.Add(new Point3d(width / -2, depth / 2, 0));
                pts.Add(new Point3d(width / 2, depth / 2, 0));
                pts.Add(new Point3d(width / 2, depth / -2, 0));
                pts.Add(new Point3d(width / -2, depth / -2, 0));
                pts.Add(new Point3d(width / -2, depth / 2, 0));
                pts.Reverse();
                var rect = new Polyline(pts);

                output.Add(rect.ToPolylineCurve());
            }

            else if (frameProp.PropType == "I")
            {
                double depth = Convert.ToDouble(frameProp.T3);
                double width = Convert.ToDouble(frameProp.T2);
                double tf = Convert.ToDouble(frameProp.Tf);
                double tw = Convert.ToDouble(frameProp.Tw);

                var pts = new List<Point3d>();
                pts.Add(new Point3d(width / -2, depth / 2, 0));
                pts.Add(new Point3d(width / 2, depth / 2, 0));
                pts.Add(new Point3d(width / 2, depth / 2 - tf, 0));
                pts.Add(new Point3d(tw / 2, depth / 2 - tf, 0));
                pts.Add(new Point3d(tw / 2, depth / -2 + tf, 0));
                pts.Add(new Point3d(width / 2, depth / -2 + tf, 0));
                pts.Add(new Point3d(width / 2, depth / -2, 0));
                pts.Add(new Point3d(width / -2, depth / -2, 0));
                pts.Add(new Point3d(width / -2, depth / -2 + tf, 0));
                pts.Add(new Point3d(tw / -2, depth / -2 + tf, 0));
                pts.Add(new Point3d(tw / -2, depth / 2 - tf, 0));
                pts.Add(new Point3d(width / -2, depth / 2 - tf, 0));
                pts.Add(new Point3d(width / -2, depth / 2, 0));
                pts.Reverse();
                var rect = new Polyline(pts);

                output.Add(rect.ToPolylineCurve());
            }

            else if (frameProp.PropType == "Box")
            {
                double depth = Convert.ToDouble(frameProp.T3);
                double width = Convert.ToDouble(frameProp.T2);
                double tf = Convert.ToDouble(frameProp.Tf);
                double tw = Convert.ToDouble(frameProp.Tw);

                var outerProfilePts = new List<Point3d>();
                outerProfilePts.Add(new Point3d(width / -2, depth / 2, 0));
                outerProfilePts.Add(new Point3d(width / 2, depth / 2, 0));
                outerProfilePts.Add(new Point3d(width / 2, depth / -2, 0));
                outerProfilePts.Add(new Point3d(width / -2, depth / -2, 0));
                outerProfilePts.Add(new Point3d(width / -2, depth / 2, 0));
                outerProfilePts.Reverse();
                var outerProfile = new Polyline(outerProfilePts);

                var innerProfilePts = new List<Point3d>();
                innerProfilePts.Add(new Point3d(width / -2 + tw, depth / 2 - tf, 0));
                innerProfilePts.Add(new Point3d(width / 2 - tw, depth / 2 - tf, 0));
                innerProfilePts.Add(new Point3d(width / 2 - tw, depth / -2 + tf, 0));
                innerProfilePts.Add(new Point3d(width / -2 + tw, depth / -2 + tf, 0));
                innerProfilePts.Add(new Point3d(width / -2 + tw, depth / 2 - tf, 0));
                innerProfilePts.Reverse();
                var innerProfile = new Polyline(innerProfilePts);

                output.Add(outerProfile.ToPolylineCurve());
                output.Add(innerProfile.ToPolylineCurve());
            }

            return output;
        }
        public static List<Brep> CreateFrameExtrusion(List<Curve> profile, FrameGeometryProp frameProp, double length)
        {
            var outputFrameBreps = new List<Brep>();

            if (frameProp.PropType == "Circle" ||
                frameProp.PropType == "Rectangular" ||
                frameProp.PropType == "I")
            {
                var extrusion = Extrusion.Create(profile[0], length, true);
                outputFrameBreps.Add(extrusion.ToBrep());
            }

            /*
            else if (frameProp.PropType == "Pipe")
            {
                var outercircle = new Circle();
                var try_outercircle = profile[0].TryGetCircle(out outercircle);
                var innercircle = new Circle();
                var try_innercircle = profile[1].TryGetCircle(out innercircle);
                var other_radius = innercircle.Radius;

                var outer_cylinder = new Cylinder(outercircle, length);
                var extrusion = Extrusion.CreatePipeExtrusion(outer_cylinder, other_radius, true, true);

                outputFrameBreps.Add(extrusion.ToBrep());
            }
            */

            else if (frameProp.PropType == "Box" ||
                        frameProp.PropType == "Pipe") //NOT TESTED WITH ETABS YET, TESTED IN GRASSHHOPPER. IF IT WORKS CAN BE USED FOR "Pipe" TOO.
            {
                var outerProfile = profile[0];
                var innerProfile = profile[1];

                var extrusion = new Extrusion();
                extrusion.SetOuterProfile(outerProfile, true);
                extrusion.AddInnerProfile(innerProfile);
                extrusion.SetPathAndUp(new Point3d(0, 0, 0), new Point3d(0, 0, length), Vector3d.YAxis);

                outputFrameBreps.Add(extrusion.ToBrep());
            }

            
            return outputFrameBreps;
        }
        public static void IncludeFrameExtrusion(Rhino.FileIO.File3dm file3dm, List<FrameGeometry> allFrame, Dictionary<string, FrameGeometryProp> allFrameProp, List<AreaGeometry> allAreas)
        {
            ////////////////////////
            // CREATE PROFILE FOR EACH FRAME PROPERTY
            ////////////////////////

            var available_frametype = new List<string> { "Circle", "Pipe", "Rectangular", "I", "Box" };

            var frameprop_profile = new Dictionary<string, List<Curve>>();

            foreach (var keyVal in allFrameProp)
            {
                var frameprop = keyVal.Value;
                if (available_frametype.Contains(frameprop.PropType))
                {
                    var profile_crv = CreateFrameProfile(frameprop);
                    frameprop_profile.Add(frameprop.Name, profile_crv);
                }
            }


            ////////////////////////
            // CREATE EXTRUSION FOR ALL FRAME OBJECTS
            ////////////////////////
            var outputPlanes = new List<Plane>();
            var outputLines = new List<LineCurve>();
            var outputFrameBreps = new List<Brep>();

            foreach (var frame in allFrame)
            {
                var frameprop = allFrameProp[frame.PropName];

                if (available_frametype.Contains(frameprop.PropType)) //filter for frame type
                {
                    var profile = frameprop_profile[frameprop.Name];

                    // create orientation transformation
                    var pt0 = frame.Pt0;
                    var pt1 = frame.Pt1;
                    var frame_vec = pt1 - pt0;

                    var plane1_x = Vector3d.CrossProduct(Vector3d.ZAxis, frame_vec);
                    var plane1_y = Vector3d.CrossProduct(frame_vec, plane1_x);

                    var plane0 = Plane.WorldXY;
                    var plane0_moveup = new Plane(new Point3d(0, 0, frame_vec.Length), Vector3d.XAxis, Vector3d.YAxis);
                    var plane1 = new Plane(pt0, plane1_x, plane1_y);

                    var angle = Rhino.RhinoMath.ToRadians(Convert.ToDouble(frame.Angle));


                    // vertical frame special case
                    if (pt1.X == pt0.X && pt1.Y == pt0.Y)
                    {
                        plane1 = new Plane(pt0, frame_vec);
                        angle -= Math.PI / 2; //etabs default depth is parallel to World X-Axis for vertical frame
                    }

                    outputPlanes.Add(plane1);

                    var t0 = Transform.Rotation(angle, Point3d.Origin);
                    var t1 = Transform.PlaneToPlane(plane0, plane1);

                    // create geometry -- NOT READY NEED FIX, resulting geometry currently is not a solid
                    var extrusions = CreateFrameExtrusion(profile, frameprop, frame_vec.Length);

                    foreach(var geo in extrusions)
                    {
                        if (frame.Cardinal == "TopCenter")
                        {
                            var vec = Vector3d.YAxis * frameprop.T3 / -2;
                            var tf = Transform.Translation(vec);
                            geo.Transform(tf);
                        }

                        geo.Transform(t0);
                        geo.Transform(t1);
                        outputFrameBreps.Add(geo);
                    }
                    
                    
                }
            }


            


            ////////////////////////
            // ADD EXTRUSION TO RHINO MODEL INSTANCE
            ////////////////////////

            int frameExtrusionObj = file3dm.AllLayers.AddLayer("frameExtrusion", System.Drawing.Color.Blue);

            var att = new Rhino.DocObjects.ObjectAttributes();
            att.LayerIndex = frameExtrusionObj;

            foreach (var obj in outputFrameBreps)
            {
                file3dm.Objects.AddBrep(obj, att);
            }
            
        }
        public static void IncludeAreaExtrusion(Rhino.FileIO.File3dm file3dm,  List<AreaGeometry> allAreas)
        {
            ////////////////////////
            // CREATE EXTRUSION FOR ALL AREA OBJECTS
            ////////////////////////
            var outputWallBreps = new List<Brep>();
            var outputFloorBreps = new List<Brep>();

            for (var i = 0; i < allAreas.Count; i++)
            {
                var area = allAreas[i];

                var polyline = new Polyline();

                for (var j = 0; j < area.BoundaryPoints.Count; j++)
                {
                    var pt = area.BoundaryPoints[j];
                    polyline.Add(pt);                    
                }

                var lastPt = area.BoundaryPoints[0];
                polyline.Add(lastPt);

                var center = polyline.CenterPoint();

                var plane0 = new Plane(area.BoundaryPoints[0], area.BoundaryPoints[1], center);
                
                var t0 = Transform.PlanarProjection(plane0);
                polyline.Transform(t0);

                if (area.DesignOrientation == "Wall")
                {
                    var wallExtrusion = Extrusion.Create(polyline.ToPolylineCurve(), area.Thickness, true);

                    var t1 = Transform.Translation(plane0.Normal * area.Thickness / -2);

                    wallExtrusion.Transform(t1);

                    outputWallBreps.Add(wallExtrusion.ToBrep());
                }

                if(area.DesignOrientation == "Floor")
                {
                    var vec = plane0.Normal;

                    var crv = polyline.ToPolylineCurve();
                    var orientation = crv.ClosedCurveOrientation(Vector3d.ZAxis);

                    if(orientation == CurveOrientation.CounterClockwise) crv.Reverse();

                    var floorExtrusion = Extrusion.Create(crv, area.Thickness, true);

                    outputFloorBreps.Add(floorExtrusion.ToBrep());
                }
                
            }
            

            ////////////////////////
            // ADD EXTRUSION TO RHINO MODEL INSTANCE
            ////////////////////////

            int wallExtrusionObj = file3dm.AllLayers.AddLayer("wallExtrusion", System.Drawing.Color.DarkGray);

            var att0 = new Rhino.DocObjects.ObjectAttributes();
            att0.LayerIndex = wallExtrusionObj;

            foreach (var obj in outputWallBreps)
            {
                file3dm.Objects.AddBrep(obj, att0);
            }

            int floorExtrusionObj = file3dm.AllLayers.AddLayer("floorExtrusion", System.Drawing.Color.Gray);

            var att1 = new Rhino.DocObjects.ObjectAttributes();
            att1.LayerIndex = floorExtrusionObj;

            foreach (var obj in outputFloorBreps)
            {
                file3dm.Objects.AddBrep(obj, att1);
            }
        }

        #endregion

        public static void ExportEtabsToRhino(cSapModel Sap, string filePath, string fileName, Dictionary<string, bool?> selectedItems, IProgress<int> progress)
        {
            //setup new rhino model instance
            Console.WriteLine("Setting up new Rhino model...");


            var newModel = new Rhino.FileIO.File3dm();
            newModel.Settings.ModelUnitSystem = Rhino.UnitSystem.Meters;
            newModel.Settings.ModelAbsoluteTolerance = 0.001;

            int layerFrameObj = newModel.AllLayers.AddLayer("frameObj", System.Drawing.Color.Black);
            int layerAreaObj = newModel.AllLayers.AddLayer("areaObj", System.Drawing.Color.Red);


            //rebuild etabs frame objects 
            //attach etabs property as user text in thino
            var allFrames = GetAllFrames(Sap);
            var allFrameProp = GetAllFrameProperties(Sap);

            progress.Report(20);

            if (allFrames != null && allFrameProp != null) 
            {
                for (var i = 0; i < allFrames.Count; i++)
                {

                    var frame = allFrames[i];
                    string framePropName = frame.PropName;

                    //if (allFrameProp.ContainsKey(framePropName) == false) continue;

                    FrameGeometryProp frameProp = allFrameProp[framePropName];


                    var att = new Rhino.DocObjects.ObjectAttributes();

                    att.SetUserString("Name", frame.Name);
                    att.SetUserString("PropName", frame.PropName);
                    att.SetUserString("StoryName", frame.StoryName);
                    att.SetUserString("Angle", frame.Angle.ToString());
                    att.SetUserString("Cardinal", frame.Cardinal);
                    att.SetUserString("DesignOrientation", frame.DesignOrientation);

                    att.SetUserString("t3", frameProp.T3.ToString());
                    att.SetUserString("t2", frameProp.T2.ToString());
                    att.SetUserString("tf", frameProp.Tf.ToString());
                    att.SetUserString("tw", frameProp.Tw.ToString());
                    att.SetUserString("t2b", frameProp.T2b.ToString());
                    att.SetUserString("tfb", frameProp.Tfb.ToString());
                    att.SetUserString("PropType", frameProp.PropType);


                    att.LayerIndex = layerFrameObj;


                    newModel.Objects.AddLine(frame.Pt0, frame.Pt1, att);
                }
            }
            

            progress.Report(30);



            //rebuild etabs area objects in rhino
            //attach etabs property as user text in rhino
            var allAreas = GetAllAreasWithProperties(Sap);

            progress.Report(70);

            if (allAreas != null)
            {
                for (var i = 0; i < allAreas.Count; i++)
                {
                    var area = allAreas[i];

                    var att = new Rhino.DocObjects.ObjectAttributes();

                    att.SetUserString("Name", area.Name);
                    att.SetUserString("PropName", area.PropName);
                    att.SetUserString("DesignOrientation", area.DesignOrientation);
                    att.SetUserString("IsOpening", area.IsOpening.ToString());
                    att.SetUserString("Thickness", area.Thickness.ToString());
                    att.SetUserString("Cardinal", area.Cardinal);

                    att.LayerIndex = layerAreaObj;


                    var polyline = new Polyline();

                    for (var j = 0; j < area.BoundaryPoints.Count; j++)
                    {
                        var pt = area.BoundaryPoints[j];
                        polyline.Add(pt);
                    }

                    var lastPt = area.BoundaryPoints[0];
                    polyline.Add(lastPt);

                    newModel.Objects.AddPolyline(polyline, att);
                }
            }
            

            progress.Report(90);


            bool includeFrameExtrusion = selectedItems["extrusionFrameModel"] ?? false;
            bool includeAreaExtrusion = selectedItems["extrusionAreaModel"] ?? false;

            if (includeFrameExtrusion && allFrames != null && allFrameProp != null)
            {
                IncludeFrameExtrusion(newModel, allFrames, allFrameProp, allAreas);
            }

            if (includeAreaExtrusion && allAreas != null)
            {
                IncludeAreaExtrusion(newModel, allAreas);
            }


            var outputModelPath = filePath;
            var outputModelName = fileName;
            var rhinoModelPath = System.IO.Path.Combine(outputModelPath, outputModelName + ".3dm");

            Console.WriteLine("Writing Rhino file...");
            WriteRhinoFile(newModel, rhinoModelPath);

            
        }
    }
}
