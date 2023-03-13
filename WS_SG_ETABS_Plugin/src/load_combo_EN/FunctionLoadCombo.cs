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
            bool addAnalysisCase = false;

            foreach (var deadLoad in deadLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, deadLoad.Name, 1, "Dead", deadLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            int i = 0;
            string DeadLoadCaseName = null;
            foreach (var deadLoad in deadLoadList)
            {
                DeadLoadCaseName = "P_Dead";
                i++;
            }
            if (i > 0)
            { 
                isError = src.HelperEtabs.Addnewloadcase(SapModel, DeadLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }
            

            string[] LoadType = new string[50];
            string[] LoadName = new string[50];
            double[] SF = new double[50];
            i = 0;
            int NumberLoads = i;
            foreach (var deadLoad in deadLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = deadLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, DeadLoadCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            





            
            foreach (var superdeadLoad in SDLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, superdeadLoad.Name, 0, "SuperDead", superdeadLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            i = 0;
            string SuperDeadLoadCaseName = null;
            foreach (var superdeadLoad in SDLoadList)
            {
                SuperDeadLoadCaseName = "P_SuperDead";
                i++; 
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Addnewloadcase(SapModel, SuperDeadLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var superdeadLoad in SDLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = superdeadLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, SuperDeadLoadCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            






            foreach (var liveLoad in LiveLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, liveLoad.Name, 0, "Live", liveLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var liveLoad in LiveLoadList)
            {
                string LiveLoadCaseName = "P_Live(" + liveLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, LiveLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var liveLoad in LiveLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = liveLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_Live(" + liveLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            






            foreach (var windLoad in WindLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, windLoad.Name, 0, "Wind", windLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var windLoad in WindLoadList)
            {
                string WindLoadCaseName = "P_Wind(" + windLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, WindLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }
            
            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var windLoad in WindLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = windLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_Wind(" + windLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            




            

            foreach (var notionalDLoad in NotionalDXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalDLoad.Name, 0, "Notional", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var notionalDLoad in NotionalDYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalDLoad.Name, 0, "Notional", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var notionalSDLoad in NotionalSDXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalSDLoad.Name, 0, "Notional", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var notionalSDLoad in NotionalSDYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalSDLoad.Name, 0, "Notional", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            if (NotionalDXLoadList.Count > 0 || NotionalDYLoadList.Count > 0 || NotionalSDXLoadList.Count > 0 || NotionalSDYLoadList.Count > 0)
            {
                i = 0;
                List<string> notionalloadcaseName = new List<string> { "P_N0", "P_N90", "P_N45", "P_N135" };
                while (i < 4)
                {
                    string notionalLoadCaseName = notionalloadcaseName[i];
                    isError = src.HelperEtabs.Addnewloadcase(SapModel, notionalLoadCaseName);
                    if (isError)
                    {
                        return true;
                    }
                    i++;
                }
                
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var notionalDLoad in NotionalDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads = i + 1;
                SF[i] = 1.00;
                i++;
            }
            if (i>0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_N0", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var notionalDLoad in NotionalDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_N90", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var notionalDLoad in NotionalDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            foreach (var notionalDLoad in NotionalDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_N45", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var notionalDLoad in NotionalDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = -0.707;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads++;
                SF[i] = -0.707;
                i++;
            }
            foreach (var notionalDLoad in NotionalDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            foreach (var notionalSDLoad in NotionalSDYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = notionalSDLoad.Name;
                NumberLoads++;
                SF[i] = 0.707;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_N135", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            






            foreach (var temperatureLoad in TemperatureLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, temperatureLoad.Name, 0, "Temperature", temperatureLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var temperatureLoad in TemperatureLoadList)
            {
                string TemperatureLoadCaseName = "P_Temp(" + temperatureLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, TemperatureLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }
            

            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var temperatureLoad in TemperatureLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = temperatureLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_Temp(" + temperatureLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            foreach (var seismicLoad in SeismicLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, seismicLoad.Name, 0, "Seismic", seismicLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var seismicLoad in SeismicLoadList)
            {
                string SeismicLoadCaseName = "P_Seismic(" + seismicLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, SeismicLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var seismicLoad in SeismicLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = seismicLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_Seismic(" + seismicLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            foreach (var accidentalLoad in AccidentalLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, accidentalLoad.Name, 0, "Accidental", accidentalLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var accidentalLoad in AccidentalLoadList)
            {
                string AccidentalLoadCaseName = "P_AL(" + accidentalLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, AccidentalLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var accidentalLoad in AccidentalLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = accidentalLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_AL(" + accidentalLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            foreach (var upliftLoad in UpliftLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, upliftLoad.Name, 0, "Uplift", upliftLoad.IsExisting, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var upliftLoad in UpliftLoadList)
            {
                string UpliftLoadCaseName = "P_UPLIFT(" + upliftLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, UpliftLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var upliftLoad in UpliftLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = upliftLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_UPLIFT(" + upliftLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            foreach (var ehfLoad in EHFDLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }

            i = 0;
            string EHFXDLCaseName = null;
            foreach (var ehfLoad in EHFDLXLoadList)
            {
                EHFXDLCaseName = "P_EHFx (DL)";
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Addnewloadcase(SapModel, EHFXDLCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var ehfLoad in EHFDLXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, EHFXDLCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }







            foreach (var ehfLoad in EHFDLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }

            i = 0;
            string EHFYDLCaseName = null;
            foreach (var ehfLoad in EHFDLYLoadList)
            {
                EHFYDLCaseName = "P_EHFy (DL)";
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Addnewloadcase(SapModel, EHFYDLCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var ehfLoad in EHFDLYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, EHFYDLCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }







            foreach (var ehfLoad in EHFSDLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }

            i = 0;
            string EHFXSDLCaseName = null;
            foreach (var ehfLoad in EHFSDLXLoadList)
            {
                EHFXSDLCaseName = "P_EHFx (SDL)";
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Addnewloadcase(SapModel, EHFXSDLCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var ehfLoad in EHFSDLXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, EHFXSDLCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }







            foreach (var ehfLoad in EHFSDLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }

            i = 0;
            string EHFYSDLCaseName = null;
            foreach (var ehfLoad in EHFSDLYLoadList)
            {
                EHFYSDLCaseName = "P_EHFy (SDL)";
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Addnewloadcase(SapModel, EHFYSDLCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = i;
            foreach (var ehfLoad in EHFSDLYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                NumberLoads++;
                SF[i] = 1.00;
                i++;
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.Updateloadcase(SapModel, EHFYSDLCaseName, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }







            foreach (var ehfLoad in EHFLLXLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var ehfLoad in EHFLLXLoadList)
            {
                string UpliftLoadCaseName = "P_EHFx (LL_" + ehfLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, UpliftLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var ehfLoad in EHFLLXLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_EHFx (LL_" + ehfLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            foreach (var ehfLoad in EHFLLYLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var ehfLoad in EHFLLYLoadList)
            {
                string UpliftLoadCaseName = "P_EHFy (LL_" + ehfLoad.Name + ")";
                isError = src.HelperEtabs.Addnewloadcase(SapModel, UpliftLoadCaseName);
                if (isError)
                {
                    return true;
                }
            }


            LoadType = new string[50];
            LoadName = new string[50];
            SF = new double[50];
            i = 0;
            NumberLoads = 1;
            foreach (var ehfLoad in EHFLLYLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = ehfLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.Updateloadcase(SapModel, "P_EHFy (LL_" + ehfLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            return false;
        }
    }
}
