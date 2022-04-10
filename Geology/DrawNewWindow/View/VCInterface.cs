using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geology.DrawNewWindow.View
{
	public interface IViewController
	{
		//void OnMouseLeave(EventArgs e);
		//void OnMouseEnter(EventArgs e);
		Geology.MainWindow window { get; set; }
		int XPrevious { get; set; }
		int YPrevious { get; set; }
		System.Windows.Forms.ContextMenuStrip mnu { get; set; }
		FontGeology fontReceivers { get; set; }
		FontGeology paletteFont { get; set; }
		void ChengeBoundingBox();
		void getBoundingBox();
		void ChengeOrtho();
		void mnuSaveBitmap_Click(object sender, EventArgs e);
		System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format);
	}

	public interface IViewDraw
	{
		void InitializeComponent();
	}
}
