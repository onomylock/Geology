using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.DrawWindow
{
    abstract class CaptionAxis
    {
        public bool Log;
        public double LogZero;

        protected FontGeology useFont;
        protected FontGeology useFontIndex;
        protected IOrthoControl orth;

        protected List<double> rlfGrid;

        public CaptionAxis(FontGeology _font, FontGeology _font_ind, IOrthoControl _orth)
        {
            orth = _orth;
            useFont = _font;
            useFontIndex = _font_ind;
            Log = false;
        }
        public virtual void GetNewViewport(int ClientWidth, int ClientHeight, out int[] newVect)
        {
            newVect = new int[] { 0, 0, ClientWidth, ClientHeight };
        }
        public double GenerateGrid(int ClientWidth, int ClientHeight, int indent = 0)
        {
            double step = 0;
            if (Log)
                GenerateLogGrid();
            else
            {
                step = GetLinGridStep(ClientWidth, ClientHeight, indent);
                GenerateLinGrid(step);
            }
            return step;
        }
        public void GenerateGrid(double step, double scale = 1.0)
        {
            GenerateLinGrid(step, scale);
        }
        abstract protected double GetLinGridStep(int ClientWidth, int ClientHeight, int indent = 0);
        abstract protected void GenerateLinGrid(double fStep, double scale = 1.0);
        abstract protected void GenerateLogGrid();
        abstract public void DrawScaleLbls(FontGeology font, FontGeology fontIndex, int ClientWidth, int ClientHeight, double scale, int indentH, int indentV);
        abstract public void DrawScaleLblsMetaFile(FontGeology font, FontGeology fontIndex, int ClientWidth, int ClientHeight, double scale, int indentH, int indentV, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis);
        abstract public void DrawGrid(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, double scaleV = 1.0);
        abstract public void DrawGridMetaFile(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0);
    }
}
