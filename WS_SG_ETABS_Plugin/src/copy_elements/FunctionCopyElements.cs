using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accord.Math.Distances;
using System.Windows.Input;
using ETABSv1;
using Microsoft.Office.Interop.Excel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;

namespace WS_SG_ETABS_Plugin.src.copy_elements
{
    public class FunctionCopyElements
    {
        public static (bool, Dictionary<string, HelperEtabs.DatabaseTable>) ExtractDatabaseTable(cSapModel SapModel, string filePathFrom)
        {
            cSapModel SapModelFrom;
            cOAPI myETABSObjectFrom;

            bool isError = false;

            //Opening model
            (isError, SapModelFrom, myETABSObjectFrom) = src.HelperEtabs.OpenModel(filePathFrom);

            if (isError)
            {
                MessageBox.Show("Unable to open file! Please recheck path!");
                return (isError, null);
            }

            //Check if modelFrom is locked
            bool modelFromLocked = SapModelFrom.GetModelIsLocked();

            if (modelFromLocked)
            {
                myETABSObjectFrom.ApplicationExit(false);
                MessageBox.Show("Reference ETABS file is locked! Please 'save as' the model to unlock it!");
                isError = true;
                return (isError, null);
            }

            //Get unit in EtabsTo
            eForce forceUnit = new eForce();
            eLength lengthUnit = new eLength();
            eTemperature temperatureUnit = new eTemperature();

            (forceUnit, lengthUnit, temperatureUnit) = src.HelperEtabs.GetPresentUnit(SapModel);

            //Set unit in EtabsFrom
            src.HelperEtabs.SetOutputUnit(SapModelFrom, forceUnit, lengthUnit, temperatureUnit);

            List<string> tableKeysToSave = new List<string>()
            {
                "Point Object Connectivity",
                "Wall Object Connectivity", "Floor Object Connectivity", "Null Area Object Connectivity",
                "Beam Object Connectivity", "Column Object Connectivity", "Brace Object Connectivity", "Null Line Object Connectivity",
                "Group Assignments"
            };


            //Copy multiple tables from reference etabs
            List<src.HelperEtabs.DatabaseTable> databaseTablesEtabsFrom = new List<HelperEtabs.DatabaseTable>() { };

            bool isErrorIgnored = false;
            (isErrorIgnored, databaseTablesEtabsFrom) = src.HelperEtabs.GetMultipleTableDisplay(SapModelFrom, tableKeysToSave, false);
            myETABSObjectFrom.ApplicationExit(false);

            //Going through each saved table in EtabsFrom and open in EtabsTo
            Dictionary<string, HelperEtabs.DatabaseTable> databaseTableDict = new Dictionary<string, HelperEtabs.DatabaseTable>();
            foreach (src.HelperEtabs.DatabaseTable databaseTableEtabsFrom in databaseTablesEtabsFrom)
            {

                databaseTableDict.Add(databaseTableEtabsFrom.TableKey, databaseTableEtabsFrom);
            }

            return (isError, databaseTableDict);
        }

