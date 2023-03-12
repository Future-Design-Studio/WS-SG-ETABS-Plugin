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
        public static bool CreateLoadPatternAndCase(cSapModel SapModel, cPluginCallback Plugin, 
            List<src.load_combo_EN.ClassLoadCombo.DeadLoad> deadLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SDLLoad> SDLoadList,
            List<src.load_combo_EN.ClassLoadCombo.LiveLoad> LiveLoadList,
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> WindLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalSDXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalSDYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.TemperatureLoad> TemperatureLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SeismicLoad> SeismicLoadList,
            List<src.load_combo_EN.ClassLoadCombo.AccidentalLoad> AccidentalLoadList,
            List<src.load_combo_EN.ClassLoadCombo.UpliftLoad> UpliftLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFSDLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFSDLYLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLLXLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLLYLoadList)
        {
            bool isError = false;
            bool addAnalysisCase = new bool();

            foreach (var deadLoad in deadLoadList)
            {                    
                isError = src.HelperEtabs.AddLoadPattern(SapModel, deadLoad.Name, 1, "Dead", deadLoad.IsExisting, addAnalysisCase);
            }
            if (isError)
            {
                return true;
            }

            foreach (var deadLoad in deadLoadList)
            {
                string DeadLoadName = "PDead";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, DeadLoadName);
            }
            if (isError)
            {
                return true;
            }

            string[] LoadType = new string[20];
            string[] LoadName = new string[20];
            double[] SF = new double[20];
            int i = 0;
            foreach (var deadLoad in deadLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = deadLoad.Name;
                int NumberLoads = i + 1;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "PDead", NumberLoads, LoadType, LoadName, SF);
                i++;
            }
            if (isError)
            {
                return true;
            }

            foreach (var superdeadLoad in SDLoadList)
            {
                if (i == 0)
                {
                    addAnalysisCase = true;
                }
                else
                {
                    addAnalysisCase = false;
                }

                isError = src.HelperEtabs.AddLoadPattern(SapModel, superdeadLoad.Name, 0, "SuperDead", superdeadLoad.IsExisting, addAnalysisCase);
                i++;
            }
            if (isError)
            {
                return true;
            }

            foreach (var liveLoad in LiveLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, liveLoad.Name, 0, "Live", liveLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var windLoad in WindLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, windLoad.Name, 0, "Wind", windLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalDLoad in NotionalDXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalDLoad.Name, 0, "Notional", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalDLoad in NotionalDYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalDLoad.Name, 0, "Notional", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalSDLoad in NotionalSDXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalSDLoad.Name, 0, "Notional", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalSDLoad in NotionalSDYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalSDLoad.Name, 0, "Notional", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var temperatureLoad in TemperatureLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, temperatureLoad.Name, 0, "Temperature", temperatureLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var seismicLoad in SeismicLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, seismicLoad.Name, 0, "Seismic", seismicLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var accidentalLoad in AccidentalLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, accidentalLoad.Name, 0, "Accidental", accidentalLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var upliftLoad in UpliftLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, upliftLoad.Name, 0, "Uplift", upliftLoad.IsExisting, true);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFDLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFDLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFSDLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFSDLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFLLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFLLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, false);
            }
            if (isError)
            {
                return true;
            }

            return false;
        }
    }
}
