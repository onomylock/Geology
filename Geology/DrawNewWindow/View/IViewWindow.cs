using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.View
{

    public interface IViewWindow
    {
        // ContextMenuStrip mnu { get; set; }
        // ToolStripMenuItem mnuAlongWindow { get; set; }
        // ToolStripMenuItem mnuAroundX { get; set; }
        // ToolStripMenuItem mnuAroundY { get; set; }
        // ToolStripMenuItem mnuAroundZ { get; set; }
        // ToolStripMenuItem mnuNone { get; set; }
        // ToolStripMenuItem mnuSaveBitmap { get; set; }
        // ToolStripMenuItem mnuStartView { get; set; }
        // ToolStripMenuItem mnuSelect { get; set; }
        void UpdateViewMatrix();
        void Draw();
		int Width { get; set; }
        int Height { get; set; }
        int WidthLocal { get; set; }
        int HeightLocal { get; set; }
        //void ChengeSelection();
        //IViewChanged changed { get; set; }
        //FontGeology caption { get; set; }
        //FontGeology fontReceivers { get; set; }
        //FontGeology paletteFont { get; set; }
        ////void InitializeComponent();
        //System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format);
    }

    public interface IViewChanged
    {
        double SelectionX0 { get; set; }
        double SelectionX1 {get; set;}
        double SelectionY0 { get; set; }
        double SelectionY1 { get; set; }
    }
}
