using Microsoft.Office.Interop.Excel;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WS_SG_ETABS_Plugin.src.wall_design_view
{
    /// <summary>
    /// Interaction logic for WindowWallDesignResultLoading.xaml
    /// </summary>
    public partial class WindowWallDesignResultLoading : System.Windows.Window
    {
        public WindowWallDesignResultLoading(List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel)
        {
            InitializeComponent();

            ProcessWallDesignData(databaseTableList, listChoosenGroup, tableInListPierDesignResult,
                    tableInListSpandrelDesignResult, titleSize, axisTitleSize, tickLabelSize, extractEtabs, extractExcel);

            ShowDialog();

        }

        private async void ProcessWallDesignData(List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenGroup,
            List<Dictionary<string, string>> tableInListPierDesignResult, List<Dictionary<string, string>> tableInListSpandrelDesignResult, int titleSize, int axisTitleSize,
            int tickLabelSize, bool? extractEtabs, bool? extractExcel)
        {
            //Create wall result instance
            List<ClassWallDesign.WallDesignDetail> wallDesignDetailList = new List<ClassWallDesign.WallDesignDetail>();

            await Task.Run(() => {
                bool isError = false;
                
                //Get the table
                src.HelperEtabs.DatabaseTable pointObjectConnectivityTable = new HelperEtabs.DatabaseTable();
                (isError, pointObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Point Object Connectivity");
                if (isError) { return; }

                src.HelperEtabs.DatabaseTable wallObjectConnectivityTable = new HelperEtabs.DatabaseTable();
                (isError, wallObjectConnectivityTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Wall Object Connectivity");
                if (isError) { return; }

                src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
                (isError, groupAssignmentsTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
                if (isError) { return; }

                src.HelperEtabs.DatabaseTable assignmentPierLabels = new HelperEtabs.DatabaseTable();
                (isError, assignmentPierLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Pier Labels");
                if (isError) { return; }

                src.HelperEtabs.DatabaseTable assignmentSpandrelLabels = new HelperEtabs.DatabaseTable();
                (isError, assignmentSpandrelLabels) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Area Assignments - Spandrel Labels");
                if (isError) { return; }

                //Filter group to only include area, to improve performance
                (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Area");
                if (isError) { return; }

                //Filter group to only include groups which are choosen, to improve performance
                src.HelperEtabs.DatabaseTable groupAssignmentsTableFilteredGroupCompiled = new HelperEtabs.DatabaseTable();
                groupAssignmentsTableFilteredGroupCompiled.TableInList = new List<Dictionary<string, string>>();
                foreach (string group in listChoosenGroup)
                {
                    src.HelperEtabs.DatabaseTable groupAssignmentsTableFilteredGroup = new HelperEtabs.DatabaseTable();
                    (isError, groupAssignmentsTableFilteredGroup) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "GroupName", group);
                    if (isError) { return; }

                    groupAssignmentsTableFilteredGroupCompiled.TableInList.AddRange(groupAssignmentsTableFilteredGroup.TableInList);
                }

                //Convert to point 3D
                List<HelperEtabs.Point3D> pointList = HelperEtabs.ConvertPointDatabaseToPoint3D(pointObjectConnectivityTable);

                //Filter by choosen group
                //(isError, wallObjectConnectivityTable) = src.HelperEtabs.FilterDatabaseTableUniqueNameByGroup(wallObjectConnectivityTable, groupAssignmentsTable, choosenGroup);

                //Get shell points
                List<HelperEtabs.Shell> shellList = HelperEtabs.GetShellsPoints(wallObjectConnectivityTable);

                double numOfWalls = shellList.Count();

                double wallIterator = 0;
                foreach (HelperEtabs.Shell shell in shellList)
                {
                    //Collect all the points
                    List<HelperEtabs.Point3D> shellPointList = new List<HelperEtabs.Point3D>();

                    foreach (string shellPointUniqueName in shell.Points)
                    {
                        foreach (HelperEtabs.Point3D point in pointList)
                        {
                            if (shellPointUniqueName == point.UniqueName)
                            {
                                shellPointList.Add(point);
                            }
                        }
                    }

                    //Find story label
                    (string label, string storyLabel) = GetLabelAndStoryLabel(shell.UniqueName, wallObjectConnectivityTable);

                    //Collect pier or spandrel label
                    string pierLabel = GetPierLabel(shell.UniqueName, assignmentPierLabels);
                    string spandrelLabel = GetSpandrelLabel(shell.UniqueName, assignmentSpandrelLabels);

                    //Get group name
                    string groupName = HelperEtabs.GetElementGroupName(groupAssignmentsTableFilteredGroupCompiled, shell.UniqueName, "Area");

                    ClassWallDesign.WallDesignDetail wallDesignDetail = new ClassWallDesign.WallDesignDetail(shell.UniqueName, label, shellPointList, storyLabel, pierLabel, spandrelLabel, groupName);

                    //Check for failure
                    (wallDesignDetail.IsFailAll, wallDesignDetail.IsFailPMN, wallDesignDetail.IsFailShear, wallDesignDetail.PierReinfPercent) = GetWallDesignResult(wallDesignDetail, 
                        tableInListPierDesignResult, tableInListSpandrelDesignResult, extractEtabs, extractExcel);
                  

                    wallDesignDetailList.Add(wallDesignDetail);

                    wallIterator++;

                    double progressPercent = Math.Round(wallIterator / numOfWalls * 100);

                    //Update progressbar every 5%
                    if (progressPercent % 5 == 0)
                    {
                        //Update progress bar
                        Dispatcher.Invoke((System.Action)(() =>
                        {
                            progressBar.Value = progressPercent;
                        }));
                    }

                }

            });

            Close();

            //Call up result window
            WindowWallDesignResult windowWallDesignResult = new WindowWallDesignResult(listChoosenGroup, tableInListPierDesignResult,
                    tableInListSpandrelDesignResult, titleSize, axisTitleSize, tickLabelSize, extractEtabs, extractExcel, wallDesignDetailList);
        }
        

        public static (string, string) GetLabelAndStoryLabel(string uniqueName, HelperEtabs.DatabaseTable wallObjectConnectivityTable)
        {
            foreach (Dictionary<string, string> row in wallObjectConnectivityTable.TableInList)
            {
                if (row["UniqueName"] == uniqueName)
                {
                    return (row["WallBay"], row["Story"]);
                }
            }
            return (null, null);
        }

        public static string GetPierLabel(string uniqueName, HelperEtabs.DatabaseTable assignmentPierLabels)
        {
            if (assignmentPierLabels.TableInList != null)
            {
                foreach (Dictionary<string, string> row in assignmentPierLabels.TableInList)
                {
                    string uniqueNameCheck = row["UniqueName"];
                    if (row["UniqueName"] == uniqueName)
                    {
                        return row["PierName"];
                    }
                }
            }
            return null;

        }

        public static string GetSpandrelLabel(string uniqueName, HelperEtabs.DatabaseTable assignmentSpandrelLabels)
        {
            if (assignmentSpandrelLabels.TableInList != null)
            {
                foreach (Dictionary<string, string> row in assignmentSpandrelLabels.TableInList)
                {
                    if (row["UniqueName"] == uniqueName)
                    {
                        return row["SpandName"];
                    }
                }
            }
            return null;
        }

        public static (bool?, bool?, bool?, double) GetWallDesignResult(ClassWallDesign.WallDesignDetail wall, List<Dictionary<string, string>> tableInListPierDesignResult,
    List<Dictionary<string, string>> tableInListSpandrelDesignResult, bool? extractEtabs, bool? extractExcel)
        {
            bool? failAll = null;
            bool? failPMN = null;
            bool? failShear = null;
            double pierReinfPercent = -1;

            bool wallExist = false;

            //Check if wall design exist
            if (extractEtabs == true)
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                        {
                            wallExist = true;
                            break;
                        }
                    }

                }
                else if (wall.SpandrelLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            wallExist = true;
                            break;
                        }

                    }
                }
            }
            else if (extractExcel == true)
            {
                if (wall.PierLabel != null && tableInListPierDesignResult != null)
                {

                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            wallExist = true;
                            break;
                        }
                    }

                }
                else if (wall.SpandrelLabel != null && tableInListSpandrelDesignResult != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            wallExist = true;
                            break;
                        }

                    }
                }
            }

            if (wallExist)
            {
                failAll = false;
                failPMN = false;
                failShear = false;
                pierReinfPercent = 0;
            }
            else
            {
                return (failAll, failPMN, failShear, pierReinfPercent);
            }

            //Start extracting result
            if (extractEtabs == true)
            {
                if (wall.PierLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["PierLabel"] == wall.PierLabel)
                        {
                            if (row["ErrMsg"] != "No Message")
                            {
                                failAll = true;
                            }

                            if (row["ErrMsg"].Contains("flexural"))
                            {
                                failPMN = true;
                            }

                            if (row["ErrMsg"].Contains("Shear"))
                            {
                                failShear = true;
                            }

                            if (pierReinfPercent < Convert.ToDouble(row["ReinfPercent"]))
                            {
                                pierReinfPercent = Convert.ToDouble(row["ReinfPercent"]);
                            }

                        }
                    }

                }
                else if (wall.SpandrelLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            if (row["ErrMsg"] != "No Message")
                            {
                                failAll = true;
                            }

                            if (row["ErrMsg"].Contains("flexural"))
                            {
                                failPMN = true;
                            }

                            if (row["ErrMsg"].Contains("Shear"))
                            {
                                failShear = true;
                            }

                        }

                    }
                }
            }
            else if (extractExcel == true)
            {
                if (wall.PierLabel != null)
                {

                    foreach (Dictionary<string, string> row in tableInListPierDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Pier Label"] == wall.PierLabel)
                        {
                            if (row["Errors"] != "No Message")
                            {
                                failAll = true;
                            }
                            if (row["Errors"].Contains("flexural"))
                            {
                                failPMN = true;
                            }
                            if (row["Errors"].Contains("Shear"))
                            {
                                failShear = true;
                            }

                            if (pierReinfPercent < Convert.ToDouble(row["Required Reinf. Percentage"]))
                            {
                                pierReinfPercent = Convert.ToDouble(row["Required Reinf. Percentage"]);
                            }
                        }
                    }

                }
                else if (wall.SpandrelLabel != null)
                {
                    foreach (Dictionary<string, string> row in tableInListSpandrelDesignResult)
                    {
                        if (row["Story"] == wall.StoryLabel && row["Spandrel"] == wall.SpandrelLabel)
                        {
                            if (row["Errors"] != "No Message")
                            {
                                failAll = true;
                            }
                            if (row["Errors"].Contains("flexural"))
                            {
                                failPMN = true;
                            }
                            if (row["Errors"].Contains("Shear"))
                            {
                                failShear = true;
                            }
                        }

                    }
                }
            }

            return (failAll, failPMN, failShear, pierReinfPercent);

        }

    }
}
