using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Geology.DrawNewWindow.Model;
using Geology.DrawNewWindow.View;
using Geology.Projection;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.Controller
{
	public interface IControllerWindow
	{
		void SetController(EPlaneType axisType, IModelWindow model);
		void SetBoundingBox(double[] newBoundingBox);
		IViewWindow View { get; set; }
		PageType Page { get; set; }
	}
}
