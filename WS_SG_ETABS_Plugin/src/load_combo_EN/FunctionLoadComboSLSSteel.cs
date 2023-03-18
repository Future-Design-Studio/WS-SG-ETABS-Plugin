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
        public static bool AddLoadCombinationSLS(cSapModel SapModel, cPluginCallback Plugin,
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

            MessageBox.Show("Starting Load Combination Service Design with Precamber");

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
                int numberOfCombi = (16 * 2 * 3) + (16 * 2) + (16 * 2 * 2) + (2 * 4 * 2) + (1 * 2 * 2) + (16 * 1) + (2 * 2 * 2);

                //add envelope combi for all C
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

                

                #region First 32 load combis for case2 ------------------------- COMBI 33-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
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

                #region Next 32 load combis for case4 ------------------------- COMBI 97-128

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 32 load combis for case6 ------------------------- COMBI 161-192

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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
                
                #region Next 16 load combis for case8 ------------------------- COMBI 209-224

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 16 load combis for case10 ------------------------- COMBI 241-256

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 32 load combis for case12 ------------------------- COMBI 289-320

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 32 load combis for case14 ------------------------- COMBI 353-384

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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

                #region Next 8 load combis for case16 ------------------------- COMBI 393-400

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;


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

                #region Next 8 load combis for case18 ------------------------- COMBI 409-416

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;


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

                #region Next 4 load combis for case20 ------------------------- COMBI 421-424

                dlFactor = 1;
                llFactor = 1;


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

                #region Next 16 load combis for case22 ------------------------- COMBI 441-456

                dlFactor = 1;
                wlFactor = 1;
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

                #region Next 8 load combis for case24 ------------------------- COMBI 465-472

                dlFactor = 1;
                tlFactor = 1;


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
            }

            #endregion



            #region No wind vertical, have wind horizontal, have temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (8 * 2 * 3) + (8 * 2) + (8 * 2 * 1) + (2 * 4 * 2) + (1 * 2 * 2) + (8 * 1) + (2 * 2 * 2);

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

                

                #region Next 16 load combis for case2 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case4 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case6 ------------------------- COMBI 81-96

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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

                #region Next 8 load combis for case8 ------------------------- COMBI 105-112

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 8 load combis for case10 ------------------------- COMBI 121-128

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 16 load combis for case12 ------------------------- COMBI 145-160

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case14 ------------------------- COMBI 177-192

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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

                #region Next 8 load combis for case16 ------------------------- COMBI 201-208

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



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

                #region Next 8 load combis for case18 ------------------------- COMBI 217-224

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



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

                #region Next 4 load combis for case20 ------------------------- COMBI 229-232

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 8 load combis for case22 ------------------------- COMBI 241-248

                dlFactor = 1;
                wlFactor = 1;
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

                #region Next 8 load combis for case24 ------------------------- COMBI 257-264

                dlFactor = 1;
                tlFactor = 1;



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

                

                #region Next 8 load combis for case8 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 8 load combis for case10 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 8 load combis for case22 ------------------------- COMBI 49-56

                dlFactor = 1;
                wlFactor = 1;
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
            }

            #endregion



            #region No wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count == 0)
            {
                int numberOfCombi = (2 * 4 * 2) + (4 * 1) + (2 * 4 * 1);

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

                

                #region Next 8 load combis for case16 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



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

                #region Next 8 load combis for case18 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



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

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 8 load combis for case24 ------------------------- COMBI 49-56

                dlFactor = 1;
                tlFactor = 1;



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

                

                #region Next 4 load combis for case20 ------------------------- COMBI 5-8

                dlFactor = 1;
                llFactor = 1;



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
            }

            #endregion



            #region Have wind vertical, have wind horizontal, no temperature

            else if (WindLoadList.Count != 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (16 * 2) + (4 * 1) + (16 * 1);

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

                

                #region Next 16 load combis for case8 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 16 load combis for case10 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 4 load combis for case20 ------------------------- COMBI 69-72

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 16 load combis for case22 ------------------------- COMBI 89-104

                dlFactor = 1;
                wlFactor = 1;
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
            }

            #endregion



            #region Have wind vertical, no wind horizontal, have temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count != 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2 * 3) + (8 * 2) + (8 * 2 * 2) + (2 * 4 * 2) + (1 * 2 * 2) + (8 * 1) + (2 * 2 * 2);

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

                

                #region Next 16 load combis for case2 ------------------------- COMBI 17-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case4 ------------------------- COMBI 49-64

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case6 ------------------------- COMBI 81-96

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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

                #region Next 8 load combis for case8 ------------------------- COMBI 105-112

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 8 load combis for case10 ------------------------- COMBI 121-128

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 16 load combis for case12 ------------------------- COMBI 145-160

                dlFactor = 1;
                wlFactor = 1;
                tlFactor = 0.6;
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

                #region Next 16 load combis for case14 ------------------------- COMBI 177-192

                dlFactor = 1;
                wlFactor = 0.5;
                tlFactor = 1;
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

                #region Next 8 load combis for case16 ------------------------- COMBI 201-208

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 0.6;



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

                #region Next 8 load combis for case18 ------------------------- COMBI 217-224

                dlFactor = 1;
                llFactor = 1;
                tlFactor = 1;



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

                #region Next 4 load combis for case20 ------------------------- COMBI 229-232

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 8 load combis for case22 ------------------------- COMBI 241-248

                dlFactor = 1;
                wlFactor = 1;
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

                #region Next 8 load combis for case24 ------------------------- COMBI 257-264

                dlFactor = 1;
                tlFactor = 1;



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
            }

            #endregion



            #region Have wind vertical, no wind horizontal, no temperature

            else if (WindLoadList.Count == 0 && TemperatureLoadList.Count == 0 && WindVertLoadList.Count != 0)
            {
                int numberOfCombi = (8 * 2) + (4 * 1) + (8 * 1);

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

                

                #region Next 8 load combis for case8 ------------------------- COMBI 9-16

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 0.5;
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

                #region Next 8 load combis for case10 ------------------------- COMBI 25-32

                dlFactor = 1;
                llFactor = 1;
                wlFactor = 1;
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

                #region Next 4 load combis for case20 ------------------------- COMBI 37-40

                dlFactor = 1;
                llFactor = 1;



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

                #region Next 8 load combis for case22 ------------------------- COMBI 49-56

                dlFactor = 1;
                wlFactor = 1;
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
            }

            #endregion


            #endregion

            MessageBox.Show("Finished creating Load Combination Service Design with Precamber");

            return false;
        }


    }
}
