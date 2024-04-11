using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ETABSv1;
using Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.joint_forces_extractor
{
    internal class FunctionJointForcesExtractorMain
    {

        public static bool ExtractJointForcesExcel(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> databaseTableList, List<string> listChoosenLoadCase, List<string> listChoosenGroup, string filePath)
        {

            bool isError = false;
            src.HelperEtabs.DatabaseTable groupAssignmentTable = new HelperEtabs.DatabaseTable();
            src.HelperEtabs.DatabaseTable frameEndForcesTable = new HelperEtabs.DatabaseTable();

            foreach (src.HelperEtabs.DatabaseTable table in databaseTableList)
            {
                if (table.TableKey == "Group Assignments")
                {
                    (isError, groupAssignmentTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
                    if (isError)
                    {
                        return isError;
                    }
                }
                else if (table.TableKey == "Element Joint Forces - Frame")
                {
                    (isError, frameEndForcesTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Joint Forces - Frame");
                    if (isError)
                    {
                        return isError;
                    }
                }
                else
                {
                    MessageBox.Show("Error, list of database tables do not match, please check that you have ");
                }
            }

            //Filter group by points only
            (isError, groupAssignmentTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentTable, "Type", "Points");

            //Filter group by selected group name
            List<Dictionary<string, string>> groupAssignmentTableFiltered = new List<Dictionary<string, string>>();
            List<string> pointUniqueNames = new List<string>();
            foreach (Dictionary<string, string> row in groupAssignmentTable.TableInList)
            {
                if (listChoosenGroup.Contains(row["Group"]))
                {
                    groupAssignmentTableFiltered.Add(row);
                    if (pointUniqueNames.Contains(row["Joint Unique Name"]))
                    {
                        continue;
                    }
                    else
                    {
                        pointUniqueNames.Add(row["Joint Unique Name"]);
                    }
                }
            }


            List<Dictionary<string, string>> frameEndForcesTableFiltered = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in frameEndForcesTable.TableInList)
            {
                if (pointUniqueNames.Contains(row["Joint Unique Name"]) && listChoosenLoadCase.Contains(row["Load"]))
                {
                    frameEndForcesTableFiltered.Add(row);
                }
            }


            List<Dictionary<string, string>> finalTableToExcel = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in frameEndForcesTableFiltered)
            {
                Dictionary<string, string> rowinternal = new Dictionary<string, string>
                {
                    {"Joint Unique Name", row["Joint Unique Name"]},
                    {"Frame Unique Name", row["Frame Unique Name"]},
                    {"Load Case/Combi", row["Load case/combi"]},
                    {"Max/Min", row["Max/Min"]},
                    {"Axial (kN)", row["F1"]},
                    {"Shear Minor (kN)", row["F2"]},
                    {"Shear Major (kN)", row["F3"]},
                    {"Moment Minor (kN)", row["M1"]},
                    {"Moment Major (kN)", row["M2"]},
                    {"Torsion (kN)", row["M3"]},
                };
                finalTableToExcel.Add(rowinternal);
            }











                return false;
        }
    }
}
