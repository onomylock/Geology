using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Utilities;
using System.IO;
using Spline;

namespace Geology.Objects
{
    public class CCurve : INotifyPropertyChanged
    {
        bool threeColumns = false;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        protected List<double> T;
        protected List<double> T2;
        protected List<double> Values;
        String doubleAxisLabel = "Year";
        public CCurve()
        {
            T = new List<double>();
            T2 = new List<double>();
            Values = new List<double>();
        }
        public CCurve(CCurve c)
        {
            T = new List<double>(c.T);
            T2 = new List<double>(c.T2);
            Values = new List<double>(c.Values);
        }
        public List<double> CurveArgs
        {
            get { return T; }
            set { T = value; OnPropertyChanged("CurveArgs"); }
        }
        public List<double> CurveArgs2
        {
            get { return T2; }
            set { T2 = value; OnPropertyChanged("CurveArgs2"); }
        }
        public List<double> CurveValues
        {
            get { return Values; }
            set { Values = value; OnPropertyChanged("CurveValues"); }
        }
        public int CurveSize
        {
            get { return T.Count; }
        }
        public bool ThreeColumns
        {
            get { return threeColumns; }
            set { threeColumns = value; }
        }
        public void Add(String label)
        {
            doubleAxisLabel = label;
        }
        public void Add(double a, double v)
        {
            T.Add(a);
            T2.Add(a);
            Values.Add(v);
        }
        public void Add(double a, double a2, double v)
        {
            threeColumns = true;
            T.Add(a);
            T2.Add(a2);
            Values.Add(v);
        }
        public static double GetLog(double v, double zero)
        {
            double r = Math.Log10((Math.Abs(v) + 1e-100) / zero);
            if (r <= 0)
                return 0;
            else if (v < 0)
                return -r;
            return r;
        }
        public static double GetLinear(double v, double zero)
        {
            if (v == 0) return 0;
            double r = Math.Pow(10f, Math.Abs(v)) * zero - 1e-100;
            return (v < 0 ? -r : r);
        }
        public void GetMinMax(double[] _mm, bool LogT, double zLogT, bool LogV, double zLogV)
        {
            double t, t2, v;
            for (int i = 0; i < T.Count; i++)
            {
                t = (LogT ? GetLog(T[i], zLogT) : T[i]);
                t2 = 0.0;
                v = (LogV ? GetLog(Values[i], zLogV) : Values[i]);
                _mm[0] = Math.Min(_mm[0], t);
                _mm[1] = Math.Max(_mm[1], t);
                _mm[2] = Math.Min(_mm[2], v);
                _mm[3] = Math.Max(_mm[3], v);
                
            }
        }
        public void DrawSecondAxis(FontGeology font, double[] ortho, int height, int width, int indent)
        {
            double dh = ortho[1] - ortho[0];
            double rh = dh / width;
            double dv = ortho[3] - ortho[2];
            double rv = dv / height;
            double bottom = ortho[2];
            double middle = ortho[2] + 5 * rv;
            bottom = middle;
            double top = ortho[2] + 12 * rv;
            double topBound = ortho[2] + (10 + font.FontSize) * rv;
            double c;
            double labelStart = ortho[1] - (font.GetWidthText(doubleAxisLabel) + 5) * rh;
            double labelLimit = labelStart - 5 * rh;
            double leftLabel = 0, rightLabel = 0;

            
            List<double> labels = new List<double>();
            List<String> values = new List<String>();

            GLContex.glColor3d(0.98, 0.98, 0.98);
            GLContex.glBegin(GLContex.GL_QUADS);
            GLContex.glVertex3d(ortho[0] + 2*rh, ortho[2], 1);
            GLContex.glVertex3d(ortho[1], ortho[2], 1);
            GLContex.glVertex3d(ortho[1], topBound, 1);
            GLContex.glVertex3d(ortho[0] + 2 * rh, topBound, 1);
            GLContex.glEnd();


            GLContex.glLineWidth(1);
            GLContex.glBegin(GLContex.GL_LINES);

            GLContex.glColor3d(0.7, 0.7, 0.7);
            GLContex.glVertex3d(ortho[0], topBound, 1);
            GLContex.glVertex3d(ortho[1], topBound, 1);

            GLContex.glColor3b(0, 0, 0);
            GLContex.glVertex3d(ortho[0], middle, 1);
            GLContex.glVertex3d(ortho[1], middle, 1);

            for (int i = 0; i < T2.Count; i++)
            {
                if (i == 0)
                {
                    labels.Add(T[0]);
                    values.Add(T2[0].ToString());
                    if (labelLimit > T[0])
                    {
                        GLContex.glVertex3d(T[0], bottom, 1);
                        GLContex.glVertex3d(T[0], top, 1);
                    }
                }
                else
                {
                    if (T2[i] != T2[i - 1])
                    {
                        c = (T[i] + T[i - 1]) * 0.5;
                        labels.Add(c);
                        values.Add(T2[i].ToString());
                        if (labelLimit > c)
                        {
                            GLContex.glVertex3d(c, bottom, 1);
                            GLContex.glVertex3d(c, top, 1);
                        }
                    }
                }

                if (i == T2.Count-1)
                {
                    labels.Add(T.Last());
                    if (labelLimit > T.Last())
                    {
                        GLContex.glVertex3d(T.Last(), bottom, 1);
                        GLContex.glVertex3d(T.Last(), top, 1);
                    }
                }
            }
            GLContex.glEnd();

            for (int i = 0; i < values.Count; i++)
            {
                c = font.GetWidthText(values[i]);
                leftLabel = (labels[i + 1] + labels[i] - c * rh) * 0.5;
                if (i > 0 && leftLabel < rightLabel + 5 * rh)
                    continue;
                rightLabel = leftLabel + c * rh;

                if (labelLimit > rightLabel)
                    font.PrintText(leftLabel, middle + 4*rv, 1, values[i]);
            }
            font.PrintText(labelStart, middle + 2 * rv, 1, doubleAxisLabel);
        }
        public void Draw(bool LogT, double zLogT, bool LogV, double zLogV,double psX, double psY, CCurveInfo ci, FontGeology font, double[] ortho, int height, int width, int indent)
        {
            GLContex.glDisable(GLContex.GL_DEPTH_TEST);
            
                    
                        GLContex.glLineWidth(3);
                        GLContex.glBegin(GLContex.GL_LINES);
                        for (int i = 0, j = 1; i < CurveSize - 1; i++, j++)
                        {
                            GLContex.glVertex3f((float)(LogT ? GetLog(T[i], zLogT) : T[i]), (float)(LogV ? GetLog(Values[i], zLogV) : Values[i]), 0);
                            GLContex.glVertex3f((float)(LogT ? GetLog(T[j], zLogT) : T[j]), (float)(LogV ? GetLog(Values[j], zLogV) : Values[j]), 0);
                        }
                        GLContex.glEnd();

            //if (ThreeColumns)
            //    DrawSecondAxis(font, ortho, height, width, indent);

            GLContex.glEnable(GLContex.GL_DEPTH_TEST);
        }


