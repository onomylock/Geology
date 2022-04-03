using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.DrawWindow
{
    class CaptionAxisHor : CaptionAxis
    {
        double step = 0.0;
        double startFrom = -1e+30;
        bool isIntegerAxis = false;
        public double Step { get { return step; } set { step = value; } }
        public double StartFrom { get { return startFrom; } set { startFrom = value; } }
        public int GetHeightText { get { return useFont.GetHeightTemplStr; } }
        public int GetWidthText { get { return useFont.GetWidthTemplStr; } }
        public bool IsIntegerAxis
        {
            get { return isIntegerAxis; }
            set { isIntegerAxis = value; }
        }
        public CaptionAxisHor(FontGeology _font, FontGeology _font_ind, IOrthoControl _orth)
            : base(_font, _font_ind, _orth)
        {
        }
        public bool InCaption(int X, int Y, int ClientWidth, int ClientHeight)
        {
            return (ClientHeight < Y && Y < ClientHeight + GetHeightText);
        }
        public override void GetNewViewport(int ClientWidth, int ClientHeight, out int[] newVect)
        {
            newVect = new int[] { 0, GetHeightText, ClientWidth, ClientHeight - GetHeightText };
        }
        protected override double GetLinGridStep(int ClientWidth, int ClientHeight, int indent = 0)
        {
            double[] ardMul = { 1, 2, 5, 10 };
            uint unMulSize = 3;
            double[] orthV;
            orth.GetOrtho(out orthV);

            double dH = orthV[1] - orthV[0];
            double hh = ClientWidth - indent;

            double nfFontSize = GetWidthText * (dH) / hh;
            double dTiles = (double)Math.Floor(dH / nfFontSize);

            double dStep = dH / dTiles;
            double dMul = Math.Pow((double)10, Math.Floor(Math.Log10(dStep)));

            uint i = 0;
            for (i = 1; i < unMulSize; ++i)
                if (dMul * ardMul[i] > dStep)
                    break;

            dStep = ardMul[i] * dMul;

            return (double)dStep;
        }
        protected override void GenerateLinGrid(double fStep, double scale = 1.0)
        {
            double[] orthV;
            orth.GetOrtho(out orthV);

            double fend = orthV[1] + (IsIntegerAxis ? 1 : 0);
            double fbeg = orthV[0] + (IsIntegerAxis ? 1 : 0);

            rlfGrid = new List<double> { };
            try
            {
                rlfGrid.Clear();
                double fCur = Math.Floor(fbeg / fStep) * fStep;
                if (IsIntegerAxis && Step > 0)
                {
                    fCur = StartFrom +  (int)(Math.Max(fbeg - StartFrom, 0) / Step) * Step;
                    while (fCur <= fend)
                    {
                        if (Math.Abs(fCur-1) < fStep / 4)
                            rlfGrid.Add(0);
                        else
                            rlfGrid.Add(fCur-1);
                        fCur += Step;
                    }
                }
                else
                    while (fCur <= fend)
                    {
                        if (Math.Abs(fCur - (IsIntegerAxis ? 1 : 0)) < fStep / 4)
                            rlfGrid.Add(0);
                        else
                            rlfGrid.Add(fCur - (IsIntegerAxis ? 1 : 0));
                        fCur += fStep;
                    }
            }
            catch (Exception e)
            {
                rlfGrid.Clear();
                MessageBox.Show(
                    "Произошла ошибка во время построения линейной сетки.",
                    "Невозможно построить линейную сетку");
            }
        }
        protected override void GenerateLogGrid()
        {
            double[] orthV;
            orth.GetOrtho(out orthV);

            double fend = orthV[1];
            double fbeg = orthV[0];

            rlfGrid = new List<double> { };
            try
            {
                fbeg = (fbeg > 0 ? Math.Floor(fbeg) : Math.Ceiling(fbeg));
                fend = (fend > 0 ? Math.Floor(fend) : Math.Ceiling(fend));
                for (int i = (int)fbeg; i <= (int)fend; i++)
                    rlfGrid.Add((double)i);
            }
            catch (Exception e)
            {
                rlfGrid.Clear();
                MessageBox.Show(
                    "Произошла ошибка во время построения логарифмической сетки.",
                    "Невозможно построить логарифмическую сетку");
            }
        }
        public override void DrawScaleLbls(FontGeology font, FontGeology fontIndex, int ClientWidth, int ClientHeight, double scale, int indentH, int indentV)
        {
            font.GetWidthCaption = font.GetWidthText(orth.HorAxisName);
            double[] orthV;
            orth.GetOrtho(out orthV);
            int heightText = font.GetHeightText("0");
            double upHor = ((orthV[1] - orthV[0]) / (ClientWidth - indentH)) * (ClientWidth) + (-orthV[1] + orthV[0]);
            double upVert = ((orthV[3] - orthV[2]) / (ClientHeight - heightText - indentV)) * (ClientHeight) + (-orthV[3] + orthV[2]);
            double hRatio = (orthV[1] - orthV[0]) / (ClientWidth - indentH);
            double vRatio = (orthV[3] - orthV[2]) / (ClientHeight - indentV);
            double valueCorrection;
            double stringPositionL;
            double stringPositionR;
            double displacementV = indentV * vRatio;

            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glPushMatrix();
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glPushMatrix();
            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glLoadIdentity();

            GLContex.glViewport(indentH,  0, ClientWidth - indentH, ClientHeight);
            GLContex.glOrtho(orthV[0], orthV[1], 0, ClientHeight, -1, 1);

            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();

            double minh = orthV[0] - upHor;
            double minv = orthV[2] - upVert;
            double hKoef = (orthV[1] - minh) / (ClientWidth);
            double vKoef = (orthV[3] - minv) / (ClientHeight);
            double maxDrawLetter = orthV[1] - font.GetWidthCaption * hRatio;
            double minDrawLetter = orthV[0] + indentH * hRatio;
            int stringLength;
            String logBase;
            String logDegree;
            int lastIndex = -1;

            int i = 0;
            foreach (var it in rlfGrid)
            {
                if (it < orthV[0] || Step > 0 && it < StartFrom-1)
                {
                    i++;
                    continue;
                }                

                double fVal = it * scale;

                if (lastIndex != -1 && (fVal - rlfGrid[lastIndex]) < Step)
                {
                    i++;
                    continue;
                }

                lastIndex = i;

                string msVal;
                if (Log)
                    msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal) + Math.Log10(LogZero));
                else
                    msVal = IsIntegerAxis ? (Math.Truncate(fVal+1)).ToString("0") : string.Format("{0:G}", fVal);
                int sizestring = font.GetWidthText(Log ? "10" : msVal);
                int sizeFirstLetter = font.GetWidthText(msVal.Substring(0, 1));

                GLContex.glColor3f(Math.Abs(fVal) < 1e-15 ? 1f : 0f, 0f, 0f);
                GLContex.glLineWidth(1);

                stringPositionL = IsIntegerAxis ? fVal - sizeFirstLetter * 0.5 * hRatio : fVal - sizestring * 0.5 * hRatio;
                stringPositionR = fVal + sizestring * 0.5 * hRatio;

                if (stringPositionR < maxDrawLetter)
                {
                    if (Log)
                    {
                        if (fVal == 0)
                            font.PrintText(stringPositionL, indentV, 0, "0");
                        else
                        {
                            logBase = fVal < 0 ? "-10" : "10";
                            logDegree = msVal;
                            stringLength = font.GetWidthText(logBase) + fontIndex.GetWidthText(logDegree);
                            stringPositionL = fVal - sizestring * 0.5 * hRatio;
                            font.PrintText(stringPositionL, indentV, 0, logBase);
                            fontIndex.PrintText(stringPositionL + font.GetWidthText(logBase) * hRatio, indentV + 0.2 * heightText, 0, msVal);
                        }
                    }
                    else
                        font.PrintText(stringPositionL, indentV, 0, msVal);
                }
                i++;
            }

            GLContex.glColor3f(0f, 0f, 0f);
            GLContex.glBegin(GLContex.GL_LINES);
            double dy = 4.0;

            foreach (var it in rlfGrid)
            {
                valueCorrection = -((it - orthV[0]) / (orthV[1] - orthV[0])) * hRatio * 10;
                valueCorrection = indentH* hRatio + it + valueCorrection;
                GLContex.glVertex3d(it, indentV + heightText -1, 0);
                GLContex.glVertex3d(it, indentV + heightText  + dy, 0);
            }

            GLContex.glEnd();

            GLContex.glColor3f(0, 0, 0);
            //горизонтальная ось
            GLContex.glBegin(GLContex.GL_LINES);
            GLContex.glVertex3d(orthV[0], indentV + heightText + 1, 0.99);
            GLContex.glVertex3d(orthV[1], indentV + heightText + 1, 0.99);
            GLContex.glEnd();
            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glLoadIdentity();

            GLContex.glOrtho(0, ClientWidth, 0, ClientHeight, -1, 1);
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();
            
            GLContex.glColor3f(0, 0, 1);
            font.PrintText(ClientWidth - font.GetWidthCaption, indentV, 0, orth.HorAxisName);

            GLContex.glLoadIdentity();
            GLContex.glPopMatrix();
            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glPopMatrix();
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
        }

        public override void DrawScaleLblsMetaFile(FontGeology font, FontGeology fontIndex, int ClientWidth, int ClientHeight, double scale, int indentH, int indentV, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis)
        {


            font.GetWidthCaption = font.GetWidthText(orth.HorAxisName);
            double[] orthV;
            orth.GetOrtho(out orthV);
            int heightText = font.GetHeightText("0");
            double hRatio = (orthV[1] - orthV[0]) / (ClientWidth - indentH);
            double valueCorrection;
            double stringPositionR;
            double vx;
            double vy;
            double vx1;
            double vy1;

            double maxDrawLetter = orthV[1] - font.GetWidthCaption * hRatio;
            double minDrawLetter = orthV[0] + indentH * hRatio;
            String logBase;
            String logDegree;
            int lastIndex = -1;

            double displaceV = indentV * (orthV[3] - orthV[2]) / ClientHeight;


            int i = 0;
            using (System.Drawing.Pen pen = new System.Drawing.Pen(new System.Drawing.Color(), 1))
            {
                foreach (var it in rlfGrid)
                {
                if (it < orthV[0] || Step > 0 && it < StartFrom - 1)
                {
                    i++;
                    continue;
                }

                double fVal = it * scale;

                if (lastIndex != -1 && (fVal - rlfGrid[lastIndex]) < Step)
                {
                    i++;
                    continue;
                }

                lastIndex = i;

                string msVal;
                if (Log)
                    msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal) + Math.Log10(LogZero));
                else
                    msVal = IsIntegerAxis ? (Math.Truncate(fVal + 1)).ToString("0") : string.Format("{0:G}", fVal);
                int sizestring = font.GetWidthText(Log ? "10" : msVal);


                    pen.Color = System.Drawing.Color.FromArgb(Math.Abs(fVal) < 1e-15 ? 255 : 0, 0, 0);

                    stringPositionR = fVal + sizestring * hRatio;

                    if (stringPositionR < maxDrawLetter)
                    {
                        if (Log)
                        {
                            if (fVal == 0)
                            {
                                vx = horzAxcis + (it - orthV[0]) * vertValue;
                                vy = g.ClipBounds.Y;


                                System.Drawing.SizeF sizeF0 = g.MeasureString("0", new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                                System.Drawing.RectangleF drawRect0 = new System.Drawing.RectangleF((float)(vx - sizeF0.Width * 0.5F), (float)vy, sizeF0.Width, horzAxcis);



                                g.DrawString("0", new System.Drawing.Font(font.StyleFont, font.FontSize), pen.Brush, drawRect0);
                            }
                            else
                            {
                                logBase = fVal < 0 ? "-10" : "10";
                                logDegree = msVal;
                                vx = horzAxcis + (it - orthV[0]) * vertValue;
                                vy = g.ClipBounds.Y;
                                System.Drawing.SizeF sizeF10 = g.MeasureString(logBase, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                                System.Drawing.SizeF sizeFDegress = g.MeasureString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.5F));
                                if (vx - sizeF10.Width * 0.5F >= g.ClipBounds.X )
                                {
                                    System.Drawing.RectangleF drawRect = new System.Drawing.RectangleF((float)(vx - sizeF10.Width * 0.5F), (float)vy, sizeF10.Width, horzAxcis);
                                    g.DrawString(logBase, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F), pen.Brush, drawRect);
                                    System.Drawing.RectangleF drawRectDegress = new System.Drawing.RectangleF((float)(vx + sizeF10.Width * 0.5F), (float)vy, sizeFDegress.Width, horzAxcis);
                                    g.DrawString(msVal, new System.Drawing.Font(fontIndex.StyleFont, (fontIndex.FontSize * 0.5F)), pen.Brush, drawRectDegress);
                                }
                            }
                        }
                        else
                        {
                           vx = horzAxcis + (it- orthV[0]) * vertValue;
                           vy = g.ClipBounds.Y;
                            System.Drawing.SizeF sizeF0degree = g.MeasureString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                            System.Drawing.RectangleF drawRect0degree = new System.Drawing.RectangleF((float)(vx - sizeF0degree.Width * 0.5F), (float)vy, sizeF0degree.Width, horzAxcis);

                            g.DrawString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize*0.7F), pen.Brush, drawRect0degree);

                        }
                    }
                    i++;

                }


                pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                double dy = 4.0;

                foreach (var it in rlfGrid)
                {
                    valueCorrection = -((it - orthV[0]) / (orthV[1] - orthV[0])) * hRatio * 10;
                    valueCorrection = indentH * hRatio + it + valueCorrection;
                    vx = horzAxcis + (it - orthV[0]) * vertValue;
                    vy = ClientHeight - (indentV + heightText * 0.7F - 1 - orthV[2]) * horzValue;
                    vx1 = horzAxcis + (it - orthV[0]) * vertValue;
                    vy1 = ClientHeight - (indentV + heightText * 0.7F + dy - orthV[2]) * horzValue;
                    g.DrawLine(pen, (float)vx, (float)(vy), (float)vx1, (float)(vy1));
                }


                //горизонтальная ось
                pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                vx = horzAxcis + (orthV[0] - orthV[0]) * vertValue;
                vy = g.ClipBounds.Y;
                vx1 = horzAxcis + (orthV[1] - orthV[0]) * vertValue;
                vy1 = g.ClipBounds.Y;
                g.DrawLine(pen, (float)vx, (float)vy, (float)vx1, (float)vy1);

                pen.Color = System.Drawing.Color.FromArgb(0, 0, 255);
               vx = horzAxcis + (orthV[1] - orthV[0]) * vertValue;
               vy = g.ClipBounds.Y;
                System.Drawing.SizeF sizeF0Name = g.MeasureString(orth.HorAxisName, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                System.Drawing.RectangleF drawRect0Name = new System.Drawing.RectangleF((float)(vx - sizeF0Name.Width), (float)vy, sizeF0Name.Width, horzAxcis);

                g.DrawString(orth.HorAxisName, new System.Drawing.Font(font.StyleFont, font.FontSize*0.7F), pen.Brush,drawRect0Name);
                }
        }
        public override void DrawGrid(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, double scaleV = 1.0)
        {

            double[] orthV;
            orth.GetOrtho(out orthV);
            double zBuf = orth.GetMaxZBuf - (orth.GetMaxZBuf - orth.GetMinZBuf) / 20.0;
            double zBuf2 = orth.GetMaxZBuf - (orth.GetMaxZBuf - orth.GetMinZBuf) / 21.0;
            double displaceV = indent * (orthV[3] - orthV[2]) / ClientHeight;

            GLContex.glColor3f(0.7f, 0.7f, 0.7f);
            GLContex.glLineWidth(1);

            if (drawGridLines)
            {
                GLContex.glBegin(GLContex.GL_LINES);

                foreach (var it in rlfGrid)
                {
                    GLContex.glVertex3d(it, displaceV + orthV[2], zBuf);
                    GLContex.glVertex3d(it, displaceV + orthV[3], zBuf);
                }

                GLContex.glEnd();
            }


            if (drawLabelsTips)
            {
                GLContex.glColor3f(0f, 0f, 0f);
                GLContex.glBegin(GLContex.GL_LINES);
                double dy = (orthV[3] - orthV[2]) * 4 / ClientHeight;

                foreach (var it in rlfGrid)
                {
                    GLContex.glVertex3d(it, displaceV + orthV[2], zBuf2);
                    GLContex.glVertex3d(it, displaceV + orthV[2] + dy, zBuf2);
                }

                GLContex.glEnd();
            }
        }

        public override void DrawGridMetaFile(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0)
        {

            double[] orthV;
            orth.GetOrtho(out orthV);
            double displaceV = indent * (orthV[3] - orthV[2]) / ClientHeight;
            double vRatio = (orthV[1] - orthV[0]) / ClientWidth;
            double minDraw = orthV[0] + vRatio * indent;
            double maxDraw = orthV[1] - font.GetWidthText(orth.HorAxisName) * vRatio;
            double crd;
            using (System.Drawing.Pen pen = new System.Drawing.Pen(new System.Drawing.Color(),1))
            {
                pen.Color = System.Drawing.Color.FromArgb(179,179,179);

            if (drawGridLines)
            {

                foreach (var it in rlfGrid)
                {
                        //GLContex.glVertex3d(it, displaceV + orthV[2], zBuf);
                        //GLContex.glVertex3d(it, displaceV + orthV[3], zBuf);
                        crd = it / scaleV;
                        if (crd > minDraw && crd < maxDraw)
                        {
                            double vx = horzAxcis + (it - orthV[0]) * vertValue;
                            double vy = ClientHeight - (displaceV + orthV[2] - orthV[2]) * horzValue;
                            double vx1 = horzAxcis + (it - orthV[0]) * vertValue;
                            double vy1 = ClientHeight - (displaceV + orthV[3] - orthV[2]) * horzValue;
                            g.DrawLine(pen, new System.Drawing.PointF((float)(vx), (float)(vy)), new System.Drawing.PointF((float)vx1, (float)(vy1)));
                        }
                    }

            }

                // Вертикальная прямая
                pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                {
                    var it1 = minDraw;// rlfGrid.First();
                    double vx = horzAxcis + (it1 - orthV[0]) * vertValue + 1;
                    double vy = ClientHeight - (displaceV + orthV[2] - orthV[2]) * horzValue;
                    double vx1 = horzAxcis + (it1 - orthV[0]) * vertValue + 1;
                    double vy1 = ClientHeight - (displaceV + orthV[3] - orthV[2]) * horzValue;
                    g.DrawLine(pen, new System.Drawing.PointF((float)(vx), (float)(vy)), new System.Drawing.PointF((float)vx1, (float)(vy1)));
                }



                if (drawLabelsTips)
            {
                    pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);

                    double dy = (orthV[3] - orthV[2]) * 4 / ClientHeight;

                foreach (var it in rlfGrid)
                {
                        //GLContex.glVertex3d(it, displaceV + orthV[2], zBuf2);
                        //GLContex.glVertex3d(it, displaceV + orthV[2] + dy, zBuf2);
                        double vx = horzAxcis + (it - orthV[0]) * vertValue;
                        double vy = ClientHeight - (displaceV + orthV[2] - orthV[2]) * horzValue;
                        double vx1 = horzAxcis + (it - orthV[0]) * vertValue;
                        double vy1 = ClientHeight - (displaceV + dy+orthV[2] - orthV[2]) * horzValue;
                        g.DrawLine(pen, new System.Drawing.PointF((float)(vx), (float)(vy)), new System.Drawing.PointF((float)vx1, (float)(vy1)));

                    }

            }
            }
        }
    }
}
