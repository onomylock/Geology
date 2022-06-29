/*
 Файл содержит классы:
 * 
 * CParallelep, наследует интерфейс INotifyPropertyChanged - тип для хранения и отображения
 * координат геологических объектов.
 * Наследование INotifyPropertyChanged позволяет контроллерам, привязанным к данным этого типа, 
 * отслеживать изменения в свойствах привязанных объектов и автоматически обновлять отображаемую информацию
 * 
 * 
 *  CGeoObject, наследует интерфейс INotifyPropertyChanged - геологический объект.
 * Объект хранит в себе информацию о геометрических свойствах, физических свойствах и свойствах отображения.
 * Объекты этого типа отображаются при графическом изображении модели, поэтому они имеют метод Draw, который отвечает
 * за графическое отображение объекта этого типа.
 * Наследование INotifyPropertyChanged позволяет контроллерам, привязанным к данным этого типа, 
 * отслеживать изменения в свойствах привязанных объектов и автоматически обновлять отображаемую информацию
 * 
 */
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
using Geology.DrawWindow;
using static Geology.DrawWindow.CObject3DDraw2D;

namespace Geology.Objects
{
    public class CParallelep :INotifyPropertyChanged, ICloneable
    {
        private double _minCoordinate = -2000;
        private double _maxCoordinate = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public double Min
        {
            get { return _minCoordinate; }
            set {  _minCoordinate = value; OnPropertyChanged("Min"); }
        }
        public double Max
        {
            get { return _maxCoordinate; }
            set {  _maxCoordinate = value; OnPropertyChanged("Max"); }
        }
      
        public CParallelep()
        {
        }
        public object Clone()
        {
            CParallelep p = new CParallelep();
            p._minCoordinate = _minCoordinate;
            p._maxCoordinate = _maxCoordinate;

