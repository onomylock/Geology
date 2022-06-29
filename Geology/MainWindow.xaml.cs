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

			NewMesh mesh = new NewMesh();
			ModelWindow geoModel1 = new ModelWindow();
			int index = 0;
			foreach (Element ver in mesh.Elements)
			{
				geoModel1.Objects.Add(new CGeoObject());
				//geoModel1.Objects[index].Color = new Color(0, mesh.NormalizeDataGrid.);
				geoModel1.Objects[index].X0 = mesh.grids[mesh.Elements[index].VertexArr.ToArray()[0]].RZ.R;
				geoModel1.Objects[index].X1 = mesh.grids[mesh.Elements[index].VertexArr.ToArray()[1]].RZ.R;
				geoModel1.Objects[index].Y0 = mesh.grids[mesh.Elements[index].VertexArr.ToArray()[0]].RZ.Z;
				geoModel1.Objects[index].Y1 = mesh.grids[mesh.Elements[index].VertexArr.ToArray()[2]].RZ.Z;
				double sum = 0;
				foreach (int  elem in mesh.Elements[index].VertexArr.ToArray())
				{
					sum += mesh.NormalizeDataGrid[elem];
				}
				sum /= 4;
				geoModel1.Objects[index].Color = new Color();
				geoModel1.Objects[index].Color = Color.FromRgb((byte)(sum + 128), (byte)(sum + 64), (byte)(sum + 100));
				index++;
			}

			geoModel1.GlobalBoundingBox[0] = -10000;
			geoModel1.GlobalBoundingBox[1] = 10000;
			geoModel1.GlobalBoundingBox[2] = -10000;
			geoModel1.GlobalBoundingBox[3] = 10000;
			geoModel1.GlobalBoundingBox[4] = -10000;
			geoModel1.GlobalBoundingBox[5] = 10000;

			Controller2DMesh.SetController(EPlaneType.XY, geoModel1);
			//Controller2DMesh.setRotateAndNameAxes(EPlaneType.XY);
			Controller2DMesh.ChangeOrtho(geoModel1.GlobalBoundingBox);
			Controller2DMesh.SetBoundingBox(geoModel1.GlobalBoundingBox);
			foreach (var DrawObj in geoModel1.Objects)
			{
				Controller2DMesh.drawableObjects[PageType.Model].Add(DrawObj);
			}

			ModelWindow modelWindow = new ModelWindow();
			//modelWindow.Objects.Add(new CGeoObject());

			modelWindow.GlobalBoundingBox[0] = -10000;
			modelWindow.GlobalBoundingBox[1] = 10000;
			modelWindow.GlobalBoundingBox[2] = -10000;
			modelWindow.GlobalBoundingBox[3] = 10000;
			modelWindow.GlobalBoundingBox[4] = -10000;
			modelWindow.GlobalBoundingBox[5] = 10000;

			List<CGeoObject> tmpList = new List<CGeoObject>();

			LoadObjectsCommon("C:/Users/onomy/Downloads/objects", ref tmpList);
			foreach (var obj in tmpList)
			{
				modelWindow.Objects.Add(obj);
			}

			Controller2DXY.SetController(EPlaneType.XY, modelWindow);
			//Controller2DXY.setRotateAndNameAxes(EPlaneType.XY);
			Controller2DXY.ChangeOrtho(modelWindow.GlobalBoundingBox);
			Controller2DXY.SetBoundingBox(modelWindow.GlobalBoundingBox);
			foreach (var DrawObj in modelWindow.Objects)
			{
				Controller2DXY.drawableObjects[PageType.Model].Add(DrawObj);
			}
			//Controller2DXY.drawableObjects[PageType.Model].Add(modelWindow.Objects.First());

			Controller2DXZ.SetController(EPlaneType.XZ, modelWindow);
			//Controller2DXZ.setRotateAndNameAxes(EPlaneType.XY);
			Controller2DXZ.ChangeOrtho(modelWindow.GlobalBoundingBox);
			Controller2DXZ.SetBoundingBox(modelWindow.GlobalBoundingBox);
			foreach (var DrawObj in modelWindow.Objects)
			{
				Controller2DXZ.drawableObjects[PageType.Model].Add(DrawObj);
			}
			//Controller2DXZ.drawableObjects[PageType.Model].Add(modelWindow.Objects.First());

			Controller2DYZ.SetController(EPlaneType.YZ, modelWindow);
			//Controller2DYZ.setRotateAndNameAxes(EPlaneType.XY);
			Controller2DYZ.ChangeOrtho(modelWindow.GlobalBoundingBox);
			Controller2DYZ.SetBoundingBox(modelWindow.GlobalBoundingBox);
			foreach (var DrawObj in modelWindow.Objects)
			{
				Controller2DYZ.drawableObjects[PageType.Model].Add(DrawObj);
			}
			//Controller2DYZ.drawableObjects[PageType.Model].Add(modelWindow.Objects.First());

			Controller3D.SetController(EPlaneType.XY, modelWindow);
			Controller3D.SetBoundingBox(modelWindow.GlobalBoundingBox);
			Controller3D.Model = modelWindow;
			Controller3D.SetMainRef(this);

			//Controller3D.SetBoundingBox(geoModel1.GlobalBoundingBox);
			//Controller3D.SetMainRef(this);

			//Controller2DYZ.setRotateAndNameAxes(EPlaneType.YZ);
			//Controller2DXY.setRotateAndNameAxes(EPlaneType.XY);
			//Controller2DXZ.setRotateAndNameAxes(EPlaneType.XZ);

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

		private int LoadObjectsCommon(String fileName, ref List<CGeoObject>  objects)
		{
			try
			{
				int n, objectNumber;
				double tmp;
				byte tmpb;
				String buffer;
				Objects.CGeoObject newObject;
				System.Windows.Media.Color newColor;
				double scaleX, scaleY, scaleZ;

				//scaleX = double.Parse(TextBoxScaleX.Text);
				//scaleY = double.Parse(TextBoxScaleY.Text);
				//scaleZ = double.Parse(TextBoxScaleZ.Text);
				scaleX = 1;
				scaleY = 1;
				scaleZ = 1;

				StreamReader inputFile = new StreamReader(fileName);

				if (inputFile == null)
					return 1;

				//if (toClearObjects == true)
				//	objects.Clear();

				List<CGeoObject> tmpObjects = new List<CGeoObject>();

				buffer = inputFile.ReadLine();
				buffer = inputFile.ReadLine(); n = int.Parse(buffer);

				objectNumber = objects.Count + 1;
				for (int i = 0; i < n; i++)
				{
					newObject = new CGeoObject();
					newColor = new Color();

					//====================================================== Name
					buffer = inputFile.ReadLine(); newObject.Name = buffer;

					//====================================================== RGB Rho
					buffer = inputFile.ReadLine();
					buffer = buffer.Replace("\t", " ");
					tmpb = byte.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newColor.R = tmpb;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmpb = byte.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newColor.G = tmpb;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmpb = byte.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newColor.B = tmpb;

					newObject.Color = System.Windows.Media.Color.FromRgb(newColor.R, newColor.G, newColor.B);

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer);
					newObject.Material.RhoH = tmp;
					newObject.Material.RhoV = tmp;
					newObject.Material.RhoY = tmp;
					newObject.Material.RhoX = tmp;

					//====================================================== XXYYZZ

					buffer = inputFile.ReadLine();
					buffer = buffer.Replace("\t", " ");
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.parallel[0].Min = tmp * scaleX;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.parallel[0].Max = tmp * scaleX;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.parallel[1].Min = tmp * scaleY;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.parallel[1].Max = tmp * scaleY;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.parallel[2].Min = tmp * scaleZ;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer);
					newObject.parallel[2].Max = tmp * scaleZ;

					//====================================================== 0

					buffer = inputFile.ReadLine();

					//====================================================== 1 Alpha t0 Beta

					buffer = inputFile.ReadLine();
					buffer = buffer.Replace("\t", " ");

					var tmpi = int.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.Material.Dep = tmpi - 1;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.Material.Alpha = tmp;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer.Substring(0, buffer.IndexOf(" ")));
					newObject.Material.T0 = tmp;

					buffer = buffer.Substring(buffer.IndexOf(" ") + 1, buffer.Length - buffer.IndexOf(" ") - 1);
					tmp = double.Parse(buffer);
					newObject.Material.Beta = tmp;

					newObject.Number = objectNumber;
					tmpObjects.Add(newObject);
					objectNumber++;
				}

				foreach (var obj in tmpObjects)
					objects.Add(obj);

				return 0;
			}
			catch (Exception ex)
			{
				return 1;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

		private void Controller2DXY_Load(object sender, EventArgs e)
		{

		}

		private void Controller2DXZ_Load(object sender, EventArgs e)
		{

		}

		private void Controller2DYZ_Load(object sender, EventArgs e)
		{

		}
	}
}
