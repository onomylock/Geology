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
            }
        }
        public double[] BoundingBox { get; set; }
        public FontGeology caption { get; set; }
        public ContextMenuStrip mnu { get; set; }
        public COrthoControlProport Ortho { get; set; }
		public CPerspective project { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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

       
        protected Geology.MainWindow window;
        protected int XPrevious = 0, YPrevious = 0;
        protected bool mouseDown = false;
       
       

        public ControllerWindow2D(bool EqualScale)
		{

        }

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
            mouseDown = true;
            XPrevious = e.X; YPrevious = e.Y;
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
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

        public void SetMainRef(MainWindow _window)
        {
            window = _window;
        }
    }
}
