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
using Tekla.Structural.InteropAssemblies.TeddsCalc;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    internal class FunctionPrecastRCColumn
    {
        public static bool PrecastRCColumn(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList)
        {

            #region Create folder to store all tedds documents and folderPath to store path
            //Check if have folder to store tedds files, otherwise create new folder to store
            string modelPath = SapModel.GetModelFilepath();
            string folderPath = modelPath + "TeddsFilesFromTeddsPowerAPP\\";
            bool exists = System.IO.Directory.Exists(folderPath);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            MessageBox.Show("Tedds files created from this plugin will be saved in ETABS source folder at: TeddsFilesFromTeddsPowerAPP");
            #endregion

            #region start main function

            foreach (ClassPrecastRCColumn.GroupandCoverandGradeMatch group in groupList)
            {
                string groupfolderPath = folderPath + group.Group;
                exists = System.IO.Directory.Exists(groupfolderPath);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(groupfolderPath);
                }


                #region set variables to be taken into document
                ICalculator calculator = new Calculator();
                calculator.Initialize();
                calculator.Functions.SetVar("_ColumnSpliceCheckBox", 1);
                calculator.Functions.SetVar("_rcc.BracedY", 1);
                calculator.Functions.SetVar("_rcc.BracedZ", 1);
                calculator.Functions.SetVar("_isLiftingCheckIncluded", 0);

                //input from plugin main required
                calculator.Functions.SetVar("c_{nom}", 50, "mm");

                //input from etabs required
                calculator.Functions.SetVar("_rcc.ColumnGrade", "C90/105");

                calculator.Functions.SetVar("_rcc.Type", "Rectangular");
                calculator.Functions.SetVar("b", 500, "mm");
                calculator.Functions.SetVar("h", 500, "mm");

                calculator.Functions.SetVar("l_{0y}", 500, "mm");
                calculator.Functions.SetVar("l_{0z}", 500, "mm");

                calculator.Functions.SetVar("M_{btmy}", 5, "kNm");
                calculator.Functions.SetVar("M_{btmz}", 5, "kNm");
                calculator.Functions.SetVar("M_{topy}", 5, "kNm");
                calculator.Functions.SetVar("M_{topz}", 5, "kNm");
                calculator.Functions.SetVar("N_{Ed}", 5, "kN");

                calculator.Functions.SetVar("M_{Fry.Splice1}", 6, "kNm");
                calculator.Functions.SetVar("M_{Frz.Splice1}", 6, "kNm");




                string variables = calculator.GetVariables();
                /*
                calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                string variables2 = calculator.GetVariables();
                */
                #endregion

                
                ITeddsDocuments doc1 = new Documents();
                doc1.Add2("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);
                doc1.Close(true);


            }
            #endregion

            //calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);




            return false;
        }
    }
}
