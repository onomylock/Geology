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
    public class ControllerWindow3D : OpenGLControl, IControllerWindow
    {
        //public IModelWindow Model
        //{
        //	get { return model; }
        //	set { model = value; }
        //}

        public IViewWindow View{ get { return view; } set { view = value; } }
        public PageType Page{ get { return page; } set { page = value; } }

		public IModelWindow Model 
        { 
            get { return model; } 
            set 
            {
                if (model == null)
                    model = new ModelWindow();
                else
                { 
                    model = value;
                    this.ResizeView();
                    this.UpdateViewMatrix();
                    this.Draw();
                } 
            } 
        }
        public CPerspective project { get; set; }
		public FontGeology caption { get; set; }
		public FontGeology fontReceivers { get; set; }
		public FontGeology paletteFont { get; set; }
		//public IViewWindow ViewWindow { get; set; }

		private int XPrevious = 0, YPrevious = 0;
        private bool selecting = false;
        private MainWindow window;
        private TypeTransformation typeTransform;
        private bool mouseDown = false;
        private IViewWindow view;
        private PageType page = PageType.Model;
        private IModelWindow model;

        private ContextMenuStrip mnu;
        private ToolStripMenuItem mnuAlongWindow;
        private ToolStripMenuItem mnuAroundX;
        private ToolStripMenuItem mnuAroundY;
        private ToolStripMenuItem mnuAroundZ;
        private ToolStripMenuItem mnuNone;
        private ToolStripMenuItem mnuSaveBitmap;
        private ToolStripMenuItem mnuStartView;
        private ToolStripMenuItem mnuSelect;

        public ControllerWindow3D() : base()
        {

            
        }

        public void SetController(EPlaneType axisType, IModelWindow model)
        {
            Model = model;
            //model.Objects.Add(new CGeoObject());
            window = null;

            // Может собираться и внешне
            //model.Objects.Add(new CGeoObject());
            //model.GlobalBoundingBox[0] = -10000;
            //model.GlobalBoundingBox[1] = 10000;
            //model.GlobalBoundingBox[2] = -10000;
            //model.GlobalBoundingBox[3] = 10000;
            //model.GlobalBoundingBox[4] = -10000;
            //model.GlobalBoundingBox[5] = 10000;
            project = new CPerspective();

            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            caption = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

            view = new ViewWindow3D(project, model, page, Width, Height, BoundingBox, caption);

            mnu = new ContextMenuStrip();
            mnuAlongWindow = new ToolStripMenuItem("Along Window");
            mnuAroundX = new ToolStripMenuItem("Around X");
            mnuAroundY = new ToolStripMenuItem("Around Y");
            mnuAroundZ = new ToolStripMenuItem("Around Z");
            mnuNone = new ToolStripMenuItem("None");
            mnuSaveBitmap = new ToolStripMenuItem("Save as JPG");
            mnuStartView = new ToolStripMenuItem("Start view");
            mnuSelect = new ToolStripMenuItem("Select");


            this.Disposed += CView3D_Disposed;
            this.Resize += Controller_Resize;

            mnuAlongWindow.CheckOnClick = true;
            mnuAlongWindow.Checked = true;
            typeTransform = TypeTransformation.alongWindow;
            mnuAlongWindow.Click += mnuAlongWindow_Click;
            mnuSaveBitmap.Click += mnuSaveBitmap_Click;
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
            ContextMenuStrip = mnu;
        }

        public void ResizeView()
		{
            view.Width = Width;
            view.Height = Height;
		}

        public void SetMainRef(MainWindow _window)
        {
            window = _window;
        }

        public void SetBoundingBox(double[] newBoundingBox)
		{
            Array.Copy(newBoundingBox, BoundingBox, newBoundingBox.Length);
		}

        private void Controller_Resize(object sender, EventArgs e)
        {
            this.ResizeView();
            Resize_Window();
            this.Draw();
        }

        private void CView3D_Disposed(object sender, EventArgs e)
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            caption.ClearFont();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        private void uncheckMenuItem(ToolStripItem itemClick)
        {
            foreach (ToolStripMenuItem item in mnu.Items)
            {
                item.Checked = false;
            }
            this.Cursor = Cursors.Default;
            selecting = false;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
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
                    ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                    ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
                    var encoder = System.Drawing.Imaging.Encoder.Quality;
                    //var encoder2 = System.Drawing.Imaging.Encoder.ColorDepth;
                    var myEncoderParameters = new EncoderParameters(1);
                    var myEncoderParameter = new EncoderParameter(encoder, 100L);
                    //var myEncoderParameter2 = new System.Drawing.Imaging.EncoderParameter(encoder2, 100L);

                    myEncoderParameters.Param[0] = myEncoderParameter;
                    //myEncoderParameters.Param[1] = myEncoderParameter2;

                    newb.Save(saveFileDialog.FileName, pngEncoder, myEncoderParameters);
                }
                //b.Save(saveFileDialog.FileName);
                g.Dispose();
            }
        }

		private void mnuStartView_Click(object sender, EventArgs e)
		{
			project.ClearView();
			Array.Copy(model.GlobalBoundingBox, BoundingBox, BoundingBox.Length);
            Resize_Window();
            //ResizeView();
            //view.Height = Height;
            //view.Width = Width;
            Invalidate();
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ViewControllerWindow3D
            // 
            this.Name = "ViewControllerWindow3D";
            this.ResumeLayout(false);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (!Focused) Focus();

            if (mouseDown)
            {
                switch (typeTransform)
                {
                    case TypeTransformation.alongWindow:
                    case TypeTransformation.alongX:
                    case TypeTransformation.alongY:
                    case TypeTransformation.alongZ:
                        project.SetTrans(Width, Height, -e.X + XPrevious, e.Y - YPrevious, typeTransform, BoundingBox); break;
                    case TypeTransformation.aroundX:
                    case TypeTransformation.aroundY:
                    case TypeTransformation.aroundZ:
                        project.setRotD(360.0 * (-e.X + XPrevious) / (float)Width, typeTransform); break;
                }

                XPrevious = e.X; YPrevious = e.Y;
                Resize_Window();
                //ResizeView();
                //view.Height = Height;
                //view.Width = Width;
                Invalidate();
            }
            //if (window != null)
            //    window.StatusBarView.Text = "View: 3D";
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (selecting && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                OpenGLControl_Prepare();
                double[] p1 = new double[3];
                double[] p2 = new double[3];
                _GetSceneCoord(e.X, Height - e.Y, 0, p1);
                _GetSceneCoord(e.X, Height - e.Y, 1, p2);
                bool selected = false;
                Utilities.Vector3 ip;
                foreach (var p in model.Objects)
                    if (p.LineIntersectsObject(p1, p2, out ip))
                    {
                        p.Selected = true;
                        selected = true;
                        break;
                    }
                OpenGLControl_Release();
                if (!selected)
                    return;
                Invalidate();
            }
            else
            {
                mouseDown = true;
                XPrevious = e.X; YPrevious = e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (selecting && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                foreach (var p in model.Objects)
                    if (p.Selected)
                    {
                        p.Selected = false;
                        Invalidate();
                        break;
                    }
            }
            mouseDown = false;
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
            project.Scale *= scal;

            Resize_Window();
            //ResizeView();
            //view.Height = Height;
            //view.Width = Width;
            Invalidate();
        }

        protected override void Draw() => view?.Draw();
		protected override void UpdateViewMatrix() => view?.UpdateViewMatrix();

		
	}
}

