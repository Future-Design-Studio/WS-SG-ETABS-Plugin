using System;
using System.Collections.Generic;
using System.Linq;
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




            for (var i = 0; i < NumberNames; i++)
            {
                var area = new AreaGeometry(MyName[i], DesignOrientaion[i], ptsGroup[i], Sap);
                allAreas.Add(area);
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
                allFrame.Add(frame);
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
        public static void ExportEtabsToRhino(cSapModel Sap, string filePath, string fileName, IProgress<int> progress)
        {
            //setup new rhino model instance
            Console.WriteLine("Setting up new Rhino model...");


            var newModel = new Rhino.FileIO.File3dm();
            newModel.Settings.ModelUnitSystem = Rhino.UnitSystem.Meters;
            newModel.Settings.ModelAbsoluteTolerance = 0.001;

            int layerFrameObj = newModel.AllLayers.AddLayer("frameObj", System.Drawing.Color.Black);
            int layerAreaObj = newModel.AllLayers.AddLayer("areaObj", System.Drawing.Color.Red);

            Console.WriteLine("Parsing through etabs properties...");

            //all etabs frame and area with property
            var allFrames = GetAllFrames(Sap);
            var allFrameProp = GetAllFrameProperties(Sap);

            progress.Report(20);

            var allAreas = GetAllAreasWithProperties(Sap);

            progress.Report(60);


            //rebuild etabs frame objects 
            //attach etabs property as user text in thino
            Console.WriteLine("Rebuilding etabs FRAME objects in Rhino...");

            for (var i = 0; i<allFrames.Count; i++)
            {
                
                var frame = allFrames[i];
                string framePropName = frame.PropName;
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

            progress.Report(70);
            


            //rebuild etabs area objects in rhino
            //attach etabs property as user text in rhino
            Console.WriteLine("Rebuilding etabs AREA objects in rhino...");

            for (var i = 0; i< allAreas.Count; i++)
            {
                var area = allAreas[i];

                var att = new Rhino.DocObjects.ObjectAttributes();

                att.SetUserString("Name",area.Name);
                att.SetUserString("PropName", area.PropName);
                att.SetUserString("DesignOrientation", area.DesignOrientation);
                att.SetUserString("IsOpening", area.IsOpening.ToString());

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

            progress.Report(90);


            var outputModelPath = filePath;
            var outputModelName = fileName;
            var rhinoModelPath = System.IO.Path.Combine(outputModelPath, outputModelName + ".3dm");

            Console.WriteLine("Writing Rhino file...");
            WriteRhinoFile(newModel, rhinoModelPath);

            
        }
    }
}
