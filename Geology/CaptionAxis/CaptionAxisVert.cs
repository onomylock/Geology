using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.DrawWindow
{
    class CaptionAxisVert : CaptionAxis
    {
        public int GetWidthText { get { return useFont.GetHeightTemplStr; } }
        public int GetHeightText { get { return useFont.GetWidthTemplStr; } }
        public CaptionAxisVert(FontGeology _font, FontGeology _font_ind, IOrthoControl _orth)
            : base(_font, _font_ind, _orth)
        {
        }
        public bool InCaption(int X, int Y, int ClientWidth, int ClientHeight)
        {
            return (0 < X && X < GetWidthText);
        }
        public override void GetNewViewport(int ClientWidth, int ClientHeight, out int[] newVect)
        {
            newVect = new int[] { GetWidthText, 0, ClientWidth - GetWidthText, ClientHeight };
        }
        protected override double GetLinGridStep(int ClientWidth, int ClientHeight, int indent = 0)
        {
            double[] ardMul = { 1, 2, 5, 10 };
            uint unMulSize = 3;
            double[] orthV;
            orth.GetOrtho(out orthV);
            double dH = orthV[3] - orthV[2];
            double hh = ClientHeight - indent;

            double nfFontSize = GetHeightText * (dH) / hh;
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

            double fend = orthV[3] * scale;
            double fbeg = orthV[2] * scale;
            double step = fStep * scale;

            rlfGrid = new List<double> { };
            try
            {
                rlfGrid.Clear();
                double fCur = Math.Floor(fbeg / step) * step;
                while (fCur <= fend)
                {
                    if (Math.Abs(fCur) < step / 4)
                        rlfGrid.Add(0);
                    else
                        rlfGrid.Add(fCur);
                    fCur += step;
                }

                //for (int i = 0; i < rlfGrid.Count; i++)
                //    rlfGrid[i] *= scale;
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

            double fend = orthV[3];
            double fbeg = orthV[2];

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
            font.GetWidthCaption = font.GetWidthText(orth.VertAxisName);
            double[] orthV;
            orth.GetOrtho(out orthV);
            int widthText = font.GetHeightText("0");
            double upHor = ((orthV[1] - orthV[0]) / (ClientWidth - widthText)) * (ClientWidth) + (-orthV[1] + orthV[0]);
            double lastV;

            double vRatio = (orthV[3] - orthV[2]) / (ClientHeight - indentV);
            double valueCorrection;
            double stringPositionL;
            double stringPositionR;

            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glPushMatrix();
            GLContex.glLoadIdentity();

            GLContex.glViewport(0, indentV, ClientWidth, ClientHeight - indentV);
            GLContex.glOrtho(0, ClientWidth, orthV[2], orthV[3], -1, 1);

            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glPushMatrix();
            GLContex.glLoadIdentity();

            double minh = orthV[0] - upHor;
            double hKoef = (orthV[1] - minh) / (ClientWidth);
            double maxDrawLetter = orthV[3] - (font.GetWidthCaption+3) * vRatio;
            double minDrawLetter = orthV[2] + indentV * vRatio;
            int stringLength;
            String logBase;
            String logDegree;

            if (rlfGrid.Count > 0)
            {
                bool[] toSkip = new bool[rlfGrid.Count];

                lastV = rlfGrid.First() - 1e+5;
                double tmpd;
                int ind = 0;
                foreach (var it in rlfGrid)
                {
                    double fVal = it / scale;
                    string msVal;
                    if (Log)
                        msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                    else
                        msVal = string.Format("{0:G}", fVal / scale);
                    int sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                    int sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;

                    stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                    stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;
                    toSkip[ind] = (stringPositionL < lastV + 10 * vRatio);
                    if (!toSkip[ind])
                        lastV = stringPositionR;

                    if (Math.Abs(fVal) < 1e-15)
                    {
                        toSkip[ind] = false;
                        lastV = stringPositionR;
                        tmpd = stringPositionL;
                        for (int i = ind - 1; i >= 0; i--)
                        {
                            fVal = rlfGrid[i] / scale;
                            if (Log)
                                msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                            else
                                msVal = string.Format("{0:G}", fVal / scale);
                            sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                            sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;

                            stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                            stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;
                            //stringPositionL = fVal - sizestring * 0.5 * vRatio;
                            //stringPositionR = fVal + sizestring * 0.5 * vRatio;

                            toSkip[i] = stringPositionR > tmpd - 10 * vRatio;
                            if (!toSkip[i])
                                tmpd = stringPositionL;
                        }
                    }


                    ind++;
                }

                lastV = rlfGrid.First() - 1e+5;
                ind = 0;
                foreach (var it in rlfGrid)
                {
                    if (toSkip[ind])
                    {
                        ind++;
                        continue;
                    }
                    double fVal = it / scale;
                    string msVal;
                    if (Log)
                        msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                    else
                        msVal = string.Format("{0:G}", fVal / scale);
                    int sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                    int sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;



                    if (Math.Abs(fVal) < 1e-15)
                        GLContex.glColor3f(1, 0, 0);
                    else
                        GLContex.glColor3f(0, 0, 0);
                    GLContex.glLineWidth(1);

                    //stringPositionL = fVal - sizestring * 0.5 * vRatio;
                    //stringPositionR = fVal + sizestring * 0.5 * vRatio;
                    stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                    stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;

                    lastV = stringPositionR;

                    if (stringPositionR < maxDrawLetter && stringPositionL > minDrawLetter)
                    {
                        if (Log)
                        {
                            if (fVal == 0)
                                font.PrintText(0, stringPositionL, 0, "0");
                            else
                            {
                                logBase = fVal < 0 ? "-10" : "10";
                                logDegree = msVal;
                                stringLength = font.GetWidthText(logBase) + fontIndex.GetWidthText(logDegree);
                                //stringPositionL = fVal - stringLength * 0.5 * vRatio;
                                //stringPositionR = fVal + stringLength * 0.5 * vRatio;
                                font.PrintText(0.1 * widthText, stringPositionL, 0, logBase);
                                fontIndex.PrintText(0, stringPositionL + font.GetWidthText(logBase) * vRatio, 0, msVal);
                            }
                        }
                        else
                            font.PrintText(0, stringPositionL, 0, msVal);
                    }
                    ind++;
                }
            }

            GLContex.glColor3f(0, 0, 0);

            GLContex.glBegin(GLContex.GL_LINES);
            GLContex.glVertex3d(widthText + 1, orthV[2], 0.99);
            GLContex.glVertex3d(widthText + 1, orthV[3], 0.99);
            GLContex.glEnd();
            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glLoadIdentity();

            GLContex.glOrtho(0, ClientWidth, 0, ClientHeight, -1, 1);
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
            GLContex.glLoadIdentity();
            
            GLContex.glColor3f(0, 0, 1);
            font.PrintText(0, ClientHeight - font.GetWidthCaption, 0, orth.VertAxisName);

            GLContex.glPopMatrix();
            GLContex.glMatrixMode(GLContex.GL_PROJECTION);
            GLContex.glPopMatrix();
            GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
        }

    //    private void DrawRotatedTextAt(System.Drawing.Graphics gr, float angle,
    //string txt, float x, float y, float w, float h, System.Drawing.Font the_font, System.Drawing.Brush the_brush)
    //    {
    //        System.Drawing.Drawing2D.GraphicsState state = gr.Save();
    //        gr.ResetTransform();

    //        // Rotate.
    //        gr.RotateTransform(angle);

    //        // Translate to desired position. Be sure to append
    //        // the rotation so it occurs after the rotation.
    //        gr.TranslateTransform(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);

    //        // Draw the text at the origin.
    //        gr.DrawString(txt, the_font, the_brush,,);
    //        gr.Restore(state);
    //    }


        public override void DrawScaleLblsMetaFile(FontGeology font, FontGeology fontIndex, int ClientWidth, int ClientHeight, double scale, int indentH, int indentV, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis)
        {
            font.GetWidthCaption = font.GetWidthText(orth.VertAxisName);
            double[] orthV;
            orth.GetOrtho(out orthV);
            int widthText = font.GetHeightText("0");
            double upHor = ((orthV[1] - orthV[0]) / (ClientWidth - widthText)) * (ClientWidth) + (-orthV[1] + orthV[0]);
            double lastV;

            double vRatio = (orthV[3] - orthV[2]) / (ClientHeight - indentV);
            double stringPositionL;
            double stringPositionR;
            double vx;
            double vy;
            double vx1;
            double vy1;


            double minh = orthV[0] - upHor;
            double hKoef = (orthV[1] - minh) / (ClientWidth);
            double maxDrawLetter = orthV[3] - (font.GetWidthCaption + 3) * vRatio;
            double minDrawLetter = orthV[2] + indentV * vRatio;
            int stringLength;
            String logBase;
            String logDegree;
            using (System.Drawing.Pen pen = new System.Drawing.Pen(new System.Drawing.Color(), 1))
            {

                if (rlfGrid.Count > 0)
                {
                    bool[] toSkip = new bool[rlfGrid.Count];

                    lastV = rlfGrid.First() - 1e+5;
                    double tmpd;
                    int ind = 0;
                    foreach (var it in rlfGrid)
                    {
                        double fVal = it / scale;
                        string msVal;
                        if (Log)
                            msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                        else
                            msVal = string.Format("{0:G}", fVal / scale);
                        int sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                        int sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;

                        stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                        stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;
                        toSkip[ind] = (stringPositionL < lastV + 10 * vRatio);
                        if (!toSkip[ind])
                            lastV = stringPositionR;

                        if (Math.Abs(fVal) < 1e-15)
                        {
                            toSkip[ind] = false;
                            lastV = stringPositionR;
                            tmpd = stringPositionL;
                            for (int i = ind - 1; i >= 0; i--)
                            {
                                fVal = rlfGrid[i] / scale;
                                if (Log)
                                    msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                                else
                                    msVal = string.Format("{0:G}", fVal / scale);
                                sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                                sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;

                                stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                                stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;
                                //stringPositionL = fVal - sizestring * 0.5 * vRatio;
                                //stringPositionR = fVal + sizestring * 0.5 * vRatio;

                                toSkip[i] = stringPositionR > tmpd - 10 * vRatio;
                                if (!toSkip[i])
                                    tmpd = stringPositionL;
                            }
                        }


                        ind++;
                    }

                    lastV = rlfGrid.First() - 1e+5;
                    ind = 0;
                    foreach (var it in rlfGrid)
                    {
                        if (toSkip[ind])
                        {
                            ind++;
                            continue;
                        }
                        double fVal = it / scale;
                        string msVal;
                        if (Log)
                            msVal = Math.Abs(fVal) < 1e-15 ? "0" : msVal = string.Format("{0:G}", Math.Abs(fVal / scale) + Math.Log10(LogZero));
                        else
                            msVal = string.Format("{0:G}", fVal / scale);
                        int sizestring = msVal == "0" ? font.GetWidthText("0") : font.GetWidthText(Log ? "10" : msVal) + (Log ? fontIndex.GetWidthText(msVal) : 0);
                        int sizeStringBase = Log ? fontIndex.GetWidthText("10") : 0;



                        pen.Color = System.Drawing.Color.FromArgb(Math.Abs(fVal) < 1e-15 ? 255 : 0, 0, 0);


                        //stringPositionL = fVal - sizestring * 0.5 * vRatio;
                        //stringPositionR = fVal + sizestring * 0.5 * vRatio;
                        stringPositionL = fVal - (Log ? sizeStringBase : sizestring) * 0.5 * vRatio;
                        stringPositionR = fVal + ((Log ? sizeStringBase : sizestring) * 0.5 + (Log ? sizestring - sizeStringBase : 0)) * vRatio;

                        lastV = stringPositionR;

                        if (stringPositionR < maxDrawLetter && stringPositionL > minDrawLetter)
                        {
                            if (Log)
                            {
                                if (fVal == 0)
                                {
                                    System.Drawing.SizeF sizeF0 = g.MeasureString("0", new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                                    vx = g.ClipBounds.X - sizeF0.Height * 0.05F;
                                    vy = ClientHeight - ((it) - orthV[2]) * horzValue - sizeF0.Width * 0.4F;
                                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                                    drawFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;


                                    System.Drawing.RectangleF drawRect = new System.Drawing.RectangleF((float)(vx), (float)vy, sizeF0.Height, sizeF0.Width);

                                    using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
                                    {
                                        float vx_c = drawRect.Left + (drawRect.Width / 2);
                                        float vy_c = drawRect.Top + (drawRect.Height / 2);

                                        m.RotateAt(180, new System.Drawing.PointF(vx_c,vy_c));

                                        g.Transform = m;

                                        g.DrawString("0", new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F), pen.Brush, drawRect, drawFormat);


                                        g.ResetTransform();
                                    }

                                    //DrawRotatedTextAt(g, -90, "0",x, y, the_font, pen.Brush);
                                    //font.PrintText(0, stringPositionL, 0, "0");
                                }
                                else
                                {
                                    logBase = fVal < 0 ? "-10" : "10";
                                    logDegree = msVal;
                                    stringLength = font.GetWidthText(logBase) + fontIndex.GetWidthText(logDegree);
                                    System.Drawing.SizeF sizeF10 = g.MeasureString(logBase, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.6F));
                                    System.Drawing.SizeF sizeFDegress = g.MeasureString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.4F));
                                    vx = g.ClipBounds.X;
                                    vy = ClientHeight - ((it) - orthV[2]) * horzValue - sizeF10.Width * 0.5;
                                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                                    drawFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;


                                    System.Drawing.RectangleF drawRect = new System.Drawing.RectangleF((float)(vx + sizeF10.Height * 0.08F), (float)vy, sizeF10.Height , sizeF10.Width);
                                    System.Drawing.RectangleF drawRectDegress = new System.Drawing.RectangleF((float)(vx - sizeF10.Height * 0.1), (float)(vy - sizeF10.Width*0.5F), sizeFDegress.Height , sizeFDegress.Width);




                                    using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
                                    {
                                        m.RotateAt(180, new System.Drawing.PointF(drawRect.Left + (drawRect.Width / 2),
                                                                  drawRect.Top + (drawRect.Height / 2)));
                                        g.Transform = m;
                                        g.DrawString(logBase, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.6F), pen.Brush, drawRect, drawFormat);
                                        g.ResetTransform();
                                    }

                                    using (System.Drawing.Drawing2D.Matrix m2 = new System.Drawing.Drawing2D.Matrix())
                                    {
                                        m2.RotateAt(180, new System.Drawing.PointF(drawRectDegress.Left + (drawRectDegress.Width / 2),
                                                                 drawRectDegress.Top + (drawRectDegress.Height / 2)));
                                        g.Transform = m2;
                                        g.DrawString(msVal, new System.Drawing.Font(fontIndex.StyleFont, (fontIndex.FontSize * 0.4F)), pen.Brush, drawRectDegress, drawFormat);
                                        g.ResetTransform();

                                    }

                                }
                            }
                            else
                            {
                                System.Drawing.SizeF sizeF0degree = g.MeasureString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                                vx = g.ClipBounds.X - sizeF0degree.Height * 0.05;
                                vy = ClientHeight - ((it) - orthV[2]) * horzValue- sizeF0degree.Width*0.4;
                                System.Drawing.StringFormat drawFormatdegree = new System.Drawing.StringFormat();
                                drawFormatdegree.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;

                                System.Drawing.RectangleF drawRect0degree = new System.Drawing.RectangleF((float)(vx), (float)(vy), sizeF0degree.Height, sizeF0degree.Width);
                                using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
                                {
                                    m.RotateAt(180, new System.Drawing.PointF(drawRect0degree.Left + (drawRect0degree.Width / 2),
                                                              drawRect0degree.Top + (drawRect0degree.Height / 2)));
                                    g.Transform = m;

                                    g.DrawString(msVal, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F), pen.Brush, drawRect0degree, drawFormatdegree);
                                 


                                    g.ResetTransform();
                                }
                                //font.PrintText(0, stringPositionL, 0, msVal);
                            }
                        }
                        ind++;
                    }
                }

                pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);


                vx = g.ClipBounds.X;
                vy = ClientHeight - (orthV[2] - orthV[2]) * horzValue;
                vx1 = g.ClipBounds.X;
                vy1 = ClientHeight - (orthV[3] - orthV[2]) * horzValue;
                g.DrawLine(pen, (float)vx, (float)vy, (float)vx1, (float)vy1);
                //GLContex.glVertex3d(widthText + 1, orthV[2], 0.99);
                //GLContex.glVertex3d(widthText + 1, orthV[3], 0.99);

                pen.Color = System.Drawing.Color.FromArgb(0, 0, 255);
                System.Drawing.SizeF sizeF0Name = g.MeasureString(orth.VertAxisName, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F));
                vx = g.ClipBounds.X - sizeF0Name.Width * 0.05F;
                vy = ClientHeight - (orthV[3] - orthV[2]) * horzValue - sizeF0Name.Width * 0.8F;
                System.Drawing.StringFormat drawFormatName = new System.Drawing.StringFormat();
                drawFormatName.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;

                System.Drawing.RectangleF drawRect0Name = new System.Drawing.RectangleF((float)(vx), (float)vy, sizeF0Name.Height, sizeF0Name.Width*2);

                using (System.Drawing.Drawing2D.Matrix m1 = new System.Drawing.Drawing2D.Matrix())
                {
                    m1.RotateAt(180, new System.Drawing.PointF(drawRect0Name.Left + (drawRect0Name.Width / 2),
                                              drawRect0Name.Top + (drawRect0Name.Height / 2)));
                    g.Transform = m1;

                    g.DrawString(orth.VertAxisName, new System.Drawing.Font(font.StyleFont, font.FontSize * 0.7F), pen.Brush, drawRect0Name, drawFormatName);



                    g.ResetTransform();
                }


                //font.PrintText(0, ClientHeight - font.GetWidthCaption, 0, orth.VertAxisName);

            }
        }


        public override void DrawGrid(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, double scaleV = 1.0)
        {
            GLContex.glColor3f(0.7f, 0.7f, 0.7f);
            GLContex.glLineWidth(1);
            double[] orthV;
            orth.GetOrtho(out orthV);
            double zBuf = orth.GetMaxZBuf - (orth.GetMaxZBuf - orth.GetMinZBuf) / 20.0;
            double zBuf2 = orth.GetMaxZBuf - (orth.GetMaxZBuf - orth.GetMinZBuf) / 21.0;

            double vRatio = (orthV[3] - orthV[2]) / ClientHeight;
            double minDraw = orthV[2] + vRatio * indent;
            double maxDraw = orthV[3] - font.GetWidthText(orth.VertAxisName) * vRatio;
            double crd;

            if (drawGridLines)
            {
                GLContex.glBegin(GLContex.GL_LINES);
                foreach (var it in rlfGrid)
                {
                    crd = it / scaleV;
                    if (crd > minDraw && crd < maxDraw)
                    {
                        GLContex.glVertex3d(orthV[0], it / scaleV, zBuf);
                        GLContex.glVertex3d(orthV[1], it / scaleV, zBuf);
                    }
                }

                GLContex.glEnd();
            }

            if (drawLabelsTips)
            {
                GLContex.glColor3f(0f, 0f, 0f);
                GLContex.glBegin(GLContex.GL_LINES);

                double dx = (orthV[1] - orthV[0]) * 4 / ClientWidth;
                foreach (var it in rlfGrid)
                {
                    crd = it / scaleV;
                    if (crd > minDraw && crd < maxDraw)
                    {
                        GLContex.glVertex3d(orthV[0], crd, zBuf2);
                        GLContex.glVertex3d(orthV[0] + dx, crd, zBuf2);
                    }
                }

                GLContex.glEnd();
            }
        }

        public override void DrawGridMetaFile(FontGeology font, int ClientWidth, int ClientHeight, int indent, bool drawGridLines, bool drawLabelsTips, System.Drawing.Graphics g, double horzValue, double vertValue, float horzAxcis, double scaleV = 1.0)
        {
            double[] orthV;
            orth.GetOrtho(out orthV);

            double vRatio = (orthV[3] - orthV[2]) / ClientHeight;
            double minDraw = orthV[2] + vRatio * indent;
            double maxDraw = orthV[3] - font.GetWidthText(orth.VertAxisName) * vRatio;
            double crd;

            using (System.Drawing.Pen pen = new System.Drawing.Pen(new System.Drawing.Color(), 1))
            {
                pen.Color = System.Drawing.Color.FromArgb(179, 179, 179);

                if (drawGridLines)
                {
                    foreach (var it in rlfGrid)
                    {
                        crd = it / scaleV;
                        if (crd > minDraw && crd < maxDraw)
                        {
                            //GLContex.glVertex3d(orthV[0], it / scaleV, zBuf);
                            //GLContex.glVertex3d(orthV[1], it / scaleV, zBuf);
                            double vx = horzAxcis + (orthV[0] - orthV[0]) * vertValue;
                            double vy = ClientHeight - ((it / scaleV) - orthV[2]) * horzValue;
                            double vx1 = horzAxcis + (orthV[1] - orthV[0]) * vertValue;
                            double vy1 = ClientHeight - ((it / scaleV) - orthV[2]) * horzValue;

                            g.DrawLine(pen, new System.Drawing.PointF((float)(vx), (float)(vy)), new System.Drawing.PointF((float)vx1, (float)(vy1)));

                        }
                    }
                }

                if (drawLabelsTips)
                {
                    pen.Color = System.Drawing.Color.FromArgb(0, 0, 0);

                    double dx = (orthV[1] - orthV[0]) * 4 / ClientWidth;
                    foreach (var it in rlfGrid)
                    {
                        crd = it / scaleV;
                        if (crd > minDraw && crd < maxDraw)
                        {
                            //GLContex.glVertex3d(orthV[0], crd, zBuf2);
                            //GLContex.glVertex3d(orthV[0] + dx, crd, zBuf2);
                            double vx = horzAxcis + (orthV[0] - orthV[0]) * vertValue;
                            double vy = ClientHeight - (crd - orthV[2]) * horzValue;
                            double vx1 = horzAxcis + (orthV[0] + dx - orthV[0]) * vertValue;
                            double vy1 = ClientHeight - (crd - orthV[2]) * horzValue;
                            g.DrawLine(pen, new System.Drawing.PointF((float)(vx), (float)(vy)), new System.Drawing.PointF((float)vx1, (float)(vy1)));
                        }
                    }

                }
            }
        }
    }
}
