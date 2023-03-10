using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    internal class FunctionLoadCombo
    {
        public static bool CreateLoadPatternAndCombo(cSapModel SapModel, cPluginCallback Plugin, List<src.load_combo_EN.ClassLoadCombo.DeadLoad> deadLoadList)
        {
            foreach (var deadLoad in deadLoadList)
            {
                //src.load_combo_EN.ClassLoadCombo.DeadLoad deadLoad = deadLoadList[0];
                src.HelperEtabs.AddDeadLoadPattern(SapModel, deadLoad.Name, 1, "Dead");
                //MessageBox.Show(deadLoad.Name);
                //MessageBox.Show(Convert.ToString(deadLoad.IsExisting));

            }
            
            

            return true;
        }
    }
}
