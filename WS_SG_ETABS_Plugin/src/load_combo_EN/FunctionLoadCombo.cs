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
            Dictionary<string, string> windcaseDict = new Dictionary<string, string>();
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
                DeadLoadCaseName = "P_Dead";
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
                SuperDeadLoadCaseName = "P_SuperDead";
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
                string LiveLoadCaseName = "P_Live(" + liveLoad.Name + ")";


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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Live(" + liveLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                string WindLoadCaseName = "P_Wind(" + windLoad.Name + ")";


                windcaseDict.Add(WindLoadCaseName, windLoad.Name);


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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Wind(" + windLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                string TemperatureLoadCaseName = "P_Temp(" + temperatureLoad.Name + ")";

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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Temp(" + temperatureLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_Seismic(" + seismicLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_AL(" + accidentalLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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
                isError = src.HelperEtabs.UpdateLoadCase(SapModel, "P_UPLIFT(" + upliftLoad.Name + ")", NumberLoads, LoadType, LoadName, SF);
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

                ehfllxcaseDict.Add(EFHLLXLoadCaseName, ehfLoad.BaseLoadPatternName);

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

                ehfllycaseDict.Add(EHFLLYLoadCaseName, ehfLoad.BaseLoadPatternName);

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
                while (numberBloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, baseBcombiname + numberBloadcombi);
                    if (isError)
                    {
                        return true;
                    }
                    numberBloadcombi++;
                }

                //load combination adding where all loads dont take into account +/-
                if (WindLoadList.Count == 4 && TemperatureLoadList.Count == 2 && WindVertLoadList.Count == 2)
                {
                    for (int combi1 = 1; combi1 <= 32; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_Dead", 1.35);
                        if (isError)
                        {
                            return true;
                        }

                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, "U_Comb_B-" + combi1, 0, "P_SuperDead", 1.35);
                        if (isError)
                        {
                            return true;
                        }

                        /*foreach (Dictionary<string, string> llquasi  in llcaseDict)
                            {

                            }*/
                    }
                }
                    

































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


            }
            ///check with kahjun, excel maybe problem
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


            }
            ///check with kahjun, excel maybe problem
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


            }
            else
            {
                MessageBox.Show("Error, 8 direction wind load support have not been added into this plugin");
                return true;
            }








            return false;
        }
    }
}
