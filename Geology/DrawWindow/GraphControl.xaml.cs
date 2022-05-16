using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.ObjectModel;
using Geology.Objects;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using Geology.Utilities;
using Geology.DrawNewWindow.Controller;

namespace Geology.Controls
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        // Grif
        public delegate void ItemRemovedEventHandler(object obj);
        public event ItemRemovedEventHandler ItemRemoved;
        public String lastOpenSaveDirectory = "";
        public int logSX = -8;
        public int logSY = -8;
        // List<System.Windows.Media.Color> oldColors = new List<System.Windows.Media.Color>();
        private void NotifyItemRemoved(object id)
        {
            if (ItemRemoved != null)
                ItemRemoved(id);
        }

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetPathFromIDListW(
            UIntPtr pidl,                // Address of an item identifier list that
                                         // specifies a file or directory location
                                         // relative to the root of the namespace (the
                                         // desktop). 
            StringBuilder pszPath);     // Address of a buffer to receive the file system
                                        // path.


        //public ObservableCollection<Objects.CCurveInfo> CurvesInfoList;
        private void chkSelectAllCurves_Checked(object sender, RoutedEventArgs e)
        {
            TGraph.CurvesInfoList.ToList().ForEach(x => x.Visible = true);
            TGraph.Invalidate();
        }
        protected void chkSelectAllCurves_UnChecked(object sender, RoutedEventArgs e)
        {
            TGraph.CurvesInfoList.ToList().ForEach(x => x.Visible = false);
            TGraph.Invalidate();
        }
        public GraphControl()
        {
            //CurvesInfoList = new ObservableCollection<Objects.CCurveInfo>();

            InitializeComponent();

            CurvesList.ItemsSource = TGraph.CurvesInfoList;
            //TGraph.CurvesInfoList = CurvesInfoList;
            TGraph.labelArg = LabelArgument;
            buttonDownX.IsEnabled = false;
            buttonUpX.IsEnabled = false;
            buttonDownY.IsEnabled = false;
            buttonUpY.IsEnabled = false;

            TGraph.CurvesInfoList.CollectionChanged += OnCurvesInfoListCollectionChanged;
        }
        private void SetCurvesOldColors()
        {
            //int i;
            //for (i = 0; i < Math.Min(oldColors.Count, CurvesInfoList.Count); i++)
            //    CurvesInfoList[i].Color = oldColors[i];

            //for (i = oldColors.Count; i < CurvesInfoList.Count; i++)
            //    oldColors.Add(CurvesInfoList[i].Color);
        }
        //private void RefreshOldColors()
        //{
        //    try
        //    {
        //        int i;
        //        for (i = 0; i < Math.Min(oldColors.Count, CurvesInfoList.Count); i++)
        //            oldColors[i] = CurvesInfoList[i].Color;
        //        for (; i < CurvesInfoList.Count; i++)
        //            oldColors.Add(CurvesInfoList[i].Color);
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}
        private void OnCurvesInfoListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                //SetCurvesOldColors();
            }
        }
        protected void CurvesListCheckedVis(object sender, RoutedEventArgs e)
        {
            TGraph.Invalidate();
        }
        protected void CurvesList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TGraph.Invalidate();
        }
        protected void GraphContextMenuLostMouse(object sender, MouseEventArgs e)
        {
            GraphContextMenu.Visibility = Visibility.Hidden;
        }
        protected void MouseDoubleClickCurvesList(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && CurvesList.CurrentCell.Column.DisplayIndex == 1)
            {
                System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
                cd.Color = System.Drawing.Color.FromArgb(
                    TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.A,
                    TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.R,
                    TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.G,
                    TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.B);

                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color = System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    TGraph.Invalidate();
                }
            }

            //RefreshOldColors();
        }
        protected void CurvesList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurvesList.SelectedIndex >= 0 && CurvesList.SelectedIndex < CurvesList.Items.Count)
                CurvesList.ContextMenu.Visibility = Visibility.Visible;
            else
                CurvesList.ContextMenu.Visibility = Visibility.Collapsed;
        }
        protected void CurvesSettings_Click(object sender, RoutedEventArgs e)
        {
            
        }
        protected void SelectedCurvesReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
               
                    foreach (var item in CurvesList.SelectedItems)
                    {
                        int i = TGraph.CurvesInfoList.IndexOf((CCurveInfo)item);
                        TGraph.Curves[i]= new Objects.CCurve(TGraph.CurvesCopy[i]);
                        TGraph.CurvesInfoList[i].Visible = true;
                    }
                TGraph.CalcValues();
            }
            catch (Exception ex)
            {

            }
            TGraph.Invalidate();
        }
        private void CurvesSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                String path = lastOpenSaveDirectory;//GetLastOpenSaveDirectory();
                if (Utilities.LittleTools.GetPath(ref path) != 0)
                    return;
                lastOpenSaveDirectory = path;

                foreach (var item in CurvesList.SelectedItems)
                {
                    var curve = (CCurveInfo)item;
                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                if (CurvesList.SelectedItems.Count < 1 && CurvesList.SelectedItem != null)
                {
                    var removedObject = (CCurveInfo)CurvesList.SelectedItem;
                    TGraph.DeleteCurve(TGraph.CurvesInfoList.IndexOf(removedObject));
                    NotifyItemRemoved(removedObject.Id);
                }
                else
                {
                    List<CCurveInfo> removedCurves = new List<CCurveInfo>();
                    List<int> indexes = new List<int>();
                    foreach (var item in CurvesList.SelectedItems)
                    {
                        var removedObject = (CCurveInfo)item;
                        indexes.Add(TGraph.CurvesInfoList.IndexOf(removedObject));
                        removedCurves.Add(removedObject);
                    }
                    for (int i = indexes.Count - 1; i >= 0; i-- )
                        TGraph.DeleteCurve(indexes[i]);
                    foreach (var curve in removedCurves)
                        NotifyItemRemoved(curve.Id);
                }
            }
            catch(Exception ex)
            {

            }
        }
        protected void CurvesDiff_Click(object sender, RoutedEventArgs e)
        {
            TGraph.DiffCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesNormalize_Click(object sender, RoutedEventArgs e)
        {
            TGraph.NormalizeOnCurve(CurvesList.SelectedIndex);
        }

        private void CalculateResidual_Click(object sender, RoutedEventArgs e)
        {
            TGraph.CalculateResidual(CurvesList.SelectedIndex);
        }
        protected void CurvesAdd_Click(object sender, RoutedEventArgs e)
        {
            TGraph.AddCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesSubstruct_Click(object sender, RoutedEventArgs e)
        {
            TGraph.SubstructCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesMultiply_Click(object sender, RoutedEventArgs e)
        {
            TGraph.MultiplyCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesDivide_Click(object sender, RoutedEventArgs e)
        {
            TGraph.DivideCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesAbs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.AbsCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
          
        }
        protected void CurvesSign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.SignCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesSquare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.SquareCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.RootCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesCubeRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.CubeRootCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
        }

        private void BuildSpline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.BuildSpline(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }            
        }
        private void WeldCurves(bool doubleWeld = false)
        {
            if (CurvesList.SelectedItems == null || CurvesList.SelectedItem == null)
                return;
            if (CurvesList.SelectedItems.Count != 2)
                return;

            int i1 = CurvesList.Items.IndexOf(CurvesList.SelectedItem);
            int i2 = CurvesList.Items.IndexOf(CurvesList.SelectedItems[1]);
            if (i2 == i1)
                i2 = CurvesList.Items.IndexOf(CurvesList.SelectedItems[0]);

            if (!doubleWeld)
                TGraph.Weld(i1, i2);
            else
                TGraph.Weldx2(i1, i2);
        }
        private void Weld_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WeldCurves();
            }
            catch (Exception ex)
            {

            }
        }
        private void Weldx2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WeldCurves(true);
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesSmooth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.SmoothCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
 //           TGraph.SmoothCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesAverage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.Average(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
            //           TGraph.SmoothCurve(CurvesList.SelectedIndex);
        }
        
        protected void CurvesCutLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.CutLeftCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }
        }
        protected void CurvesCutRight_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (CurvesList.SelectedItems == null)
                    return;
                foreach (var item in CurvesList.SelectedItems)
                    TGraph.CutRightCurve(TGraph.CurvesInfoList.IndexOf((CCurveInfo)item));
            }
            catch (Exception ex)
            {

            }

       //     TGraph.CutRightCurve(CurvesList.SelectedIndex);
        }
        protected void CurvesViewAll_Click(object sender, RoutedEventArgs e)
        {
            TGraph.ViewAll(true, true);
        }
        protected void CurvesViewAll0_Click(object sender, RoutedEventArgs e)
        {
            TGraph.ViewAll0();
        }
        protected void CurvesScale_Click(object sender, RoutedEventArgs e)
        {
            TGraph.Scale();
        }
        protected String GetLastOpenSaveDirectory()
        {
            String mru = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU";
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(mru);
            List<string> filePaths = new List<string>();

            foreach (string skName in rk.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey sk = rk.OpenSubKey(skName);
                object value = sk.GetValue("0");
                if (value == null)
                    break;

                byte[] data = (byte[])(value);

                IntPtr p = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, p, data.Length);

                // get number of data;
                UInt32 cidl = (UInt32)Marshal.ReadInt16(p);

                // get parent folder
                UIntPtr parentpidl = (UIntPtr)((UInt32)p);

                StringBuilder path = new StringBuilder(256);

                SHGetPathFromIDListW(parentpidl, path);

                Marshal.Release(p);

                return path.ToString();
            }
            return "";
        }
        protected void CurvesLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = true;
            //openFileDialog.InitialDirectory = GetLastOpenSaveDirectory();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String fName in openFileDialog.FileNames)
                {
                    String cName = fName;
                    if (TGraph.Load(cName) == 0)
                    {
                        if (cName.Contains('\\'))
                            cName = cName.Substring(cName.LastIndexOf('\\') + 1);
                        TGraph.CurvesInfoList.Add(new CCurveInfo(cName));
                        if (TGraph.Curves.Count == 1)
                            TGraph.ViewAll(true, true);
                        else
                            TGraph.Invalidate();
                        TGraph.Arg = TGraph.Arg;
                    }
                }
            }
        }
        protected void CurvesSave_Click(object sender, RoutedEventArgs e)
        {
            if (CurvesList.SelectedIndex < 0 || CurvesList.SelectedIndex >= CurvesList.Items.Count)
                return;
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            //saveFileDialog.InitialDirectory = GetLastOpenSaveDirectory();
            saveFileDialog.Filter = "All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                TGraph.Save(saveFileDialog.FileName, CurvesList.SelectedIndex);
        }
        protected void CurvesReset_Click(object sender, RoutedEventArgs e)
        {
            TGraph.Reset();
        }
        protected void CurvesZoom_Click(object sender, RoutedEventArgs e)
        {
            //TGraph.mState = DrawWindow.CView2DGraph.MouseState.msZoom;
            TGraph.mState = ControllerCurve.MouseState.msZoom;
        }
        protected void CurvesZoomBack_Click(object sender, RoutedEventArgs e)
        {
            TGraph.ZoomBack();
        }
        protected void LogXCheck(object sender, RoutedEventArgs e)
        {
            buttonDownX.IsEnabled = true;
            buttonUpX.IsEnabled = true;
            TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }
        protected void LogXUnchecked(object sender, RoutedEventArgs e)
        {
            buttonDownX.IsEnabled = false;
            buttonUpX.IsEnabled = false;
            TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }
        protected void LogYCheck(object sender, RoutedEventArgs e)
        {
            buttonDownY.IsEnabled = true;
            buttonUpY.IsEnabled = true;
            TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }
        protected void LogYUnchecked(object sender, RoutedEventArgs e)
        {
            buttonDownY.IsEnabled = false;
            buttonUpY.IsEnabled = false;
            TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }
        //protected void sLogX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if ((bool)fLogX.IsChecked)
        //        TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)sLogX.Value), (bool)fLogY.IsChecked, Math.Pow(10, (double)sLogY.Value));
        //}
        //protected void sLogY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if ((bool)fLogY.IsChecked)
        //        TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)sLogX.Value), (bool)fLogY.IsChecked, Math.Pow(10, (double)sLogY.Value));
        //}

        private void CurvesSetColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.Color = System.Drawing.Color.FromArgb(
                TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.A,
                TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.R,
                TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.G,
                TGraph.CurvesInfoList[CurvesList.SelectedIndex].Color.B);

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in CurvesList.SelectedItems)
                    ((CCurveInfo)item).Color = System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B);
                TGraph.Invalidate();
            }

            //RefreshOldColors();
        }

        private void buttonUpX_Click(object sender, RoutedEventArgs e)
        {
            logSX++;
            if ((bool)fLogX.IsChecked)
                TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }

        private void buttonDownX_Click(object sender, RoutedEventArgs e)
        {
            logSX--;
            if ((bool)fLogX.IsChecked)
                TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }

        private void buttonUpY_Click(object sender, RoutedEventArgs e)
        {
            logSY++;
            if ((bool)fLogY.IsChecked)
                TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }

        private void buttonDownY_Click(object sender, RoutedEventArgs e)
        {
            logSY--;
            if ((bool)fLogY.IsChecked)
                TGraph.SetLog((bool)fLogX.IsChecked, Math.Pow(10, (double)logSX), (bool)fLogY.IsChecked, Math.Pow(10, (double)logSY));
        }
    }

}
