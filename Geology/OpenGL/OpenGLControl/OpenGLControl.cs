﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Geology.DrawNewWindow.Controller;
using Geology.DrawNewWindow.View;
using Geology.DrawWindow;

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
        public IControllerWindow Controller;
        System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap;

        public ConstructorType constructorType;
        //private delegate void InvalidateDekegate();
        //Cursor Cursor;
        
        public OpenGLControl()
        {
            InitializeComponent();
			//Cursor = new Cursor(Handle);
			mnuSaveBitmap.Click += mnuSaveBitmap_Click;
            Controller.InvalidateEvent += Invalidate;
			this.Disposed += Controller.DisposedController;
            this.ContextMenuStrip = Controller.mnu;
        }

		public void SetCostructor(ConstructorType constructorType)
		{
            this.constructorType = constructorType;
            mnuSaveBitmap = new ToolStripMenuItem("Save as JPG");

            switch (constructorType)
			{
				case ConstructorType.ThreeDimensional:
                    {
                        Controller = new ControllerWindow3D(Width, Height, Handle, mnuSaveBitmap);                        
                        break; 
                    }

				case ConstructorType.TwoDimensional:
					{
                        Controller = new ControllerWindow3DDraw2D(Width, Height, Handle);
                        break;
                    }
				case ConstructorType.Curve:
					{
                        Controller = new ControllerCurve(Width, Height, Handle, mnuSaveBitmap);
                        break;
                    }
					
				default:
					break;
			}

            View = Controller.View;
            mnuSaveBitmap.Click += mnuSaveBitmap_Click;

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

        protected void mnuSaveBitmap_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Png-files (*.png)|*.png";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(View.Width, View.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
                System.Drawing.Point loc = PointToScreen(new System.Drawing.Point(0, 0));
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
