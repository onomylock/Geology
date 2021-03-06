using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Geology.Projection
{
     public enum TypeTransformation
        {
            alongX = 0, alongY, alongZ, alongWindow, aroundX, aroundY, aroundZ,None
        }
    public class CPerspective
    {
       
        double scale; 
        public double transPerHor, transPerVert;
   //     double transPerX, transPerY,transPerZ;
        MatrixPerspective matrixPersp;
        MatrixPerspective matrixOrth;
        double predRot;
        CCamera camera;
        public double Scale { get { return scale; } set { scale = value; } }

        public CCamera GetCamera { get { return camera; } }

        public MatrixPerspective MatrixPersp
        {
            get { return matrixPersp; }
            set { matrixPersp = value; }
        }
        public MatrixPerspective MatrixOrth
        {
            get { return matrixOrth; }
            set { matrixOrth = value; }
        }
        public CPerspective()
        {
            camera = new CCamera();
            matrixPersp = new MatrixPerspective();
            matrixOrth = new MatrixPerspective();
            scale = 1;
            transPerHor = transPerVert = 0;
    /*        transPerX = 0;
            transPerY = 0;
            transPerZ = 0;
            */
        }
        public void GetMatrixOrth(out double []res)
        {
            matrixOrth.GetMat(out res);
        }
        public void GetMatrixPersp(out double[] res)
        {
            matrixPersp.GetMat(out res);
        }
        void MultMatrixRot(double[] rotPer, double[] rotOrt)
        {
            matrixPersp.MultX(rotPer[0], rotPer[1], rotPer[2],
                       rotPer[3], rotPer[4], rotPer[5],
                       rotPer[6], rotPer[7], rotPer[8]);
            matrixOrth.Mult(rotOrt[0], rotOrt[1], rotOrt[2],
                       rotOrt[3], rotOrt[4], rotOrt[5],
                       rotOrt[6], rotOrt[7], rotOrt[8]);
         //   double newX, newY, newZ;
         //   camera.MultMatrixView(rotPer, transPerX, transPerY, transPerZ, out newX, out newY, out newZ);
          //  transPerX = newX; transPerY = newY; transPerZ = newZ;
        }
        public void ClearView()
        {
            scale = 1;
            transPerHor = transPerVert = 0;
            matrixPersp.ClearMatrix();
            matrixOrth.ClearMatrix();
        }
        public void setRotD(double Da, TypeTransformation type)
        {
            Da /= 360;
            Da *= 2 * Math.PI;
            double drot = Da; //- predRot;
            predRot = Da;
            switch (type)
            {

                case TypeTransformation.aroundX:
                    {
                        double[] rotPer = new double[]{ 1, 0, 0,
				                                        0, Math.Cos(drot), Math.Sin(drot),
				                                        0, -Math.Sin(drot), Math.Cos(drot) };
                        double[] rotOrt = new double[]{ 1, 0, 0,
				                                        0, Math.Cos(drot), -Math.Sin(drot),
				                                        0, Math.Sin(drot), Math.Cos(drot) };
                        MultMatrixRot(rotPer, rotOrt);
                    } break;
                case TypeTransformation.aroundY:
                    {
                        double[] rotPer = new double[]{ Math.Cos(drot), 0, -Math.Sin(drot),
				                                        0, 1, 0,
				                                        Math.Sin(drot), 0, Math.Cos(drot) };
                        double[] rotOrt = new double[] { Math.Cos(drot), 0, Math.Sin(drot),
				                                        0, 1, 0,
				                                        -Math.Sin(drot), 0, Math.Cos(drot) };
                        MultMatrixRot(rotPer, rotOrt);
                    } break;
                case TypeTransformation.aroundZ:
                    {
                        double[] rotPer = new double[] { Math.Cos(drot), Math.Sin(drot), 0,
				                                        -Math.Sin(drot), Math.Cos(drot), 0,
				                                        0, 0, 1 };
                        double[] rotOrt = new double[] { Math.Cos(drot), -Math.Sin(drot), 0,
				                                        Math.Sin(drot), Math.Cos(drot), 0,
				                                        0, 0, 1 };
                        MultMatrixRot(rotPer, rotOrt);
                    } break;
                default:
                    break;
            }
        }
        public void SetTrans(int width, int height, int transPerx, int transPery, TypeTransformation type, double[] BoundingBox)
	    {
		    int countangl = 6;
		    camera.InvalidateCamera();
		    double viewX = (BoundingBox[1] + BoundingBox[0]) / 2,
			    viewY = (BoundingBox[3] + BoundingBox[2]) / 2,
			    viewZ = (BoundingBox[5] + BoundingBox[4]) / 2;
          
            double dMax = Math.Max(Math.Max(viewX - BoundingBox[0], viewY - BoundingBox[2]), viewZ - BoundingBox[4]);
            double dMaxDiag = Math.Sqrt((BoundingBox[1] - BoundingBox[0])*(BoundingBox[1] - BoundingBox[0]) + 
                              (BoundingBox[3] - BoundingBox[2])*(BoundingBox[3] - BoundingBox[2]) + 
                               (BoundingBox[5] - BoundingBox[4])*(BoundingBox[5] - BoundingBox[4]));
            double AngleDmax = Math.Max(Math.Max((BoundingBox[1] - BoundingBox[0]) / 2, (BoundingBox[3] - BoundingBox[2]) / 2), (BoundingBox[5] - BoundingBox[4]) / 2);
		    double dView = AngleDmax / 2;
		    dMax = dMax * 2 + dView;
            double startAngle = Math.Atan(AngleDmax / (dMax));

		
		    camera.PositionCamera(viewX, viewY, viewZ + dMax , viewX, viewY, viewZ, 0, 1, 0);

		    //////////////////////////////////////////////////////////////////////////////////////////////
            if (startAngle * scale > Math.PI / countangl || startAngle * scale < 0)
		    {
                if (startAngle * scale >= Math.PI / 2)
			    {
				    camera.MoveCamera(0.001);
				    startAngle = 360.0 / countangl;
			    }
			    else
			    {
                    camera.MoveCamera(Math.Tan(Math.PI / countangl) / Math.Tan(startAngle * scale));
				    startAngle = 360.0 / countangl;
			    }
		    }
		    else
		    {
			    startAngle *= scale;
		    }

            double h = camera.GetDistanse();
            double curv = 2 * h * Math.Tan(startAngle);
            startAngle *= 360 / Math.PI;
           
          /*  switch (type)
            {
                case TypeTransformation.alongX: transPerX += transPerx * (curv) / (double)width; break;
                case TypeTransformation.alongY: transPerY += transPerx * (curv) / (double)width; break;
                case TypeTransformation.alongZ: transPerZ += transPerx * (curv) / (double)width; break;
                case TypeTransformation.alongWindow:
                    {
                        transPerHor += transPerx * (curv) / (double)width;
                        transPerVert += transPery * (curv) / (double)height;
                    } break;
            }*/
            transPerHor += transPerx * (curv) / (double)width;
            transPerVert += transPery * (curv) / (double)height;
     /*       double[] mat;
            matrixPersp.GetMat(out mat);
            camera.MultMatrixView(mat);
            camera.MultMatrixView(mat, transPerHor, transPerVert, 0);
            double dist = camera.GetDistanse();
      //      camera.MultMatrixViewTransXYZ(transPerX, transPerY, transPerZ);
            if (camera.GetDistanse()<dist)
               {
                   scale = Math.Atan(Math.Tan(Math.PI / countangl) / (dist / camera.GetDistanse()));
               }*/
	  }
      public void PrepareDraw(out double dMax, out double startAngle,out double dView, double[] BoundingBox)
	{
		
		int countangl = 6;
		camera.InvalidateCamera();
		double viewX = (BoundingBox[1] + BoundingBox[0]) / 2,
			viewY = (BoundingBox[3] + BoundingBox[2]) / 2,
			viewZ = (BoundingBox[5] + BoundingBox[4]) / 2;

		dMax = Math.Max(Math.Max(viewX - BoundingBox[0], viewY - BoundingBox[2]), viewZ - BoundingBox[4]);
		double AngleDmax = Math.Max(Math.Max((BoundingBox[1] - BoundingBox[0]) / 2, (BoundingBox[3] - BoundingBox[2]) / 2), (BoundingBox[5] - BoundingBox[4]) / 2 );
		dView = AngleDmax / 2;
		dMax = dMax*2+ dView;
		startAngle = Math.Atan(AngleDmax / (dMax));

		camera.PositionCamera(viewX, viewY, viewZ + dMax, viewX, viewY, viewZ, 0, 1, 0);

		//////////////////////////////////////////////////////////////////////////////////////////////
		if (startAngle*scale > Math.PI / countangl || startAngle*scale < 0)
		{
			if (startAngle*scale >= Math.PI/2)
			{
				camera.MoveCamera(0.001);
				startAngle = 360.0 / countangl;
			}
			else
			{
				camera.MoveCamera(Math.Tan(Math.PI / countangl) / Math.Tan(startAngle*scale));
				startAngle = 360.0 / countangl;
			}
		}
		else
		{
			startAngle *= scale * 360 / Math.PI;
		}


		double[]mat;
        matrixPersp.GetMat(out mat);
        camera.MultMatrixView(mat);
        camera.MultMatrixView(mat, transPerHor, transPerVert, 0);
     //   camera.MultMatrixViewTransXYZ(transPerX, transPerY, transPerZ);
     /*   if (camera.GetDistanse() < dMax - dView)
        {
            scale *= (dMax - dView) / camera.GetDistanse();
        }*/
	}
    }
}
