using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_SG_ETABS_Plugin.src.apply_modifier;
using ETABSv1;
using System.Windows.Forms;

namespace WS_SG_ETABS_Plugin.src.apply_section_property
{
    public class FunctionApplySectProp
    {
        public static bool ApplySectProp(cSapModel SapModel, cPluginCallback Plugin, List<ClassApplySectProp.ApplySectPropDefinition> applySectPropDefinitionList)
        {
            bool isError = false;
            int ret = 0;

            foreach (ClassApplySectProp.ApplySectPropDefinition applySectPropDefinition in applySectPropDefinitionList)
            {

                if (applySectPropDefinition.FrameSectProp == null)
                {
                    ret = SapModel.AreaObj.SetProperty(applySectPropDefinition.GroupName, applySectPropDefinition.AreaSectProp, eItemType.Group);

                    if (ret != 0)
                    {
                        isError = true;
                        MessageBox.Show($"Error is applying area section property to {applySectPropDefinition.GroupName}");
                    }
                }

                if (applySectPropDefinition.AreaSectProp == null)
                {
                    ret = SapModel.FrameObj.SetSection(applySectPropDefinition.GroupName, applySectPropDefinition.FrameSectProp, eItemType.Group);


                    if (ret != 0)
                    {
                        isError = true;
                        MessageBox.Show($"Error is applying frame section property to {applySectPropDefinition.GroupName}");
                    }
                }
            }

            return isError;
        }
    }
}
