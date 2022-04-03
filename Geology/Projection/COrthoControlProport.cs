using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geology.DrawWindow
{
    public class COrthoControlProport : IOrthoControl
    {
        string nameHorAxis = "";
        string nameHorAxis2 = "Year";
        string nameVertAxis = "";
        bool doubleAxis = false;
        double[] additionalOrth = { -1, 1};
        int sizeFont = 16;
        string styleFont = "Arial";
        double centerHor, centerVert, dHor, dVer;
        double zBufmin, zBufmax;
        double baseCenterHor, baseCenterVert, basedHor, basedVert;
        bool additionOnJob = false;
        public bool AdditionalonJob
        {
            get { return additionOnJob; }
            set { additionOnJob = value; }
        }
        double coefHeightToWidth = 1;
        public double CoefHeightToWidth { get { return coefHeightToWidth; } set { coefHeightToWidth = value; } }
        double dVert { get { return eqScale ? dHor * coefHeightToWidth : dVer; } set { dVer = value; } }
        public double[] AdditionalOrth
        {
            get { return additionalOrth;  }
            set { additionalOrth = value; }
        }
        bool eqScale;


        public COrthoControlProport(string _nameHorAxis, string _nameVertAxis, double _coefHeightToWidth)
        {
            nameHorAxis = _nameHorAxis;
            nameHorAxis2 = _nameHorAxis;
            nameVertAxis = _nameVertAxis;
            coefHeightToWidth = _coefHeightToWidth;
            eqScale = true;
        }
        public COrthoControlProport(string _nameHorAxis, string _nameVertAxis, double _coefHeightToWidth, double[] orth, bool _eq)
            : this(_nameHorAxis, _nameVertAxis, _coefHeightToWidth)
        {
            eqScale = _eq;
            basedHor = dHor = (orth[1] - orth[0]) / 2;
            basedVert = dVert = (orth[3] - orth[2]) / 2;
            baseCenterHor = centerHor = (orth[0] + orth[1]) / 2;
            baseCenterVert = centerVert = (orth[2] + orth[3]) / 2;
            double hzbuf = (orth[5] - orth[4]) / 5;
            zBufmin = orth[4] - hzbuf;
            zBufmax = orth[5] + hzbuf;
        }
        public void ClearView()
        {
            dHor = basedHor;
            dVert = basedVert;
            centerHor = baseCenterHor;
            centerVert = baseCenterVert;
        }
        public bool DoubleAxis { get { return doubleAxis; } set { doubleAxis = value; } }
        public string HorAxisName { get { return nameHorAxis; } set { nameHorAxis = value; } }
        public string HorAxisName2 { get { return nameHorAxis2; } set { nameHorAxis2 = value; } }
        public string VertAxisName { get { return nameVertAxis; } set { nameVertAxis = value; } }
        public double GetDHor { get { return 2 * dHor; } }
        public double GetDVert { get { return 2 * dVert; } }
        public void ConvertWorldToScreenCoord(int Width, int Height, double resX, double resY, out int eX, out int eY, int indentX = 0, int indentY = 0)
        {
            double dx = resX - (centerHor - dHor);
            double dy = resY - (centerVert - dVert);

            double coef;

            coef = dx / (2 * dHor);
            eX = (int)(coef * Width + indentX);

            coef = dy / (2 * dVert);
            eY = Height - (int)(coef * Height);
        }
        public void ConvertScreenToWorldCoord(int Width, int Height, int eX, int eY, out double resX, out double resY, int indentX = 0, int indentY = 0)
        {
            double coef;
            if (eX < indentX)
                resX = centerHor - dHor;
            else
                if (eX > Width + indentX)
                resX = centerHor + dHor;
            else
            {
                coef = (eX - indentX) / (double)Width;
                resX = centerHor + (2 * coef - 1) * dHor;
            }
            if (eY < 0)
                resY = centerVert + dVert;
            else
                if (eY > Height)
                resY = centerVert - dVert;
            else
            {
                coef = (Height - eY) / (double)Height;
                resY = centerVert + (2 * coef - 1) * dVert;
            }
        }
        public void ConvertScreenToWorldCoordAddition(int Width, int eX, out double resX, int indentX = 0)
        {
            double coef;
            if (eX < indentX)
                resX = additionalOrth[0];
            else
                if (eX > Width + indentX)
                resX = additionalOrth[1];
            else
            {
                coef = (eX - indentX) / (double)Width;
                resX = additionalOrth[0] + (additionalOrth[1] - additionalOrth[0])*coef;
            }
           
        }
        public double GetMinHor { get { return centerHor - dHor; } }
        public double GetMaxHor { get { return centerHor + dHor; } }
        public double GetMinVert { get { return centerVert - dVert; } }
        public double GetMaxVert { get { return centerVert + dVert; } }
        public double GetMinZBuf { get { return zBufmin; } }
        public double GetMaxZBuf { get { return zBufmax; } }
        public void GetOrtho(out double[] orth)
        {
            orth = new double[8];
            if (AdditionalonJob)
            {
                orth[6] = GetMinHor;
                orth[7] = orth[0] + 2 * dHor;
                orth[0] = additionalOrth[0];
                orth[1] = additionalOrth[1];
            }
            else
            {
                orth[0] = GetMinHor;
                orth[1] = orth[0] + 2 * dHor;
                orth[6] = additionalOrth[0];
                orth[7] = additionalOrth[1];
            }
           
            orth[2] = GetMinVert;
            orth[3] = orth[2] + 2 * dVert;
            
        }
        public void SetZBuffer(double minz, double maxz)
        {
            double hzbuf = (maxz - minz) / 5;
            zBufmin = minz - hzbuf;
            zBufmax = maxz + hzbuf;
        }
        public void SetOrtho(double[] orth)
        {
            basedHor = dHor = (orth[1] - orth[0]) / 2;
            basedVert = dVert = (orth[3] - orth[2]) / 2;
            baseCenterHor = centerHor = (orth[0] + orth[1]) / 2;
            baseCenterVert = centerVert = (orth[2] + orth[3]) / 2;
            double hzbuf = (orth[5] - orth[4]) / 5;
            zBufmin = orth[4] - hzbuf;
            zBufmax = orth[5] + hzbuf;

            if (orth.Length == 8)
            {
                if (orth[7] - orth[6] < 1e-30)
                {
                    orth[7] += 1;
                    orth[6] -= 1;
                }
                additionalOrth[0] = orth[6];
                additionalOrth[1] = orth[7];
            }
            else
            {
                additionalOrth[0] = -1.0;
                additionalOrth[1] = 1.0;
            }
        }
        public void SetOrtho(double[] orth, bool x, bool y)
        {
            if (orth[1] - orth[0] < 1e-30)
            {
                orth[1] += 1;
                orth[0] -= 1;
            }
            if (orth[3] - orth[2] < 1e-30)
            {
                orth[3] += 1;
                orth[2] -= 1;
            }
            if (orth[5] - orth[4] < 1e-30)
            {
                orth[5] += 1;
                orth[4] -= 1;
            }
            
            if (x) basedHor = dHor = (orth[1] - orth[0]) / 2;
            if (y) basedVert = dVert = (orth[3] - orth[2]) / 2;
            if (x) baseCenterHor = centerHor = (orth[0] + orth[1]) / 2;
            if (y) baseCenterVert = centerVert = (orth[2] + orth[3]) / 2;
            double hzbuf = (orth[5] - orth[4]) / 5;
            zBufmin = orth[4] - hzbuf;
            zBufmax = orth[5] + hzbuf;

            if (orth.Length == 8)
            {
                if (orth[7] - orth[6] < 1e-30)
                {
                    orth[7] += 1;
                    orth[6] -= 1;
                }
                additionalOrth[0] = orth[6];
                additionalOrth[1] = orth[7];
            }
            else
            {
                additionalOrth[0] = -1.0;
                additionalOrth[1] = 1.0;
            }
        }
        public void Translate(double hor, double vert)
        {
            centerHor += hor;
            centerVert += vert;
        }
        public void Translate(double hor, double vert, double hor2)
        {
            centerHor += hor;
            centerVert += vert;
            additionalOrth[0] += hor2;
            additionalOrth[1] += hor2;
        }
        public void Scale(double hor, double vert, double delt)
        {
            double scl = delt < 1.05 ? 1.05 : 1.0 / 1.05;
            double centerXNew, centerYNew, dHorNew, dVertNew;
            double minHor = hor + scl * (GetMinHor - hor);
            double maxHor = hor + scl * (GetMinHor + 2 * dHor - hor);
            centerXNew = (minHor + maxHor) / 2;
            double minVert = vert + scl * (GetMinVert - vert);
            double maxVert = vert + scl * (GetMinVert + 2 * dVert - vert);
            centerYNew = (minVert + maxVert) / 2;
            dHorNew = centerXNew - minHor;
            dVertNew = centerYNew - minVert;
            if (Math.Abs(2 * dHorNew) > Math.Max(Math.Abs(centerXNew - dHor), Math.Abs(centerXNew + dHor)) * 10E-5 &&
                Math.Abs(2 * dVertNew) > Math.Max(Math.Abs(centerYNew - dVert), Math.Abs(centerYNew + dVert)) * 10E-5)
            {
                centerHor = centerXNew;
                centerVert = centerYNew;
                dHor = dHorNew;
                dVert = dVertNew;

                baseCenterHor = centerHor;
                baseCenterVert = centerVert;
                basedHor = dHor;
                basedVert = dVert;
            }
        }
        public void Scale(double hor, double vert, double x2, double delt)
        {
            double[] o1, o2;

            GetOrtho(out o1);
            Scale(hor, vert, delt);
            GetOrtho(out o2);

            double d, c, dNew, cNew;
            d = (o2[1] - o2[0]) / (o1[1] - o1[0]);
            c = ((o2[1] + o2[0]) * 0.5 - o1[0]) / (o1[1] - o1[0]);

            dNew = d * (AdditionalOrth[1] - AdditionalOrth[0]) * 0.5;
            cNew = AdditionalOrth[0] + (AdditionalOrth[1] - AdditionalOrth[0]) * c;
            AdditionalOrth[0] = cNew - dNew;
            AdditionalOrth[1] = cNew + dNew;
        }
    }
}
