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

			

			GeoModel geoModel1 = new GeoModel();
			geoModel1.Objects.Add(new CGeoObject());
			geoModel1.GlobalBoundingBox[0] = -10000;
			geoModel1.GlobalBoundingBox[1] = 10000;
			geoModel1.GlobalBoundingBox[2] = -10000;
			geoModel1.GlobalBoundingBox[3] = 10000;
			geoModel1.GlobalBoundingBox[4] = -10000;
			geoModel1.GlobalBoundingBox[5] = 10000;

			ModelWindow model = new ModelWindow();
			model.Objects.Add(new CGeoObject());
			model.GlobalBoundingBox[0] = -10000;
			model.GlobalBoundingBox[1] = 10000;
			model.GlobalBoundingBox[2] = -10000;
			model.GlobalBoundingBox[3] = 10000;
			model.GlobalBoundingBox[4] = -10000;
			model.GlobalBoundingBox[5] = 10000;
			
			Controller.SetBoundingBox(model.GlobalBoundingBox);
			Controller.SetMainRef(this);
			
			
			

			//Controller2D.SetMainRef(this);
			Controller2D.setRotateAndNameAxes(EPlaneType.YZ);
			//Controller2D.ChangeOrtho(model.GlobalBoundingBox);
			//Controller2D.SetBoundingBox(model.GlobalBoundingBox);
			//Controller2D.drawableObjects[PageType.Model].Add(model.Objects.First());
			Controller2D.ChangeOrtho(geoModel1.GlobalBoundingBox);
			Controller2D.SetBoundingBox(geoModel1.GlobalBoundingBox);
			Controller2D.drawableObjects[PageType.Model].Add(geoModel1.Objects.First());



			View3DWindow.SetObjects(model.Layers, model.Objects, geoModel1);
			View3DWindow.SetMainRef(this);
			View3DWindow.ChangeBoundingBox(geoModel1.GlobalBoundingBox);





			XZOpenGlWindow.setRotateAndNameAxes(EPlaneType.XZ); //XZ
			XZOpenGlWindow.ChangeOrtho(geoModel1.GlobalBoundingBox);
			XZOpenGlWindow.ChangeBoundingBox(geoModel1.GlobalBoundingBox);
			XZOpenGlWindow.drawableObjects[PageType.Model].Add(geoModel1.Objects.First());

			var curve = BuildCurve();
			//graphViewerControl.CurvesInfoList.Add(curve.Item2);
			
			graphViewerControl.TGraph.Curves.Add(curve.Item1);
			graphViewerControl.TGraph.CurvesInfoList.Add(curve.Item2);
			//graphViewerControl.TGraph.CurvesInfoList = graphViewerControl.TGraph.CurvesInfoList;
			int i = 0;
			i = 1;
		}
	
		private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
