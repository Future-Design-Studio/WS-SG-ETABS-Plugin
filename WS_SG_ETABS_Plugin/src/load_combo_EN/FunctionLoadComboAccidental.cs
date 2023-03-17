using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    internal class FunctionLoadComboAccidental
    {
        public static bool AddLoadCombinationAccidental(cSapModel SapModel, cPluginCallback Plugin,
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
            List<src.load_combo_EN.ClassLoadCombo.WindLoad> WindVertLoadList,
            Dictionary<string, string> llcaseDict,
            List<string> windcaseList,
            List<string> notional2caseList,
            List<string> windvertcaseList,
            List<string> tempcaselist,
            List<string> seismiccaselist,
            List<string> alcaselist,
            List<string> upliftcaselist,
            Dictionary<string, string> ehfllxcaseDict,
            Dictionary<string, string> ehfllycaseDict,
            Dictionary<string, List<double>> quasiPermanentDict,
            int ALneedtimes2)

        {

            int numberloadcombi = 1;
            string basecombiname = "Acomb-";
            double dlFactor = new double();
            double sdlFactor = new double();
            double llFactor = new double();
            double wlFactor = new double();
            double tlFactor = new double();
            double alFactor = new double();
            double nlFactor = new double();
            int notionalcaselistIndex = new int();
            bool isError = false;
            bool addAnalysisCase = false;

            #region Load Combination Accidental

            //from here start creating load combinations Accidental
            numberloadcombi = 1;
            basecombiname = "Acomb-";
            dlFactor = new double();
            sdlFactor = new double();
            llFactor = new double();
            wlFactor = new double();
            tlFactor = new double();
            nlFactor = new double();
            alFactor = new double();
            notionalcaselistIndex = new int();

            if (SeismicLoadList.Count != 0)
            {

                MessageBox.Show("Starting Load Combination set for Accidental loads");

                #region Have wind vertical, have wind horizontal, have temperature

                if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
                {
                    int numberOfCombi = ((4 * 2) + (16 * 2) + (16 * 2) + (8 * 2) + (8 * 2) + (4 * 2)) * ALneedtimes2 * alcaselist.Count;

                    //add envelope combi for all B
                    isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                    if (isError)
                    {
                        return true;
                    }

                    //create number of combinations required
                    while (numberloadcombi <= numberOfCombi)
                    {
                        isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                        if (isError)
                        {
                            return true;
                        }
                        //update envelope load combi
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + "ENV", 1, basecombiname + numberloadcombi, 1);
                        if (isError)
                        {
                            return true;
                        }
                        numberloadcombi++;
                    }

                    int loadcombinumber = 1;

                    #region Case1 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;

                    int endcase = 4 * ALneedtimes2 * alcaselist.Count;
                    int whileloopnumber = 0;
                    while (loadcombinumber <= endcase)
                    {
                        for (loadcombinumber = loadcombinumber; loadcombinumber < (loadcombinumber + 4); loadcombinumber++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (loadcombinumber - (whileloopnumber * 4) == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            
                        }






                        whileloopnumber++;
                    }

                    #endregion

                    #region Case2 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    while (loadcombinumber <= endcase)
                    {
                        for (loadcombinumber = loadcombinumber; loadcombinumber < (loadcombinumber + 4); loadcombinumber++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (loadcombinumber - (whileloopnumber * 4) == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                        }






                        whileloopnumber++;
                    }

                    #endregion

                    #region Case3 ------------------------- COMBI 16 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (16 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    while (loadcombinumber <= endcase)
                    {
                        for (loadcombinumber = loadcombinumber; loadcombinumber < (loadcombinumber + 4); loadcombinumber++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (loadcombinumber - (whileloopnumber * 4) == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (loadcombinumber - (whileloopnumber * 4) == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + loadcombinumber, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                        }






                        whileloopnumber++;
                    }

                    #endregion



                    

                }

                #endregion


                #region No wind vertical, have wind horizontal, have temperature

                #endregion

                #region No wind vertical, have wind horizontal, no temperature

                #endregion

                #region No wind vertical, no wind horizontal, have temperature

                #endregion

                #region No wind vertical, no wind horizontal, no temperature

                else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
                {
                    int numberOfCombi = ((4 * 2) + (4 * 2)) * ALneedtimes2 * alcaselist.Count;

                    //add envelope combi for all B
                    isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                    if (isError)
                    {
                        return true;
                    }

                    //create number of combinations required
                    while (numberloadcombi <= numberOfCombi)
                    {
                        isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                        if (isError)
                        {
                            return true;
                        }
                        //update envelope load combi
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + "ENV", 1, basecombiname + numberloadcombi, 1);
                        if (isError)
                        {
                            return true;
                        }
                        numberloadcombi++;
                    }
                }

                #endregion

                #region Have wind vertical, have wind horizontal, no temperature

                #endregion

                #region Have wind vertical, no wind horizontal, have temperature

                #endregion

                #region Have wind vertical, no wind horizontal, no temperature

                #endregion

                #endregion

                MessageBox.Show("Finished creating Load Combination set for Accidental loads");
            }
                            
            else
            {
                MessageBox.Show("No Accidental Loads defined, skipping Accidental Load Combination Set");
            }
            return false;
        }
    }
}