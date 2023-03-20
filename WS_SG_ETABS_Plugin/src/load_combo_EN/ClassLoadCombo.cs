using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    public class ClassLoadCombo
    {
        #region Load CLasses

        public class DeadLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public DeadLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class SDLLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public SDLLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class LiveLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }
            public string CategoryPhi { get; set; }

            public LiveLoad(string name, bool isExisting, string categoryPhi)
            {
                Name = name;
                IsExisting = isExisting;
                CategoryPhi = categoryPhi;
            }
        }

        public class WindLoad
        {
            public string Name { get; set; }

            public WindLoad(string name)
            {
                Name = name;
            }
        }

        public class NotionalLoad
        {
            public string Name { get; set; }
            public double Factor { get; set; }
            public string BaseLoadPatternName { get; set; }

            public NotionalLoad(string name, double factor, string baseLoadPatternName)
            {
                Name = name;
                Factor = factor;
                BaseLoadPatternName = baseLoadPatternName;
            }
        }

        public class TemperatureLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public TemperatureLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class SeismicLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public SeismicLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class AccidentalLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public AccidentalLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class UpliftLoad
        {
            public string Name { get; set; }
            public bool IsExisting { get; set; }

            public UpliftLoad(string name, bool isExisting)
            {
                Name = name;
                IsExisting = isExisting;
            }
        }

        public class EHFLoad
        {
            public string Name { get; set; }
            public double Factor { get; set; }
            public string BaseLoadPatternName { get; set; }

            public EHFLoad(string name, double factor, string baseLoadPatternName)
            {
                Name = name;
                Factor = factor;
                BaseLoadPatternName = baseLoadPatternName;
            }
        }

        #endregion

    }
}
