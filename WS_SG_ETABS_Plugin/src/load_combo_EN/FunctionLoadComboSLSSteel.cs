using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    internal class FunctionLoadComboSLSSteel
    {
        public static bool AddLoadCombinationSLSSteel(cSapModel SapModel, cPluginCallback Plugin,
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


            #region Load Combination SLS

            //from here start creating load combinations SLS
            numberloadcombi = 1;
            basecombiname = "SL_Comb_C-";
            dlFactor = new double();
            sdlFactor = new double();
            llFactor = new double();
            wlFactor = new double();
            tlFactor = new double();
            nlFactor = new double();
            notionalcaselistIndex = new int();

            #region Have wind vertical, have wind horizontal, have temperature

            if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 2 * 3) + (16 * 2) + (20 * 2) + (2 * 4 * 2) + (1 * 2 * 2) + (10 * 1) + (2 * 1);

                //add envelope combi for all C
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region First 32 load combis for case2 ------------------------- COMBI 1-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
                nlFactor = 1;


                //first 32 load combis DL LL TEMP ------------------------- COMBI 1-32
                for (int combi1 = 1; combi1 <= 32; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 32 load combis for case4 ------------------------- COMBI 33-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;


                //first 32 load combis DL LL TEMP ------------------------- COMBI 33-64
                for (int combi1 = 33; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 32 load combis for case6 ------------------------- COMBI 65-96

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;


                //first 32 load combis DL LL TEMP ------------------------- COMBI 65-96
                for (int combi1 = 65; combi1 <= 96; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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
                
                #region Next 16 load combis for case8 ------------------------- COMBI 97-112

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;


                //first 16 load combis DL LL ------------------------- COMBI 97-112
                for (int combi1 = 97; combi1 <= 112; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 97-112
                for (int combi1 = 97; combi1 <= 112; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #endregion

                #region Next 16 load combis for case10 ------------------------- COMBI 113-128

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;


                //first 16 load combis DL LL ------------------------- COMBI 113-128
                for (int combi1 = 113; combi1 <= 128; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 113-128
                for (int combi1 = 113; combi1 <= 128; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 20 load combis for case12 ------------------------- COMBI 129-148

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;


                //first 20 load combis DL TEMP ------------------------- COMBI 129-148
                for (int combi1 = 129; combi1 <= 148; combi1++)
                {
                    if (combi1 <= 138)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 138 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 138 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
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

                //add windz load combis x 2 ------------------------- COMBI 133-134
                for (int combi1 = 133; combi1 <= 134; combi1++)
                {
                    if (combi1 <= 133)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 133 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 133 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 135-138
                notionalcaselistIndex = 0;
                for (int combi1 = 135; combi1 <= 138; combi1++)
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

                //add first 4 wind load combis ------------------------- COMBI 139-142
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 139; combi1 <= 142; combi1++)
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
                    for (int combi1 = 139; combi1 <= 142; combi1++)
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

                //add windz load combis x 2 ------------------------- COMBI 143-144
                for (int combi1 = 143; combi1 <= 144; combi1++)
                {
                    if (combi1 <= 143)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 143 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 143 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 145-148
                notionalcaselistIndex = 0;
                for (int combi1 = 145; combi1 <= 148; combi1++)
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

                #region Next 20 load combis for case14 ------------------------- COMBI 149-168

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;


                //first 20 load combis DL TEMP ------------------------- COMBI 149-168
                for (int combi1 = 149; combi1 <= 168; combi1++)
                {
                    if (combi1 <= 158)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 158 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 158 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add first 4 wind load combis ------------------------- COMBI 149-152
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 149; combi1 <= 152; combi1++)
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
                    for (int combi1 = 149; combi1 <= 152; combi1++)
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

                //add windz load combis x 2 ------------------------- COMBI 153-154
                for (int combi1 = 153; combi1 <= 154; combi1++)
                {
                    if (combi1 <= 153)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 153 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 153 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 155-158
                notionalcaselistIndex = 0;
                for (int combi1 = 155; combi1 <= 158; combi1++)
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

                //add first 4 wind load combis ------------------------- COMBI 159-162
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 159; combi1 <= 162; combi1++)
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
                    for (int combi1 = 159; combi1 <= 162; combi1++)
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

                //add windz load combis x 2 ------------------------- COMBI 163-164
                for (int combi1 = 163; combi1 <= 164; combi1++)
                {
                    if (combi1 <= 163)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 163 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 163 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
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

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 169-176

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;


                //first 8 load combis DL LL TEMP ------------------------- COMBI 169-176
                for (int combi1 = 169; combi1 <= 176; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 172)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 172 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 172 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 169-176
                for (int combi1 = 169; combi1 <= 176; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case18 ------------------------- COMBI 177-184

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;


                //first 8 load combis DL LL TEMP ------------------------- COMBI 177-184
                for (int combi1 = 177; combi1 <= 184; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 180)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 180 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 180 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 177-184
                for (int combi1 = 177; combi1 <= 184; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 4 load combis for case20 ------------------------- COMBI 185-188

                dlFactor = 1;
                llFactor = 1;


                //first 4 load combis DL LL ------------------------- COMBI 185-188
                for (int combi1 = 185; combi1 <= 188; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 185-188
                for (int combi1 = 185; combi1 <= 188; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 10 load combis for case22 ------------------------- COMBI 189-198

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;


                //add first 4 wind load combis ------------------------- COMBI 189-192
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 189; combi1 <= 192; combi1++)
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
                    for (int combi1 = 189; combi1 <= 192; combi1++)
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

                //add windz load combis x 2 ------------------------- COMBI 193-194
                for (int combi1 = 193; combi1 <= 194; combi1++)
                {
                    if (combi1 <= 193)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 193 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 193 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 195-198
                notionalcaselistIndex = 0;
                for (int combi1 = 195; combi1 <= 198; combi1++)
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

                #region Next 2 load combis for case24 ------------------------- COMBI 199-200

                dlFactor = 1;
                tlFactor = 1;


                //first 2 load combis DL LL TEMP ------------------------- COMBI 199-200
                for (int combi1 = 199; combi1 <= 200; combi1++)
                {
                    if (combi1 <= 199)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 199 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 199 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion
            }

            #endregion



            #region No wind vertical, have wind horizontal, have temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = 126;

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 16 load combis for case2 ------------------------- COMBI 1-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 16 load combis for case4 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 16 load combis for case6 ------------------------- COMBI 33-48

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case8 ------------------------- COMBI 49-56

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 57-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 57-64
                for (int combi1 = 57; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 57-64
                for (int combi1 = 57; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
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

                #region Next 16 load combis for case12 ------------------------- COMBI 65-80

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 16 load combis DL TEMP ------------------------- COMBI 65-80
                for (int combi1 = 65; combi1 <= 80; combi1++)
                {
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

                #region Next 16 load combis for case14 ------------------------- COMBI 81-96

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;



                //first 16 load combis DL TEMP ------------------------- COMBI 81-96
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
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

                #region Next 8 load combis for case16 ------------------------- COMBI 97-104

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 100)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 100 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 100 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 97-104
                for (int combi1 = 97; combi1 <= 104; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case18 ------------------------- COMBI 105-112

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 108)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 108 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 108 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 105-112
                for (int combi1 = 105; combi1 <= 112; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 4 load combis for case20 ------------------------- COMBI 113-116

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 113-116
                for (int combi1 = 113; combi1 <= 116; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 113-116
                for (int combi1 = 113; combi1 <= 116; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case22 ------------------------- COMBI 117-124

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                

                //add first 4 wind load combis ------------------------- COMBI 117-120
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 117; combi1 <= 120; combi1++)
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
                    for (int combi1 = 117; combi1 <= 120; combi1++)
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

                //add next 4 notional loads ------------------------- COMBI 121-124
                notionalcaselistIndex = 0;
                for (int combi1 = 121; combi1 <= 124; combi1++)
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

                #region Next 2 load combis for case24 ------------------------- COMBI 125-126

                dlFactor = 1;
                tlFactor = 1;



                //first 8 load combis DL TEMP ------------------------- COMBI 125-126
                for (int combi1 = 125; combi1 <= 126; combi1++)
                {
                    if (combi1 <= 125)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 125 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 125 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion
            }

            #endregion



            #region No wind vertical, have wind horizontal, no temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 2) + (4 * 1) + (8 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 8 load combis for case8 ------------------------- COMBI 1-8

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case10 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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

                #region Next 4 load combis for case20 ------------------------- COMBI 17-20

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case22 ------------------------- COMBI 21-28

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                

                //add first 4 wind load combis ------------------------- COMBI 21-24
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 21; combi1 <= 24; combi1++)
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
                    for (int combi1 = 21; combi1 <= 24; combi1++)
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

                //add next 4 notional loads ------------------------- COMBI 25-28
                notionalcaselistIndex = 0;
                for (int combi1 = 25; combi1 <= 28; combi1++)
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
            }

            #endregion



            #region No wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (2 * 4 * 2) + (4 * 1) + (2 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 8 load combis for case16 ------------------------- COMBI 1-8

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case18 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 4 load combis for case20 ------------------------- COMBI 17-20

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 2 load combis for case24 ------------------------- COMBI 21-22

                dlFactor = 1;
                tlFactor = 1;



                //first 2 load combis DL TEMP ------------------------- COMBI 21-22
                for (int combi1 = 21; combi1 <= 22; combi1++)
                {
                    if (combi1 <= 21)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 21 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 21 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion
            }

            #endregion



            #region No wind vertical, no wind horizontal, no temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (4 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 4 load combis for case20 ------------------------- COMBI 1-4

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 1-4
                for (int combi1 = 1; combi1 <= 4; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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
            }

            #endregion



            #region Have wind vertical, have wind horizontal, no temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 2) + (4 * 1) + (10 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 16 load combis for case8 ------------------------- COMBI 1-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;



                //first 16 load combis DL LL ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 16 load combis for case10 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                //first 16 load combis DL LL ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 16 ehf loads ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 4 load combis for case20 ------------------------- COMBI 33-36

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 33-36
                for (int combi1 = 33; combi1 <= 36; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 10 load combis for case22 ------------------------- COMBI 37-46

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                

                //add first 4 wind load combis ------------------------- COMBI 37-40
                if (WindLoadList.Count == 4)
                {
                    int windcaselistIndex = 0;
                    for (int combi1 = 37; combi1 <= 40; combi1++)
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
                    for (int combi1 = 37; combi1 <= 40; combi1++)
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

                //add windz load combis x 2 ------------------------- COMBI 41-42
                for (int combi1 = 41; combi1 <= 42; combi1++)
                {
                    if (combi1 <= 41)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 41 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 41 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add next 4 notional loads ------------------------- COMBI 43-46
                notionalcaselistIndex = 0;
                for (int combi1 = 43; combi1 <= 46; combi1++)
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
            }

            #endregion



            #region Have wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2 * 3) + (8 * 2) + (4 * 2) + (2 * 4 * 2) + (1 * 2 * 2) + (2 * 1) + (2 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 16 load combis for case2 ------------------------- COMBI 1-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 1-16
                for (int combi1 = 1; combi1 <= 16; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 16 load combis for case4 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 17-32
                for (int combi1 = 17; combi1 <= 32; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
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
                for (int combi1 = 49; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 16 load combis for case6 ------------------------- COMBI 33-48

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;



                //first 16 load combis DL LL TEMP ------------------------- COMBI 33-48
                for (int combi1 = 33; combi1 <= 48; combi1++)
                {
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
                for (int combi1 = 81; combi1 <= 96; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case8 ------------------------- COMBI 49-56

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 49-56
                for (int combi1 = 49; combi1 <= 56; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -llFactor);
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

                #endregion

                #region Next 8 load combis for case10 ------------------------- COMBI 57-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 57-64
                for (int combi1 = 57; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 57-64
                for (int combi1 = 57; combi1 <= 64; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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

                #region Next 4 load combis for case12 ------------------------- COMBI 65-68

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
                nlFactor = 1;



                //first 4 load combis DL TEMP ------------------------- COMBI 65-68
                for (int combi1 = 65; combi1 <= 68; combi1++)
                {
                    if (combi1 <= 66)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 66 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 66 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 2 ------------------------- COMBI 65-66
                for (int combi1 = 65; combi1 <= 66; combi1++)
                {
                    if (combi1 <= 65)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 65 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 65 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 2 ------------------------- COMBI 67-68
                for (int combi1 = 67; combi1 <= 68; combi1++)
                {
                    if (combi1 <= 67)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 67 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 67 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 4 load combis for case14 ------------------------- COMBI 69-72

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
                nlFactor = 1;



                //first 4 load combis DL TEMP ------------------------- COMBI 69-72
                for (int combi1 = 69; combi1 <= 72; combi1++)
                {
                    if (combi1 <= 70)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 70 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 70 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 2 ------------------------- COMBI 69-70
                for (int combi1 = 69; combi1 <= 70; combi1++)
                {
                    if (combi1 <= 69)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 69 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 69 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add windz load combis x 2 ------------------------- COMBI 71-72
                for (int combi1 = 71; combi1 <= 72; combi1++)
                {
                    if (combi1 <= 71)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 71 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 71 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 8 load combis for case16 ------------------------- COMBI 73-80

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 73-80
                for (int combi1 = 73; combi1 <= 80; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 76)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 76 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 76 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 73-80
                for (int combi1 = 73; combi1 <= 80; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case18 ------------------------- COMBI 81-88

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



                //first 8 load combis DL LL TEMP ------------------------- COMBI 81-88
                for (int combi1 = 81; combi1 <= 88; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }

                    if (combi1 <= 84)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 84 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 84 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 81-88
                for (int combi1 = 81; combi1 <= 88; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 4 load combis for case20 ------------------------- COMBI 89-92

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 89-92
                for (int combi1 = 89; combi1 <= 92; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 89-92
                for (int combi1 = 89; combi1 <= 92; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 2 load combis for case22 ------------------------- COMBI 93-94

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                

                //add windz load combis x 2 ------------------------- COMBI 93-94
                for (int combi1 = 93; combi1 <= 94; combi1++)
                {
                    if (combi1 <= 93)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 93 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 93 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion

                #region Next 2 load combis for case24 ------------------------- COMBI 95-96

                dlFactor = 1;
                tlFactor = 1;



                //first 2 load combis DL LL TEMP ------------------------- COMBI 95-96
                for (int combi1 = 95; combi1 <= 96; combi1++)
                {
                    if (combi1 <= 95)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 95 && tempcaselist.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[1], tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 95 && tempcaselist.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, tempcaselist[0], -tlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                #endregion
            }

            #endregion



            #region Have wind vertical, no wind horizontal, no temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2) + (4 * 1) + (2 * 1);

                //add envelope combi for all B
                isError = src.HelperEtabs.AddNewLoadCombiEnvelope(SapModel, basecombiname + "ENV");
                if (isError)
                {
                    MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
                    return true;
                }

                //create number of combinations required
                while (numberloadcombi <= numberOfCombi)
                {
                    isError = src.HelperEtabs.AddNewLoadCombiLinear(SapModel, basecombiname + numberloadcombi);
                    if (isError)
                    {
                        MessageBox.Show("Error adding load combination set Service Design with Precamber, there could be existing combinations in the model with the same naming.");
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

                

                #region Next 8 load combis for case8 ------------------------- COMBI 1-8

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 1-8
                for (int combi1 = 1; combi1 <= 8; combi1++)
                {
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 8 load combis for case10 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                //first 8 load combis DL LL ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 8 ehf loads ------------------------- COMBI 9-16
                for (int combi1 = 9; combi1 <= 16; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (-llFactor * quasiPermanentDict[ehfxLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][0]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (-llFactor * quasiPermanentDict[ehfyLL.Value][0]));
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

                #region Next 4 load combis for case20 ------------------------- COMBI 17-20

                dlFactor = 1;
                llFactor = 1;



                //first 4 load combis DL LL ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 17-20
                for (int combi1 = 17; combi1 <= 20; combi1++)
                {
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    foreach (KeyValuePair<string, string> ehfyLL in ehfllycaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, llFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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

                #region Next 2 load combis for case22 ------------------------- COMBI 21-22

                dlFactor = 1;
                wlFactor = 1;
                nlFactor = 1;



                

                //add windz load combis x 2 ------------------------- COMBI 21-22
                for (int combi1 = 21; combi1 <= 22; combi1++)
                {
                    if (combi1 <= 21)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 21 && WindVertLoadList.Count == 2)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[1], wlFactor);
                        if (isError)
                        {
                            return true;
                        }
                    }
                    else if (combi1 > 21 && WindVertLoadList.Count == 1)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, windvertcaseList[0], -wlFactor);
                        if (isError)
                        {
                            return true;
                        }
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
