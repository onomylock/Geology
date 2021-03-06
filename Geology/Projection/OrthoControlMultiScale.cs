using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geology.DrawWindow
{
    class COrthoControlMultiScale : IOrthoControl
    {
        string nameHorAxis = "";
        string nameHorAxis2 = "";
        string nameVertAxis = "";
        bool doubleAxis = false;
        double[] _orth;
        double[] _baseorth;
        double[] additionalOrth = { -1, 1 };
        double zBufmin;
        double zBufmax;
        double coefHeightToWidth = 1; bool additionOnJob = false;
        public bool AdditionalonJob
        {
            get { return additionOnJob; }
            set { additionOnJob = value; }
        }
        public bool DoubleAxis { get { return doubleAxis; } set { doubleAxis = value; } }
        public double[] AdditionalOrth
        {
            get { return additionalOrth; }
            set { additionalOrth = value; }
        }
        public double CoefHeightToWidth { get { return coefHeightToWidth; } set { coefHeightToWidth = value; } }
        public COrthoControlMultiScale(string _nameHorAxis, string _nameVertAxis, double coefHToW)
        {
            nameHorAxis = _nameHorAxis;
            nameHorAxis2 = _nameHorAxis;
            nameVertAxis = _nameVertAxis;
            coefHeightToWidth = coefHToW;
            _orth = new double[4] { -1, 1, -1, 1 };
            Array.Copy(_orth, _baseorth, 4);
            zBufmin = -1;
            zBufmax = 1;
        }
        public COrthoControlMultiScale(string _nameHorAxis, string _nameVertAxis, double coefHToW, double[] orth)
            : this(_nameHorAxis, _nameVertAxis, coefHToW)
        {

            Array.Copy(orth, _orth, 4);
            double hzbuf = (orth[5] - orth[4]) / 5;
            zBufmin = orth[4] - hzbuf;
            zBufmax = orth[5] + hzbuf;
        }
        public void ClearView()
        {
            Array.Copy(_baseorth, _orth, 4);
        }
        public double GetMinZBuf { get { return zBufmin; } }
        public double GetMaxZBuf { get { return zBufmax; } }
        public string HorAxisName { get { return nameHorAxis; } set { nameHorAxis = value; } }
        public string HorAxisName2 { get { return nameHorAxis2; } set { nameHorAxis2 = value; } }
        public string VertAxisName { get { return nameVertAxis; } set { nameVertAxis = value; } }
        public double GetDHor { get { return _orth[1] - _orth[0]; } }
        public double GetDVert { get { return _orth[3] - _orth[2]; } }
        public double GetMinHor { get { return _orth[0]; } }
        public double GetMaxHor { get { return _orth[1]; } }

        public double GetMinVert { get { return _orth[2]; } }
        public double GetMaxVert { get { return _orth[3]; } }
        public void GetOrtho(out double[] orth)
        {
            orth = new double[4];
            Array.Copy(_orth, orth, 4);
        }
        public void SetOrtho(double[] orth)
        {
            Array.Copy(orth, _orth, 4);
            Array.Copy(_orth, _baseorth, 4);
            double hzbuf = (orth[5] - orth[4]) / 5;
            zBufmin = orth[4] - hzbuf;
            zBufmax = orth[5] + hzbuf;
        }
        public void ConvertWorldToScreenCoord(int Width, int Height, double resX, double resY, out int eX, out int eY, int indentX = 0, int indentY = 0)
        {
            double dx = resX - _orth[0];
            double dy = resY - _orth[2];

            double coef = dx / (_orth[1] - _orth[0]);
            eX = (int)coef * Width + indentX;


            coef = dy / (_orth[3] - _orth[2]);
            eY = Height - (int)coef * Height;
        }
        public void ConvertScreenToWorldCoord(int Width, int Height, int eX, int eY, out double resX, out double resY, int indentX = 0, int indentY = 0)
        {
            double coef;
            if (eX < indentX)
                resX = _orth[0];
            else
                if (eX > Width + indentX)
                resX = _orth[1];
            else
            {
                coef = (eX - indentX) / (double)Width;
                resX = _orth[0] + (_orth[1] - _orth[0]) * coef;
            }
            if (eY < 0)
                resY = _orth[3];
            else
                if (eY > Height)
                resY = _orth[2];
            else
            {
                coef = (Height - eY) / (double)Height;
                resY = _orth[2] + (_orth[3] - _orth[2]) * coef;
            }
        }
        public void Translate(double hor, double vert)
        {
            _orth[0] += hor;
            _orth[1] += hor;
            _orth[2] += vert;
            _orth[3] += vert;
        }
        public void Scale(double hor, double vert, double delt)
        {
            double[] orthoNew = new double[4];
            double scl = delt > 1.05 ? 1.05 : 1.0 / 1.05;
            orthoNew[0] = hor + scl * (_orth[0] - hor);
            orthoNew[1] = hor + scl * (_orth[1] - hor);
            orthoNew[2] = vert + scl * (_orth[2] - vert);
            orthoNew[3] = vert + scl * (_orth[3] - vert);

            if (Math.Abs(orthoNew[1] - orthoNew[0]) > Math.Max(Math.Abs(orthoNew[0]), Math.Abs(orthoNew[1])) * 10E-5 && Math.Abs(orthoNew[3] - orthoNew[2]) > Math.Max(Math.Abs(orthoNew[2]), Math.Abs(orthoNew[3])) * 10E-5)
            {
                Array.Copy(orthoNew, _orth, 4);
            }


        }
    }
}
