/*
 Файл содержит классы:
 * 
 * CView3D, потомок OpenGL.OpenGLControl.OpenGLControl - область для отображения графических 
 * объектов в 3D (в перспективе). Объекты этого типа отслеживают события нажатия мыши,
 * обеспечивают работу со своим контекстным меню для изменения настроек отображения
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geology.Projection;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using System.Collections.ObjectModel;
using Geology.Objects.GeoModel;
namespace Geology.DrawWindow
{
    public class CView3D : OpenGL.OpenGLControl.OpenGLControl
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

        private Geology.MainWindow window;
        public ObservableCollection<Objects.CGeoLayer> layers;
        public ObservableCollection<Objects.CGeoObject> objects;
        CPerspective project;
        int XPrevious = 0, YPrevious = 0;
        bool mouseDown = false;
        System.Windows.Forms.ContextMenuStrip mnuStrip;
        TypeTransformation typeTransform;
        FontGeology caption;
        FontGeology fontReceivers;
        public FontGeology paletteFont;
        public double[] DrawBox;
        //public double[] BoundingBox;
        double coef = 10000000;
        bool selecting = false;
        //     Objects.Spline Spline;
        //   public CGridObjects buildingGrid;
        private Objects.GeoModel.GeoModel model;

        public CPerspective Project
        {
            get { return project; }
            set { project = value; }
        }
        public CView3D() : base()
        {
            window = null;
            DrawBox = new double[6] { -1, 1, -1, 1, -1, 1 };
            BoundingBox = new double[6] { -1, 1, -1, 1, -1, 1 };
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            caption = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            project = new CPerspective();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            this.Disposed += CView3D_Disposed;
            //this.MouseMove += CView3D_MouseMove;
            //this.MouseDown += CView3D_MouseDown;
            //this.MouseUp += CView3D_MouseUp;
            //this.MouseWheel += CView3D_MouseWheel;

            mnuStrip = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem mnuAlongWindow = new System.Windows.Forms.ToolStripMenuItem("Along Window");
            System.Windows.Forms.ToolStripMenuItem mnuAroundX = new System.Windows.Forms.ToolStripMenuItem("Around X");
            System.Windows.Forms.ToolStripMenuItem mnuAroundY = new System.Windows.Forms.ToolStripMenuItem("Around Y");
            System.Windows.Forms.ToolStripMenuItem mnuAroundZ = new System.Windows.Forms.ToolStripMenuItem("Around Z");
            System.Windows.Forms.ToolStripMenuItem mnuNone = new System.Windows.Forms.ToolStripMenuItem("None");
            System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap = new System.Windows.Forms.ToolStripMenuItem("Save as JPG");
            System.Windows.Forms.ToolStripMenuItem mnuStartView = new System.Windows.Forms.ToolStripMenuItem("Start view");
            System.Windows.Forms.ToolStripMenuItem mnuSelect = new System.Windows.Forms.ToolStripMenuItem("Select");
            layers = new ObservableCollection<Objects.CGeoLayer>();
            objects = new ObservableCollection<Objects.CGeoObject>();
            model = new Objects.GeoModel.GeoModel();
           
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

            mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuAlongWindow,/* mnuAlongX, mnuAlongY, mnuAlongZ,*/ mnuAroundX, mnuAroundY, mnuAroundZ, mnuNone,mnuSaveBitmap, mnuStartView, mnuSelect });
            this.ContextMenuStrip = mnuStrip;

        }
        public void SetMainRef(Geology.MainWindow _window)
        {
            window = _window;
        }
        public void SetObjects(ObservableCollection<Objects.CGeoLayer> _layers, ObservableCollection<Objects.CGeoObject> _objects, Objects.GeoModel.GeoModel model)
        {
            layers = _layers;
            objects = _objects;
            this.model = model;
        }
        public void SetSpline(Objects.Spline _spl)
        {
            //         Spline = _spl;
        }
        private void CView3D_Disposed(object sender, EventArgs e)
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            caption.ClearFont();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }
        void uncheckMenuItem(System.Windows.Forms.ToolStripItem itemClick)
        {
            foreach (System.Windows.Forms.ToolStripMenuItem item in mnuStrip.Items)
            {
                item.Checked = false;
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
            selecting = false;
        }
        public void ChangeBoundingBox(double[] newBoundingBox)
        {
            Array.Copy(newBoundingBox, BoundingBox, 6);
        }
        public void ChangeDrawBox(double[] newDrawBox)
        {
            Array.Copy(newDrawBox, DrawBox, 6);
            //Array.Copy(newDrawBox, BoundingBox, 6);
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

        void mnuAlongWindow_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.alongWindow;
        }
        void getBoundingBox(out double[] boundingBox)
        {
            double[] observingBoundingBox = new double[6];
            double[] modelBoundingBox = new double[6];

            boundingBox = new double[6];
            boundingBox[0] = Math.Min(observingBoundingBox[0], modelBoundingBox[0]) * 1.1;
            boundingBox[1] = Math.Max(observingBoundingBox[1], modelBoundingBox[1]) * 1.1;
            boundingBox[2] = Math.Min(observingBoundingBox[2], modelBoundingBox[2]) * 1.1;
            boundingBox[3] = Math.Max(observingBoundingBox[3], modelBoundingBox[3]) * 1.1;
            boundingBox[4] = Math.Min(observingBoundingBox[4], modelBoundingBox[4]) * 1.1;
            boundingBox[5] = Math.Max(observingBoundingBox[5], modelBoundingBox[5]) * 1.1;
        }
        public void UpdateBoundingBox()
        {
            getBoundingBox(out var bb);
            ChangeBoundingBox(bb);
        }
        void mnuStartView_Click(object sender, EventArgs e)
        {
            project.ClearView();
            UpdateBoundingBox();
            Resize_Window();
            Invalidate();
        }
        void mnuSelect_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            selecting = true;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
        }
        void mnuAroundX_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundX;
        }
        void mnuAroundY_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundY;
        }
        void mnuAroundZ_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.aroundZ;
        }
        void mnuNone_Click(object sender, EventArgs e)
        {
            uncheckMenuItem(sender as System.Windows.Forms.ToolStripMenuItem);
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
            typeTransform = TypeTransformation.None;
        }
        //private void CView3D_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
        //    project.Scale *= scal;

        //    Resize_Window();
        //    Invalidate();
        //}
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            double scal = e.Delta < 0 ? 1.05 : 1 / 1.05;
            project.Scale *= scal;

            Resize_Window();
            Invalidate();
        }

        void drawHexaderon()
        {



            GLContex.glBegin(GLContex.GL_QUADS);

            GLContex.glColor3f(0, 0, 0);
            GLContex.glVertex3d(-0.5 * coef / 3, -0.5 * coef / 3, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef / 3, -0.5 * coef / 3, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef / 3, 0.5 * coef / 3, -0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef / 3, 0.5 * coef / 3, -0.5 * coef);

            GLContex.glEnd();


            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1, 2);

            GLContex.glBegin(GLContex.GL_QUADS);

            GLContex.glColor3f(1, 0, 0);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, -0.5 * coef);

            GLContex.glColor3f(0, 1, 0);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, 0.5 * coef);


            GLContex.glColor3f(0, 0, 1);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, 0.5 * coef);


            GLContex.glColor3f(1, 1, 0);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, 0.5 * coef);

            GLContex.glColor3f(1, 0, 1);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, 0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(-0.5 * coef, -0.5 * coef, 0.5 * coef);


            GLContex.glColor3f(0, 1, 1);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, -0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, 0.5 * coef, 0.5 * coef);
            GLContex.glVertex3d(0.5 * coef, -0.5 * coef, 0.5 * coef);


            GLContex.glEnd();
            GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);

        }
        protected override void Draw()
        {


            double scale = 1.0;
            if (page == PageType.ViewModel)

            GLContex.glEnable(GLContex.GL_LIGHT_MODEL_TWO_SIDE);
            GLContex.glEnable(GLContex.GL_NORMALIZE);
            /*
            float[] light_ambient = { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            CCamera myCam = project.GetCamera;
            double xVector = myCam.m_vPosition.X - myCam.m_vView.X;
            double yVector = myCam.m_vPosition.Y - myCam.m_vView.Y;
            double zVector = myCam.m_vPosition.Z - myCam.m_vView.Z;
            xVector = myCam.m_vPosition.X + xVector * 3;
            yVector = myCam.m_vPosition.Y + yVector * 3;
            zVector = myCam.m_vPosition.Z + zVector * 3;
            float[] position = { (float)xVector, (float)yVector, (float)zVector };

            GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_AMBIENT, light_ambient);
            GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_DIFFUSE, light_diffuse);
            GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_SPECULAR, light_specular);
            GLContex.glLightfv(GLContex.GL_LIGHT0, GLContex.GL_POSITION, position);
            float[] color = { 0.5f, 0.5f, 0.5f, 0.5f }; // красный цвет
            float[] shininess = { 100 };
            GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_DIFFUSE, color); // цвет чайника
            GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SPECULAR, color); // отраженный свет
            GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SHININESS, shininess); // степень отраженного света
            GLContex.glLightModelf(GLContex.GL_LIGHT_MODEL_TWO_SIDE, GLContex.GL_TRUE);
            */
            //GLContex.glEnable(GLContex.GL_LIGHT0);
            //GLContex.glEnable(GLContex.GL_LIGHTING);


            var lightSettings = DrawWindow.GlobalDrawingSettings.LightSettings;

            if (lightSettings.Enabled)
            {
                CCamera myCam = project.GetCamera;
                double xVector = myCam.m_vPosition.X - myCam.m_vView.X;
                double yVector = myCam.m_vPosition.Y - myCam.m_vView.Y;
                double zVector = myCam.m_vPosition.Z - myCam.m_vView.Z;
                xVector = myCam.m_vPosition.X;// + xVector * 3;
                yVector = myCam.m_vPosition.Y;// + yVector * 3;
                zVector = myCam.m_vPosition.Z;// + zVector * 3;
                //float[] position = { (float)xVector, (float)yVector, (float)zVector, 1f };
                var position = new float[] { lightSettings.X, lightSettings.Y, (float)(lightSettings.Z * scale), 1f };

                GLContex.glEnable(GLContex.GL_LIGHTING);
                GLContex.glEnable(GLContex.GL_LIGHT1);
                GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);
                GLContex.glEnable(GLContex.GL_NORMALIZE);

                float[] light_ambient = { lightSettings.Ambient, lightSettings.Ambient, lightSettings.Ambient, 1.0f };
                float[] light_diffuse = { lightSettings.Diffuse, lightSettings.Diffuse, lightSettings.Diffuse, 1.0f };
                float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                float[] mS = { lightSettings.Specular, lightSettings.Specular, lightSettings.Specular };


                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_AMBIENT, light_ambient);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_DIFFUSE, light_diffuse);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_SPECULAR, light_specular);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_POSITION, position);
                GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SPECULAR, mS); // отраженный свет
                GLContex.glMaterialf(GLContex.GL_FRONT, GLContex.GL_SHININESS, lightSettings.Shininess); // степень отраженного света

                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_DIFFUSE, light_ambient); // цвет чайника
                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SPECULAR, light_ambient); // отраженный свет
                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SHININESS, light_ambient); // степень отраженного света
            }

            // Следующая строка позволяет закрашивать полигоны цветом при включенном освещении:
            GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);

                foreach (var p in objects)
                    p.Draw(CObject3DDraw2D.EPlaneType.XYZ, model.DrawObjectsBounds, p.DrawColor);

            GLContex.glDisable(GLContex.GL_LIGHT0);
            GLContex.glDisable(GLContex.GL_LIGHT1);
            GLContex.glDisable(GLContex.GL_LIGHTING);
            GLContex.glDisable(GLContex.GL_NORMALIZE);

            GLContex.glDisable(GLContex.GL_COLOR_MATERIAL);

            draw3DAxis();
        }

        void getVectorMult(double x1, double x2, double y1, double y2, double z1, double z2, double[] res)
        {
            res[0] = y1 * z2 - y2 * z1;
            res[1] = x2 * z1 - x1 * z2;
            res[2] = x1 * y2 - x2 * y1;
            double norm = Math.Sqrt(res[0] * res[0] + res[1] * res[1] + res[2] * res[2]);
            res[0] /= norm;
            res[1] /= norm;
            res[2] /= norm;
        }
        void draw3DAxis() //рисовка рамки с осями
        {
            {
                GLContex.glLineWidth(1);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glPushMatrix();
                GLContex.glLoadIdentity();
                GLContex.glMatrixMode(GLContex.GL_PROJECTION);

                GLContex.glPushMatrix();
                GLContex.glViewport(0, 0, 80, 80);
                GLContex.glLoadIdentity();
                GLContex.glClearColor(1, 1, 1, 1);
                GLContex.glOrtho(-20, 20, -20, 20, -20, 20);

                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glColor3f(0, 0, 1.0f);
                GLContex.glLoadIdentity();


                double[] mat;
                project.GetMatrixOrth(out mat);
                double[] multmat = new double[]{
                    mat[0], mat[3], mat[6], 0,
                    mat[1], mat[4], mat[7], 0,
                    mat[2], mat[5], mat[8], 0,
                    0, 0, 0, 1
                };

                GLContex.glMultMatrixd(multmat);
                GLContex.glBegin(GLContex.GL_LINES);
                GLContex.glColor3f(1, 0, 0);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(10, 0, 0);
                GLContex.glColor3f(0, 1, 0);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(0, 10, 0);
                GLContex.glColor3f(0, 0, 1);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(0, 0, 10);
                GLContex.glEnd();

                double sizeArrow = 1, sizeArrowAx = 7;
                //glEnable(GL_DEPTH_TEST);
                GLContex.glBegin(GLContex.GL_TRIANGLES);

                GLContex.glColor3f(1, 0, 0);
                GLContex.glVertex3f(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);

                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);

                GLContex.glColor3f(0, 1, 0);
                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);

                GLContex.glColor3f(0, 0, 1);
                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);


                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);


                GLContex.glEnd();
                GLContex.glColor3f(1, 0, 0);
                caption.PrintText(12, 0, 0, "X");
                GLContex.glColor3f(0, 1, 0);
                caption.PrintText(0, 12, 0, "Y");
                GLContex.glColor3f(0, 0, 1);
                caption.PrintText(0, 0, 12, "Z");

                GLContex.glMatrixMode(GLContex.GL_PROJECTION);
                GLContex.glPopMatrix();
                GLContex.glViewport(0, 0, Width, Height);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glPopMatrix();
            }
        }
        protected override void UpdateViewMatrix()
        {
            try
            {
                GLContex.glMatrixMode(GLContex.GL_PROJECTION);
                GLContex.glLoadIdentity();
                double dMax, startAngle, dView;
                GLContex.glViewport(0, 0, Width, Height);
                project.PrepareDraw(out dMax, out startAngle, out dView, BoundingBox);

                GLContex.gluPerspective(startAngle, Width / (double)Height, dView, (dMax) * 30);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);     // To operate on model-view matrix
                GLContex.glLoadIdentity();
                CCamera tmpCam = project.GetCamera;
                GLContex.gluLookAt(tmpCam.m_vPosition.X, tmpCam.m_vPosition.Y, tmpCam.m_vPosition.Z /* mesh3D.BoundingBox[0] - 0.1*/,
                    tmpCam.m_vView.X, tmpCam.m_vView.Y, tmpCam.m_vView.Z,
                    tmpCam.m_vUpVector.X, tmpCam.m_vUpVector.Y, tmpCam.m_vUpVector.Z);
            }
            catch (Exception ex)
            {

            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CView3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "CView3D";
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CView3D_MouseDown);
            //this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CView3D_MouseMove);
            this.ResumeLayout(false);

        }

        //private void CView3D_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (mouseDown)
        //    {
        //        switch (typeTransform)
        //        {
        //            case TypeTransformation.alongWindow:
        //            case TypeTransformation.alongX:
        //            case TypeTransformation.alongY:
        //            case TypeTransformation.alongZ:
        //                project.SetTrans(Width, Height, -e.X + XPrevious, e.Y - YPrevious, typeTransform, BoundingBox);break;
        //            case TypeTransformation.aroundX:
        //            case TypeTransformation.aroundY:
        //            case TypeTransformation.aroundZ:
        //                project.setRotD(360.0 * (-e.X + XPrevious) / (float)Width, typeTransform);break;
        //        }

        //        XPrevious = e.X; YPrevious = e.Y;
        //        Resize_Window();
        //        Invalidate();
        //    }
        //    if (window != null)
        //        window.StatusBarView.Text = "View: 3D";
        //}
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
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

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
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
                foreach (var p in objects)
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
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (selecting && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                foreach (var p in objects)
                    if (p.Selected)
                    {
                        p.Selected = false;
                        Invalidate();
                        break;
                    }
            }
            mouseDown = false;
        }

    }
}
