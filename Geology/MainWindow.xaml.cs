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
using Geology.DrawNewWindow.View;


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
            XYOpenGlWindow.setRotateAndNameAxes(Geology.DrawWindow.CObject3DDraw2D.EPlaneType.XY); //XY
            XZOpenGlWindow.setRotateAndNameAxes(Geology.DrawWindow.CObject3DDraw2D.EPlaneType.XZ); //XZ
            YZOpenGlWindow.setRotateAndNameAxes(Geology.DrawWindow.CObject3DDraw2D.EPlaneType.YZ); //YZ
            //XZOpenGl.setRotateAndNameAxes(Geology.DrawWindow.CObject3DDraw2D.EPlaneType.YZ); //YZ


            GeoModel geoModel = new GeoModel();
            geoModel.Objects.Add(new CGeoObject());
            geoModel.GlobalBoundingBox[0] = -10000;
            geoModel.GlobalBoundingBox[1] = 10000;
            geoModel.GlobalBoundingBox[2] = -10000;
            geoModel.GlobalBoundingBox[3] = 10000;
            geoModel.GlobalBoundingBox[4] = -10000;
            geoModel.GlobalBoundingBox[5] = 10000;



            XYOpenGlWindow.ChangeOrtho(geoModel.GlobalBoundingBox);
            XZOpenGlWindow.ChangeOrtho(geoModel.GlobalBoundingBox);
            YZOpenGlWindow.ChangeOrtho(geoModel.GlobalBoundingBox);
            //XZOpenGl.ChangeOrtho(geoModel.GlobalBoundingBox);

            XYOpenGlWindow.ChangeBoundingBox(geoModel.GlobalBoundingBox);
            XZOpenGlWindow.ChangeBoundingBox(geoModel.GlobalBoundingBox);
            YZOpenGlWindow.ChangeBoundingBox(geoModel.GlobalBoundingBox);
            //XZOpenGl.ChangeBoundingBox(geoModel.GlobalBoundingBox);

            XYOpenGlWindow.drawableObjects[PageType.Model].Add(geoModel.Objects.First());
            XZOpenGlWindow.drawableObjects[PageType.Model].Add(geoModel.Objects.First());
            YZOpenGlWindow.drawableObjects[PageType.Model].Add(geoModel.Objects.First());
            //XZOpenGl.drawableObjects[PageType.Model].Add(geoModel.Objects.First());

            View3DWindow.SetObjects(geoModel.Layers, geoModel.Objects, geoModel);
            View3DWindow.SetMainRef(this);
            View3DWindow.ChangeBoundingBox(geoModel.GlobalBoundingBox);

            var curve = BuildCurve();
            //graphViewerControl.CurvesInfoList.Add(curve.Item2);
            graphViewerControl.TGraph.CurvesInfoList.Add(curve.Item2);
            graphViewerControl.TGraph.Curves.Add(curve.Item1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
