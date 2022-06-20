﻿using System;
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

namespace Geology.DrawNewWindow.View
{
    public class ViewWindow3D : ViewAbstract, IViewWindow
    {
        public ViewWindow3D(CPerspective project, List<IViewportObjectsDrawable> viewportObjectsDrawables, double[] BoundingBox,
            PageType page, int Width, int Height, IntPtr Handle)
        {
            base.viewportObjectsDrawables = viewportObjectsDrawables;
            base.BoundingBox = BoundingBox;
            //base.caption = caption;
            base.project = project;
            base.Height = Height;
            base.Width = Width;
            base.page = page;

            oglcontext = GLContex.InitOpenGL((int)Handle);
            hdc = Win32.GetDC(Handle);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext);
            GLContex.glClearColor(1, 1, 1, 1);
            caption = new FontGeology(hdc, oglcontext, FontGeology.TypeFont.Horizontal, "Arial", 16);
            GLContex.glEnable(GLContex.GL_DEPTH_TEST);
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

		public override void Draw()
        {
            double scale = 1.0;
            if (page == PageType.ViewModel)
                GLContex.glEnable(GLContex.GL_LIGHT_MODEL_TWO_SIDE);
            GLContex.glEnable(GLContex.GL_NORMALIZE);


            var lightSettings = DrawWindow.GlobalDrawingSettings.LightSettings;

            if (lightSettings.Enabled)
            {
                CCamera myCam = project.GetCamera;
                double xVector = myCam.m_vPosition.X - myCam.m_vView.X;
                double yVector = myCam.m_vPosition.Y - myCam.m_vView.Y;
                double zVector = myCam.m_vPosition.Z - myCam.m_vView.Z;
                xVector = myCam.m_vPosition.X;// + xVector * 3;
                yVector = myCam.m_vPosition.Y;// + yVector * 3;
                zVector = myCam.m_vPosition.Z;// + zVector * 3;
                                              //float[] position = { (float)xVector, (float)yVector, (float)zVector, 1f };
                var position = new float[] { lightSettings.X, lightSettings.Y, (float)(lightSettings.Z * scale), 1f };

                GLContex.glEnable(GLContex.GL_LIGHTING);
                GLContex.glEnable(GLContex.GL_LIGHT1);
                GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);
                GLContex.glEnable(GLContex.GL_NORMALIZE);

                float[] light_ambient = { lightSettings.Ambient, lightSettings.Ambient, lightSettings.Ambient, 1.0f };
                float[] light_diffuse = { lightSettings.Diffuse, lightSettings.Diffuse, lightSettings.Diffuse, 1.0f };
                float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                float[] mS = { lightSettings.Specular, lightSettings.Specular, lightSettings.Specular };


                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_AMBIENT, light_ambient);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_DIFFUSE, light_diffuse);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_SPECULAR, light_specular);
                GLContex.glLightfv(GLContex.GL_LIGHT1, GLContex.GL_POSITION, position);
                GLContex.glMaterialfv(GLContex.GL_FRONT, GLContex.GL_SPECULAR, mS); // отраженный свет
                GLContex.glMaterialf(GLContex.GL_FRONT, GLContex.GL_SHININESS, lightSettings.Shininess); // степень отраженного света

                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_DIFFUSE, light_ambient); // цвет чайника
                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SPECULAR, light_ambient); // отраженный свет
                GLContex.glMaterialfv(GLContex.GL_BACK, GLContex.GL_SHININESS, light_ambient); // степень отраженного света
            }

            // Следующая строка позволяет закрашивать полигоны цветом при включенном освещении:
            GLContex.glEnable(GLContex.GL_COLOR_MATERIAL);

            foreach (var p in viewportObjectsDrawables)
                p.Draw3D(true, p.DrawColor);

            GLContex.glDisable(GLContex.GL_LIGHT0);
            GLContex.glDisable(GLContex.GL_LIGHT1);
            GLContex.glDisable(GLContex.GL_LIGHTING);
            GLContex.glDisable(GLContex.GL_NORMALIZE);

            GLContex.glDisable(GLContex.GL_COLOR_MATERIAL);

            draw3DAxis();
        }

        private void draw3DAxis() //рисовка рамки с осями
        {
            {
                GLContex.glLineWidth(1);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glPushMatrix();
                GLContex.glLoadIdentity();
                GLContex.glMatrixMode(GLContex.GL_PROJECTION);

                GLContex.glPushMatrix();
                GLContex.glViewport(0, 0, 80, 80);
                GLContex.glLoadIdentity();
                GLContex.glClearColor(1, 1, 1, 1);
                GLContex.glOrtho(-20, 20, -20, 20, -20, 20);

                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glColor3f(0, 0, 1.0f);
                GLContex.glLoadIdentity();


                double[] mat;
                project.GetMatrixOrth(out mat);
                double[] multmat = new double[]{
                    mat[0], mat[3], mat[6], 0,
                    mat[1], mat[4], mat[7], 0,
                    mat[2], mat[5], mat[8], 0,
                    0, 0, 0, 1
                };

                GLContex.glMultMatrixd(multmat);
                GLContex.glBegin(GLContex.GL_LINES);
                GLContex.glColor3f(1, 0, 0);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(10, 0, 0);
                GLContex.glColor3f(0, 1, 0);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(0, 10, 0);
                GLContex.glColor3f(0, 0, 1);
                GLContex.glVertex3f(0, 0, 0);
                GLContex.glVertex3f(0, 0, 10);
                GLContex.glEnd();

                double sizeArrow = 1, sizeArrowAx = 7;
                //glEnable(GL_DEPTH_TEST);
                GLContex.glBegin(GLContex.GL_TRIANGLES);

                GLContex.glColor3f(1, 0, 0);
                GLContex.glVertex3f(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(10, 0, 0);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);

                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, sizeArrow);

                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, sizeArrow, -sizeArrow);
                GLContex.glVertex3d(sizeArrowAx, -sizeArrow, -sizeArrow);

                GLContex.glColor3f(0, 1, 0);
                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(0, 10, 0);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, sizeArrow);

                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, sizeArrow);
                GLContex.glVertex3d(sizeArrow, sizeArrowAx, -sizeArrow);
                GLContex.glVertex3d(-sizeArrow, sizeArrowAx, -sizeArrow);

                GLContex.glColor3f(0, 0, 1);
                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(0, 0, 10);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);

                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, sizeArrow, sizeArrowAx);


                GLContex.glVertex3d(-sizeArrow, sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(sizeArrow, -sizeArrow, sizeArrowAx);
                GLContex.glVertex3d(-sizeArrow, -sizeArrow, sizeArrowAx);


                GLContex.glEnd();
                GLContex.glColor3f(1, 0, 0);
                caption.PrintText(12, 0, 0, "X");
                GLContex.glColor3f(0, 1, 0);
                caption.PrintText(0, 12, 0, "Y");
                GLContex.glColor3f(0, 0, 1);
                caption.PrintText(0, 0, 12, "Z");

                GLContex.glMatrixMode(GLContex.GL_PROJECTION);
                GLContex.glPopMatrix();
                GLContex.glViewport(0, 0, Width, Height);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);
                GLContex.glPopMatrix();
            }
        }

        public override void UpdateViewMatrix()
        {
            try
            {
                GLContex.glMatrixMode(GLContex.GL_PROJECTION);
                GLContex.glLoadIdentity();
                double dMax, startAngle, dView;
                GLContex.glViewport(0, 0, width, height);
                project.PrepareDraw(out dMax, out startAngle, out dView, BoundingBox);

                GLContex.gluPerspective(startAngle, width / (double)height, dView, (dMax) * 30);
                GLContex.glMatrixMode(GLContex.GL_MODELVIEW);     // To operate on model-view matrix
                GLContex.glLoadIdentity();
                CCamera tmpCam = project.GetCamera;
                GLContex.gluLookAt(tmpCam.m_vPosition.X, tmpCam.m_vPosition.Y, tmpCam.m_vPosition.Z /* mesh3D.BoundingBox[0] - 0.1*/,
                    tmpCam.m_vView.X, tmpCam.m_vView.Y, tmpCam.m_vView.Z,
                    tmpCam.m_vUpVector.X, tmpCam.m_vUpVector.Y, tmpCam.m_vUpVector.Z);
            }
            catch (Exception ex)
            {

            }
        }
	}
}
