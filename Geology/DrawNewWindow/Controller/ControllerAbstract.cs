using Geology.DrawNewWindow.Model;
using Geology.DrawNewWindow.View;
using Geology.DrawWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geology.OpenGL;
using System.Drawing.Imaging;

namespace Geology.DrawNewWindow.Controller
{
	public class ControllerAbstract : IControllerWindow
	{
		public IViewWindow View { get { return view; } set { view = value; } }
		public PageType Page { get { return page; } set { page = value; } }
		public double[] BoundingBox { get; set; }
		public FontGeology caption { get; set; }

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
					//this.ResizeView();
					this.View.UpdateViewMatrix();
					this.View.Draw();
				}
			}
		}

		protected IModelWindow model;
		protected IViewWindow view;
		protected PageType page = PageType.Model;

		public virtual void OnMouseDown(MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public virtual void OnMouseWheel(MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		protected ImageCodecInfo GetEncoder(ImageFormat format)
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

		protected void mnuSaveBitmap_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			
			saveFileDialog.Filter = "Png-files (*.png)|*.png";
			saveFileDialog.FilterIndex = 0;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				System.Drawing.Bitmap b = new System.Drawing.Bitmap(view.Width, view.Height);
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

		protected void DisposedController(object sender, EventArgs e)
		{
			Win32.wglMakeCurrent(View.Hdc, (IntPtr)View.OglContext);
			caption.ClearFont();
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}
	}
}
