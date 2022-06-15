/*
 * 
 * Обработка событий и логическая часть главного окна.
 * В классе этого окна хранится также вся информация о модели - 
 * слои, объекты, система наблюдений, области инверсии и т.д.
 * 
 */
//#define _PROTECTION_
//
//#define _LIMITED_
//#define _LIMITED2_ // Aero
//#define _LIMITED_CED_ // CED
//#define _LIMITED_AA_ // ArticleAreo


// in Grouping.cs look for LIMITED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Objects;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections.Specialized;
using Geology.Objects.GeoModel;
using Microsoft.Win32;
using System.Collections;
using Geology.DrawWindow;
using Geology.DrawNewWindow.Controller;
using Geology.DrawNewWindow.View;
using Geology.DrawNewWindow.Model;
using Geology.DrawNewWindow.Mesh;

namespace Geology
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private Tuple<CCurve, CCurveInfo> BuildCurve()
        {
            CCurve curve = new CCurve();
            CCurveInfo info = new CCurveInfo();


            for (int i = 0; i < 100; i++)
            {
                curve.CurveArgs.Add(i);
                curve.CurveValues.Add(Math.Sin(i * 0.1));
            }

            info.Name = "Some Curve";

            return new Tuple<CCurve, CCurveInfo>(curve, info);
        }
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

#if _PROTECTION_
            if (Utilities.LittleTools.ExecuteExe("C:/Geology/Protector.exe", "", ".") != 9121990)
                this.Close();
#endif
            InitializeComponent();

			//Mesh mesh = new Mesh();
			ModelWindow geoModel1 = new ModelWindow();
			//int index = 0;
			//foreach(Element ver in mesh.Elements)
			//{
			//	geoModel1.Objects.Add(new CGeoObject());
			//	geoModel1.Objects[index].Color = (Color)ColorConverter.ConvertFromString("Blue");
			//	geoModel1.Objects[index].X0 = mesh.RZ[mesh.Elements[index].VertexArr.ToArray()[0]].R;
			//	geoModel1.Objects[index].X1 = mesh.RZ[mesh.Elements[index].VertexArr.ToArray()[1]].R;
			//	geoModel1.Objects[index].Y0 = mesh.RZ[mesh.Elements[index].VertexArr.ToArray()[0]].Z;
			//	geoModel1.Objects[index].Y1 = mesh.RZ[mesh.Elements[index].VertexArr.ToArray()[2]].Z;
			//	index++;
			//}
			//Controller3D.Controller.Model.Objects.Add(new CGeoObject());
			//Controller3D.Controller.Model.GlobalBoundingBox[0] = -10000;
			//Controller3D.Controller.Model.GlobalBoundingBox[1] = 10000;
			//Controller3D.Controller.Model.GlobalBoundingBox[2] = -10000;
			//Controller3D.Controller.Model.GlobalBoundingBox[3] = 10000;
			//Controller3D.Controller.Model.GlobalBoundingBox[4] = -10000;
			//Controller3D.Controller.Model.GlobalBoundingBox[5] = 10000;

						
			//Controller2DMesh.setRotateAndNameAxes(EPlaneType.XY);
			//Controller2DMesh.ChangeOrtho(geoModel1.GlobalBoundingBox);
			//Controller2DMesh.BoundingBox = geoModel1.GlobalBoundingBox;
			//foreach(var DrawObj in geoModel1.Objects)
			//{
			//	Controller2DMesh.Model.drawableObjects[PageType.Model].Add(DrawObj);
			//}

			//Controller3D.SetCostructor(OpenGL.OpenGLControl.ConstructorType.ThreeDimensional);

			//Controller3D.Controller.BoundingBox = geoModel1.GlobalBoundingBox;
			//Controller3D.Controller.Model. = geoModel1;
			//Controller3D.Controller.SetMainRef(this);

			//XZOpenGlWindow.Controller.Model = geoModel1;
			//XZOpenGlWindow.Controller.SetMainRef(this);

			//Controller2DYZ.setRotateAndNameAxes(EPlaneType.YZ);
			//Controller2DXY.setRotateAndNameAxes(EPlaneType.XY);
			//XZOpenGlWindow.Controller.setRotateAndNameAxes(EPlaneType.XZ);

			//Controller2DYZ.ChangeOrtho(geoModel1.GlobalBoundingBox);
			//Controller2DYZ.SetBoundingBox(geoModel1.GlobalBoundingBox);
			//Controller2DYZ.drawableObjects[PageType.Model].Add(geoModel1.Objects.First());


			//Controller2DXY.ChangeOrtho(geoModel1.GlobalBoundingBox);
			//Controller2DXY.SetBoundingBox(geoModel1.GlobalBoundingBox);
			//Controller2DXY.drawableObjects[PageType.Model].Add(geoModel1.Objects.First());

			//Controller2DXZ.ChangeOrtho(geoModel1.GlobalBoundingBox);
			//Controller2DXZ.SetBoundingBox(geoModel1.GlobalBoundingBox);
			//Controller2DXZ.drawableObjects[PageType.Model].Add(geoModel1.Objects.First());
		}
	
		private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
