using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tekla;
using Tekla.Structural.InteropAssemblies.TeddsCalc;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    internal class FunctionPrecastRCColumn
    {
        public static bool PrecastRCColumn(cSapModel SapModel, cPluginCallback Plugin, List<src.HelperEtabs.DatabaseTable> DatabaseTableListETABS, List<ClassPrecastRCColumn.GroupandCoverandGradeMatch> groupList)
        {
            ICalculator calculator = new Calculator();

            calculator.Initialize();
            calculator.Functions.SetVar("b", 300.0, "mm");
            calculator.Functions.SetVar("h", 500.5, "mm");
            calculator.Functions.SetVar("_CalcUI", 0);

            string variables = calculator.GetVariables();
            calculator.InitializeCalc("$(SysLbrDir)Precast column design-EN1992-x-engb.lbr", "Precast column design (EN1992)", variables);




            return false;
        }
    }
}
