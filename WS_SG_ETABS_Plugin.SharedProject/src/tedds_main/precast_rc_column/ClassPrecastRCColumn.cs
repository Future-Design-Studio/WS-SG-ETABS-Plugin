using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.tedds_main.precast_rc_column
{
    public class ClassPrecastRCColumn
    {
        public class GroupandCoverandGradeMatch
        {
            public string Group { get; set; }
            public string Cover { get; set; }
            public int Method { get; set; }

            public GroupandCoverandGradeMatch(string group, string cover, int method)
            {
                Group = group;
                Cover = cover;
                Method = method;
            }
        }
    }
}
