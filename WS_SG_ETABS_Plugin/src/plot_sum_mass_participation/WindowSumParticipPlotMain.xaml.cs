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
using System.Windows.Threading;
using ETABSv1;

namespace WS_SG_ETABS_Plugin.src.plot_sum_mass_participation
{
    /// <summary>
    /// Interaction logic for WindowSumParticipPlotMain.xaml
    /// </summary>
    public partial class WindowSumParticipPlotMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        public WindowSumParticipPlotMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            textBoxFileName.Text = "sum_mass_participation_factor_plot";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            textBoxFilePath.Text = modelPath;

            ShowDialog();
        }

        #region Button Method

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            textBoxFilePath.Text = modelPath;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                /*
                //Casting listbox content to list
                List<string> listChoosenLoadCase = listBoxChoosenLoadCase.Items.OfType<string>().ToList();
                */

                string fileName = textBoxFileName.Text;
                string filePath = textBoxFilePath.Text;

                int legendTextSize = Convert.ToInt32(textBoxLegendTextSize.Text);
                int titleSize = Convert.ToInt32(textBoxTitleSize.Text);
                int axisTitleSize = Convert.ToInt32(textBoxAxisTitleSize.Text);
                int tickLabelSize = Convert.ToInt32(textBoxTickLabelSize.Text);

                bool showUz= new bool();
                if (checkBoxUz.IsChecked == true)
                {
                    showUz = true;
                }
                else
                {
                    showUz = false;
                }

                //Call up copy result window
                plot_sum_mass_participation.WindowSumParticipPlotResult windowSumParticipPlotResult = new plot_sum_mass_participation.WindowSumParticipPlotResult(
                    _SapModel, _Plugin, legendTextSize, titleSize, axisTitleSize, tickLabelSize, showUz, fileName, filePath);
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }


            IsEnabled = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = textBoxFilePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFilePath.Text = dialog.SelectedPath;
            }
        }

        #endregion

    }
}
