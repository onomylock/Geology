using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Geology.DrawNewWindow.Controller;
using Geology.DrawNewWindow.View;

namespace Geology.OpenGL.OpenGLControl
{
	public enum ConstructorType
	{
        ThreeDimensional, 
        TwoDimensional, 
        Curve 
	}

    public partial class OpenGLControl : UserControl
    {
        IViewWindow View;
        IControllerWindow Controller;

        public ConstructorType constructorType;
        
        public OpenGLControl()
        {
            InitializeComponent();

			this.Disposed += OpenGLControl_Disposed;
        }

		private void OpenGLControl_Disposed(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void SetCostructor(ConstructorType constructorType)
		{
            this.constructorType = constructorType;
			switch (constructorType)
			{
				case ConstructorType.ThreeDimensional:
                    {
                        Controller = new ControllerWindow3D(Width, Height, Handle);                        
                        break; 
                    }
				case ConstructorType.TwoDimensional:
					{
                        Controller = new ControllerWindow3DDraw2D(Width, Height, Handle);
                        break;
                    }
				case ConstructorType.Curve:
					{
                        Controller = new ControllerCurve(Width, Height, Handle);
                        break;
                    }
					
				default:
					break;
			}

            View = Controller.View;
        }

        private void OpenGLControl_Paint(object sender, PaintEventArgs e)
        {
            if (View.OglContext != 0)
			{
                View.Paint(sender, e);
			}
            //if (oglcontext != 0)
            //{
            //    Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            //    OpenGL.glClearColor(1, 1, 1, 1);
            //    OpenGL.glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //    Draw();

            //    Win32.SwapBuffers(hdc);
            //    Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            //}
        }

        private void OpenGLControl_Load(object sender, EventArgs e)
        {
          
        }

		public virtual void OpenGLControl_Resize(object sender, EventArgs e)
		{
			Resize_Window();
		}

		public void Resize_Window()
        {
            //Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            //UpdateViewMatrix();
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            View.ResizeWindow();
        }

        protected void OpenGLControl_Prepare()
        {
            View.Prepare();
            //Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            //UpdateViewMatrix();
        }

        protected void OpenGLControl_Release()
        {
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            View.Release();
        }

		protected override void OnMouseDown(MouseEventArgs e)
		{
            if (!Focused) Focus();
            Controller.OnMouseDown(e);
		}
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused) Focus();
            //Controller.OpenGLControl_Prepare() => this.OpenGLControl_Prepare();
            Controller.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) => Controller.OnMouseWheel(e);
        protected override void OnMouseMove(MouseEventArgs e) => Controller.OnMouseMove(e);
        protected virtual void Draw() => View?.Draw();
        protected virtual void UpdateViewMatrix() => View?.UpdateViewMatrix();
    }
}
