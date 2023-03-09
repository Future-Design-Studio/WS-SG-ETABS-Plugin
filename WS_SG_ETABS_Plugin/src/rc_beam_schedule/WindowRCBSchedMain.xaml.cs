using ETABSv1;
using System;
using System.Collections.Generic;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace WS_SG_ETABS_Plugin.src.rc_beam_schedule
{
    /// <summary>
    /// Interaction logic for WindowRCBSchedMain.xaml
    /// </summary>
    public partial class WindowRCBSchedMain : Window
    {
        cSapModel _SapModel;
        cPluginCallback _Plugin;
        string modelPath;

        public WindowRCBSchedMain(cSapModel SapModel, cPluginCallback Plugin)
        {
            _SapModel = SapModel;
            _Plugin = Plugin;

            InitializeComponent();

            //Fill in default excel result name
            fileName.Text = "rc_beam_schedule";

            //Fill in model path (default path)
            modelPath = SapModel.GetModelFilepath();
            filePath.Text = modelPath;

            //Default check the rebar preference
            radioButtonSpcAuto.IsChecked = true;

            ShowDialog();
        }

        private void buttonDefaultPath_Click(object sender, RoutedEventArgs e)
        {
            filePath.Text = modelPath;
        }

        private void buttonModifyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = filePath.Text;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = dialog.SelectedPath;
            }

        }



        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = fileName.Text;
                string path = filePath.Text;

                double leftLength = Convert.ToDouble(textBoxLengthLeft.Text);
                double midLength = Convert.ToDouble(textBoxLengthMid.Text);
                double rightLength = Convert.ToDouble(textBoxLengthRight.Text);

                double cover = Convert.ToDouble(textBoxCover.Text);
                double maxShearLinksSpacing = Convert.ToDouble(textBoxMaxShearLinksSpacing.Text);
                double minShearLinksSpacing = Convert.ToDouble(textBoxMinShearLinksSpacing.Text);
                double shearLinksSpacingStep = Convert.ToDouble(textBoxShearLinksSpacingStep.Text);

                string[] rebarDiameterListInString = textBoxRebarDiameterList.Text.Split(',');
                int[] rebarDiameterList = Array.ConvertAll(rebarDiameterListInString, int.Parse);

                string[] shearLinksDiameterListInString = textBoxShearLinksDiameterList.Text.Split(',');
                int[] shearLinksDiameterList = Array.ConvertAll(shearLinksDiameterListInString, int.Parse);

                //Getting rebar spc preference
                double rebarSpc = 0;
                string rebarSpcPreference = "";
                if (radioButtonSpcClearSpc.IsChecked == true)
                {
                    rebarSpc = Convert.ToDouble(textBoxSpcClearSpc.Text);
                    rebarSpcPreference = "clear";
                }
                else if (radioButtonSpcCToC.IsChecked == true)
                {
                    rebarSpc = Convert.ToDouble(textBoxSpcCToC.Text);
                    rebarSpcPreference = "ctoc";
                }
                else if (radioButtonSpcAuto.IsChecked == true)
                {
                    rebarSpc = 0;
                    rebarSpcPreference = "auto";
                }

                //Check if total = 1, else will show warning
                double totalLength = leftLength + midLength + rightLength;

                if (totalLength != 1)
                {
                    System.Diagnostics.Debug.WriteLine("Total division of length must be 1!, currently " + totalLength);
                    MessageBox.Show("Total division of length must be 1!, currently " + totalLength);
                }
                else
                {
                    IsEnabled = false;
                    //Call up carbon result window
                    src.rc_beam_schedule.WindowRCBSchedResult windowRCBSchedResult = new src.rc_beam_schedule.WindowRCBSchedResult(_SapModel, _Plugin, name, path, leftLength,
                                                                                     midLength, rightLength, rebarSpc, rebarSpcPreference, maxShearLinksSpacing, minShearLinksSpacing, shearLinksSpacingStep, cover,
                                                                                     rebarDiameterList, shearLinksDiameterList);
                    IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void radioButtonSpcClearSpc_Checked(object sender, RoutedEventArgs e)
        {
            textBoxSpcClearSpc.IsEnabled = true;
            textBoxSpcCToC.IsEnabled = false;
        }

        private void radioButtonSpcCToC_Checked(object sender, RoutedEventArgs e)
        {
            textBoxSpcCToC.IsEnabled = true;
            textBoxSpcClearSpc.IsEnabled = false;
        }

    }
}
