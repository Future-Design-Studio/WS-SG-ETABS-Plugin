using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.select_uniquename
{
    internal class FunctionSelUniqueName
    {
        public static (bool, string) SelectByUniqueName (cSapModel SapModel, string pointUniqueName, string frameUniqueName, string areaUniqueName)
        {
            //Deselect all
            SapModel.SelectObj.All(true);

            int Ret = 0;
            List<string> unableToFindPointUniqueName = new List<string>();
            List<string> unableToFindFrameUniqueName = new List<string>();
            List<string> unableToFindAreaUniqueName = new List<string>();

            string[] splitPointUniqueName = pointUniqueName.Split (',');
            string[] splitFrameUniqueName = frameUniqueName.Split(',');
            string[] splitAreaUniqueName = areaUniqueName.Split(',');

            int itemSelectedBefore = 0;
            int itemSelectedAfter = 0;

            //If splitPointUniqueName not empty
            if (splitPointUniqueName.Length == 1 && splitPointUniqueName[0] == "")
            {
            }
            else 
            {
                foreach (var singlePointUniqueName in splitPointUniqueName)
                {
                    itemSelectedBefore = src.HelperEtabs.CountSelected(SapModel);

                    Ret = SapModel.PointObj.SetSelected(singlePointUniqueName, true, eItemType.Objects);

                    //Ret does not return to 1 when unable to find
                    /*
                    if (Ret !=0)
                    {
                        unableToFindPointUniqueName.Add(singlePointUniqueName);
                    }*/

                    itemSelectedAfter = src.HelperEtabs.CountSelected(SapModel);

                    if (itemSelectedAfter == itemSelectedBefore)
                    {
                        unableToFindPointUniqueName.Add(singlePointUniqueName);
                    }
                }
            }

            //If splitFrameUniqueName not empty
            if (splitFrameUniqueName.Length == 1 && splitFrameUniqueName[0] == "")
            {
            }
            else
            {
                foreach (var singleFrameUniqueName in splitFrameUniqueName)
                {
                    itemSelectedBefore = src.HelperEtabs.CountSelected(SapModel);

                    Ret = SapModel.FrameObj.SetSelected(singleFrameUniqueName, true, eItemType.Objects);

                    //Ret does not return to 1 when unable to find
                    /*
                    if (Ret != 0)
                    {
                        unableToFindFrameUniqueName.Add(singleFrameUniqueName);
                    }*/

                    itemSelectedAfter = src.HelperEtabs.CountSelected(SapModel);

                    if (itemSelectedAfter == itemSelectedBefore)
                    {
                        unableToFindFrameUniqueName.Add(singleFrameUniqueName);
                    }
                }
            }

            //If splitAreaUniqueName not empty
            if (splitAreaUniqueName.Length == 1 && splitAreaUniqueName[0] == "")
            {
            }
            else
            {
                foreach (var singleAreaUniqueName in splitAreaUniqueName)
                {
                    itemSelectedBefore = src.HelperEtabs.CountSelected(SapModel);

                    Ret = SapModel.AreaObj.SetSelected(singleAreaUniqueName, true, eItemType.Objects);

                    //Ret does not return to 1 when unable to find
                    /*
                    if (Ret != 0)
                    {
                        unableToFindAreaUniqueName.Add(singleAreaUniqueName);
                    }*/

                    itemSelectedAfter = src.HelperEtabs.CountSelected(SapModel);

                    if (itemSelectedAfter == itemSelectedBefore)
                    {
                        unableToFindAreaUniqueName.Add(singleAreaUniqueName);
                    }
                }
            }

            //Create error message
            string errorMessage = "";

            if (unableToFindPointUniqueName.Count > 0)
            {
                errorMessage += "Unable to find point: ";

                foreach (string singleUnableToFindPointUniqueName in unableToFindPointUniqueName)
                {
                    errorMessage += singleUnableToFindPointUniqueName + ", ";
                }

                errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                errorMessage += "\n\n";
            }

            if (unableToFindFrameUniqueName.Count > 0)
            {
                errorMessage += "Unable to find frame: ";

                foreach (string singleUnableToFindFrameUniqueName in unableToFindFrameUniqueName)
                {
                    errorMessage += singleUnableToFindFrameUniqueName + ", ";
                }

                errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                errorMessage += "\n\n";
            }

            if (unableToFindAreaUniqueName.Count > 0)
            {
                errorMessage += "Unable to find area: ";

                foreach (string singleUnableToFindAreaUniqueName in unableToFindAreaUniqueName)
                {
                    errorMessage += singleUnableToFindAreaUniqueName + ", ";
                }

                errorMessage = errorMessage.Remove(errorMessage.Length - 2);
            }

            return (false, errorMessage);
        }
    }
}