        public double GetAt(double arg, out bool found)
        {
            const double eps = 1e-6;
            double wes1, wes2;
            int i;

            found = false;

            if (CurveSize < 1)
                return 0;
            bool reverse = false;
            if (T[0] > T[T.Count - 1]) { reverse = true; T.Reverse(); Values.Reverse(); }
            if (arg < T[0])
                return Values[0];
            for (i = 1; i < CurveSize; i++)
                if (arg > (T[i - 1] - eps) && arg < (T[i] + eps))
                {
                    wes1 = arg - T[i - 1];
                    wes2 = T[i] - arg;
                    found = true;
                    var val1 = (wes1 + wes2) != 0 ? (Values[i - 1] * wes2 + Values[i] * wes1) / (wes1 + wes2) : 0.0;
                    if (reverse) { T.Reverse(); Values.Reverse(); }

                    return val1;
                }
            var val = Values[i - 1];
            if (reverse)
            { T.Reverse(); Values.Reverse(); }

            return val;
        }
        public void Normalize(CCurve c)
        {
            for (int i = 0; i < CurveSize; i++)
            {
                bool found;
                double v = c.GetAt(T[i], out found);
                if (found)
                {
                    try
                    {
                        Values[i] = v != 0 ? 100 * (Values[i] - v) / v : 0.0;
                    }
                    catch (DivideByZeroException e)
                    {
                        T.RemoveAt(i);
                        Values.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        public void Abs()
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] = Math.Abs(Values[i]);
        }
        public void Sign()
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] = -Values[i];
        }
        public void Square()
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] = Values[i] * Values[i];
        }
        public void CutLeft(double t)
        {
            int c = CurveSize, i = T.FindIndex(delegate (double tn) { return tn > t; });
            if (i < c && i > 0)
            {
                T.RemoveRange(0, i);
                Values.RemoveRange(0, i);
            }
        }
        public void CutRight(double t)
        {
            int c = CurveSize, i = T.FindIndex(delegate (double tn) { return tn > t; });
            if (i < c)
            {
                T.RemoveRange(i, c - i);
                Values.RemoveRange(i, c - i);
            }
        }
        public static CCurve operator +(CCurve l, CCurve c)
        {
            CCurve r = new CCurve(l);
            for (int i = 0; i < r.CurveSize; i++)
            {
                bool found;
                double v = c.GetAt(r.T[i], out found);
                if (found)
                    r.Values[i] += v;
            }
            return r;
        }
        public static CCurve operator -(CCurve l, CCurve c)
        {
            CCurve r = new CCurve(l);
            for (int i = 0; i < r.CurveSize; i++)
            {
                bool found;
                double v = c.GetAt(r.T[i], out found);
                if (found)
                    r.Values[i] -= v;
            }
            return r;
        }

        public static CCurve operator *(CCurve l, CCurve c)
        {
            CCurve r = new CCurve(l);
            for (int i = 0; i < r.CurveSize; i++)
            {
                bool found;
                double v = c.GetAt(r.T[i], out found);
                if (found)
                    r.Values[i] *= v;
            }
            return r;
        }
        public static CCurve operator /(CCurve l, CCurve c)
        {
            CCurve r = new CCurve(l);
            for (int i = 0; i < r.CurveSize; i++)
            {
                bool found;
                double v = c.GetAt(r.T[i], out found);
                if (found)
                    r.Values[i] /= v;
            }
            return r;
        }

        public void AddT(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                T[i] += v;
        }
        public void SubT(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                T[i] -= v;
        }
        public void MulT(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                T[i] *= v;
        }
        public void DivT(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                T[i] /= v;
        }
        public void AddV(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] += v;
        }
        public void SubV(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] -= v;
        }
        public void MulV(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] *= v;
        }
        public void DivV(double v)
        {
            for (int i = 0; i < CurveSize; i++)
                Values[i] /= v;
        }
        public void Differentiate()//по параболе
        {
            int i, k, n = CurveSize;
            if (n < 2) return;
            double[] x, f, df;

            x = new double[n];
            f = new double[n];
            df = new double[n];
            double xx, xx_prev, yy;
            bool IsDuplicatesExist = false;

            k = 1;
            x[0] = T[0];
            f[0] = Values[0];
            xx_prev = x[0];

            for (i = 1; i < n; i++)
            {
                xx = T[i];
                yy = Values[i];
                if (xx > xx_prev)
                {
                    x[k] = xx;
                    f[k] = yy;
                    xx_prev = xx;
                    k++;
                }
                else IsDuplicatesExist = true;
            }

            if (IsDuplicatesExist)
            {
                String message = "There are duplicates in curve. It's impossible to differentiate it.";
                return;
            }

            n = k;
            df[0] = (f[1] - f[0]) / (x[1] - x[0]);
            df[n - 1] = (f[n - 1] - f[n - 2]) / (x[n - 1] - x[n - 2]);
            double dt, dt0, dt1, t0, t1, t2, f0, f1, f2;
            t1 = x[0];
            t2 = x[1];
            f1 = f[0];
            f2 = f[1];
            for (i = 1; i < n - 1; i++)
            {
                t0 = t1; t1 = t2; t2 = x[i + 1];
                f0 = f1; f1 = f2; f2 = f[i + 1];
                dt = t2 - t0; dt1 = t1 - t0; dt0 = t2 - t1;
                df[i] = -f0 * dt0 / (dt * dt1) +
                f1 * (dt0 - dt1) / (dt1 * dt0) +
                f2 * dt1 / (dt * dt0);
            }
            for (i = 0; i < n; i++)
            {
                T[i] = x[i];
                Values[i] = df[i];
            }
        }
        public double GetValue(double arg)
        {
            double ksi;
            int interval = LittleTools.FindIntervalInSorted(T, arg, out ksi);

            return Values[interval]*(1.0-ksi) + Values[interval+1]*ksi;

        }
        public double GetDerivative(double arg)
        {
            double ksi;
            int interval = LittleTools.FindIntervalInSorted(T, arg, out ksi);

            return (Values[interval+1] - Values[interval]) / (T[interval+1] - T[interval]);
        }
        public List<double> GetArguments(double arg1, double arg2)
        {
            List<double> args = new List<double>();
            int i;
            for (i = 0; i < T.Count && T[i] <= arg1; i++) ;
            for (; i < T.Count && T[i] < arg2; i++)
                args.Add(T[i]);

            return args;
        }
        public void Smooth()
        {
            CubicSpline cs = new CubicSpline();
            cs.BuildSpline(T.ToArray(), Values.ToArray(), T.Count);
            int tsize = (int)(0.5 * T.Count);
            double tmin = T[0], ht = (T[T.Count - 1] - T[0]) / (tsize - 1);
            T.Clear();
            Values.Clear();
            T = new List<double>(tsize);
            for (int i = 0; i < tsize; i++)
            {
                T.Add(tmin + i * ht);
                Values.Add(cs.Interpolate(T[i]));
            }
        }
        public void Average()
        {
            for (int i = 1; i < Values.Count; i++)
                Values[0] += Values[i];
            Values[0] /= Values.Count;

            for (int i = 1; i < Values.Count; i++)
                Values[i] = Values[0];
        }
        public int WriteForSpline(String fileName, double d0, double d1)
        {
            StreamWriter outputFile = null;
            try
            {
                outputFile = new StreamWriter(fileName, false, Encoding.GetEncoding("Windows-1251"));

                if (outputFile == null)
                    return 1;

                for (int i = 0; i < T.Count; i++)
                {
                    outputFile.WriteLine(T[i] + "\t" + (d0 * 1.00 + d1 * 0.00) + "\t" + Values[i]);
                    outputFile.WriteLine(T[i] + "\t" + (d0 * 0.33 + d1 * 0.67) + "\t" + Values[i]);
                    outputFile.WriteLine(T[i] + "\t" + (d0 * 0.67 + d1 * 0.33) + "\t" + Values[i]);
                    outputFile.WriteLine(T[i] + "\t" + (d0 * 0.00 + d1 * 1.00) + "\t" + Values[i]);
                }

                outputFile.Close();
                return 0;
            }

            catch (Exception ex)
            {
                if (outputFile != null)
                    outputFile.Close();
                return 1;
            }
        }
        internal void BuildSpline()
        {
        }
    }

    public enum ELabelState
    {
        None,
        Filled,
        Frame
    }
    public enum ELabelPlacing
    {
        ByPoints,
        ByLength
    }
    public enum ELabelType
    {
        None,
        Square,
        Triangle,
        Rhombus,
        Circle
    };
    public enum ELineType
    {
        None,
        Solid,
        Dotted,
        Dashed,
        DashDotted
    };
    public enum ECurveObservingInformationTag
    {
        ProfileNumber,
        PositionNumber,
        PositionHashNumber,
        ReceiverNumber,
        GlobalGeneratorNumber,
        ComponentName
    }
}
