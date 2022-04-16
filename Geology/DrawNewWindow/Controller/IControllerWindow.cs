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
		void ShowView();
		void Update();
		IModelWindow model { get; set; }
		IViewWindow ViewWindow { get; set; }
		CPerspective project { get; set; }
		PageType page { get; set; }
		FontGeology caption { get; set; }
		FontGeology fontReceivers { get; set; }
		FontGeology paletteFont { get; set; }
	}
}
