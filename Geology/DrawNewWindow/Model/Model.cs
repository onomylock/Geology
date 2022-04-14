using Geology.DrawWindow;
using Geology.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Geology.DrawWindow.CObject3DDraw2D;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Objects;
using Geology.Objects.SaturationModel;

namespace Geology.DrawNewWindow.Model
{
    public static class ModelSettings
    {
        public static double stepX = 200.0;
        public static double stepY = 200.0;
        public static Dictionary<String, System.Reflection.PropertyInfo[]> FacePropertiesX;
        public static Dictionary<String, System.Reflection.PropertyInfo[]> FacePropertiesY;
        public static Dictionary<String, System.Reflection.PropertyInfo[]> FacePropertiesZ;
        public static bool UseZSparse = false;
        public static double ZSparse = 1.5;

        static ModelSettings()
        {
            var propX0 = typeof(CGeoObject).GetProperty("X0");
            var propX1 = typeof(CGeoObject).GetProperty("X1");
            var propY0 = typeof(CGeoObject).GetProperty("Y0");
            var propY1 = typeof(CGeoObject).GetProperty("Y1");
            var propZ0 = typeof(CGeoObject).GetProperty("Z0");
            var propZ1 = typeof(CGeoObject).GetProperty("Z1");
            FacePropertiesX = new Dictionary<string, System.Reflection.PropertyInfo[]>();
            FacePropertiesX.Add("X0", new System.Reflection.PropertyInfo[] { propX0, propX0, propX0, propX0 });
            FacePropertiesX.Add("X1", new System.Reflection.PropertyInfo[] { propX1, propX1, propX1, propX1 });
            FacePropertiesX.Add("Y0", new System.Reflection.PropertyInfo[] { propX0, propX1, propX1, propX0 });
            FacePropertiesX.Add("Y1", new System.Reflection.PropertyInfo[] { propX0, propX1, propX1, propX0 });
            FacePropertiesX.Add("Z0", new System.Reflection.PropertyInfo[] { propX0, propX1, propX1, propX0 });
            FacePropertiesX.Add("Z1", new System.Reflection.PropertyInfo[] { propX0, propX1, propX1, propX0 });

            FacePropertiesY = new Dictionary<string, System.Reflection.PropertyInfo[]>();
            FacePropertiesY.Add("X0", new System.Reflection.PropertyInfo[] { propY0, propY1, propY1, propY0 });
            FacePropertiesY.Add("X1", new System.Reflection.PropertyInfo[] { propY0, propY1, propY1, propY0 });
            FacePropertiesY.Add("Y0", new System.Reflection.PropertyInfo[] { propY0, propY0, propY0, propY0 });
            FacePropertiesY.Add("Y1", new System.Reflection.PropertyInfo[] { propY1, propY1, propY1, propY1 });
            FacePropertiesY.Add("Z0", new System.Reflection.PropertyInfo[] { propY0, propY0, propY1, propY1 });
            FacePropertiesY.Add("Z1", new System.Reflection.PropertyInfo[] { propY0, propY0, propY1, propY1 });

            FacePropertiesZ = new Dictionary<string, System.Reflection.PropertyInfo[]>();
            FacePropertiesZ.Add("X0", new System.Reflection.PropertyInfo[] { propZ0, propZ0, propZ1, propZ1 });
            FacePropertiesZ.Add("X1", new System.Reflection.PropertyInfo[] { propZ0, propZ0, propZ1, propZ1 });
            FacePropertiesZ.Add("Y0", new System.Reflection.PropertyInfo[] { propZ0, propZ0, propZ1, propZ1 });
            FacePropertiesZ.Add("Y1", new System.Reflection.PropertyInfo[] { propZ0, propZ0, propZ1, propZ1 });
            FacePropertiesZ.Add("Z0", new System.Reflection.PropertyInfo[] { propZ0, propZ0, propZ0, propZ0 });
            FacePropertiesZ.Add("Z1", new System.Reflection.PropertyInfo[] { propZ1, propZ1, propZ1, propZ1 });
        }
        static public int WriteB(BinaryWriter outputFile)
        {
            try
            {
                outputFile.Write(UseZSparse);
                outputFile.Write(ZSparse);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        static public int ReadB(BinaryReader inputFile, double fileVersion)
        {
            try
            {
                UseZSparse = inputFile.ReadBoolean();
                ZSparse = inputFile.ReadDouble();

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class ModelLayer
    {
        double z0 = 0, z1 = 0;
        int nnx, nny, nnz, nnxy, nn;
        int nex, ney, nez, nexy, ne;
        public List<ModelObject> objects = new List<ModelObject>();
        public double[] meshXSource = null, meshYSource = null, meshZetaSource = null, nodesZSource = null;
        public double[] meshX = null, meshY = null, meshZeta = null, nodesZ = null;
        public int[] objectInCell = null;
        public bool[] visible = null;
        public bool[] hidden = null;
        public bool[] visibleFaces = null;
        public double Z0
        {
            get { return z0; }
            set { z0 = value; }
        }
        public double Z1
        {
            get { return z1; }
            set { z1 = value; }
        }

        public ModelLayer()
        {

        }

        public double CalculateZeta(double z)
        {
            if (z < z0 - 1e-10) return -1;
            if (z > z1 + 1e-10) return -1;

            return (z - z0) / (z1 - z0);
        }
        public bool ZInLayer(double z)
        {
            if (z < z0) return false;
            if (z > z1) return false;

            return true;
        }

        public void BuildDiscretization(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            Build1DMeshes(x0, x1, y0, y1, z0, z1);
            SetObjectsInCells();
        }
        private void Build1DMeshes(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            int n;

            SortedSet<double> coords = new SortedSet<double>();

            coords.Clear();
            coords.Add(x0);
            coords.Add(x1);
            foreach (var obj in objects)
            {
                coords.Add(obj.X0);
                coords.Add(obj.X1);
            }
            Refine1DMesh(coords, ModelSettings.stepX);
            meshXSource = new double[coords.Count];
            n = 0;
            foreach (var coord in coords)
            {
                meshXSource[n] = coord;
                n++;
            }

            coords.Clear();
            coords.Add(y0);
            coords.Add(y1);
            foreach (var obj in objects)
            {
                coords.Add(obj.Y0);
                coords.Add(obj.Y1);
            }
            Refine1DMesh(coords, ModelSettings.stepY);
            meshYSource = new double[coords.Count];
            n = 0;
            foreach (var coord in coords)
            {
                meshYSource[n] = coord;
                n++;
            }

            coords.Clear();
            coords.Add(0);
            coords.Add(1);
            foreach (var obj in objects)
            {
                coords.Add(obj.Zeta0);
                coords.Add(obj.Zeta1);
            }
            meshZetaSource = new double[coords.Count];
            n = 0;
            foreach (var coord in coords)
            {
                meshZetaSource[n] = coord;
                n++;
            }

            nnx = meshXSource.Length;
            nny = meshYSource.Length;
            nnz = meshZetaSource.Length;
            nnxy = nnx * nny;
            nn = nnxy * nnz;

            nex = nnx - 1;
            ney = nny - 1;
            nez = nnz - 1;
            nexy = nex * ney;
            ne = nexy * nez;

            objectInCell = new int[ne];
            visible = new bool[ne];
            visibleFaces = new bool[ne * 6];
            hidden = new bool[ne];
        }
        private void Refine1DMesh(SortedSet<double> coords, double step)
        {
            List<double> coordsL = coords.ToList();
            for (int i = 0; i < coordsL.Count - 1; i++)
            {
                while (coordsL[i + 1] - coordsL[i] > step)
                {
                    double c = (coordsL[i + 1] + coordsL[i]) * 0.5;
                    coords.Add(c);
                    coordsL = coords.ToList();
                }
            }
        }
        private void SetObjectsInCells()
        {
            int e, o;
            for (int i = 0; i < objectInCell.Length; i++)
                objectInCell[i] = -1;

            o = 0;
            foreach (var obj in objects)
            {
                obj.ix0 = Utilities.LittleTools.PointInArray1D(meshXSource, obj.X0);
                obj.ix1 = Utilities.LittleTools.PointInArray1D(meshXSource, obj.X1);
                obj.iy0 = Utilities.LittleTools.PointInArray1D(meshYSource, obj.Y0);
                obj.iy1 = Utilities.LittleTools.PointInArray1D(meshYSource, obj.Y1);
                obj.iz0 = Utilities.LittleTools.PointInArray1D(meshZetaSource, obj.Zeta0);
                obj.iz1 = Utilities.LittleTools.PointInArray1D(meshZetaSource, obj.Zeta1);

                if (obj.ix0 != -1 && obj.ix1 != -1 && obj.iy0 != -1 && obj.iy1 != -1 && obj.iz0 != -1 && obj.iz1 != -1)
                {
                    for (int iz = obj.iz0; iz < obj.iz1; iz++)
                        for (int iy = obj.iy0; iy < obj.iy1; iy++)
                            for (int ix = obj.ix0; ix < obj.ix1; ix++)
                            {
                                e = nexy * iz + nex * iy + ix;
                                objectInCell[e] = o;
                            }
                }
                o++;
            }
        }
        //public void UpdateDrawingGeometry()
        //{
        //    int e;

        //    for (int i = 0; i < ne; i++)
        //        hidden[i] = false;
        //    for (int i = 0; i < ne * 6; i++)
        //        visibleFaces[i] = true;

        //    /*
        //    foreach (var obj in objects)
        //    {
        //        if (obj.Visible)
        //            for (int iz = obj.iz0; iz < obj.iz1; iz++)
        //                for (int iy = obj.iy0; iy < obj.iy1; iy++)
        //                    for (int ix = obj.ix0; ix < obj.ix1; ix++)
        //                    {
        //                        e = nexy * iz + nex * iy + ix;
        //                        hidden[e] = true;
        //                    }
        //    }
        //    */

        //    for (int iz = 0; iz < nez; iz++)
        //        for (int iy = 0; iy < ney; iy++)
        //            for (int ix = 0; ix < nex; ix++)
        //            {
        //                e = nexy * iz + nex * iy + ix;
        //                if (iz == 0)
        //                {
        //                    visibleFaces[e * 6 + 4] = true;
        //                    if (iz < nez - 1)
        //                        visibleFaces[e * 6 + 5] = hidden[e + nexy];
        //                }
        //                if (iz == nez - 1)
        //                {
        //                    visibleFaces[e * 6 + 5] = true;
        //                    if (iz > 0)
        //                        visibleFaces[e * 6 + 4] = hidden[e - nexy];
        //                }
        //                if (iy == 0)
        //                {
        //                    visibleFaces[e * 6 + 2] = true;
        //                    if (iy < ney - 1)
        //                        visibleFaces[e * 6 + 3] = hidden[e + nex];
        //                }
        //                if (iy == ney - 1)
        //                {
        //                    visibleFaces[e * 6 + 3] = true;
        //                    if (iy > 0)
        //                        visibleFaces[e * 6 + 2] = hidden[e - nex];
        //                }
        //                if (ix == 0)
        //                {
        //                    visibleFaces[e * 6 + 0] = true;
        //                    if (ix < nex - 1)
        //                        visibleFaces[e * 6 + 1] = hidden[e + 1];
        //                }
        //                if (ix == nez - 1)
        //                {
        //                    visibleFaces[e * 6 + 1] = true;
        //                    if (ix > 0)
        //                        visibleFaces[e * 6 + 0] = hidden[e - 1];
        //                }
        //            }
        //    for (int i = 0; i < ne * 6; i++)
        //    {
        //        visibleFaces[i] = !visibleFaces[i];
        //    }
        //}
        //public void UpdateDrawingColors()
        //{

        //}
        //public void UpdateDrawingData()
        //{
        //    UpdateDrawingGeometry();

        //}
        private void GetFacesNodes(int faceNumber, int node, int[] nodes)
        {
            switch (faceNumber)
            {
                case 0:
                    nodes[0] = node;
                    nodes[1] = node + nnx;
                    nodes[2] = node + nnxy + nnx;
                    nodes[3] = node + nnxy;
                    break;
                case 1:
                    nodes[0] = node + 1;
                    nodes[1] = node + 1 + nnx;
                    nodes[2] = node + 1 + nnxy + nnx;
                    nodes[3] = node + 1 + nnxy;
                    break;
                case 2:
                    nodes[0] = node;
                    nodes[1] = node + 1;
                    nodes[2] = node + nnxy + 1;
                    nodes[3] = node + nnxy;
                    break;
                case 3:
                    nodes[0] = node + nnx;
                    nodes[1] = node + nnx + 1;
                    nodes[2] = node + nnx + nnxy + 1;
                    nodes[3] = node + nnx + nnxy;
                    break;
                case 4:
                    nodes[0] = node;
                    nodes[1] = node + 1;
                    nodes[2] = node + nnx + 1;
                    nodes[3] = node + nnx;
                    break;
                case 5:
                    nodes[0] = node + nnxy;
                    nodes[1] = node + nnxy + 1;
                    nodes[2] = node + nnxy + nnx + 1;
                    nodes[3] = node + nnxy + nnx;
                    break;
            }
        }
        private void GetFacesNodes(int faceNumber, int ix, int iy, int[] nodesX, int[] nodesY)
        {
            switch (faceNumber)
            {
                case 0:
                    nodesX[0] = ix;
                    nodesX[1] = ix;
                    nodesX[2] = ix;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;
                    break;
                case 1:
                    nodesX[0] = ix + 1;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix + 1;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;
                    break;
                case 2:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy;
                    nodesY[2] = iy;
                    nodesY[3] = iy;
                    break;
                case 3:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy + 1;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy + 1;
                    break;
                case 4:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;
                    break;
                case 5:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;
                    break;
            }
        }
        private void GetFacesNodes(int faceNumber, int ix, int iy, int iz, int[] nodesX, int[] nodesY, int[] nodesZ)
        {
            switch (faceNumber)
            {
                case 0:
                    nodesX[0] = ix;
                    nodesX[1] = ix;
                    nodesX[2] = ix;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;

                    nodesZ[0] = iz;
                    nodesZ[1] = iz;
                    nodesZ[2] = iz + 1;
                    nodesZ[3] = iz + 1;
                    break;
                case 1:
                    nodesX[0] = ix + 1;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix + 1;

                    nodesY[0] = iy;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy;

                    nodesZ[0] = iz;
                    nodesZ[1] = iz;
                    nodesZ[2] = iz + 1;
                    nodesZ[3] = iz + 1;
                    break;
                case 2:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy;
                    nodesY[2] = iy;
                    nodesY[3] = iy;

                    nodesZ[0] = iz;
                    nodesZ[1] = iz;
                    nodesZ[2] = iz + 1;
                    nodesZ[3] = iz + 1;
                    break;
                case 3:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy + 1;
                    nodesY[1] = iy + 1;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy + 1;

                    nodesZ[0] = iz;
                    nodesZ[1] = iz;
                    nodesZ[2] = iz + 1;
                    nodesZ[3] = iz + 1;
                    break;
                case 4:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy + 1;

                    nodesZ[0] = iz;
                    nodesZ[1] = iz;
                    nodesZ[2] = iz;
                    nodesZ[3] = iz;
                    break;
                case 5:
                    nodesX[0] = ix;
                    nodesX[1] = ix + 1;
                    nodesX[2] = ix + 1;
                    nodesX[3] = ix;

                    nodesY[0] = iy;
                    nodesY[1] = iy;
                    nodesY[2] = iy + 1;
                    nodesY[3] = iy + 1;

                    nodesZ[0] = iz + 1;
                    nodesZ[1] = iz + 1;
                    nodesZ[2] = iz + 1;
                    nodesZ[3] = iz + 1;
                    break;
            }
        }
        private void DrawFace(int faceNumber, int ix, int iy, int iz)
        {
            double zb, zt;
            switch (faceNumber)
            {
                case 0:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    break;
                case 1:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    break;
                case 2:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    break;
                case 3:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    break;
                case 4:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    break;
                case 5:
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    break;
            }
        }
        private void DrawFaceEdge(int faceNumber, int ix, int iy, int iz)
        {
            double zb, zt;
            switch (faceNumber)
            {
                case 0:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    break;
                case 1:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    break;
                case 2:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    break;
                case 3:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    break;
                case 4:
                    zb = z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz];

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zb);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zb);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zb);
                    break;
                case 5:
                    zt = z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1];

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], zt);
                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);

                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], zt);
                    GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], zt);
                    break;
            }
        }
        public void Draw()
        {
            int e, n;
            int[] nodesZ = new int[4];
            int[] nodesX = new int[4];
            int[] nodesY = new int[4];
            double x, y, z;
            //========================================================================================= faces

            GLContex.glDisable(GLContex.GL_BLEND);
            GLContex.glDisable(GLContex.GL_SMOOTH);

            GLContex.glEnable(GLContex.GL_POLYGON_OFFSET_FILL);
            GLContex.glPolygonOffset(1, 2);

            GLContex.glBegin(GLContex.GL_QUADS);
            e = 0;
            for (int iz = 0; iz < nez; iz++)
                for (int iy = 0; iy < ney; iy++)
                {
                    n = iz * nnxy + iy * nnx;
                    for (int ix = 0; ix < nex; ix++, e++, n++)
                    {

                        if (objectInCell[e] != -1)
                            GLContex.glColor3f(1.0f, 0.0f, 0.0f);
                        else
                            GLContex.glColor3f(0.0f, 1.0f, 1.0f);
                        for (int j = 0; j < 6; j++)
                            if (visibleFaces[e * 6 + j])
                            {
                                DrawFace(j, ix, iy, iz);
                                /*
                                GetFacesNodes(j, ix, iy, iz, nodesX, nodesY, nodesZ);
                                for (int k = 0; k < 4; k++)
                                {
                                    x = meshXSource[nodesX[k]];
                                    y = meshYSource[nodesY[k]];
                                    z = z0 * (1.0 - meshZetaSource[nodesZ[k]]) + z1 * meshZetaSource[nodesZ[k]];
                                    GLContex.glVertex3d(meshXSource[nodesX[k]], meshYSource[nodesY[k]], z0 * (1.0 - meshZetaSource[nodesZ[k]]) + z1 * meshZetaSource[nodesZ[k]]);
                                }
                                */
                            }
                    }
                }
            GLContex.glEnd();


            GLContex.glDisable(GLContex.GL_LINE_SMOOTH);

            GLContex.glLineWidth(1);
            GLContex.glColor3f(0.0f, 0.0f, 0.0f);

            GLContex.glBegin(GLContex.GL_LINES);
            e = 0;
            for (int iz = 0; iz < nez; iz++)
                for (int iy = 0; iy < ney; iy++)
                {
                    for (int ix = 0; ix < nex; ix++, e++)
                    {
                        for (int j = 0; j < 6; j++)
                            if (visibleFaces[e * 6 + j])
                                DrawFaceEdge(j, ix, iy, iz);
                    }
                }
            GLContex.glEnd();



            //========================================================================================= lines

            GLContex.glEnable(GLContex.GL_BLEND);
            GLContex.glDisable(GLContex.GL_LINE_SMOOTH);


            GLContex.glLineWidth(1);
            GLContex.glColor3f(0.0f, 0.0f, 0.0f);

            /*
            GLContex.glBegin(GLContex.GL_LINES);
            e = 0;
            for (int iz = 0; iz < nnz; iz++)
                for (int iy = 0; iy < nny; iy++)
                {
                    n = iz * nnxy + iy * nnx;
                    for (int ix = 0; ix < nnx; ix++, e++, n++)
                    {
                        if (ix < nex)
                        {
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                            GLContex.glVertex3d(meshXSource[ix + 1], meshYSource[iy], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                        }

                        if (iy < ney)
                        {
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy + 1], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                        }

                        if (iz < nez)
                        {
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], z0 * (1.0 - meshZetaSource[iz]) + z1 * meshZetaSource[iz]);
                            GLContex.glVertex3d(meshXSource[ix], meshYSource[iy], z0 * (1.0 - meshZetaSource[iz + 1]) + z1 * meshZetaSource[iz + 1]);
                        }
                    }
                }

            GLContex.glEnd();
            */
        }
    }
    public class ModelObject
    {
        bool visible = true;
        double zeta0 = 0, zeta1 = 0;
        double x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;
        public int ix0 = 0, ix1 = 0, iy0 = 0, iy1 = 0, iz0 = 0, iz1 = 0;

        public double X0
        {
            get { return x0; }
            set { x0 = value; }
        }
        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        public double Y0
        {
            get { return y0; }
            set { y0 = value; }
        }
        public double Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        public double Z0
        {
            get { return z0; }
            set { z0 = value; }
        }
        public double Z1
        {
            get { return z1; }
            set { z1 = value; }
        }
        public double Zeta0
        {
            get { return zeta0; }
            set { zeta0 = value; }
        }
        public double Zeta1
        {
            get { return zeta1; }
            set { zeta1 = value; }
        }
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        public ModelObject()
        {

        }
    }
    public class Model : IModel, INotifyPropertyChanged, IViewportObjectsDrawable, IViewportMouseMoveReaction, IViewportObjectsSelectable, IViewportObjectsClickable
    {
        private class HighlightedObjectInfo
        {

            DataGrid dataGridObjects = null;
            private double draggingC10 = 0;
            private double draggingC20 = 0;
            CGeoObject highlightedObject = null;
            public bool Dragging { set; get; } = false;
            public List<CGeoObject> SelectedObjects { get; } = new List<CGeoObject>();
            public List<int> SelectedHexVertices { get; } = new List<int>();
            public String Face { set; get; } = "";
            public int HexVertex { set; get; } = -1;
            public CGeoObject HighlightedObject
            {
                get { return highlightedObject; }
                set { highlightedObject = value; if (highlightedObject == null) Face = ""; }
            }
            public DataGrid DataGridObjects
            {
                get { return dataGridObjects; }
                set { dataGridObjects = value; dataGridObjects.SelectionChanged += DataGridObjects_SelectionChanged; }
            }


            public HighlightedObjectInfo()
            {

            }

            public bool Drag(double c1, double c2, EPlaneType planeType)
            {
                if (HighlightedObject == null && !(SelectedObjects.Count == 1 && SelectedObjects[0].IsHex))
                    return false;

                String c1Name = null;
                String c2Name = null;

                switch (planeType)
                {
                    case EPlaneType.XY: c1Name = "X"; c2Name = "Y"; break;
                    case EPlaneType.XZ: c1Name = "X"; c2Name = "Z"; break;
                    case EPlaneType.YZ: c1Name = "Y"; c2Name = "Z"; break;
                    default: return false;
                }

                if (!Dragging)
                    BbeginDragging(c1, c2, c1Name, c2Name);

                DragObject(c1, c2, c1Name, c2Name);

                return true;
            }

            private void DrawHighlightedFace()
            {
                if (Face != "")
                {
                    GLContex.glDisable(GLContex.GL_DEPTH_TEST);
                    double[] x = new double[4], y = new double[4], z = new double[4];
                    var ix = ModelSettings.FacePropertiesX[Face];
                    var iy = ModelSettings.FacePropertiesY[Face];
                    var iz = ModelSettings.FacePropertiesZ[Face];

                    x[0] = (double)ix[0].GetValue(HighlightedObject); y[0] = (double)iy[0].GetValue(HighlightedObject); z[0] = (double)iz[0].GetValue(HighlightedObject);
                    x[1] = (double)ix[1].GetValue(HighlightedObject); y[1] = (double)iy[1].GetValue(HighlightedObject); z[1] = (double)iz[1].GetValue(HighlightedObject);
                    x[2] = (double)ix[2].GetValue(HighlightedObject); y[2] = (double)iy[2].GetValue(HighlightedObject); z[2] = (double)iz[2].GetValue(HighlightedObject);
                    x[3] = (double)ix[3].GetValue(HighlightedObject); y[3] = (double)iy[3].GetValue(HighlightedObject); z[3] = (double)iz[3].GetValue(HighlightedObject);

                    GLContex.glColor3ub((byte)255, (byte)0, (byte)0);
                    GLContex.glLineWidth(5);
                    GLContex.glBegin(GLContex.GL_LINE_STRIP);

                    GLContex.glVertex3d(x[0], y[0], z[0]);
                    GLContex.glVertex3d(x[1], y[1], z[1]);
                    GLContex.glVertex3d(x[2], y[2], z[2]);
                    GLContex.glVertex3d(x[3], y[3], z[3]);

                    GLContex.glEnd();

                    GLContex.glEnable(GLContex.GL_DEPTH_TEST);
                }
            }
            private void DrawHighlightedVertex()
            {
                var o = HighlightedObject;
                if (o == null)
                    o = SelectedObjects.Count == 1 && SelectedObjects[0].IsHex ? SelectedObjects[0] : null;

                if (o == null)
                    return;

                if (HexVertex != -1)
                {
                    GLContex.glDisable(GLContex.GL_DEPTH_TEST);
                    GLContex.glEnable(GLContex.GL_POINT_SMOOTH);
                    double[] v = new double[3];
                    o.GetHexVertex(HexVertex, v);

                    GLContex.glPointSize(11);
                    GLContex.glColor3ub((byte)0, (byte)0, (byte)0);
                    GLContex.glBegin(GLContex.GL_POINTS);
                    GLContex.glVertex3d(v[0], v[1], v[2]);
                    GLContex.glEnd();

                    GLContex.glPointSize(9);
                    GLContex.glColor3ub((byte)255, (byte)0, (byte)0);
                    GLContex.glBegin(GLContex.GL_POINTS);
                    GLContex.glVertex3d(v[0], v[1], v[2]);
                    GLContex.glEnd();

                    GLContex.glDisable(GLContex.GL_POINT_SMOOTH);
                    GLContex.glEnable(GLContex.GL_DEPTH_TEST);
                }
            }
            private void DrawSelectedVerticies()
            {
                if (SelectedHexVertices.Count > 0 && SelectedObjects.Count == 1 && SelectedObjects[0].IsHex)
                {
                    var o = SelectedObjects[0];
                    GLContex.glDisable(GLContex.GL_DEPTH_TEST);
                    GLContex.glEnable(GLContex.GL_POINT_SMOOTH);
                    double[] v = new double[3];

                    GLContex.glPointSize(7);
                    GLContex.glColor3ub((byte)0, (byte)128, (byte)0);
                    GLContex.glBegin(GLContex.GL_POINTS);
                    foreach (var vertex in SelectedHexVertices)
                    {
                        o.GetHexVertex(vertex, v);
                        GLContex.glVertex3d(v[0], v[1], v[2]);
                    }
                    GLContex.glEnd();

                    GLContex.glDisable(GLContex.GL_POINT_SMOOTH);
                    GLContex.glEnable(GLContex.GL_DEPTH_TEST);
                }
            }
            private void DrawHighlighteInfo()
            {
                if (HighlightedObject.IsHex)
                    DrawHighlightedVertex();
                else
                    DrawHighlightedFace();
            }
            public void Draw(EPlaneType planeType, bool DrawObjectsBounds)
            {
                foreach (var o in SelectedObjects)
                {
                    if (o != HighlightedObject)
                        o.Draw(planeType, DrawObjectsBounds, Color.Subtract(Colors.White, o.DrawColor));
                }


                if (HighlightedObject != null)
                {
                    HighlightedObject.Draw(planeType, DrawObjectsBounds, SelectedObjects.Contains(HighlightedObject) ? HighlightedObject.DrawColor : Color.Subtract(Colors.White, HighlightedObject.DrawColor));

                    DrawSelectedVerticies();
                    DrawHighlighteInfo();
                }
                else
                {
                    DrawSelectedVerticies();
                    DrawHighlightedVertex();
                }
            }

            private void BbeginDragging(double c1, double c2, String c1Name, String c2Name)
            {
                draggingC10 = c1;
                draggingC20 = c2;


                Dragging = true;
            }
            private void DragObject(double c1, double c2, String c1Name, String c2Name)
            {
                bool dragFace = Face != "" && HighlightedObject != null;
                bool dragVertex = HexVertex != -1 && (HighlightedObject != null || SelectedObjects.Count == 1 && SelectedObjects[0].IsHex);
                bool dragWholeObject = !dragFace && !dragVertex;

                if (dragWholeObject)
                {
                    double dc1, dc2;
                    dc1 = c1 - draggingC10; Utilities.LittleTools.RoundNumber(ref dc1, GlobalDrawingSettings.Discrete(c1Name));
                    dc2 = c2 - draggingC20; Utilities.LittleTools.RoundNumber(ref dc2, GlobalDrawingSettings.Discrete(c2Name));

                    if (dc1 != 0)
                        HighlightedObject.Move(c1Name, dc1);
                    if (dc2 != 0)
                        HighlightedObject.Move(c2Name, dc2);

                    foreach (var o in SelectedObjects)
                        if (o != HighlightedObject)
                        {
                            if (dc1 != 0) o.Move(c1Name, dc1);
                            if (dc2 != 0) o.Move(c2Name, dc2);
                        }

                    draggingC10 += dc1;
                    draggingC20 += dc2;
                }
                if (dragFace)
                {
                    var propFace = typeof(CGeoObject).GetProperty(Face);
                    double d;

                    if (Face.Contains(c1Name)) { Utilities.LittleTools.RoundNumber(ref c1, GlobalDrawingSettings.Discrete(c1Name)); d = c1; }
                    else if (Face.Contains(c2Name)) { Utilities.LittleTools.RoundNumber(ref c2, GlobalDrawingSettings.Discrete(c2Name)); d = c2; }
                    else { HighlightedObject = null; Dragging = false; return; }

                    if ((double)propFace.GetValue(HighlightedObject) - d != 0)
                        propFace.SetValue(HighlightedObject, d);

                    for (int i = 0; i < SelectedObjects.Count; i++)
                        if ((double)propFace.GetValue(SelectedObjects[i]) - d != 0)
                            propFace.SetValue(SelectedObjects[i], d);
                }
                if (dragVertex)
                {
                    double dc1, dc2;
                    double[] v = new double[2];
                    bool topMoved = false;
                    bool botMoved = false;
                    var o = HighlightedObject == null ? SelectedObjects[0] : HighlightedObject;

                    dc1 = c1 - draggingC10; Utilities.LittleTools.RoundNumber(ref dc1, GlobalDrawingSettings.Discrete(c1Name));
                    dc2 = c2 - draggingC20; Utilities.LittleTools.RoundNumber(ref dc2, GlobalDrawingSettings.Discrete(c2Name));

                    if (dc1 != 0) { o.Move(HexVertex, c1Name, dc1); topMoved = c1Name == "Z" && HexVertex > 3; botMoved = c1Name == "Z" && HexVertex < 4; }
                    if (dc2 != 0) { o.Move(HexVertex, c2Name, dc2); topMoved = c2Name == "Z" && HexVertex > 3; botMoved = c2Name == "Z" && HexVertex < 4; }

                    foreach (var vertex in SelectedHexVertices)
                    {
                        if (vertex == HexVertex)
                            continue;

                        if (dc1 != 0 && (c1Name != "Z" || (!topMoved || HexVertex < 4) && (!botMoved || HexVertex > 3))) { o.Move(vertex, c1Name, dc1); topMoved = topMoved || c1Name == "Z" && HexVertex > 3; botMoved = botMoved || c1Name == "Z" && HexVertex < 4; }
                        if (dc2 != 0 && (c2Name != "Z" || (!topMoved || HexVertex < 4) && (!botMoved || HexVertex > 3))) { o.Move(vertex, c2Name, dc2); topMoved = topMoved || c2Name == "Z" && HexVertex > 3; botMoved = botMoved || c2Name == "Z" && HexVertex < 4; }
                    }

                    draggingC10 += dc1;
                    draggingC20 += dc2;
                }
            }
            private void DataGridObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                SelectedObjects.Clear();
                foreach (var item in DataGridObjects.SelectedItems)
                    SelectedObjects.Add(item as CGeoObject);
            }
        }

        HighlightedObjectInfo HighlightedInfo { get; set; } = new HighlightedObjectInfo();
        static Dictionary<Tuple<double, double>, String> reliefFixingPointsLabels = new Dictionary<Tuple<double, double>, String>();
        static HashSet<Tuple<double, double>> reliefFixingPoints = new HashSet<Tuple<double, double>>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        double x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;
        double horizon = 0.0;
        ObservableCollection<CGeoLayer> layers = new ObservableCollection<CGeoLayer>();
        ObservableCollection<CGeoObject> objects = new ObservableCollection<CGeoObject>();
        private ObservableCollection<SaturationVolumeStack> stacks = new ObservableCollection<SaturationVolumeStack>();
        PolarizationDecayCurvesCollection polarizationDecayCurvesCollection = new PolarizationDecayCurvesCollection();
        bool visibleLayers = true;
        bool drawLayersBounds = true;
        bool drawObjectsBounds = true;
        double[] globalBoundingBox = new double[6] { -4000, 4000, -4000, 4000, -4000, 4000 };
        public List<int> AlphaGroupNumbers { set; get; } = new List<int>();
        public List<int> AlphaGroupPriority { set; get; } = new List<int>();

        public ObservableCollection<CGeoLayer> Layers
        {
            get { return layers; }
            set { layers = value; OnPropertyChanged("Layers"); }
        }
        public ObservableCollection<CGeoObject> Objects
        {
            get { return objects; }
            set { objects = value; OnPropertyChanged("Objects"); }
        }
        public ObservableCollection<SaturationVolumeStack> Stacks
        {
            get { return stacks; }
            set { stacks = value; OnPropertyChanged("Stacks"); }
        }
        public PolarizationDecayCurvesCollection PolarizationDecayCurvesCollection
        {
            get { return polarizationDecayCurvesCollection; }
            set { polarizationDecayCurvesCollection = value; OnPropertyChanged("PolarizationDecayCurvesCollection"); }
        }
        public CGeoObject SelectedObject
        {
            get { return HighlightedInfo.HighlightedObject; }
            set { HighlightedInfo.HighlightedObject = value; OnPropertyChanged("SelectedObject"); }
        }
        public DataGrid DataGridObjects
        {
            get { return HighlightedInfo.DataGridObjects; }
            set { HighlightedInfo.DataGridObjects = value; }
        }
        public double[] GlobalBoundingBox
        {
            get { return globalBoundingBox; }
            set { globalBoundingBox = value; OnPropertyChanged("GlobalBoundingBox"); }
        }
        public double X0
        {
            get { return x0; }
            set { x0 = value; }
        }
        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        public double Y0
        {
            get { return y0; }
            set { y0 = value; }
        }
        public double Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        public double Z0
        {
            get { return z0; }
            set { z0 = value; }
        }
        public double Z1
        {
            get { return z1; }
            set { z1 = value; }
        }
        public double Horizon
        {
            get { return horizon; }
            set { horizon = value; OnPropertyChanged("Horizon"); }
        }
        public bool VisibleLayers
        {
            get { return visibleLayers; }
            set { visibleLayers = value; OnPropertyChanged("VisibleLayers"); }
        }
        public bool DrawLayersBounds
        {
            get { return drawLayersBounds; }
            set { drawLayersBounds = value; OnPropertyChanged("DrawLayersBounds"); }
        }
        public bool DrawObjectsBounds
        {
            get { return drawObjectsBounds; }
            set { drawObjectsBounds = value; OnPropertyChanged("DrawObjectsBounds"); }
        }
        public Model()
        {
            objects.CollectionChanged += this.OnCollectionChanged;
            layers.CollectionChanged += this.OnLayersCollectionChanged;
        }
        public Model(Model geoModel)
        {

            X0 = geoModel.X0;
            X1 = geoModel.X1;
            Y0 = geoModel.Y0;
            Y1 = geoModel.Y1;
            Z0 = geoModel.Z0;
            Z1 = geoModel.Z1;
            Horizon = geoModel.Horizon;
            Layers = new ObservableCollection<CGeoLayer>();
            foreach (var item in geoModel.Layers)
                Layers.Add(new CGeoLayer(item));

            Objects = new ObservableCollection<CGeoObject>();
            foreach (var item in geoModel.Objects)
                Objects.Add(new CGeoObject(item));
            VisibleLayers = geoModel.VisibleLayers;
            DrawLayersBounds = geoModel.DrawLayersBounds;
            DrawObjectsBounds = geoModel.DrawObjectsBounds;
            GlobalBoundingBox[0] = geoModel.GlobalBoundingBox[0];
            GlobalBoundingBox[1] = geoModel.GlobalBoundingBox[1];
            GlobalBoundingBox[2] = geoModel.GlobalBoundingBox[2];
            GlobalBoundingBox[3] = geoModel.GlobalBoundingBox[3];
            GlobalBoundingBox[4] = geoModel.GlobalBoundingBox[4];
            GlobalBoundingBox[5] = geoModel.GlobalBoundingBox[5];
        }
        public bool RecalculateBoundingBox(int numObject = -1, bool changeBoundingBox = false)
        {
            double minx = Double.MaxValue,
                miny = Double.MaxValue,
                minz = Double.MaxValue,
                maxx = Double.MinValue,
                maxy = Double.MinValue,
                maxz = Double.MinValue;
            if (numObject == -1)
                foreach (var p in Objects)
                {
                    if (p.parallel[0].Min < minx)
                        minx = p.parallel[0].Min;

                    if (p.parallel[0].Max > maxx)
                        maxx = p.parallel[0].Max;

                    if (p.parallel[1].Min < miny)
                        miny = p.parallel[1].Min;

                    if (p.parallel[1].Max > maxy)
                        maxy = p.parallel[1].Max;

                    if (p.parallel[2].Min < minz)
                        minz = p.parallel[2].Min;

                    if (p.parallel[2].Max > maxz)
                        maxz = p.parallel[2].Max;

                }
            else
            {
                var p = Objects[numObject];
                if (p.parallel[0].Min < GlobalBoundingBox[0])
                    GlobalBoundingBox[0] = p.parallel[0].Min;

                if (p.parallel[0].Max > GlobalBoundingBox[1])
                    GlobalBoundingBox[1] = p.parallel[0].Max;

                if (p.parallel[1].Min < GlobalBoundingBox[2])
                    GlobalBoundingBox[2] = p.parallel[1].Min;

                if (p.parallel[1].Max > GlobalBoundingBox[3])
                    GlobalBoundingBox[3] = p.parallel[1].Max;

                if (p.parallel[2].Min < GlobalBoundingBox[4])
                    GlobalBoundingBox[4] = p.parallel[2].Min;

                if (p.parallel[2].Max > GlobalBoundingBox[5])
                    GlobalBoundingBox[5] = p.parallel[2].Max;
                return true;
            }

            if (changeBoundingBox == true || (minx < GlobalBoundingBox[0] || maxx > GlobalBoundingBox[1] || miny < GlobalBoundingBox[2] || maxy > GlobalBoundingBox[3] || minz < GlobalBoundingBox[4] || maxz > GlobalBoundingBox[5]))
            {
                GlobalBoundingBox[0] = minx;
                GlobalBoundingBox[2] = miny;
                GlobalBoundingBox[4] = minz;
                GlobalBoundingBox[1] = maxx;
                GlobalBoundingBox[3] = maxy;
                GlobalBoundingBox[5] = maxz;
                return true;
            }
            return false;
        }
        public void RecalculateLayersZ()
        {
            double z = Horizon;

            foreach (var layer in Layers)
            {
                //if (layer.FitMesh.bindToLayerZ)
                //    layer.FitMesh.RegZ = z;
                layer.Z = z;
                z -= layer.H;
            }
        }
        public double GetZBottom()
        {
            double z = Horizon;

            for (int i = 0; i < layers.Count - 1; i++)
                z -= layers[i].H;

            return z;
        }
        public double GetZTop()
        {
            return Horizon;
        }

        //======================================================== alpha groups
        public void BuildUniqueAlphaGroups()
        {
            int n = 1;
            foreach (var obj in Objects)
            {
                obj.GroupNumber = n;
                obj.GroupPriority = 1;
                n++;
            }
        }
        public void SetAlphaPriority()
        {
            SortedSet<int> numbers = new SortedSet<int>();
            foreach (var obj in objects)
                numbers.Add(obj.GroupNumber);

            int m;
            foreach (var num in numbers)
            {
                var polObjects = objects.Where(x => x.GroupNumber == num).ToList().OrderBy(x => x.Material.Alpha);

                m = 1;
                foreach (var obj in polObjects)
                {
                    obj.GroupPriority = m;
                    m++;
                }
            }
        }


        //======================================================== mouse reaction
        CGeoObject FindObject(double[] r1, double[] r2, String c3PropertyName, bool min = false)
        {
            try
            {
                SortedDictionary<double, List<CGeoObject>> objectsFound = new SortedDictionary<double, List<CGeoObject>>();
                var prop = typeof(Utilities.Vector3).GetProperty(c3PropertyName);
                Utilities.Vector3 ip;
                foreach (var o in Objects)
                    if (o.Visible)
                        if (o.LineIntersectsObject(r1, r2, out ip))
                        {
                            if (!objectsFound.ContainsKey((double)prop.GetValue(ip)))
                                objectsFound.Add((double)prop.GetValue(ip), new List<CGeoObject>());
                            objectsFound[(double)prop.GetValue(ip)].Add(o);
                        }

                if (objectsFound.Count == 0)
                    return null;

                return min ? objectsFound.First().Value[0] : objectsFound.Last().Value[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        void GetObjectControlHighlighted(double c1, double c2, double mPerPixel, String c1PropertyName, String c2PropertyName)
        {
            var o = HighlightedInfo.HighlightedObject;
            if (o == null)
            {
                GetObjectVertexHighlighted(c1, c2, mPerPixel, c1PropertyName, c2PropertyName);
                return;
            }

            if (o.IsHex)
                GetObjectVertexHighlighted(c1, c2, mPerPixel, c1PropertyName, c2PropertyName);
            else
                GetObjectBoundHighlighted(c1, c2, mPerPixel, c1PropertyName, c2PropertyName);
        }
        void GetObjectBoundHighlighted(double c1, double c2, double mPerPixel, String c1PropertyName, String c2PropertyName)
        {
            var o = HighlightedInfo.HighlightedObject;
            if (o == null)
                return;

            int precision = 5;
            var c10Prop = (typeof(CGeoObject)).GetProperty(c1PropertyName + "0");
            var c11Prop = (typeof(CGeoObject)).GetProperty(c1PropertyName + "1");
            var c20Prop = (typeof(CGeoObject)).GetProperty(c2PropertyName + "0");
            var c21Prop = (typeof(CGeoObject)).GetProperty(c2PropertyName + "1");

            int closestC1;
            int closestC2;
            double closestDC1;
            double closestDC2;

            closestC1 = (Math.Abs(c1 - (double)c10Prop.GetValue(o))) < Math.Abs(((double)c11Prop.GetValue(o) - c1)) ? 0 : 1; closestDC1 = Math.Min(Math.Abs(c1 - (double)c10Prop.GetValue(o)), Math.Abs((double)c11Prop.GetValue(o) - c1));
            closestC2 = (Math.Abs(c2 - (double)c20Prop.GetValue(o))) < Math.Abs(((double)c21Prop.GetValue(o) - c2)) ? 0 : 1; closestDC2 = Math.Min(Math.Abs(c2 - (double)c20Prop.GetValue(o)), Math.Abs((double)c21Prop.GetValue(o) - c2));

            HighlightedInfo.Face = "";
            if (closestDC1 < closestDC2)
            {
                if (closestDC1 < mPerPixel * precision)
                    HighlightedInfo.Face = c1PropertyName + closestC1;
            }
            else
            {
                if (closestDC2 < mPerPixel * precision)
                    HighlightedInfo.Face = c2PropertyName + closestC2;
            }
        }
        void GetObjectVertexHighlighted(double c1, double c2, double mPerPixel, String c1PropertyName, String c2PropertyName)
        {
            HighlightedInfo.HexVertex = -1;
            var o = HighlightedInfo.HighlightedObject;
            if (o == null)
                o = HighlightedInfo.SelectedObjects.Count == 1 && HighlightedInfo.SelectedObjects[0].IsHex ? HighlightedInfo.SelectedObjects[0] : null;

            if (o == null)
                return;

            int precision = 5;
            int closest = -1;
            double dMin = double.MaxValue;
            double d;
            double[] v = new double[2];

            for (int i = 0; i < 8; i++)
            {
                o.GetHexVertex(i, c1PropertyName, c2PropertyName, v);
                d = Utilities.LittleTools.CalculateDistance2D(v[0], v[1], c1, c2);
                if (dMin > d)
                {
                    closest = i;
                    dMin = d;
                }
            }

            o.GetHexVertex(closest, c1PropertyName, c2PropertyName, v);
            if (Math.Abs(v[0] - c1) < precision * mPerPixel)
                if (Math.Abs(v[1] - c2) < precision * mPerPixel)
                {
                    HighlightedInfo.HexVertex = closest;
                }
        }
        bool HighlightObject(double[] r1, double[] r2, double mPerPixel, EPlaneType planeType) // -1 - no reaction, no redraw, 0 - reaction, no redraw, 1 - reaction, redraw
        {
            try
            {
                double c1 = 0, c2 = 0;
                String p1 = null, p2 = null, p3 = null;

                HighlightedInfo.Face = "";
                switch (planeType)
                {
                    case EPlaneType.XY: c1 = r1[0]; c2 = r1[1]; p1 = "X"; p2 = "Y"; p3 = "Z"; break;
                    case EPlaneType.XZ: c1 = r1[0]; c2 = r1[2]; p1 = "X"; p2 = "Z"; p3 = "Y"; break;
                    case EPlaneType.YZ: c1 = r1[1]; c2 = r1[2]; p1 = "Y"; p2 = "Z"; p3 = "X"; break;
                }

                HighlightedInfo.HighlightedObject = FindObject(r1, r2, p3);
                GetObjectControlHighlighted(c1, c2, mPerPixel, p1, p2);

                if (HighlightedInfo.SelectedObjects.Count != 1 || !HighlightedInfo.SelectedObjects[0].IsHex)
                    HighlightedInfo.SelectedHexVertices.Clear();

                if (HighlightedInfo.HighlightedObject == null)
                {
                    HighlightedInfo.Face = "";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Draw(EPlaneType planeType, double[] boundingBox, int widthLocal, int heightLocal, FontGeology labelFont, FontGeology paletteFont)
        {
            double prevHeight = Horizon;
            foreach (var p in layers)
            {
                p.Draw(planeType, prevHeight, 1e+7, boundingBox, DrawLayersBounds);
                prevHeight -= p.H;
            }

            foreach (var p in objects.Except(HighlightedInfo.SelectedObjects))
                if (p != HighlightedInfo.HighlightedObject)
                    p.Draw(planeType, DrawObjectsBounds, p.DrawColor);

            HighlightedInfo.Draw(planeType, DrawObjectsBounds);


            GLContex.glColor3ub((byte)180, 0, 0);
            GLContex.glBegin(GLContex.GL_POINTS);
            GLContex.glPointSize(2);
            int step = reliefFixingPoints.Count / 100000 + 1;

            foreach (var point in reliefFixingPoints)
                GLContex.glVertex3d(point.Item1, point.Item2, 10000);
            GLContex.glEnd();

            foreach (var point in reliefFixingPoints)
            {
                labelFont.PrintText(point.Item1, point.Item2, 10000, reliefFixingPointsLabels[point]);
            }
            return;


            //additionElementsCollection.Draw(planeType);
        }
        public bool MouseMove(double x, double y, double[] r1, double[] r2, double mPerPixel, bool ctrlPressed, bool shiftPressed, bool LMBPressed, bool RMBPressed, EPlaneType planeType)
        {
            try
            {
                if (!ctrlPressed || shiftPressed)
                {
                    HighlightedInfo.HighlightedObject = null;
                    return false;
                }

                if (LMBPressed)
                    return HighlightedInfo.Drag(x, y, planeType);
                else
                    HighlightedInfo.Dragging = false;


                return HighlightObject(r1, r2, mPerPixel, planeType);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void StartSelection(double x0, double y0, bool toAdd, EPlaneType planeType)
        {
        }
        public void ContinueSelection(double x0, double y0, double x1, double y1, bool toAdd, EPlaneType planeType)
        {
        }
        public void FinishSelection(double x0, double y0, double x1, double y1, bool toAdd, EPlaneType planeType)
        {
            try
            {
                List<CGeoObject> s = null;
                String p1 = null, p2 = null;
                switch (planeType)
                {
                    case EPlaneType.XY: p1 = "X"; p2 = "Y"; break;
                    case EPlaneType.XZ: p1 = "X"; p2 = "Z"; break;
                    case EPlaneType.YZ: p1 = "Y"; p2 = "Z"; break;
                }

                if (SelectVertices(p1, p2, x0, y0, x1, y1, toAdd))
                    return;

                HighlightedInfo.SelectedHexVertices.Clear();

                s = SelectObjects(p1 + "0", p2 + "0", p1 + "1", p2 + "1", x0, y0, x1, y1);

                if (s == null)
                    return;

                if (!toAdd)
                    HighlightedInfo.DataGridObjects.SelectedItems.Clear();

                foreach (var o in s)
                    HighlightedInfo.DataGridObjects.SelectedItems.Add(o);
                HighlightedInfo.DataGridObjects.ScrollIntoView(s.Last());
            }
            catch (Exception ex)
            {

            }
        }
        public void Click(double x, double y, double[] ray1, double[] ray2, double mPerPixel, EPlaneType planeType, bool ctrlPressed, bool shiftPressed)
        {
            try
            {
                HighlightObject(ray1, ray2, mPerPixel, planeType);
                var o = HighlightedInfo.HighlightedObject;
                if (!ctrlPressed)
                {
                    HighlightedInfo.DataGridObjects.SelectedItems.Clear();
                    if (o == null)
                        return;
                    else
                    {
                        HighlightedInfo.DataGridObjects.SelectedItems.Add(o);
                        HighlightedInfo.DataGridObjects.ScrollIntoView(o);
                        return;
                    }
                }

                if (HighlightedInfo.SelectedObjects.Contains(o))
                    HighlightedInfo.DataGridObjects.SelectedItems.Remove(o);
                else
                {
                    HighlightedInfo.DataGridObjects.SelectedItems.Add(o);
                    HighlightedInfo.DataGridObjects.ScrollIntoView(o);
                }
            }
            catch (Exception ex)
            {

            }
        }

        static private bool ToAddIntersectionPoint(double x, double y, double dz, double radius, List<Tuple<double, double, double>> displacements)
        {
            foreach (var displacement in displacements)
            {
                if (Math.Abs(displacement.Item1 - x) < radius)
                    if (Math.Abs(displacement.Item2 - y) < radius)
                    {
                        if (Math.Abs(displacement.Item3) < Math.Abs(dz))
                            displacements[displacements.IndexOf(displacement)] = new Tuple<double, double, double>(x, y, dz);
                        return false;
                    }
            }

            return true;
        }
        private double GetLayerZTop(int layerNumber)
        {
            double z = Horizon;

            for (int layerIndex = 0; layerIndex < layerNumber - 1; layerIndex++)
                z -= Layers[layerIndex].H;

            return z;
        }

        private List<CGeoObject> SelectObjects(String px0, String py0, String px1, String py1, double x0, double y0, double x1, double y1)
        {
            try
            {
                double[] v = new double[2];
                String px = px0.Substring(0, 1);
                String py = py0.Substring(0, 1);
                bool cont = false;
                List<CGeoObject> res = new List<CGeoObject>();
                var propX0 = (typeof(CGeoObject)).GetProperty(px0);
                var propX1 = (typeof(CGeoObject)).GetProperty(px1);
                var propY0 = (typeof(CGeoObject)).GetProperty(py0);
                var propY1 = (typeof(CGeoObject)).GetProperty(py1);

                foreach (var o in Objects)
                    if (o.Visible)
                        if (!o.IsHex)
                        {
                            if ((double)propX0.GetValue(o) >= x0 && (double)propX1.GetValue(o) <= x1)
                                if ((double)propY0.GetValue(o) >= y0 && (double)propY1.GetValue(o) <= y1)
                                    res.Add(o);
                        }
                        else
                        {
                            cont = false;
                            for (int i = 0; i < 8; i++)
                            {
                                o.GetHexVertex(i, px, py, v);
                                if (v[0] < x0 || v[0] > x1) { cont = true; break; }
                                if (v[1] < y0 || v[1] > y1) { cont = true; break; }
                            }
                            if (cont)
                                continue;
                            res.Add(o);
                        }

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private bool SelectVertices(String px, String py, double x0, double y0, double x1, double y1, bool toAdd)
        {
            try
            {
                if (HighlightedInfo.SelectedObjects.Count != 1 || !HighlightedInfo.SelectedObjects[0].IsHex)
                {
                    HighlightedInfo.SelectedHexVertices.Clear();
                    return false;
                }

                if (!toAdd)
                    HighlightedInfo.SelectedHexVertices.Clear();

                var o = HighlightedInfo.SelectedObjects[0];

                double[] v = new double[2];

                for (int i = 0; i < 8; i++)
                {
                    o.GetHexVertex(i, px, py, v);

                    if (x0 > v[0] || v[0] > x1)
                        continue;
                    if (y0 > v[1] || v[1] > y1)
                        continue;

                    HighlightedInfo.SelectedHexVertices.Add(i);
                }

                return HighlightedInfo.SelectedHexVertices.Count != 0;
            }
            catch (Exception ex)
            {
                HighlightedInfo.SelectedHexVertices.Clear();
                return false;
            }
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }
        private void OnLayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecalculateLayersZ();
            OnCollectionChanged(sender, e);
        }

        internal void GetBoundingBox(double[] boundingBox)
        {
            boundingBox[0] = boundingBox[2] = boundingBox[4] = -1;
            boundingBox[1] = boundingBox[3] = boundingBox[5] = 1;

            foreach (var obj in Objects)
            {
                if (boundingBox[0] > obj.X0) boundingBox[0] = obj.X0;
                if (boundingBox[1] < obj.X1) boundingBox[1] = obj.X1;
                if (boundingBox[2] > obj.Y0) boundingBox[2] = obj.Y0;
                if (boundingBox[3] < obj.Y1) boundingBox[3] = obj.Y1;
                if (boundingBox[4] > obj.Z0) boundingBox[4] = obj.Z0;
                if (boundingBox[5] < obj.Z1) boundingBox[5] = obj.Z1;
            }
        }
    }
}
