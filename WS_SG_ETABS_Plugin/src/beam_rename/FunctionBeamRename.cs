using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.rc_beam_rename
{
    internal class FunctionBeamRename
    {
        public static void RenameRcBeam(cSapModel SapModel)
        {
            //Get list of all RC Beam

            GetRcBeamPoints(SapModel, rcBeamList);
            SetNewName(SapModel, rcBeamList);
        }

        public static bool GetRcBeamPoints(cSapModel SapModel, List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            int ret = 0;
            bool isError = false;
            HelperEtabs.DatabaseTable pointObjConnectivity = new HelperEtabs.DatabaseTable();

            //Remove ~temp group if already exist
            string[] existingGroupList = HelperEtabs.GetGroupList(SapModel);
            if (existingGroupList.Contains("~temp"))
            {
                //Select points which is connected to the wall
                //Delete temp group
                ret = SapModel.GroupDef.Delete("~temp");
                if (ret != 0) { isError = true; return isError; }
            }

            List<string> pointsConnectedToRcBeam = new List<string>();
            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                string pointIUniqueName;
                string pointJUniqueName;
                (pointIUniqueName, pointJUniqueName, isError) = HelperEtabs.GetFramePointsUniqueName(SapModel, rcBeam.UniqueName);

                rcBeam.PointIUniqueName = pointIUniqueName;
                rcBeam.PointJUniqueName = pointJUniqueName;

                pointsConnectedToRcBeam.Add(pointIUniqueName);
                pointsConnectedToRcBeam.Add(pointJUniqueName);
            }

            isError = HelperEtabs.CreateNewGroup(SapModel, "~temp");
            if (isError) { return isError; }
            isError = HelperEtabs.AddPointsInsideGroup(SapModel, pointsConnectedToRcBeam, "~temp");
            if (isError) { return isError; }

            List<string> tempListChoosenGroup = new List<string>();
            tempListChoosenGroup.Add("~temp");
            (isError, pointObjConnectivity) = src.HelperEtabs.GetTableDisplaySpecificGroup(SapModel, "Point Object Connectivity", tempListChoosenGroup, false);

            //Convert point connectivity database to Point3D
            List<HelperEtabs.Point3D> allPointList = HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjConnectivity);

            foreach (src.HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                List<HelperEtabs.Point3D> point3DList = new List<HelperEtabs.Point3D>();

                HelperEtabs.Point3D point = null;
                (isError, point) = HelperEtabs.GetSpecificPoint3D(rcBeam.PointIUniqueName, allPointList);
                if (!isError) { rcBeam.PointI = point; } else { return isError; }

                (isError, point) = HelperEtabs.GetSpecificPoint3D(rcBeam.PointJUniqueName, allPointList);
                if (!isError) { rcBeam.PointJ = point; } else { return isError; }
            }

            //Delete temp group
            ret = SapModel.GroupDef.Delete("~temp");

            return isError;
        }

        public static void SetNewName(cSapModel SapModel, List<src.HelperEtabs.RcBeam> rcBeamList)
        {
            rcBeamList = rcBeamList.OrderBy(x => x.PointI.ZCoordinate).ThenBy(x => x.PointI.XCoordinate).ThenBy(x => x.PointI.ZCoordinate).ToList();

            //Get unique z cord from all points, give name to each
            List<double> uniqueZList = new List<double>();
            foreach (HelperEtabs.RcBeam rcBeam in rcBeamList)
            {
                //Only do beam which is on 1 level for now, no slanted beam
                if (rcBeam.PointI.ZCoordinate == rcBeam.PointJ.ZCoordinate)
                {
                    //Can see point I only
                    if (!uniqueZList.Contains(rcBeam.PointI.ZCoordinate))
                    {
                        uniqueZList.Add(rcBeam.PointI.ZCoordinate);
                    }
                }
            }

            //Get unique ij pair in xy coord, give name to each
            int prefixStory = 1;// start with 1
            foreach (double z in uniqueZList)
            {
                int prefixBeam = 1;// start with 1

                foreach (HelperEtabs.RcBeam rcBeam in rcBeamList)
                {
                    //Only do beam which is on 1 level for now, no slanted beam
                    if (rcBeam.PointI.ZCoordinate == z)
                    {
                        Ret = SapModel.FrameObj.ChangeName(rcBeam.UniqueName, $"{prefixStory}B{prefixBeam}");

                        prefixBeam++;
                    }
                }

                prefixStory++;
            }
        }
    }
}