            return p;
        }
        public CParallelep(double newMinCoordinates, double newMaxCoordinates)
         {
             Min = newMinCoordinates;
             Max = newMaxCoordinates;
         }

        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                outputFile.WriteLine(Min.ToString());
                outputFile.WriteLine(Max.ToString());
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
                String buffer;
                if (fileVersion >= 1.0)
                {
                    buffer = inputFile.ReadLine(); _minCoordinate = double.Parse(buffer);
                    buffer = inputFile.ReadLine(); _maxCoordinate = double.Parse(buffer);
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
                outputFile.Write(Min);
                outputFile.Write(Max);
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
                if (fileVersion >= 1.0)
                {
                    _minCoordinate = inputFile.ReadDouble();
                    _maxCoordinate = inputFile.ReadDouble();
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class HexParameter :  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private String name;
        private double value;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public String Name
        {
            get { return name; }
            set {  name = value; OnPropertyChanged("Name"); }
        }
        public double Value
        {
            get { return value; }
            set {  this.value = value; OnPropertyChanged("Value"); }
        }
        public HexParameter()
        {
            name = "";
            value = 0.0;
        }
        public HexParameter(HexParameter hex)
        {
            Name = hex.Name;
            Value = hex.Value;
        }
        public HexParameter(String name, double value)
        {
            this.name = name;
            this.value = value;
        }

        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                outputFile.WriteLine(name);
                outputFile.WriteLine(value.ToString());
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
                CParallelep newParallel;

                if (fileVersion >= 1.0)
                {
                    buffer = inputFile.ReadLine(); Name = buffer;
                    buffer = inputFile.ReadLine(); Value = double.Parse(buffer);
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
                outputFile.Write(name);
                outputFile.Write(value);
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

                if (fileVersion >= 1.0)
                {
                    Name = inputFile.ReadString();
                    Value = inputFile.ReadDouble();
                }


                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class CGeoObject : INotifyPropertyChanged, ICloneable, IViewportObjectsDrawable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Color backgroundColor;
        private Color paletteColor;
        private PhysicalMaterial material = new PhysicalMaterial();
        private string name;
        private int number;
        private bool visible;
        private bool isHex;
        private bool isDeformation;
        private bool selected;
        public int regN1, regN2, regN3, regN4, regN5, regN6;
        public int level;
        public ObservableCollection<HexParameter> hexParameters;
        private bool madeFromContour;
        private String magniteMaterial = "";  // magnite
        private int groupNumber = 1;
        private int groupPriority = 1;
        private int numberContour;

        public ObservableCollection<CParallelep> parallel;

        private void Initialize()
        {
            hexParameters = null;
            isHex = false;
            isDeformation = false;
            backgroundColor = LittleTools.GetRandomColor();
            parallel = new ObservableCollection<CParallelep>() { new CParallelep(), new CParallelep(), new CParallelep() };
            name = "New Object";
            number = 0;
            visible = true;
            selected = false;
            level = 0;
            madeFromContour = false;
            numberContour = 0;
        }
        public CGeoObject()
        {
            Initialize();
        }
        public CGeoObject(CGeoObject obj)
        {
            isHex = obj.IsHex;
            parallel = new ObservableCollection<CParallelep>() {
                new CParallelep(obj.parallel[0].Min, obj.parallel[0].Max),
                new CParallelep(obj.parallel[1].Min, obj.parallel[1].Max),
                new CParallelep(obj.parallel[2].Min, obj.parallel[2].Max) };
            Name = obj.Name;
            Color = obj.Color;
            hexParameters = obj.hexParameters == null ? null : new ObservableCollection<HexParameter>(obj.hexParameters.Select(x => new HexParameter(x)));
            Material = (PhysicalMaterial)obj.Material.Clone();
            Visible = obj.Visible;
            Selected = false;
            IsForDeformation = obj.IsForDeformation;
        }
        public object Clone()
        {
            CGeoObject NewObject = new CGeoObject();
            NewObject.parallel = new ObservableCollection<CParallelep>() {
                (CParallelep)this.parallel[0].Clone(),
                (CParallelep)this.parallel[1].Clone(),
                (CParallelep)this.parallel[2].Clone()};
            NewObject.name = this.name;
            NewObject.backgroundColor = this.backgroundColor;
            NewObject.material = (PhysicalMaterial)this.material.Clone();
            NewObject.visible = this.visible;
            NewObject.selected = false;
            return NewObject;
        }
        public string Name
        {
            get { return name; }
            set {  name = value; OnPropertyChanged("Name"); }
        }
        public int GroupNumber
        {
            get { return groupNumber; }
            set {  groupNumber = value; OnPropertyChanged("GroupNumber"); }
        }
        public int GroupPriority
        {
            get { return groupPriority; }
            set {  groupPriority = value; OnPropertyChanged("GroupPriority"); }
        }


        public int Number { get { return number; } set { number = value; } }
        public PhysicalMaterial Material
        {
            get { return material; }
            set {  material = value; OnPropertyChanged("Material"); }
        }
        public bool IsForDeformation
        {
            get { return isDeformation; }
            set { isDeformation = value; OnPropertyChanged("IsForDeformation"); }
        }
        bool ray_intersects_polygon(double[] p1, double[] p2, double[] p3, double[] p4, double[] r1, double[] r2, double[] ip)
        {
            if (ray_intersects_triangle(p1, p2, p3, r1, r2, ip) == true) return true;
            if (ray_intersects_triangle(p1, p3, p4, r1, r2, ip) == true) return true;
            return false;
        }
        double V_m_V(double[] V1, double[] V2)
        {
            int i;
            double res;
            res = 0.0;
            for (i = 0; i < 3; i++)
                res += V1[i] * V2[i];
            return res;
        }
        void normalize_V(double[] V)
        {
            int i;
            double norm;
            norm = Math.Sqrt(V_m_V(V, V));
            for (i = 0; i < 3; i++)
                V[i] /= norm;
        }
        double V_norm(double[] V)
        {
            return Math.Sqrt(V_m_V(V, V));
        }
        bool ray_intersects_triangle(double[] p1, double[] p2, double[] p3, double[] r1, double[] r2, double[] ip)
        {
            int j;
            double d, a1, a2, a3, ksi, ang, den;
            double[] v1 = new double[3], v2 = new double[3], v3 = new double[3], n = new double[3], dir = new double[3];

            for (j = 0; j < 3; j++) dir[j] = r2[j] - r1[j];
            //normalize_V(dir);

            n[0] = (p2[1] - p1[1]) * (p3[2] - p1[2]) - (p2[2] - p1[2]) * (p3[1] - p1[1]);
            n[1] = (p2[2] - p1[2]) * (p3[0] - p1[0]) - (p2[0] - p1[0]) * (p3[2] - p1[2]);
            n[2] = (p2[0] - p1[0]) * (p3[1] - p1[1]) - (p2[1] - p1[1]) * (p3[0] - p1[0]);
            normalize_V(n);

            d = -n[0] * p1[0] - n[1] * p1[1] - n[2] * p1[2];

            den = V_m_V(n, dir);
            if (Math.Abs(den) < 1e-10) return false;

            ksi = -(d + n[0] * r1[0] + n[1] * r1[1] + n[2] * r1[2]) / den;

            if (ksi < 0.0 || ksi > 1.0) return false;

            ip[0] = r1[0] + ksi * (r2[0] - r1[0]);
            ip[1] = r1[1] + ksi * (r2[1] - r1[1]);
            ip[2] = r1[2] + ksi * (r2[2] - r1[2]);

            for (j = 0; j < 3; j++) v1[j] = p1[j] - ip[j];
            for (j = 0; j < 3; j++) v2[j] = p2[j] - ip[j];
            for (j = 0; j < 3; j++) v3[j] = p3[j] - ip[j];

            if (V_norm(v1) < 1e-10)
                return true;
            if (V_norm(v2) < 1e-10)
                return true;
            if (V_norm(v3) < 1e-10)
                return true;

            normalize_V(v1);
            normalize_V(v2);
            normalize_V(v3);

            a1 = V_m_V(v1, v2);
            a2 = V_m_V(v2, v3);
            a3 = V_m_V(v1, v3);

            if (a1 > 1.0) a1 = 1.0 - 1e-13;
            if (a2 > 1.0) a2 = 1.0 - 1e-13;
            if (a3 > 1.0) a3 = 1.0 - 1e-13;

            if (a1 < -1.0) a1 = -1.0 + 1e-13;
            if (a2 < -1.0) a2 = -1.0 + 1e-13;
            if (a3 < -1.0) a3 = -1.0 + 1e-13;

            ang = (Math.Acos(a1) + Math.Acos(a2) + Math.Acos(a3)) * 180.0 / Math.PI;
            if (Math.Abs(ang - 360) > 1e-4) return false;

            return true;
        }
        double DistanceFromLineToPoint(
            ref double a, ref double b, ref double c,   // line, vector 
            ref double x1, ref double y1, ref double z1,//      and start point
            ref double x0, ref double y0, ref double z0)// point
        {
            double x01 = x0 - x1;
            double y01 = y0 - y1;
            double z01 = z0 - z1;
            double bc = y01 * c - z01 * b;
            double ca = z01 * a - x01 * c;
            double ab = x01 * b - y01 * a;
            return Math.Sqrt(bc * bc + ca * ca + ab * ab) / Math.Sqrt(a * a + b * b + c * c);
        }
        void GetCenterAndRadius(out double[] c, out double r)
        {
            c = new double[3];
            for (int i = 0; i < 3; i++)
                c[i] = 0.5 * (parallel[i].Min + parallel[i].Max);

            r = Math.Sqrt((X1 - X0) * (X1 - X0) + (Y1 - Y0) * (Y1 - Y0) + (Z1 - Z0) * (Z1 - Z0)) * 0.5;
        }
        void GetCenterAndRadiusHex(out double[] c, out double r)
        {
            c = new double[3];
            r = -Double.MaxValue;
            for (int i = 0; i < 8; i++)
                c[0] += hexParameters[i].Value;

            for (int i = 9; i < 16; i++)
                c[1] += hexParameters[i].Value;

            c[0] *= 0.125;
            c[1] *= 0.125;
            c[2]  = (hexParameters[16].Value + hexParameters[17].Value) * 0.5;

            double[] p = new double[3];

            for (int i = 0; i < 8; i++)
            {
                p[0] = hexParameters[i].Value;
                p[1] = hexParameters[i+8].Value;
                p[2] = hexParameters[16 + i/4].Value;

                r = Math.Max(r, LittleTools.CalculateDistance3D(c[0], c[1], c[2], p[0], p[1], p[2]));
            }
        }

        private bool LineIntersectsPolygon(double[] v1, double[] v2, double[] v3, double[] v4, double[] p1, double[] p2, double[] ip, Vector3 ray, out Vector3 intersect)
        {
            intersect = null;
            Vector3 norm;
            Vector3 t1 = new Vector3();
            Vector3 t2 = new Vector3();

            t1.X = v3[0] - v1[0];
            t1.Y = v3[1] - v1[1];
            t1.Z = v3[2] - v1[2];
            t2.X = v4[0] - v2[0];
            t2.Y = v4[1] - v2[1];
            t2.Z = v4[2] - v2[2];

            t1.Normalize();
            t2.Normalize();

            norm = t1.VectorMult(t2);

            if (norm.ScalarMult(ray) < 1e-10)
                return false;

            if (ray_intersects_polygon(v1, v2, v3, v4, p1, p2, ip))
            {
                intersect = new Vector3();
                intersect.X = ip[0];
                intersect.Y = ip[1];
                intersect.Z = ip[2];
                return true;
            }

            return false;
        }
        private bool LineIntersectsRegObject(double[] p1, double[] p2, out Vector3 intersect)
        {
            intersect = null;
            double[] c;
            double r;
            double[] v = new double[3];

            for (int i = 0; i < 3; i++)
                v[i] = p2[i] - p1[i];

            GetCenterAndRadius(out c, out r);
            if (DistanceFromLineToPoint(ref v[0], ref v[1], ref v[2], ref p1[0], ref p1[1], ref p1[2], ref c[0], ref c[1], ref c[2]) > r)
                return false;

            double[] v1 = new double[3];
            double[] v2 = new double[3];
            double[] v3 = new double[3];
            double[] v4 = new double[3];
            double[] ip = new double[3];
            Vector3 ray = new Vector3(p2[0] - p1[0], p2[1] - p1[1], p2[2] - p1[2]); ray.Normalize();

            v1[0] = parallel[0].Min; v1[1] = parallel[1].Min; v1[2] = parallel[2].Min;
            v2[0] = parallel[0].Min; v2[1] = parallel[1].Max; v2[2] = parallel[2].Min;
            v3[0] = parallel[0].Max; v3[1] = parallel[1].Max; v3[2] = parallel[2].Min;
            v4[0] = parallel[0].Max; v4[1] = parallel[1].Min; v4[2] = parallel[2].Min;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = parallel[0].Min; v1[1] = parallel[1].Min; v1[2] = parallel[2].Max;
            v2[0] = parallel[0].Max; v2[1] = parallel[1].Min; v2[2] = parallel[2].Max;
            v3[0] = parallel[0].Max; v3[1] = parallel[1].Max; v3[2] = parallel[2].Max;
            v4[0] = parallel[0].Min; v4[1] = parallel[1].Max; v4[2] = parallel[2].Max;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = parallel[0].Min; v1[1] = parallel[1].Min; v1[2] = parallel[2].Min;
            v2[0] = parallel[0].Min; v2[1] = parallel[1].Min; v2[2] = parallel[2].Max;
            v3[0] = parallel[0].Min; v3[1] = parallel[1].Max; v3[2] = parallel[2].Max;
            v4[0] = parallel[0].Min; v4[1] = parallel[1].Max; v4[2] = parallel[2].Min;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = parallel[0].Max; v1[1] = parallel[1].Min; v1[2] = parallel[2].Min;
            v2[0] = parallel[0].Max; v2[1] = parallel[1].Max; v2[2] = parallel[2].Min;
            v3[0] = parallel[0].Max; v3[1] = parallel[1].Max; v3[2] = parallel[2].Max;
            v4[0] = parallel[0].Max; v4[1] = parallel[1].Min; v4[2] = parallel[2].Max;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = parallel[0].Min; v1[1] = parallel[1].Min; v1[2] = parallel[2].Min;
            v2[0] = parallel[0].Max; v2[1] = parallel[1].Min; v2[2] = parallel[2].Min;
            v3[0] = parallel[0].Max; v3[1] = parallel[1].Min; v3[2] = parallel[2].Max;
            v4[0] = parallel[0].Min; v4[1] = parallel[1].Min; v4[2] = parallel[2].Max;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = parallel[0].Min; v1[1] = parallel[1].Max; v1[2] = parallel[2].Min;
            v2[0] = parallel[0].Min; v2[1] = parallel[1].Max; v2[2] = parallel[2].Max;
            v3[0] = parallel[0].Max; v3[1] = parallel[1].Max; v3[2] = parallel[2].Max;
            v4[0] = parallel[0].Max; v4[1] = parallel[1].Max; v4[2] = parallel[2].Min;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            return false;
        }
        private bool LineIntersectsHexObject(double[] p1, double[] p2, out Vector3 intersect)
        {
            intersect = null;
            double[] c;
            double r;
            double[] v = new double[3];

            for (int i = 0; i < 3; i++)
                v[i] = p2[i] - p1[i];

            GetCenterAndRadiusHex(out c, out r);
            if (DistanceFromLineToPoint(ref v[0], ref v[1], ref v[2], ref p1[0], ref p1[1], ref p1[2], ref c[0], ref c[1], ref c[2]) > r)
                return false;

            double[] v1 = new double[3];
            double[] v2 = new double[3];
            double[] v3 = new double[3];
            double[] v4 = new double[3];
            double[] ip = new double[3];
            Vector3 ray = new Vector3(p2[0] - p1[0], p2[1] - p1[1], p2[2] - p1[2]); ray.Normalize();
            
                    
            v1[0] = hexParameters[0].Value; v1[1] = hexParameters[ 8].Value; v1[2] = hexParameters[16].Value;
            v2[0] = hexParameters[2].Value; v2[1] = hexParameters[10].Value; v2[2] = hexParameters[16].Value;
            v3[0] = hexParameters[3].Value; v3[1] = hexParameters[11].Value; v3[2] = hexParameters[16].Value;
            v4[0] = hexParameters[1].Value; v4[1] = hexParameters[ 9].Value; v4[2] = hexParameters[16].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = hexParameters[4].Value; v1[1] = hexParameters[12].Value; v1[2] = hexParameters[17].Value;
            v2[0] = hexParameters[5].Value; v2[1] = hexParameters[13].Value; v2[2] = hexParameters[17].Value;
            v3[0] = hexParameters[7].Value; v3[1] = hexParameters[15].Value; v3[2] = hexParameters[17].Value;
            v4[0] = hexParameters[6].Value; v4[1] = hexParameters[14].Value; v4[2] = hexParameters[17].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = hexParameters[0].Value; v1[1] = hexParameters[ 8].Value; v1[2] = hexParameters[16].Value;
            v2[0] = hexParameters[1].Value; v2[1] = hexParameters[ 9].Value; v2[2] = hexParameters[16].Value;
            v3[0] = hexParameters[5].Value; v3[1] = hexParameters[13].Value; v3[2] = hexParameters[17].Value;
            v4[0] = hexParameters[4].Value; v4[1] = hexParameters[12].Value; v4[2] = hexParameters[17].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = hexParameters[2].Value; v1[1] = hexParameters[10].Value; v1[2] = hexParameters[16].Value;
            v2[0] = hexParameters[6].Value; v2[1] = hexParameters[14].Value; v2[2] = hexParameters[17].Value;
            v3[0] = hexParameters[7].Value; v3[1] = hexParameters[15].Value; v3[2] = hexParameters[17].Value;
            v4[0] = hexParameters[3].Value; v4[1] = hexParameters[11].Value; v4[2] = hexParameters[16].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = hexParameters[0].Value; v1[1] = hexParameters[ 8].Value; v1[2] = hexParameters[16].Value;
            v2[0] = hexParameters[4].Value; v2[1] = hexParameters[12].Value; v2[2] = hexParameters[17].Value;
            v3[0] = hexParameters[6].Value; v3[1] = hexParameters[14].Value; v3[2] = hexParameters[17].Value;
            v4[0] = hexParameters[2].Value; v4[1] = hexParameters[10].Value; v4[2] = hexParameters[16].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            v1[0] = hexParameters[1].Value; v1[1] = hexParameters[ 9].Value; v1[2] = hexParameters[16].Value;
            v2[0] = hexParameters[3].Value; v2[1] = hexParameters[11].Value; v2[2] = hexParameters[16].Value;
            v3[0] = hexParameters[7].Value; v3[1] = hexParameters[15].Value; v3[2] = hexParameters[17].Value;
            v4[0] = hexParameters[5].Value; v4[1] = hexParameters[13].Value; v4[2] = hexParameters[17].Value;
            if (LineIntersectsPolygon(v1, v2, v3, v4, p1, p2, ip, ray, out intersect))
                return true;

            return false;
        }
        public bool LineIntersectsObject(double[] p1, double[] p2, out Vector3 intersect)
        {
            if (IsHex)
                return LineIntersectsHexObject(p1, p2, out intersect);
            else
                return LineIntersectsRegObject(p1, p2, out intersect);
        }
        public Color DrawColor
        {
            get
            {
                return Color;
            }
        }
        
        public void Draw3D(bool drawBounds, Color col)
		{
            if (isHex)
            {
                DrawHex(drawBounds, col);
                return;
            }

            if (!Visible)
                return;

            double[] p = new double[12];
            double c1, c2, c3, c4, c5, c6;
            c1 = parallel[0].Min;
            c2 = parallel[0].Max;
            c3 = parallel[1].Min;
            c4 = parallel[1].Max;
            c5 = parallel[2].Min;
            c6 = parallel[2].Max;

            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1, 2);

            GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);
            LittleTools.DrawParallelepipedTriangles(c1, c2, c3, c4, c5, c6);
            GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);

            if (drawBounds)
            {
                GLContex.glColor3f(0, 0, 0);
                GLContex.glLineWidth(1);
                LittleTools.DrawParallelepipedFrame(c1, c2, c3, c4, c5, c6);
            }
        }
        