        public static (bool, List<HelperEtabs.Shell>, List<HelperEtabs.Frame>) ProcessElementsToCopy(Dictionary<string, HelperEtabs.DatabaseTable> databaseTableDict)
        {
            bool isError = false;
            List<HelperEtabs.Shell> shellList = new List<HelperEtabs.Shell>();
            List<HelperEtabs.Frame> frameList = new List<HelperEtabs.Frame>();

            //Get shell points
            if (databaseTableDict.ContainsKey("Wall Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Shell> wallList = null;
                (isError, wallList) = HelperEtabs.GetShellsPoints(databaseTableDict["Wall Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { shellList.AddRange(wallList); };
            }

            if (databaseTableDict.ContainsKey("Floor Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Shell> floorList = null;
                (isError, floorList) = HelperEtabs.GetShellsPoints(databaseTableDict["Floor Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { shellList.AddRange(floorList); };
            }

            if (databaseTableDict.ContainsKey("Null Area Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Shell> nullAreaList = null;
                (isError, nullAreaList) = HelperEtabs.GetShellsPoints(databaseTableDict["Null Area Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { shellList.AddRange(nullAreaList); };
            }

            //Get frame points
            if (databaseTableDict.ContainsKey("Beam Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Frame> beamList = null;
                (isError, beamList) = HelperEtabs.GetFramesPoints(databaseTableDict["Beam Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { frameList.AddRange(beamList); };
            }

            if (databaseTableDict.ContainsKey("Column Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Frame> columnList = null;
                (isError, columnList) = HelperEtabs.GetFramesPoints(databaseTableDict["Column Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { frameList.AddRange(columnList); };
            }

            if (databaseTableDict.ContainsKey("Brace Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Frame> braceList = null;
                (isError, braceList) = HelperEtabs.GetFramesPoints(databaseTableDict["Brace Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { frameList.AddRange(braceList); };
            }

            if (databaseTableDict.ContainsKey("Null Line Object Connectivity") && databaseTableDict.ContainsKey("Point Object Connectivity"))
            {
                List<HelperEtabs.Frame> nullLineList = null;
                (isError, nullLineList) = HelperEtabs.GetFramesPoints(databaseTableDict["Null Line Object Connectivity"], databaseTableDict["Point Object Connectivity"]);
                if (!isError) { frameList.AddRange(nullLineList); };
            }

            return (isError, shellList, frameList);
        }

        public static (bool, List<string>) GetGroupName(Dictionary<string, HelperEtabs.DatabaseTable> databaseTableDict, List<HelperEtabs.Shell> shellList, List<HelperEtabs.Frame> frameList)
        {
            bool isError = false;
            List<string> groupNames = new List<string>();

            HelperEtabs.DatabaseTable groupAssignmentDatabaseTable = new HelperEtabs.DatabaseTable();
            if (databaseTableDict.ContainsKey("Group Assignments"))
            {
                groupAssignmentDatabaseTable = databaseTableDict["Group Assignments"];
            }

            foreach (HelperEtabs.Shell shell in shellList)
            {
                string groupName = HelperEtabs.GetElementGroupName(groupAssignmentDatabaseTable, shell.UniqueName, "Area");

                if (groupName != null)
                {
                    shell.Group = groupName;
                    if (!groupNames.Contains(groupName))
                    {
                        groupNames.Add(groupName);
                    }
                }
                else
                {
                    MessageBox.Show($"Shell {shell.UniqueName} does not belong in a group! Please add each elements into a group!");
                    isError = true;
                    return (isError, null);
                }
            }

            foreach (HelperEtabs.Frame frame in frameList)
            {
                string groupName = HelperEtabs.GetElementGroupName(groupAssignmentDatabaseTable, frame.UniqueName, "Line");

                if (groupName != null)
                {
                    frame.Group = groupName;
                    if (!groupNames.Contains(groupName))
                    {
                        groupNames.Add(groupName);
                    }
                }
                else
                {
                    MessageBox.Show($"Frame {frame.UniqueName} does not belong in a group! Please add each elements into a group!");
                    isError = true;
                    return (isError, null);
                }
            }

            return (isError, groupNames);

        }

        public static Dictionary<string, string> CreateGroups(cSapModel SapModel, List<string> groupNames, List<HelperEtabs.Shell> shellList, List<HelperEtabs.Frame> frameList)
        {
            int ret = 0;
            string[] existingGroupList = HelperEtabs.GetGroupList(SapModel);
            Dictionary<string, string> translatedGroupNames = new Dictionary<string, string>();

            foreach (string groupName in groupNames)
            {
                string groupNameToAdd = groupName;

                while (existingGroupList.Contains(groupNameToAdd))
                {
                    groupNameToAdd += "_1";
                }

                ret = SapModel.GroupDef.SetGroup_1(groupNameToAdd);

                translatedGroupNames.Add(groupName, groupNameToAdd);
            }

            return translatedGroupNames;
        }

        public static void WriteTxtFile(cSapModel SapModel, List<string> shellFailToCreate, List<string> frameFailToCreate)
        {
            string filePath = SapModel.GetModelFilepath();
            string fileName = filePath + @"\copy_elements_warning.txt";

            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes($"Area unique name fail to create (total = {shellFailToCreate.Count()}):");
                    fs.Write(title, 0, title.Length);
                    //New line
                    Byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);

                    foreach (string shell in shellFailToCreate)
                    {
                        // Add some text to file
                        title = new UTF8Encoding(true).GetBytes($"{shell},");
                        fs.Write(title, 0, title.Length);
                    }

                    //New line
                    newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);

                    // Add some text to file
                    title = new UTF8Encoding(true).GetBytes($"Frame unique name fail to create (total = {frameFailToCreate.Count()}):");
                    fs.Write(title, 0, title.Length);
                    //New line
                    newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);

                    foreach (string frame in frameFailToCreate)
                    {
                        // Add some text to file
                        title = new UTF8Encoding(true).GetBytes($"{frame},");
                        fs.Write(title, 0, title.Length);
                        //New line
                        newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                        fs.Write(newline, 0, newline.Length);
                    }

                }
                /*
                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
                */
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

    }
}
