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
		void OnMouseDown(MouseEventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseWheel(MouseEventArgs e);
		//void Resize_Window();

		//FontGeology caption { get; set; }
		//CaptionAxisHorAndVert CaptionHorAndVert { get; set; }
		//COrthoControlProport Ortho { get; set; }
		//CPerspective project { get; set; }
	}
}
