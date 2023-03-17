using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.load_combo_EN
{
    internal class FunctionLoadComboSeismic
    {
        public static bool AddLoadCombinationSeismic(cSapModel SapModel, cPluginCallback Plugin,
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
            double seismicFactor = new double();
            double tlFactor = new double();
            double nlFactor = new double();
            int notionalcaselistIndex = new int();
            bool isError = false;
            bool addAnalysisCase = false;

            #region Load Combination Seismic

            //from here start creating load combinations Seismic
            numberloadcombi = 1;
            basecombiname = "Seismic-";
            dlFactor = new double();
            sdlFactor = new double();
            llFactor = new double();
            wlFactor = new double();
            seismicFactor = new double();
            tlFactor = new double();
            nlFactor = new double();
            notionalcaselistIndex = new int();

            if (SeismicLoadList.Count != 0)
            {

                MessageBox.Show("Starting Load Combination set for Seismic loads");

                int numberOfCombi = (1 * 12) + (1 * 12) + (1 * 12) + (1 * 12);

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

                #region First 12 load combis for case1 ------------------------- COMBI 1-12

                dlFactor = 1;
                sdlFactor = 1;
                llFactor = 1;
                seismicFactor = 1;



                //first 12 load combis DL SDL LL ------------------------- COMBI 1-12
                for (int combi1 = 1; combi1 <= 12; combi1++)
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //first 4 load combis Seismic ------------------------- COMBI 1-4
                for (int combi1 = 1; combi1 <= 4; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[0], seismicFactor);
                    if (isError)
                    {
                        return true;
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //next 8 load combis Seismic ------------------------- COMBI 5-12
                for (int combi1 = 5; combi1 <= 12; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                }

                //next 8 ehf loads ------------------------- COMBI 5-12
                for (int combi1 = 5; combi1 <= 12; combi1++)
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                #endregion

                #region Next 12 load combis for case2 ------------------------- COMBI 13-24

                dlFactor = 1;
                llFactor = 1;
                seismicFactor = 1;



                //first 12 load combis DL LL ------------------------- COMBI 13-24
                for (int combi1 = 13; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> llDetails in llcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, llDetails.Key, (llFactor * quasiPermanentDict[llDetails.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //first 4 load combis Seismic ------------------------- COMBI 13-16
                for (int combi1 = 13; combi1 <= 16; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[0], seismicFactor);
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
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }

                //next 8 load combis Seismic ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                }

                //next 8 ehf loads ------------------------- COMBI 17-24
                for (int combi1 = 17; combi1 <= 24; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    foreach (KeyValuePair<string, string> ehfxLL in ehfllxcaseDict)
                    {
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, (llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfxLL.Key, -(llFactor * quasiPermanentDict[ehfxLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, (llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
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
                        isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, ehfyLL.Key, -(llFactor * quasiPermanentDict[ehfyLL.Value][2]));
                        if (isError)
                        {
                            return true;
                        }
                    }
                }
                #endregion

                #region Next 12 load combis for case3 ------------------------- COMBI 25-36

                dlFactor = 1;
                sdlFactor = 1;
                seismicFactor = 1;



                //first 12 load combis DL SDL ------------------------- COMBI 25-36
                for (int combi1 = 25; combi1 <= 36; combi1++)
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

                //first 4 load combis Seismic ------------------------- COMBI 25-28
                for (int combi1 = 25; combi1 <= 28; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[0], seismicFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //add 4 ehf loads ------------------------- COMBI 25-28
                for (int combi1 = 25; combi1 <= 28; combi1++)
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

                //next 8 load combis Seismic ------------------------- COMBI 29-36
                for (int combi1 = 29; combi1 <= 36; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                }

                //next 8 ehf loads ------------------------- COMBI 29-36
                for (int combi1 = 29; combi1 <= 36; combi1++)
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

                #region Next 12 load combis for case4 ------------------------- COMBI 37-48

                dlFactor = 1;
                llFactor = 1;
                seismicFactor = 1;



                //first 12 load combis DL ------------------------- COMBI 37-48
                for (int combi1 = 37; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_DL", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }

                //first 4 load combis Seismic ------------------------- COMBI 37-40
                for (int combi1 = 37; combi1 <= 40; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[0], seismicFactor);
                    if (isError)
                    {
                        return true;
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

                //next 8 load combis Seismic ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], 1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], 0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[2], -1);
                    if (isError)
                    {
                        return true;
                    }
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, seismiccaselist[1], -0.3);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;
                }

                //next 8 ehf loads ------------------------- COMBI 41-48
                for (int combi1 = 41; combi1 <= 48; combi1++)
                {
                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFx (DL)", dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                    combi1++;

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
                    combi1++;

                    isError = src.HelperEtabs.UpdateLoadCombi(SapModel, basecombiname + combi1, 0, "P_EHFy (DL)", -dlFactor);
                    if (isError)
                    {
                        return true;
                    }
                }
                #endregion

                MessageBox.Show("Finished creating Load Combination set for Seismic loads");

            }
            else
            {
                MessageBox.Show("No Seismic Loads defined, skipping Seismic Load Combination Set");
            }

            #endregion

            return false;
        }
    }
}