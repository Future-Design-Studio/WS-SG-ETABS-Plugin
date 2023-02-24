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
            string groupAssignmentsTableKey = "Group Assignments";
            src.HelperEtabs.DatabaseTable groupAssignmentsTable = src.HelperEtabs.GetTableEdit(Sap, groupAssignmentsTableKey);

            //Copy original area assigments
            List<src.HelperEtabs.DatabaseTable> originalAreaAssignments = src.HelperEtabs.CopyAreaAssigments(Sap);

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
            src.HelperEtabs.DatabaseTable pointObjConnectivity = null;
            src.HelperEtabs.DatabaseTable floorObjConnectivity = null;
            src.HelperEtabs.DatabaseTable wallObjConnectivity = null;
            src.HelperEtabs.DatabaseTable nullAreaObjConnectivity = null;

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

            //List to save flipped shells
            List<src.HelperEtabs.Shell> listFlipped = new List<src.HelperEtabs.Shell>();

            //Getting floor list and flip it
            List<src.HelperEtabs.Shell> floorList = new List<src.HelperEtabs.Shell>();
            if (floorObjConnectivity != null)
            {
                //Parse floor points from ETABS database to Shell type
                floorList = src.HelperEtabs.GetShellsPoints(floorObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell floor in floorList)
                {
                    floor.NormalDirection = GetShellNormalDirection(floor, point3DList);
                }

                //Check if floor need to be flipped
                foreach (src.HelperEtabs.Shell floor in floorList)
                {
                    bool shellFlipped = new bool();
                    shellFlipped = FlipShellIfNeeded(Sap, floor, axisFinal, isPositiveFinal);

                    //Add to flippedList if shell is flipped
                    if (shellFlipped) {
                        listFlipped.Add(floor);
                    }
                }

            }

            //Getting wall list and flip it
            List<src.HelperEtabs.Shell> wallList = new List<src.HelperEtabs.Shell>();
            if (wallObjConnectivity != null)
            {
                //Parse wall points from ETABS database to Shell
                wallList = src.HelperEtabs.GetShellsPoints(wallObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell wall in wallList)
                {
                    wall.NormalDirection = GetShellNormalDirection(wall, point3DList);
                }

                //Flip  wall normal if necessarry
                foreach (src.HelperEtabs.Shell wall in wallList)
                {
                    bool shellFlipped = new bool();
                    shellFlipped = FlipShellIfNeeded(Sap, wall, axisFinal, isPositiveFinal);

                    //Add to flippedList if shell is flipped
                    if (shellFlipped)
                    {
                        listFlipped.Add(wall);
                    }
                }
            }

            //Getting null area list and flip it
            List<src.HelperEtabs.Shell> nullAreaList = new List<src.HelperEtabs.Shell>();
            if (nullAreaObjConnectivity != null)
            {
                //Parse wall points from ETABS database to Shell
                nullAreaList = src.HelperEtabs.GetShellsPoints(nullAreaObjConnectivity);

                //Getting floor normal directions
                foreach (src.HelperEtabs.Shell nullArea in nullAreaList)
                {
                   nullArea.NormalDirection = GetShellNormalDirection(nullArea, point3DList);
                }

                //Flip null area normal if necessarry
                foreach (src.HelperEtabs.Shell nullArea in nullAreaList)
                {
                    bool shellFlipped = new bool();
                    shellFlipped = FlipShellIfNeeded(Sap, nullArea, axisFinal, isPositiveFinal);

                    //Add to flippedList if shell is flipped
                    if (shellFlipped)
                    {
                        listFlipped.Add(nullArea);
                    }
                }
            }


            //Filtering area assignments database to only have flipped shells
            filterAreaAssignments(originalAreaAssignments, listFlipped);

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
            isError = src.HelperEtabs.ApplyEtabsTable(Sap, groupAssignmentsTable);
            if (isError) { return isError; }

            //Applying original area assignments
            isError = src.HelperEtabs.ApplyMultipleEtabsTable(Sap, originalAreaAssignments);
            if (isError) { return isError; }

            //Set isError = false to show windows
            isError = false;
            return isError;

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
        public static bool FlipShellIfNeeded(cSapModel Sap, src.HelperEtabs.Shell shell, char axisFinal, bool isPositiveFinal)
        {
            bool flipped = false;

            double axisDirection = shell.NormalDirection[axisFinal.ToString()];

            //If shell is not perpendicular to normal axis and axisDirection negative but isPositiveFinal is true
            //If shell is not perpendicular to normal axis and axisDirection positive but isPositiveFinal is false
            if ((axisDirection != 0 && axisDirection < 0 && isPositiveFinal) || 
                (axisDirection != 0 && axisDirection > 0 && !isPositiveFinal))
            {
                flipped = true;

                //Flipping axis
                shell.Points.Reverse();

                HelperEtabs.DeleteArea(Sap, shell);
                HelperEtabs.RecreateExistingAreaByPoints(Sap, shell);

            }

            return flipped;

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


        public static void filterAreaAssignments(List<src.HelperEtabs.DatabaseTable> originalAreaAssignments, List<src.HelperEtabs.Shell> listFlipped)
        {
            List<src.HelperEtabs.DatabaseTable> originalAreaAssignmentsFiltered = new List<src.HelperEtabs.DatabaseTable>();

            //Goto each table
            foreach (src.HelperEtabs.DatabaseTable databaseTable in originalAreaAssignments)
            {
                List<Dictionary<string, string>> tableInList = databaseTable.TableInList;
                List<Dictionary<string, string>> removedRows = new List<Dictionary<string, string>>();

                //Goto each row
                foreach (Dictionary<string, string> row in tableInList)
                {
                    bool isInFlippedList = false;
                    
                    //If shell in not in flippedList, take note of the row
                    foreach (src.HelperEtabs.Shell shell in listFlipped)
                    {
                        if (row["UniqueName"] == shell.UniqueName)
                        {
                            isInFlippedList = true;
                            break;
                        }
                    }

                    if (!isInFlippedList)
                    {
                        removedRows.Add(row);
                    }
                }

                //After taking all the rows which are not flipped, remove it from the table
                foreach (Dictionary<string, string> removedRow in removedRows)
                {
                    tableInList.Remove(removedRow);
                }

                databaseTable.NumberRecords = tableInList.Count;

            }



               
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
