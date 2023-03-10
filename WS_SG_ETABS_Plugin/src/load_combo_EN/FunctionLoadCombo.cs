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
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalDLoadList,
            List<src.load_combo_EN.ClassLoadCombo.NotionalLoad> NotionalSDLoadList,
            List<src.load_combo_EN.ClassLoadCombo.TemperatureLoad> TemperatureLoadList,
            List<src.load_combo_EN.ClassLoadCombo.SeismicLoad> SeismicLoadList,
            List<src.load_combo_EN.ClassLoadCombo.AccidentalLoad> AccidentalLoadList,
            List<src.load_combo_EN.ClassLoadCombo.UpliftLoad> UpliftLoadList,
            List<src.load_combo_EN.ClassLoadCombo.EHFLoad> EHFLoadList)
        {
            bool isError = false;

            foreach (var deadLoad in deadLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, deadLoad.Name, 1, "Dead", deadLoad.IsExisting);
                MessageBox.Show("Finished DL");
            }
            if (isError)
            {
                return true;
            }

            foreach (var superdeadLoad in SDLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, superdeadLoad.Name, 0, "SuperDead", superdeadLoad.IsExisting);
                MessageBox.Show("Finished SDL");
            }
            if (isError)
            {
                return true;
            }

            foreach (var liveLoad in LiveLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, liveLoad.Name, 0, "Live", liveLoad.IsExisting);
                MessageBox.Show("Finished LL");
            }
            if (isError)
            {
                return true;
            }

            foreach (var windLoad in WindLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, windLoad.Name, 0, "Wind", windLoad.IsExisting);
                MessageBox.Show("Finished Wind Load");
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalDLoad in NotionalDLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalDLoad.Name, 0, "Notional", false);
                MessageBox.Show("Finished Notional DLoad");
            }
            if (isError)
            {
                return true;
            }

            foreach (var notionalSDLoad in NotionalSDLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, notionalSDLoad.Name, 0, "Notional", false);
                MessageBox.Show("Finished Notional SDLoad");
            }
            if (isError)
            {
                return true;
            }

            foreach (var temperatureLoad in TemperatureLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, temperatureLoad.Name, 0, "Temperature", temperatureLoad.IsExisting);
                MessageBox.Show("Finished Temperature Load");
            }
            if (isError)
            {
                return true;
            }

            foreach (var seismicLoad in SeismicLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, seismicLoad.Name, 0, "Seismic", seismicLoad.IsExisting);
                MessageBox.Show("Finished Seismic Load");
            }
            if (isError)
            {
                return true;
            }

            foreach (var accidentalLoad in AccidentalLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, accidentalLoad.Name, 0, "Accidental", accidentalLoad.IsExisting);
                MessageBox.Show("Finished Accidental Load");
            }
            if (isError)
            {
                return true;
            }

            foreach (var upliftLoad in UpliftLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, upliftLoad.Name, 0, "Uplift", upliftLoad.IsExisting);
                MessageBox.Show("Finished Uplift Load");
            }
            if (isError)
            {
                return true;
            }

            foreach (var ehfLoad in EHFLoadList)
            {
                isError = src.HelperEtabs.AddLoadPattern(SapModel, ehfLoad.Name, 0, "EHF", false);
                MessageBox.Show("Finished EHF Load");
            }
            if (isError)
            {
                return true;
            }


            return false;
        }
    }
}
