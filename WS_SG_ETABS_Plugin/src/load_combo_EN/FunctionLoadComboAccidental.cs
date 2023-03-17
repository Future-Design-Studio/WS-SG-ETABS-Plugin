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

            if (AccidentalLoadList.Count != 0)
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
                    int caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case2 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case3 ------------------------- COMBI 16 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (16 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 16); caseloop++)
                        {
                            //first 16 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 16 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 16 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if ((caseloop - 8) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 16)
                            {
                                if ((internalloop - 12) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 16 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }


                            //add windz load combis x 8 -------------------------
                            else if (internalloop <= 8)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], wlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if (WindVertLoadList.Count == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[1], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (WindVertLoadList.Count == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], -wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check WIND Vertical Loop Accidental load");
                                    return true;
                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 16)
                            {
                                if (internalloop == 13)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 14)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 15)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 16)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case4 ------------------------- COMBI 16 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (16 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 16); caseloop++)
                        {
                            //first 16 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 16 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 16 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if ((caseloop - 8) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((caseloop - 8) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 16)
                            {
                                if ((internalloop - 12) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 12) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 16 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }


                            //add windz load combis x 8 -------------------------
                            else if (internalloop <= 8)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], wlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if (WindVertLoadList.Count == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[1], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (WindVertLoadList.Count == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], -wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check WIND Vertical Loop Accidental load");
                                    return true;
                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 16)
                            {
                                if (internalloop == 13)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 14)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 15)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 16)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case5 ------------------------- COMBI 16 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (16 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 16); caseloop++)
                        {
                            //first 16 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 16 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if ((caseloop - 8) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 16)
                            {
                                if ((internalloop - 12) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 16 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }


                            //add windz load combis x 8 -------------------------
                            else if (internalloop <= 8)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], wlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if (WindVertLoadList.Count == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[1], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (WindVertLoadList.Count == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], -wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check WIND Vertical Loop Accidental load");
                                    return true;
                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 16)
                            {
                                if (internalloop == 13)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 14)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 15)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 16)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case6 ------------------------- COMBI 16 * Number AL

                    dlFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (16 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 16); caseloop++)
                        {
                            //first 16 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 16 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 16 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if ((caseloop - 8) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((caseloop - 8) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 16)
                            {
                                if ((internalloop - 12) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 12) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 16 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }


                            //add windz load combis x 8 -------------------------
                            else if (internalloop <= 8)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], wlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 12)
                            {
                                if (WindVertLoadList.Count == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[1], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (WindVertLoadList.Count == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windvertcaseList[0], -wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check WIND Vertical Loop Accidental load");
                                    return true;
                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 16)
                            {
                                if (internalloop == 13)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 14)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 15)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 16)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case7 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case8 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case9 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case10 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case11 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case12 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                }

                #endregion

                #region No wind vertical, have wind horizontal, have temperature

                if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
                {
                    int numberOfCombi = ((4 * 2) + (8 * 2) + (8 * 2) + (8 * 2) + (8 * 2) + (4 * 2)) * ALneedtimes2 * alcaselist.Count;

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
                    int caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case2 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][1]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][1]));
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case3 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }
                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 8)
                            {
                                if (internalloop == 5)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 6)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 7)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 8)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case4 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 8)
                            {
                                if (internalloop == 5)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 6)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 7)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 8)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case5 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 8)
                            {
                                if (internalloop == 5)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 6)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 7)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 8)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case6 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    alFactor = 1;
                    wlFactor = 0.2;
                    nlFactor = 1;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis WIND------------------------

                            //add first 4 wind load combis -------------------------
                            if (internalloop <= 4)
                            {
                                if (WindLoadList.Count == 4)
                                {

                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[(internalloop - 1)], wlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }

                                }
                                else
                                {

                                    if (internalloop == 1)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 2)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[0], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 3)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else if (internalloop == 4)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, windcaseList[1], -wlFactor);
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Check WIND Loop Accidental load");
                                        return true;
                                    }

                                }
                            }

                            //add next 4 notional loads -------------------------
                            else if (internalloop <= 8)
                            {
                                if (internalloop == 5)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 6)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[0], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 7)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 8)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, notional2caseList[1], -nlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Check Notional Loop Accidental load");
                                    return true;
                                }
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case7 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case8 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    llFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis LL------------------------
                            foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                                if (isError)
                                {
                                    return true;
                                }
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                                    {
                                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                                        if (isError)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case9 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case10 ------------------------- COMBI 8 * Number AL

                    dlFactor = 1;
                    alFactor = 1;
                    tlFactor = 0.5;

                    endcase = endcase + (8 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 8); caseloop++)
                        {
                            //first 8 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 8 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 8 load combis EHF------------------------
                            if (internalloop <= 4)
                            {
                                if (internalloop == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if (internalloop == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (internalloop <= 8)
                            {
                                if ((internalloop - 4) == 1)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 2)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 3)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else if ((internalloop - 4) == 4)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }

                            //first 8 load combis Temp------------------------
                            if (internalloop <= 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[1], tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop <= 8 && tempcaselist.Count == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, tempcaselist[0], -tlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check Temperature Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case11 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    sdlFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis SDL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_SDL", sdlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (SDL)", -sdlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                    #region Case12 ------------------------- COMBI 4 * Number AL

                    dlFactor = 1;
                    alFactor = 1;

                    endcase = endcase + (4 * ALneedtimes2 * alcaselist.Count);
                    whileloopnumber = 0;
                    caseloop = new int();
                    while (loadcombinumber <= endcase)
                    {
                        int internalloop = 1;
                        for (caseloop = loadcombinumber; caseloop < (loadcombinumber + 4); caseloop++)
                        {
                            //first 4 load combis DL------------------------
                            isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_DL", dlFactor);
                            if (isError)
                            {
                                return true;
                            }

                            //first 4 load combis AL------------------------
                            if (ALneedtimes2 == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[whileloopnumber], alFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (whileloopnumber % 2 != 0)
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[((whileloopnumber - 1) / 2)], -alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, alcaselist[(whileloopnumber / 2)], alFactor);
                                    if (isError)
                                    {
                                        return true;
                                    }
                                }
                            }

                            //first 4 load combis EHF------------------------
                            if (internalloop == 1)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 2)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFx (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 3)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else if (internalloop == 4)
                            {
                                isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + caseloop, 0, "P_EHFy (DL)", -dlFactor);
                                if (isError)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Check EHF Loop Accidental load");
                                return true;
                            }
                            internalloop++;
                        }
                        whileloopnumber++;
                        loadcombinumber = caseloop;
                    }

                    #endregion

                }

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