/*
 Файл содержит классы:
 * 
 * LittleTools - статический класс с различными методами, выполняющими функцию мелких утилит
 * для работы с файлами, директорями, обработок логических, геометрически, линейной алгебры и т.д.
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using GLContex = Geology.OpenGL.OpenGL;



namespace Geology.Utilities
{
    public static class EInversionAreaBackgroundUsingAndEMaterialParameterConverter
    {
    }
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> inner;
        public ReverseComparer() : this(null) { }
        public ReverseComparer(IComparer<T> inner)
        {
            this.inner = inner ?? Comparer<T>.Default;
        }
        int IComparer<T>.Compare(T x, T y) { return inner.Compare(y, x); }
    }

    public class Pair<T1, T2> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public T1 first;
        public T2 second;

        public T1 First
        {
            get { return first; }
            set { first = value; OnPropertyChanged("First"); }
        }
        public T2 Second
        {
            get { return second; }
            set { second = value; OnPropertyChanged("Second"); }
        }
        public Pair()
        {
            first = default(T1);
            second = default(T2);
        }
        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
    public class Trio<T1, T2, T3>
    {
        public T1 first;
        public T2 second;
        public T3 third;

        public Trio()
        {
            first = default(T1);
            second = default(T2);
            third = default(T3);
        }
        public Trio(T1 first, T2 second, T3 third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }
    }

    public class Vector3
    {
        private double x, y, z;

        public double X
        {
            set { /*OnPropertyChanged(x, value, "X");*/  x = value; }
            get { return x; }
        }
        public double Y
        {
            set { /*OnPropertyChanged(y, value, "Y");*/ y = value; }
            get { return y; }
        }
        public double Z
        {
            set { /*OnPropertyChanged(z, value, "Z");*/ z = value; }
            get { return z; }
        }
        public Vector3(Vector3 vector3 = null)
        {
            if (vector3 == null)
                x = y = z = 0.0;
            else
            {
                x = vector3.X;
                y = vector3.Y;
                z = vector3.Z;

                Set(vector3.X, vector3.Y, vector3.Z);
            }
        }
        public Vector3(double x, double y, double z)
        {
            Set(x, y, z);
        }
        public void Set(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void Set(Vector3 vector3)
        {
            x = vector3.X;
            y = vector3.Y;
            z = vector3.Z;
        }

        public double Norm()
        {
            return Math.Sqrt(x*x + y*y + z*z);
        }
        public void Normalize()
        {
            double norm = Norm();

            if (norm < 1e-14)
                return;

            x /= norm;
            y /= norm;
            z /= norm;
        }
        public void Add(Vector3 v, ref Vector3 res)
        {
            res.X = x + v.X;
            res.Y = y + v.Y;
            res.Z = z + v.Z;
        }
        public Vector3 Add(Vector3 v)
        {
            return (new Vector3(x + v.X, y + v.Y, z + v.Z));
        }
        public void Sub(Vector3 v, ref Vector3 res)
        {
            res.X = x - v.X;
            res.Y = y - v.Y;
            res.Z = z - v.Z;
        }
        public Vector3 Sub(Vector3 v)
        {
            return (new Vector3(x - v.X, y - v.Y, z - v.Z));
        }
        public double ScalarMult(Vector3 v)
        {
            return (x*v.X + y*v.Y + z*v.Z);
        }
        public Vector3 VectorMult(Vector3 v)
        {
            Vector3 r = new Vector3();

            r.X = Y * v.Z - Z * v.Y;
            r.Y = Z * v.X - X * v.Z;
            r.Z = X * v.Y - Y * v.X;

            return r;
        }
        public void Scale(double c)
        {
            x *= c;
            y *= c;
            z *= c;
        }
        public static double Colinear(Vector3 v1, Vector3 v2)
        {
            return Math.Sqrt(v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
        }
        public double Colinear(Vector3 v)
        {
            return Math.Sqrt(x * v.X + y * v.Y + z * v.Z);
        }
        public double Distance(Vector3 v)
        {
            return Math.Sqrt((x - v.X) * (x - v.X) + (y - v.Y) * (y - v.Y) + (z - v.Z) * (z - v.Z));
        }
        public double DistanceSqr(Vector3 v)
        {
            return (x - v.X) * (x - v.X) + (y - v.Y) * (y - v.Y) + (z - v.Z) * (z - v.Z);
        }


        //================================================================================ Read/Write

        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                outputFile.WriteLine(x.ToString());
                outputFile.WriteLine(y.ToString());
                outputFile.WriteLine(z.ToString());

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public int Read(ref StreamReader inputFile, double fileVersion)
        {
            try
            {
                int n;
                String buffer;
                if (fileVersion >= 1.0)
                {

                    buffer = inputFile.ReadLine(); if (buffer != null) x = double.Parse(buffer);
                    buffer = inputFile.ReadLine(); if (buffer != null) y = double.Parse(buffer);
                    buffer = inputFile.ReadLine(); if (buffer != null) z = double.Parse(buffer);
                }


                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public int WriteB(BinaryWriter outputFile)
        {
            try
            {
                outputFile.Write(x);
                outputFile.Write(y);
                outputFile.Write(z);

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public int ReadB(BinaryReader inputFile, double fileVersion)
        {
            try
            {
                int n;
                String buffer;
                if (fileVersion >= 1.0)
                {

                    x = inputFile.ReadDouble();
                    y = inputFile.ReadDouble();
                    z = inputFile.ReadDouble();
                }


                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class Vector3Observable : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        double x, y, z;
        int number;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public double X
        {
            set { x = value; OnPropertyChanged("X");}
            get { return x; }
        }
        public double Y
        {
            set { y = value; OnPropertyChanged("Y");}
            get { return y; }
        }
        public double Z
        {
            set { z = value; OnPropertyChanged("Z");}
            get { return z; }
        }
        public int Number
        {
            set { number = value; OnPropertyChanged("Number"); }
            get { return number; }
        }
        public Vector3Observable()
        {
            x = y = z = 0.0;
            number = 0;
        }
        public Vector3Observable(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            number = 0;
        }

        public double Norm()
        {
            return Math.Sqrt(x*x + y*y + z*z);
        }
        public void Normalize()
        {
            double norm = Norm();

            if (norm < 1e-14)
                return;

            x /= norm;
            y /= norm;
            z /= norm;
        }
        public void Add(Vector3Observable v, ref Vector3Observable res)
        {
            res.X = x + v.X;
            res.Y = y + v.Y;
            res.Z = z + v.Z;
        }
        public Vector3Observable Add(Vector3Observable v)
        {
            return (new Vector3Observable(x + v.X, y + v.Y, z + v.Z));
        }
        public void Sub(Vector3Observable v, ref Vector3Observable res)
        {
            res.X = x - v.X;
            res.Y = y - v.Y;
            res.Z = z - v.Z;
        }
        public Vector3Observable Sub(Vector3Observable v)
        {
            return (new Vector3Observable(x - v.X, y - v.Y, z - v.Z));
        }
        public double ScalarMult(Vector3Observable v)
        {
            return (x*v.X + y*v.Y + z*v.Z);
        }
        public void Scale(double c)
        {
            x *= c;
            y *= c;
            z *= c;
        }
    }
    public class DoubleListElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        double val;
        int number;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public double Value
        {
            set { val = value; OnPropertyChanged("Value"); }
            get { return val; }
        }
        public int Number
        {
            set { number = value; OnPropertyChanged("Number"); }
            get { return number; }
        }
        public DoubleListElement()
        {
            Value = 0.0;
            number = 0;
        }
        public DoubleListElement(int number, double value)
        {
            this.number = number;
            this.val = value;
        }
    }

    public class Plane
    {
        public Vector3 pivot;
        public Vector3 normal;

        public void ProjectPoint(double x, double y, double z, out double projectionX, out double projectionY, out double projectionZ)
        {
            if (normal.Norm() < 1e-14)
            {
                projectionX = projectionY = projectionZ = 0.0;
                return;
            }

            double a = -(pivot.X*normal.X + pivot.Y*normal.Y + pivot.Z*normal.Z);
            double t = -(a + normal.X * x + normal.Y * y + normal.Z * z) / (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            projectionX = normal.X * t + x;
            projectionY = normal.Y * t + y;
            projectionZ = normal.Z * t + z;
        }
        public Vector3 ProjectPoint(Vector3 point)
        {
            double x, y, z;
            ProjectPoint(point.X, point.Y, point.Z, out x, out y, out z);
            Vector3 projection = new Vector3(x, y, z);
            return projection;
        }

        public bool PointIsInsideHalfSpace(double x, double y, double z)
        {
            if (normal.Norm() < 1e-14)
                return false;

            double a = -(pivot.X * normal.X + pivot.Y * normal.Y + pivot.Z * normal.Z);
            double t = -(a + normal.X * x + normal.Y * y + normal.Z * z) / (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            if (t < 0.0)
                return false;
            return true;
        }
        public bool PointIsInsideHalfSpace(Vector3 point)
        {
            return PointIsInsideHalfSpace(point.X, point.Y, point.Z);
        }
        public bool IntrcectsLine(Vector3 point, Vector3 direction, out Vector3 intersectionPoint)
        {
            intersectionPoint = new Vector3();
            if (normal.Norm() < 1e-14)
                return false;

            double d = normal.ScalarMult(direction);
            if (Math.Abs(d) < 1e-14)
                return false;

            double a = -(pivot.X * normal.X + pivot.Y * normal.Y + pivot.Z * normal.Z);
            double t = -(a + normal.ScalarMult(point)) / d;
            intersectionPoint.X = direction.X * t + point.X;
            intersectionPoint.Y = direction.Y * t + point.Y;
            intersectionPoint.Z = direction.Z * t + point.Z;
            return true;
        }
        public int IntrcectsLineSegment(Vector3 point1, Vector3 point2, out Vector3 intersectionPoint)
        {
            bool flag = PointIsInsideHalfSpace(point1);
            if (flag == PointIsInsideHalfSpace(point2))
            {
                intersectionPoint = new Vector3();
                if (flag == true)
                    return -1;
                else
                    return 1;
            }


            Vector3 direction = point2.Sub(point1);
            direction.Normalize();
            flag = IntrcectsLine(point1, direction, out intersectionPoint);
            if (flag == true)
                return 0;
            else
                return 666;
        }
        public Plane()
        {
            pivot = new Vector3(0.0, 0.0, 0.0);
            normal = new Vector3(0.0, 0.0, 1.0);
        }
    }
    static public class LittleTools
    {
        static Random random = new Random();
        //========================================================= Logic

        static public float IBMFloatToIEEE(BitArray bits)
        {
            int exp = 0;
            double mant = 0.0f;
            bool sign;

            sign = bits[31];
            if (bits[24]) exp += 1;
            if (bits[25]) exp += 2;
            if (bits[26]) exp += 4;
            if (bits[27]) exp += 8;
            if (bits[28]) exp += 16;
            if (bits[29]) exp += 32;
            if (bits[30]) exp += 64;
            exp -= 64;

            if (bits[23]) mant += 5.00000000000000E-01;
            if (bits[22]) mant += 2.50000000000000E-01;
            if (bits[21]) mant += 1.25000000000000E-01;
            if (bits[20]) mant += 6.25000000000000E-02;
            if (bits[19]) mant += 3.12500000000000E-02;
            if (bits[18]) mant += 1.56250000000000E-02;
            if (bits[17]) mant += 7.81250000000000E-03;
            if (bits[16]) mant += 3.90625000000000E-03;
            if (bits[15]) mant += 1.95312500000000E-03;
            if (bits[14]) mant += 9.76562500000000E-04;
            if (bits[13]) mant += 4.88281250000000E-04;
            if (bits[12]) mant += 2.44140625000000E-04;
            if (bits[11]) mant += 1.22070312500000E-04;
            if (bits[10]) mant += 6.10351562500000E-05;
            if (bits[9]) mant += 3.05175781250000E-05;
            if (bits[8]) mant += 1.52587890625000E-05;
            if (bits[7]) mant += 7.62939453125000E-06;
            if (bits[6]) mant += 3.81469726562500E-06;
            if (bits[5]) mant += 1.90734863281250E-06;
            if (bits[4]) mant += 9.53674316406250E-07;
            if (bits[3]) mant += 4.76837158203125E-07;
            if (bits[2]) mant += 2.38418579101562E-07;
            if (bits[1]) mant += 1.19209289550781E-07;
            if (bits[0]) mant += 5.96046447753906E-08;

            mant = Math.Pow(16.0, exp) * mant;

            return (float)(sign ? -mant : mant);
        }
        static public int IEEEFloatToIBM(float from)
        {
            byte[] bytes = BitConverter.GetBytes(from);
            int fconv = (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];

            if (fconv == 0) return 0;
            int fmant = (0x007fffff & fconv) | 0x00800000;
            int t = (int)((0x7f800000 & fconv) >> 23) - 126;
            while (0 != (t & 0x3)) { ++t; fmant >>= 1; }
            fconv = (int)(0x80000000 & fconv) | (((t >> 2) + 64) << 24) | fmant;
            return fconv; // big endian order
        }
        static public void Swap<T>(ref T a, ref T b)
        {
            T tmp;
            tmp = a;
            a = b;
            b = tmp;
        }

        public static int ParseString(String str, ref double variable)
        {
            try
            {
                variable = double.Parse(str);
                return 0;
            }
            catch (Exception ex)
            {
                str = variable.ToString();
                return 1;
            }
        }

        public static int ParseString(String str, ref int variable)
        {
            try
            {
                variable = int.Parse(str);
                return 0;
            }
            catch (Exception ex)
            {
                str = variable.ToString();
                return 1;
            }
        }

        public static void CheckMinMax(ref int minValue, ref int maxValue, int value)
        {
            if (minValue > value) minValue = value;
            if (maxValue < value) maxValue = value;
        }
        public static void CheckMinMax(ref float minValue, ref float maxValue, float value)
        {
            if (minValue > value) minValue = value;
            if (maxValue < value) maxValue = value;
        }
        public static void CheckMinMax(ref double minValue, ref double maxValue, double value)
        {
            if (minValue > value) minValue = value;
            if (maxValue < value) maxValue = value;
        }
        
        public static bool CompareNumbers2x2(int a1, int a2, int b1, int b2)
        {
            if (a1 == b1)
                if (a2 == b2)
                    return true;
                else
                    return false;
            else if (a1 == b2)
                if (a2 == b1)
                    return true;
                else
                    return false;

            return false;
        }
        public static bool CompareNumbers3x3(int a1, int a2, int a3, int b1, int b2, int b3)
        {
            if (a1 == b1)
                return CompareNumbers2x2(a2, a3, b2, b3);
            else if (a1 == b2)
                return CompareNumbers2x2(a2, a3, b1, b3);
            else if (a1 == b3)
                return CompareNumbers2x2(a2, a3, b1, b2);
            return false;
        }
        public static bool CompareNumbers4x4(int a1, int a2, int a3, int a4, int b1, int b2, int b3, int b4)
        {
            try
            {
                if (a1 == b1)
                    return CompareNumbers3x3(a2, a3, a4, b2, b3, b4);
                else if (a1 == b2)
                    return CompareNumbers3x3(a2, a3, a4, b1, b3, b4);
                else if (a1 == b3)
                    return CompareNumbers3x3(a2, a3, a4, b1, b2, b4);
                else if (a1 == b4)
                    return CompareNumbers3x3(a2, a3, a4, b1, b2, b3);

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //========================================================= Math

        static public double GetCurve(IList<double> arguments, IList<double> values, double x)
        {
            double d;
            int interval = FindIntervalInSorted(arguments, x);
            if (interval < 0 || interval > arguments.Count - 2)
                return 0.0;
            d = (x - arguments[interval]) / (arguments[interval + 1] - arguments[interval]);
            return (1.0 - d) * values[interval] + d * values[interval + 1];
        }
        static public double IntegrateSegment(double x1, double x2, double v1, double v2)
        {
            double dx = x2 - x1;
            if (dx < 1e-10)
                return 0.0;

            if (v1 * v2 >= 0.0)
            {
                if (Math.Abs(v1) > Math.Abs(v2)) Swap(v1, v2);

                return dx * v1 + 0.5 * dx * (v2 - v1);
            }
            else
            {
                double d = Math.Abs(v1) / Math.Abs(v2);
                return dx * d * v1 + dx * (1.0 - d) * v2;
            }
        }

        static public double IntegrateCurve(IList<double> arguments, IList<double> values, double t0, double t1)
        {
            if (t1 <= t0)
                return 0.0;
            double r = 0.0;
            int i0 = FindIntervalInSorted(arguments, t0); if (i0 < 0) i0 = 0; if (i0 > arguments.Count - 2) i0 = arguments.Count - 2;
            int i1 = FindIntervalInSorted(arguments, t1); if (i1 < 0) i1 = 0; if (i1 > arguments.Count - 2) i1 = arguments.Count - 2;

            r += IntegrateSegment(t0, arguments[i0 + 1], GetCurve(arguments, values, t0), values[i0 + 1]);
            r += IntegrateSegment(arguments[i1], t1, values[i1], GetCurve(arguments, values, t1));

            for (int i = i0 + 1; i < i1; i++)
                r += IntegrateSegment(arguments[i], arguments[i + 1], values[i], values[i + 1]);

            return r;
        }
        static public bool Intersect1D(double a1, double a2, double b1, double b2)
        {
            return (Math.Max(a2, b2) - Math.Min(a1, b1) < (a2 - a1) + (b2 - b1));
        }
        static public bool Intersect1D(double[] a, double[] b)
        {
            return Intersect1D(a[0], a[1], b[0], b[1]);
        }
        static public bool Intersect1D(double a1, double a2, double b1, double b2, double i1, double i2)
        {
            bool intersected = false;
            double x1, x2, x3, x4, d1, d2, d;

            if (a1 > a2)
                Swap(ref a1, ref a2);

            if (b1 > b2)
                Swap(ref b1, ref b2);

            if (a1 < b1)
            {
                x1 = a1;
                x2 = b1;
            }
            else
            {
                x1 = b1;
                x2 = a1;
            }

            if (a2 < b2)
            {
                x3 = a2;
                x4 = b2;
            }
            else
            {
                x3 = b2;
                x4 = a2;
            }

            d = x4 - x1;
            d1 = a2 - a1;
            d2 = b2 - b1;

            if (d - a2 - a1 < 0.0)
            {
                i1 = x2;
                i2 = x3;
                return true;
            }
            else
            {
                i1 = 0.0;
                i2 = 0.0;
                return false;
            }
        }
        static public bool Intersect2D(double a1, double a2, double a3, double a4, double b1, double b2, double b3, double b4)
        {
            return (Intersect1D(a1, a2, b1, b2) && Intersect1D(a3, a4, b3, b4));
        }
        static public bool Intersect2D(double[] a, double[] b)
        {
            return (Intersect1D(a[0], a[1], b[0], b[1]) && Intersect1D(a[2], a[3], b[2], b[3]));
        }
        static public bool Intersect3D(double a1, double a2, double a3, double a4, double a5, double a6, double b1, double b2, double b3, double b4, double b5, double b6)
        {
            return (Intersect1D(a1, a2, b1, b2) && Intersect1D(a3, a4, b3, b4) && Intersect1D(a5, a6, b5, b6));
        }
        static public bool Intersect3D(double[] a, double[] b)
        {
            return (Intersect1D(a[0], a[1], b[0], b[1]) && Intersect1D(a[2], a[3], b[2], b[3]) && Intersect1D(a[4], a[5], b[4], b[5]));
        }
        static public bool PointInBox1D(double a, double b, double p)
        {
            return (p >= a && p <= b);
        }
        static public bool PointInBox2D(double a1, double a2, double a3, double a4, double p1, double p2)
        {
            if (PointInBox1D(a1, a2, p1) == true)
                return PointInBox1D(a3, a4, p2);

            return false;
        }
        static public bool PointInBox3D(double a1, double a2, double a3, double a4, double a5, double a6, double p1, double p2, double p3)
        {
            if (PointInBox1D(a1, a2, p1) == true)
                if (PointInBox1D(a3, a4, p2) == true)
                    return PointInBox1D(a5, a6, p3);
            return false;
        }
        static public bool BoxAInBoxB1D(double a1, double a2, double b1, double b2)
        {
            double eps;

            eps = (b2 - b1) * 1e-6;
            if (a1 < b1 - eps || a1 > b2 + eps)
                return false;
            if (a2 < b1 - eps || a2 > b2 + eps)
                return false;

            return true;
        }
        static public bool BoxAInBoxB2D(double a1, double a2, double a3, double a4, double b1, double b2, double b3, double b4)
        {
            double eps;

            eps = (b2 - b1) * 1e-6;
            if (a1 < b1 - eps || a1 > b2 + eps)
                return false;
            if (a2 < b1 - eps || a2 > b2 + eps)
                return false;

            eps = (b4 - b3) * 1e-6;
            if (a3 < b3 - eps || a3 > b4 + eps)
                return false;
            if (a4 < b3 - eps || a4 > b4 + eps)
                return false;

            return true;
        }
        static public bool BoxAInBoxB3D(double a1, double a2, double a3, double a4, double a5, double a6, double b1, double b2, double b3, double b4, double b5, double b6)
        {
            double eps;

            eps = (b2 - b1) * 1e-6;
            if (a1 < b1 - eps || a1 > b2 + eps)
                return false;
            if (a2 < b1 - eps || a2 > b2 + eps)
                return false;

            eps = (b4 - b3) * 1e-6;
            if (a3 < b3 - eps || a3 > b4 + eps)
                return false;
            if (a4 < b3 - eps || a4 > b4 + eps)
                return false;

            eps = (b6 - b5) * 1e-6;
            if (a5 < b5 - eps || a5 > b6 + eps)
                return false;
            if (a6 < b5 - eps || a6 > b6 + eps)
                return false;

            return true;
        }
        static public bool PointAInBoxB3D(double a1, double a2, double a3, double b1, double b2, double b3, double b4, double b5, double b6)
        {
            double eps;

            eps = (b2 - b1) * 1e-6;
            if (a1 < b1 - eps || a1 > b2 + eps)
                return false;

            eps = (b4 - b3) * 1e-6;
            if (a2 < b3 - eps || a2 > b4 + eps)
                return false;

            eps = (b6 - b5) * 1e-6;
            if (a3 < b5 - eps || a3 > b6 + eps)
                return false;

            return true;
        }
        static public double SegmentLength(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0) + (z1 - z0) * (z1 - z0));
        }
        static public void SegmentDirection(double x0, double y0, double z0, double x1, double y1, double z1, ref double dx, ref double dy, ref double dz)
        {
            double length = SegmentLength(x0, y0, z0, x1, y1, z1);
            dx = (x1 - x0) / length;
            dy = (y1 - y0) / length;
            dz = (z1 - z0) / length;
        }
        static public double AngleCosX(double xV1, double yV1)
        {
            double cos;
            cos = xV1 * 1.0;
            return cos;
        }
        static public double AngleSinX(double xV1, double yV1)
        {
            double cos, sin;
            cos = xV1 * 1.0;
            if (yV1 < 0.0)
                sin = -Math.Sqrt(1.0 - cos * cos);
            else
                sin =  Math.Sqrt(1.0 - cos * cos);
            return sin;
        }
        static public double AngleCos(double xV1, double yV1, double zV1, double xV2, double yV2, double zV2)
        {
            double cos;
            cos = xV1 * xV2 + yV1 * yV2 + zV1 * zV2;
            return cos;
        }
        static public double AngleSin(double xV1, double yV1, double zV1, double xV2, double yV2, double zV2)
        {
            double cos;
            cos = xV1 * xV2 + yV1 * yV2 + zV1 * zV2;
            return Math.Sqrt(1.0 - cos*cos);
        }
        static public void ShiftPoint(ref double x, ref double y, ref double z, double dx, double dy, double dz)
        {
            x += dx;
            y += dy;
            z += dz;
        }
        static public void ShiftPoint(Vector3 p, double dx, double dy, double dz)
        {
            p.X += dx;
            p.Y += dy;
            p.Z += dz;
        }
        static public void ShiftPoint(Vector3 p, Vector3 shift)
        {
            p.X += shift.X;
            p.Y += shift.Y;
            p.Z += shift.Z;
        }
        static public void RotatePoint(ref double x, ref double y, ref double z, double cosa, double sina)
        {
            double tmpx, tmpy;
            tmpx = x;
            tmpy = y;
            x = tmpx * cosa - tmpy * sina;
            y = tmpy * cosa + tmpx * sina;
        }
        static public void RotatePoint(ref double x, ref double y, double cosa, double sina)
        {
            double tmpx, tmpy;
            tmpx = x;
            tmpy = y;
            x = tmpx * cosa - tmpy * sina;
            y = tmpy * cosa + tmpx * sina;
        }
        static public void RotatePoint(double x, double y, double cosa, double sina, out double xr, out double yr)
        {
            xr = x * cosa - y * sina;
            yr = y * cosa + x * sina;
        }
        static public void RotatePointAroundPoint(ref double x, ref double y, ref double z, double cosa, double sina, double x0, double y0, double z0)
        {
            double tmpx, tmpy, tmpz;
            ShiftPoint(ref x, ref y, ref z, -x0, -y0, -z0);
            tmpx = x * cosa - y * sina;
            tmpy = y * cosa + x * sina;
            tmpz = z;
            ShiftPoint(ref tmpx, ref tmpy, ref tmpz, x0, y0, z0);
            x = tmpx;
            y = tmpy;
            z = tmpz;
        }
        static public void RotatePointAroundPoint(Vector3 p, double cosa, double sina, double x0, double y0, double z0)
        {
            double tmpx, tmpy, tmpz;
            ShiftPoint(p, -x0, -y0, -z0);
            tmpx = p.X * cosa - p.Y * sina;
            tmpy = p.Y * cosa + p.X * sina;
            tmpz = p.Z;
            ShiftPoint(ref tmpx, ref tmpy, ref tmpz, x0, y0, z0);
            p.X = tmpx;
            p.Y = tmpy;
            p.Z = tmpz;
        }
        static public void RotatePointAroundPoint(Vector3 p, double cosa, double sina, Vector3 pc)
        {
            double tmpx, tmpy, tmpz;
            ShiftPoint(p, -pc.X, -pc.Y, -pc.Z);
            tmpx = p.X * cosa - p.Y * sina;
            tmpy = p.Y * cosa + p.X * sina;
            tmpz = p.Z;
            ShiftPoint(ref tmpx, ref tmpy, ref tmpz, pc.X, pc.Y, pc.Z);
            p.X = tmpx;
            p.Y = tmpy;
            p.Z = tmpz;
        }

        
        static public void RotatePointAroundX(ref double x, ref double y, ref double z, double cosa, double sina)
        {
            double tmpy, tmpz;
            tmpy = y;
            tmpz = z;
            y = tmpy * cosa - tmpz * sina;
            z = tmpz * cosa + tmpy * sina;
        }
        static public void RotatePointAroundY(ref double x, ref double y, ref double z, double cosa, double sina)
        {
            double tmpx, tmpz;
            tmpx = x;
            tmpz = z;
            x = tmpx * cosa - tmpz * sina;
            z = tmpz * cosa + tmpx * sina;
        }
        static public void RotatePointAroundZ(ref double x, ref double y, ref double z, double cosa, double sina)
        {
            double tmpx, tmpy;
            tmpx = x;
            tmpy = y;
            x = tmpx * cosa - tmpy * sina;
            y = tmpy * cosa + tmpx * sina;
        }
        static public void RotatePointToVector(ref double x, ref double y, ref double z, Vector3 vector)
        {
            RotatePointAroundY(ref x, ref y, ref z, Math.Sqrt(vector.X*vector.X + vector.Y*vector.Y), vector.Z);
            Vector3 p = new Vector3(vector);
            p.Z = 0;
            p.Normalize();
            RotatePointAroundZ(ref x, ref y, ref z, p.X, p.Y);
        }
        static public void NormalizeVector(ref double x, ref double y, ref double z)
        {
            double d = Math.Sqrt(x*x + y*y + z*z);

            x /= d;
            y /= d;
            z /= d;
        }
        static public void NormalizeVector(ref double x, ref double y)
        {
            double d = Math.Sqrt(x * x + y * y);

            x /= d;
            y /= d;
        }
        static public void NormalizeVector(ref float x, ref float y)
        {
            float d = (float)Math.Sqrt(x * x + y * y);

            x /= d;
            y /= d;
        }
        static public void StepByVectorLocal2D(double vX, double vY, double stepLocalX, double stepLocalY, double originX, double originY, ref double x, ref double y, bool vectorIsNormalized)
        {
            if (vectorIsNormalized == false)
                NormalizeVector(ref vX, ref vY);

            double dx = stepLocalX, dy = stepLocalY;

            x = originX + dx * vX + dy * vY;
            y = originY - dx * vY + dy * vX;
        }
        static public List<double> GenerateSimpleMesh(double v1, double v2, int count)
        {
            try
            {
                if (count < 1)
                    return null;

                List<double> mesh = new List<double>();

                if (count == 1)
                {
                    mesh.Add(v1);
                    return mesh;
                }

                double step = (v2 - v1) / (count-1);

                for (int i = 0; i < count; i++)
                    mesh.Add(v1 + i * step);

                return mesh;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static public double CalculateDistance3D(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
        }
        static public double CalculateDistance2D(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        static public double CalculateDistance2DSqr(double x1, double y1, double x2, double y2)
        {
            return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
        }
        static public void MakeAnalogRectangleLoopFromCircle(double x, double y, double z, double r, double[] sqr)
        {
            double a = Math.Sqrt(Math.PI * r * r) * 0.5;
            sqr[0] = x - a;
            sqr[1] = y - a;
            sqr[2] = z;

            sqr[3] = x + a;
            sqr[4] = y - a;
            sqr[5] = z;

            sqr[6] = x + a;
            sqr[7] = y + a;
            sqr[8] = z;

            sqr[9] = x - a;
            sqr[10] = y + a;
            sqr[11] = z;
        }
        static public double TriangleMeasure(double a, double b, double c)
        {
            double p, a1, a2, a3;
            p = 0.5 * (a + b + c);
            a1 = p - a; if (a1 < 0.0) return 0.0;
            a2 = p - b; if (a2 < 0.0) return 0.0;
            a3 = p - c; if (a3 < 0.0) return 0.0;
            return Math.Sqrt(p * a1 * a2 * a3);
        }
        static public double TriangleMasure(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return TriangleMeasure(p1.Distance(p2), p3.Distance(p2), p1.Distance(p3));
        }
        static public void DetermineBarycentricCoordinates(double x1, double y1, double x2, double y2, double x3, double y3, double px, double py, out double lc1, out double lc2, out double lc3)
        {
            double d1, d2, d3, dd1, dd2, dd3, s, s1, s2, s3;

            d1 = CalculateDistance2D(x1, y1, x2, y2);
            d2 = CalculateDistance2D(x3, y3, x2, y2);
            d3 = CalculateDistance2D(x1, y1, x3, y3);

            dd1 = CalculateDistance2D(x1, y1, px, py);
            dd2 = CalculateDistance2D(x2, y2, px, py);
            dd3 = CalculateDistance2D(x3, y3, px, py);

            s = TriangleMeasure(d1, d2, d3);
            s1 = TriangleMeasure(d1, dd1, dd2);
            s2 = TriangleMeasure(d2, dd2, dd3);
            s3 = TriangleMeasure(d3, dd3, dd1);

            lc1 = s2 / s;
            lc2 = s3 / s;
            lc3 = s1 / s;
        }
        static public bool PointInTriangle2D(double x1, double y1, double x2, double y2, double x3, double y3, double px, double py)
        {
            double lc1, lc2, lc3;

            DetermineBarycentricCoordinates(x1, y1, x2, y2, x3, y3, px, py, out lc1, out lc2, out lc3);
            if (Math.Abs(lc1 + lc2 + lc3 - 1.0) < 1e-2)
                return true;

            return false;
        }
        static public bool PointInQuadrangle2D(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double px, double py)
        {
            if (PointInTriangle2D(x1, y1, x2, y2, x3, y3, px, py) == true) return true;
            if (PointInTriangle2D(x2, y2, x3, y3, x4, y4, px, py) == true) return true;

            return false;
        }
        static public void CalculatePointInPolygon2D(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, double u, double v, Vector3 p)
        {
            if (u < 0.0) u = 0.0;
            if (u > 1.0) u = 1.0;
            if (v < 0.0) v = 0.0;
            if (v > 1.0) v = 1.0;

            p.X = (1.0 - u) * (1.0 - v) * p1.X + u * (1.0 - v) * p2.X + u * v * p3.X + (1.0 - u) * v * p4.X;
            p.Y = (1.0 - u) * (1.0 - v) * p1.Y + u * (1.0 - v) * p2.Y + u * v * p3.Y + (1.0 - u) * v * p4.Y;
        }
        static public void CalculatePointInPolygon2D(ref double u, ref double v, double[] p, out double x, out double y)
        {
            if (u < 0.0) u = 0.0;
            if (u > 1.0) u = 1.0;
            if (v < 0.0) v = 0.0;
            if (v > 1.0) v = 1.0;

            int i;
            i = 0; x = (1.0 - u) * (1.0 - v) * p[i] + u * (1.0 - v) * p[i + 2] + u * v * p[i + 4] + (1.0 - u) * v * p[i + 6];
            i = 1; y = (1.0 - u) * (1.0 - v) * p[i] + u * (1.0 - v) * p[i + 2] + u * v * p[i + 4] + (1.0 - u) * v * p[i + 6];
        }
        static public void CalculatePointInHexagon3D(ref double u, ref double v, ref double w, double[] p, out double x, out double y, out double z)
        {
            if (u < 0.0) u = 0.0;
            if (u > 1.0) u = 1.0;
            if (v < 0.0) v = 0.0;
            if (v > 1.0) v = 1.0;
            if (w < 0.0) w = 0.0;
            if (w > 1.0) w = 1.0;

            int i;
            i = 0;
            x = (1.0 - u) * (1.0 - v) * (1.0 - w) * p[i] +
                (u) * (1.0 - v) * (1.0 - w) * p[i + 3] +
                (1.0 - u) * (v) * (1.0 - w) * p[i + 6] +
                (u) * (v) * (1.0 - w) * p[i + 9] +
                (1.0 - u) * (1.0 - v) * (w) * p[i + 12] +
                (u) * (1.0 - v) * (w) * p[i + 15] +
                (1.0 - u) * (v) * (w) * p[i + 18] +
                (u) * (v) * (w) * p[i + 21];
            i = 1;
            y = (1.0 - u) * (1.0 - v) * (1.0 - w) * p[i] +
                (u) * (1.0 - v) * (1.0 - w) * p[i + 3] +
                (1.0 - u) * (v) * (1.0 - w) * p[i + 6] +
                (u) * (v) * (1.0 - w) * p[i + 9] +
                (1.0 - u) * (1.0 - v) * (w) * p[i + 12] +
                (u) * (1.0 - v) * (w) * p[i + 15] +
                (1.0 - u) * (v) * (w) * p[i + 18] +
                (u) * (v) * (w) * p[i + 21];
            i = 2;
            z = (1.0 - u) * (1.0 - v) * (1.0 - w) * p[i] +
                (u) * (1.0 - v) * (1.0 - w) * p[i + 3] +
                (1.0 - u) * (v) * (1.0 - w) * p[i + 6] +
                (u) * (v) * (1.0 - w) * p[i + 9] +
                (1.0 - u) * (1.0 - v) * (w) * p[i + 12] +
                (u) * (1.0 - v) * (w) * p[i + 15] +
                (1.0 - u) * (v) * (w) * p[i + 18] +
                (u) * (v) * (w) * p[i + 21];
        }
        static public void LocalCoordinatesInPolygon2D(double x1, double x2, double x3, double x4, double y1, double y2, double y3, double y4, double x, double y, out double ksi, out double eta)
        {
            int i, j, pmin, pminPrevious;
            double step, um = 0.0, vm = 0.0, dmin, d, cx, cy;
            double[] p = new double[8], u = new double[4], v = new double[4], pn = new double[8];

            u[0] = 0.0; v[0] = 0.0;
            u[1] = 1.0; v[1] = 0.0;
            u[2] = 1.0; v[2] = 1.0;
            u[3] = 0.0; v[3] = 1.0;

            p[0] = pn[0] = x1;
            p[1] = pn[1] = y1;
            p[2] = pn[2] = x2;
            p[3] = pn[3] = y2;
            p[4] = pn[4] = x3;
            p[5] = pn[5] = y3;
            p[6] = pn[6] = x4;
            p[7] = pn[7] = y4;

            step = 1.0;
            pminPrevious = -1;
            for (i = 0; i < 100; i++)
            {
                dmin = CalculateDistance2D(pn[0], pn[1], x, y);
                pmin = 0;

                for (j = 1; j < 4; j++)
                {
                    d = CalculateDistance2D(pn[j * 2], pn[j * 2 + 1], x, y);
                    if (dmin > d)
                    {
                        dmin = d;
                        pmin = j;
                    }
                }

                cx = pn[pmin * 2];
                cy = pn[pmin * 2 + 1];

                step *= 0.8;
                pminPrevious = pmin;

                um = u[pmin];
                vm = v[pmin];
                u[0] = um - step; v[0] = vm - step;
                u[1] = um + step; v[1] = vm - step;
                u[2] = um + step; v[2] = vm + step;
                u[3] = um - step; v[3] = vm + step;

                for (j = 0; j < 4; j++)
                    CalculatePointInPolygon2D(ref u[j], ref v[j], p, out pn[j * 2], out pn[j * 2 + 1]);
            }

            ksi = um;
            eta = vm;

        }
        static public void LocalCoordinatesInHexagon3D(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5, Vector3 p6, Vector3 p7, Vector3 p8, Vector3 point, out double ksi, out double eta, out double zeta)
        {
            int i, j, pmin, pminPrevious;
            double step, um = 0.0, vm = 0.0, wm = 0.0, dmin, d, cx, cy, cz;
            double[] p = new double[24], u = new double[8], v = new double[8], w = new double[8], pn = new double[24];

            u[0] = 0.0; v[0] = 0.0; w[0] = 0.0;
            u[1] = 1.0; v[1] = 0.0; w[1] = 0.0;
            u[2] = 0.0; v[2] = 1.0; w[2] = 0.0;
            u[3] = 1.0; v[3] = 1.0; w[3] = 0.0;
            u[4] = 0.0; v[4] = 0.0; w[4] = 1.0;
            u[5] = 1.0; v[5] = 0.0; w[5] = 1.0;
            u[6] = 0.0; v[6] = 1.0; w[6] = 1.0;
            u[7] = 1.0; v[7] = 1.0; w[7] = 1.0;

            p[0] = pn[0] = p1.X;
            p[1] = pn[1] = p1.Y;
            p[2] = pn[2] = p1.Z;
            p[3] = pn[3] = p2.X;
            p[4] = pn[4] = p2.Y;
            p[5] = pn[5] = p2.Z;
            p[6] = pn[6] = p3.X;
            p[7] = pn[7] = p3.Y;
            p[8] = pn[8] = p3.Z;
            p[9] = pn[9] = p4.X;
            p[10] = pn[10] = p4.Y;
            p[11] = pn[11] = p4.Z;
            p[12] = pn[12] = p5.X;
            p[13] = pn[13] = p5.Y;
            p[14] = pn[14] = p5.Z;
            p[15] = pn[15] = p6.X;
            p[16] = pn[16] = p6.Y;
            p[17] = pn[17] = p6.Z;
            p[18] = pn[18] = p7.X;
            p[19] = pn[19] = p7.Y;
            p[20] = pn[20] = p7.Z;
            p[21] = pn[21] = p8.X;
            p[22] = pn[22] = p8.Y;
            p[23] = pn[23] = p8.Z;

            step = 1.0;
            pminPrevious = -1;
            for (i = 0; i < 100; i++)
            {
                dmin = CalculateDistance3D(pn[0], pn[1], pn[2], point.X, point.Y, point.Z);
                pmin = 0;

                for (j = 1; j < 8; j++)
                {
                    d = CalculateDistance3D(pn[j * 3], pn[j * 3 + 1], pn[j * 3 + 2], point.X, point.Y, point.Z);
                    if (dmin > d)
                    {
                        dmin = d;
                        pmin = j;
                    }
                }

                cx = pn[pmin * 3];
                cy = pn[pmin * 3 + 1];
                cz = pn[pmin * 3 + 2];

                step *= 0.8;
                pminPrevious = pmin;

                um = u[pmin];
                vm = v[pmin];
                wm = w[pmin];
                u[0] = um - step; v[0] = vm - step; w[0] = wm - step;
                u[1] = um + step; v[1] = vm - step; w[1] = wm - step;
                u[2] = um - step; v[2] = vm + step; w[2] = wm - step;
                u[3] = um + step; v[3] = vm + step; w[3] = wm - step;
                u[4] = um - step; v[4] = vm - step; w[4] = wm + step;
                u[5] = um + step; v[5] = vm - step; w[5] = wm + step;
                u[6] = um - step; v[6] = vm + step; w[6] = wm + step;
                u[7] = um + step; v[7] = vm + step; w[7] = wm + step;

                for (j = 0; j < 8; j++)
                    CalculatePointInHexagon3D(ref u[j], ref v[j], ref w[j], p, out pn[j * 3], out pn[j * 3 + 1], out pn[j * 3 + 2]);
            }

            ksi = um;
            eta = vm;
            zeta = wm;

        }
        static public double DistanceFromPointToLineSegment(double x1, double x2, double y1, double y2, double x, double y)
        {
            double d, t;
            double dx, dy, nx, ny, cx, cy;

            dx = x2 - x1;
            dy = y2 - y1;
            nx = dy;
            ny = -dx;
            t = (y1 * nx - y * nx - x1 * ny + x * ny) / (dx * ny - dy * nx);
            if (t > 1.0)
            {
                d = CalculateDistance2D(x1, y1, x, y);
                t = CalculateDistance2D(x2, y2, x, y);
                return Math.Min(d, t);
            }

            cx = t * dx + x1;
            cy = t * dy + y1;
            return CalculateDistance2D(cx, cy, x, y);
        }
        static public void PointHorizontalProjectionOnLineSegment(double x1, double y1, double z1, double x2, double y2, double z2, double z, out double xr, out double yr, out double zr)
        {
            double d, t;

            if (Math.Abs(z2 - z1) < 1e-14)
            {
                xr = z1;
                yr = y1;
                zr = z1;
                return;
            }

            d = (z - z1) / (z2 - z1);
            xr = x1 * (1.0 - d) + x2 * d;
            yr = y1 * (1.0 - d) + y2 * d;
            zr = z1 * (1.0 - d) + z2 * d;
        }
        static public void PointProjectionToLineSegment(double x1, double x2, double y1, double y2, double x, double y, out double cx, out double cy)
        {
            double d, t;
            double dx, dy, nx, ny;

            dx = x2 - x1;
            dy = y2 - y1;
            nx = dy;
            ny = -dx;
            t = (y1 * nx - y * nx - x1 * ny + x * ny) / (dx * ny - dy * nx);
            if (t > 1.0)
            {
                d = CalculateDistance2D(x1, y1, x, y);
                t = CalculateDistance2D(x2, y2, x, y);
                if (d < t)
                {
                    cx = x1;
                    cy = y1;
                }
                else
                {
                    cx = x2;
                    cy = y2;
                }
                return;
            }

            cx = t * dx + x1;
            cy = t * dy + y1;
        }
        static public double PointProjectionToLineSegment(double x1, double x2, double y1, double y2, double x, double y)
        {
            double dx, dy, nx, ny;

            dx = x2 - x1;
            dy = y2 - y1;
            nx = dy;
            ny = -dx;
            return (y1 * nx - y * nx - x1 * ny + x * ny) / (dx * ny - dy * nx);
            
        }
        static public void ScaleBounds(ref double x0, ref double x1, double scale, double trans = 0.0)
        {
            double d;

            d = (x1 - x0) * scale + trans;
            x0 -= d;
            x1 += d;


        }
        static public void ScaleBounds(ref double x0, ref double x1, ref double y0, ref double y1, double scale, double trans = 0.0)
        {
            ScaleBounds(ref x0, ref x1, scale, trans);
            ScaleBounds(ref y0, ref y1, scale, trans);
        }
        static public void ScaleBounds(ref double x0, ref double x1, ref double y0, ref double y1, ref double z0, ref double z1, double scale, double trans = 0.0)
        {
            ScaleBounds(ref x0, ref x1, scale, trans);
            ScaleBounds(ref y0, ref y1, scale, trans);
            ScaleBounds(ref z0, ref z1, scale, trans);
        }

        static public void RoundNumber(ref double number, double eps)
        {
            if (eps > 0.0)
            {
                double d = number % eps;
                if (Math.Abs(d) < eps * 0.5)
                    number = number - d;
                else
                    number = number - d + eps * Math.Sign(d);
            }

        }
        static public void RoundNumberToGrid(ref double number, double eps)
        {
            if (eps > 0.0)
            {
                int i = (int)(number / eps);
                if (Math.Abs(i * eps - number) < Math.Abs((i-1) * eps - number))
                    if (Math.Abs(i * eps - number) < Math.Abs((i + 1) * eps - number))
                    number = eps * i;
                else
                    if (Math.Abs((i+1) * eps - number) < Math.Abs((i - 1) * eps - number))
                        number = eps * (i+1);
                else
                        number = eps * (i-1);
            }

        }

        static public bool HLineIntersect(Vector3 p1, Vector3 p2, double y, out double x)
        {
            double t;
            if (Math.Abs(p1.Y - p2.Y) < 1e-10)
            {
                x = 0.0;
                return false;
            }

            if (y < Math.Min(p1.Y, p2.Y) || y > Math.Max(p1.Y, p2.Y))
            {
                x = 0.0;
                return false;
            }

            t = (y - p1.Y) / (p2.Y - p1.Y);
            x = p1.X + (p2.X - p1.X) * t;
            return true;
        }
        static public bool VLineIntersect(Vector3 p1, Vector3 p2, double x, out double y)
        {
            double t;
            if (Math.Abs(p1.X - p2.X) < 1e-10)
            {
                y = -1.0;
                return false;
            }

            if (x < Math.Min(p1.X, p2.X) || x > Math.Max(p1.X, p2.X))
            {
                y = -1.0;
                return false;
            }

            t = (x - p1.X) / (p2.X - p1.X);
            y = p1.Y + (p2.Y - p1.Y) * t;
            return true;
        }
        static public bool HLineIntersect(Vector3 p1, Vector3 p2, double y, double x0, double x1, out double x)
        {
            double t;
            if (Math.Abs(p1.Y - p2.Y) < 1e-10)
            {
                x = 0.0;
                return false;
            }

            if (y < Math.Min(p1.Y, p2.Y) || y > Math.Max(p1.Y, p2.Y))
            {
                x = 0.0;
                return false;
            }

            t = (y - p1.Y) / (p2.Y - p1.Y);
            x = p1.X + (p2.X - p1.X) * t;
            if (x < x0 || x > x1)
                return false;
            return true;
        }
        static public bool VLineIntersect(Vector3 p1, Vector3 p2, double x, double y0, double y1, out double y)
        {
            double t;
            if (Math.Abs(p1.X - p2.X) < 1e-10)
            {
                y = -1.0;
                return false;
            }

            if (x < Math.Min(p1.X, p2.X) || x > Math.Max(p1.X, p2.X))
            {
                y = -1.0;
                return false;
            }

            t = (x - p1.X) / (p2.X - p1.X);
            y = p1.Y + (p2.Y - p1.Y) * t;
            if (y < y0 || y > y1)
                return false;
            return true;
        }
        static public bool HLineIntersectT(Vector3 p1, Vector3 p2, double y, double x0, double x1, out double t)
        {
            if (Math.Abs(p1.Y - p2.Y) < 1e-10)
            {
                t = 0.0;
                return false;
            }

            if (y < Math.Min(p1.Y, p2.Y) || y > Math.Max(p1.Y, p2.Y))
            {
                t = 0.0;
                return false;
            }

            t = (y - p1.Y) / (p2.Y - p1.Y);
            double x = p1.X + (p2.X - p1.X) * t;
            if (x < x0 || x > x1)
                return false;
            return true;
        }
        static public bool VLineIntersectT(Vector3 p1, Vector3 p2, double x, double y0, double y1, out double t)
        {
            if (Math.Abs(p1.X - p2.X) < 1e-10)
            {
                t = -1.0;
                return false;
            }

            if (x < Math.Min(p1.X, p2.X) || x > Math.Max(p1.X, p2.X))
            {
                t = -1.0;
                return false;
            }

            t = (x - p1.X) / (p2.X - p1.X);
            double y = p1.Y + (p2.Y - p1.Y) * t;
            if (y < y0 || y > y1)
                return false;
            return true;
        }

        //========================================================= Arrays
        static public object FindMin<T>(IList<T> objects, String propertyName)
        {
            try
            {
                if (objects.Count == 0)
                    return null;

                int rIndex = 0;
                var prop = (typeof(T)).GetProperty(propertyName);
                IComparable r = prop.GetValue(objects[0]) as IComparable;
                for (int i = 1; i < objects.Count; i++)
                    if (r.CompareTo(prop.GetValue(objects[i]) as IComparable) > 0)
                    {
                        rIndex = i;
                        r = prop.GetValue(objects[i]) as IComparable;
                    }

                return objects[rIndex];
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        static public object FindMax<T>(IList<T> objects, String propertyName)
        {
            try
            {
                if (objects.Count == 0)
                    return null;

                int rIndex = 0;
                var prop = (typeof(T)).GetProperty(propertyName);
                IComparable r = prop.GetValue(objects[0]) as IComparable;
                for (int i = 1; i < objects.Count; i++)
                    if (r.CompareTo(prop.GetValue(objects[i]) as IComparable) < 0)
                    {
                        rIndex = i;
                        r = prop.GetValue(objects[i]) as IComparable;
                    }

                return objects[rIndex];
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        static public int FindClosestInSorted(List<double> arr, double val)
        {
            int i;
            for (i = 0; i < arr.Count && val > arr[i]; i++) ;
            if (i == 0)
                return 0;
            if (i == arr.Count)
                return i-1;

            return Math.Abs(arr[i] - val) < Math.Abs(arr[i-1] - val) ? i : i-1;
        }
        static public int FindClosestInSorted(double[] arr, double val)
        {
            int i;
            for (i = 0; i < arr.Length && val > arr[i]; i++) ;
            if (i == 0)
                return 0;
            if (i == arr.Length)
                return i - 1;

            return Math.Abs(arr[i] - val) < Math.Abs(arr[i - 1] - val) ? i : i - 1;
        }
        static public int FindIntervalInSorted(double[] arr, double val)
        {
            int i;
            for (i = 0; i < arr.Length && val > arr[i]; i++);
            return i != 0 ? i-1: i;
        }
        static public int FindIntervalInSorted<T>(IList<T> arr, T val)  where T : IComparable
        {
            int i;
            for (i = 0; i < arr.Count() && val.CompareTo(arr[i]) == 1; i++) ;
            return i != 0 ? i - 1 : i;
        }
        static public int FindIntervalInSorted(double[] arr, double val, out double ksi)
        {
            if (val <= arr[0])
            {
                ksi = 0.0;
                return 0;
            }

            if (val >= arr.Last())
            {
                ksi = 1.0;
                return arr.Length-2;
            }

            int i;
            for (i = 0; i < arr.Length && val > arr[i]; i++) ;
            i = i != 0 ? i - 1 : i;
            ksi = (val - arr[i]) / (arr[i + 1] - arr[i]);
            return i;
        }
        static public int FindIntervalInSorted(float[] arr, float val, out double ksi)
        {
            if (val <= arr[0])
            {
                ksi = 0.0;
                return 0;
            }

            if (val >= arr.Last())
            {
                ksi = 1.0;
                return arr.Length - 2;
            }

            int i;
            for (i = 0; i < arr.Length && val > arr[i]; i++) ;
            i = i != 0 ? i - 1 : i;
            ksi = (val - arr[i]) / (arr[i + 1] - arr[i]);
            return i;
        }
        static public int FindIntervalInSorted(List<double> arr, double val, out double ksi)
        {
            if (val <= arr[0])
            {
                ksi = 0.0;
                return 0;
            }

            if (val >= arr.Last())
            {
                ksi = 1.0;
                return arr.Count - 2;
            }

            int i;
            for (i = 0; i < arr.Count && val > arr[i]; i++) ;
            i = i != 0 ? i - 1 : i;
            ksi = (val - arr[i]) / (arr[i + 1] - arr[i]);
            return i;
        }
        static public void FindMinMax(double[] arr, Int64 size, out double min, out double max)
        {
            min = max = 0.0;

            if (arr.Length < 1)
                return;

            min = max = arr[0];
            for (Int64 i =1; i<size; i++)
            {
                if (min > arr[i]) min = arr[i];
                if (max < arr[i]) max = arr[i];
            }
        }
        static public void FindMinMax(float[] arr, Int64 size, out float min, out float max)
        {
            min = max = 0.0f;

            if (arr.Length < 1)
                return;

            min = max = arr[0];
            for (Int64 i = 1; i < size; i++)
            {
                if (min > arr[i]) min = arr[i];
                if (max < arr[i]) max = arr[i];
            }
        }
        static public void FindMinMax(int[] arr, Int64 size, out int min, out int max)
        {
            min = max = 0;

            if (arr.Length < 1)
                return;

            min = max = arr[0];
            for (Int64 i = 1; i < size; i++)
            {
                if (min > arr[i]) min = arr[i];
                if (max < arr[i]) max = arr[i];
            }
        }
        static public int PointInInterval1D(double[] coords, int size, double point)
        {
            if (size < 2)
                return -1;
            if (coords[0] > point)
                return -1;
            if (coords[size - 1] < point)
                return -1;
            for(int i=0; i<size; i++)
                if (coords[i] > point)
                {
                    if (i > 0)
                        i--;
                    return i;
                }
            return -1;
        }
        static public int PointInInterval1D(double[] coords, double point)
        {
            if (coords.Length < 2)
                return -1;
            if (coords[0] > point)
                return -1;
            if (coords[coords.Length - 1] < point)
                return -1;
            for (int i = 0; i < coords.Length; i++)
                if (coords[i] > point)
                {
                    if (i > 0)
                        i--;
                    return i;
                }
            return -1;
        }
        static public int PointInInterval1D(List<double> coords, double point)
        {
            if (coords.Count < 2)
                return -1;
            if (coords[0] > point)
                return -1;
            if (coords[coords.Count - 1] < point)
                return -1;
            for (int i = 0; i < coords.Count; i++)
                if (coords[i] > point)
                {
                    if (i > 0)
                        i--;
                    return i;
                }
            return -1;
        }
        static public int PointInArray1D(double[] coords, double point)
        {
            if (coords.Length < 1)
                return -1;
            if (coords[0] > point)
                return 0;
            if (coords[coords.Length - 1] < point)
                return coords.Length - 1;
            for (int i = 0; i < coords.Length; i++)
                if (coords[i] > point)
                {
                    if (Math.Abs(coords[i] - point) > Math.Abs(coords[i - 1] - point))
                        return i - 1;
                    else
                        return i;
                }
            return coords.Length - 1;
        }
        static public int PointInArray1D(List<double> coords, double point)
        {
            if (coords.Count < 1)
                return -1;
            if (coords[0] > point)
                return 0;
            if (coords[coords.Count - 1] < point)
                return coords.Count - 1;
            for (int i = 0; i < coords.Count; i++)
                if (coords[i] > point)
                {
                    if (Math.Abs(coords[i] - point) > Math.Abs(coords[i - 1] - point))
                        return i - 1;
                    else
                        return i;
                }

            return coords.Count - 1;
        }
        static public void UniqueArray(List<double> arr, double eps)
        {
            for (int i = 0; i < arr.Count - 1; i++)
            {
                if (arr[i + 1] - arr[i] < eps)
                {
                    arr.RemoveAt(i + 1);
                    i--;
                }
            }
        }
        static public void СondenseMesh(double step, List<double> coords)
        {
            if (Math.Abs(step) < 1e-15)
                return;

            int stepCount;
            double h;
            for (int i = 0; i < coords.Count - 1; i++)
            {
                h = coords[i + 1] - coords[i];
                if (h > step)
                {
                    stepCount = (int)(0.5 + h / step);

                    if (stepCount != 0)
                        h = h / stepCount;
                    for (int j = 1; j < stepCount; j++)
                        coords.Insert(i + j, coords[i] + h * j);
                }
            }
        }
        static public void Build1DMeshFromZero(double step, double b0, double b1, out double[] mesh)
        {
            int n0 = (int)(b0 / step); if (Math.Abs(n0 * step - b0) > 1e-3) n0--;
            int n1 = (int)(b1 / step); if (Math.Abs(n1 * step - b1) > 1e-3) n1++;

            mesh = new double[n1 - n0 + 1];

            int n = 0;
            for (int i = n0; i <= n1; i++, n++)
                mesh[n] = i * step;
        }
        static public void Build1DMeshSparse(double step, double b0, double b1, double sparse, out double[] mesh)
        {
            List<double> m = new List<double>();
            double c = b0;

            while(step < 0 && c > b1 || step > 0 && c < b1)
            {
                m.Add(c);
                c += step;
                step *= sparse;
            }

            m.Add(c);
            m.Sort();
            mesh = m.ToArray();
        }
        static public void Build1DMeshFromZero(double step, double b0, double b1, double ib0, double ib1, double sparse, out double[] mesh)
        {
            double[] internalMesh;
            Build1DMeshFromZero(step, ib0, ib1, out internalMesh);

            double[] meshL, meshR;
            Build1DMeshSparse(-step, internalMesh.First(), b0, sparse, out meshL);
            Build1DMeshSparse(step, internalMesh.Last(), b1, sparse, out meshR);

            mesh = meshL.Concat(internalMesh).Concat(meshR).Distinct().ToArray();
        }

        static public void Swap<T>(ObservableCollection<T> collection, int index1, int index2)
        {
            if (index1 < 0 || index1 >= collection.Count)
                return;

            if (index2 < 0 || index2 >= collection.Count)
                return;
            T tmp;
            tmp = collection[index1];
            collection[index1] = collection[index2];
            collection[index2] = tmp;
        }
        static public void Swap<T>(T v1, T v2)
        {
            T tmp;
            tmp = v1;
            v1 = v2;
            v2 = tmp;
        }
        static public void Swap(byte[] array, int index1, int index2)
        {
            if (index1 < 0 || index1 >= array.Length)
                return;

            if (index2 < 0 || index2 >= array.Length)
                return;
            byte tmp;
            tmp = array[index1];
            array[index1] = array[index2];
            array[index2] = tmp;
        }

        static public void ClearCollection(IList list)
        {
            while (list.Count > 0)
                list.RemoveAt(list.Count - 1);
        }

        //========================================================= Graphics

        static public void DrawParallelepipedTriangles(double c1, double c2, double c3, double c4, double c5, double c6)
        {
            GLContex.glBegin(GLContex.GL_TRIANGLES);

            //===================================== x0
            GLContex.glNormal3d(-1.0, 0.0, 0.0);
            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c1, c3, c6);
            GLContex.glVertex3d(c1, c4, c6);

            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c1, c4, c5);

            //===================================== x1
            GLContex.glNormal3d(1.0, 0.0, 0.0);
            GLContex.glVertex3d(c2, c4, c6);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c2, c3, c5);

            GLContex.glVertex3d(c2, c4, c5);
            GLContex.glVertex3d(c2, c4, c6);
            GLContex.glVertex3d(c2, c3, c5);

            //===================================== y0
            GLContex.glNormal3d(0.0, -1.0, 0.0);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c1, c3, c6);
            GLContex.glVertex3d(c1, c3, c5);


            GLContex.glVertex3d(c2, c3, c5);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c1, c3, c5);

            //===================================== y1
            GLContex.glNormal3d(0.0, 1.0, 0.0);
            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c2, c4, c6);

            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c2, c4, c6);
            GLContex.glVertex3d(c2, c4, c5);

            //===================================== z0
            GLContex.glNormal3d(0.0, 0.0, -1.0);
            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c2, c4, c5);

            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c2, c4, c5);
            GLContex.glVertex3d(c2, c3, c5);

            //===================================== z1
            GLContex.glNormal3d(0.0, 0.0, 1.0);
            GLContex.glVertex3d(c2, c4, c6);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c1, c3, c6);

            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c2, c4, c6);
            GLContex.glVertex3d(c1, c3, c6);


            GLContex.glEnd();
        }
        static public void DrawParallelepipedTriangles(double [] c)
        {
            DrawParallelepipedTriangles(c[0], c[1], c[2], c[3], c[4], c[5]);
        }
        static public void DrawParallelepipedFrame(double c1, double c2, double c3, double c4, double c5, double c6)
        {
            GLContex.glBegin(GLContex.GL_LINES);

            //===================================== x
            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c2, c3, c5);
            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c2, c4, c5);
            GLContex.glVertex3d(c1, c3, c6);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c2, c4, c6);

            //===================================== y
            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c2, c3, c5);
            GLContex.glVertex3d(c2, c4, c5);
            GLContex.glVertex3d(c1, c3, c6);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c2, c4, c6);

            //===================================== z
            GLContex.glVertex3d(c1, c3, c5);
            GLContex.glVertex3d(c1, c3, c6);
            GLContex.glVertex3d(c2, c3, c5);
            GLContex.glVertex3d(c2, c3, c6);
            GLContex.glVertex3d(c1, c4, c5);
            GLContex.glVertex3d(c1, c4, c6);
            GLContex.glVertex3d(c2, c4, c5);
            GLContex.glVertex3d(c2, c4, c6);

            GLContex.glEnd();
        }
        static public void DrawParallelepipedFrame(double[] c)
        {
            DrawParallelepipedFrame(c[0], c[1], c[2], c[3], c[4], c[5]);
        }

        public static int getRandomValue()
        {
            try
            {
                return random.Next(0, 255);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static Color GetRandomColor()
        {
            try
            {
                return Color.FromRgb((byte)getRandomValue(), (byte)getRandomValue(), (byte)getRandomValue());
            }
            catch (Exception ex)
            {
                return Color.FromRgb(0, 0, 0);
            }
        }
        public static Color InverseColor(Color c)
        {
            try
            {
                return Color.Subtract(Colors.White, c);
            }
            catch (Exception ex)
            {
                return Color.FromRgb(0, 0, 0);
            }
        }

        public static bool ShowColorDialog(ref Color color)
        {
            try
            {
                System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
                cd.Color = System.Drawing.Color.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B);

                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    color.R = cd.Color.R;
                    color.G = cd.Color.G;
                    color.B = cd.Color.B;
                    color.A = cd.Color.A;
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public static bool ShowColorDialog(Color color)
        {
            try
            {
                System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
                cd.Color = System.Drawing.Color.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B);

                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    color.R = cd.Color.R;
                    color.G = cd.Color.G;
                    color.B = cd.Color.B;
                    color.A = cd.Color.A;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //========================================================= Files/Directories
        public static string GetJavaInstallationPath()
        {
            try
            {
                string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                if (!string.IsNullOrEmpty(environmentPath))
                {
                    return environmentPath;
                }

                string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
                using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(javaKey))
                {
                    string currentVersion = rk.GetValue("CurrentVersion").ToString();
                    using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(currentVersion))
                    {
                        return key.GetValue("JavaHome").ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        static public void WriteBytesReverseToBinary(BinaryWriter stream, byte[] bytes)
        {
            for (int i = bytes.Length-1; i >= 0; i--)
                stream.Write(bytes[i]);
        }
        
        static public bool CopyFile(String sourceFileName, String newFileName)
        {
            try
            {
                if (!File.Exists(sourceFileName)) return false;
                File.Copy(sourceFileName, newFileName, true);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        static public int CreateDirectory(String path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public int RemoveDirectory(String path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                    return 0;

                    Directory.Delete(path, true);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public int RemoveFile(String fileName)
        {
            try
            {
                if (File.Exists(fileName) == false)
                    return 0;

                File.Delete(fileName);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public void ClearDirectoryFromFiles(String path, String mask)
        {
            try
            {
                if (Directory.Exists(path) == false)
                    return;

                var fileList = Directory.GetFiles(path, mask);
                foreach (string fileName in fileList)
                    File.Delete(fileName);
            }
            catch(Exception ex)
            {

            }
        }
        static public int CreateCalculationDirectory(String path)
        {
            try
            {
                if (Directory.Exists(path + "/Calculation") == false)
                    Directory.CreateDirectory(path + "/Calculation");
                else
                {
                    Directory.Delete(path + "/Calculation", true);
                    Directory.CreateDirectory(path + "/Calculation");
                }

                if (Directory.Exists(path + "/Calculation/Calculations") == false)
                    Directory.CreateDirectory(path + "/Calculation/Calculations");

                if (Directory.Exists(path + "/Calculation/GroupsData") == false)
                    Directory.CreateDirectory(path + "/Calculation/GroupsData");

                if (Directory.Exists(path + "/Calculation/Results") == false)
                    Directory.CreateDirectory(path + "/Calculation/Results");

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public int CopyDirectoryContent(String sourceDirectory, String destinationDirectory)
        {
            try
            {
                if (Directory.Exists(destinationDirectory) == false)
                    Directory.CreateDirectory(destinationDirectory);

                var fileList = Directory.GetFiles(sourceDirectory, "*");

                foreach (var fileName in fileList)
                    if (File.Exists(destinationDirectory + "/" + Path.GetFileName(fileName)) == false)
                        File.Copy(fileName, destinationDirectory + "/" + Path.GetFileName(fileName), false);

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public int GetPath(ref String path)
        {
            System.Windows.Forms.OpenFileDialog openDirDialog = new System.Windows.Forms.OpenFileDialog();
            openDirDialog.InitialDirectory = path;
            openDirDialog.FileName = "This folder";
            openDirDialog.Filter = "folders|*.neverseenthisfile";
            openDirDialog.FilterIndex = 0;
            openDirDialog.CheckFileExists = false;
            openDirDialog.CheckPathExists = false;
            if (openDirDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = openDirDialog.FileName;
                path = Directory.GetParent(path).ToString();
                return 0;
            }

            return 1;
        }
        static public String GetPath(String lastPath)
        {
            String path = lastPath;
            System.Windows.Forms.OpenFileDialog openDirDialog = new System.Windows.Forms.OpenFileDialog();
            openDirDialog.InitialDirectory = path;
            openDirDialog.FileName = "This folder";
            openDirDialog.Filter = "folders|*.neverseenthisfile";
            openDirDialog.FilterIndex = 0;
            openDirDialog.CheckFileExists = false;
            openDirDialog.CheckPathExists = false;
            if (openDirDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = openDirDialog.FileName;
                path = Directory.GetParent(path).ToString();
                return path;
            }

            return "";
        }
        static public void CopyFiles(String pathFrom, String pathTo, String mask)
        {
            try
            {
                if (Directory.Exists(pathFrom) == false)
                    return;

                if (Directory.Exists(pathTo) == false)
                    Directory.CreateDirectory(pathTo);

                var fileList = Directory.GetFiles(pathFrom, mask);
                foreach (string fileName in fileList)
                    File.Copy(fileName, pathTo + "/" + Path.GetFileName(fileName));
            }
            catch (Exception ex)
            {

            }
        }
        static public int GetSaveFile(ref String path, String filter, String title = "Save as ...")
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.Title = title;
                dialog.InitialDirectory = path;
                dialog.Filter = filter;
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dialog.FileName;
                    return 0;
                }
                return 1;
            }
            catch(Exception ex)
            {
                return 1;
            }
        }
        static public int GetOpenFile(ref String path, String filter, String title = "Open ...")
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Title = title;
                dialog.InitialDirectory = path;
                dialog.Filter = filter;
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dialog.FileName;
                    return 0;
                }

                return 1;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        //========================================================= CommonSettings


        //========================================================= System
        static public string FindHashCode(object o, int hashCode)
        {
            return FindHashCodeImpl(o, hashCode, o.GetType().Name);
        }
        static public string FindHashCodeImpl(object o, int hashCode, string partialPath)
        {
            var type = o.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propValue = property.GetValue(o, null);
                if (propValue.GetHashCode() == hashCode)
                {
                    return partialPath + " > " + property.Name + ".hashcode = " + hashCode;
                }

                var path = FindHashCodeImpl(propValue, hashCode, partialPath + " > " + property.Name);
                if (path != null)
                {
                    return path;
                }
            }
            return null;
        }

        //========================================================= Parsing

        static public void ParsingPrepareLongString(ref String buffer)
        {
            buffer = buffer.Replace('\t', ' ');
            while (buffer.IndexOf("  ") != -1)
                buffer = buffer.Replace("  ", " ");
            buffer = buffer.Trim();
        }
        static public void ParsingGoToNextWord(ref String buffer)
        {
            int ind = buffer.IndexOf(' ');
            if (ind != -1)
            {
                buffer = buffer.Substring(ind + 1, buffer.Length - ind - 1);
                //buffer = buffer.Trim();
            }
            else
                buffer = "";
        }
        static public String ParsingGetWord(String buffer)
        {
            int ind = buffer.IndexOf(' ');
            if (ind != -1)
                return buffer.Substring(0, ind);
            else
                return buffer;
        }
        static public int ParsingDigitIndex(String buffer)
        {
            int index = -1;
            foreach (char c in buffer)
            {
                index++;
                if (Char.IsDigit(c))
                    return index;
            }
            return index;
        }

        static public void ParsingGetWord(String buffer, int lastIndex, int lastLength, out int start, out int length)
        {
            start = lastIndex + lastLength;
            if(buffer[start] == ' ')
                start++;
            int i;
            for (i = start; i < buffer.Length && buffer[i] != ' '; i++) ;
            length = i - start;
        }

        //========================================================= Processes

        static public int ExecuteExe(String pathToExe, String arguments, String workingDirectory, StreamWriter sw = null)
        {
            try
            {
                if (sw != null)
                    sw.WriteLine("Starting exe : " + pathToExe + " Arguments : " + arguments + " Woriking directory : " + workingDirectory);
                var processInfo = new ProcessStartInfo(pathToExe, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory
                };

                Process proc;

                if ((proc = Process.Start(processInfo)) == null)
                {
                    throw new InvalidOperationException("Error in executing " + pathToExe);
                }

                proc.WaitForExit();
                int exitCode = proc.ExitCode;
                proc.Close();

                if (sw != null)
                    sw.WriteLine("Exe has finished with code : " + exitCode);

                return exitCode;
            }
            catch(Exception ex)
            {
                return 1;
            }
        }

        public static void ShowErrorMessage(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowInfoMessage(String message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static void ShowExclamationMessage(String message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        //========================================================= Other

        
    }
}
