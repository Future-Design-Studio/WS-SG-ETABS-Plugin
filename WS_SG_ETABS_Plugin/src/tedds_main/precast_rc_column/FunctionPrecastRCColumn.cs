using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tekla;
using Tekla.Structural.InteropAssemblies.Tedds;
using ETABSv1;
using System.Windows.Controls;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    internal class FunctionPrecastRCColumn
    {
        public static bool PrecastRCColumn(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList)
        {

            #region Create folder to store all tedds documents and folderPath to store path
            //Check if have folder to store tedds files, otherwise create new folder to store
            string modelPath = SapModel.GetModelFilepath();
            string folderPath = modelPath + "TeddsFilesFromTeddsPowerAPP";
            bool exists = System.IO.Directory.Exists(folderPath);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            MessageBox.Show("Tedds files created from this plugin will be save in folder: TeddsFilesFromTeddsPowerAPP");
            MessageBox.Show("Folder location: " + folderPath);
            #endregion



            //calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);




            return false;
        }
    }
}
