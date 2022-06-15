using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Geology.DrawNewWindow.Model;
using Geology.DrawNewWindow.View;
using Geology.Projection;
using System.Windows.Forms;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.Controller
{
	public interface IControllerWindow
	{
		double[] BoundingBox { get; set; }
		IViewWindow View { get; set; }
		PageType Page { get; set; }
		IModelWindow Model { get; set; }
		ContextMenuStrip mnu { get; set; }
		void OnMouseDown(MouseEventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseWheel(MouseEventArgs e);
		event Action InvalidateEvent;
		void DisposedController(object sender, EventArgs e);
		//delegate InvalidateEventArgs
		//event InvalidateEventArgs e;
		//delegate void InvalidateMethod();
		//event InvalidateEventArgs();
		//void Resize_Window();

		//FontGeology caption { get; set; }
		//CaptionAxisHorAndVert CaptionHorAndVert { get; set; }
		//COrthoControlProport Ortho { get; set; }
		//CPerspective project { get; set; }
	}

	public interface IFactoryController
	{
		string Name { get; }
		IControllerWindow CreateController(int Width, int Height, IntPtr Handle, System.Windows.Forms.ToolStripMenuItem mnuSaveBitmap);
	}
}
 