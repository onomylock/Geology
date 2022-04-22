using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geology.DrawNewWindow.View;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.Controller
{
	public abstract class VirtualController
	{
		public abstract IViewWindow View { get; set; }
		public abstract PageType Page{ get; set;}
		public abstract void SetBoundingBox();
	}
}
