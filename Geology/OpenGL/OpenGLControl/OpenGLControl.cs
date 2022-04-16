using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geology.OpenGL.OpenGLControl
{
    public partial class OpenGLControl : UserControl
    {
        protected int oglcontext = 0;
        protected IntPtr hdc;
        public double[] BoundingBox;
        public int OglContex { get { return oglcontext; } }
        public IntPtr Hdc { get { return hdc; } }
        protected override void OnPaintBackground(PaintEventArgs pevent) { }
   
        public OpenGLControl()
        {
            BoundingBox = new double[]{-1,1,-1,1,-1,1};
            InitializeComponent();
          
            oglcontext = OpenGL.InitOpenGL((int)Handle);
            hdc = Win32.GetDC(Handle);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            OpenGL.glClearColor(1, 1, 1, 1);

            OpenGL.glEnable(OpenGL.GL_DEPTH_TEST);
          //  OpenGL.glHint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
         //   OpenGL.glDepthFunc(OpenGL.GL_LEQUAL);    // Set the type of depth-test
         //   OpenGL.glShadeModel(OpenGL.GL_POLYGON_SMOOTH);   // Enable smooth shading
        //    OpenGL.glHint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);  // Nice perspective corrections
       //     OpenGL.glEnable(OpenGL.GL_POLYGON_SMOOTH);
         //   OpenGL.glEnable(OpenGL.GL_LINE_SMOOTH);
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
         
        }

     /*   private void OpenGLControl_Disposed(object sender, EventArgs e)
        {
          ;
        }*/

        private void OpenGLControl_Paint(object sender, PaintEventArgs e)
        {
            if (oglcontext != 0)
            {
                Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
                OpenGL.glClearColor(1, 1, 1, 1);
                OpenGL.glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                Draw();

                Win32.SwapBuffers(hdc);
                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            }
        }
        private void OpenGLControl_Load(object sender, EventArgs e)
        {
          
        }
		private void OpenGLControl_Resize(object sender, EventArgs e)
		{
			Resize_Window();
		}
		public void Resize_Window()
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            UpdateViewMatrix();
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        protected void OpenGLControl_Prepare()
        {
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            UpdateViewMatrix();
        }
        protected void OpenGLControl_Release()
        {
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        protected virtual void Draw(){}
        protected virtual void UpdateViewMatrix() { }
    }
}
