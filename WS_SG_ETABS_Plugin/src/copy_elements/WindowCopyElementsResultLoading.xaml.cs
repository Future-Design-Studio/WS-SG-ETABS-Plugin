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
using ScottPlot.Drawing.Colormaps;
using WS_SG_ETABS_Plugin.src.wall_design_view;

namespace WS_SG_ETABS_Plugin.src.copy_elements
{
    /// <summary>
    /// Interaction logic for WindowCopyElementsResultLoading.xaml
    /// </summary>
    public partial class WindowCopyElementsResultLoading : Window
    {
        public  WindowCopyElementsResultLoading(cSapModel SapModel, string filePathFrom)
        {
            InitializeComponent();
            
            ProcessCopyElements(SapModel, filePathFrom);

            ShowDialog();
        }

        public async void ProcessCopyElements(cSapModel SapModel, string filePathFrom)
        {
            bool isError = false;
            Dictionary<string, HelperEtabs.DatabaseTable> databaseTableDict = new Dictionary<string, HelperEtabs.DatabaseTable> ();

            List<HelperEtabs.Shell> shellList = new List<HelperEtabs.Shell> ();
            List<HelperEtabs.Frame> frameList = new List<HelperEtabs.Frame>();

            List<string> groupNames = new List<string> ();

            await Task.Run(() => {

                (isError, databaseTableDict) =  FunctionCopyElements.ExtractDatabaseTable(SapModel, filePathFrom);

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxExtractTable.IsChecked = true; }));

                (isError, shellList, frameList) = FunctionCopyElements.ProcessElementsToCopy(databaseTableDict);

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxProcessConnectivity.IsChecked = true; }));

                (isError, groupNames) = FunctionCopyElements.GetGroupName(databaseTableDict, shellList, frameList);

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxProcessGroupName.IsChecked = true; }));

                Dictionary<string, string> translatedGroupNames = FunctionCopyElements.CreateGroups(SapModel, groupNames, shellList, frameList);

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxCreateGroups.IsChecked = true; }));

                FunctionCopyElements.CreateElements(SapModel, shellList, frameList, translatedGroupNames);

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxCreateElements.IsChecked = true; }));
            });

            Close();

            
            if (!isError)
            {
                //Call up window
                WindowCopyElementsResult windowCopyElementsResult = new WindowCopyElementsResult();
            }
            
        }
    }
}
