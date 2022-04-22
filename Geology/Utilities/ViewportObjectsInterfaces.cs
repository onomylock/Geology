using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geology.DrawWindow;
using static Geology.DrawWindow.CObject3DDraw2D;

namespace Geology.Utilities
{
    public delegate void ClickFunction(double x, double y, double[] ray1, double[] ray2, double mPerPixel, EPlaneType planeType, bool ctrlPressed, bool shiftPressed);
    public delegate void DrawFunction(EPlaneType planeType, double[] boundingBox, int widthLocal, int heightLocal, FontGeology labelFont, FontGeology paletteFont);
    public interface IViewportObjectsSelectable
    {
        void StartSelection(double x0, double y0, bool toAdd, EPlaneType planeType);
        void ContinueSelection(double x0, double y0, double x1, double y1, bool toAdd, EPlaneType planeType);
        void FinishSelection(double x0, double y0, double x1, double y1, bool toAdd, EPlaneType planeType);
    }
    public interface IViewportMouseMoveReaction
    {
        bool MouseMove(double x, double y, double[] ray1, double[] ray2, double mPerPixel, bool ctrlPressed, bool shiftPressed, bool LMBPressed, bool RMBPressed, EPlaneType planeType); // -1 - no reaction, no redraw, 0 - reaction, no redraw, 1 - reaction, redraw
    }
    public interface IViewportObjectsClickable
    {
        void Click(double x, double y, double[] ray1, double[] ray2, double mPerPixel, EPlaneType planeType, bool ctrlPressed, bool shiftPressed);
    }
    public interface IViewportObjectsContextmenuClickable
    {
        ContextMenuStrip ContextMenuClick(double x, double y, double mPerPixel, EPlaneType planeType, bool ctrlPressed, bool shiftPressed);
    }
    

    public interface IViewportObjectsDrawable
    {
        void Draw(EPlaneType planeType, double[] boundingBox, int widthLocal, int heightLocal, FontGeology labelFont, FontGeology paletteFont);
    }
    public interface IViewportCurveDrawable
    {
        void Draw(double[] ortho, int widthLocal, int heightLocal, int indentH, int indentV, bool logH, double logHZero, bool logV, double logVZero);
    }
    public interface IViewportObjectsDrawableAlternate
    {
        void DrawAlternate(EPlaneType planeType, double[] boundingBox, int widthLocal, int heightLocal, FontGeology labelFont, FontGeology paletteFont);
    }
}
