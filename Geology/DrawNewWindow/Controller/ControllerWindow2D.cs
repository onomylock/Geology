using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geology.OpenGL.OpenGLControl;
using Geology.DrawWindow;
using Geology.OpenGL;
using System.Windows.Forms;
using Geology.DrawNewWindow.View;
using Geology.DrawNewWindow.Model;

namespace Geology.DrawNewWindow.Controller
{
    public class ControllerWindow2D : IControllerWindow
    {
        public double[] BoundingBox { get; set; }
        public PageType Page { get { return page; } set { page = value; } }
        public COrthoControlProport Ortho { get; set; }
        public FontGeology wellFont { get; set; }
        public FontGeology fontReceivers { get; set; }
        public FontGeology paletteFont { get; set; }
        public IViewWindow View { get { return view; } set { view = value; } }
        public CaptionAxisHorAndVert CaptionHorAndVert { get { return captionHorAndVert; } set { captionHorAndVert = value; } }
        public IModelWindow Model { get; set; }
        


        protected IViewWindow view;
        protected CaptionAxisHorAndVert captionHorAndVert;
        protected Geology.MainWindow window;
        protected int XPrevious = 0, YPrevious = 0;
        protected bool mouseDown = false;
        protected System.Windows.Forms.ContextMenuStrip mnu; // base context menu 
        protected PageType page = PageType.Model;

        public ControllerWindow2D(bool EqualScale)
		{
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            Ortho = new COrthoControlProport("X", "Y", Height / (double)Width, BoundingBox, EqualScale);
            captionHorAndVert = new CaptionAxisHorAndVert(hdc, oglcontext, "Arial", 16, Ortho, Width, Height);
            wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 14);
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            
            //this.Disposed += OpenGLControl_Disposed;
            //this.Resize += Controller_Resize;

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
            //this.ContextMenuStrip = mnu;
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

        protected void mnuLabelSettings_Click(object sender, EventArgs e)
        {
        }

        protected void mnuStartView_Click(object sender, EventArgs e)
        {
            Ortho.ClearView();
            Resize_Window();
            Invalidate();
        }

        protected void mnuShowGrid_Click(object sender, EventArgs e)
        {
            captionHorAndVert.DrawGridFlag = ((System.Windows.Forms.ToolStripMenuItem)sender).Checked;
            Resize_Window();
            Invalidate();
        }

        protected void ScreenToWorldCoord(int eX, int eY, out double resX, out double resY)
        {
            //Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, eX, eY, out resX, out resY, captionHorAndVert.GetIndentVert);
            Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, eX, eY, out resX, out resY, captionHorAndVert.myfontVert.GetHeightText("0"), captionHorAndVert.myfontHor.GetHeightText("0") +
                (captionHorAndVert.DoubleAxis ? captionHorAndVert.myfontHor.GetHeightText("0") + 3 : 0));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseDown)
            {
                double prevX, prevY, curX, curY;
                Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, XPrevious, YPrevious, out prevX, out prevY, captionHorAndVert.GetIndentVert);
                Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, e.X, e.Y, out curX, out curY, captionHorAndVert.GetIndentVert);
                if (Ortho.DoubleAxis)
                {
                    double x2, x2Prev;
                    Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, XPrevious, out x2Prev, captionHorAndVert.GetIndentVert);
                    Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, e.X, out x2, captionHorAndVert.GetIndentVert);
                    Ortho.Translate(-curX + prevX, -curY + prevY, -x2 + x2Prev);
                }
                else
                {
                    if (-curX + prevX != 0 || -curY + prevY != 0)
                        curX = curX;
                    Ortho.Translate(-curX + prevX, -curY + prevY);
                }
                XPrevious = e.X; YPrevious = e.Y;
                Resize_Window();
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = true;
            XPrevious = e.X; YPrevious = e.Y;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            double resX, resY;
            Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, e.X, e.Y, out resX, out resY, captionHorAndVert.GetIndentHor, captionHorAndVert.GetIndentVert);
            if (Ortho.DoubleAxis)
            {
                double x2;
                Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, e.X, out x2, captionHorAndVert.GetIndentVert);
                Ortho.Scale(resX, resY, x2, e.Delta);
            }
            else
                Ortho.Scale(resX, resY, e.Delta);

            Resize_Window();
            Invalidate();
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

        public void DisposedController(object sender, EventArgs e)
        {
            // здесь происходит очистка шрифта, необходимая функция, чтобы не утекала память
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            captionHorAndVert.ClearFont();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }
    }
}
