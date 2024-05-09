using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.wind_coeff
{
    public class ClassWindCoeff
    {
        #region Wind stuff
        /// <summary>
        /// Takes in string groupname, string load pattern, int direction = 1 for windward, int direction = 2 for leeward, double Cp)
        /// </summary>
        public class WindLoad
        {
            public string GroupName { get; set; }
            public string LoadPattern { get; set; }
            public int Direction { get; set; } 
            public double CP { get; set; }
            public WindLoad(string groupName, string loadPattern, int direction, double cP)
            {
                GroupName = groupName;
                LoadPattern = loadPattern;
                Direction = direction;
                CP = cP;
            }
        }

        #endregion
    }
}
