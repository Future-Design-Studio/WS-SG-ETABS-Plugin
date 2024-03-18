using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperUI
    {
        /// <summary>
        /// Populate listBox with available groups
        /// </summary>
        /// <param name="SapModel"></param>
        /// <param name="listBox"></param>
        public static void PopulateListBoxAvailGroup(cSapModel SapModel, ListBox listBox)
        {
            string[] groupList = src.HelperEtabs.GetGroupList(SapModel);
            foreach (string group in groupList)
            {
                if (group != "All")
                {
                    listBox.Items.Add(group);
                }
            }
        }

        /// <summary>
        /// Populate comboBox with available groups
        /// </summary>
        /// <param name="SapModel"></param>
        /// <param name="listBox"></param>
        public static void PopulateComboBoxAvailGroup(cSapModel SapModel, ComboBox comboBox)
        {
            string[] groupList = src.HelperEtabs.GetGroupList(SapModel);
            List<string> groupListConverted = groupList.ToList();

            List<int> indexToRemove = new List<int>();
            foreach (string group in groupListConverted)
            {
                if (group == "All")
                {
                    indexToRemove.Add(groupListConverted.IndexOf(group));
                }
            }

            foreach (int index in indexToRemove)
            {
                groupListConverted.RemoveAt(index);
            }

            groupListConverted.Sort();

            comboBox.ItemsSource = groupListConverted;
            comboBox.SelectedIndex = 0;

        }

        /// <summary>
        /// Move items from listboxFrom to listboxTo
        /// </summary>
        /// <param name="listBoxFrom"></param>
        /// <param name="listBoxTo"></param>
        public static void MoveItemsFromListBox(ListBox listBoxFrom, ListBox listBoxTo)
        {
            List<Object> selectedItems = new List<Object>();
            foreach (var item in listBoxFrom.SelectedItems)
            {
                selectedItems.Add(item);
            }

            foreach (var item in selectedItems)
            {
                listBoxFrom.Items.RemoveAt(listBoxFrom.Items.IndexOf(item));
                listBoxTo.Items.Add(item);
            }
        }

        public static bool CheckGroupListExist(cSapModel Sap, List<string> listChoosenGroup)
        {
            if (listChoosenGroup.Count == 0)
            {
                MessageBox.Show("Please choose atleast 1 group");
                return false;
            }
            return true;
        }

        public static bool CheckLoadCaseListExist(cSapModel Sap, List<string> listChoosenLoadCase)
        {
            if (listChoosenLoadCase.Count == 0)
            {
                MessageBox.Show("Please choose atleast 1 load case");
                return false;
            }
            return true;
        }

        /*        public static void PopulateComboBox(ComboBox comboBox, List<T> data, string DisplayMemberPath, )
                {
                    comboBox.ItemsSource = data;
                    comboBox.DisplayMemberPath = DisplayMemberPath;
                    comBoxColumnConcreteGrade.SelectedValuePath

                    string[] groupList = src.HelperEtabs.GetGroupList(SapModel);
                    foreach (string group in groupList)
                    {
                        listBox.Items.Add(group);
                    }

                    //Fill in combobox for column
                    comBoxColumnConcreteGrade.ItemsSource = src.carbon.FunctionCarbon.concreteEmissionDataList;
                    comBoxColumnConcreteGrade.DisplayMemberPath = "ConcreteGrade";
                    comBoxColumnConcreteGrade.SelectedValuePath = "Emission";
                    comBoxColumnConcreteGrade.SelectedIndex = 0;
                }*/


        /// <summary>
        /// Populate listBox with available load cases
        /// </summary>
        /// <param name="SapModel"></param>
        /// <param name="listBox"></param>
        public static void PopulateListBoxAvailLoadCase(cSapModel SapModel, ListBox listBox)
        {
            List<string> loadCaseList = src.HelperEtabs.GetLoadCaseAndComboList(SapModel);

            loadCaseList.Sort();

            foreach (string loadCase in loadCaseList)
            {
                listBox.Items.Add(loadCase);
            }
        }


        /// <summary>
        /// Populate listBox with available frame section properties
        /// </summary>
        /// <param name="SapModel"></param>
        /// <param name="listBox"></param>
        public static void PopulateComboBoxAvailFrameSectProp(cSapModel SapModel, ComboBox comboBox)
        {
            int NumberNames = 0;
            string[] frameSectPropList = null;

            int ret = SapModel.PropFrame.GetNameList(ref NumberNames, ref frameSectPropList);

            foreach (string frameSectProp in frameSectPropList)
            {
                comboBox.Items.Add(frameSectProp);
            }

            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Populate listBox with available area section properties
        /// </summary>
        /// <param name="SapModel"></param>
        /// <param name="listBox"></param>
        public static void PopulateComboBoxAvailAreaSectProp(cSapModel SapModel, ComboBox comboBox)
        {
            int NumberNames = 0;
            string[] areaSectPropList = null;

            int ret = SapModel.PropArea.GetNameList(ref NumberNames, ref areaSectPropList);

            foreach (string areaSectProp in areaSectPropList)
            {
                comboBox.Items.Add(areaSectProp);
            }

            comboBox.SelectedIndex = 0;
        }

    }
}
