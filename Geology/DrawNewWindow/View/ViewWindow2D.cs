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
using Geology.DrawNewWindow.Controller;


namespace Geology.DrawNewWindow.View
{
	class ViewWindow2D : ViewWindow3D
	{
		public PlaneType axisType;
		public double scaleV;
		public int WidthLocal, HeightLocal;

		private CaptionAxisHorAndVert captionHorAndVert;
		private COrthoControlProport Ortho;
		private bool selectionStarted = false;
		private bool selectionFinished = true;

		public ViewWindow2D(CaptionAxisHorAndVert captionHorAndVert, COrthoControlProport Ortho) : base()
		{
			this.captionHorAndVert = captionHorAndVert;
			this.Ortho = Ortho;
		}

		public override void Draw()
		{
			GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
			GLContex.glLoadIdentity();
			if (axisType == PlaneType.XZ || axisType == PlaneType.YZ)
				scaleV = GlobalDrawingSettings.ScaleZ;
			else
				scaleV = 1.0;
			captionHorAndVert.GenerateGrid(WidthLocal, HeightLocal, scaleV);
			captionHorAndVert.DrawScaleLbls(WidthLocal, HeightLocal, scaleV);
			DrawObjetcs();
		}

		public override void UpdateViewMatrix()
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
					case PlaneType.XY:
						GLContex.glVertex3d(selectionX0, selectionY0, zRange);
						GLContex.glVertex3d(selectionX1, selectionY0, zRange);
						GLContex.glVertex3d(selectionX1, selectionY1, zRange);
						GLContex.glVertex3d(selectionX0, selectionY1, zRange);
						break;
					case PlaneType.XZ:
						GLContex.glVertex3d(selectionX0, zRange, selectionY0);
						GLContex.glVertex3d(selectionX1, zRange, selectionY0);
						GLContex.glVertex3d(selectionX1, zRange, selectionY1);
						GLContex.glVertex3d(selectionX0, zRange, selectionY1);
						break;
					case PlaneType.YZ:
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
				case PlaneType.XZ:
					GLContex.gluLookAt(0.0, -1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);
					break;
				case PlaneType.YZ:
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
