using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.apply_modifier
{
    internal class FunctionApplyModifier
    {
        public static bool ApplyModifier(cSapModel SapModel, cPluginCallback plugin, List<ClassApplyModifier.ModifierDefinition> modifierDefinitionList)
        {
            bool isError = false;
            int ret = 0;

            foreach(ClassApplyModifier.ModifierDefinition modifierDefinition in modifierDefinitionList)
            {

                //Check if have area in group
                List<string> frameUniqueNameList = HelperEtabs.GetGroupFrameElement(SapModel, modifierDefinition.GroupName);

                if (frameUniqueNameList.Count > 0)
                {
                    double[] valueFrame = { modifierDefinition.FrameModA, modifierDefinition.FrameModV2, modifierDefinition.FrameModV3,
                        modifierDefinition.FrameModT, modifierDefinition.FrameModM2, modifierDefinition.FrameModM3,
                        modifierDefinition.FrameModMass, modifierDefinition.FrameModWeight};

                    ret = SapModel.FrameObj.SetModifiers(modifierDefinition.GroupName, ref valueFrame, eItemType.Group);

                    if (ret != 0)
                    {
                        isError = true;
                        MessageBox.Show($"Error is applying frame modifier to {modifierDefinition.GroupName}");
                    }
                }




                //Check if have area in group
                List<string> areaUniqueNameList = HelperEtabs.GetGroupAreaElement(SapModel, modifierDefinition.GroupName);

                if (areaUniqueNameList.Count > 0)
                {
                    double[] valueArea = { modifierDefinition.AreaModF11, modifierDefinition.AreaModF22, modifierDefinition.AreaModF12,
                        modifierDefinition.AreaModM11, modifierDefinition.AreaModM22, modifierDefinition.AreaModM12,
                        modifierDefinition.AreaModV13, modifierDefinition.AreaModV23, modifierDefinition.AreaModMass,
                        modifierDefinition.AreaModWeight };

                    ret = SapModel.AreaObj.SetModifiers(modifierDefinition.GroupName, ref valueArea, eItemType.Group);

                    if (ret != 0)
                    {
                        isError = true;
                        MessageBox.Show($"Error is applying area modifier to {modifierDefinition.GroupName}");
                    }

                }

            }

            return isError;

        }
        
    }
}
