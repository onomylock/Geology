﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geology.Projection;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.OpenGL;
using Geology.Objects.GeoModel;
using Geology.Objects;
using Geology.DrawNewWindow.Model;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.View
{
	public abstract class ViewAbstract : IViewWindow
	{
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }

		public int WidthLocal { get { return widthLocal; } set { widthLocal = value; } }
		public int HeightLocal { get { return heightLocal; } set { heightLocal = value; } }

		public int OglContext { get { return oglcontext; } set { oglcontext = value; } }
		public IntPtr Hdc { get { return hdc; } set { hdc = value; } }

		public FontGeology caption { get; set; }
		public FontGeology wellFont { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public FontGeology fontReceivers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public FontGeology paletteFont { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public CaptionAxisHorAndVert CaptionHorAndVert { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		protected IntPtr hdc;
		protected int oglcontext;
		protected CPerspective project;
		protected IModelWindow model;
		protected PageType page;
		protected int widthLocal, heightLocal;
		protected int height;
		protected int width;
		protected double[] BoundingBox;

		public virtual void Draw()
		{
			throw new NotImplementedException();
		}

		public virtual void UpdateViewMatrix()
		{
			throw new NotImplementedException();
		}

		public virtual void DisposedView(object sender, EventArgs e)
		{
			Win32.wglMakeCurrent(Hdc, (IntPtr)OglContext);
			caption.ClearFont();
			Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
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
