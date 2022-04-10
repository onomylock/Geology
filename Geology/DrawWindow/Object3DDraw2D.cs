using Geology.Objects.GeoModel;
using Geology.Controls;
/*
 Файл содержит классы:
 * 
 * CObject3DDraw2D, потомок CView2D - используется для отображения
 * трехмерных объектов в проекциях
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GLContex = Geology.OpenGL.OpenGL;
using System.Windows.Input;
using Geology.Utilities;

namespace Geology.DrawWindow
{
    public class CObject3DDraw2D : CView2D
    {
        protected override void OnMouseLeave(EventArgs e)
        {
            //if (Focused) Parent.Focus();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //if (!Focused) Focus();
        }

        private double zRange;
        public double scaleV;
        public bool alwaysDrawObserving = false;
        bool toAdd = false;
        bool operationStackMultipleChecked = false;
        public Dictionary<PageType, List<IViewportObjectsSelectable> > selectableObjects = new Dictionary<PageType, List<IViewportObjectsSelectable> >();
        public Dictionary<PageType, List<IViewportObjectsDrawable> > drawableObjects = new Dictionary<PageType, List<IViewportObjectsDrawable>>();
        public Dictionary<PageType, List<IViewportObjectsClickable> > clickableObjects = new Dictionary<PageType, List<IViewportObjectsClickable>>();
        public Dictionary<PageType, List<IViewportObjectsContextmenuClickable> > contextMenuClickableObjects = new Dictionary<PageType, List<IViewportObjectsContextmenuClickable>>();
        public Dictionary<PageType, List<IViewportMouseMoveReaction>> mouseMoveReactionObjects = new Dictionary<PageType, List<IViewportMouseMoveReaction>>();
        

        public enum EPlaneType
        { XY = 0, XZ, YZ, XYZ }
        double prevX = 0, prevY = 0;
        double curX = 0, curY = 0;
        bool mouseMoved = false;
        bool LMBDown = false;
        bool RMBDown = false;
        double selectionX0 = 0, selectionX1 = 0;
        double selectionY0 = 0, selectionY1 = 0;
        bool selectionStarted = false;
        bool selectionFinished = true;

        public EPlaneType axisType;

        public CObject3DDraw2D() : base(true)
        {
            axisType = EPlaneType.XY;
            zRange = 1e+7;
            ChangeOrtho(new double[] { -1, 1, -1, 1, -1, 1 });

            foreach (var item in (PageType[])Enum.GetValues(typeof(PageType)))
            {
                selectableObjects.Add(item, new List<IViewportObjectsSelectable>());
                drawableObjects.Add(item, new List<IViewportObjectsDrawable>());
                clickableObjects.Add(item, new List<IViewportObjectsClickable>());
                contextMenuClickableObjects.Add(item, new List<IViewportObjectsContextmenuClickable>());
                mouseMoveReactionObjects.Add(item, new List<IViewportMouseMoveReaction>());
            }

            this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
        }

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Invalidate();
        }

        private void StartSelection(double x, double y)
        {
            selectionX1 = selectionX0 = x;
            selectionY1 = selectionY0 = y;
            selectionStarted = true;
            selectionFinished = false;

            toAdd = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl);

            Invalidate();
        }
        private void ContinueSelection(double x, double y)
        {
            selectionX1 = x;
            selectionY1 = y;

            Invalidate();
        }
        private void FinishSelection(double x, double y)
        {
            selectionX1 = x;
            selectionY1 = y;
            selectionStarted = false;
            selectionFinished = true;

            toAdd = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl);

            if (selectionX1 < selectionX0) Utilities.LittleTools.Swap(ref selectionX0, ref selectionX1);
            if (selectionY1 < selectionY0) Utilities.LittleTools.Swap(ref selectionY0, ref selectionY1);

            if (selectableObjects.ContainsKey(page))
            foreach (var obj in selectableObjects[page])
                obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);

            foreach (var obj in selectableObjects[PageType.None])
                obj.FinishSelection(selectionX0, selectionY0, selectionX1, selectionY1, toAdd, axisType);

            Invalidate();
        }

        private void ConvertScreenToWorldCoord(int screenX, int screenY, out double x, out double y)
        {
            Ortho.ConvertScreenToWorldCoord(WidthLocal, HeightLocal, screenX, screenY, out x, out y, captionHorAndVert.myfontVert.GetHeightText("0"), captionHorAndVert.myfontHor.GetHeightText("0"));
            double c;
            int indent;
            double[] orthV;
            Ortho.GetOrtho(out orthV);

            indent = captionHorAndVert.myfontVert.GetHeightText("0");
            c = (orthV[1] - orthV[0]) / (WidthLocal - indent);
            x = orthV[0] + (screenX - indent) * c;

            screenY = HeightLocal + indent - screenY;
            indent = captionHorAndVert.myfontHor.GetHeightText("0");
            c = (orthV[3] - orthV[2]) / (HeightLocal - indent);
            y = orthV[2] + (screenY - indent) * c;
        }
        private double MetersPerPixel()
        {
            double[] orthV;
            Ortho.GetOrtho(out orthV);
            int indent = captionHorAndVert.myfontVert.GetHeightText("0");
            return (orthV[1] - orthV[0]) / (WidthLocal - indent);
        }

        void SetContextMenu(double x, double y)
        {
            double mPerPixel = MetersPerPixel();
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            foreach (var obj in contextMenuClickableObjects[page])
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    this.ContextMenuStrip = m;
                    this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            foreach (var obj in contextMenuClickableObjects[PageType.None])
            {
                var m = obj.ContextMenuClick(x, y, mPerPixel, axisType, ctrlPressed, shiftPressed);
                if (m != null)
                {
                    this.ContextMenuStrip = m;
                    this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
                    return;
                }
            }

            this.ContextMenuStrip = mnu;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);

            if (!Focused) Focus();

            if (e.Button == MouseButtons.Right)
            {
                RMBDown = true;
                SetContextMenu(x, y);
                return;
            }

            LMBDown = true;
            mouseMoved = false;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                StartSelection(x, y);
                prevX = x;
                prevY = y;
                return;
            }

            base.OnMouseDown(e);
        }
        protected void GetRays(double x, double y, out double[] r1, out double[] r2)
        {
            r1 = new double[3];
            r2 = new double[3];
            switch (axisType)
            {
                case EPlaneType.XY:
                    r1[0] = x; r2[0] = x;
                    r1[1] = y; r2[1] = y;
                    r1[2] = BoundingBox[4]; r2[2] = BoundingBox[5];
                    break;
                case EPlaneType.XZ:
                    r1[0] = x; r2[0] = x;
                    r1[1] = BoundingBox[2]; r2[1] = BoundingBox[3];
                    r1[2] = y; r2[2] = y;
                    break;
                case EPlaneType.YZ:
                    r1[0] = BoundingBox[0]; r2[0] = BoundingBox[1];
                    r1[1] = x; r2[1] = x;
                    r1[2] = y; r2[2] = y;
                    break;
            }
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            LMBDown = false;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);
            if (!Focused) Focus();

            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            if (selectionStarted && ! selectionFinished)
            {
                FinishSelection(x, y);
                Invalidate();
                return;
            }

            if (!mouseMoved)
            {
                double[] r1, r2;
                GetRays(x, y, out r1, out r2);

                double mPerPixel = MetersPerPixel();
                bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) ||Keyboard.IsKeyDown(Key.RightCtrl);
                bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                foreach (var item in clickableObjects[page])
                    item.Click(x, y, r1, r2, mPerPixel, axisType, ctrlPressed, shiftPressed);
                foreach (var item in clickableObjects[PageType.None])
                    item.Click(x, y, r1, r2, mPerPixel, axisType, ctrlPressed, shiftPressed);
            }


            base.OnMouseUp(e);
            Invalidate();
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            double x, y;
            ConvertScreenToWorldCoord(e.X, e.Y, out x, out y);

            double mPerPixel = MetersPerPixel();
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!selectionStarted)
            {

                double[] r1 = new double[3];
                double[] r2 = new double[3];
                switch (axisType)
                {
                    case EPlaneType.XY:
                        r1[0] = x; r2[0] = x;
                        r1[1] = y; r2[1] = y;
                        r1[2] = BoundingBox[4]; r2[2] = BoundingBox[5];
                        break;
                    case EPlaneType.XZ:
                        r1[0] = x; r2[0] = x;
                        r1[1] = BoundingBox[2]; r2[1] = BoundingBox[3];
                        r1[2] = y; r2[2] = y;
                        break;
                    case EPlaneType.YZ:
                        r1[0] = BoundingBox[0]; r2[0] = BoundingBox[1];
                        r1[1] = x; r2[1] = x;
                        r1[2] = y; r2[2] = y;
                        break;
                }

                foreach (var item in mouseMoveReactionObjects[page])
                    if (item.MouseMove(x, y, r1, r2, mPerPixel, ctrlPressed, shiftPressed, LMBDown, RMBDown, axisType))
                    {
                        Invalidate();
                        return;
                    }

                foreach (var item in mouseMoveReactionObjects[PageType.None])
                    if (item.MouseMove(x, y, r1, r2, mPerPixel, ctrlPressed, shiftPressed, LMBDown, RMBDown, axisType))
                    {
                        Invalidate();
                        return;
                    }
            }

            if (LMBDown)
                mouseMoved = true;

            if (selectionStarted && !selectionFinished)
            {
                ContinueSelection(x, y);
                Invalidate();
                return;
            }


            base.OnMouseMove(e);
            Invalidate();
        }

        public void ChangeBoundingBox(double[] newBoundingBox)
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
            for (int i = 0; i < 6; i++) BoundingBox[i] = newBoundingBox[i];
        }

        public void ChangeOrtho(double[] globalOrthoBox, bool direct = false)
        {
            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };

            if (direct)
            {
                newOrtho[0] = globalOrthoBox[0];
                newOrtho[1] = globalOrthoBox[1];
                newOrtho[2] = globalOrthoBox[2];
                newOrtho[3] = globalOrthoBox[3];
                newOrtho[4] = -zRange;
                newOrtho[5] = zRange;

                Ortho.SetOrtho(newOrtho);
                Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
                Resize_Window();
                return;
            }
            switch (axisType)
            {
                case EPlaneType.XY:
                    newOrtho[0] = globalOrthoBox[0];
                    newOrtho[1] = globalOrthoBox[1];
                    newOrtho[2] = globalOrthoBox[2];
                    newOrtho[3] = globalOrthoBox[3];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
                case EPlaneType.XZ:
                    newOrtho[0] = globalOrthoBox[0];
                    newOrtho[1] = globalOrthoBox[1];
                    newOrtho[2] = globalOrthoBox[4];
                    newOrtho[3] = globalOrthoBox[5];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
                case EPlaneType.YZ:
                    newOrtho[0] = globalOrthoBox[2];
                    newOrtho[1] = globalOrthoBox[3];
                    newOrtho[2] = globalOrthoBox[4];
                    newOrtho[3] = globalOrthoBox[5];
                    newOrtho[4] = -zRange;
                    newOrtho[5] = zRange;
                    break;
            }

            Ortho.SetOrtho(newOrtho);
            Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
            Resize_Window();
        }
        public void setRotateAndNameAxes(EPlaneType numAxis)
        {
            axisType = numAxis;
            switch (axisType)
            {
                case EPlaneType.XY: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Y"; break;
                case EPlaneType.XZ: Ortho.HorAxisName = "X"; Ortho.VertAxisName = "Z"; break;
                case EPlaneType.YZ: Ortho.HorAxisName = "Y"; Ortho.VertAxisName = "Z"; break;
            }
        }
        protected override void Draw()
        {
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();
            if (axisType == EPlaneType.XZ || axisType == EPlaneType.YZ)
                scaleV = GlobalDrawingSettings.ScaleZ;
            else
                scaleV = 1.0;
            captionHorAndVert.GenerateGrid(WidthLocal, HeightLocal, scaleV);
            captionHorAndVert.DrawScaleLbls(WidthLocal, HeightLocal, scaleV);
            DrawObjetcs();
        }

        protected void DrawSelection()
        {
            if (selectionStarted && !selectionFinished)
            {
                GLContex.glLineWidth(3);
                GLContex.glColor3f(0.5f, 0.5f, 0.5f);
                GLContex.glBegin(GLContex.GL_LINE_LOOP);
                switch (axisType)
                {
                    case EPlaneType.XY:
                        GLContex.glVertex3d(selectionX0, selectionY0, zRange);
                        GLContex.glVertex3d(selectionX1, selectionY0, zRange);
                        GLContex.glVertex3d(selectionX1, selectionY1, zRange);
                        GLContex.glVertex3d(selectionX0, selectionY1, zRange);
                        break;
                    case EPlaneType.XZ:
                        GLContex.glVertex3d(selectionX0, zRange, selectionY0);
                        GLContex.glVertex3d(selectionX1, zRange, selectionY0);
                        GLContex.glVertex3d(selectionX1, zRange, selectionY1);
                        GLContex.glVertex3d(selectionX0, zRange, selectionY1);
                        break;
                    case EPlaneType.YZ:
                        GLContex.glVertex3d(zRange, selectionX0, selectionY0);
                        GLContex.glVertex3d(zRange, selectionX1, selectionY0);
                        GLContex.glVertex3d(zRange, selectionX1, selectionY1);
                        GLContex.glVertex3d(zRange, selectionX0, selectionY1);
                        break;
                }
                GLContex.glEnd();
            }
        }
        protected override void DrawObjetcs()
        {
            switch (axisType)
            {
                case EPlaneType.XZ:
                    GLContex.gluLookAt(0.0, -1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);
                    break;
                case EPlaneType.YZ:
                    GLContex.gluLookAt(1.0, 0.0, 0.0, -1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
                    break;
            }

            foreach (var item in drawableObjects[PageType.None])
                item.Draw(axisType, BoundingBox, WidthLocal, HeightLocal, fontReceivers, paletteFont);

            foreach (var item in drawableObjects[page])
                item.Draw(axisType, BoundingBox, WidthLocal, HeightLocal, fontReceivers, paletteFont);


            DrawSelection();
        }
    }
}
