/*
 Файл содержит классы:
 * 
 * CDep - селектор формулы поляризации
 * 
 * CGeoLayer, наследует интерфейс INotifyPropertyChanged - геологический слой.
 * Слой хранит в себе информацию о толщине, физических свойствах и свойствах отображения.
 * также слой может иметь рельеф на кровле
 * Объекты этого типа отображаются при графическом изображении модели, поэтому они имеют метод Draw, который отвечает
 * за графическое отображение объекта этого типа.
 * Наследование INotifyPropertyChanged позволяет контроллерам, привязанным к данным этого типа, 
 * отслеживать изменения в свойствах привязанных объектов и автоматически обновлять отображаемую информацию
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Utilities;
using System.IO;

namespace Geology.Objects
{
    public class CDep
    {
        public CDep(int id, int title)
        {
            Id = id;
            Title = title;
        }
        public CDep()
        {
            Id = 0;
            Title = 0;
        }
        public int Id
        {
            get;
            set;
        }
        public int Title
        {
            get;
            set;
        }

        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                outputFile.WriteLine(Id.ToString());
                outputFile.WriteLine(Title.ToString());
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
                    buffer = inputFile.ReadLine(); Id = int.Parse(buffer);
                    buffer = inputFile.ReadLine(); Title = int.Parse(buffer);
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
                outputFile.Write(Id);
                outputFile.Write(Title);
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
                String buffer;
                if (fileVersion >= 1.0)
                {
                    Id    = inputFile.ReadInt32();
                    Title = inputFile.ReadInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class CGeoLayer :  INotifyPropertyChanged
    {
        private PhysicalMaterial material = new PhysicalMaterial();
        public bool drawColored;
        public event PropertyChangedEventHandler PropertyChanged;
        public Relief relief;
        private System.Windows.Media.Color paletteColor;
        int bindedLayer = -1;
        bool edit = false;

        private System.Windows.Media.Color backgroundColor;
        private double h;
        private double z;
        private int number;
        private int materialNumber;
        private bool rel;
        private bool visible;
        public Objects.Spline splineTop;
        public double H
        {
            get { return h; }
            set {  h = value; OnHChanged(); OnPropertyChanged("H"); }
        }
        public double Z
        {
            get { return z; }
            set {  z = value; OnPropertyChanged("Z"); }
        }
        public int Number
        {
            get { return number; }
            set {  number = value; OnPropertyChanged("Number"); }
        }
        public PhysicalMaterial Material
        {
            get { return material; }
            set { material = value; OnPropertyChanged("Material"); }
        }
        public int MaterialNumber
        {
            get { return materialNumber; }
            set { materialNumber = value; OnPropertyChanged("MaterialNumber"); }
        }
        public bool Rel
        {
            get { return rel; }
            set {  rel = value; OnPropertyChanged("Rel"); }
        }
        public bool Visible
        {
            get { return visible; }
            set { visible = value; OnPropertyChanged("Visible"); }
        }
       

        public System.Windows.Media.Color Color
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                OnPropertyChanged("Color");
            }
        }
       
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        private void OnHChanged()
        {
        }
        public void setSpline(Objects.Spline spl)
        {
            splineTop = spl;
        }
        public void Draw(DrawWindow.CObject3DDraw2D.EPlaneType axisType, double horizon, double zRange, double[] drawBox, bool drawBounds)
        {
            double[] p = new double[12];
            double z0, z1;
            //zRange = 1e+4;
            if (axisType == DrawWindow.CObject3DDraw2D.EPlaneType.XYZ)
                return;

            if (LittleTools.Intersect1D(horizon - H, horizon, drawBox[4], drawBox[5]) == false)
                return;

            if (horizon > drawBox[5])
                z1 = drawBox[5];
            else
                z1 = horizon;

            if (horizon - H < drawBox[4])
                z0 = drawBox[4];
            else
                z0 = horizon - H;

            if (z1 - z0 <= 0.0)
                return;

            switch (axisType)
            {
                case DrawWindow.CObject3DDraw2D.EPlaneType.XY:
                    p[0] = drawBox[0];
                    p[1] = drawBox[2];
                    p[2] = z1 - 1;

                    p[3] = drawBox[1];
                    p[4] = drawBox[2];
                    p[5] = z1 - 1;

                    p[6] = drawBox[0];
                    p[7] = drawBox[3];
                    p[8] = z1 - 1;

                    p[9] = drawBox[1];
                    p[10] = drawBox[3];
                    p[11] = z1 - 1;
                    break;
                case DrawWindow.CObject3DDraw2D.EPlaneType.XZ:
                    p[0] = drawBox[0];
                    p[1] = zRange;
                    p[2] = z0;

                    p[3] = drawBox[1];
                    p[4] = zRange;
                    p[5] = z0;

                    p[6] = drawBox[0];
                    p[7] = zRange;
                    p[8] = z1;

                    p[9] = drawBox[1];
                    p[10] = zRange;
                    p[11] = z1;

                    if (drawColored == false)
                        p[1] = p[4] = p[7] = p[10] = drawBox[3];

                    break;
                case DrawWindow.CObject3DDraw2D.EPlaneType.YZ:
                    p[0] = -zRange;
                    p[1] = drawBox[2];
                    p[2] = z0;

                    p[3] = -zRange;
                    p[4] = drawBox[3];
                    p[5] = z0;

                    p[6] = -zRange;
                    p[7] = drawBox[2];
                    p[8] = z1;

                    p[9] = -zRange;
                    p[10] = drawBox[3];
                    p[11] = z1;

                    if (drawColored == false)
                        p[0] = p[3] = p[6] = p[9] = drawBox[1];
                    break;
            }



            if (Visible == true)
            {
                GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
                GLContex.glPolygonOffset(1.0f, 2.0f);

                    GLContex.glColor3f(Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f);

                GLContex.glBegin(GLContex.GL_TRIANGLES);
                GLContex.glVertex3d(p[6], p[7], p[8]);
                GLContex.glVertex3d(p[9], p[10], p[11]);
                GLContex.glVertex3d(p[3], p[4], p[5]);

                GLContex.glVertex3d(p[3], p[4], p[5]);
                GLContex.glVertex3d(p[0], p[1], p[2]);
                GLContex.glVertex3d(p[6], p[7], p[8]);
                GLContex.glEnd();

                GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);
            }

            if (drawBounds)
            {
                GLContex.glColor3f(0, 0, 0);
                GLContex.glLineWidth(1);
                GLContex.glBegin(GLContex.GL_LINES);
                GLContex.glVertex3d(p[0], p[1], p[2]);
                GLContex.glVertex3d(p[3], p[4], p[5]);
                GLContex.glVertex3d(p[6], p[7], p[8]);
                GLContex.glVertex3d(p[9], p[10], p[11]);
                GLContex.glEnd();
                GLContex.glLineWidth(1);
            }

            relief.Draw();
        }
        public void DrawPolygon(double[] Ortho)
        {

            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1.0f, 1.0f);
            GLContex.glPolygonMode(GLContex.GL_FRONT_AND_BACK, GLContex.GL_FILL);
            GLContex.glColor3f(Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f);

            GLContex.glBegin(GLContex.GL_TRIANGLES);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);


            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);


            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);


            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);


            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);


            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);


            GLContex.glEnd();
            GLContex.glDisable(GLContex.GL_POLYGON_OFFSET_FILL);
            //  DrawCarcas(Ortho);
        }
        public void DrawCarcas(double[] Ortho)
        {
            //  GLContex.glLineWidth(2);
            GLContex.glColor3f(0, 0, 0);
            GLContex.glBegin(GLContex.GL_LINES);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);

            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);

            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[2], Ortho[4]);

            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[2], Ortho[4]);

            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[1], Ortho[3], Ortho[4]);

            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[5]);
            GLContex.glVertex3d(Ortho[0], Ortho[3], Ortho[4]);

            GLContex.glEnd();
            //     GLContex.glLineWidth(1);


        }
        
        public double getValueSpline(double x, double y, double level)
        {
            if (splineTop == null)
            {
                return level;
            }
            else
            {
                return splineTop.getValueInPoint(x, y, level);
            }
        }
        public void recalcNumber(int newNum)
        {
            if (Number > newNum)
                Number--;
        }
        public CGeoLayer()
        {
            h = 10;
            z = 0.0;
            backgroundColor = LittleTools.GetRandomColor();
            paletteColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
            rel = false;
            visible = true;
            splineTop = null;
            drawColored = true;
            relief = new Relief();
        }
        public CGeoLayer(int newNum)
        {
            paletteColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
            backgroundColor = LittleTools.GetRandomColor();
            number = newNum;
            h = 10;
            z = 0.0;
            splineTop = null;
            rel = false;
            visible = true;
            drawColored = true;
            relief = new Relief();
            MaterialNumber = 0;
        }
        public CGeoLayer(CGeoLayer layer)
        {
            paletteColor = layer.paletteColor;
            backgroundColor = layer.Color;
            number = layer.Number;
            h = layer.H;
            z = layer.Z;
            Material = (PhysicalMaterial)layer.Material.Clone();
            splineTop = layer.splineTop;
            rel = layer.Rel;
            visible = layer.Visible;
            drawColored = layer.drawColored;
            relief = new Relief(layer.relief);
            MaterialNumber = layer.MaterialNumber;
            bindedLayer = layer.bindedLayer;
        }

    }
}
