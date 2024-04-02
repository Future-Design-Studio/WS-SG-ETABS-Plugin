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

            List<string> shellFailToCreate = new List<string> ();
            List<string> frameFailToCreate = new List<string>();

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

                //FunctionCopyElements.CreateElements(SapModel, shellList, frameList, translatedGroupNames, progressBarCreateElements);

                //Create elements
                int ret = 0;

                double totalElements = shellList.Count() + frameList.Count();
                double elementsIterator = 0;

                foreach (HelperEtabs.Shell shell in shellList)
                {
                    List<string> pointUniqueNameForShellList = new List<string>();

                    /*
                    //Create points
                    foreach(HelperEtabs.Point3D point3D in shell.Points)
                    {
                        string pointUniqueName = "";
                        ret = SapModel.PointObj.AddCartesian(point3D.XCoordinate, point3D.YCoordinate, point3D.ZCoordinate, ref pointUniqueName);

                        pointUniqueNameForShellList.Add(pointUniqueName);
                    }
                    */
                    List<double> xList = new List<double>();
                    List<double> yList = new List<double>();
                    List<double> zList = new List<double>();
                    foreach (HelperEtabs.Point3D point3D in shell.Points)
                    {
                        xList.Add(point3D.XCoordinate);
                        yList.Add(point3D.YCoordinate);
                        zList.Add(point3D.ZCoordinate);
                    }

                    double[] xArray = xList.ToArray();
                    double[] yArray = yList.ToArray();
                    double[] zArray = zList.ToArray();

                    //Create shell
                    string shellUniqueName = "";
                    ret = SapModel.AreaObj.AddByCoord(xList.Count(), ref xArray, ref yArray, ref zArray, ref shellUniqueName);
                    if (ret != 0) { shellFailToCreate.Add(shell.UniqueName); }

                    ret = SapModel.AreaObj.SetGroupAssign(shellUniqueName, translatedGroupNames[shell.Group]);

                    elementsIterator++;
                    double progressPercent = Math.Round(elementsIterator / totalElements * 100);

                    //Update progressbar every 5%
                    if (progressPercent % 5 == 0)
                    {
                        //Update progress bar
                        Dispatcher.Invoke((System.Action)(() =>
                        {
                            progressBarCreateElements.Value = progressPercent;
                        }));
                    }
                }

                foreach (HelperEtabs.Frame frame in frameList)
                {
                    double xI = frame.Points[0].XCoordinate;
                    double yI = frame.Points[0].YCoordinate;
                    double zI = frame.Points[0].ZCoordinate;

                    double xJ = frame.Points[1].XCoordinate;
                    double yJ = frame.Points[1].YCoordinate;
                    double zJ = frame.Points[1].ZCoordinate;

                    //Create frame
                    string frameUniqueName = "";
                    ret = SapModel.FrameObj.AddByCoord(xI, yI, zI, xJ, yJ, zJ, ref frameUniqueName);
                    if (ret != 0) { frameFailToCreate.Add(frame.UniqueName); }

                    ret = SapModel.AreaObj.SetGroupAssign(frameUniqueName, translatedGroupNames[frame.Group]);

                    elementsIterator++;
                    double progressPercent = Math.Round(elementsIterator / totalElements * 100);

                    //Update progressbar every 5%
                    if (progressPercent % 5 == 0)
                    {
                        //Update progress bar
                        Dispatcher.Invoke((System.Action)(() =>
                        {
                            progressBarCreateElements.Value = progressPercent;
                        }));
                    }
                }

                //Update UI
                Dispatcher.Invoke((Action)(() => { checkBoxCreateElements.IsChecked = true; }));

                if (shellFailToCreate.Count > 0 || frameFailToCreate.Count > 0) 
                {
                    MessageBox.Show("Warning exist! Please refer to generated copy_elements_warning.txt generated in the ETABS file folder");

                    //Write txt
                    FunctionCopyElements.WriteTxtFile(SapModel, shellFailToCreate, frameFailToCreate);
                }


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
