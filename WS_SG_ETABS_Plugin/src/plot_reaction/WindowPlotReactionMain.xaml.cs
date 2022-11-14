using ETABSv1;
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

namespace WS_SG_ETABS_Plugin.src.plot_reaction
{
    /// <summary>
    /// Interaction logic for WindowPlotReactionMain.xaml
    /// </summary>
    public partial class WindowPlotReactionMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;

        public WindowPlotReactionMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in available groups list box
            src.HelperUI.PopulateListBoxAvailGroup(_SapModel, listBoxAvailGroup);

            //Fill in combobox for output case
            List<string> outputCasesList = src.HelperEtabs.GetLoadCaseAndComboList(_SapModel);
            comBoxOutputCase.ItemsSource = outputCasesList;
            comBoxOutputCase.SelectedIndex = 0;

            //Fill in combobox for direction
            comBoxReactionDirection.ItemsSource = src.plot_reaction.FunctionPlotReaction.reactionDirectionList;
            comBoxReactionDirection.SelectedIndex = 0;

            ShowDialog();
        }

        #region Button Method

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxAvailGroup, listBoxChoosenGroup);
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            src.HelperUI.MoveItemsFromListBox(listBoxChoosenGroup, listBoxAvailGroup);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            //Casting listbox content to list
            List<string> listChoosenGroup = listBoxChoosenGroup.Items.OfType<string>().ToList();

            string outputCase = comBoxOutputCase.SelectedValue.ToString();
            string reactionDirection = comBoxReactionDirection.SelectedValue.ToString();
            double bubbleScaleFactor = Convert.ToDouble(textBoxBubbleScaleFactor.Text);
            double textScaleFactor = Convert.ToDouble(textBoxTextScaleFactor.Text);

            IsEnabled = false;
            //Call up copy property result window
            src.plot_reaction.WindowPlotReactionResult windowPlotReactionResult = new src.plot_reaction.WindowPlotReactionResult(_SapModel, _Plugin, listChoosenGroup, 
                outputCase, reactionDirection, bubbleScaleFactor, textScaleFactor);
            IsEnabled = true;

        }

        #endregion


    }
}
