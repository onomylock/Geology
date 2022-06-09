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

		public int Width { get { return _Width; } set { _Width = value; } }
		public int Height { get { return _Height; } set { _Height = value; } }

		public int WidthLocal { get { return widthLocal; } set { widthLocal = value; } }
		public int HeightLocal { get { return heightLocal; } set { heightLocal = value; } }

		public int OglContex { get { return oglcontext; } set { oglcontext = value; } }
		public IntPtr Hdc { get { return hdc; } set { hdc = value; } }

		public double scaleV;
		private int widthLocal, heightLocal;

		protected readonly EPlaneType axisType;
		protected IntPtr hdc;
		protected int oglcontext;
		protected readonly PageType page;
		private readonly Dictionary<PageType, List<IViewportObjectsDrawable>> drawableObjects;
		private readonly CaptionAxisHorAndVert captionHorAndVert;
		private COrthoControlProport Ortho;
		private bool selectionStarted = false;
		private bool selectionFinished = true;
		private readonly double zRange = 1e+7;
		private double selectionX0, selectionX1;
		private double selectionY0, selectionY1;
		protected int _Width, _Height;
		protected readonly double[] BoundingBox;
		private readonly FontGeology fontReceivers;
		private readonly FontGeology paletteFont;

		

		public ViewWindow2D(CaptionAxisHorAndVert captionHorAndVert, COrthoControlProport Ortho, Dictionary<PageType, 
			List<IViewportObjectsDrawable>> drawableObjects, EPlaneType axisType, double zRange, PageType page, 
			int Width, int Height, double[] BoundingBox, FontGeology fontReceivers, FontGeology paletteFont, IntPtr Handle) : base()
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

			oglcontext = GLContex.InitOpenGL((int)Handle);
			hdc = Win32.GetDC(Handle);
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			GLContex.glClearColor(1, 1, 1, 1);

			GLContex.glEnable(GLContex.GL_DEPTH_TEST);
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public void Draw()
		{
			GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
			GLContex.glLoadIdentity();
			if (axisType == EPlaneType.XZ || axisType == EPlaneType.YZ)
				scaleV = GlobalDrawingSettings.ScaleZ;
			else
				scaleV = 1.0;
			captionHorAndVert.GenerateGrid(widthLocal, heightLocal, scaleV);
			captionHorAndVert.DrawScaleLbls(widthLocal, heightLocal, scaleV);
			DrawObjetcs();
		}

		public void UpdateViewMatrix()
		{
			try
			{
				captionHorAndVert.GetNewViewport(Width, Height, out int[] viewPoint);
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

			foreach (var item in drawableObjects[PageType.None])
				item.Draw(axisType, BoundingBox, widthLocal, heightLocal, fontReceivers, paletteFont);

			foreach (var item in drawableObjects[page])
				item.Draw(axisType, BoundingBox, widthLocal, heightLocal, fontReceivers, paletteFont);


			DrawSelection();
		}

		public void Paint(object sender, PaintEventArgs e)
		{
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			GLContex.glClearColor(1, 1, 1, 1);
			GLContex.glClear(GLContex.GL_COLOR_BUFFER_BIT | GLContex.GL_DEPTH_BUFFER_BIT);

			Draw();

			Win32.SwapBuffers(hdc);
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public void ResizeWindow()
		{
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			UpdateViewMatrix();
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public void Release()
		{
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public void Prepare()
		{
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			UpdateViewMatrix();
		}
	}
}
