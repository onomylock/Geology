using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Geology.OpenGL.OpenGLControl;
using System.Drawing.Imaging;
using Geology.Projection;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using Geology.Objects.GeoModel;
using Geology.Objects;


namespace Geology.DrawNewWindow.View
{
	public class DrawView3D : OpenGLControl
	{
		bool mouseDown = false;
		TypeTransformation typeTransform;
		private ObservableCollection<CGeoLayer> layers;
		private ObservableCollection<CGeoObject> objects;
		private GeoModel model;

		
	}
}
