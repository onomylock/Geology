using System;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Objects;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geology.DrawWindow
{
    public class CView2DGraph : CView2D
    {
        bool png = true;
        protected override void OnMouseLeave(EventArgs e)
        {
            //if (Focused) Parent.Focus();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //if (!Focused) Focus();
        }

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetPathFromIDListW(
            UIntPtr pidl,                // Address of an item identifier list that
                                         // specifies a file or directory location
                                         // relative to the root of the namespace (the
                                         // desktop). 
            StringBuilder pszPath);     // Address of a buffer to receive the file system
                                        // path.
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFile(  // Copy EMF to file
IntPtr hemfSrc,   // Handle to EMF
String lpszFile // File
);

        [DllImport("gdi32.dll")]
        static extern int DeleteEnhMetaFile(  // Delete EMF
          IntPtr hemf // Handle to EMF
        );



        public class Rect
        {
            public double x1;
            public double x2;
            public double y1;
            public double y2;
            public double addx1;
            public double addx2;
            public Rect()
            {
                x1 = x2 = y1 = y2 = 0;
            }
            public Rect(double[] rect)
            {
                x1 = rect[0];
                x2 = rect[1];
                y1 = rect[2];
                y2 = rect[3];
                addx1 = rect[6];
                addx2 = rect[7];
            }
        }
        public enum MouseState { msNormal, msZoom }

        public ObservableCollection<Objects.CCurve> Curves;
        public ObservableCollection<Objects.CCurve> CurvesCopy;
        public ObservableCollection<Objects.CCurveInfo> CurvesInfoList;
        protected Stack<Rect> OrthoHistory;
        public System.Windows.Controls.Label labelArg = new System.Windows.Controls.Label();

        ContextMenuStrip mnuAxis, mnuProp;

        bool drawLegend;

        // for mouse zoom
        protected Rect mRect;
        public MouseState mState;
        protected bool mZoomStarted;

        private int LSize=3, LSkip=0, LWidth=1;

        protected double _Arg;

        bool clickOnHor, clickOnVer;

        public double Arg
        {
            get { return _Arg; }
            set { _Arg = value; CalcValues(); labelArg.Content = _Arg.ToString();}
        }



        public CView2DGraph() : base(false)
        {
            Curves = new ObservableCollection<Objects.CCurve>();
            CurvesCopy = new ObservableCollection<Objects.CCurve>();
            CurvesInfoList = null;
            OrthoHistory = new Stack<Rect>();

            mRect = new Rect();

            mState = MouseState.msNormal;
            mZoomStarted = false;

            //this.MouseDown += CView2DGraph_MouseDown;
            //this.MouseUp += CView2DGraph_MouseUp;
            //this.MouseMove += CView2DGraph_MouseMove;

            //base.MouseMove -= CView2D_MouseMove;
            //base.MouseDown -= CView2D_MouseDown;
            //base.MouseUp -= CView2D_MouseUp;

            base.captionHorAndVert.SetForCurves();

            _Arg = 0;

            mnuAxis = new ContextMenuStrip();
            ToolStripMenuItem mnuAdd = new ToolStripMenuItem("Add");
            ToolStripMenuItem mnuSub = new ToolStripMenuItem("Substract");
            ToolStripMenuItem mnuMul = new ToolStripMenuItem("Multiply");
            ToolStripMenuItem mnuDiv = new ToolStripMenuItem("Divide");

            mnuAdd.Click += mnuAdd_Click;
            mnuSub.Click += mnuSub_Click;
            mnuMul.Click += mnuMul_Click;
            mnuDiv.Click += mnuDiv_Click;

            mnuAxis.Items.AddRange(new ToolStripItem[] { mnuAdd, mnuSub, mnuMul, mnuDiv });


            mnuProp = new ContextMenuStrip();
            ToolStripMenuItem mnuSetCursor = new ToolStripMenuItem("Set cursor");
            ToolStripMenuItem mnuLabelSettings = new ToolStripMenuItem("Label settings");
            ToolStripMenuItem mnuSetFontSize = new ToolStripMenuItem("Font size");
            ToolStripMenuItem mnuSetXAxisLabel = new ToolStripMenuItem("X-axis label");
            ToolStripMenuItem mnuSetYAxisLabel = new ToolStripMenuItem("Y-axis label");
            ToolStripMenuItem mnuSaveBitmap = png ? new ToolStripMenuItem("Save as PNG") : new ToolStripMenuItem("Save as JPG");
            ToolStripMenuItem mnuSaveMetaFile = new ToolStripMenuItem("Save as EMF");
            ToolStripMenuItem mnuLegendSettings = new ToolStripMenuItem("Legend settings");
            ToolStripMenuItem mnuShowGrid = new ToolStripMenuItem("Show grid");

            mnuSetCursor.Click += mnuSetCursorClick;
            //mnuSetFontSize.Click += mnuSetFontSizeClick;
            //mnuSetXAxisLabel.Click += mnuSetXAxisLabelClick;
            //mnuSetYAxisLabel.Click += mnuSetYAxisLabelClick;
            mnuLabelSettings.Click += mnuLabelSettingsClick;
            mnuSaveBitmap.Click += mnuSaveBitmapClick;
            mnuLegendSettings.Click += mnuLegendSettingsClick;
            mnuSaveMetaFile.Click += mnuSaveMetaFileClick;
            mnuShowGrid.CheckOnClick = true;
            mnuShowGrid.Click += mnuShowGrid_Click;
            mnuShowGrid.Checked = captionHorAndVert.DrawGridFlag;

            mnuProp.Items.Add(mnuSetCursor);
            mnuProp.Items.Add(new ToolStripSeparator());
            mnuProp.Items.Add(mnuLabelSettings);
            mnuProp.Items.Add(mnuLegendSettings);
            mnuProp.Items.Add(new ToolStripSeparator());
            //mnuProp.Items.Add(mnuSetFontSize);
            //mnuProp.Items.Add(new ToolStripSeparator());
            //mnuProp.Items.Add(mnuSetXAxisLabel);
            //mnuProp.Items.Add(mnuSetYAxisLabel);
            mnuProp.Items.Add(mnuSaveBitmap);
            mnuProp.Items.Add(mnuSaveMetaFile);
            mnuProp.Items.Add(new ToolStripSeparator());
            mnuProp.Items.Add(mnuShowGrid);

            clickOnHor = false;
            clickOnVer = false;

            LSize = 3;
            LSkip = 0;
            LWidth = 1;

            drawLegend = false;
        }
        
        public virtual void AddCurve(Objects.CCurve c)
        {
            Curves.Add(c);
            CurvesCopy.Add(new Objects.CCurve(c));
        }
        public virtual void AddCurveWithInfo(Objects.CCurve c)
        {
            if (c.CurveArgs.Count != c.CurveArgs2.Count)
                c.CurveArgs2 = new List<double>(c.CurveArgs);

            Curves.Add(c);
            CurvesCopy.Add(new Objects.CCurve(c));
            CurvesInfoList.Add(new CCurveInfo());
        }
        public virtual void Reset()
        {
            for (int i = 0; i < Curves.Count; i++)
            {
                Curves[i] = new Objects.CCurve(CurvesCopy[i]);
                CurvesInfoList[i].Visible = true;
            }
            CalcValues();
            ViewAll(true, true);
        }
        public virtual void CalcValues()
        {
            for (int i = 0; i < Curves.Count; i++)
            {
                bool found;
                CurvesInfoList[i].Val = Curves[i].GetAt(_Arg, out found).ToString("G3");
            }
        }
        public virtual bool GetGabarit(double[] _g)
        {
            bool found = false;
            bool hasDoubleAxis = false;
            _g[0] = _g[2] = _g[6] = Double.MaxValue;
            _g[1] = _g[3] = _g[7] = -Double.MaxValue;
            for (int i = 0; i < Curves.Count; i++)
            {
                if (!CurvesInfoList[i].Visible)
                    continue;
                Curves[i].GetMinMax(_g,
                    base.captionHorAndVert.LogH,
                    base.captionHorAndVert.ZeroLogH,
                    base.captionHorAndVert.LogV,
                    base.captionHorAndVert.ZeroLogV);

                if (Curves[i].ThreeColumns)
                    hasDoubleAxis = true;
                if (!found)
                    found = true;
            }


            
            double d1 = _g[1] - _g[0];
            double d2 = _g[3] - _g[2];
            double d4 = _g[7] - _g[6];

            _g[0] -= d1 * 0.005;
            _g[1] += d1 * 0.005;
            _g[2] -= d2 * 0.005;
            _g[3] += d2 * 0.005;
            _g[6] -= d4 * 0.005;
            _g[7] += d4 * 0.005;

            //if (Ortho.DoubleAxis)
            //    _g[2] -= (3 + captionHorAndVert.myfontHor.GetHeightText("0")) * ((_g[3] - _g[2]) / Height);

            return found;
        }
        protected override void Draw()
        {
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();
            captionHorAndVert.GenerateGrid(Width, Height);
            captionHorAndVert.DrawScaleLbls(Width, Height);
            DrawObjetcs();
        }
        protected override void DrawObjetcs()
        {
            double[] ortho;
            Ortho.GetOrtho(out ortho);
            int indent = captionHorAndVert.myfontHor.GetHeightText("0");
            // curves
            for (int i=Math.Min(Curves.Count, CurvesInfoList.Count)- 1; i >= 0;i--)
            {
                if (CurvesInfoList == null || !CurvesInfoList[i].Visible)
                    continue;
                System.Windows.Media.Color col = CurvesInfoList[i].Color;
                GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);
                Curves[i].Draw(
                    base.captionHorAndVert.LogH,
                    base.captionHorAndVert.ZeroLogH,
                    base.captionHorAndVert.LogV,
                    base.captionHorAndVert.ZeroLogV,
                    Ortho.GetDHor / WidthLocal,
                    Ortho.GetDVert / HeightLocal,
                    CurvesInfoList[i],
                    captionHorAndVert.myfontHor,
                    ortho,
                    Height,
                    Width,
                    indent);
            }

            // red line
            double[] _Ortho;
            double _ArgToDraw = (base.captionHorAndVert.LogH ? Objects.CCurve.GetLog(_Arg, base.captionHorAndVert.ZeroLogH) : _Arg);
            Ortho.GetOrtho(out _Ortho);
            GLContex.glColor3f(1, 0, 0);
            GLContex.glLineWidth(1.0f);
            GLContex.glDisable(GLContex.GL_DEPTH_TEST);
            GLContex.glBegin(GLContex.GL_LINES);
            GLContex.glVertex3f((float)_ArgToDraw, (float)_Ortho[2], 0);
            GLContex.glVertex3f((float)_ArgToDraw, (float)_Ortho[3], 0);
            GLContex.glEnd();
            GLContex.glEnable(GLContex.GL_DEPTH_TEST);

            // zoom
            if (mZoomStarted)
            {
                // xor zoom rectangle
                GLContex.glEnable(GLContex.GL_COLOR_LOGIC_OP);
                GLContex.glLogicOp(GLContex.GL_XOR);

                GLContex.glColor3f(1, 1, 1);
                GLContex.glLineWidth(1.0f);

                GLContex.glBegin(GLContex.GL_LINE_LOOP);
                GLContex.glVertex3f((float)mRect.x1, (float)mRect.y1, 0);
                GLContex.glVertex3f((float)mRect.x2, (float)mRect.y1, 0);
                GLContex.glVertex3f((float)mRect.x2, (float)mRect.y2, 0);
                GLContex.glVertex3f((float)mRect.x1, (float)mRect.y2, 0);
                GLContex.glEnd();

                GLContex.glDisable(GLContex.GL_COLOR_LOGIC_OP);
            }
        }
        public virtual void DeleteCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            // delete from interface and memory
            CurvesInfoList.RemoveAt(i);
            Curves.RemoveAt(i);
            CurvesCopy.RemoveAt(i);
            Invalidate();
        }
        public virtual void DiffCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Differentiate();
            Invalidate();
        }
        private static double CalculateDifference(Objects.CCurve cBase, Objects.CCurve c)
        {
            try
            {
                double maxValue = 0.0;
                double res = 0.0;
                foreach (var val in cBase.CurveValues)
                    if (maxValue < Math.Abs(val)) maxValue = Math.Abs(val);

                for (int i = 0; i < Math.Min(cBase.CurveValues.Count, c.CurveValues.Count); i++)
                    res += (cBase.CurveValues[i] - c.CurveValues[i]) * (cBase.CurveValues[i] - c.CurveValues[i]);
                res /= maxValue * maxValue;
                res /= Math.Min(cBase.CurveValues.Count, c.CurveValues.Count);
                return Math.Sqrt(res)*100;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        private static double CalculateDifference2(Objects.CCurve cBase, Objects.CCurve c)
        {
            try
            {
                double maxValue = 0.0;
                double res = 0.0;
                foreach (var val in cBase.CurveValues)
                    if (maxValue < Math.Abs(val)) maxValue = Math.Abs(val);

                for (int i = 0; i < Math.Min(cBase.CurveValues.Count, c.CurveValues.Count); i++)
                        res += Math.Abs(cBase.CurveValues[i] - c.CurveValues[i]);
                res /= maxValue;
                return 100 * res / Math.Min(cBase.CurveValues.Count, c.CurveValues.Count);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public virtual void CalculateResidual(int i)
        {
            
        }
        public virtual void NormalizeOnCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Objects.CCurve ci = Curves[i];
            CurvesInfoList[i].Visible = false;
            for (int j = 0; j < Curves.Count; j++)
            {
                if (i == j || !CurvesInfoList[j].Visible)
                    continue;
                Curves[j].Normalize(ci);
            }
            Arg = Arg;
            Invalidate();
        }

        public virtual void AddCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            CurvesInfoList[i].Visible = false;
            for (int j = 0; j < Curves.Count; j++)
            {
                if (i == j || !CurvesInfoList[j].Visible)
                    continue;
                Curves[j] = Curves[j] + Curves[i];
            }
            Arg = Arg;
            Invalidate();
        }
        public virtual void SubstructCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            CurvesInfoList[i].Visible = false;
            for (int j = 0; j < Curves.Count; j++)
            {
                if (i == j || !CurvesInfoList[j].Visible)
                    continue;
                Curves[j] = Curves[j] - Curves[i];
            }
            Arg = Arg;
            Invalidate();
        }
        public virtual void MultiplyCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            CurvesInfoList[i].Visible = false;
            for (int j = 0; j < Curves.Count; j++)
            {
                if (i == j || !CurvesInfoList[j].Visible)
                    continue;
                Curves[j] = Curves[j] * Curves[i];
            }
            Arg = Arg;
            Invalidate();
        }
        public virtual void DivideCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            CurvesInfoList[i].Visible = false;
            for (int j = 0; j < Curves.Count; j++)
            {
                if (i == j || !CurvesInfoList[j].Visible)
                    continue;
                Objects.CCurve cj_copy = new Objects.CCurve(Curves[j]);
                try
                {
                    Curves[j] = Curves[j] / Curves[i];
                }
                catch (DivideByZeroException)
                {
                    System.Windows.MessageBox.Show("Division by zero!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Curves[j] = cj_copy;
                }
            }
            Arg = Arg;
            Invalidate();
        }
        public virtual void AbsCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Abs();
            Invalidate();
        }
        public virtual void SignCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Sign();
            Invalidate();
        }
        public virtual void SquareCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Square();
            Invalidate();
        }
        public virtual void RootCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Objects.CCurve ci = Curves[i];
            Objects.CCurve ci_copy = new Objects.CCurve(ci);
            try
            {
                for (int j = 0; j < ci.CurveSize; j++)
                {
                    if (ci.CurveValues[j] < 0)
                        //throw new Exception("Root from negavive value!");
                        ci.CurveValues[j] = -Math.Sqrt(-ci.CurveValues[j]);
                    else
                        ci.CurveValues[j] = Math.Sqrt(ci.CurveValues[j]);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ci = ci_copy;
            }
            Invalidate();
        }
        public virtual void CubeRootCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Objects.CCurve ci = Curves[i];
            Objects.CCurve ci_copy = new Objects.CCurve(ci);
            try
            {
                for (int j = 0; j < ci.CurveSize; j++)
                    if (ci.CurveValues[j] < 0.0)
                        ci.CurveValues[j] = -Math.Pow(-ci.CurveValues[j], 1.0 / 3.0);
                    else
                        ci.CurveValues[j] = Math.Pow(ci.CurveValues[j], 1.0 / 3.0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ci = ci_copy;
            }
            Invalidate();
        }
        public virtual void SmoothCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Smooth();
            Invalidate();
        }
        public virtual void Average(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].Average();
            Invalidate();
        }
        public virtual void CutLeftCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].CutLeft(Arg);
            Invalidate();
        }
        public virtual void CutRightCurve(int i)
        {
            if (i < 0 || i >= Curves.Count)
                return;
            Curves[i].CutRight(Arg);
            Invalidate();
        }
        internal void BuildSpline(int i)
        {
            
        }
        internal void Weld(int i1, int i2)
        {
            
        }
        internal void Weldx2(int i1, int i2)
        {
            
        }
        public virtual void ViewAll(bool x, bool y)
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1, -1, 1 };
            if (GetGabarit(newOrtho))
            {
                double[] oldOrtho;
                Ortho.GetOrtho(out oldOrtho);
                OrthoHistory.Push(new Rect(oldOrtho));
                Ortho.SetOrtho(newOrtho, x, y);
                Resize_Window();
                Invalidate();
            }
        }
        public virtual void ViewAll0()
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1, -1, 1 };
            if (GetGabarit(newOrtho))
            {
                double[] oldOrtho;
                Ortho.GetOrtho(out oldOrtho);
                double
                    x_max = Math.Max(Math.Abs(newOrtho[0]), Math.Abs(newOrtho[1])),
                    y_max = Math.Max(Math.Abs(newOrtho[2]), Math.Abs(newOrtho[3]));
                newOrtho[0] = -x_max;
                newOrtho[1] = x_max;
                newOrtho[2] = -y_max;
                newOrtho[3] = y_max;
                OrthoHistory.Push(new Rect(oldOrtho));
                Ortho.SetOrtho(newOrtho);
                Resize_Window();
                Invalidate();
            }
        }
        public virtual void Scale()
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
            double[] oldOrtho;
            Ortho.GetOrtho(out oldOrtho);
            double
                c01 = 0.5 * (oldOrtho[0] + oldOrtho[1]),
                c23 = 0.5 * (oldOrtho[2] + oldOrtho[3]),
                s01 = oldOrtho[1] - oldOrtho[0],
                s23 = oldOrtho[3] - oldOrtho[2];
            newOrtho[0] = c01 - s01;
            newOrtho[1] = c01 + s01;
            newOrtho[2] = c23 - s23;
            newOrtho[3] = c23 + s23;
            OrthoHistory.Push(new Rect(oldOrtho));
            Ortho.SetOrtho(newOrtho);
            Resize_Window();
            Invalidate();
        }
        public new int Load(String fileName)
        {
            try
            {
                int i;
                bool threeColumns = false;
                String buffer;
                StreamReader inputFile = new StreamReader(fileName);
                var sep = new char[] { ' ', '\t' };
                string[] words;

                if (inputFile == null)
                    return 1;

                // skip two lines
                buffer = inputFile.ReadLine();
                buffer = inputFile.ReadLine();

                Objects.CCurve c = new Objects.CCurve();

                buffer = inputFile.ReadLine();
                while (buffer != null && buffer.Length > 0)
                {
                    words = buffer.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    if (threeColumns && words.Length == 1)
                        c.Add(words[0]);
                    else
                    {
                        threeColumns = words.Length == 3;
                        if (threeColumns)
                        {
                            if (c.CurveArgs.Count == 0)
                            {
                                c.Add(double.Parse(words[0]), 1985.83333, double.Parse(words[2]));
                            }
                            else // > 1
                            {
                                c.Add(double.Parse(words[0]), double.Parse(words[1])-1, double.Parse(words[2]));
                                double dt_modeling = c.CurveArgs[c.CurveArgs.Count - 1] - c.CurveArgs[c.CurveArgs.Count - 2];
                                double dt_pseudo_real = dt_modeling / 365;
                                c.CurveArgs2[c.CurveArgs2.Count - 1] = c.CurveArgs2[c.CurveArgs2.Count - 2] + dt_pseudo_real;
                            }
                        }
                        else
                            c.Add(double.Parse(words[0]), double.Parse(words[1]));
                    }
                    buffer = inputFile.ReadLine();
                }

                inputFile.Close();
                AddCurve(c);
                Invalidate();
            }
            catch (Exception ex)
            {
                return 1;
            }
            //Resize_Window();
            //Invalidate();
            return 0;
        }
        public int Save(String fileName, int i)
        {
            if (i < 0 || i >= Curves.Count)
                return 1;
            Objects.CCurve ci = Curves[i];
            StreamWriter outputFile = new StreamWriter(fileName);
            // skip two lines
            outputFile.WriteLine();
            outputFile.WriteLine();
            // write curve
            for (int j = 0; j < ci.CurveSize; j++)
                outputFile.WriteLine(ci.CurveArgs[j].ToString() + "\t" + ci.CurveValues[j].ToString());
            outputFile.Close();
            return 0;
        }
        public virtual void ZoomBack()
        {
            if (OrthoHistory.Count == 0)
                return;
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
            Rect r = OrthoHistory.Pop();
            newOrtho[0] = r.x1;
            newOrtho[1] = r.x2;
            newOrtho[2] = r.y1;
            newOrtho[3] = r.y2;
            Ortho.SetOrtho(newOrtho);
            Resize_Window();
            Invalidate();
        }

        //protected void CView2DGraph_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        if (clickOnHor = captionHorAndVert.ClickOnHor(e.X, e.Y, WidthLocal, HeightLocal))
        //            this.ContextMenuStrip = mnuAxis;
        //        else if (clickOnVer = captionHorAndVert.ClickOnVer(e.X, e.Y, WidthLocal, HeightLocal))
        //            this.ContextMenuStrip = mnuAxis;
        //        else
        //            this.ContextMenuStrip = mnuProp;
        //        CView2D_MouseDown(sender, e);
        //        return;
        //    }
        //    this.ContextMenuStrip = null;
        //    if (mState == MouseState.msNormal)
        //    {
        //        CView2D_MouseDown(sender, e);
        //        double x, y;
        //        ScreenToWorldCoord(e.X, e.Y, out x, out y);
        //        Arg = (base.captionHorAndVert.LogH ? Objects.CCurve.GetLinear(x, base.captionHorAndVert.ZeroLogH) : x);
        //        Invalidate();
        //    }
        //    else
        //    {
        //        ScreenToWorldCoord(e.X, e.Y, out mRect.x1, out mRect.y1);
        //        mRect.x2 = mRect.x1;
        //        mRect.y2 = mRect.y1;
        //        mZoomStarted = true;
        //    }
        //}
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (e.Button == MouseButtons.Right)
            {
                if (clickOnHor = captionHorAndVert.ClickOnHor(e.X, e.Y, WidthLocal, HeightLocal))
                    this.ContextMenuStrip = mnuAxis;
                else if (clickOnVer = captionHorAndVert.ClickOnVer(e.X, e.Y, WidthLocal, HeightLocal))
                    this.ContextMenuStrip = mnuAxis;
                else
                    this.ContextMenuStrip = mnuProp;
                base.OnMouseDown(e);
                return;
            }
            this.ContextMenuStrip = null;
            if (mState == MouseState.msNormal)
            {
                base.OnMouseDown(e);
                double x, y;
                ScreenToWorldCoord(e.X, e.Y, out x, out y);
                Arg = (base.captionHorAndVert.LogH ? Objects.CCurve.GetLinear(x, base.captionHorAndVert.ZeroLogH) : x);
                Invalidate();
            }
            else
            {
                ScreenToWorldCoord(e.X, e.Y, out mRect.x1, out mRect.y1);
                mRect.x2 = mRect.x1;
                mRect.y2 = mRect.y1;
                mZoomStarted = true;
            }
        }

        //protected void CView2DGraph_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (mState == MouseState.msNormal)
        //        CView2D_MouseUp(sender, e);
        //    else
        //    {
        //        ScreenToWorldCoord(e.X, e.Y, out mRect.x2, out mRect.y2);
        //        mState = MouseState.msNormal;
        //        mZoomStarted = false;
        //        double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
        //        double[] oldOrtho;
        //        Ortho.GetOrtho(out oldOrtho);
        //        newOrtho[0] = Math.Min(mRect.x1, mRect.x2);
        //        newOrtho[1] = Math.Max(mRect.x1, mRect.x2);
        //        newOrtho[2] = Math.Min(mRect.y1, mRect.y2);
        //        newOrtho[3] = Math.Max(mRect.y1, mRect.y2);
        //        OrthoHistory.Push(new Rect(oldOrtho));
        //        Ortho.SetOrtho(newOrtho);
        //        Resize_Window();
        //        Invalidate();
        //    }
        //}
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (mState == MouseState.msNormal)
                base.OnMouseUp(e);
            else
            {
                ScreenToWorldCoord(e.X, e.Y, out mRect.x2, out mRect.y2);
                mState = MouseState.msNormal;
                mZoomStarted = false;
                double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
                double[] oldOrtho;
                Ortho.GetOrtho(out oldOrtho);
                newOrtho[0] = Math.Min(mRect.x1, mRect.x2);
                newOrtho[1] = Math.Max(mRect.x1, mRect.x2);
                newOrtho[2] = Math.Min(mRect.y1, mRect.y2);
                newOrtho[3] = Math.Max(mRect.y1, mRect.y2);
                OrthoHistory.Push(new Rect(oldOrtho));
                Ortho.SetOrtho(newOrtho);
                Resize_Window();
                Invalidate();
            }
        }

        //protected void CView2DGraph_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (mState == MouseState.msNormal)
        //        CView2D_MouseMove(sender, e);
        //    else
        //    {
        //        ScreenToWorldCoord(e.X, e.Y, out mRect.x2, out mRect.y2);
        //        Invalidate();
        //    }
        //}
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (!Focused) Focus();

            if (mState == MouseState.msNormal)
                base.OnMouseMove(e);
            else
            {
                ScreenToWorldCoord(e.X, e.Y, out mRect.x2, out mRect.y2);
                Invalidate();
            }
        }

        public void SetLog(bool LogX, double zLogX, bool LogY, double zLogY)
        {
            base.captionHorAndVert.LogH = LogX;
            base.captionHorAndVert.ZeroLogH = zLogX;
            base.captionHorAndVert.LogV = LogY;
            base.captionHorAndVert.ZeroLogV = zLogY;
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1, -1, 1 };
            GetGabarit(newOrtho);
            if (LogX)
            {
                newOrtho[0] = Objects.CCurve.GetLog(newOrtho[0], zLogX);
                newOrtho[1] = Objects.CCurve.GetLog(newOrtho[1], zLogX);
            }
            if (LogY)
            {
                newOrtho[2] = Objects.CCurve.GetLog(newOrtho[2], zLogY);
                newOrtho[3] = Objects.CCurve.GetLog(newOrtho[3], zLogY);
            }
            Ortho.SetOrtho(newOrtho, LogX, LogY);
            if (Curves.Count > 0)
            {
                ViewAll(LogX, LogY);
            }
            else
            {
                Resize_Window();
                Invalidate();
            }
        }
        private System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {

            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();

            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
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
        private void mnuSaveBitmapClick(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            //saveFileDialog.InitialDirectory = FilesWorking.LastOpenSaveDirectory;
           
            
            //saveFileDialog.InitialDirectory = GetLastOpenSaveDirectory();
            saveFileDialog.Filter = png ? "png-files (*.png)|*.png" : "Jpg-files (*.jpg)|*.jpg";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(this.Size.Width, this.Size.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
                System.Drawing.Point loc = this.PointToScreen(new System.Drawing.Point(0, 0));
                g.CopyFromScreen(loc, new System.Drawing.Point(0, 0), this.Size);
                System.Drawing.Size size = new System.Drawing.Size(b.Width, b.Height);
                using (System.Drawing.Bitmap newb = new System.Drawing.Bitmap(b, size))
                {
                    System.Drawing.Imaging.ImageCodecInfo jgpEncoder = GetEncoder(png ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg);
                    var encoder = System.Drawing.Imaging.Encoder.Quality;
                    //var encoder2 = System.Drawing.Imaging.Encoder.ColorDepth;
                    var myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    var myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(encoder, 100L);
                    //var myEncoderParameter2 = new System.Drawing.Imaging.EncoderParameter(encoder2, 100L);

                    myEncoderParameters.Param[0] = myEncoderParameter;
                    //myEncoderParameters.Param[1] = myEncoderParameter2;

                    newb.Save(saveFileDialog.FileName, jgpEncoder, myEncoderParameters);
                }
                //b.Save(saveFileDialog.FileName);
                g.Dispose();
            }
        }

        private void mnuSaveMetaFileClick(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            //saveFileDialog.InitialDirectory = FilesWorking.LastOpenSaveDirectory;


            //saveFileDialog.InitialDirectory = GetLastOpenSaveDirectory();
            saveFileDialog.Filter =  "emf-files (*.emf)|*.emf";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                double[] ort;
                Ortho.GetOrtho(out ort);
                Graphics offScreenBufferGraphics;
                Metafile m;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (offScreenBufferGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
                    {
                        IntPtr deviceContextHandle = offScreenBufferGraphics.GetHdc();

                        m = new Metafile(
                          stream,
                          deviceContextHandle,
                          new RectangleF(0, 0, this.Size.Width, this.Size.Height),
                            MetafileFrameUnit.Pixel,
                          EmfType.EmfPlusOnly);
                        offScreenBufferGraphics.ReleaseHdc();
                    }
                    stream.Dispose();
                }
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(m))
                {
                    double horzValue = HeightLocal / (ort[3] - ort[2]);
                    double vertValue = WidthLocal / (ort[1] - ort[0]);
                    //float horzAxcis = Width - WidthLocal;
                    float horzAxcis = Height - HeightLocal;

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    System.Drawing.Imaging.MetafileHeader metafileHeader = m.GetMetafileHeader();


                    g.PageUnit = System.Drawing.GraphicsUnit.Pixel;
                    g.SetClip(new System.Drawing.RectangleF(horzAxcis, 0, WidthLocal, HeightLocal));

                    // clears the image and colors the entire background
                    g.Clear(System.Drawing.Color.White);

                    captionHorAndVert.DrawGridMetaFile(WidthLocal, HeightLocal, g, horzValue, vertValue, horzAxcis);
                    for (int i = Math.Min(Curves.Count, CurvesInfoList.Count) - 1; i >= 0; i--)
                    {
                        if (CurvesInfoList == null || !CurvesInfoList[i].Visible)
                            continue;
                        
                    }
                    //g.Dispose();
//Ось X

                    g.SetClip(new System.Drawing.RectangleF(horzAxcis, HeightLocal, WidthLocal, Height-HeightLocal));

                    // clears the image and colors the entire background
                    g.Clear(System.Drawing.Color.White);
                    captionHorAndVert.DrawScaleLblsHorzMetaFile(WidthLocal, HeightLocal , g, horzValue, vertValue, horzAxcis);

                    g.SetClip(new System.Drawing.RectangleF(0, 0,Width- WidthLocal,  HeightLocal));

                    // clears the image and colors the entire background
                    g.Clear(System.Drawing.Color.White);
                    captionHorAndVert.DrawScaleLblsVertMetaFile(WidthLocal, HeightLocal, g, horzValue, vertValue, horzAxcis);

                    g.Dispose();




                }
                IntPtr iptrMetafileHandle = m.GetHenhmetafile();

                // Export metafile to an image file
                IntPtr iptrMetafileHandleCopy =CopyEnhMetaFile(iptrMetafileHandle, saveFileDialog.FileName);
                if (iptrMetafileHandleCopy != null)
                {
                    DeleteEnhMetaFile(iptrMetafileHandleCopy);
                }
                // Delete the metafile from memory
                DeleteEnhMetaFile(iptrMetafileHandle);
                offScreenBufferGraphics.Dispose();
                m.Dispose();
               
            }
            
        }
        private void mnuLegendSettingsClick(object sender, EventArgs e)
        {
           
        }
        private void mnuLabelSettingsClick(object sender, EventArgs e)
        {
           
        }
        private void mnuSetCursorClick(object sender, EventArgs e)
        {
            
        }

        private void mnuAdd_Click(object sender, EventArgs e)
        {
            
        }
        private void mnuSub_Click(object sender, EventArgs e)
        {
            
        }
        private void mnuMul_Click(object sender, EventArgs e)
        {
           
        }
        private void mnuDiv_Click(object sender, EventArgs e)
        {
            
        }

        public void ChangeOrtho(double[] globalOrthoBox)
        {
            double[] newOrtho;
            if (globalOrthoBox.Length == 8)
                newOrtho = new double[] { -1, 1, -1, 1, -1, 1, -1, 1 };
            else
                newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };

            newOrtho[0] = globalOrthoBox[0];
            newOrtho[1] = globalOrthoBox[1];
            newOrtho[2] = globalOrthoBox[2];
            newOrtho[3] = globalOrthoBox[3];
            newOrtho[4] = globalOrthoBox[4];
            newOrtho[5] = globalOrthoBox[5];
            if (globalOrthoBox.Length == 8)
            {
                newOrtho[6] = globalOrthoBox[6];
                newOrtho[7] = globalOrthoBox[7];
            }

            Ortho.SetOrtho(newOrtho);
            Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
            Resize_Window();
        }
    }
}
