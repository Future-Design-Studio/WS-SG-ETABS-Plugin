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
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLLYLoadList,
            int NotionalDirectionFactor,
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> WindVertLoadList)
        {
            Dictionary<string, string> llcaseDict = new Dictionary<string, string>();
            List<string> windcaseList = new List<string>();
            List<string> notional2caseList = new List<string>() { "P_NX", "P_NY" };
            List<string> windvertcaseList = new List<string>();
            List<string> tempcaselist = new List<string>();
            List<string> seismiccaselist = new List<string>();
            List<string> alcaselist = new List<string>();
            List<string> upliftcaselist = new List<string>();
            Dictionary<string, string> ehfllxcaseDict = new Dictionary<string, string>();
            Dictionary<string, string> ehfllycaseDict = new Dictionary<string, string>();

            Dictionary<string, List<double>> quasiPermanentDict = new Dictionary<string, List<double>>()
            {
                { "A: dometic,residential areas", new List<double>(){0.7, 0.5, 0.3 } },
                { "B: office areas", new List<double>(){0.7, 0.5, 0.3 } },
                { "C: congregation areas", new List<double>(){0.7, 0.7, 0.6 } },
                { "D: shopping areas", new List<double>(){0.7, 0.7, 0.6 } },
                { "E: storage areas", new List<double>(){1.0, 0.9, 0.8 } },
                { "F: traffic area, vw<30kN", new List<double>(){0.7, 0.7, 0.6 } },
                { "G: traffic area, 30kN<vw<160kN", new List<double>(){0.7, 0.5, 0.3 } },
                { "H: roofs", new List<double>(){0.7, 0, 0 } },
                { "Wind load", new List<double>(){0.5, 0.2, 0 } },
                { "Temperature", new List<double>(){0.6, 0.5, 0 } },
            };

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
                DeadLoadCaseName = "P_DL";
                i++;
            }
            if (i > 0)
            { 
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, DeadLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, DeadLoadCaseName, NumberLoads, LoadType, LoadName, SF);
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
                SuperDeadLoadCaseName = "P_SDL";
                i++; 
            }
            if (i > 0)
            {
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, SuperDeadLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, SuperDeadLoadCaseName, NumberLoads, LoadType, LoadName, SF);
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
                string LiveLoadCaseName = "P_LL (" + liveLoad.Name + ")";


                llcaseDict.Add(LiveLoadCaseName, liveLoad.CategoryPhi);


                isError = src.HelperEtabs.AddNewLoadCase(SapModel, LiveLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_LL (" + liveLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }
            






            foreach (var windLoad in WindLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, windLoad.Name, 0, "Wind", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }
            

            foreach (var windLoad in WindLoadList)
            {
                string WindLoadCaseName = "P_" + windLoad.Name;


                windcaseList.Add(WindLoadCaseName);


                isError = src.HelperEtabs.AddNewLoadCase(SapModel, WindLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_" + windLoad.Name, NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }







            foreach (var windvertLoad in WindVertLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, windvertLoad.Name, 0, "Wind", false, addAnalysisCase);
                if (isError)
                {
                    return true;
                }
            }


            foreach (var windvertLoad in WindVertLoadList)
            {
                string WindLoadCaseName = "P_" + windvertLoad.Name;


                windvertcaseList.Add(WindLoadCaseName);


                isError = src.HelperEtabs.AddNewLoadCase(SapModel, WindLoadCaseName);
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
            foreach (var windvertLoad in WindVertLoadList)
            {
                LoadType[i] = "Load";
                LoadName[i] = windvertLoad.Name;
                SF[i] = 1.00;
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_" + windvertLoad.Name, NumberLoads, LoadType, LoadName, SF);
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
            

            if (NotionalDirectionFactor == 4)
            { 
                if (NotionalDXLoadList.Count > 0 || NotionalDYLoadList.Count > 0 || NotionalSDXLoadList.Count > 0 || NotionalSDYLoadList.Count > 0)
                {
                    i = 0;
                    List<string> notionalloadcaseName = new List<string> { "P_N0", "P_N90", "P_N45", "P_N135" };
                    while (i < 4)
                    {
                        string notionalLoadCaseName = notionalloadcaseName[i];
                        isError = src.HelperEtabs.AddNewLoadCase(SapModel, notionalLoadCaseName);
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
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_N0", NumberLoads, LoadType, LoadName, SF);
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
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_N90", NumberLoads, LoadType, LoadName, SF);
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
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_N45", NumberLoads, LoadType, LoadName, SF);
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
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_N135", NumberLoads, LoadType, LoadName, SF);
                    if (isError)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (NotionalDXLoadList.Count > 0 || NotionalDYLoadList.Count > 0 || NotionalSDXLoadList.Count > 0 || NotionalSDYLoadList.Count > 0)
                {
                    i = 0;
                    List<string> notionalloadcaseName = new List<string> { "P_NX", "P_NY"};
                    while (i < 2)
                    {
                        string notionalLoadCaseName = notionalloadcaseName[i];
                        isError = src.HelperEtabs.AddNewLoadCase(SapModel, notionalLoadCaseName);
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
                if (i > 0)
                {
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_NX", NumberLoads, LoadType, LoadName, SF);
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
                    isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_NY", NumberLoads, LoadType, LoadName, SF);
                    if (isError)
                    {
                        return true;
                    }
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
                string TemperatureLoadCaseName = "P_Temp (" + temperatureLoad.Name + ")";

                tempcaselist.Add(TemperatureLoadCaseName);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, TemperatureLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Temp (" + temperatureLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                string SeismicLoadCaseName = "P_Seismic (" + seismicLoad.Name + ")";

                seismiccaselist.Add(SeismicLoadCaseName);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, SeismicLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Seismic (" + seismicLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                string AccidentalLoadCaseName = "P_AL (" + accidentalLoad.Name + ")";

                alcaselist.Add(AccidentalLoadCaseName);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, AccidentalLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_AL (" + accidentalLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                string UpliftLoadCaseName = "P_UPLIFT (" + upliftLoad.Name + ")";

                upliftcaselist.Add(UpliftLoadCaseName);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, UpliftLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_UPLIFT (" + upliftLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EHFXDLCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, EHFXDLCaseName, NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EHFYDLCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, EHFYDLCaseName, NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EHFXSDLCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, EHFXSDLCaseName, NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EHFYSDLCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, EHFYSDLCaseName, NumberLoads, LoadType, LoadName, SF);
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
                string EFHLLXLoadCaseName = "P_EHFx (LL_" + ehfLoad.BaseLoadPatternName + ")";

                ehfllxcaseDict.Add(EFHLLXLoadCaseName, ehfLoad.QuasiPermenantChosen);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EFHLLXLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_EHFx (LL_" + ehfLoad.BaseLoadPatternName + ")", NumberLoads, LoadType, LoadName, SF);
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
                string EHFLLYLoadCaseName = "P_EHFy (LL_" + ehfLoad.BaseLoadPatternName + ")";

                ehfllycaseDict.Add(EHFLLYLoadCaseName, ehfLoad.QuasiPermenantChosen);

                isError = src.HelperEtabs.AddNewLoadCase(SapModel, EHFLLYLoadCaseName);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_EHFy (LL_" + ehfLoad.BaseLoadPatternName + ")", NumberLoads, LoadType, LoadName, SF);
                if (isError)
                {
                    return true;
                }
            }






            src.HelperEtabs.DatabaseTable databaseTableAutoNotional = new src.HelperEtabs.DatabaseTable();
            databaseTableAutoNotional = src.HelperEtabs.GetTableEdit(SapModel, "Load Pattern Definitions - Auto Notional Loads");
                        
            foreach (var notionalDx in NotionalDXLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", notionalDx.Name},
                    {"BasePattern", notionalDx.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(notionalDx.Factor)},
                    {"LoadDir", "X"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var notionalDy in NotionalDYLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", notionalDy.Name},
                    {"BasePattern", notionalDy.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(notionalDy.Factor)},
                    {"LoadDir", "Y"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var notionalSDx in NotionalSDXLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", notionalSDx.Name},
                    {"BasePattern", notionalSDx.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(notionalSDx.Factor)},
                    {"LoadDir", "X"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var notionalSDy in NotionalSDYLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", notionalSDy.Name},
                    {"BasePattern", notionalSDy.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(notionalSDy.Factor)},
                    {"LoadDir", "Y"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfDx in EHFDLXLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfDx.Name},
                    {"BasePattern", ehfDx.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfDx.Factor)},
                    {"LoadDir", "X"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfDy in EHFDLYLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfDy.Name},
                    {"BasePattern", ehfDy.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfDy.Factor)},
                    {"LoadDir", "Y"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfSDx in EHFSDLXLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfSDx.Name},
                    {"BasePattern", ehfSDx.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfSDx.Factor)},
                    {"LoadDir", "X"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfSDy in EHFSDLYLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfSDy.Name},
                    {"BasePattern", ehfSDy.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfSDy.Factor)},
                    {"LoadDir", "Y"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfLLx in EHFLLXLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfLLx.Name},
                    {"BasePattern", ehfLLx.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfLLx.Factor)},
                    {"LoadDir", "X"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            foreach (var ehfLLy in EHFLLYLoadList)
            {
                Dictionary<string, string> row = new Dictionary<string, string>()
                {
                    {"LoadPattern", ehfLLy.Name},
                    {"BasePattern", ehfLLy.BaseLoadPatternName},
                    {"LoadRatio", Convert.ToString(ehfLLy.Factor)},
                    {"LoadDir", "Y"},
                };

                databaseTableAutoNotional.TableInList.Add(row);
            }

            isError = src.HelperEtabs.ApplyEtabsTable(SapModel, databaseTableAutoNotional);

            if (isError)
            {
                return true;
            }




            //from here start creating load combinations
            int numberBloadcombi = 1;
            string baseBcombiname = "U_Comb_B-";

            if (NotionalDirectionFactor == 2 && WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 2 * 6) + (16 * 4) + (16 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (16 * 2) + (2 * 2 * 4) + (1 * 2 * 4);
                
                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, baseBcombiname + "ENV");
                if (isError)
                {
                    return true;
                }

                //create number of combinations required
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    //update envelope load combi
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, baseBcombiname + "ENV", 1, baseBcombiname + numberBloadcombi, 1);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }

                #region First 32 load combis for case1 ------------------------- COMBI 1-32

                double dlFactor = 1.35;
                double sdlFactor = 1.35;
                double llFactor = 1.5;
                double wlFactor = 0.75;
                double tlFactor = 0.9;
                double nlFactor = 1;

                //first 32 load combis DL SDL LL TEMP ------------------------- COMBI 1-32
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails  in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <=16)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 16 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 16 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 1-32
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 1-4
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 1; combi1 <= 4; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 1; combi1 <= 3; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 5-12
                for (int combi1 = 5; combi1 <= 12; combi1++)
                {
                    if (combi1 <= 8)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 >8 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 13-16
                int notionalcaselistIndex = 0;
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 17-20
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 17; combi1 <= 19; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 21-28
                for (int combi1 = 21; combi1 <= 28; combi1++)
                {
                    if (combi1 <= 24)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 29-32
                notionalcaselistIndex = 0;
                for (int combi1 = 29; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case2 ------------------------- COMBI 33-64

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL LL TEMP ------------------------- COMBI 33-64
                for (int combi1 = 33; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 48)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 48 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 48 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 33-64
                for (int combi1 = 33; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 33-36
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 33; combi1 <= 36; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 33; combi1 <= 36; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 37-44
                for (int combi1 = 37; combi1 <= 44; combi1++)
                {
                    if (combi1 <= 40)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 45-48
                notionalcaselistIndex = 0;
                for (int combi1 = 45; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 49-52
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 49; combi1 <= 52; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 49; combi1 <= 52; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 53-60
                for (int combi1 = 53; combi1 <= 60; combi1++)
                {
                    if (combi1 <= 56)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 61-64
                notionalcaselistIndex = 0;
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case3 ------------------------- COMBI 65-96

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL SDL LL TEMP ------------------------- COMBI 65-96
                for (int combi1 = 65; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 80)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 65-96
                for (int combi1 = 65; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 65-68
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 65; combi1 <= 68; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 65; combi1 <= 67; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 69-76
                for (int combi1 = 69; combi1 <= 76; combi1++)
                {
                    if (combi1 <= 72)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 77-80
                notionalcaselistIndex = 0;
                for (int combi1 = 77; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 81-84
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 81; combi1 <= 84; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 81; combi1 <= 83; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 85-92
                for (int combi1 = 85; combi1 <= 92; combi1++)
                {
                    if (combi1 <= 88)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 93-96
                notionalcaselistIndex = 0;
                for (int combi1 = 93; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case4 ------------------------- COMBI 97-128

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL LL TEMP ------------------------- COMBI 97-128
                for (int combi1 = 97; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 112)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 112 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 112 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 97-128
                for (int combi1 = 97; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 97-100
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 97; combi1 <= 100; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 97; combi1 <= 99; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 101-108
                for (int combi1 = 101; combi1 <= 108; combi1++)
                {
                    if (combi1 <= 104)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 104 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 104 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 109-112
                notionalcaselistIndex = 0;
                for (int combi1 = 109; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 113-116
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 113; combi1 <= 116; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 113; combi1 <= 115; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 117-124
                for (int combi1 = 117; combi1 <= 124; combi1++)
                {
                    if (combi1 <= 120)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 120 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 120 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 125-128
                notionalcaselistIndex = 0;
                for (int combi1 = 125; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case5 ------------------------- COMBI 129-160

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 32 load combis DL SDL LL TEMP ------------------------- COMBI 129-160
                for (int combi1 = 129; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 144)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 144 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 144 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 129-160
                for (int combi1 = 129; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 129-132
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 129; combi1 <= 132; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 129; combi1 <= 131; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 133-140
                for (int combi1 = 133; combi1 <= 140; combi1++)
                {
                    if (combi1 <= 136)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 141-144
                notionalcaselistIndex = 0;
                for (int combi1 = 141; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 145-148
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 145; combi1 <= 148; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 145; combi1 <= 147; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 149-156
                for (int combi1 = 149; combi1 <= 156; combi1++)
                {
                    if (combi1 <= 152)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 157-160
                notionalcaselistIndex = 0;
                for (int combi1 = 157; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case6 ------------------------- COMBI 161-192

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 32 load combis DL LL TEMP ------------------------- COMBI 161-192
                for (int combi1 = 161; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 176)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 176 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 176 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 161-192
                for (int combi1 = 161; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 161-164
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 161; combi1 <= 164; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 161; combi1 <= 163; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 165-172
                for (int combi1 = 165; combi1 <= 172; combi1++)
                {
                    if (combi1 <= 168)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 173-176
                notionalcaselistIndex = 0;
                for (int combi1 = 173; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 177-180
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 177; combi1 <= 180; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        windcaselistIndex++;
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 177; combi1 <= 179; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 181-188
                for (int combi1 = 181; combi1 <= 188; combi1++)
                {
                    if (combi1 <= 184)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 189-192
                notionalcaselistIndex = 0;
                for (int combi1 = 189; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






































            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 2 * 6) + (8 * 4) + (8 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (8 * 2) + (2 * 2 * 4) + (1 * 2 * 4);
                
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 4) + (4 * 2) + (8 * 2) + (4 * 2);

                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 4) + (4 * 2) + (16 * 2) + (4 * 2);
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2 * 6) + (8 * 4) + (8 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (8 * 2) + (2 * 2 * 4) + (1 * 2 * 4);
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (2 * 4 * 4) + (4 * 2) + (2 * 4 * 2) + (4 * 2);
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 4) + (4 * 2) + (8 * 2) + (4 * 2);
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else if (NotionalDirectionFactor == 2 && WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (4 * 2) + (4 * 2);
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }
                MessageBox.Show("Error, combinations for missing vert wind or temp or lateral wind not available in free version");
                return true;


            }
            else
            {
                MessageBox.Show("Error, combinations for 8 direction wind load support not available in free version");
                return true;
            }








            return false;
        }
    }
}
