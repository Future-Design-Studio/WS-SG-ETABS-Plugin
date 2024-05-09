using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    internal class FunctionLoadComboB
    {
        public static bool AddLoadCombinationB(cSapModel SapModel, cPluginCallback Plugin,
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
            Dictionary<string, List<double>> quasiPermanentDict)

        {


            #region Load Combination B

            //from here start creating load combinations B
            int numberloadcombi = 1;
            string basecombiname = "U_Comb_B-";
            double dlFactor = new double();
            double sdlFactor = new double();
            double llFactor = new double();
            double wlFactor = new double();
            double tlFactor = new double();
            double nlFactor = new double();
            int notionalcaselistIndex = new int();
            bool isError = false;
            bool addAnalysisCase = false;


            #region Have wind vertical, have wind horizontal, have temperature

            if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 2 * 6) + (16 * 4) + (16 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (16 * 2) + (2 * 2 * 4) + (1 * 2 * 4);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 32 load combis for case1 ------------------------- COMBI 1-32

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL SDL LL TEMP ------------------------- COMBI 1-32
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 16)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 16 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 16 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 1-32
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 13-16
                notionalcaselistIndex = 0;
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 48)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 48 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 48 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 33-64
                for (int combi1 = 33; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 80)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 65-96
                for (int combi1 = 65; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 112)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 112 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 112 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 97-128
                for (int combi1 = 97; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 104 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 104 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 120 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 120 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 144)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 144 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 144 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 129-160
                for (int combi1 = 129; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 176)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 176 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 176 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 161-192
                for (int combi1 = 161; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 16 load combis for case7 ------------------------- COMBI 193-208

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 16 load combis DL SDL LL ------------------------- COMBI 193-208
                for (int combi1 = 193; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 193-208
                for (int combi1 = 193; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 193-196
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 193; combi1 <= 196; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 193; combi1 <= 195; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 197-204
                for (int combi1 = 197; combi1 <= 204; combi1++)
                {
                    if (combi1 <= 200)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 200 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 200 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 205-208
                notionalcaselistIndex = 0;
                for (int combi1 = 205; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case8 ------------------------- COMBI 209-224

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 16 load combis DL LL ------------------------- COMBI 209-224
                for (int combi1 = 209; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 209-224
                for (int combi1 = 209; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 209-212
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 209; combi1 <= 212; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 209; combi1 <= 212; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 213-220
                for (int combi1 = 213; combi1 <= 220; combi1++)
                {
                    if (combi1 <= 216)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 216 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 216 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 221-224
                notionalcaselistIndex = 0;
                for (int combi1 = 221; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case9 ------------------------- COMBI 225-240

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL LL ------------------------- COMBI 225-240
                for (int combi1 = 225; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 225-240
                for (int combi1 = 225; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 225-228
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 225; combi1 <= 228; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 225; combi1 <= 228; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 229-236
                for (int combi1 = 229; combi1 <= 236; combi1++)
                {
                    if (combi1 <= 232)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 232 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 232 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 237-240
                notionalcaselistIndex = 0;
                for (int combi1 = 237; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case10 ------------------------- COMBI 241-256

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL LL ------------------------- COMBI 241-256
                for (int combi1 = 241; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 241-256
                for (int combi1 = 241; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 241-244
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 241; combi1 <= 244; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 241; combi1 <= 244; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 245-252
                for (int combi1 = 245; combi1 <= 252; combi1++)
                {
                    if (combi1 <= 248)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 248 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 248 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 253-256
                notionalcaselistIndex = 0;
                for (int combi1 = 253; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 32 load combis for case11 ------------------------- COMBI 257-288

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL SDL TEMP ------------------------- COMBI 257-288
                for (int combi1 = 257; combi1 <= 288; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 272)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 272 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 272 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 257-288
                for (int combi1 = 257; combi1 <= 288; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 257-260
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 257; combi1 <= 260; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 257; combi1 <= 260; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 261-268
                for (int combi1 = 261; combi1 <= 268; combi1++)
                {
                    if (combi1 <= 264)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 264 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 264 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 269-272
                notionalcaselistIndex = 0;
                for (int combi1 = 269; combi1 <= 272; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 273-276
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 273; combi1 <= 276; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 273; combi1 <= 276; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 277-284
                for (int combi1 = 277; combi1 <= 284; combi1++)
                {
                    if (combi1 <= 280)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 280 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 280 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 285-288
                notionalcaselistIndex = 0;
                for (int combi1 = 285; combi1 <= 288; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case12 ------------------------- COMBI 289-320

                dlFactor = 1;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 32 load combis DL TEMP ------------------------- COMBI 289-320
                for (int combi1 = 289; combi1 <= 320; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 304)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 304 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 304 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 289-320
                for (int combi1 = 289; combi1 <= 320; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 289-292
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 289; combi1 <= 292; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 289; combi1 <= 292; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 293-300
                for (int combi1 = 293; combi1 <= 300; combi1++)
                {
                    if (combi1 <= 296)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 296 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 296 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 301-304
                notionalcaselistIndex = 0;
                for (int combi1 = 301; combi1 <= 304; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 305-308
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 305; combi1 <= 308; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 305; combi1 <= 308; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 309-316
                for (int combi1 = 309; combi1 <= 316; combi1++)
                {
                    if (combi1 <= 312)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 312 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 312 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 317-320
                notionalcaselistIndex = 0;
                for (int combi1 = 317; combi1 <= 320; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case13 ------------------------- COMBI 321-352

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 32 load combis DL SDL TEMP ------------------------- COMBI 321-352
                for (int combi1 = 321; combi1 <= 352; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 336)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 336 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 336 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 321-352
                for (int combi1 = 321; combi1 <= 352; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 321-324
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 321; combi1 <= 324; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 321; combi1 <= 324; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 325-332
                for (int combi1 = 325; combi1 <= 332; combi1++)
                {
                    if (combi1 <= 328)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 328 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 328 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 333-336
                notionalcaselistIndex = 0;
                for (int combi1 = 333; combi1 <= 336; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 337-340
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 337; combi1 <= 340; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 337; combi1 <= 340; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 341-348
                for (int combi1 = 341; combi1 <= 348; combi1++)
                {
                    if (combi1 <= 344)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 344 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 344 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 349-352
                notionalcaselistIndex = 0;
                for (int combi1 = 349; combi1 <= 352; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 32 load combis for case14 ------------------------- COMBI 353-384

                dlFactor = 1;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 32 load combis DL TEMP ------------------------- COMBI 353-384
                for (int combi1 = 353; combi1 <= 384; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 368)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 368 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 368 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 353-384
                for (int combi1 = 353; combi1 <= 384; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 353-356
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 353; combi1 <= 356; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 353; combi1 <= 356; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 357-364
                for (int combi1 = 357; combi1 <= 364; combi1++)
                {
                    if (combi1 <= 360)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 360 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 360 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 365-368
                notionalcaselistIndex = 0;
                for (int combi1 = 365; combi1 <= 368; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 369-372
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 369; combi1 <= 372; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 369; combi1 <= 372; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 373-380
                for (int combi1 = 373; combi1 <= 380; combi1++)
                {
                    if (combi1 <= 376)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 376 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 376 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 381-384
                notionalcaselistIndex = 0;
                for (int combi1 = 381; combi1 <= 384; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 8 load combis for case15 ------------------------- COMBI 385-392

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 385-392
                for (int combi1 = 385; combi1 <= 392; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 388)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 388 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 388 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 385-392
                for (int combi1 = 385; combi1 <= 392; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 393-400

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 393-400
                for (int combi1 = 393; combi1 <= 400; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 396)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 396 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 396 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 393-400
                for (int combi1 = 393; combi1 <= 400; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case17 ------------------------- COMBI 401-408

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 401-408
                for (int combi1 = 401; combi1 <= 408; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 404)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 404 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 404 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 401-408
                for (int combi1 = 401; combi1 <= 408; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case18 ------------------------- COMBI 409-416

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 409-416
                for (int combi1 = 409; combi1 <= 416; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 412)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 412 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 412 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 409-416
                for (int combi1 = 409; combi1 <= 416; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 417-420

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 417-420
                for (int combi1 = 417; combi1 <= 420; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 417-420
                for (int combi1 = 417; combi1 <= 420; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 421-424

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 421-424
                for (int combi1 = 421; combi1 <= 424; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 421-424
                for (int combi1 = 421; combi1 <= 424; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 16 load combis for case21 ------------------------- COMBI 425-440

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL ------------------------- COMBI 425-440
                for (int combi1 = 425; combi1 <= 440; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 425-440
                for (int combi1 = 425; combi1 <= 440; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 425-428
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 425; combi1 <= 428; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 425; combi1 <= 428; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 429-436
                for (int combi1 = 429; combi1 <= 436; combi1++)
                {
                    if (combi1 <= 432)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 432 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 432 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 437-440
                notionalcaselistIndex = 0;
                for (int combi1 = 437; combi1 <= 440; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case22 ------------------------- COMBI 441-456

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL ------------------------- COMBI 441-456
                for (int combi1 = 441; combi1 <= 456; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 441-456
                for (int combi1 = 441; combi1 <= 456; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 441-444
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 441; combi1 <= 444; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 441; combi1 <= 444; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 445-452
                for (int combi1 = 445; combi1 <= 452; combi1++)
                {
                    if (combi1 <= 448)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 448 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 448 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 453-456
                notionalcaselistIndex = 0;
                for (int combi1 = 453; combi1 <= 456; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 8 load combis for case23 ------------------------- COMBI 457-464

                dlFactor = 1.35;
                sdlFactor = 1.35;
                tlFactor = 1.5;

                //first 8 load combis DL SDL TEMP ------------------------- COMBI 457-464
                for (int combi1 = 457; combi1 <= 464; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 460)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 460 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 460 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 457-464
                for (int combi1 = 457; combi1 <= 464; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 8 load combis for case24 ------------------------- COMBI 465-472

                dlFactor = 1;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 465-472
                for (int combi1 = 465; combi1 <= 472; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 468)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 468 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 468 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 465-472
                for (int combi1 = 465; combi1 <= 472; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 473-476

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 473-476
                for (int combi1 = 473; combi1 <= 476; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 473-476
                for (int combi1 = 473; combi1 <= 476; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 477-480

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 477-480
                for (int combi1 = 477; combi1 <= 480; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 477-480
                for (int combi1 = 477; combi1 <= 480; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region No wind vertical, have wind horizontal, have temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 2 * 6) + (8 * 4) + (8 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (8 * 2) + (2 * 2 * 4) + (1 * 2 * 4);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 16 load combis for case1 ------------------------- COMBI 1-16

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 8)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 1; combi1 <= 4; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 5-8
                notionalcaselistIndex = 0;
                for (int combi1 = 5; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 9-12
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 9; combi1 <= 12; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 9; combi1 <= 12; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 13-16
                notionalcaselistIndex = 0;
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case2 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 24)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 17-20
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 21-24
                notionalcaselistIndex = 0;
                for (int combi1 = 21; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 25-28
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 25; combi1 <= 28; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 25; combi1 <= 28; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 29-32
                notionalcaselistIndex = 0;
                for (int combi1 = 29; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case3 ------------------------- COMBI 33-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 40)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 32 ehf loads ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 37-40
                notionalcaselistIndex = 0;
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 41-44
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 41; combi1 <= 44; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 41; combi1 <= 44; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 45-48
                notionalcaselistIndex = 0;
                for (int combi1 = 45; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case4 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 56)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 49-52
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 49; combi1 <= 52; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 53-56
                notionalcaselistIndex = 0;
                for (int combi1 = 53; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 57-60
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 57; combi1 <= 60; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 57; combi1 <= 60; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 61-64
                notionalcaselistIndex = 0;
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case5 ------------------------- COMBI 65-80

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 65-80
                for (int combi1 = 65; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 72)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 65-80
                for (int combi1 = 65; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 65; combi1 <= 68; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 69-72
                notionalcaselistIndex = 0;
                for (int combi1 = 69; combi1 <= 72; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 73-76
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 73; combi1 <= 76; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 73; combi1 <= 76; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 77-80
                notionalcaselistIndex = 0;
                for (int combi1 = 77; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case6 ------------------------- COMBI 81-96

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 81-96
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 88)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 81-96
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 81-84
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 81; combi1 <= 84; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 81; combi1 <= 84; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 85-88
                notionalcaselistIndex = 0;
                for (int combi1 = 85; combi1 <= 88; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 89-92
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 89; combi1 <= 92; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 89; combi1 <= 92; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 93-96
                notionalcaselistIndex = 0;
                for (int combi1 = 93; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 8 load combis for case7 ------------------------- COMBI 97-104

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 97; combi1 <= 100; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 101-104
                notionalcaselistIndex = 0;
                for (int combi1 = 101; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case8 ------------------------- COMBI 105-112

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 105-108
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 105; combi1 <= 108; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 105; combi1 <= 108; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 109-112
                notionalcaselistIndex = 0;
                for (int combi1 = 109; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case9 ------------------------- COMBI 113-120

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 113-120
                for (int combi1 = 113; combi1 <= 120; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 113-120
                for (int combi1 = 113; combi1 <= 120; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 113-116
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 113; combi1 <= 116; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 113; combi1 <= 116; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 117-120
                notionalcaselistIndex = 0;
                for (int combi1 = 117; combi1 <= 120; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 121-128

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 121-128
                for (int combi1 = 121; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 121-128
                for (int combi1 = 121; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 121-124
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 121; combi1 <= 124; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 121; combi1 <= 124; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 125-128
                notionalcaselistIndex = 0;
                for (int combi1 = 125; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 16 load combis for case11 ------------------------- COMBI 129-144

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL TEMP ------------------------- COMBI 129-144
                for (int combi1 = 129; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 136)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 129-144
                for (int combi1 = 129; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 129-132
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 129; combi1 <= 132; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 129; combi1 <= 132; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 133-136
                notionalcaselistIndex = 0;
                for (int combi1 = 133; combi1 <= 136; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 137-140
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 137; combi1 <= 140; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 137; combi1 <= 140; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 141-144
                notionalcaselistIndex = 0;
                for (int combi1 = 141; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case12 ------------------------- COMBI 145-160

                dlFactor = 1;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL TEMP ------------------------- COMBI 145-160
                for (int combi1 = 145; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 152)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 145-160
                for (int combi1 = 145; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 145-148
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 145; combi1 <= 148; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 145; combi1 <= 148; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 149-152
                notionalcaselistIndex = 0;
                for (int combi1 = 149; combi1 <= 152; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 153-156
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 153; combi1 <= 156; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 153; combi1 <= 156; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 157-160
                notionalcaselistIndex = 0;
                for (int combi1 = 157; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case13 ------------------------- COMBI 161-176

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL TEMP ------------------------- COMBI 161-176
                for (int combi1 = 161; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 168)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 161-176
                for (int combi1 = 161; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 161-164
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 161; combi1 <= 164; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 161; combi1 <= 164; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 165-168
                notionalcaselistIndex = 0;
                for (int combi1 = 165; combi1 <= 168; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 169-172
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 169; combi1 <= 172; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 169; combi1 <= 172; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 173-176
                notionalcaselistIndex = 0;
                for (int combi1 = 173; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case14 ------------------------- COMBI 177-192

                dlFactor = 1;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL TEMP ------------------------- COMBI 177-192
                for (int combi1 = 177; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 184)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 177-192
                for (int combi1 = 177; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 177-180
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 177; combi1 <= 180; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 177; combi1 <= 180; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 181-184
                notionalcaselistIndex = 0;
                for (int combi1 = 181; combi1 <= 184; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                //add first 4 wind load combis ------------------------- COMBI 185-188
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 185; combi1 <= 188; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 185; combi1 <= 188; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 189-192
                notionalcaselistIndex = 0;
                for (int combi1 = 189; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 8 load combis for case15 ------------------------- COMBI 193-200

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 193-200
                for (int combi1 = 193; combi1 <= 200; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 196)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 196 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 196 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 193-200
                for (int combi1 = 193; combi1 <= 200; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 201-208

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 201-208
                for (int combi1 = 201; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 204)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 204 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 204 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 201-208
                for (int combi1 = 201; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case17 ------------------------- COMBI 209-216

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 209-216
                for (int combi1 = 209; combi1 <= 216; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 212)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 212 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 212 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 209-216
                for (int combi1 = 209; combi1 <= 216; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case18 ------------------------- COMBI 217-224

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 217-224
                for (int combi1 = 217; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 220)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 220 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 220 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 217-224
                for (int combi1 = 217; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 225-228

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 225-228
                for (int combi1 = 225; combi1 <= 228; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 225-228
                for (int combi1 = 225; combi1 <= 228; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 229-232

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 229-232
                for (int combi1 = 229; combi1 <= 232; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 229-232
                for (int combi1 = 229; combi1 <= 232; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case21 ------------------------- COMBI 233-240

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL ------------------------- COMBI 233-240
                for (int combi1 = 233; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 233-240
                for (int combi1 = 233; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 233-236
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 233; combi1 <= 236; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 233; combi1 <= 236; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 237-240
                notionalcaselistIndex = 0;
                for (int combi1 = 237; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case22 ------------------------- COMBI 241-248

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL ------------------------- COMBI 241-248
                for (int combi1 = 241; combi1 <= 248; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 241-248
                for (int combi1 = 241; combi1 <= 248; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 241-244
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 241; combi1 <= 244; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 241; combi1 <= 244; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 245-248
                notionalcaselistIndex = 0;
                for (int combi1 = 245; combi1 <= 248; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 8 load combis for case23 ------------------------- COMBI 249-256

                dlFactor = 1.35;
                sdlFactor = 1.35;
                tlFactor = 1.5;

                //first 8 load combis DL SDL TEMP ------------------------- COMBI 249-256
                for (int combi1 = 249; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 252)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 252 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 252 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 249-256
                for (int combi1 = 249; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 8 load combis for case24 ------------------------- COMBI 257-264

                dlFactor = 1;
                tlFactor = 1.5;

                //first 8 load combis DL TEMP ------------------------- COMBI 257-264
                for (int combi1 = 257; combi1 <= 264; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 260)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 260 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 260 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 257-264
                for (int combi1 = 257; combi1 <= 264; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 265-268

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 265-268
                for (int combi1 = 265; combi1 <= 268; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 265-268
                for (int combi1 = 265; combi1 <= 268; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 269-272

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 269-272
                for (int combi1 = 269; combi1 <= 272; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 269-272
                for (int combi1 = 269; combi1 <= 272; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region No wind vertical, have wind horizontal, no temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 4) + (4 * 2) + (8 * 2) + (4 * 2);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 8 load combis for case7 ------------------------- COMBI 1-8

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 1; combi1 <= 4; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 5-8
                notionalcaselistIndex = 0;
                for (int combi1 = 5; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case8 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 9-12
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 9; combi1 <= 12; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 9; combi1 <= 12; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 13-16
                notionalcaselistIndex = 0;
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case9 ------------------------- COMBI 17-24

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 17-20
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 21-24
                notionalcaselistIndex = 0;
                for (int combi1 = 21; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 25-28
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 25; combi1 <= 28; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 25; combi1 <= 28; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 29-32
                notionalcaselistIndex = 0;
                for (int combi1 = 29; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 33-36

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case21 ------------------------- COMBI 41-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 41-44
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 41; combi1 <= 44; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 41; combi1 <= 44; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 45-48
                notionalcaselistIndex = 0;
                for (int combi1 = 45; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 8 load combis for case22 ------------------------- COMBI 49-56

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 49-52
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 49; combi1 <= 52; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 53-56
                notionalcaselistIndex = 0;
                for (int combi1 = 53; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 57-60

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 61-64

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region No wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (2 * 4 * 4) + (4 * 2) + (2 * 4 * 2) + (4 * 2);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 8 load combis for case15 ------------------------- COMBI 1-8

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 193-200
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 4)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 12)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case17 ------------------------- COMBI 17-24

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 20)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case18 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 28)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 33-36

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case23 ------------------------- COMBI 41-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                tlFactor = 1.5;

                //first 8 load combis DL SDL TEMP ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 44)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 44 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 44 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 8 load combis for case24 ------------------------- COMBI 49-56

                dlFactor = 1;
                tlFactor = 1.5;

                //first 8 load combis DL TEMP ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 52)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 57-60

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 61-64

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region No wind vertical, no wind horizontal, no temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (4 * 2) + (4 * 2);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 4 load combis for case19 ------------------------- COMBI 1-4

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 1-4
                for (int combi1 = 1; combi1 <= 4; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 1-4
                for (int combi1 = 1; combi1 <= 4; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 5-8

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 5-8
                for (int combi1 = 5; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 5-8
                for (int combi1 = 5; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 9-12

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 9-12
                for (int combi1 = 9; combi1 <= 12; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 9-12
                for (int combi1 = 9; combi1 <= 12; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 13-16

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 13-16
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 13-16
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region Have wind vertical, have wind horizontal, no temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 4) + (4 * 2) + (16 * 2) + (4 * 2);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 16 load combis for case7 ------------------------- COMBI 1-16

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 16 load combis DL SDL LL ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 1; combi1 <= 4; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 13-16
                notionalcaselistIndex = 0;
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case8 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 16 load combis DL LL ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 17-20
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 17; combi1 <= 20; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case9 ------------------------- COMBI 33-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL LL ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case10 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL LL ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 49-52
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 49; combi1 <= 52; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
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
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 65-68

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 65-68
                for (int combi1 = 65; combi1 <= 68; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 65-68
                for (int combi1 = 65; combi1 <= 68; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 69-72

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 69-72
                for (int combi1 = 69; combi1 <= 72; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 69-72
                for (int combi1 = 69; combi1 <= 72; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 16 load combis for case21 ------------------------- COMBI 73-88

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL ------------------------- COMBI 73-88
                for (int combi1 = 73; combi1 <= 88; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 73-88
                for (int combi1 = 73; combi1 <= 88; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 73-76
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 73; combi1 <= 76; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 73; combi1 <= 76; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 77-84
                for (int combi1 = 77; combi1 <= 84; combi1++)
                {
                    if (combi1 <= 80)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 80 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 85-88
                notionalcaselistIndex = 0;
                for (int combi1 = 85; combi1 <= 88; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion

                #region Next 16 load combis for case22 ------------------------- COMBI 89-104

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL ------------------------- COMBI 89-104
                for (int combi1 = 89; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 89-104
                for (int combi1 = 89; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 89-92
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 89; combi1 <= 92; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
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
                    for (int combi1 = 89; combi1 <= 92; combi1++)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        combi1++;
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windcaseList[windcaselistIndex], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                        windcaselistIndex++;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 93-100
                for (int combi1 = 93; combi1 <= 100; combi1++)
                {
                    if (combi1 <= 96)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 96 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 96 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 101-104
                notionalcaselistIndex = 0;
                for (int combi1 = 101; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, notional2caseList[notionalcaselistIndex], -nlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    notionalcaselistIndex++;
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 105-108

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 105-108
                for (int combi1 = 105; combi1 <= 108; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 105-108
                for (int combi1 = 105; combi1 <= 108; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 109-112

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 109-112
                for (int combi1 = 109; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 109-112
                for (int combi1 = 109; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region Have wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2 * 6) + (8 * 4) + (8 * 2 * 4) + (2 * 4 * 4) + (1 * 2 * 4) + (8 * 2) + (2 * 2 * 4) + (1 * 2 * 4);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 16 load combis for case1 ------------------------- COMBI 1-16

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 8)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 8 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    if (combi1 <= 4)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    if (combi1 <= 12)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case2 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 24)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 24 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    if (combi1 <= 20)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    if (combi1 <= 28)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case3 ------------------------- COMBI 33-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 40)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 40 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 33-40
                for (int combi1 = 33; combi1 <= 40; combi1++)
                {
                    if (combi1 <= 36)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 36 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 36 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    if (combi1 <= 44)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 44 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 44 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case4 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 56)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 56 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 49-64
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    if (combi1 <= 52)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 57-64
                for (int combi1 = 57; combi1 <= 64; combi1++)
                {
                    if (combi1 <= 60)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 60 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 60 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case5 ------------------------- COMBI 65-80

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL LL TEMP ------------------------- COMBI 65-80
                for (int combi1 = 65; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 72)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 72 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 65-80
                for (int combi1 = 65; combi1 <= 80; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 65-72
                for (int combi1 = 65; combi1 <= 72; combi1++)
                {
                    if (combi1 <= 68)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 68 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 68 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 73-80
                for (int combi1 = 73; combi1 <= 80; combi1++)
                {
                    if (combi1 <= 76)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 76 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 76 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case6 ------------------------- COMBI 81-96

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL LL TEMP ------------------------- COMBI 81-96
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 88)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 88 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 81-96
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 81-88
                for (int combi1 = 81; combi1 <= 88; combi1++)
                {
                    if (combi1 <= 84)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 84 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 84 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 89-96
                for (int combi1 = 89; combi1 <= 96; combi1++)
                {
                    if (combi1 <= 92)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 92 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 92 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case7 ------------------------- COMBI 97-104

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    if (combi1 <= 100)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 100 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 100 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case8 ------------------------- COMBI 105-112

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    if (combi1 <= 108)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 108 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 108 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case9 ------------------------- COMBI 113-120

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 113-120
                for (int combi1 = 113; combi1 <= 120; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 113-120
                for (int combi1 = 113; combi1 <= 120; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 113-120
                for (int combi1 = 113; combi1 <= 120; combi1++)
                {
                    if (combi1 <= 116)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 116 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 116 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 121-128

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 121-128
                for (int combi1 = 121; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 121-128
                for (int combi1 = 121; combi1 <= 128; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 121-128
                for (int combi1 = 121; combi1 <= 128; combi1++)
                {
                    if (combi1 <= 124)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 124 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 124 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 16 load combis for case11 ------------------------- COMBI 129-144

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL SDL TEMP ------------------------- COMBI 129-144
                for (int combi1 = 129; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 136)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 136 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 129-144
                for (int combi1 = 129; combi1 <= 144; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 129-136
                for (int combi1 = 129; combi1 <= 136; combi1++)
                {
                    if (combi1 <= 132)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 132 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 132 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 137-144
                for (int combi1 = 137; combi1 <= 144; combi1++)
                {
                    if (combi1 <= 140)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 140 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 140 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case12 ------------------------- COMBI 145-160

                dlFactor = 1;
                wlFactor = 1.5;
                tlFactor = 0.9;
                nlFactor = 1;

                //first 16 load combis DL TEMP ------------------------- COMBI 145-160
                for (int combi1 = 145; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 152)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 152 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 145-160
                for (int combi1 = 145; combi1 <= 160; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 145-152
                for (int combi1 = 145; combi1 <= 152; combi1++)
                {
                    if (combi1 <= 148)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 148 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 148 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 153-160
                for (int combi1 = 153; combi1 <= 160; combi1++)
                {
                    if (combi1 <= 156)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 312 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 312 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case13 ------------------------- COMBI 161-176

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL SDL TEMP ------------------------- COMBI 161-176
                for (int combi1 = 161; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 168)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 168 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 161-176
                for (int combi1 = 161; combi1 <= 176; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 161-168
                for (int combi1 = 161; combi1 <= 168; combi1++)
                {
                    if (combi1 <= 164)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 164 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 164 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 169-176
                for (int combi1 = 169; combi1 <= 176; combi1++)
                {
                    if (combi1 <= 172)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 172 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 172 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 16 load combis for case14 ------------------------- COMBI 177-192

                dlFactor = 1;
                wlFactor = 0.75;
                tlFactor = 1.5;
                nlFactor = 1;

                //first 16 load combis DL TEMP ------------------------- COMBI 177-192
                for (int combi1 = 177; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 184)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 184 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 177-192
                for (int combi1 = 177; combi1 <= 192; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 177-184
                for (int combi1 = 177; combi1 <= 184; combi1++)
                {
                    if (combi1 <= 180)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 180 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 180 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 185-192
                for (int combi1 = 185; combi1 <= 192; combi1++)
                {
                    if (combi1 <= 188)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 188 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 188 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case15 ------------------------- COMBI 193-200

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 193-200
                for (int combi1 = 193; combi1 <= 200; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 196)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 196 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 196 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 193-200
                for (int combi1 = 193; combi1 <= 200; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 201-208

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 0.9;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 201-208
                for (int combi1 = 201; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 204)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 204 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 204 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 201-208
                for (int combi1 = 201; combi1 <= 208; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case17 ------------------------- COMBI 209-216

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL SDL LL TEMP ------------------------- COMBI 209-216
                for (int combi1 = 209; combi1 <= 216; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 212)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 212 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 212 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 209-216
                for (int combi1 = 209; combi1 <= 216; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case18 ------------------------- COMBI 217-224

                dlFactor = 1;
                llFactor = 1.5;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 217-224
                for (int combi1 = 217; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 220)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 220 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 220 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 217-224
                for (int combi1 = 217; combi1 <= 224; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 225-228

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 225-228
                for (int combi1 = 225; combi1 <= 228; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 225-228
                for (int combi1 = 225; combi1 <= 228; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 229-232

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 229-232
                for (int combi1 = 229; combi1 <= 232; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 229-232
                for (int combi1 = 229; combi1 <= 232; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case21 ------------------------- COMBI 233-240

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL ------------------------- COMBI 233-240
                for (int combi1 = 233; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 233-240
                for (int combi1 = 233; combi1 <= 240; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 233-240
                for (int combi1 = 233; combi1 <= 240; combi1++)
                {
                    if (combi1 <= 236)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 236 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 236 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case22 ------------------------- COMBI 241-248

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL ------------------------- COMBI 241-248
                for (int combi1 = 241; combi1 <= 248; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 241-248
                for (int combi1 = 241; combi1 <= 248; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 241-248
                for (int combi1 = 241; combi1 <= 248; combi1++)
                {
                    if (combi1 <= 244)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 244 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 244 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case23 ------------------------- COMBI 249-256

                dlFactor = 1.35;
                sdlFactor = 1.35;
                tlFactor = 1.5;

                //first 8 load combis DL SDL TEMP ------------------------- COMBI 249-256
                for (int combi1 = 249; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 252)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 252 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 252 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 249-256
                for (int combi1 = 249; combi1 <= 256; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 8 load combis for case24 ------------------------- COMBI 257-264

                dlFactor = 1;
                tlFactor = 1.5;

                //first 8 load combis DL LL TEMP ------------------------- COMBI 257-264
                for (int combi1 = 257; combi1 <= 264; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    if (combi1 <= 260)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 260 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 260 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 257-264
                for (int combi1 = 257; combi1 <= 264; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 265-268

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 265-268
                for (int combi1 = 265; combi1 <= 268; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 265-268
                for (int combi1 = 265; combi1 <= 268; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 269-272

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 269-272
                for (int combi1 = 269; combi1 <= 272; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 269-272
                for (int combi1 = 269; combi1 <= 272; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion



            #region Have wind vertical, no wind horizontal, no temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 4) + (4 * 2) + (8 * 2) + (4 * 2);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set ULS Design (ULS B), there could be existing combinations in the model with the same naming.");
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

                #region First 8 load combis for case7 ------------------------- COMBI 1-8

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
                    if (combi1 <= 4)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 4 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case8 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 0.75;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    if (combi1 <= 12)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 12 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case9 ------------------------- COMBI 17-24

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL LL ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    if (combi1 <= 20)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 20 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1.5;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL LL ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 25-32
                for (int combi1 = 25; combi1 <= 32; combi1++)
                {
                    if (combi1 <= 28)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 28 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case19 ------------------------- COMBI 33-36

                dlFactor = 1.35;
                sdlFactor = 1.35;
                llFactor = 1.5;

                //first 4 load combis DL SDL LL ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1.5;

                //first 4 load combis DL LL ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 8 load combis for case21 ------------------------- COMBI 41-48

                dlFactor = 1.35;
                sdlFactor = 1.35;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL SDL ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    if (combi1 <= 44)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 236 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 236 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case22 ------------------------- COMBI 49-56

                dlFactor = 1;
                wlFactor = 1.5;
                nlFactor = 1;

                //first 8 load combis DL ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add windz load combis x 8 ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    if (combi1 <= 52)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 52 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion






                #region Next 4 load combis for case25 ------------------------- COMBI 57-60

                dlFactor = 1.35;
                sdlFactor = 1.35;

                //first 4 load combis DL SDL ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_SDL", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 57-60
                for (int combi1 = 57; combi1 <= 60; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (SDL)", -sdlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

                #region Next 4 load combis for case26 ------------------------- COMBI 61-64

                dlFactor = 1;

                //first 4 load combis DL ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 61-64
                for (int combi1 = 61; combi1 <= 64; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                #endregion

            }

            #endregion


            #endregion 


            return false;
        }


    }
}
