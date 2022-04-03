/*
 Файл содержит классы:
 * 
 * Relief - рельеф границы между слоями. Содержит в себе набор точек, образующих 
 * поверхность рельефа
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.Objects
{
    public class Relief
    {
        public int pointsCount;
        public int highlightedPoint = -1;
        public bool loaded;
        private bool toDraw;
        public List<double> x, y, z;
        public List<double> zSource;
        public String pathToRelief;

        public Relief()
        {
            pointsCount = 0;
            loaded = false;
            x = new List<double>();
            y = new List<double>();
            z = new List<double>();
            zSource = new List<double>(z);
            pathToRelief = "";
        }
        public Relief(Relief relief)
        {
            pointsCount = relief.pointsCount;
            loaded = relief.loaded;
            x = new List<double>(relief.x);
            y = new List<double>(relief.y);
            z = new List<double>(relief.z);

            zSource = new List<double>(z);

            pathToRelief = relief.pathToRelief;
        }
        public bool ToDraw
        {
            get
            {
                return toDraw;
            }
            set
            {
                toDraw = value;
            }
        }
       

        public bool IsCurved()
        {
            double minZ = double.MaxValue;
            double maxZ = -double.MaxValue;
            for (int i = 0; i < z.Count; i++)
            {
                if (minZ > z[i]) minZ = z[i];
                if (maxZ < z[i]) maxZ = z[i];
            }

            return maxZ - minZ > 1e-3;
        }

        public int Load(String fileName, double dx = 0, double dy = 0, double dz = 0, double rx = 0, double ry = 0, double angle = 0, bool swapXY = false)
        {
            try
            {
                if (!File.Exists(fileName))
                    return 1;
                loaded = false;
                double tmp;
                String buffer;
                StreamReader inputFile = new StreamReader(fileName);

                if (inputFile == null)
                    return 1;

                if (x != null)
                    x.Clear();
                else
                    x = new List<double>();

                if (y != null)
                    y.Clear();
                else
                    y = new List<double>();

                if (z != null)
                    z.Clear();
                else
                    z = new List<double>();

                buffer = inputFile.ReadLine();
                while (buffer != null)
                {
                    if (buffer.Length > 0)
                    {
                        buffer = buffer.Replace("\t", " ");
                        buffer = buffer.Trim();
                        tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
                        x.Add(tmp);

                        buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
                        buffer = buffer.Trim();
                        tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
                        y.Add(tmp);

                        buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
                        buffer = buffer.Trim();
                        tmp = double.Parse(buffer);
                        z.Add(tmp);
                    }
                    buffer = inputFile.ReadLine();
                }

                for (int i = 0; i < x.Count; i++)
                    x[i] += dx;

                for (int i = 0; i < y.Count; i++)
                    y[i] += dy;

                for (int i = 0; i < z.Count; i++)
                    z[i] += dz;

                double cosa, sina, xt, yt, zt;
                cosa = Math.Cos(angle * Math.PI / 180.0);
                sina = Math.Sin(angle * Math.PI / 180.0);
                for (int i = 0; i < x.Count; i++)
                {
                    xt = x[i];
                    yt = y[i];
                    zt = z[i];
                    Utilities.LittleTools.RotatePointAroundPoint(ref xt, ref yt, ref zt, cosa, sina, rx, ry, 0);
                    x[i] = xt;
                    y[i] = yt;
                    z[i] = zt;
                }
                
                pointsCount = x.Count();
                inputFile.Close();

                if (swapXY)
                {
                    for (int i = 0; i < pointsCount; i++)
                    {
                        tmp = x[i];
                        x[i] = y[i];
                        y[i] = tmp;
                    }
                }

                loaded = true;
                pathToRelief = fileName;

                zSource = new List<double>(z);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public double[] GetBounds()
        {
            try
            {
                if (x.Count < 1 || y.Count < 1 || x.Count != y.Count)
                    return new double[] { 0.0, 0.0, 0.0, 0.0 };

                double[] bounds = new double[4];

                bounds[0] = double.MaxValue;
                bounds[1] = -double.MaxValue;
                bounds[2] = double.MaxValue;
                bounds[3] = -double.MaxValue;

                for (int i = 0; i < x.Count; i++)
                {
                    if (bounds[0] > x[i]) bounds[0] = x[i];
                    if (bounds[1] < x[i]) bounds[1] = x[i];
                }

                for (int i = 0; i < y.Count; i++)
                {
                    if (bounds[2] > y[i]) bounds[2] = y[i];
                    if (bounds[3] < y[i]) bounds[3] = y[i];
                }

                return bounds;
            }
            catch (Exception ex)
            {
                return new double[] { 0.0, 0.0, 0.0, 0.0 } ;
            }
        }

        public void Draw()
        {
            if (ToDraw)
            {
                GLContex.glPointSize(3);
                GLContex.glColor3f(0, 0, 0);
                
                GLContex.glBegin(GLContex.GL_POINTS);

                for (int i = 0; i < pointsCount; i++ )
                    GLContex.glVertex3d(x[i], y[i], z[i]);
                
                GLContex.glEnd();
            }
        }

    }
}
