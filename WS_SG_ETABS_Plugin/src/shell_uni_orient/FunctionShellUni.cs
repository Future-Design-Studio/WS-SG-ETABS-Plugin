using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.shell_uni_orient
{
    internal class FunctionShellUni
    {


        #region Data source

        public static List<string> shellDirectionList = new List<string>()
        {
            "X+", "X-", "Y+", "Y-", "Z+", "Z-"
        };

        #endregion



        /// <summary>
        /// Check if the required tables exist
        /// </summary>
        /// <param name="Sap"></param>
        /// <returns>Return true if all required tables exist</returns>
        public static bool CheckTableExistShellUni(cSapModel Sap)
        {
            //Check if the table exist
            List<string> tableKeys = new List<string>();
            tableKeys.Add("Point Object Connectivity");
            tableKeys.Add("Floor Object Connectivity");
            tableKeys.Add("Wall Object Connectivity");

            List<string> tableKeysNotExist = src.HelperEtabs.CheckIfTableExistForDisplay(Sap, tableKeys);

            if (tableKeysNotExist.Count > 0)
            {
                foreach (string tableKeyNotExist in tableKeysNotExist)
                {
                    src.HelperEtabs.WarningTableDoesNotExist(tableKeyNotExist);
                }

                return false;
            }

            return true;
        }


       public static bool MakeShellOrientationUniform(cSapModel Sap, List<string> listChoosenGroup, string shellDir)
        {
            //Copy original group assigments
            List<Dictionary<string, string>> groupAssignmentsEdit = new List<Dictionary<string, string>>();
            int groupAsignmentsTableVersion = 0;
            string[] groupAsignmentsFieldsKeysIncluded = null;
            int groupAsignmentsNumberRecords = 0;

            string groupAssignmentsTableKey = "Group Assignments";
            (groupAssignmentsEdit, groupAsignmentsTableVersion, groupAsignmentsFieldsKeysIncluded, groupAsignmentsNumberRecords) = src.HelperEtabs.GetTableEdit(Sap, groupAssignmentsTableKey);

            //Copy original area assigments
            List<List<Dictionary<string, string>>> originalAreaAssignments = src.HelperEtabs.CopyAreaAssigments(Sap);


            //Parsing intended direction
            char axisFinal = shellDir[0];
            bool isPositiveFinal;
            if (shellDir[1].ToString() == "+")
            {
                isPositiveFinal = true;
            }
            else
            {
                isPositiveFinal = false;
            }
            
            bool isError = false;
            bool showWarning = false;

            //Open required tables
            List<Dictionary<string, string>> pointObjConnectivity = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> floorObjConnectivity = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> wallObjConnectivity = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> nullAreaObjConnectivity = new List<Dictionary<string, string>>();

            string tableKey = "Point Object Connectivity";
            (isError, pointObjConnectivity) = src.HelperEtabs.GetTableDisplay(Sap, tableKey, showWarning);

            if (isError) { return isError; }

            tableKey = "Floor Object Connectivity";
            (isError, floorObjConnectivity) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            /*if (isError) { return isError; }*/

            tableKey = "Wall Object Connectivity";
            (isError, wallObjConnectivity) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            /* if (isError) { return isError; }*/

            tableKey = "Null Area Object Connectivity";
            (isError, nullAreaObjConnectivity) = src.HelperEtabs.GetTableDisplaySpecificGroup(Sap, tableKey,
                listChoosenGroup, showWarning);

            //Convert points ETABS datase to Point3D
            List<src.HelperEtabs.Point3D> point3DList = src.HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjConnectivity);

            //Getting floor list
            List<src.HelperEtabs.Shell> floorList = new List<src.HelperEtabs.Shell>();
            if (floorObjConnectivity != null)
            {
                //Parse floor points from ETABS database to Shell type
                floorList = GetShellsPoints(floorObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell floor in floorList)
                {
                    floor.NormalDirection = GetShellNormalDirection(floor, point3DList);
                }

                //Check if floor need to be flipped
                foreach (src.HelperEtabs.Shell floor in floorList)
                {
                    FlipShellIfNeeded(Sap, floor, axisFinal, isPositiveFinal);
                }

            }

            //Getting wall list
            List<src.HelperEtabs.Shell> wallList = new List<src.HelperEtabs.Shell>();
            if (wallObjConnectivity != null)
            {
                //Parse wall points from ETABS database to Shell
                wallList = GetShellsPoints(wallObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell wall in wallList)
                {
                    wall.NormalDirection = GetShellNormalDirection(wall, point3DList);
                }

                //Flip  wall normal if necessarry
                foreach (src.HelperEtabs.Shell wall in wallList)
                {
                    FlipShellIfNeeded(Sap, wall, axisFinal, isPositiveFinal);
                }
            }

            //Getting null area list
            List<src.HelperEtabs.Shell> nullAreaList = new List<src.HelperEtabs.Shell>();
            if (nullAreaObjConnectivity != null)
            {
                //Parse wall points from ETABS database to Shell
                nullAreaList = GetShellsPoints(nullAreaObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell nullArea in nullAreaList)
                {
                   nullArea.NormalDirection = GetShellNormalDirection(nullArea, point3DList);
                }

                //Flip null area normal if necessarry
                foreach (src.HelperEtabs.Shell nullArea in nullAreaList)
                {
                    FlipShellIfNeeded(Sap, nullArea, axisFinal, isPositiveFinal);
                }
            }

            /*
            //Applying edited table (floor)
            if (floorObjConnectivity != null)
            {
                List<Dictionary<string, string>> floorObjConnectivityEdit = new List<Dictionary<string, string>>();
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;

                tableKey = "Floor Object Connectivity";
                (floorObjConnectivityEdit, TableVersion, FieldsKeysIncluded, NumberRecords) = src.HelperEtabs.GetTableEdit(Sap, tableKey);

                floorObjConnectivityEdit = EditTableShell(floorObjConnectivityEdit, floorList);

                isError = src.HelperEtabs.ApplyEtabsTable(Sap, tableKey, floorObjConnectivityEdit, TableVersion, FieldsKeysIncluded, NumberRecords);

                if (isError) { return isError; }
            }
            
            if (wallObjConnectivity != null)
            {
                //Applying edited table (wall)
                List<Dictionary<string, string>> wallObjConnectivityEdit = new List<Dictionary<string, string>>();
                int TableVersion = 0;
                string[] FieldsKeysIncluded = null;
                int NumberRecords = 0;

                tableKey = "Wall Object Connectivity";
                (wallObjConnectivityEdit, TableVersion, FieldsKeysIncluded, NumberRecords) = src.HelperEtabs.GetTableEdit(Sap, tableKey);

                wallObjConnectivityEdit = EditTableShell(wallObjConnectivityEdit, floorList);

                isError = src.HelperEtabs.ApplyEtabsTable(Sap, tableKey, wallObjConnectivityEdit, TableVersion, FieldsKeysIncluded, NumberRecords);

                if (isError) { return isError; }
            }
            */

            //Apply area assignment

            //Applying original group assignment
            isError = src.HelperEtabs.ApplyEtabsTable(Sap, groupAssignmentsTableKey, groupAssignmentsEdit, groupAsignmentsTableVersion, groupAsignmentsFieldsKeysIncluded, groupAsignmentsNumberRecords);
            if (isError) { return isError; }

            //Applying original area assignments
       
            //Set isError = false to show windows
            isError = false;
            return isError;

        }

        /// <summary>
        /// Parsing shell edge points from ETABS database
        /// </summary>
        /// <param name="shellObjConnectivity"></param>
        /// <returns></returns>
        public static List<src.HelperEtabs.Shell> GetShellsPoints (List<Dictionary<string,string>> shellObjConnectivity)
        {
            int index = 0;
            List<src.HelperEtabs.Shell> shellList = new List<src.HelperEtabs.Shell>();

            while (index < shellObjConnectivity.Count) 
            {
                List<string> pointList = new List<string>();

                //Assume that row below is same shell so can enter while loop
                bool rowBelowIsSameShell = true;

                while (rowBelowIsSameShell)
                {
                    if (shellObjConnectivity[index]["UniquePt1"] != null)
                    {
                        pointList.Add(shellObjConnectivity[index]["UniquePt1"]);
                    }
                    if (shellObjConnectivity[index]["UniquePt2"] != null)
                    {
                        pointList.Add(shellObjConnectivity[index]["UniquePt2"]);
                    }
                    if (shellObjConnectivity[index]["UniquePt3"] != null)
                    {
                        pointList.Add(shellObjConnectivity[index]["UniquePt3"]);
                    }

                    //Check if UniquePt4 key exist, if exist take UniquePt4 else skip
                    bool keyExists = shellObjConnectivity[index].ContainsKey("UniquePt4");
                    if (keyExists)
                    {
                        if (shellObjConnectivity[index]["UniquePt4"] != null)
                        {
                            pointList.Add(shellObjConnectivity[index]["UniquePt4"]);
                        }
                    }                     

                    //If this is not last row and below this row still the same shell, go to next row, loop again to get more points
                    if (index < shellObjConnectivity.Count - 1 && shellObjConnectivity[index]["UniqueName"] == shellObjConnectivity[index + 1]["UniqueName"])
                    {
                        index++;
                    }
                    //else go out the loop and save points
                    else
                    {
                        rowBelowIsSameShell = false;
                    }
                }
                src.HelperEtabs.Shell shell = new src.HelperEtabs.Shell(shellObjConnectivity[index]["UniqueName"], pointList);
                shellList.Add(shell);

                index++;
            }

            return shellList;
        }

        public static Dictionary<string, double> GetShellNormalDirection(src.HelperEtabs.Shell shell, List<src.HelperEtabs.Point3D> point3DList)
        {
            List<Dictionary<string, string>> shellPointList = new List<Dictionary<string, string>>();

            //Only look at point 0 1 2
            shellPointList.Add(new Dictionary<string, string>(){  { "Unique Name", shell.Points[0] } });
            shellPointList.Add(new Dictionary<string, string>() { { "Unique Name", shell.Points[1] } });
            shellPointList.Add(new Dictionary<string, string>() { { "Unique Name", shell.Points[2] } });

            //Finding coordinate for point 0 1 2
            foreach (Dictionary<string, string> shellPoint in shellPointList)
            {
                foreach (src.HelperEtabs.Point3D point3D in point3DList)
                {
                    if (shellPoint["Unique Name"] == point3D.UniqueName)
                    {
                        shellPoint["X"] = point3D.XCoordinate.ToString();
                        shellPoint["Y"] = point3D.YCoordinate.ToString();
                        shellPoint["Z"] = point3D.ZCoordinate.ToString();
                        break;
                    }
                }
            }

            //Calculate vector from point 1 -> point 0
            Dictionary<string, double> vectorOneToZero = new Dictionary<string, double>();
            vectorOneToZero["X"] = Convert.ToDouble(shellPointList[0]["X"]) - Convert.ToDouble(shellPointList[1]["X"]);
            vectorOneToZero["Y"] = Convert.ToDouble(shellPointList[0]["Y"]) - Convert.ToDouble(shellPointList[1]["Y"]);
            vectorOneToZero["Z"] = Convert.ToDouble(shellPointList[0]["Z"]) - Convert.ToDouble(shellPointList[1]["Z"]);

            //Calculate vector from point 1 -> point 2
            Dictionary<string, double> vectorOneToTwo = new Dictionary<string, double>();
            vectorOneToTwo["X"] = Convert.ToDouble(shellPointList[2]["X"]) - Convert.ToDouble(shellPointList[1]["X"]);
            vectorOneToTwo["Y"] = Convert.ToDouble(shellPointList[2]["Y"]) - Convert.ToDouble(shellPointList[1]["Y"]);
            vectorOneToTwo["Z"] = Convert.ToDouble(shellPointList[2]["Z"]) - Convert.ToDouble(shellPointList[1]["Z"]);

            //Calculate 
            (double X, double Y, double Z) = src.HelperMethod.CrossProduct(vectorOneToTwo["X"], vectorOneToTwo["Y"], vectorOneToTwo["Z"], vectorOneToZero["X"], vectorOneToZero["Y"], vectorOneToZero["Z"]);

            Dictionary<string, double> normalShell = new Dictionary<string, double>()
            { 
                {"X", X }, 
                {"Y", Y}, 
                {"Z", Z } 
            };

            return normalShell;
        }

        /// <summary>
        /// Check if axisFinal direction is positive or negative. Flip if necesarry
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="axisFinal"></param>
        /// <param name="isPositiveFinal"></param>
        public static void FlipShellIfNeeded(cSapModel Sap, src.HelperEtabs.Shell shell, char axisFinal, bool isPositiveFinal)
        {
            double axisDirection = shell.NormalDirection[axisFinal.ToString()];

            //If shell is not perpendicular to normal axis and axisDirection negative but isPositiveFinal is true
            //If shell is not perpendicular to normal axis and axisDirection positive but isPositiveFinal is false
            if ((axisDirection != 0 && axisDirection < 0 && isPositiveFinal) || 
                (axisDirection != 0 && axisDirection > 0 && !isPositiveFinal))
            {
                //Flipping axis
                shell.Points.Reverse();

                HelperEtabs.DeleteArea(Sap, shell);
                HelperEtabs.RecreateExistingAreaByPoints(Sap, shell);

            }

        }

        public static List<Dictionary<string, string>> EditTableShell(List<Dictionary<string, string>> floorObjConnectivityEdit, List<src.HelperEtabs.Shell> floorList)
        {
            foreach (src.HelperEtabs.Shell floor in floorList)
            {
                int index = 0;
                while (index < floor.Points.Count)
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    row["Unique Name"] = floor.UniqueName;

                    if (index < floor.Points.Count) { row["UniquePt1"] = floor.Points[index]; }
                    index++;
                    if (index < floor.Points.Count) { row["UniquePt2"] = floor.Points[index]; }
                    index++;
                    if (index < floor.Points.Count) { row["UniquePt3"] = floor.Points[index]; }
                    index++;
                    if (index < floor.Points.Count) { row["UniquePt4"] = floor.Points[index]; }
                    index++;

                    floorObjConnectivityEdit.Add(row);
                }                        
            }

            return floorObjConnectivityEdit;
        }




        /*        public static void CheckSingleShell(src.HelperEtabs.Point3D point3DList, Dictionary<string, string> singleShell)
                {



                    singleShell["point"]
                }

                /// <summary>
                /// Check if shell normal is oriented to vector
                /// </summary>
                /// <returns></returns>
                public static bool ShellNormalOrientedToAxis()
                {

                }*/
    }
}
