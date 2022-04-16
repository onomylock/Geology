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
    public class ControllerWindow : OpenGLControl
    {
        private int XPrevious = 0, YPrevious = 0;
        //private newView3D drawView3D;
        private bool selecting = false;
        private MainWindow window;
        public CPerspective project;
        private TypeTransformation typeTransform;
        public PageType page;
        private bool mouseDown = false;
        public FontGeology caption;
        public FontGeology fontReceivers;
        public FontGeology paletteFont;
        public IModel model = new ModelWindow();
        private IViewWindow view;

        private ContextMenuStrip mnu;
        private ToolStripMenuItem mnuAlongWindow;
        private ToolStripMenuItem mnuAroundX;
        private ToolStripMenuItem mnuAroundY;
        private ToolStripMenuItem mnuAroundZ;
        private ToolStripMenuItem mnuNone;
        private ToolStripMenuItem mnuSaveBitmap;
        private ToolStripMenuItem mnuStartView;
        private ToolStripMenuItem mnuSelect;

        public ControllerWindow() : base()
        {
            view = new ViewWindow3D();
            window = null;

            model.Objects.Add(new CGeoObject());
            model.GlobalBoundingBox[0] = -10000;

            model.GlobalBoundingBox[1] = 10000;
            model.GlobalBoundingBox[2] = -10000;
            model.GlobalBoundingBox[3] = 10000;
            model.GlobalBoundingBox[4] = -10000;
            model.GlobalBoundingBox[5] = 10000;
            project = new CPerspective();

			mnu = new ContextMenuStrip();
			mnuAlongWindow = new ToolStripMenuItem("Along Window");
			mnuAroundX = new ToolStripMenuItem("Around X");
			mnuAroundY = new ToolStripMenuItem("Around Y");
			mnuAroundZ = new ToolStripMenuItem("Around Z");
			mnuNone = new ToolStripMenuItem("None");
			mnuSaveBitmap = new ToolStripMenuItem("Save as JPG");
			mnuStartView = new ToolStripMenuItem("Start view");
			mnuSelect = new ToolStripMenuItem("Select");

			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            caption = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            this.Disposed += CView3D_Disposed;

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

        public void SetMainRef(MainWindow _window)
        {
            window = _window;
        }

        public new void Update()
        {
            view.UpdateViewMatrix();
            view.Draw();
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

		void mnuStartView_Click(object sender, EventArgs e)
		{
			project.ClearView();
			Array.Copy(model.GlobalBoundingBox, BoundingBox, BoundingBox.Length);
            Resize_Window();
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

        //protected override void Draw() => _view.Draw();

        //protected override void UpdateViewMatrix() => _view.UpdateViewMatrix();

        //protected override void Draw()
        //{
        //	double scale = 1.0;
        //	if (page == PageType.ViewModel)
        //		GLContex.glEnable(GLContex.GL_LIGHT_MODEL_TWO_SIDE);
        //	GLContex.glEnable(GLContex.GL_NORMALIZE);
        //	/*
        //    float[] light_ambient = { 0.0f, 0.0f, 0.0f, 1.0f };
        //    float[] light_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        //    float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        //    CCamera myCam = project.GetCamera;
        //    double xVector = myCam.m_vPosition.X - myCam.m_vView.X;
        //    double yVector = myCam.m_vPosition.Y - myCam.m_vView.Y;
        //    double zVector = myCam.m_vPosition.Z - myCam.m_vView.Z;
        //    xVector = myCam.m_vPosition.X + xVector * 3;
        //    yVector = myCam.m_vPosition.Y + yVector * 3;
        //    zVector = myCam.m_vPosition.Z + zVector * 3;
        //    float[] position = { (float)xVector, (float)yVector, (float)zVector };

        //    GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_AMBIENT, light_ambient);
        //    GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_DIFFUSE, light_diffuse);
        //    GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_SPECULAR, light_specular);
        //    GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_POSITION, position);
        //    float[] color = { 0.5f, 0.5f, 0.5f, 0.5f }; // красный цвет
        //    float[] shininess = { 100 };
        //    GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_DIFFUSE, color); // цвет чайника
        //    GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SPECULAR, color); // отраженный свет
        //    GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SHININESS, shininess); // степень отраженного света
        //    GLContex.glLightModelf(GLContex.GL_LIGHT_MODEL_TWO_SIDE, GLContex.GL_TRUE);
        //    */
        //	//GLContex.glEnable(GLContex.GL_LIGHT0);
        //	//GLContex.glEnable(GLContex.GL_LIGHTING);


        //	var lightSettings = DrawWindow.GlobalDrawingSettings.LightSettings;

        //	if (lightSettings.Enabled)
        //	{
        //		CCamera myCam = project.GetCamera;
        //		double xVector = myCam.m_vPosition.X - myCam.m_vView.X;
        //		double yVector = myCam.m_vPosition.Y - myCam.m_vView.Y;
        //		double zVector = myCam.m_vPosition.Z - myCam.m_vView.Z;
        //		xVector = myCam.m_vPosition.X;// + xVector * 3;
        //		yVector = myCam.m_vPosition.Y;// + yVector * 3;
        //		zVector = myCam.m_vPosition.Z;// + zVector * 3;
        //									  //float[] position = { (float)xVector, (float)yVector, (float)zVector, 1f };
        //		var position = new float[] { lightSettings.X, lightSettings.Y, (float)(lightSettings.Z * scale), 1f };

        //		GLContex.glEnable(GLContex.GL_LIGHTING);
        //		GLContex.glEnable(GLContex.GL_LIGHT1);
        //		GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);
        //		GLContex.glEnable(GLContex.GL_NORMALIZE);

        //		float[] light_ambient = { lightSettings.Ambient, lightSettings.Ambient, lightSettings.Ambient, 1.0f };
        //		float[] light_diffuse = { lightSettings.Diffuse, lightSettings.Diffuse, lightSettings.Diffuse, 1.0f };
        //		float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        //		float[] mS = { lightSettings.Specular, lightSettings.Specular, lightSettings.Specular };


        //		GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_AMBIENT, light_ambient);
        //		GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_DIFFUSE, light_diffuse);
        //		GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_SPECULAR, light_specular);
        //		GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_POSITION, position);
        //		GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SPECULAR, mS); // отраженный свет
        //		GLContex.glMaterialf(GLContex.GL_FRONT, GLContex.GL_SHININESS, lightSettings.Shininess); // степень отраженного света

        //		GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_DIFFUSE, light_ambient); // цвет чайника
        //		GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SPECULAR, light_ambient); // отраженный свет
        //		GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SHININESS, light_ambient); // степень отраженного света
        //	}

        //	// Следующая строка позволяет закрашивать полигоны цветом при включенном освещении:
        //	GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);

        //	foreach (var p in _model.Objects)
        //		p.Draw(EPlaneType.XYZ, _model.DrawObjectsBounds, p.DrawColor);

        //	GLContex.glDisable(GLContex.GL_LIGHT0);
        //	GLContex.glDisable(GLContex.GL_LIGHT1);
        //	GLContex.glDisable(GLContex.GL_LIGHTING);
        //	GLContex.glDisable(GLContex.GL_NORMALIZE);

        //	GLContex.glDisable(GLContex.GL_COLOR_MATERIAL);

        //	draw3DAxis();
        //}

        //protected override void UpdateViewMatrix()
        //{
        //          try
        //          {
        //              GLContex.glMatrixMode(GLContex.GL_PROJECTION);
        //              GLContex.glLoadIdentity();
        //              double dMax, startAngle, dView;
        //              GLContex.glViewport(0, 0, Width, Height);
        //              project.PrepareDraw(out dMax, out startAngle, out dView, BoundingBox);

        //              GLContex.gluPerspective(startAngle, Width / (double)Height, dView, (dMax) * 30);
        //              GLContex.glMatrixMode(GLContex.GL_MODELVIEW);     // To operate on model-view matrix
        //              GLContex.glLoadIdentity();
        //              CCamera tmpCam = project.GetCamera;
        //              GLContex.gluLookAt(tmpCam.m_vPosition.X, tmpCam.m_vPosition.Y, tmpCam.m_vPosition.Z /* mesh3D.BoundingBox[0] - 0.1*/,
        //                  tmpCam.m_vView.X, tmpCam.m_vView.Y, tmpCam.m_vView.Z,
        //                  tmpCam.m_vUpVector.X, tmpCam.m_vUpVector.Y, tmpCam.m_vUpVector.Z);
        //          }
        //          catch (Exception ex)
        //          {

        //          }
        //      }
    }
}

