using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.apply_section_property
{
    public class ClassApplySectProp
    {
        public class ApplySectPropDefinition
        {
            public string GroupName { get; set; }
            public string FrameSectProp { get; set; }
            public string AreaSectProp { get; set; }


            public ApplySectPropDefinition(string groupName, string frameSectProp, string areaSectProp)
            {
                GroupName = groupName;
                FrameSectProp = frameSectProp;
                AreaSectProp = areaSectProp;
            }
        }
    }
}
