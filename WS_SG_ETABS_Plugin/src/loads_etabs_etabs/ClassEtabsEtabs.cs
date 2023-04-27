using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.loads_etabs_etabs
{
    public class ClassEtabsEtabs
    {
        /// <summary>
        /// Stores the matching load case from reaction and load pattern to assign to
        /// </summary>
        public class CaseandPatternMatch
        {
            public string LoadCaseFrom { get; set; }
            public string LoadPatternTo { get; set; }
            public bool IsCreateNewPattern { get; set; }

            public CaseandPatternMatch(string loadCaseFrom, string loadPatternTo, bool isCreateNewPattern)
            {
                LoadCaseFrom = loadCaseFrom;
                LoadPatternTo = loadPatternTo;
                IsCreateNewPattern = isCreateNewPattern;
            }
        }
    }
}
