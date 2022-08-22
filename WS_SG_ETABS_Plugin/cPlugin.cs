using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin
{

    /// <summary>
    /// cPlugin class is the entry point from ETABS
    /// </summary>
    public class cPlugin
    {
        public void Main(ref ETABSv1.cSapModel SapModel, ref ETABSv1.cPluginCallback ISapPlugin)
        {
            //Call up main window
            WindowMain windowMain = new WindowMain(ref SapModel, ref ISapPlugin);

        }

        public long Info(ref string Text)
        {
            Text = "WS SG ETABS Plugin";
            return 0;
        }

    }
}
