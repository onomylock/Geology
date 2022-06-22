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
using Geology.Projection;
using Geology.Objects;

namespace Geology.DrawNewWindow.Controller
{
    public class ControllerWindow2D : IControllerWindow
    {

        public IViewWindow View { get { return view; } set { view = value; } }
        public PageType Page { get { return page; } set { page = value; } }
        public IModelWindow Model
        {
            get 
            {
                if (model == null)
                {
                    model = new ModelWindow();
                    model.Objects.Add(new CGeoObject());
                    model.GlobalBoundingBox[0] = -10000;
                    model.GlobalBoundingBox[1] = 10000;
                    model.GlobalBoundingBox[2] = -10000;
                    model.GlobalBoundingBox[3] = 10000;
                    model.GlobalBoundingBox[4] = -10000;
                    model.GlobalBoundingBox[5] = 10000;
                    BoundingBox = model.GlobalBoundingBox;
                }
                return model; 
            }
            set
            {
                model = value;
                //if (model == null)
                //    model = new ModelWindow();
                //else
                //{
                //    model = value;
                //    //this.ResizeView();
                //    this.View.UpdateViewMatrix();
                //    this.View.Draw();
                //}
            }
        }
        public double[] BoundingBox { get; set; }
        public FontGeology caption { get; set; }
        public ContextMenuStrip mnu { get; set; }
        public COrthoControlProport Ortho { get; set; }
		public CPerspective project { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public event InvalidateDelegate;
        //public FontGeology wellFont { get; set; }
        //public FontGeology fontReceivers { get; set; }
        //public FontGeology paletteFont { get; set; } 
        //public CaptionAxisHorAndVert CaptionHorAndVert { get { return captionHorAndVert; } set { captionHorAndVert = value; } }
        //public event A InvalidateEvent;
        public event InvalidateDelegate InvalidateEvent;

        public void InvokeEvent()
		{
            InvalidateEvent.Invoke();
		}

        protected IModelWindow model;
        protected IViewWindow view;
        protected PageType page = PageType.Model;
        protected ToolStripMenuItem mnuSaveBitmap;
        protected Cursor Cursor;
        protected IntPtr Handle;

        //protected CaptionAxisHorAndVert captionHorAndVert;
        protected Geology.MainWindow window;
        protected int XPrevious = 0, YPrevious = 0;
        protected bool mouseDown = false;
        //protected System.Windows.Forms.ContextMenuStrip mnu; // base context menu 
       

        public ControllerWindow2D(bool EqualScale)
		{

            //Win32.wglMakeCurrent((IntPtr)view?.Hdc, (IntPtr)view?.OglContext);
            //Ortho = new COrthoControlProport("X", "Y", view.Height / (double)view.Width, BoundingBox, EqualScale);
            //captionHorAndVert = new CaptionAxisHorAndVert(view.Hdc, view.OglContext, "Arial", 16, Ortho, view.Width, view.Height);
            //wellFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 14);
            //paletteFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            //fontReceivers = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            
            ////this.Disposed += OpenGLControl_Disposed;
            ////this.Resize += Controller_Resize;

            //window = null;

            //mnu = new System.Windows.Forms.ContextMenuStrip();
            //System.Windows.Forms.ToolStripMenuItem mnuShowGrid = new System.Windows.Forms.ToolStripMenuItem("Show grid");
            //System.Windows.Forms.ToolStripMenuItem mnuStartView = new System.Windows.Forms.ToolStripMenuItem("Start view");
            //System.Windows.Forms.ToolStripMenuItem mnuLabelSettings = new System.Windows.Forms.ToolStripMenuItem("Label Settings");
            

            //mnuShowGrid.CheckOnClick = true;
            //mnuShowGrid.Checked = captionHorAndVert.DrawGridFlag;
            //mnuShowGrid.Click += mnuShowGrid_Click;
            //mnuLabelSettings.Click += mnuLabelSettings_Click;
            //mnuStartView.Click += mnuStartView_Click;
            //mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuShowGrid, mnuStartView, mnuLabelSettings, mnuSaveBitmap });
            //this.ContextMenuStrip = mnu;
        }

		//

        

		//public void SetLabelFormat(int FontSize, string NameFontStyle)
		//{

		//    captionHorAndVert.DrawScaleLbls(view.Width, view.Height, 1.0);
		//    Win32.wglMakeCurrent(view.Hdc, (IntPtr)view.OglContext);
		//    captionHorAndVert.myfontHor.ClearFont();
		//    captionHorAndVert.myfontHor = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
		//    captionHorAndVert.myfontVert.ClearFont();
		//    captionHorAndVert.myfontVert = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Vertical, NameFontStyle, FontSize - 2);
		//    captionHorAndVert.myfontHor_ind.ClearFont();
		//    captionHorAndVert.myfontHor_ind = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize - 4);
		//    captionHorAndVert.myfontVert_ind.ClearFont();
		//    captionHorAndVert.myfontVert_ind = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Vertical, NameFontStyle, FontSize - 6);
		//    wellFont.ClearFont();
		//    wellFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
		//    fontReceivers.ClearFont();
		//    fontReceivers = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
		//    paletteFont.ClearFont();
		//    paletteFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, NameFontStyle, FontSize);
		//    InvalidateEvent();
		//}

		//public void SetFontSize(int fsize)
		//{
		//    Win32.wglMakeCurrent(view.Hdc, (IntPtr)view.OglContext);
		//    captionHorAndVert.myfontHor.ClearFont();
		//    captionHorAndVert.myfontHor = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
		//    captionHorAndVert.myfontVert.ClearFont();
		//    captionHorAndVert.myfontVert = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Vertical, "Arial", fsize - 2);
		//    captionHorAndVert.myfontHor_ind.ClearFont();
		//    captionHorAndVert.myfontHor_ind = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", fsize - 2);
		//    captionHorAndVert.myfontVert_ind.ClearFont();
		//    captionHorAndVert.myfontVert_ind = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Vertical, "Arial", fsize - 4);
		//    wellFont.ClearFont();
		//    wellFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
		//    fontReceivers.ClearFont();
		//    fontReceivers = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
		//    paletteFont.ClearFont();
		//    paletteFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", fsize);
		//    InvalidateEvent();
		//}

		//public int GetFontSize()
		//{
		//    return captionHorAndVert.myfontHor.FontSize;
		//}

		//public string GetFontStyle()
		//{
		//    return captionHorAndVert.myfontHor.StyleFont;
		//}


		//protected void SetBaseContextMenu()
		//{
		//    this.ContextMenuStrip = mnu;
		//}

		protected void Invalidate_event()
		{
            InvalidateEvent?.Invoke();
		}

        protected void mnuLabelSettings_Click(object sender, EventArgs e)
        {
        }

        protected void mnuStartView_Click(object sender, EventArgs e)
        {
            Ortho.ClearView();
            view.ResizeWindow();
            InvalidateEvent.Invoke();
        }

        protected void mnuShowGrid_Click(object sender, EventArgs e)
        {
            view.CaptionHorAndVert.DrawGridFlag = ((System.Windows.Forms.ToolStripMenuItem)sender).Checked;
            view.ResizeWindow();
            InvalidateEvent.Invoke();
        }

        protected void ScreenToWorldCoord(int eX, int eY, out double resX, out double resY)
        {
            //Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, eX, eY, out resX, out resY, captionHorAndVert.GetIndentVert);
            Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, eX, eY, out resX, out resY, view.CaptionHorAndVert.myfontVert.GetHeightText("0"), view.CaptionHorAndVert.myfontHor.GetHeightText("0") +
                (view.CaptionHorAndVert.DoubleAxis ? view.CaptionHorAndVert.myfontHor.GetHeightText("0") + 3 : 0));
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (mouseDown)
            {
                double prevX, prevY, curX, curY;
                Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, XPrevious, YPrevious, out prevX, out prevY, view.CaptionHorAndVert.GetIndentVert);
                Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, e.X, e.Y, out curX, out curY, view.CaptionHorAndVert.GetIndentVert);
                if (Ortho.DoubleAxis)
                {
                    double x2, x2Prev;
                    Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, XPrevious, out x2Prev, view.CaptionHorAndVert.GetIndentVert);
                    Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, e.X, out x2, view.CaptionHorAndVert.GetIndentVert);
                    Ortho.Translate(-curX + prevX, -curY + prevY, -x2 + x2Prev);
                }
                else
                {
                    if (-curX + prevX != 0 || -curY + prevY != 0)
                        //curX = curX;
                    Ortho.Translate(-curX + prevX, -curY + prevY);
                }
                XPrevious = e.X; YPrevious = e.Y;
                view.ResizeWindow();
                InvalidateEvent?.Invoke();
            }
        }

        public virtual void OnMouseDown(MouseEventArgs e)
        {
            //if (!Focused) Focus();

            mouseDown = true;
            XPrevious = e.X; YPrevious = e.Y;
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
            //if (!Focused) Focus();

            mouseDown = false;
        }

        public virtual void OnMouseWheel(MouseEventArgs e)
        {
            double resX, resY;
            Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, e.X, e.Y, out resX, out resY, view.CaptionHorAndVert.GetIndentHor, view.CaptionHorAndVert.GetIndentVert);
            if (Ortho.DoubleAxis)
            {
                double x2;
                Ortho.ConvertScreenToWorldCoordAddition(view.WidthLocal, e.X, out x2, view.CaptionHorAndVert.GetIndentVert);
                Ortho.Scale(resX, resY, x2, e.Delta);
            }
            else
                Ortho.Scale(resX, resY, e.Delta);

            view.ResizeWindow();
            InvalidateEvent.Invoke();
        }

        //public void DisposedController(object sender, EventArgs e)
        //{
        //    // здесь происходит очистка шрифта, необходимая функция, чтобы не утекала память
        //    Win32.wglMakeCurrent(view.Hdc, (IntPtr)view.OglContext);
        //    captionHorAndVert.ClearFont();
        //    Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        //}

        public void SetMainRef(MainWindow _window)
        {
            window = _window;
        }
    }
}
