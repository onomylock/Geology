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
using Geology.DrawNewWindow.Model;
using Geology.DrawWindow;
using System.Windows;
using System.Reflection;

namespace Geology.OpenGL.OpenGLControl
{
	

    public partial class OpenGLControl : UserControl
    {
        //public readonly DependencyProperty dependencyProperty = 
        //    DependencyProperty.Register(nameof(constructorType), typeof(ConstructorType), typeof(OpenGLControl), new PropertyMetadata(default(ConstructorType)));
        public enum ConstructorType
        {
            TwoDimensional,
            ThreeDimensional,
            Curve
        }
        IViewWindow View;
        IFactory Factory;
        IControllerWindow Controller;
        //public IModelWindow Model { get; set; }
        System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap;

        public string nameConstructor { get; set; }
        public EPlaneType axisType { get; set; }
        public ConstructorType constructorType { get; set; }

        public OpenGLControl()
        {
			
        }

        private void OpenGLControl_Paint(object sender, PaintEventArgs e)
        {
            if (View.OglContext != 0)
			{
                View?.Paint(sender, e);
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

		public void OpenGLControl_Load(object sender, EventArgs e)
		{
			switch (constructorType)
			{
				case ConstructorType.ThreeDimensional:
					Factory = new Factory3D();
					break;
				case ConstructorType.TwoDimensional:
					Factory = new Factory3DDraw2D();
					break;
				case ConstructorType.Curve:
					Factory = new FactoryCurve();
					break;
				default:
					break;
			}
            
			mnuSaveBitmap = new ToolStripMenuItem("Save as JPG");
			//mnuSaveBitmap.Click += mnuSaveBitmap_Click;

			Factory.CreateControllerAndView(Width, Height, Handle, mnuSaveBitmap, axisType);

			Controller = Factory.Controller;
			View = Factory.View;
            Controller.View = View;

			InitializeComponent();
            this.ContextMenuStrip = Controller.mnu;

            mnuSaveBitmap.Click += mnuSaveBitmap_Click;
			Controller.InvalidateEvent += Invalidate;
			this.Disposed += View.DisposedView;
			this.Resize += OpenGLControl_Resize;
            mnuSaveBitmap.Click += mnuSaveBitmap_Click;
		}

		public virtual void OpenGLControl_Resize(object sender, EventArgs e)
		{
            View?.ResizeWindow();
            View.Height = Height;
            View.Width = Width;
        }

		//public void Resize_Window()
  //      {
  //          //Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
  //          //UpdateViewMatrix();
  //          //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            
  //      }

        protected void OpenGLControl_Prepare()
        {
            View?.Prepare();
            //Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            //UpdateViewMatrix();
        }

        protected void OpenGLControl_Release()
        {
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            View?.Release();
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
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Controller.OnMouseMove(e);
        }
        protected virtual void Draw() => View?.Draw();
        protected virtual void UpdateViewMatrix() => View?.UpdateViewMatrix();
    }
}
