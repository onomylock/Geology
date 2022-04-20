using Geology.DrawNewWindow.View;
using Geology.DrawWindow;
using System;
using Geology.OpenGL.OpenGLControl;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geology.Utilities;
using System.Windows.Forms;
using System.Windows.Input;


namespace Geology.DrawNewWindow.Controller
{
    public enum PlaneType
    { XY = 0, XZ, YZ }

    class ControllerWindow2D : OpenGLControl, IControllerWindow
	{
		public IViewWindow View{ get { return view; } set { view = value; } }
		public PageType Page{ get { return page; } set { page = value; } }
        public PlaneType axisType;

        public Dictionary<PageType, List<IViewportObjectsSelectable>> selectableObjects = new Dictionary<PageType, List<IViewportObjectsSelectable>>();
		public Dictionary<PageType, List<IViewportObjectsDrawable>> drawableObjects = new Dictionary<PageType, List<IViewportObjectsDrawable>>();
		public Dictionary<PageType, List<IViewportObjectsClickable>> clickableObjects = new Dictionary<PageType, List<IViewportObjectsClickable>>();
		public Dictionary<PageType, List<IViewportObjectsContextmenuClickable>> contextMenuClickableObjects = new Dictionary<PageType, List<IViewportObjectsContextmenuClickable>>();
		public Dictionary<PageType, List<IViewportMouseMoveReaction>> mouseMoveReactionObjects = new Dictionary<PageType, List<IViewportMouseMoveReaction>>();
        public COrthoControlProport Ortho;
        public int WidthLocal, HeightLocal;

        private IViewWindow view;
		private PageType page = PageType.Model;
        private int XPrevious = 0, YPrevious = 0;
        

        protected bool mouseDown = false;
        protected CaptionAxisHorAndVert captionHorAndVert;

        ControllerWindow2D()
		{

		}

		public void SetBoundingBox()
		{
			//Array.Copy(model.GlobalBoundingBox, BoundingBox, model.GlobalBoundingBox.Length);
		}

        public void setRotateAndNameAxes(PlaneType numAxis)
        {
            axisType = numAxis;
            switch (axisType)
            {
                case PlaneType.XY: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Y"; break;
                case PlaneType.XZ: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Z"; break;
                case PlaneType.YZ: Ortho.HorAxisName = "Y"; Ortho.VertAxisName = "Z"; break;
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
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

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = true;
            XPrevious = e.X; YPrevious = e.Y;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Focused) Focus();

            mouseDown = false;
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
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

        private double MetersPerPixel()
        {
            double[] orthV;
            Ortho.GetOrtho(out orthV);
            int indent = captionHorAndVert.myfontVert.GetHeightText("0");
            return (orthV[1] - orthV[0]) / (WidthLocal - indent);
        }

        private void SetContextMenu(double x, double y)
        {
            double mPerPixel = MetersPerPixel();
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            foreach (var obj in contextMenuClickableObjects[page])
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    this.ContextMenuStrip = m;
                    this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            foreach (var obj in contextMenuClickableObjects[PageType.None])
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    this.ContextMenuStrip = m;
                    this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            this.ContextMenuStrip = mnu;
        }

        protected override void Draw() => view?.Draw();
        protected override void UpdateViewMatrix() => view?.UpdateViewMatrix();
    }
}
