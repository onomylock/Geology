using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.DrawWindow
{
    public class CaptionAxisHorAndVert
    {
        bool doubleAxis = false;
        public FontGeology myfontHor;
        public FontGeology myfontVert;
        public FontGeology myfontHor_ind;
        public FontGeology myfontVert_ind;
        CaptionAxisHor captionHor;
        CaptionAxisHor captionHor2;
        CaptionAxisVert captionVert;
        IOrthoControl Ortho;

        bool for_model; // or for curves

        bool logH;
        bool logV;
        public double Step { get { return captionHor2.Step; } set { captionHor2.Step = value; } }
        public double StartFrom { get { return captionHor2.StartFrom; } set { captionHor2.StartFrom = value; } }
        public bool DoubleAxis { get { return doubleAxis; } set { doubleAxis = value; if (Ortho != null) Ortho.DoubleAxis = value; } }
        public bool LogH { get { return logH; } set { logH = value; captionHor.Log = logH; } }
        public bool LogV { get { return logV; } set { logV = value; captionVert.Log = logV; } }

        double zeroLogH;
        double zeroLogV;
        public double ZeroLogH { get { return zeroLogH; } set { zeroLogH = value; captionHor.LogZero = zeroLogH; } }
        public double ZeroLogV { get { return zeroLogV; } set { zeroLogV = value; captionVert.LogZero = zeroLogV; } }

        bool drawGrid;
        public bool DrawGridFlag { get { return drawGrid; } set { drawGrid = value; } }

        public int GetIndentHor { get { return myfontVert.GetHeightTemplStr; } }
        public int GetIndentVert { get { return myfontHor.GetHeightTemplStr; } }

        public CaptionAxisHorAndVert(IntPtr hdc, int oglcontext, string fontName, int size, IOrthoControl Ortho, int Width, int Height)
        {
            drawGrid = true;
            this.Ortho = Ortho;
            myfontHor = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, fontName, size);
            myfontVert = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, fontName, size - 2);

            myfontHor_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, fontName, size - 4);
            myfontVert_ind = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Vertical, fontName, size - 6);

            captionHor = new CaptionAxisHor(myfontHor, myfontHor_ind, Ortho);
            captionHor2 = new CaptionAxisHor(myfontHor, myfontHor_ind, Ortho);
            captionHor2.IsIntegerAxis = true;
            captionHor2.Step = 5.0;
            captionHor2.StartFrom = 1990.0;
            captionVert = new CaptionAxisVert(myfontVert, myfontVert_ind, Ortho);

            logH = false;
            logV = false;
            zeroLogH = 1e-5;
            zeroLogV = 1e-5;
            for_model = true;
        }
        public CaptionAxisHorAndVert()
        {
            myfontHor = null;
            myfontVert = null;
            captionHor = null;
            captionVert = null;
        }
        public void SetForCurves()
        {
            for_model = false;
        }
        void SwapHorAndAdditional()
        {
            double[] o;
            double t;
            Ortho.GetOrtho(out o);
            t = o[0]; o[0] = o[6]; o[6] = t;
            t = o[1]; o[1] = o[7]; o[7] = t;
            Ortho.SetOrtho(o);
        }
        public void GenerateGrid(int ClientWidth, int ClientHeight, double scaleV = 1.0)
        {
            if (captionHor != null && captionVert != null)
            {
                if (for_model)
                {
                    double step;
                    step = captionHor.GenerateGrid(ClientWidth, ClientHeight, captionVert.GetWidthText);
                    captionVert.GenerateGrid(step, scaleV);
                }
                else
                {
                    captionHor.GenerateGrid(ClientWidth, ClientHeight, captionVert.GetWidthText);
                    captionVert.GenerateGrid(ClientWidth, ClientHeight, captionHor.GetHeightText);
                }

                if (DoubleAxis && captionHor2 != null)
                {
                    Ortho.AdditionalonJob = true;
                    captionHor2.GenerateGrid(ClientWidth, ClientHeight, captionVert.GetWidthText);
                    Ortho.AdditionalonJob = false;
                }
            }
        }
        public void DrawScaleLbls(int ClientWidth, int ClientHeight, double scaleV = 1.0)
        {
            if (captionHor != null && captionVert != null)
            {
                int indentV = DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0;
                captionHor.DrawScaleLbls(myfontHor, myfontHor_ind, ClientWidth, ClientHeight, 1.0, myfontVert.GetHeightText("0"), indentV);
                captionVert.DrawScaleLbls(myfontVert, myfontVert_ind, ClientWidth, ClientHeight, scaleV, 0, myfontHor.GetHeightText("0") + indentV);
                if (DoubleAxis)
                {
                    Ortho.AdditionalonJob = true;
                    String name = Ortho.HorAxisName;
                    Ortho.HorAxisName = Ortho.HorAxisName2;
                    captionHor2.DrawScaleLbls(myfontHor, myfontHor_ind, ClientWidth, ClientHeight, 1.0, myfontVert.GetHeightText("0"), 0);
                    Ortho.HorAxisName = name;
                    Ortho.AdditionalonJob = false;
                }

                int[] resView;
                GetNewViewport(ClientWidth, ClientHeight, out resView);
                GLContex.glViewport(resView[0], resView[1], resView[2], resView[3]);

                GLContex.glLineWidth(1);
                if (DrawGridFlag)
                    DrawGrid(resView[2], resView[3], scaleV);
            }
        }

        public void DrawScaleLblsHorzMetaFile(int ClientWidth, int ClientHeight,System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0)
        {
            if (captionHor != null)
            {
                int indentV = DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0;
                captionHor.DrawScaleLblsMetaFile(myfontHor, myfontHor_ind, ClientWidth, ClientHeight, 1.0, myfontVert.GetHeightText("0"), indentV,g, horzValue, vertValue,horzAxcis);
            }
        }

        public void DrawScaleLblsVertMetaFile(int ClientWidth, int ClientHeight, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0)
        {
            if (captionVert != null)
            {
                int indentV = DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0;
                captionVert.DrawScaleLblsMetaFile(myfontHor, myfontHor_ind, ClientWidth, ClientHeight, 1.0, myfontVert.GetHeightText("0"), indentV, g, horzValue, vertValue, horzAxcis);
            }
        }

        public void DrawGrid(int ClientWidth, int ClientHeight, double scaleV = 1.0)
        {
            if (captionHor != null && captionVert != null)
            {
                int indentV = DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0;
                captionHor.DrawGrid(myfontHor, ClientWidth, ClientHeight, 0, true, true);
                if (DoubleAxis)
                {
                    Ortho.AdditionalonJob = true;
                    captionHor2.DrawGrid(myfontHor, ClientWidth, ClientHeight, 0, false, false);
                    Ortho.AdditionalonJob = false;
                }
                captionVert.DrawGrid(myfontVert, ClientWidth, ClientHeight, 0, true, true, scaleV);
            }
        }

        public void DrawGridMetaFile(int ClientWidth, int ClientHeight, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0)
        {
            if (captionHor != null && captionVert != null)
            {
                captionHor.DrawGridMetaFile(myfontHor, ClientWidth, ClientHeight, 0, true, true, g, horzValue, vertValue, horzAxcis );
                captionVert.DrawGridMetaFile(myfontVert, ClientWidth, ClientHeight, 0, true, true, g, horzValue, vertValue, horzAxcis, scaleV);
            }
        }

        public void GetNewViewport(int ClientWidth, int ClientHeight, out int[] newView)
        {
            newView = new int[] { 0, 0, ClientWidth, ClientHeight };
            if (captionHor != null && captionVert != null)
            {
                int[] resVert;
                int[] resHor;
                captionHor.GetNewViewport(ClientWidth, ClientHeight, out resHor);
                captionVert.GetNewViewport(ClientWidth, ClientHeight, out resVert);
                newView[0] = myfontVert.GetHeightText("0");
                newView[1] = myfontHor.GetHeightText("0")+1 + (DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0);
                newView[2] = ClientWidth - myfontVert.GetHeightText("0");
                newView[3] = ClientHeight - myfontVert.GetHeightText("0") - (DoubleAxis ? myfontHor.GetHeightText("0") + 3 : 0);
            }
        }
        public void ClearFont()
        {
            myfontHor.ClearFont();
            myfontVert.ClearFont();
        }
        public bool ClickOnHor(int X, int Y, int ClientWidth, int ClientHeight)
        {
            return captionHor.InCaption(X, Y, ClientWidth, ClientHeight);
        }
        public bool ClickOnVer(int X, int Y, int ClientWidth, int ClientHeight)
        {
            return captionVert.InCaption(X, Y, ClientWidth, ClientHeight);
        }

    }

}
