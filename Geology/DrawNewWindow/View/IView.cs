using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geology.DrawNewWindow.View
{

	public interface IViewDraw
	{
		void InitializeComponent();
		System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format);
	}
}
