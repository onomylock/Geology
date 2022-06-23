using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLContex = Geology.OpenGL.OpenGL;
using System.Collections.ObjectModel;
using Geology.Objects;
using Geology.OpenGL;
using System.Windows.Forms;
using Geology.DrawWindow;
using Geology.DrawNewWindow.Controller;

namespace Geology.DrawNewWindow.View
{
	public class ViewWindowCurve : ViewAbstract, IViewWindow
	{
		private CaptionAxisHorAndVert captionHorAndVert;
		private COrthoControlProport Ortho;
		private bool mZoomStarted;
		private ObservableCollection<Objects.CCurve> Curves;
		private ObservableCollection<Objects.CCurveInfo> CurvesInfoList;
		private Rect mRect;
		private double Arg;

		public ViewWindowCurve(CaptionAxisHorAndVert captionHorAndVert, COrthoControlProport Ortho, 
			ObservableCollection<Objects.CCurveInfo> CurvesInfoList, Rect mRect, bool mZoomStarted, ObservableCollection<Objects.CCurve> Curves, double Arg, IntPtr Handle)
		{
			this.captionHorAndVert = captionHorAndVert;
			this.Curves = Curves;
			this.CurvesInfoList = CurvesInfoList;
			this.mRect = mRect;
			this.mZoomStarted = mZoomStarted;
			this.Ortho = Ortho;
			this.Arg = Arg;

			oglcontext = GLContex.InitOpenGL((int)Handle);
			hdc = Win32.GetDC(Handle);
			Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
			GLContex.glClearColor(1, 1, 1, 1);

			GLContex.glEnable(GLContex.GL_DEPTH_TEST);
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}

		public override void Draw()
		{
			GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
			GLContex.glLoadIdentity();
			captionHorAndVert.GenerateGrid(Width, Height);
			captionHorAndVert.DrawScaleLbls(Width, Height);
			if (CurvesInfoList != null)
				DrawObjects();
		}

		protected void DrawObjects()
		{
			double[] ortho;
			Ortho.GetOrtho(out ortho);
			int indent = captionHorAndVert.myfontHor.GetHeightText("0");
			// curves
			
			for (int i = Math.Min(Curves.Count, CurvesInfoList.Count) - 1; i >= 0; i--)
			{
				if (CurvesInfoList == null || !CurvesInfoList[i].Visible)
					continue;
				System.Windows.Media.Color col = CurvesInfoList[i].Color;
				GLContex.glColor3f(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f);
				Curves[i].Draw(
					captionHorAndVert.LogH,
					captionHorAndVert.ZeroLogH,
					captionHorAndVert.LogV,
					captionHorAndVert.ZeroLogV,
					Ortho.GetDHor / WidthLocal,
					Ortho.GetDVert / HeightLocal,
					CurvesInfoList[i],
					captionHorAndVert.myfontHor,
					ortho,
					Height,
					Width,
					indent);
			}

			// red line
			double[] _Ortho;
			double _ArgToDraw = (captionHorAndVert.LogH ? Objects.CCurve.GetLog(Arg, captionHorAndVert.ZeroLogH) : Arg);
			Ortho.GetOrtho(out _Ortho);
			GLContex.glColor3f(1, 0, 0);
			GLContex.glLineWidth(1.0f);
			GLContex.glDisable(GLContex.GL_DEPTH_TEST);
			GLContex.glBegin(GLContex.GL_LINES);
			GLContex.glVertex3f((float)_ArgToDraw, (float)_Ortho[2], 0);
			GLContex.glVertex3f((float)_ArgToDraw, (float)_Ortho[3], 0);
			GLContex.glEnd();
			GLContex.glEnable(GLContex.GL_DEPTH_TEST);

			// zoom
			if (mZoomStarted)
			{
				// xor zoom rectangle
				GLContex.glEnable(GLContex.GL_COLOR_LOGIC_OP);
				GLContex.glLogicOp(GLContex.GL_XOR);

				GLContex.glColor3f(1, 1, 1);
				GLContex.glLineWidth(1.0f);

				GLContex.glBegin(GLContex.GL_LINE_LOOP);
				GLContex.glVertex3f((float)mRect.x1, (float)mRect.y1, 0);
				GLContex.glVertex3f((float)mRect.x2, (float)mRect.y1, 0);
				GLContex.glVertex3f((float)mRect.x2, (float)mRect.y2, 0);
				GLContex.glVertex3f((float)mRect.x1, (float)mRect.y2, 0);
				GLContex.glEnd();

				GLContex.glDisable(GLContex.GL_COLOR_LOGIC_OP);
			}
		}

		public override void UpdateViewMatrix()
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
	}
}
