using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.punching_shear_plot
{
    internal class FunctionPunchingShrPlot
    {
        public static bool PunchingShearPlot(cSapModel SapModel, cPluginCallback Plugin, List<string> listChoosenLoadCase, string choosenGroup, string plotTitle, 
            double bubbleScaleFactor, double textSize, double titleSize, double axisTitleSize, double gridSpacingX, double gridSpacingY, List<src.HelperEtabs.DatabaseTable> databaseTableList)
        {
            bool isError = false;

            //Get the table
            src.HelperEtabs.DatabaseTable jointReactionTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Joint Reactions");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable groupAssignmentsTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Group Assignments");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable elementForcesColumnsTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Element Forces - Columns");
            if (isError) { return isError; }

            src.HelperEtabs.DatabaseTable frameAssignmentSectPropTable = new HelperEtabs.DatabaseTable();
            (isError, jointReactionTable) = src.HelperEtabs.CallDatabaseTableByTableKey(databaseTableList, "Frame Assignments - Section Properties");
            if (isError) { return isError; }

            //Filter group to only include points, to improve performance
            (isError, groupAssignmentsTable) = src.HelperEtabs.FilterDatabaseTable(groupAssignmentsTable, "ObjectType", "Point");
            if (isError) { return isError; }

            //Container
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();

            jointList = GetJointList(groupAssignmentsTable);
            GetColumnConnectedToJoint()




            return false;
        }

        public static List<src.HelperEtabs.Point3D> GetJointList(src.HelperEtabs.DatabaseTable groupAssignmentsTable)
        {
            List<src.HelperEtabs.Point3D> jointList = new List<src.HelperEtabs.Point3D>();

            //Get table from database
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            table = groupAssignmentsTable.TableInList;

            foreach(Dictionary<string, string> row in table)
            {
                string 
            }
        }
    }
}
