using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Geology.OpenGL.OpenGLControl;
using System.Drawing.Imaging;
using Geology.Projection;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using Geology.Objects.GeoModel;
using Geology.Objects;
using Geology.DrawNewWindow.View;
using Geology.DrawNewWindow.Model;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.Controller
{
	public class Factory3D : IFactory
	{
		public string Name { get { return name; } }

		public IControllerWindow Controller { get; set; }

		public IViewWindow View { get; set; }

        public IModelWindow Model { get; set; }

		string name = "Controller3D";

		public void CreateControllerAndView(int Width, int Height, IntPtr Handle, ToolStripMenuItem mnuSaveBitmap, EPlaneType axisType)
		{
            Model = new ModelWindow();
            Model.Objects.Add(new CGeoObject());
            Model.viewportObjectsDrawablesSet(PageType.Model, Model.Objects.ToList());
            Controller = new ControllerWindow3D(Model, Handle, mnuSaveBitmap);
            View = new ViewWindow3D(Controller.project, Model.viewportObjectsDrawablesGet(Controller.Page), Controller.BoundingBox, Controller.Page, Width, Height, Handle);
		}
	}

	public class ControllerWindow3D : IControllerWindow
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
        public ContextMenuStrip mnu { get; set; }
        public CPerspective project { get; set; }
		public COrthoControlProport Ortho { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


		protected IModelWindow model;
        protected IViewWindow view;
        protected PageType page = PageType.Model;
        protected ToolStripMenuItem mnuSaveBitmap;
        protected Cursor Cursor;


        private int XPrevious = 0, YPrevious = 0;
        private bool selecting = false;
        private MainWindow window;
        private TypeTransformation typeTransform;
        private bool mouseDown = false;
 
        
        private ToolStripMenuItem mnuAlongWindow;
        private ToolStripMenuItem mnuAroundX;
        private ToolStripMenuItem mnuAroundY;
        private ToolStripMenuItem mnuAroundZ;
        private ToolStripMenuItem mnuNone;
        
        private ToolStripMenuItem mnuStartView;
        private ToolStripMenuItem mnuSelect;

        public ControllerWindow3D(IModelWindow Model, IntPtr Handle, System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap)
        {
            this.Model = Model;
            project = new CPerspective();
            Cursor = new Cursor(Handle);
            BoundingBox = Model.GlobalBoundingBox;

            mnu = new ContextMenuStrip();
			mnuAlongWindow = new ToolStripMenuItem("Along Window");
			mnuAroundX = new ToolStripMenuItem("Around X");
			mnuAroundY = new ToolStripMenuItem("Around Y");
			mnuAroundZ = new ToolStripMenuItem("Around Z");
			mnuNone = new ToolStripMenuItem("None");
			mnuStartView = new ToolStripMenuItem("Start view");
			mnuSelect = new ToolStripMenuItem("Select");
            this.mnuSaveBitmap = mnuSaveBitmap; 

            mnuAlongWindow.CheckOnClick = true;
            mnuAlongWindow.Checked = true;
            typeTransform = TypeTransformation.alongWindow;
            mnuAlongWindow.Click += mnuAlongWindow_Click;
            mnuAroundX.CheckOnClick = true;
            mnuAroundX.Click += mnuAroundX_Click;
            mnuAroundY.CheckOnClick = true;
            mnuAroundY.Click += mnuAroundY_Click;
            mnuAroundZ.CheckOnClick = true;
            mnuAroundZ.Click += mnuAroundZ_Click;
            mnuNone.CheckOnClick = true;
            mnuNone.Click += mnuNone_Click;
            mnuStartView.Click += mnuStartView_Click;
            mnuSelect.CheckOnClick = true;
            mnuSelect.Click += mnuSelect_Click;
            

            mnu.Items.AddRange(new ToolStripItem[] { mnuAlongWindow,/* mnuAlongX, mnuAlongY, mnuAlongZ,*/ 
                mnuAroundX, mnuAroundY, mnuAroundZ, mnuNone, 
                mnuSaveBitmap, mnuStartView, mnuSelect });
        }

        public event InvalidateDelegate InvalidateEvent;

		public void SetMainRef(MainWindow _window)
        {
            window = _window;
        }

		private void uncheckMenuItem(ToolStripItem itemClick)
        {
            foreach (ToolStripMenuItem item in mnu.Items)
            {
                item.Checked = false;
            }
            Cursor = Cursors.Default;
            selecting = false;
        }

		private void mnuStartView_Click(object sender, EventArgs e)
		{
			project.ClearView();
			Array.Copy(model.GlobalBoundingBox, BoundingBox, BoundingBox.Length);
            view.ResizeWindow();
            InvalidateEvent();
		}

		private void mnuAlongWindow_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.alongWindow;
        }

        private void mnuSelect_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            selecting = true;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void mnuAroundX_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundX;
        }

        private void mnuAroundY_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundY;
        }

        private void mnuAroundZ_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundZ;
        }

        private void mnuNone_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.None;
        }

        public void OnMouseMove(MouseEventArgs e)
        {         
            if (mouseDown)
            {
                switch (typeTransform)
                {
                    case TypeTransformation.alongWindow:
                    case TypeTransformation.alongX:
                    case TypeTransformation.alongY:
                    case TypeTransformation.alongZ:
                        project.SetTrans(view.Width, view.Height, -e.X + XPrevious, e.Y - YPrevious, typeTransform, BoundingBox); break;
                    case TypeTransformation.aroundX:
                    case TypeTransformation.aroundY:
                    case TypeTransformation.aroundZ:
                        project.setRotD(360.0 * (-e.X + XPrevious) / (float)view.Width, typeTransform); break;
                }

                XPrevious = e.X; YPrevious = e.Y;
                view.ResizeWindow();  
                InvalidateEvent();
            }          
        }

        private void _GetSceneCoord(int x, int y, double fZDepth, double[] p)
        {
            double[] dMatProj = new double[16];
            GLContex.glGetDoublev(GLContex.GL_PROJECTION_MATRIX, dMatProj);

            double[] dMatModelView = new double[16];
            GLContex.glGetDoublev(GLContex.GL_MODELVIEW_MATRIX, dMatModelView);

            int[] nMatViewport = new int[4];
            GLContex.glGetIntegerv(GLContex.GL_VIEWPORT, nMatViewport);

            GLContex.gluUnProject(x, y, fZDepth,
                dMatModelView,
                dMatProj,
                nMatViewport,
                ref p[0], ref p[1], ref p[2]);
        }

        public void OnMouseDown(MouseEventArgs e)
        {           
            if (selecting && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                view.Prepare();             

                double[] p1 = new double[3];
                double[] p2 = new double[3];
                _GetSceneCoord(e.X, view.Height - e.Y, 0, p1);
                _GetSceneCoord(e.X, view.Height - e.Y, 1, p2);
                bool selected = false;
                Utilities.Vector3 ip;
                foreach (var p in model.Objects)
                    if (p.LineIntersectsObject(p1, p2, out ip))
                    {
                        p.Selected = true;
                        selected = true;
                        break;
                    }
                view.Release();
                if (!selected)
                    return;
                InvalidateEvent();                
            }
            else
            {
                mouseDown = true;
                XPrevious = e.X; YPrevious = e.Y;
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {            
            if (selecting && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                foreach (var p in model.Objects)
                    if (p.Selected)
                    {
                        p.Selected = false;
                        InvalidateEvent();
                        break;
                    }
            }
            mouseDown = false;
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
            project.Scale *= scal;

            view.ResizeWindow();          
            InvalidateEvent();
        }
	}
}

