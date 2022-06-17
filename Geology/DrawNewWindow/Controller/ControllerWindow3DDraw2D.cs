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
	public class Factory3DDraw2D : IFactoryController
	{
        public string Name { get { return name; } }

		public IControllerWindow Controller { get; set; }
		public IViewWindow View { get; set; }

		string name = "Controller3DDraw2D";

		public void CreateControllerAndView(int Width, int Height, IntPtr Handle, ToolStripMenuItem mnuSaveBitmap, EPlaneType axisType)
		{
			
		}
	}

    public class ControllerWindow3DDraw2D : ControllerWindow2D, IControllerWindow
	{	
        public EPlaneType axisType;
#pragma warning disable CS0108 // Член скрывает унаследованный член: отсутствует новое ключевое слово
		public event Action InvalidateEvent;
#pragma warning restore CS0108 // Член скрывает унаследованный член: отсутствует новое ключевое слово

		//public Dictionary<PageType, List<IViewportObjectsSelectable>> selectableObjects = new Dictionary<PageType, List<IViewportObjectsSelectable>>();
		//public Dictionary<PageType, List<IViewportObjectsDrawable>> drawableObjects = new Dictionary<PageType, List<IViewportObjectsDrawable>>();
		//public Dictionary<PageType, List<IViewportObjectsClickable>> clickableObjects = new Dictionary<PageType, List<IViewportObjectsClickable>>();
		//public Dictionary<PageType, List<IViewportObjectsContextmenuClickable>> contextMenuClickableObjects = new Dictionary<PageType, List<IViewportObjectsContextmenuClickable>>();
		//public Dictionary<PageType, List<IViewportMouseMoveReaction>> mouseMoveReactionObjects = new Dictionary<PageType, List<IViewportMouseMoveReaction>>();


		//private PageType page = PageType.Model;

		private double zRange;
        private double selectionX0 = 0, selectionX1 = 0;
        private double selectionY0 = 0, selectionY1 = 0;
        private double prevX = 0, prevY = 0;
        private double curX = 0, curY = 0;
        private bool selectionStarted = false;
        private bool selectionFinished = true;
        private bool toAdd = false;
        private bool mouseMoved = false;
        private bool LMBDown = false;
        private bool RMBDown = false;
        
        public ControllerWindow3DDraw2D(int Width, int Height, IntPtr Handle, ToolStripMenuItem mnuSaveBitmap, EPlaneType axisType) : base(true)
		{
			this.axisType = axisType;
			zRange = 1e+7;
            this.Handle = Handle;
            BoundingBox = new double[] { -10000, 10000, -10000, 10000, -10000, 10000 };
            Ortho = new COrthoControlProport("X", "Y", Height / (double)Width, BoundingBox, true);
            ChangeOrtho(new double[] { -1, 1, -1, 1, -1, 1 });

            //foreach (var item in (PageType[])Enum.GetValues(typeof(PageType)))
            //{
            //	selectableObjects.Add(item, new List<IViewportObjectsSelectable>());
            //	drawableObjects.Add(item, new List<IViewportObjectsDrawable>());
            //	clickableObjects.Add(item, new List<IViewportObjectsClickable>());
            //	contextMenuClickableObjects.Add(item, new List<IViewportObjectsContextmenuClickable>());
            //	mouseMoveReactionObjects.Add(item, new List<IViewportMouseMoveReaction>());
            //}

            //this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;

            //captionHorAndVert = new CaptionAxisHorAndVert(IntPtr.Zero, 0, "Arial", 16, Ortho, Width, Height);
            //wellFont = new FontGeology(IntPtr.Zero, 0, FontGeology.TypeFont.Horizontal, "Arial", 14);
            //paletteFont = new FontGeology(IntPtr.Zero, 0, FontGeology.TypeFont.Horizontal, "Arial", 16);
            //fontReceivers = new FontGeology(IntPtr.Zero, 0, FontGeology.TypeFont.Horizontal, "Arial", 16);
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

            //view = new ViewWindow2D(captionHorAndVert, Ortho, model.viewportObjectsDrawablesGet(page), axisType, zRange, page, Width, Height, BoundingBox, fontReceivers, paletteFont, Handle);
            Win32.wglMakeCurrent((IntPtr)view?.Hdc, (IntPtr)view?.OglContext);

			//captionHorAndVert = new CaptionAxisHorAndVert(view.Hdc, view.OglContext, "Arial", 16, Ortho, Width, Height);
			//wellFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 14);
			//paletteFont = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 16);
			//fontReceivers = new FontGeology(view.Hdc, view.OglContext, FontGeology.TypeFont.Horizontal, "Arial", 16);
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

			//this.Disposed += OpenGLControl_Disposed;
			//this.Resize += Controller_Resize;

			window = null;

            mnu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem mnuShowGrid = new System.Windows.Forms.ToolStripMenuItem("Show grid");
            System.Windows.Forms.ToolStripMenuItem mnuStartView = new System.Windows.Forms.ToolStripMenuItem("Start view");
            System.Windows.Forms.ToolStripMenuItem mnuLabelSettings = new System.Windows.Forms.ToolStripMenuItem("Label Settings");


            mnuShowGrid.CheckOnClick = true;
            mnuShowGrid.Checked = view.CaptionHorAndVert.DrawGridFlag;
            mnuShowGrid.Click += mnuShowGrid_Click;
            mnuLabelSettings.Click += mnuLabelSettings_Click;
            mnuStartView.Click += mnuStartView_Click;
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuShowGrid, mnuStartView, mnuLabelSettings, mnuSaveBitmap });
            //this.Resize += Controller_Resize;	
        }

		//public void ResizeView()
		//{
		//	view.Width = Width;
		//	view.Height = Height;
		//}

		//public void SetMainRef(MainWindow _window)
  //      {
  //          window = _window;
  //      }

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            InvalidateEvent();
        }

		private void Controller_Resize(object sender, EventArgs e)
		{
			//this.ResizeView();
			view.ResizeWindow();
		    view.Draw();
		}

		private void StartSelection(double x, double y)
        {
            selectionX1 = selectionX0 = x;
            selectionY1 = selectionY0 = y;
            selectionStarted = true;
            selectionFinished = false;

            toAdd = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl);

            InvalidateEvent();
        }
        
        private void ContinueSelection(double x, double y)
        {
            selectionX1 = x;
            selectionY1 = y;
            //this.ControllerWindow3DDraw2D_MouseMove();

            InvalidateEvent();
        }
        
        private void FinishSelection(double x, double y)
        {
            selectionX1 = x;
            selectionY1 = y;
            selectionStarted = false;
            selectionFinished = true;
            //this.ControllerWindow3DDraw2D_MouseMove();
            toAdd = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl);

            if (selectionX1 < selectionX0) Utilities.LittleTools.Swap(ref selectionX0, ref selectionX1);
            if (selectionY1 < selectionY0) Utilities.LittleTools.Swap(ref selectionY0, ref selectionY1);

            if (Model.viewportObjectsSelectablesGet(page) != null)
                foreach (var obj in model.viewportObjectsSelectablesGet(page))
                    obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);

            foreach (var obj in model.viewportObjectsSelectablesGet(PageType.None))
                obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);
            //if (selectableObjects.ContainsKey(page))
            //foreach (var obj in selectableObjects[page])
            //obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);

            //foreach (var obj in selectableObjects[PageType.None])
            //obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);

            InvalidateEvent();
        }

        private void ConvertScreenToWorldCoord(int screenX, int screenY, out double x, out double y)
        {
            Ortho.ConvertScreenToWorldCoord(view.WidthLocal, view.HeightLocal, screenX, screenY, out x, out y, view.CaptionHorAndVert.myfontVert.GetHeightText("0"), view.CaptionHorAndVert.myfontHor.GetHeightText("0"));
            double c;
            int indent;
            double[] orthV;
            Ortho.GetOrtho(out orthV);

            indent = view.CaptionHorAndVert.myfontVert.GetHeightText("0");
            c = (orthV[1] - orthV[0]) / (view.WidthLocal - indent);
            x = orthV[0] + (screenX - indent) * c;

            screenY = view.HeightLocal + indent - screenY;
            indent = view.CaptionHorAndVert.myfontHor.GetHeightText("0");
            c = (orthV[3] - orthV[2]) / (view.HeightLocal - indent);
            y = orthV[2] + (screenY - indent) * c;
        }
        
        private double MetersPerPixel()
        {
            double[] orthV;
            Ortho.GetOrtho(out orthV);
            int indent = view.CaptionHorAndVert.myfontVert.GetHeightText("0");
            return (orthV[1] - orthV[0]) / (view.WidthLocal - indent);
        }

        void SetContextMenu(double x, double y)
        {
            double mPerPixel = MetersPerPixel();
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            foreach (var obj in model.viewportObjectsContextmenuClickablesGet(page))
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    mnu = m;
                    mnu.ItemClicked += ContextMenuStrip_ItemClicked;
                    //this.ContextMenuStrip = m;
                    //this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            foreach (var obj in model.viewportObjectsContextmenuClickablesGet(PageType.None))
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    mnu = m;
                    mnu.ItemClicked += ContextMenuStrip_ItemClicked;
                    //this.ContextMenuStrip = m;
                    //this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            //this.ContextMenuStrip = mnu;
        }

        protected void GetRays(double x, double y, out double[] r1, out double[] r2)
        {
            r1 = new double[3];
            r2 = new double[3];
            switch (axisType)
            {
                case EPlaneType.XY:
                    r1[0] = x; r2[0] = x;
                    r1[1] = y; r2[1] = y;
                    r1[2] = BoundingBox[4]; r2[2] = BoundingBox[5];
                    break;
                case EPlaneType.XZ:
                    r1[0] = x; r2[0] = x;
                    r1[1] = BoundingBox[2]; r2[1] = BoundingBox[3];
                    r1[2] = y; r2[2] = y;
                    break;
                case EPlaneType.YZ:
                    r1[0] = BoundingBox[0]; r2[0] = BoundingBox[1];
                    r1[1] = x; r2[1] = x;
                    r1[2] = y; r2[2] = y;
                    break;
            }
        }

        public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);

            //if (!Focused) Focus();

            if (e.Button == MouseButtons.Right)
            {
                RMBDown = true;
                SetContextMenu(x, y);
                return;
            }

            LMBDown = true;
            mouseMoved = false;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                StartSelection(x, y);
                prevX = x;
                prevY = y;
                return;
            }

            
            base.OnMouseDown(e);
        }

        public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            LMBDown = false;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);
            //if (!Focused) Focus();

            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            if (selectionStarted && !selectionFinished)
            {
                FinishSelection(x, y);
                InvalidateEvent();
                return;
            }

            if (!mouseMoved)
            {
                double[] r1, r2;
                GetRays(x, y, out r1, out r2);

                double mPerPixel = MetersPerPixel();
                bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                foreach (var item in model.viewportObjectsClickablesGet(page))
                    item.Click(x, y, r1, r2, mPerPixel, axisType, ctrlPressed, shiftPressed);
                foreach (var item in model.viewportObjectsClickablesGet(PageType.None))
                    item.Click(x, y, r1, r2, mPerPixel, axisType, ctrlPressed, shiftPressed);
            }


            base.OnMouseUp(e);
            InvalidateEvent();
        }
       
        public override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);

            double mPerPixel = MetersPerPixel();
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!selectionStarted)
            {

                double[] r1 = new double[3];
                double[] r2 = new double[3];
                switch (axisType)
                {
                    case EPlaneType.XY:
                        r1[0] = x; r2[0] = x;
                        r1[1] = y; r2[1] = y;
                        r1[2] = BoundingBox[4]; r2[2] = BoundingBox[5];
                        break;
                    case EPlaneType.XZ:
                        r1[0] = x; r2[0] = x;
                        r1[1] = BoundingBox[2]; r2[1] = BoundingBox[3];
                        r1[2] = y; r2[2] = y;
                        break;
                    case EPlaneType.YZ:
                        r1[0] = BoundingBox[0]; r2[0] = BoundingBox[1];
                        r1[1] = x; r2[1] = x;
                        r1[2] = y; r2[2] = y;
                        break;
                }

                foreach (var item in model.viewportMouseMoveReactionsGet(page))
                    if (item.MouseMove(x, y, r1, r2, mPerPixel, ctrlPressed, shiftPressed, LMBDown, RMBDown, axisType))
                    {
                        InvalidateEvent();
                        return;
                    }

                foreach (var item in model.viewportMouseMoveReactionsGet(PageType.None))
                    if (item.MouseMove(x, y, r1, r2, mPerPixel, ctrlPressed, shiftPressed, LMBDown, RMBDown, axisType))
                    {
                        InvalidateEvent();
                        return;
                    }
            }

            if (LMBDown)
                mouseMoved = true;

            if (selectionStarted && !selectionFinished)
            {
                ContinueSelection(x, y);
                InvalidateEvent();
                return;
            }


            base.OnMouseMove(e);
            InvalidateEvent();
        }

        public void ChangeBoundingBox(double[] newBoundingBox)
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
            for (int i = 0; i < 6; i++) BoundingBox[i] = newBoundingBox[i];
        }

        public void ChangeOrtho(double[] globalOrthoBox, bool direct = false)
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };

            if (direct)
            {
                newOrtho[0] = globalOrthoBox[0];
                newOrtho[1] = globalOrthoBox[1];
                newOrtho[2] = globalOrthoBox[2];
                newOrtho[3] = globalOrthoBox[3];
                newOrtho[4] = -zRange;
                newOrtho[5] = zRange;

                Ortho.SetOrtho(newOrtho);
                Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
                view.ResizeWindow();
                return;
            }
            switch (axisType)
            {
                case EPlaneType.XY:
                    newOrtho[0] = globalOrthoBox[0];
                    newOrtho[1] = globalOrthoBox[1];
                    newOrtho[2] = globalOrthoBox[2];
                    newOrtho[3] = globalOrthoBox[3];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
                case EPlaneType.XZ:
                    newOrtho[0] = globalOrthoBox[0];
                    newOrtho[1] = globalOrthoBox[1];
                    newOrtho[2] = globalOrthoBox[4];
                    newOrtho[3] = globalOrthoBox[5];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
                case EPlaneType.YZ:
                    newOrtho[0] = globalOrthoBox[2];
                    newOrtho[1] = globalOrthoBox[3];
                    newOrtho[2] = globalOrthoBox[4];
                    newOrtho[3] = globalOrthoBox[5];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
            }

            Ortho.SetOrtho(newOrtho);
            Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
            view?.ResizeWindow();
        }
        
        public void setRotateAndNameAxes(EPlaneType numAxis)
        {
            axisType = numAxis;
            switch (axisType)
            {
                case EPlaneType.XY: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Y"; break;
                case EPlaneType.XZ: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Z"; break;
                case EPlaneType.YZ: Ortho.HorAxisName = "Y"; Ortho.VertAxisName = "Z"; break;
            }
        }

        //protected override void Draw() => view?.Draw();
        //protected override void UpdateViewMatrix()
        //{
        //    view?.UpdateViewMatrix();
        //    //view.WidthLocal = view.WidthLocal;
        //    //view.HeightLocal = view.HeightLocal;
        //}
	}
}
