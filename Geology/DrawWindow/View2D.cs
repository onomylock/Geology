/*
 Файл содержит классы:
 * 
 * CView2D, потомок OpenGL.OpenGLControl.OpenGLControl - область для отображения графических 
 * объектов в 2D (в проекциях). Объекты этого типа отслеживают события нажатия мыши,
 * обеспечивают работу со своим контекстным меню для изменения настроек отображения
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using System.Windows.Forms;
using System.Windows.Media;

namespace Geology.DrawWindow
{
    public enum PageType
    {
        None,
        ObservingSystem,
        Model,
        InversionModel,
        InversionResults,
        ViewModel,
        Meshes
    }
    public class CView2D : OpenGL.OpenGLControl.OpenGLControl
    {
        public PageType page = PageType.Model;
        protected override void OnMouseLeave(EventArgs e)
        {
            //if (Focused) Parent.Focus();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //if (!Focused) Focus();
        }

        protected Geology.MainWindow window;
        public COrthoControlProport Ortho;
        protected CaptionAxisHorAndVert captionHorAndVert;
        protected int XPrevious = 0, YPrevious = 0;
        protected bool mouseDown = false;
        public int WidthLocal, HeightLocal;
        public FontGeology wellFont;
		public FontGeology fontReceivers;
        public FontGeology paletteFont;
        protected String wellFontName;
        protected int wellFontSize;
        protected System.Windows.Forms.ContextMenuStrip mnu; // base context menu
        public CView2D(bool EqualScale) : base()
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            Ortho = new COrthoControlProport("X", "Y", Height / (double)Width, BoundingBox, EqualScale);

            captionHorAndVert = new CaptionAxisHorAndVert(hdc, oglcontext, "Arial", 16, Ortho, Width, Height);
            wellFontName = "Arial";
            wellFontSize = 14;
            wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, wellFontName, wellFontSize);
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            //this.MouseMove += CView2D_MouseMove;
            //this.MouseDown += CView2D_MouseDown;
            //this.MouseUp += CView2D_MouseUp;
            //this.MouseWheel += CView2D_MouseWheel;
            this.Disposed += OpenGLControl_Disposed;

            window = null;

            mnu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem mnuShowGrid = new System.Windows.Forms.ToolStripMenuItem("Show grid");
            System.Windows.Forms.ToolStripMenuItem mnuStartView = new System.Windows.Forms.ToolStripMenuItem("Start view");
            System.Windows.Forms.ToolStripMenuItem mnuLabelSettings = new System.Windows.Forms.ToolStripMenuItem("Label Settings");
            System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap = new System.Windows.Forms.ToolStripMenuItem("Save as JPG");


            mnuShowGrid.CheckOnClick = true;
            mnuShowGrid.Checked = captionHorAndVert.DrawGridFlag;
            mnuShowGrid.Click += mnuShowGrid_Click;
            mnuLabelSettings.Click += mnuLabelSettings_Click;
            mnuSaveBitmap.Click += mnuSaveBitmap_Click;

            mnuStartView.Click += mnuStartView_Click;
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuShowGrid, mnuStartView, mnuLabelSettings, mnuSaveBitmap });
            this.ContextMenuStrip = mnu;

            /*
            System.Windows.Forms.ContextMenuStrip mnu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem mnuShowGrid = new System.Windows.Forms.ToolStripMenuItem("Show grid");
            System.Windows.Forms.ToolStripMenuItem mnuStartView = new System.Windows.Forms.ToolStripMenuItem("Start view");

            mnuShowGrid.CheckOnClick = true;
            mnuShowGrid.Checked = captionHorAndVert.DrawGridFlag;
            mnuShowGrid.Click += mnuShowGrid_Click;

            mnuStartView.Click += mnuStartView_Click;
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuShowGrid, mnuStartView });
            this.ContextMenuStrip = mnu;
            */


        }


        public void SetLabelFormat(int FontSize, string NameFontStyle)
        {

            captionHorAndVert.DrawScaleLbls(Width, Height, 1.0);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            captionHorAndVert.myfontHor.ClearFont();
            captionHorAndVert.myfontHor = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
            captionHorAndVert.myfontVert.ClearFont();
            captionHorAndVert.myfontVert = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, NameFontStyle, FontSize - 2);
            captionHorAndVert.myfontHor_ind.ClearFont();
            captionHorAndVert.myfontHor_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize - 4);
            captionHorAndVert.myfontVert_ind.ClearFont();
            captionHorAndVert.myfontVert_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, NameFontStyle, FontSize - 6);
            wellFont.ClearFont();
            wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
            fontReceivers.ClearFont();
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
            paletteFont.ClearFont();
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
            Invalidate();
        }

        public void SetFontSize(int fsize)
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            captionHorAndVert.myfontHor.ClearFont();
            captionHorAndVert.myfontHor = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
            captionHorAndVert.myfontVert.ClearFont();
            captionHorAndVert.myfontVert = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, "Arial", fsize - 2);
            captionHorAndVert.myfontHor_ind.ClearFont();
            captionHorAndVert.myfontHor_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", fsize - 2);
            captionHorAndVert.myfontVert_ind.ClearFont();
            captionHorAndVert.myfontVert_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, "Arial", fsize - 4);
            wellFont.ClearFont();
            wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
            fontReceivers.ClearFont();
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
            paletteFont.ClearFont();
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
            Invalidate();
        }
        public int GetFontSize()
        {
            return captionHorAndVert.myfontHor.FontSize;
        }
        public string GetFontStyle()
        {
            return captionHorAndVert.myfontHor.StyleFont;
        }
        protected void SetBaseContextMenu()
        {
            this.ContextMenuStrip = mnu;
        }
        private void OpenGLControl_Disposed(object sender, EventArgs e)
        {
            // здесь происходит очистка шрифта, необходимая функция, чтобы не утекала память
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            captionHorAndVert.ClearFont();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        protected void mnuShowGrid_Click(object sender, EventArgs e)
        {
            captionHorAndVert.DrawGridFlag = ((System.Windows.Forms.ToolStripMenuItem)sender).Checked;
            Resize_Window();
            Invalidate();
        }

        protected void mnuLabelSettings_Click(object sender, EventArgs e)
        {
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

        private void mnuSaveBitmap_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            //saveFileDialog.InitialDirectory = FilesWorking.LastOpenSaveDirectory;


            //saveFileDialog.InitialDirectory = GetLastOpenSaveDirectory();
            saveFileDialog.Filter = "Png-files (*.png)|*.png";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(this.Size.Width, this.Size.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
                System.Drawing.Point loc = this.PointToScreen(new System.Drawing.Point(0, 0));
                g.CopyFromScreen(loc, new System.Drawing.Point(0, 0), this.Size);
                System.Drawing.Size size = new System.Drawing.Size(b.Width * 2, b.Height * 2);
                using (System.Drawing.Bitmap newb = new System.Drawing.Bitmap(b, size))
                {
                    System.Drawing.Imaging.ImageCodecInfo jgpEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                    System.Drawing.Imaging.ImageCodecInfo pngEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Png);
                    var encoder = System.Drawing.Imaging.Encoder.Quality;
                    //var encoder2 = System.Drawing.Imaging.Encoder.ColorDepth;
                    var myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    var myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(encoder, 100L);
                    //var myEncoderParameter2 = new System.Drawing.Imaging.EncoderParameter(encoder2, 100L);

                    myEncoderParameters.Param[0] = myEncoderParameter;
                    //myEncoderParameters.Param[1] = myEncoderParameter2;

                    newb.Save(saveFileDialog.FileName, pngEncoder, myEncoderParameters);
                }
                //b.Save(saveFileDialog.FileName);
                g.Dispose();
            }
        }



        protected override void Draw()
        {
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();
            //double scaleZ = Objects.
            captionHorAndVert.GenerateGrid(Width, Height);
            captionHorAndVert.DrawScaleLbls(Width, Height);
            DrawObjetcs();
        }
        protected void mnuStartView_Click(object sender, EventArgs e)
        {
            Ortho.ClearView();
            Resize_Window();
            Invalidate();
        }
        protected virtual void DrawObjetcs()
        {

        }

        protected override void UpdateViewMatrix()
        {
            try
            {
                captionHorAndVert.GetNewViewport(Width, Height, out int[] viewPoint);
                WidthLocal = viewPoint[2];
                HeightLocal = viewPoint[3];

                double[] ortho;
                Ortho.CoefHeightToWidth = HeightLocal / (double)WidthLocal;
                Ortho.GetOrtho(out ortho);

                GLContex.glMatrixMode(GLContex.GL_PROJECTION);
                GLContex.glLoadIdentity();
                GLContex.glViewport(viewPoint[0], viewPoint[1], viewPoint[2], viewPoint[3]);
                GLContex.glOrtho(ortho[0], ortho[1], ortho[2], ortho[3], Ortho.GetMinZBuf, Ortho.GetMaxZBuf);

                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glLoadIdentity();
            }
            catch(Exception ex)
            {

            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CView2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "CView2D";
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CView2D_MouseDown);
            //this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CView2D_MouseMove);
            this.ResumeLayout(false);

        }

        //protected void CView2D_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (mouseDown)
        //    {
        //        double prevX, prevY, curX, curY;
        //        Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, XPrevious, YPrevious, out prevX, out prevY, captionHorAndVert.GetIndentVert);
        //        Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, e.X, e.Y, out curX, out curY, captionHorAndVert.GetIndentVert);
        //        Ortho.Translate(-curX + prevX, -curY + prevY);
        //        XPrevious = e.X; YPrevious = e.Y;
        //        Resize_Window();
        //        Invalidate();
        //    }
        //}
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (!Focused) Focus();

            if (mouseDown)
            {
                double prevX, prevY, curX, curY;
                Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, XPrevious, YPrevious, out prevX, out prevY, captionHorAndVert.GetIndentVert);
                Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, e.X, e.Y, out curX, out curY, captionHorAndVert.GetIndentVert);
                if (Ortho.DoubleAxis)
                {
                    double x2, x2Prev;
                    Ortho.ConvertScreenToWorldCoordAddition(WidthLocal, XPrevious, out x2Prev, captionHorAndVert.GetIndentVert);
                    Ortho.ConvertScreenToWorldCoordAddition(WidthLocal, e.X, out x2, captionHorAndVert.GetIndentVert);
                    Ortho.Translate(-curX + prevX, -curY + prevY, -x2 + x2Prev);
                }
                else
                    Ortho.Translate(-curX + prevX, -curY + prevY);
                XPrevious = e.X; YPrevious = e.Y;
                Resize_Window();
                Invalidate();
            }
        }

        //protected void CView2D_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    mouseDown = true;
        //    XPrevious = e.X; YPrevious = e.Y;
        //}
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = true;
            XPrevious = e.X; YPrevious = e.Y;
        }

        //protected void CView2D_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    mouseDown = false;
        //}
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            double resX, resY;
            Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, e.X, e.Y, out resX, out resY, captionHorAndVert.GetIndentHor, captionHorAndVert.GetIndentVert);
            if (Ortho.DoubleAxis)
            {
                double x2;
                Ortho.ConvertScreenToWorldCoordAddition(WidthLocal, e.X, out x2, captionHorAndVert.GetIndentVert);
                Ortho.Scale(resX, resY, x2, e.Delta);
            }
            else
                Ortho.Scale(resX, resY, e.Delta);

            Resize_Window();
            Invalidate();
        }
        protected void ScreenToWorldCoord(int eX, int eY, out double resX, out double resY)
        {
            //Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, eX, eY, out resX, out resY, captionHorAndVert.GetIndentVert);
            Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, eX, eY, out resX, out resY, captionHorAndVert.myfontVert.GetHeightText("0"), captionHorAndVert.myfontHor.GetHeightText("0") + 
                (captionHorAndVert.DoubleAxis ? captionHorAndVert.myfontHor.GetHeightText("0") + 3 : 0));
            
        }
    }
}
