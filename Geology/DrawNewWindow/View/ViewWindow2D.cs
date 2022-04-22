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
	class ViewWindow2D : IViewWindow
	{
		//public FontGeology caption { get; set; }
		//public FontGeology wellFont { get; set; }
		

		public double scaleV;
		public int WidthLocal, HeightLocal;

		protected readonly EPlaneType axisType;
		//protected readonly IntPtr hdc;
		//protected readonly int oglcontext;
		protected readonly PageType page;
		private readonly Dictionary<PageType, List<IViewportObjectsDrawable>> drawableObjects;
		private readonly CaptionAxisHorAndVert captionHorAndVert;
		private readonly COrthoControlProport Ortho;
		private bool selectionStarted = false;
		private bool selectionFinished = true;
		private readonly double zRange = 1e+7;
		private double selectionX0 = 0, selectionX1 = 0;
		private double selectionY0 = 0, selectionY1 = 0;
		protected readonly int Width, Height;
		protected readonly double[] BoundingBox;
		private readonly FontGeology fontReceivers;
		private readonly FontGeology paletteFont;

		public ViewWindow2D(CaptionAxisHorAndVert captionHorAndVert, COrthoControlProport Ortho, Dictionary<PageType, 
			List<IViewportObjectsDrawable>> drawableObjects, EPlaneType axisType, double zRange, PageType page, 
			int Width, int Height, double[] BoundingBox, FontGeology fontReceivers, FontGeology paletteFont) : base()
		{
			this.captionHorAndVert = captionHorAndVert;
			this.drawableObjects = drawableObjects;
			this.fontReceivers = fontReceivers;
			this.paletteFont = paletteFont;
			this.BoundingBox = BoundingBox;
			this.axisType = axisType;
			this.Height = Height;
			this.zRange = zRange;
			this.Ortho = Ortho;
			this.Width = Width;
			this.page = page;
			//Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);

			//captionHorAndVert = new CaptionAxisHorAndVert(hdc, oglcontext, "Arial", 16, Ortho, Width, Height);
			//wellFontName = "Arial";
			//wellFontSize = 14;
			////wellFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 14);
			//paletteFont = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
			//fontReceivers = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);

			//Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public void Draw()
		{
			GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
			GLContex.glLoadIdentity();
			if (axisType == EPlaneType.XZ || axisType == EPlaneType.YZ)
				scaleV = GlobalDrawingSettings.ScaleZ;
			else
				scaleV = 1.0;
			captionHorAndVert.GenerateGrid(WidthLocal, HeightLocal, scaleV);
			captionHorAndVert.DrawScaleLbls(WidthLocal, HeightLocal, scaleV);
			DrawObjetcs();
		}

		public void UpdateViewMatrix()
		{
			try
			{
				captionHorAndVert.GetNewViewport(Width, Height, out int[] viewPoint);
				WidthLocal = viewPoint[2];
				HeightLocal = viewPoint[3];

				double[] ortho;
				Ortho.CoefHeightToWidth = HeightLocal / (double)WidthLocal;
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

			foreach (var item in drawableObjects[PageType.None])
				item.Draw(axisType, BoundingBox, WidthLocal, HeightLocal, fontReceivers, paletteFont);

			foreach (var item in drawableObjects[page])
				item.Draw(axisType, BoundingBox, WidthLocal, HeightLocal, fontReceivers, paletteFont);


			DrawSelection();
		}
	}
}