        public void Draw(EPlaneType axisType, bool drawBounds, Color col)
        {
            if (isHex)
            {
                DrawHex(drawBounds, col);
                return;
            }

            if (!Visible)
                return;

            double[] p = new double[12];
            double c1, c2, c3, c4, c5, c6;
            c1 = parallel[0].Min;
            c2 = parallel[0].Max;
            c3 = parallel[1].Min;
            c4 = parallel[1].Max;
            c5 = parallel[2].Min;
            c6 = parallel[2].Max;

            if (axisType == EPlaneType.XYZ)
            {
                GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
                GLContex.glPolygonOffset(1, 2);

                GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);
                LittleTools.DrawParallelepipedTriangles(c1, c2, c3, c4, c5, c6);
                GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);

                if (drawBounds)
                {
                    GLContex.glColor3f(0, 0, 0);
                    GLContex.glLineWidth(1);
                    LittleTools.DrawParallelepipedFrame(c1, c2, c3, c4, c5, c6);
                }
            }

            switch (axisType)
            {
                case EPlaneType.XY:
                    p[0] = c1;
                    p[1] = c3;
                    p[2] = c6;

                    p[3] = c2;
                    p[4] = c3;
                    p[5] = c6;

                    p[6] = c1;
                    p[7] = c4;
                    p[8] = c6;

                    p[9] = c2;
                    p[10] = c4;
                    p[11] = c6;

                    break;
                case EPlaneType.XZ:
                    p[0] = c1;
                    p[1] = c3;
                    p[2] = c5;

                    p[3] = c2;
                    p[4] = c3;
                    p[5] = c5;

                    p[6] = c1;
                    p[7] = c3;
                    p[8] = c6;

                    p[9] = c2;
                    p[10] = c3;
                    p[11] = c6;
                    break;
                case EPlaneType.YZ:
                    p[0] = c2;
                    p[1] = c3;
                    p[2] = c5;

                    p[3] = c2;
                    p[4] = c4;
                    p[5] = c5;

                    p[6] = c2;
                    p[7] = c3;
                    p[8] = c6;

                    p[9] = c2;
                    p[10] = c4;
                    p[11] = c6;
                    break;
            }


            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1.0f, 2.0f);

            GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);

            GLContex.glBegin(GLContex.GL_TRIANGLES);

            GLContex.glVertex3d(p[6], p[7], p[8]);
            GLContex.glVertex3d(p[9], p[10], p[11]);
            GLContex.glVertex3d(p[3], p[4], p[5]);

            GLContex.glVertex3d(p[3], p[4], p[5]);
            GLContex.glVertex3d(p[0], p[1], p[2]);
            GLContex.glVertex3d(p[6], p[7], p[8]);
            GLContex.glEnd();


            GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);

            if (drawBounds)
            {
                GLContex.glColor3f(0, 0, 0);
                GLContex.glLineWidth(1);

                GLContex.glBegin(GLContex.GL_LINES);

                GLContex.glVertex3d(p[0], p[1], p[2]);
                GLContex.glVertex3d(p[3], p[4], p[5]);
                GLContex.glVertex3d(p[6], p[7], p[8]);
                GLContex.glVertex3d(p[9], p[10], p[11]);
                GLContex.glVertex3d(p[0], p[1], p[2]);
                GLContex.glVertex3d(p[6], p[7], p[8]);
                GLContex.glVertex3d(p[3], p[4], p[5]);
                GLContex.glVertex3d(p[9], p[10], p[11]);
                GLContex.glEnd();
                GLContex.glLineWidth(1);
            }
        }
        public void DrawHex(bool drawBounds, Color col)
        {
            if (!Visible)
                return;

            double x1, x2, x3, x4, x5, x6, x7, x8;
            double y1, y2, y3, y4, y5, y6, y7, y8;
            double z1, z2;
            x1 = hexParameters[0].Value;
            x2 = hexParameters[1].Value;
            x3 = hexParameters[2].Value;
            x4 = hexParameters[3].Value;
            x5 = hexParameters[4].Value;
            x6 = hexParameters[5].Value;
            x7 = hexParameters[6].Value;
            x8 = hexParameters[7].Value;
            y1 = hexParameters[8].Value;
            y2 = hexParameters[9].Value;
            y3 = hexParameters[10].Value;
            y4 = hexParameters[11].Value;
            y5 = hexParameters[12].Value;
            y6 = hexParameters[13].Value;
            y7 = hexParameters[14].Value;
            y8 = hexParameters[15].Value;
            z1 = hexParameters[16].Value;
            z2 = hexParameters[17].Value;

            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1.0f, 2.0f);

            GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);

            GLContex.glBegin(GLContex.GL_QUADS);

            GLContex.glVertex3d(x1, y1, z1);
            GLContex.glVertex3d(x3, y3, z1);
            GLContex.glVertex3d(x7, y7, z2);
            GLContex.glVertex3d(x5, y5, z2);

            GLContex.glVertex3d(x2, y2, z1);
            GLContex.glVertex3d(x4, y4, z1);
            GLContex.glVertex3d(x8, y8, z2);
            GLContex.glVertex3d(x6, y6, z2);

            GLContex.glVertex3d(x1, y1, z1);
            GLContex.glVertex3d(x2, y2, z1);
            GLContex.glVertex3d(x6, y6, z2);
            GLContex.glVertex3d(x5, y5, z2);

            GLContex.glVertex3d(x3, y3, z1);
            GLContex.glVertex3d(x4, y4, z1);
            GLContex.glVertex3d(x8, y8, z2);
            GLContex.glVertex3d(x7, y7, z2);

            GLContex.glVertex3d(x1, y1, z1);
            GLContex.glVertex3d(x2, y2, z1);
            GLContex.glVertex3d(x4, y4, z1);
            GLContex.glVertex3d(x3, y3, z1);

            GLContex.glVertex3d(x5, y5, z2);
            GLContex.glVertex3d(x6, y6, z2);
            GLContex.glVertex3d(x8, y8, z2);
            GLContex.glVertex3d(x7, y7, z2);

            GLContex.glEnd();


            GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);

            if (drawBounds)
            {
                GLContex.glColor3f(0, 0, 0);
                GLContex.glLineWidth(1);

                GLContex.glBegin(GLContex.GL_LINES);

                GLContex.glVertex3d(x1, y1, z1);
                GLContex.glVertex3d(x2, y2, z1);
                GLContex.glVertex3d(x3, y3, z1);
                GLContex.glVertex3d(x4, y4, z1);
                GLContex.glVertex3d(x1, y1, z1);
                GLContex.glVertex3d(x3, y3, z1);
                GLContex.glVertex3d(x2, y2, z1);
                GLContex.glVertex3d(x4, y4, z1);
                GLContex.glVertex3d(x5, y5, z2);
                GLContex.glVertex3d(x6, y6, z2);
                GLContex.glVertex3d(x7, y7, z2);
                GLContex.glVertex3d(x8, y8, z2);
                GLContex.glVertex3d(x5, y5, z2);
                GLContex.glVertex3d(x7, y7, z2);
                GLContex.glVertex3d(x6, y6, z2);
                GLContex.glVertex3d(x8, y8, z2);
                GLContex.glVertex3d(x1, y1, z1);
                GLContex.glVertex3d(x5, y5, z2);
                GLContex.glVertex3d(x2, y2, z1);
                GLContex.glVertex3d(x6, y6, z2);
                GLContex.glVertex3d(x3, y3, z1);
                GLContex.glVertex3d(x7, y7, z2);
                GLContex.glVertex3d(x4, y4, z1);
                GLContex.glVertex3d(x8, y8, z2);

                GLContex.glEnd();
                GLContex.glLineWidth(1);
            }
        }
        public bool Visible
        {
            get { return visible; }
            set {  visible = value; OnPropertyChanged("Visible"); }
        }
        public bool IsHex
        {
            get { return isHex; }
            set {  isHex = value; if (isHex == true && hexParameters == null) BuildHex(); OnPropertyChanged("IsHex"); }
        }
        public Color Color
        {
            get { return backgroundColor; }
            set
            {
                
                backgroundColor = value;
                OnPropertyChanged("Color");
            }
        }
        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("Selected"); }
        }
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        
        public void BuildHex()
        {
            hexParameters = new ObservableCollection<HexParameter>();
            hexParameters.Add(new HexParameter("x1", parallel[0].Min));
            hexParameters.Add(new HexParameter("x2", parallel[0].Max));
            hexParameters.Add(new HexParameter("x3", parallel[0].Min));
            hexParameters.Add(new HexParameter("x4", parallel[0].Max));
            hexParameters.Add(new HexParameter("x5", parallel[0].Min));
            hexParameters.Add(new HexParameter("x6", parallel[0].Max));
            hexParameters.Add(new HexParameter("x7", parallel[0].Min));
            hexParameters.Add(new HexParameter("x8", parallel[0].Max));
            hexParameters.Add(new HexParameter("y1", parallel[1].Min));
            hexParameters.Add(new HexParameter("y2", parallel[1].Min));
            hexParameters.Add(new HexParameter("y3", parallel[1].Max));
            hexParameters.Add(new HexParameter("y4", parallel[1].Max));
            hexParameters.Add(new HexParameter("y5", parallel[1].Min));
            hexParameters.Add(new HexParameter("y6", parallel[1].Min));
            hexParameters.Add(new HexParameter("y7", parallel[1].Max));
            hexParameters.Add(new HexParameter("y8", parallel[1].Max));
            hexParameters.Add(new HexParameter("z1", parallel[2].Min));
            hexParameters.Add(new HexParameter("z2", parallel[2].Max));
        }
        public void GetHexVertex(int index, String p, out double v)
        {
            v = 0;
            switch (p)
            {
                case "X":
                    v = hexParameters[index].Value;
                    break;
                case "Y":
                    v = hexParameters[8+index].Value;
                    break;
                case "Z":
                    v = hexParameters[16 + index/4].Value;
                    break;
            }
        }
        public void GetHexVertex(int index, String p1, String p2, double[] v)
        {
            GetHexVertex(index, p1, out v[0]);
            GetHexVertex(index, p2, out v[1]);
        }
        public void GetHexVertex(int index, double[] v)
        {
            GetHexVertex(index, "X", out v[0]);
            GetHexVertex(index, "Y", out v[1]);
            GetHexVertex(index, "Z", out v[2]);
        }
        public void Move(String axis, double v)
        {
            switch(axis)
            {
                case "X":
                    X0 += v;
                    X1 += v;

                    if (hexParameters != null && hexParameters.Count == 18)
                        for (int i = 0; i < 8; i++)
                            hexParameters[i].Value += v;
                    break;

                case "Y":
                    Y0 += v;
                    Y1 += v;

                    if (hexParameters != null && hexParameters.Count == 18)
                        for (int i = 8; i < 16; i++)
                            hexParameters[i].Value += v;
                    break;

                case "Z":
                    Z0 += v;
                    Z1 += v;

                    if (hexParameters != null && hexParameters.Count == 18)
                        for (int i = 16; i < 18; i++)
                            hexParameters[i].Value += v;
                    break;
            }
        }
        public void Move(int vertex, String axis, double v)
        {
            switch (axis)
            {
                case "X":
                    if (hexParameters != null && hexParameters.Count == 18)
                        hexParameters[vertex].Value += v;
                    break;

                case "Y":
                    if (hexParameters != null && hexParameters.Count == 18)
                        hexParameters[8 + vertex].Value += v;
                    break;

                case "Z":
                    if (hexParameters != null && hexParameters.Count == 18)
                        hexParameters[16 + vertex/4].Value += v;
                    break;
            }
        }

        public void Draw(EPlaneType planeType, double[] boundingBox, int widthLocal, int heightLocal, FontGeology labelFont, FontGeology paletteFont)
        {
            Draw(planeType, true, Color);
        }

        public double X0
        {
            get { return parallel[0].Min; }
            set {  parallel[0].Min = value; OnPropertyChanged("X0"); }
        }
        public double X1
        {
            get { return parallel[0].Max; }
            set {  parallel[0].Max = value; OnPropertyChanged("X1"); }
        }
        public double Y0
        {
            get { return parallel[1].Min; }
            set { parallel[1].Min = value; OnPropertyChanged("Y0"); }
        }
        public double Y1
        {
            get { return parallel[1].Max; }
            set {  parallel[1].Max = value; OnPropertyChanged("Y1"); }
        }
        public double Z0
        {
            get { return parallel[2].Min; }
            set {  parallel[2].Min = value; OnPropertyChanged("Z0"); }
        }
        public double Z1
        {
            get { return parallel[2].Max; }
            set {  parallel[2].Max = value; OnPropertyChanged("Z1"); }
        }

    }
}
