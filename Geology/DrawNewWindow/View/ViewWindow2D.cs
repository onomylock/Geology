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
using Geology.DrawNewWindow.Model;
using Geology.DrawWindow;
using Geology.Utilities;
using Geology.DrawNewWindow.Controller;


namespace Geology.DrawNewWindow.View
{
	class ViewWindow2D : ViewAbstract, IViewWindow
	{
		public double scaleV;
		protected readonly EPlaneType axisType;
		private COrthoControlProport Ortho;
		private bool selectionStarted = false;
		private bool selectionFinished = true;
		private readonly double zRange = 1e+7;
		private double selectionX0, selectionX1;
		private double selectionY0, selectionY1;

		public ViewWindow2D(COrthoControlProport Ortho,
			List<IViewportObjectsDrawable> viewportObjectsDrawables, EPlaneType axisType, PageType page,
			int Width, int Height, double[] BoundingBox, IntPtr Handle) : base()
		{
			this.viewportObjectsDrawables = viewportObjectsDrawables;
			this.BoundingBox = BoundingBox;
			this.axisType = axisType;
			this.Height = Height;
			this.zRange = 1e+7;
			this.Ortho = Ortho;
			this.Width = Width;
			this.page = page;

			oglcontext = GLContex.InitOpenGL((int)Handle);
			hdc = Win32.GetDC(Handle);
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			GLContex.glClearColor(1, 1, 1, 1);

			CaptionHorAndVert = new CaptionAxisHorAndVert(hdc, oglcontext, "Arial", 16, Ortho, Width, Height);
			wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 14);
			paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
			fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

			GLContex.glEnable(GLContex.GL_DEPTH_TEST);
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public override void DisposedView(object sender, EventArgs e)
		{
			// здесь происходит очистка шрифта, необходимая функция, чтобы не утекала память
			Win32.wglMakeCurrent(Hdc, (IntPtr)OglContext);
			CaptionHorAndVert.ClearFont();
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public override void Draw()
		{
			GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
			GLContex.glLoadIdentity();
			if (axisType == EPlaneType.XZ || axisType == EPlaneType.YZ)
				scaleV = GlobalDrawingSettings.ScaleZ;
			else
				scaleV = 1.0;
			CaptionHorAndVert.GenerateGrid(widthLocal, heightLocal, scaleV);
			CaptionHorAndVert.DrawScaleLbls(widthLocal, heightLocal, scaleV);
			DrawObjetcs();
		}

		public override void UpdateViewMatrix()
		{
			try
			{
				CaptionHorAndVert.GetNewViewport(Width, Height, out int[] viewPoint);
				widthLocal = viewPoint[2];
				heightLocal = viewPoint[3];

				double[] ortho;
				Ortho.CoefHeightToWidth = heightLocal / (double)widthLocal;
				Ortho.GetOrtho(out ortho);

				GLContex.glMatrixMode(GLContex.GL_PROJECTION);
				GLContex.glLoadIdentity();
				GLContex.glViewport(viewPoint[0], viewPoint[1], viewPoint[2], viewPoint[3]);
				GLContex.glOrtho(ortho[0], ortho[1], ortho[2], ortho[3], Ortho.GetMinZBuf, Ortho.GetMaxZBuf);

				GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
				GLContex.glLoadIdentity();
			}
			catch (Exception ex)
			{

			}
		}

		private void DrawSelection()
		{
			if (selectionStarted && !selectionFinished)
			{
				GLContex.glLineWidth(3);
				GLContex.glColor3f(0.5f, 0.5f, 0.5f);
				GLContex.glBegin(GLContex.GL_LINE_LOOP);
				switch (axisType)
				{
					case EPlaneType.XY:
						GLContex.glVertex3d(selectionX0, selectionY0, zRange);
						GLContex.glVertex3d(selectionX1, selectionY0, zRange);
						GLContex.glVertex3d(selectionX1, selectionY1, zRange);
						GLContex.glVertex3d(selectionX0, selectionY1, zRange);
						break;
					case EPlaneType.XZ:
						GLContex.glVertex3d(selectionX0, zRange, selectionY0);
						GLContex.glVertex3d(selectionX1, zRange, selectionY0);
						GLContex.glVertex3d(selectionX1, zRange, selectionY1);
						GLContex.glVertex3d(selectionX0, zRange, selectionY1);
						break;
					case EPlaneType.YZ:
						GLContex.glVertex3d(zRange, selectionX0, selectionY0);
						GLContex.glVertex3d(zRange, selectionX1, selectionY0);
						GLContex.glVertex3d(zRange, selectionX1, selectionY1);
						GLContex.glVertex3d(zRange, selectionX0, selectionY1);
						break;
				}
				GLContex.glEnd();
			}
		}

		private void DrawObjetcs()
		{
			switch (axisType)
			{
				case EPlaneType.XZ:
					GLContex.gluLookAt(0.0, -1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);
					break;
				case EPlaneType.YZ:
					GLContex.gluLookAt(1.0, 0.0, 0.0, -1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
					break;
			}

			//foreach (var item in viewportObjectsdrawableObjects[PageType.None])
			//	item.Draw(axisType, BoundingBox, widthLocal, heightLocal, fontReceivers, paletteFont);

			foreach (var item in viewportObjectsDrawables)
				item.Draw(axisType, BoundingBox, widthLocal, heightLocal, fontReceivers, paletteFont);

			DrawSelection();
		}
	}
}
