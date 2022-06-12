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
		public ContextMenuStrip mnu { get; set; }
		


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
		protected ToolStripMenuItem mnuSaveBitmap;
		protected Cursor Cursor;

		

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

		protected void DisposedController(object sender, EventArgs e)
		{
			Win32.wglMakeCurrent(View.Hdc, (IntPtr)View.OglContext);
			caption.ClearFont();
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}
	}
}
