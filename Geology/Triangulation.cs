using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
namespace Geology
{
    public class CNodes
    {
        public double [] coords;
        public int marker = 0;
        public bool process;
        public bool inTriangle = false;
        public int countTriangles = 0;
        public CNodes()
        {
            process = false;
            process = false;
            coords = null;
        }
        public CNodes(double _x,double _y,double _z)
        {
            process = false;
            coords = new double[] { _x ,_y,_z};
        }
    }
    public class CNodeTree
    {
       public  CTriangle triangle;
       public CNodeTree[] childNode;
       public bool InTriangle(CNodes node,CNodes [] arrayNodes)
       {
           if (triangle.pointInside(node, arrayNodes) != -1)
           {
               return true;
           }
           return false;
       }
       public CNodeTree(CNodeTree cNode, CNodes[] arrayNodes)
       {
           triangle = new CTriangle(triangle.node[0], triangle.node[1], triangle.node[2], arrayNodes);

       }
       public CNodeTree(CTriangle _triangle)
       {
           triangle = _triangle;
           childNode = null;
       }
    }
    public class CMyTree
    {
        public CNodeTree topNode;

        void buildOppositeTriangle(int type2, bool rev, CNodeTree ChangeNodeAl, int numNode, CNodeTree ChangeNode,CNodes[] arrayNodes)
        {
            var p2 = ChangeNodeAl.triangle;
            switch (type2)
            {

                case 1:
                    {

                        ChangeNodeAl.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p2.node[2], numNode, p2.node[0],arrayNodes)), 
                                                                                    new CNodeTree(new CTriangle(p2.node[2],numNode, p2.node[1],arrayNodes)) };

                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[2] = p2.aligmTriangl[2];
                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[1];

                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[2] = p2.aligmTriangl[1];
                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[0];

                        if (p2.aligmTriangl[2] != null)
                        {
                            int t2 = p2.aligmTriangl[2].triangle.GetEdge(p2.node[0], p2.node[2]);
                            p2.aligmTriangl[2].triangle.aligmTriangl[t2 - 1] = ChangeNodeAl.childNode[0];
                        }
                        if (p2.aligmTriangl[1] != null)
                        {
                            int t1 = p2.aligmTriangl[1].triangle.GetEdge(p2.node[1], p2.node[2]);
                            p2.aligmTriangl[1].triangle.aligmTriangl[t1 - 1] = ChangeNodeAl.childNode[1];
                        }

                        if (rev)
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                        }
                        else
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                        }
                    }
                    break;
                case 2:
                    {

                        ChangeNodeAl.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p2.node[0], numNode, p2.node[1],arrayNodes)), 
                                                                                      new CNodeTree(new CTriangle(p2.node[0],numNode, p2.node[2],arrayNodes)) };


                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[2] = p2.aligmTriangl[0];
                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[1];

                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[2] = p2.aligmTriangl[2];
                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[0];
                        if (p2.aligmTriangl[0] != null)
                        {
                            int t0 = p2.aligmTriangl[0].triangle.GetEdge(p2.node[0], p2.node[1]);
                            p2.aligmTriangl[0].triangle.aligmTriangl[t0 - 1] = ChangeNodeAl.childNode[0];
                        }
                        if (p2.aligmTriangl[2] != null)
                        {
                            int t2 = p2.aligmTriangl[2].triangle.GetEdge(p2.node[0], p2.node[2]);
                            p2.aligmTriangl[2].triangle.aligmTriangl[t2 - 1] = ChangeNodeAl.childNode[1];
                        }


                        if (rev)
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                        }
                        else
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                        }



                    }
                    break;
                case 3:
                    {
                        ChangeNodeAl.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p2.node[1], numNode, p2.node[0],arrayNodes)), 
                                                                                      new CNodeTree(new CTriangle(p2.node[1],numNode, p2.node[2],arrayNodes)) };


                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[2] = p2.aligmTriangl[0];
                        ChangeNodeAl.childNode[0].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[1];

                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[2] = p2.aligmTriangl[1];
                        ChangeNodeAl.childNode[1].triangle.aligmTriangl[0] = ChangeNodeAl.childNode[0];
                        if (p2.aligmTriangl[0] != null)
                        {
                            int t0 = p2.aligmTriangl[0].triangle.GetEdge(p2.node[0], p2.node[1]);
                            p2.aligmTriangl[0].triangle.aligmTriangl[t0 - 1] = ChangeNodeAl.childNode[0];
                        }
                        if (p2.aligmTriangl[1] != null)
                        {
                            int t1 = p2.aligmTriangl[1].triangle.GetEdge(p2.node[1], p2.node[2]);
                            p2.aligmTriangl[1].triangle.aligmTriangl[t1 - 1] = ChangeNodeAl.childNode[1];
                        }

                        if (rev)
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                        }
                        else
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[0];
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = ChangeNodeAl.childNode[1];

                            ChangeNodeAl.childNode[0].triangle.aligmTriangl[1] = ChangeNode.childNode[0];
                            ChangeNodeAl.childNode[1].triangle.aligmTriangl[1] = ChangeNode.childNode[1];
                        }
                    }
                    break;
            }
        }
        public void searchLastTriangle(CNodes [] arrayNodes,int numNode)
        {
            CNodes node = arrayNodes[numNode];
            CNodeTree TmpNodeLeft = topNode.childNode[0];
            CNodeTree TmpNodeRight = topNode.childNode[1];
            CNodeTree ChangeNode  = topNode;



            int numChild = 0;
            int type = 0;
            while (ChangeNode.childNode != null)
            {
                numChild = 0;
                type = 0;
                foreach (var tr in ChangeNode.childNode)
                {
                    type = tr.triangle.pointInside(node, arrayNodes);
                    if (type!=-1)
                    {
                        break;
                    }
                    numChild++;
                }
                ChangeNode = ChangeNode.childNode[numChild];
            }

            var p = ChangeNode.triangle;
            
            switch (type)
            {
                case 1:
                    {
                        ChangeNode.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p.node[2], numNode, p.node[0],arrayNodes)), 
                                                                  new CNodeTree(new CTriangle(p.node[2],numNode, p.node[1],arrayNodes)) };

                        ChangeNode.childNode[0].triangle.aligmTriangl[2] = p.aligmTriangl[2];
                        ChangeNode.childNode[0].triangle.aligmTriangl[0] = ChangeNode.childNode[1];

                        ChangeNode.childNode[1].triangle.aligmTriangl[2] = p.aligmTriangl[1];
                        ChangeNode.childNode[1].triangle.aligmTriangl[0] = ChangeNode.childNode[0];


                        if (p.aligmTriangl[2] != null)
                        {
                            int t2 = p.aligmTriangl[2].triangle.GetEdge(p.node[0], p.node[2]);
                            p.aligmTriangl[2].triangle.aligmTriangl[t2 - 1] = ChangeNode.childNode[0];
                        }

                        if (p.aligmTriangl[1] != null)
                        {
                            int t1 = p.aligmTriangl[1].triangle.GetEdge(p.node[1], p.node[2]);
                            p.aligmTriangl[1].triangle.aligmTriangl[t1 - 1] = ChangeNode.childNode[1];
                        }
                        CNodeTree ChangeNodeAl = ChangeNode.triangle.aligmTriangl[0];
                        if (ChangeNodeAl == null)
                        { 
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = null;
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = null;
                        }
                        else
                        {
                            bool rev;
                            int type2 = ChangeNodeAl.triangle.GetEdge(p.node[0], p.node[1], out rev);
                            if (ChangeNodeAl != null)
                                if (ChangeNodeAl.childNode == null)
                                {
                                    buildOppositeTriangle(type2, rev, ChangeNodeAl, numNode, ChangeNode, arrayNodes);
                                }
                                else
                                {
                                    MessageBox.Show("Error in build tringulation");
                                }
                        }

                    }
                    break;
                case 2:
                    {
                        ChangeNode.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p.node[0], numNode, p.node[1],arrayNodes)), 
                                                                  new CNodeTree(new CTriangle(p.node[0],numNode, p.node[2],arrayNodes)) };

                        ChangeNode.childNode[0].triangle.aligmTriangl[2] = p.aligmTriangl[0];
                        ChangeNode.childNode[0].triangle.aligmTriangl[0] = ChangeNode.childNode[1];

                        ChangeNode.childNode[1].triangle.aligmTriangl[2] = p.aligmTriangl[2];
                        ChangeNode.childNode[1].triangle.aligmTriangl[0] = ChangeNode.childNode[0];

                          if (p.aligmTriangl[0] != null)
                          {
                              int t0 = p.aligmTriangl[0].triangle.GetEdge(p.node[0], p.node[1]);
                              p.aligmTriangl[0].triangle.aligmTriangl[t0 - 1] = ChangeNode.childNode[0];
                          }
                          if (p.aligmTriangl[2] != null)
                          {
                              int t2 = p.aligmTriangl[2].triangle.GetEdge(p.node[0], p.node[2]);
                              p.aligmTriangl[2].triangle.aligmTriangl[t2 - 1] = ChangeNode.childNode[1];
                          }

                        CNodeTree ChangeNodeAl = ChangeNode.triangle.aligmTriangl[1];
                        if (ChangeNodeAl == null)
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = null;
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = null;
                        }
                        else
                        {
                            bool rev;
                            int type2 = ChangeNodeAl.triangle.GetEdge(p.node[1], p.node[2], out rev);
                            if (ChangeNodeAl != null)
                                if (ChangeNodeAl.childNode == null)
                                {
                                    buildOppositeTriangle(type2, rev, ChangeNodeAl, numNode, ChangeNode, arrayNodes);
                                }
                                else
                                {
                                    MessageBox.Show("Error in build tringulation");
                                }
                        }
                    }
                    break;
                case 3:
                    {
                        ChangeNode.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(p.node[1], numNode, p.node[0],arrayNodes)), 
                                                                  new CNodeTree(new CTriangle(p.node[1],numNode, p.node[2],arrayNodes)) };

                        ChangeNode.childNode[0].triangle.aligmTriangl[2] = p.aligmTriangl[0];
                        ChangeNode.childNode[0].triangle.aligmTriangl[0] = ChangeNode.childNode[1];

                        ChangeNode.childNode[1].triangle.aligmTriangl[2] = p.aligmTriangl[1];
                        ChangeNode.childNode[1].triangle.aligmTriangl[0] = ChangeNode.childNode[0];
                        if (p.aligmTriangl[0] != null)
                        {
                            int t0 = p.aligmTriangl[0].triangle.GetEdge(p.node[0], p.node[1]);
                            p.aligmTriangl[0].triangle.aligmTriangl[t0 - 1] = ChangeNode.childNode[0];
                        }
                        if (p.aligmTriangl[1] != null)
                        {
                            int t1 = p.aligmTriangl[1].triangle.GetEdge(p.node[1], p.node[2]);
                            p.aligmTriangl[1].triangle.aligmTriangl[t1 - 1] = ChangeNode.childNode[1];
                        }

                        CNodeTree ChangeNodeAl = ChangeNode.triangle.aligmTriangl[2];
                        if (ChangeNodeAl == null)
                        {
                            ChangeNode.childNode[0].triangle.aligmTriangl[1] = null;
                            ChangeNode.childNode[1].triangle.aligmTriangl[1] = null;
                        }
                        else
                        {
                            bool rev;
                            int type2 = ChangeNodeAl.triangle.GetEdge(p.node[0], p.node[2], out rev);
                            if (ChangeNodeAl != null)
                                if (ChangeNodeAl.childNode == null)
                                {
                                    buildOppositeTriangle(type2, rev, ChangeNodeAl, numNode, ChangeNode, arrayNodes);
                                }
                                else
                                {
                                    MessageBox.Show("Error in build tringulation");
                                }
                        }
                    }
                    break;
                case 4:
                    {
                        ChangeNode.childNode = new CNodeTree[] { new CNodeTree(new CTriangle(numNode,p.node[0], p.node[1],arrayNodes)), 
                                                                 new CNodeTree(new CTriangle(numNode,p.node[1], p.node[2],arrayNodes)),
                                                                 new CNodeTree(new CTriangle(numNode,p.node[0], p.node[2],arrayNodes))};


                        ChangeNode.childNode[0].triangle.aligmTriangl[0] = ChangeNode.childNode[2];
                        ChangeNode.childNode[0].triangle.aligmTriangl[1] = p.aligmTriangl[0];
                        ChangeNode.childNode[0].triangle.aligmTriangl[2] = ChangeNode.childNode[1];

                        ChangeNode.childNode[1].triangle.aligmTriangl[0] = ChangeNode.childNode[0];
                        ChangeNode.childNode[1].triangle.aligmTriangl[1] = p.aligmTriangl[1];
                        ChangeNode.childNode[1].triangle.aligmTriangl[2] = ChangeNode.childNode[2];

                        ChangeNode.childNode[2].triangle.aligmTriangl[0] = ChangeNode.childNode[0];
                        ChangeNode.childNode[2].triangle.aligmTriangl[1] = p.aligmTriangl[2];
                        ChangeNode.childNode[2].triangle.aligmTriangl[2] = ChangeNode.childNode[1];
                        if (p.aligmTriangl[0] != null)
                        {
                            int t0 = p.aligmTriangl[0].triangle.GetEdge(p.node[0], p.node[1]);
                            p.aligmTriangl[0].triangle.aligmTriangl[t0 - 1] = ChangeNode.childNode[0];
                        }
                        if (p.aligmTriangl[1] != null)
                        {
                            int t1 = p.aligmTriangl[1].triangle.GetEdge(p.node[1], p.node[2]);
                            p.aligmTriangl[1].triangle.aligmTriangl[t1 - 1] = ChangeNode.childNode[1];
                        }
                        if (p.aligmTriangl[2] != null)
                        {
                            int t2 = p.aligmTriangl[2].triangle.GetEdge(p.node[0], p.node[2]);
                            p.aligmTriangl[2].triangle.aligmTriangl[t2 - 1] = ChangeNode.childNode[2];
                        }
                    } break;
            };
            
        }
        public CMyTree()
        {
            topNode = new CNodeTree(new CTriangle(0,0,0,null));
        }
    }
    public class CEdges
    {
        public CNodes node1, node2;
        List<int> triangulation;
        bool Del = false;
        public CEdges()
        {
            node1 = null;
            node2 = null;
            triangulation = new List<int>();
        }
       
        public CEdges(CNodes _node1, CNodes _node2/*, CTriangle _clockWise, CTriangle _counterClockWise*/)
        {
            node1 = _node1;
            node2 = _node2;

          //  clockWise = _clockWise;
           // counterClockWise = _counterClockWise;
        }
    }

    public class CTriangleReconstruct
    {
        public int[] node;
        public int[] aligmTriangl;
        public double[] angle;
        public double maxAngle;
       /* public double[] center;
        public double r2;*/
        public CTriangleReconstruct(int _node1, int _node2, int _node3, CNodes[] arrayNodes)
        {
         
            node = new int[3];
            aligmTriangl = new int[3];
            angle=new double[3];
            node[0] = _node1;
            node[1] = _node2;
            node[2] = _node3;
            formAngle(arrayNodes);
           // createCirctle(arrayNodes);
        }
        double Jacobian(double J11, double J12, double J13,
                      double J21, double J22, double J23,
                      double J31, double J32, double J33)
        {
            return J11 * J22 * J33 + J12 * J23 * J31 + J21 * J32 * J13 - (J31 * J22 * J13 + J12 * J21 * J33 + J23 * J32 * J11);
        }
        public void formAngle(CNodes[] arrayNodes)
        {
             double x1 = arrayNodes[node[0]].coords[0];
            double x2 = arrayNodes[node[1]].coords[0];
            double x3 = arrayNodes[node[2]].coords[0];

            double y1 = arrayNodes[node[0]].coords[1];
            double y2 = arrayNodes[node[1]].coords[1];
            double y3 = arrayNodes[node[2]].coords[1];

            double a0 = (x3-x2)*(x3-x2)+(y3-y2)*(y3-y2);
            double a1 = (x3-x1)*(x3-x1)+(y3-y1)*(y3-y1);
            double a2 = (x2-x1)*(x2-x1)+(y2-y1)*(y2-y1);

            if (a1 < 10E-10 || a2 < 10E-10 || a0 < 10E-10)
            {
                angle[1] = 0;
                angle[0] = 180;
                angle[2] = 180;
            }
            else
            {
                angle[0] = Math.Acos((a1 + a2 - a0) / (2 * Math.Sqrt(a1 * a2)));
                angle[1] = Math.Acos((a0 + a2 - a1) / (2 * Math.Sqrt(a0 * a2)));
                angle[2] = Math.PI - (angle[0] + angle[1]);
            }
             maxAngle = angle[0];
             if (maxAngle < angle[1])
                 maxAngle = angle[1];
             if (maxAngle < angle[2])
                 maxAngle = angle[2];

        }
   /*     public void createCirctle(CNodes[] arrayNodes)
        {
            double x1 = arrayNodes[node[0]].coords[0];
            double x2 = arrayNodes[node[1]].coords[0];
            double x3 = arrayNodes[node[2]].coords[0];

            double y1 = arrayNodes[node[0]].coords[1];
            double y2 = arrayNodes[node[1]].coords[1];
            double y3 = arrayNodes[node[2]].coords[1];
            double a = Jacobian(x1, y1, 1,
                                x2, y2, 1,
                                x3, y3, 1);
            double b = Jacobian(x1 * x1 + y1 * y1, y1, 1,
                                x2 * x2 + y2 * y2, y2, 1,
                                x3 * x3 + y3 * y3, y3, 1);
            double c = Jacobian(x1 * x1 + y1 * y1, x1, 1,
                                x2 * x2 + y2 * y2, x2, 1,
                                x3 * x3 + y3 * y3, x3, 1);
            double d = Jacobian(x1 * x1 + y1 * y1, x1, y1,
                                x2 * x2 + y2 * y2, x2, y2,
                                x3 * x3 + y3 * y3, x3, y3);
            center = new double[2];
            center[0] = b / (2 * a);
            center[1] = -c / (2 * a);
            //  r2 = (b*b+c*c -4*a*d)/(4*a*a);
            r2 = (x1 - center[0]) * (x1 - center[0]) + (y1 - center[1]) * (y1 - center[1]);
            double delt1 = (x1 - center[0]) * (x1 - center[0]) + (y1 - center[1]) * (y1 - center[1]) - r2;
            double delt2 = (x2 - center[0]) * (x2 - center[0]) + (y2 - center[1]) * (y2 - center[1]) - r2;
            double delt3 = (x3 - center[0]) * (x3 - center[0]) + (y3 - center[1]) * (y3 - center[1]) - r2;
            int tmp=0;
            if (r2 != r2)
                tmp = 98;
        }*/

      /*  public bool PointInCircle(CNodes node)
        {
            if ((node.coords[0] - center[0]) * (node.coords[0] - center[0]) + (node.coords[1] - center[1]) * (node.coords[1] - center[1]) <= r2)
                return true;
            else
                return false;
        }*/

           
        public int GetEdge(int n1, int n2, out bool rev)
        {
            rev = false;
            if (n1 == node[0] && n2 == node[1] || n2 == node[0] && n1 == node[1])
            {
                rev = (n2 == node[0] && n1 == node[1]);
                return 1;
            }
            else
                if (n1 == node[1] && n2 == node[2] || n2 == node[1] && n1 == node[2])
                {
                    rev = (n2 == node[1] && n1 == node[2]);
                    return 2;
                }
                else
                    if (n1 == node[0] && n2 == node[2] || n2 == node[0] && n1 == node[2])
                    {
                        rev = (n2 == node[0] && n1 == node[2]);
                        return 3;
                    }
            return -1;
        }
        public int GetEdge(int n1, int n2)
        {
            if (n1 == node[0] && n2 == node[1] || n2 == node[0] && n1 == node[1])
            {
                return 1;
            }
            else
                if (n1 == node[1] && n2 == node[2] || n2 == node[1] && n1 == node[2])
                {
                    return 2;
                }
                else
                    if (n1 == node[0] && n2 == node[2] || n2 == node[0] && n1 == node[2])
                    {
                        return 3;
                    }
            return -1;
        }
    }

    public class CTriangle
    {
        public double[] center;
      //  CNodes[] arrayNodes;
        public double r2;
        public bool[] edge;
        public int[] node;
        public CNodeTree[] aligmTriangl;
        public int numInList;
        public CTriangle()
        {
            node[0] = -1;
            node[1] = -1;
            node[2] = -1;
        }
    /*    public bool delEdge(int n1,int n2)
        {
            bool res = true;
            if (edge[0] || edge[1] || edge[2])
                res = false;
            if (node[0] == n1 && node[1] == n2 || node[0]  == n2 && node[1] == n1)
            {
                edge[0] = true;
                    return res;
            }
            else
                if (node[1] == n1 && node[2] == n2 || node[1] == n2 && node[2] == n1)
                {
                    edge[1] = true;
                    return res;
                }
                else
                    if (node[0] == n1 && node[2] == n2 || node[0] == n2 && node[2] == n1)
                    {
                        edge[2] = true;
                        return res;
                    }
            return false;
        }*/
 /*       public bool containAddpoint(int point)
        {
            for (int i=0;i<3;i++)
                if (node[i]>=point)
                    return true;
            return false;
        }*/
        double Jacobian(double J11,double J12,double J13,
                        double J21,double J22,double J23,
                        double J31,double J32,double J33)
        {
             return J11*J22*J33 + J12*J23*J31 +J21*J32*J13 - (J31*J22*J13 +J12*J21*J33 +J23*J32*J11 );
        }
        public int GetEdge(int n1,int n2,out bool rev)
        {
            rev = false;
            if (n1 == node[0] && n2 == node[1] || n2 == node[0] && n1 == node[1])
            {
                rev =( n2 == node[0] && n1 == node[1]);
                return 1;
            }
            else
                if (n1 == node[1] && n2 == node[2] || n2 == node[1] && n1 == node[2])
                {
                    rev =( n2 == node[1] && n1 == node[2]);
                    return 2; 
                }
                else
                    if (n1 == node[0] && n2 == node[2] || n2 == node[0] && n1 == node[2])
                    {
                        rev = (n2 == node[0] && n1 == node[2]);
                        return 3; 
                    }
            return -1;
        }
        public int GetEdge(int n1, int n2)
        {
            if (n1 == node[0] && n2 == node[1] || n2 == node[0] && n1 == node[1])
            {
                return 1;
            }
            else
                if (n1 == node[1] && n2 == node[2] || n2 == node[1] && n1 == node[2])
                {
                    return 2;
                }
                else
                    if (n1 == node[0] && n2 == node[2] || n2 == node[0] && n1 == node[2])
                    {
                        return 3;
                    }
            return -1;
        }
      
        public int pointInside(CNodes nodeIn, CNodes[] arrayNodes)
        {

            double x0 = nodeIn.coords[0];
            double x1 = arrayNodes[node[0]].coords[0];
            double x2 = arrayNodes[node[1]].coords[0];
            double x3 = arrayNodes[node[2]].coords[0];

            double y0 = nodeIn.coords[1];
            double y1 = arrayNodes[node[0]].coords[1];
            double y2 = arrayNodes[node[1]].coords[1];
            double y3 = arrayNodes[node[2]].coords[1];

            double SGlobal = Math.Abs(Jacobian(x1, y1, 1,
                                x2, y2, 1,
                                x3, y3, 1));

            double SLocal1= Math.Abs(Jacobian(x1, y1, 1,
                                x2, y2, 1,
                                x0, y0, 1));

            double SLocal2 =  Math.Abs(Jacobian(x2, y2, 1,
                                x3, y3, 1,
                                x0, y0, 1));

            double SLocal3 = Math.Abs(Jacobian(x1, y1, 1,
                                x3, y3, 1,
                                x0, y0, 1));

            if (SLocal1 + SLocal2 + SLocal3 + SGlobal * 10E-8 > SGlobal && SLocal1 + SLocal2 + SLocal3 - SGlobal * 10E-8 < SGlobal)
            {
                if (SLocal1 < SGlobal * 10E-8)
                {
                    return 1;
                }
                else
                    if (SLocal2 < SGlobal * 10E-8)
                    {
                        return 2;
                    }
                    else
                        if (SLocal3 < SGlobal * 10E-8)
                        {
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
            }
                else
                return -1;
        }
        public void createCirctle(CNodes[] arrayNodes)
        {
            double x1 = arrayNodes[node[0]].coords[0];
            double x2 = arrayNodes[node[1]].coords[0];
            double x3 = arrayNodes[node[2]].coords[0];

            double y1 = arrayNodes[node[0]].coords[1];
            double y2 = arrayNodes[node[1]].coords[1];
            double y3 = arrayNodes[node[2]].coords[1];
            double a = Jacobian(x1,y1,1,
                                x2,y2,1,
                                x3,y3,1);
              double b = Jacobian(x1*x1+y1*y1,y1,1,
                                  x2*x2+y2*y2,y2,1,
                                  x3*x3+y3*y3,y3,1);
              double c = Jacobian(x1*x1+y1*y1,x1,1,
                                  x2*x2+y2*y2,x2,1,
                                  x3*x3+y3*y3,x3,1);
             double d = Jacobian(x1*x1+y1*y1,x1,y1,
                                 x2*x2+y2*y2,x2,y2,
                                 x3*x3+y3*y3,x3,y3);
             center = new double[2];
            center[0]= b/(2*a);
            center[1]= -c/(2*a);
          //  r2 = (b*b+c*c -4*a*d)/(4*a*a);
            r2 = (x1 - center[0]) * (x1 - center[0]) + (y1 - center[1]) * (y1 - center[1]);
            double delt1 = (x1 - center[0]) * (x1 - center[0]) + (y1 - center[1]) * (y1 - center[1]) - r2;
            double delt2 = (x2 - center[0]) * (x2 - center[0]) + (y2 - center[1]) * (y2 - center[1]) - r2;
            double delt3 = (x3 - center[0]) * (x3 - center[0]) + (y3 - center[1]) * (y3 - center[1]) - r2;
            int tmp=3;
            if (r2 != r2)
                tmp = 4;
        }
        public bool PointInCircle(CNodes node)
        {
            if ((node.coords[0] - center[0]) * (node.coords[0] - center[0]) + (node.coords[1] - center[1]) * (node.coords[1] - center[1]) <= r2)
                return true;
            else
                return false;
        }
     
        
        public void AddLinks(CNodeTree ed1, CNodeTree ed2, CNodeTree ed3)
        {
            aligmTriangl[0] = ed1;
            aligmTriangl[1] = ed2;
            aligmTriangl[2] = ed3;

        }
        public CTriangle(int _node1, int _node2, int _node3, CNodes[] _arrayNodes)
        {
            if (_arrayNodes != null)
            {
                node = new int[3];
                aligmTriangl = new CNodeTree[3];

                node[0] = _node1;
                node[1] = _node2;
                node[2] = _node3;
                createCirctle(_arrayNodes);
            }
        }
    }

    public class CTriangulation
    {
        int countNodes;
        int CountAllNodes;
        public CNodes[] arrayNodes;
        public List<CTriangle> listTriangles;
        public CTriangleReconstruct[] arrayTriangles;
        public List<CTriangleReconstruct> ListReconTriangles;
        int marker;
        int[][] pointsInBand;
        List<int>[] edgesInPoint;
        int countBand;
        public int[] border;

        public bool ReadTringulation(string fileName)
        {
            List<CNodes> nodesTmp = new List<CNodes>();
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] res = (sr.ReadLine().Split(' ','\t')).Where(x1=>x1!="").ToArray();
                        double x,y,z;
                        if (!Double.TryParse(res[0],out x)||!Double.TryParse(res[1],out y) ||!Double.TryParse(res[2],out z) )
                        {
                            MessageBox.Show("Can't parse number");
                            return false;
                        }
                       nodesTmp.Add(new CNodes(x,y,z));
                    }
                    countNodes = nodesTmp.Count;
                    nodesTmp.Add(new CNodes(nodesTmp[0].coords[0],nodesTmp[0].coords[1],nodesTmp[0].coords[2]));
                    nodesTmp.Add(new CNodes(nodesTmp[0].coords[0],nodesTmp[0].coords[1],nodesTmp[0].coords[2]));
                    nodesTmp.Add(new CNodes(nodesTmp[0].coords[0],nodesTmp[0].coords[1],nodesTmp[0].coords[2]));
                    nodesTmp.Add(new CNodes(nodesTmp[0].coords[0],nodesTmp[0].coords[1],nodesTmp[0].coords[2]));
                    arrayNodes = nodesTmp.ToArray();
                }
            }
            else
            {
                MessageBox.Show("Can't find relief " + fileName);
                return false;
            }
            return true;
        }
        public bool vectorMult(int num1,int num2,int num3)
        {
            double x1 = arrayNodes[num1].coords[0], y1 = arrayNodes[num1].coords[1],
                   x2 = arrayNodes[num2].coords[0], y2 = arrayNodes[num2].coords[1],
                   x3 = arrayNodes[num3].coords[0], y3 = arrayNodes[num3].coords[1];

            if ((x2 - x1) * (y3 - y2) - (y2 - y1) * (x3 - x2) < 0)
                return false;
            else
                return true;

        }
        public class CEdgeLiveDead {
            
            public int EndEdge;
            public bool liveEdge;
            public CEdgeLiveDead(int _ed,bool _live)
            {
                EndEdge = _ed;
                liveEdge = _live;
            }
            public static bool operator< (CEdgeLiveDead ed1, CEdgeLiveDead ed2)
            {
                return ed1.EndEdge<ed2.EndEdge;
            }
            public static bool operator >(CEdgeLiveDead ed1, CEdgeLiveDead ed2)
            {
                return ed1.EndEdge > ed2.EndEdge;
            }
            public static bool operator ==(CEdgeLiveDead ed1, CEdgeLiveDead ed2)
            {
                return ed1.EndEdge == ed2.EndEdge;
            }
            public static bool operator !=(CEdgeLiveDead ed1, CEdgeLiveDead ed2)
            {
                return ed1.EndEdge != ed2.EndEdge;
            }
        }
        public void convexBandRight(List<int> curRightLine,int[] arrElem)
        {
            if (curRightLine.Count < 3)
                return;
            Stack<int> pointsRight = new Stack<int>();
            pointsRight.Push(curRightLine[0]);
            pointsRight.Push(curRightLine[1]);

            Queue<CEdgeLiveDead> edgeInProcessRight = new Queue<CEdgeLiveDead>();
            for (int j = 2; j < curRightLine.Count; j++)
            {
                int num2 = pointsRight.Pop(), num1 = pointsRight.Pop(), num3 = curRightLine[j];
                CTriangleReconstruct trian = new CTriangleReconstruct(arrElem[num1], arrElem[num2], arrElem[num3], arrayNodes);
                if (vectorMult(arrElem[num1], arrElem[num2],arrElem[num3]) || trian.maxAngle > Math.PI *3/4)
                {
                    pointsRight.Push(num1);
                    pointsRight.Push(num2);
                    pointsRight.Push(num3);
                }
                else
                {
                    pointsRight.Push(num1);
                    if (pointsRight.Count == 1)
                    {
                        pointsRight.Push(num3);
                    }
                    else
                        j--;
                }
            }
            curRightLine.Clear();
            foreach (var p in pointsRight.Reverse<int>())
            {
                curRightLine.Add(p);
            }
        }
        public void convexBandLeft(List<int> curLeftLine, int[] arrElem)
        {
            if (curLeftLine.Count < 3)
                return;
            Stack<int> pointsLeft = new Stack<int>();
            pointsLeft.Push(curLeftLine[0]);
            pointsLeft.Push(curLeftLine[1]);

            Queue<CEdgeLiveDead> edgeInProcessRight = new Queue<CEdgeLiveDead>();
            for (int j = 2; j < curLeftLine.Count; j++)
            {
                int num2 = pointsLeft.Pop(), num1 = pointsLeft.Pop(), num3 = curLeftLine[j];
                CTriangleReconstruct trian = new CTriangleReconstruct(arrElem[num1], arrElem[num2], arrElem[num3], arrayNodes);
                if (!vectorMult(arrElem[num1], arrElem[num2], arrElem[num3]) || trian.maxAngle > Math.PI * 3 / 4.0)
                {
                    pointsLeft.Push(num1);
                    pointsLeft.Push(num2);
                    pointsLeft.Push(num3);
                }
                else
                {
                    pointsLeft.Push(num1);
                    if (pointsLeft.Count == 1)
                    {
                        pointsLeft.Push(num3);
                    }
                    else
                        j--;
                }
            }
            curLeftLine.Clear();
            foreach (var p in pointsLeft.Reverse<int>())
            {
                curLeftLine.Add(p);
            }
        }

        public bool intersectionEdge(double minyLine, double maxyLine, int startTmp, double xP, double yP, int[] currentBand, List<int> curLeftLine,double[] aEdge,double[] bEdge)
        {

             int upy = startTmp ;
            int boty = startTmp - 1;
            double dxPred = xP - arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0];
            double dxPred1 = xP - arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[0];

            double dyPred = yP - arrayNodes[currentBand[curLeftLine[startTmp]]].coords[1];
            double dyPred1 = yP - arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[1];

            double ac, bc, ac1, bc1;
            if (Math.Abs(dxPred) < 10E-14)
            {
                ac = 0;
            }
            else
            {
                ac = dyPred / dxPred;
            }
            if (Math.Abs(dxPred1) < 10E-14)
            {
                ac1 = 0;
            }
            else
            {
                ac1 = dyPred1 / dxPred1;
            }
            bc = yP - xP * ac;
            bc1 = yP - xP * ac1;
            bool error = false;
            while (upy < curLeftLine.Count - 1 && arrayNodes[currentBand[curLeftLine[upy + 1]]].coords[1] < maxyLine)
            {
                double x2 =Math.Max(arrayNodes[currentBand[curLeftLine[upy + 1]]].coords[0], arrayNodes[currentBand[curLeftLine[upy]]].coords[0]);
                double x1 =Math.Min(arrayNodes[currentBand[curLeftLine[upy + 1]]].coords[0], arrayNodes[currentBand[curLeftLine[upy]]].coords[0]);;
                double dx = (x2 - x1) * 10E-14;
              
                if (Math.Abs(ac - aEdge[upy]) > 10E-15 * Math.Max(Math.Abs(ac), Math.Abs(aEdge[upy])))
                {
                    double nx = (-bc + bEdge[upy]) / (ac - aEdge[upy]);
                    if (nx < x2 && nx > x1 && Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0] - nx) > 10E-15 * Math.Max(Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0]), Math.Abs(nx)))
                    {
                        error = true;
                        break;
                    }
                  
                }
                if (Math.Abs(ac1 - aEdge[upy]) > 10E-15 * Math.Max(Math.Abs(ac1), Math.Abs(aEdge[upy])))
                {
                    double nx = (-bc1 + bEdge[upy]) / (ac1 - aEdge[upy]);
                    if (nx < x2 && nx > x1 && Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[0] - nx) > 10E-15 * Math.Max(Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[0]), Math.Abs(nx)))
                    {
                        error = true;
                        break;
                    }
                }
                upy++;
            }
            if (!error)
                while (boty > 0 && arrayNodes[currentBand[curLeftLine[boty-1]]].coords[1] > minyLine)
                {
                    double x2 = Math.Max(arrayNodes[currentBand[curLeftLine[boty - 1]]].coords[0], arrayNodes[currentBand[curLeftLine[boty]]].coords[0]);
                    double x1 = Math.Min(arrayNodes[currentBand[curLeftLine[boty - 1]]].coords[0], arrayNodes[currentBand[curLeftLine[boty]]].coords[0]); ;
                
                    if (Math.Abs(ac - aEdge[boty]) > 10E-15 * Math.Max(Math.Abs(ac), Math.Abs(aEdge[boty])) )
                    {
                        double nx = -(bc - bEdge[boty]) / (ac - aEdge[boty]);
                        if (nx < x2 && nx > x1 && Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0] - nx) >10E-15*Math.Max(Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0]), Math.Abs(nx)))
                        {
                            error = true;
                            break;
                        }
                    }

                    if (Math.Abs(ac1 - aEdge[boty]) > 10E-15 * Math.Max(Math.Abs(ac1), Math.Abs(aEdge[boty])))
                    {
                        double nx = -(bc1 - bEdge[boty]) / (ac1 - aEdge[boty]);
                        if (nx < x2 && nx > x1 && Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[0] - nx) > 10E-15 * Math.Max(Math.Abs(arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[0]), Math.Abs(nx)))
                        {
                            error = true;
                            break;
                        }
                    }
                }
            return error;
        }
        public CTriangulation(string file)
        {
            countNodes = 0;
            listTriangles = new List<CTriangle>();
            List<CEdgeLiveDead>[] nodeEdge;
            arrayNodes = new CNodes[0];
            countBand=100;
            List<int>[] pointsInBandTmp = new List<int>[countBand];
            ListReconTriangles=new List<CTriangleReconstruct>();
            edgesInPoint = new List<int>[countNodes];
            for (int i = 0; i < countBand; i++)
            {
                pointsInBandTmp[i] = new List<int>();
            }
                pointsInBand = new int[countBand][];
            if (ReadTringulation(file))
            {

                double minx = arrayNodes[0].coords[0],
                       maxx = arrayNodes[0].coords[0],
                       miny = arrayNodes[0].coords[1],
                       maxy = arrayNodes[0].coords[1];
                int minxNode = 0;
                int counter = 0;
                foreach (var p in arrayNodes)
                {
                    if (minx > p.coords[0])
                        minx = p.coords[0];
                    if (maxx < p.coords[0])
                        maxx = p.coords[0];
                    if (miny > p.coords[1])
                        miny = p.coords[1];
                    if (maxy < p.coords[1])
                        maxy = p.coords[1];

                     if (arrayNodes[minxNode].coords[0] > p.coords[0])
                        minxNode = counter;
                    counter++;
                }
                maxx += (maxx - minx)/10000;
                double h = (maxx-minx)/(countBand-1);
/*
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CNodes Tmp = arrayNodes[0];
                arrayNodes[0] = arrayNodes[minxNode];
                arrayNodes[minxNode] = Tmp;

                int[] nodes = new int[countNodes - 1];
                for (int i = 1; i < countNodes; i++)
                    nodes[i - 1] = i;
                QuickSortMin(0, countNodes - 2, arrayNodes[0].coords[0], arrayNodes[0].coords[1], nodes);
                Stack<int> points = new Stack<int>();
                points.Push(0);
                points.Push(nodes[0]);
   
                Queue<CEdgeLiveDead> edgeInProcess = new Queue<CEdgeLiveDead>();
                for (int i = 1; i < countNodes - 1; i++)
                {
                    int num2 = points.Pop(), num1 = points.Pop(), num3 = nodes[i];
                    if (vectorMult(num1, num2, num3))
                    {
                        points.Push(num1);
                        points.Push(num2);
                        points.Push(num3);
                    }
                    else
                    {
                        points.Push(num1);
                        i--;
                    }
                }
                border = points.ToArray();
                points.Clear();
                for (int i = 0; i < border.Length-1; i++)
                {
                    if (edgesInPoint[border[i]] == null)
                    {
                        edgesInPoint[border[i]] = new List<int> { border[i + 1] };
                    }
                    else
                    {
                        edgesInPoint[border[i]].Add(border[i + 1]);
                    }
                }
                edgesInPoint[border[0]].Add(border[border.Length - 1]);
                edgesInPoint[border[border.Length - 1]].Add(border[0]);
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                */
                for (int i = 0; i < countNodes; i++)
                { pointsInBandTmp[(int)((arrayNodes[i].coords[0] - minx) / h)].Add(i); }
               
                List<int> predLine= new List<int>();
                List<int> curRightLine = new List<int>();
                List<int> curLeftLine = new List<int>();

                double[] aPred=null, bPred=null;
                double[] aRight, bRight;
                int[] currentBandPred = null;
                for (int i = 0; i < countBand; i++)
                {
                    int tmp = 0;
                    if (i == 21)
                        tmp = 9;

                    pointsInBand[i] =  pointsInBandTmp[i].ToArray();
                    if (pointsInBand[i].Length!=0)
                        QuickSortY(0, pointsInBand[i].Length - 1, pointsInBand[i]);
                    pointsInBandTmp[i].Clear();
                    curRightLine.Clear();

                    curLeftLine.Clear();
                    int []currentBand =   pointsInBand[i];
                    if (currentBand.Length == 0)
                        continue;
                    int stPredBand = -1;
                    int endPredBand = -1;
                    if (currentBand.Length < 3)
                    {
                        int sti=0;
                        while (sti<currentBand.Length)
                        {
                            curRightLine.Add(sti);
                            curLeftLine.Add(sti);
                        }
                        endPredBand = currentBand.Length;
                    }
                    while (endPredBand < currentBand.Length-1)
                    {
                        if (endPredBand > 280)
                            tmp = 98;
                        double dy = (maxy - miny) / 100;
                        int startB = endPredBand + 1;
                        double ystart = arrayNodes[currentBand[startB]].coords[1];
                        double dx=0;
                        int endB = startB+1;
                        if (endB < currentBand.Length)
                        {
                            dx = arrayNodes[currentBand[endB]].coords[1] - arrayNodes[currentBand[startB]].coords[1];
                            dy = Math.Abs((ystart - arrayNodes[currentBand[endB]].coords[1]));
                        }
                        
                        List<int> equalCoords=new List<int>();
                        equalCoords.Add(startB);
                        while (endB < currentBand.Length && Math.Abs(arrayNodes[currentBand[endB]].coords[1] - arrayNodes[currentBand[startB]].coords[1])<(maxy-miny)/1000)
                        {
                           endB++;
                        }
                        endB--;
                        curRightLine.Add(endB);
                        curLeftLine.Add(startB);
                        if (endB != startB)
                            QuickSortX(startB, endB, currentBand);

                        if (endPredBand != -1)
                        {
                            if (endB != startB)
                            {
                                if (endPredBand - stPredBand == 0)
                                {
                                    int node =currentBand[ curLeftLine[curLeftLine.Count - 2]];
                                    int stTmp = startB;
                                    while (stTmp < endB)
                                    {
                                        ListReconTriangles.Add(new CTriangleReconstruct(node, currentBand[stTmp], currentBand[stTmp + 1], arrayNodes));
                                        stTmp++;
                                    }
                                }
                                else
                                {
                                    int stTmpPred = stPredBand;
                                    int startTmp = startB;
                                    while (stTmpPred < endPredBand && startTmp < endB)
                                    {
                                        CTriangleReconstruct tmpTr1 = new CTriangleReconstruct(currentBand[stTmpPred], currentBand[startTmp], currentBand[startTmp + 1], arrayNodes);
                                        CTriangleReconstruct tmpTr2 = new CTriangleReconstruct(currentBand[startTmp], currentBand[stTmpPred], currentBand[stTmpPred + 1], arrayNodes);
                                        if (tmpTr1.maxAngle < tmpTr2.maxAngle)
                                        {
                                            ListReconTriangles.Add(tmpTr1);
                                            startTmp++;
                                        }
                                        else
                                        {
                                            ListReconTriangles.Add(tmpTr2);
                                            stTmpPred++;
                                        }
                                    }

                                    if (stTmpPred == endPredBand)
                                    {
                                        while (startTmp < endB)
                                        {
                                            ListReconTriangles.Add(new CTriangleReconstruct(currentBand[stTmpPred], currentBand[startTmp], currentBand[startTmp + 1], arrayNodes));
                                            startTmp++;
                                        }
                                    }
                                    else
                                    {
                                        while (stTmpPred < endPredBand)
                                        {
                                            ListReconTriangles.Add(new CTriangleReconstruct(currentBand[startTmp], currentBand[stTmpPred], currentBand[stTmpPred + 1], arrayNodes));
                                            stTmpPred++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (endB - startB == 0)
                                {
                                    int node =  currentBand[curLeftLine[curLeftLine.Count - 1]];
                                    int stTmp = stPredBand;
                                    while (stTmp < endPredBand)
                                    {
                                        ListReconTriangles.Add(new CTriangleReconstruct(node, currentBand[stTmp], currentBand[stTmp + 1], arrayNodes));
                                        stTmp++;
                                    }
                                }
                            }
                        }

                        stPredBand=startB;
                        endPredBand=endB;
                    }

                   // break;
              //      convexBandLeft(curLeftLine, currentBand);
           //         
                    tmp = 0;
                    if ((i)*h>3300-minx)
                        tmp = 98;
                 //  
                //    convexBandRight(curRightLine, currentBand);
                  
                        if (predLine.Count != 0)
                        {
                           // aPred = new double[curLeftLine.Count - 1];
                           // bPred = new double[curLeftLine.Count - 1];

                            aRight = new double[curLeftLine.Count - 1];
                            bRight = new double[curLeftLine.Count - 1];

                            for (int j = 0; j < curLeftLine.Count - 1; j++)
                            {
                                double dx = arrayNodes[currentBand[curLeftLine[j + 1]]].coords[0] - arrayNodes[currentBand[curLeftLine[j]]].coords[0];
                                double dy = arrayNodes[currentBand[curLeftLine[j + 1]]].coords[1] - arrayNodes[currentBand[curLeftLine[j]]].coords[1];
                                if (Math.Abs(dx) < 10E-14)
                                {
                                    aRight[j] = 0;
                                }
                                else
                                {
                                    aRight[j] = dy / dx;
                                }
                                bRight[j] = arrayNodes[currentBand[curLeftLine[j + 1]]].coords[1] - arrayNodes[currentBand[curLeftLine[j + 1]]].coords[0] * aRight[j];
                            }
                            int stTmpPred = 0;
                            int startTmp = 0;
                            while (stTmpPred < predLine.Count - 1 && startTmp < curLeftLine.Count - 1)
                            {

                                double maxyLine = Math.Max(arrayNodes[currentBand[curLeftLine[startTmp + 1]]].coords[1], arrayNodes[currentBandPred[predLine[stTmpPred + 1]]].coords[1]);
                                double minyLine = Math.Max(arrayNodes[currentBand[curLeftLine[startTmp]]].coords[1], arrayNodes[currentBandPred[predLine[stTmpPred]]].coords[1]);
                                bool inter1 = intersectionEdge(minyLine, maxyLine, startTmp, arrayNodes[currentBandPred[predLine[stTmpPred]]].coords[0], arrayNodes[currentBandPred[predLine[stTmpPred]]].coords[1], currentBand, curLeftLine, aRight, bRight);
                                bool inter2 = intersectionEdge(minyLine, maxyLine, stTmpPred, arrayNodes[currentBand[curLeftLine[startTmp]]].coords[0], arrayNodes[currentBand[curLeftLine[startTmp]]].coords[1], currentBandPred, predLine, aPred, bPred);


                                CTriangleReconstruct tmpTr1 = new CTriangleReconstruct(currentBandPred[predLine[stTmpPred]], currentBand[curLeftLine[startTmp]], currentBand[curLeftLine[startTmp + 1]], arrayNodes);
                                CTriangleReconstruct tmpTr2 = new CTriangleReconstruct(currentBand[curLeftLine[startTmp]], currentBandPred[predLine[stTmpPred]], currentBandPred[predLine[stTmpPred + 1]], arrayNodes);
                                if (inter1 && inter2)
                                {
                                    if (arrayNodes[currentBand[curLeftLine[startTmp]]].coords[1] > arrayNodes[currentBandPred[predLine[stTmpPred]]].coords[1])
                                        stTmpPred++;
                                    else
                                        startTmp++;
                                }
                                else
                                {
                                    if (inter1)
                                    {
                                        stTmpPred++;
                                    }
                                    else
                                        if (inter2)
                                        {
                                            startTmp++;
                                        }
                                        else
                                            if (tmpTr1.maxAngle < tmpTr2.maxAngle)
                                            {
                                              //  if (tmpTr1.maxAngle /*< Math.PI * 9 / 10*/)
                                                {
                                                    ListReconTriangles.Add(tmpTr1);
                                                }
                                                startTmp++;
                                            }
                                            else
                                            {
                                             //   if (tmpTr2.maxAngle /*< Math.PI * 9 / 10*/)
                                                {
                                                    ListReconTriangles.Add(tmpTr2);
                                                }
                                                stTmpPred++;
                                            }
                                        }
                            }

                            if (stTmpPred == predLine.Count - 1)
                            {
                                stTmpPred = predLine.Count - 1;
                                while (startTmp < curLeftLine.Count - 1)
                                {
                                    ListReconTriangles.Add(new CTriangleReconstruct(currentBandPred[predLine[stTmpPred]], currentBand[curLeftLine[startTmp]], currentBand[curLeftLine[startTmp + 1]], arrayNodes));
                                    startTmp++;
                                }
                            }
                            else
                            {
                                startTmp = curLeftLine.Count - 1;
                                while (stTmpPred < predLine.Count - 1)
                                {
                                    ListReconTriangles.Add(new CTriangleReconstruct(currentBand[curLeftLine[startTmp]], currentBandPred[predLine[stTmpPred]], currentBandPred[predLine[stTmpPred + 1]], arrayNodes));
                                    stTmpPred++;
                                }
                            }
                        }
                        
                        aPred = new double[curRightLine.Count - 1];
                        bPred = new double[curRightLine.Count - 1];

                        for (int j = 0; j < curRightLine.Count - 1; j++)
                        {
                            double dx = arrayNodes[currentBand[curRightLine[j + 1]]].coords[0] - arrayNodes[currentBand[curRightLine[j]]].coords[0];
                            double dy = arrayNodes[currentBand[curRightLine[j + 1]]].coords[1] - arrayNodes[currentBand[curRightLine[j]]].coords[1];
                            if (Math.Abs(dx) < 10E-14)
                            {
                                aPred[j] = 0;
                            }
                            else
                            {
                                aPred[j] = dy / dx;
                            }
                            bPred[j] = arrayNodes[currentBand[curRightLine[j + 1]]].coords[1] - arrayNodes[currentBand[curRightLine[j + 1]]].coords[0] * aPred[j];
                        }
                    predLine.Clear();
                    foreach (var p in curRightLine)
                        predLine.Add(p);
                    currentBandPred = currentBand;
                }

                int res = 98;
                arrayTriangles=ListReconTriangles.ToArray();


               
      /*          double xBegP = arrayNodes[0].coords[0];
                double yBegP = arrayNodes[0].coords[1];
                double resi;
                nodeEdge = new List<CEdgeLiveDead>[countNodes];
                double pivot = -Math.PI;
                double lenghtp = 0;
                for (int i = 0; i < countNodes - 1; i++)
                {
                    double xcoordi = arrayNodes[nodes[i]].coords[0] - xBegP;
                    double ycoordi = arrayNodes[nodes[i]].coords[1] - yBegP;
                     if (xcoordi < 10E-15)
                        resi = Math.Sign(ycoordi) * Math.PI / 2;
                     else
                        resi = Math.Atan(ycoordi / xcoordi);
                    double lenghtpi = Math.Sqrt(xcoordi * xcoordi + ycoordi * ycoordi);

                    if (resi < pivot || resi == pivot && lenghtpi < lenghtp)
                    {
                        int error = 98;
                        error = 567;
                    }
                    pivot = resi;
                    lenghtp = lenghtpi;
                }

               
                for (int i = 0; i < border.Length; i++)
                {
                    arrayNodes[border[i]].process = true;
                }
                int nodeCenter=0;
                counter=0;
                double minDistance = maxx-minx+maxy-miny;
                minDistance*=minDistance;
                double centerx=(minx+maxx)/2,centery=(miny+maxy)/2;

                foreach (var p in arrayNodes)
                {
                   if (!p.process&&(p.coords[0]-centerx)*(p.coords[0]-centerx) + 
                       (p.coords[1]-centery)*(p.coords[1]-centery)<minDistance)
                   {
                       nodeCenter=counter;
                       minDistance= (p.coords[0]-centerx)*(p.coords[0]-centerx)+(p.coords[1]-centery)*(p.coords[1]-centery);
                   }
                       counter++;
                }
                arrayNodes[nodeCenter].process = true;


                CMyTree treeMy = new CMyTree();

                treeMy.topNode.childNode = new CNodeTree[border.Length];
                for (int i=0;i<border.Length-1;i++)
                {
                     treeMy.topNode.childNode[i] = new CNodeTree(new CTriangle(border[i],border[i+1],nodeCenter,arrayNodes));
                }
                 treeMy.topNode.childNode[border.Length-1] = new CNodeTree(new CTriangle(border[border.Length-1],border[0],nodeCenter,arrayNodes));
                 for (int i=0;i<border.Length;i++)
                {
                     treeMy.topNode.childNode[i].triangle.aligmTriangl[0]=null;
                     treeMy.topNode.childNode[i].triangle.aligmTriangl[1]=treeMy.topNode.childNode[(i+1)%border.Length];
                     if (i==0)
                        treeMy.topNode.childNode[i].triangle.aligmTriangl[2]=treeMy.topNode.childNode[border.Length-1];
                     else
                          treeMy.topNode.childNode[i].triangle.aligmTriangl[2]=treeMy.topNode.childNode[i-1];

                }
                int[] pointRev = new int[countNodes];
                for (int i=0;i<countNodes;i++)
                    pointRev[i] = i;
                QuickSortRev(0, countNodes-1, arrayNodes[nodeCenter].coords[0], arrayNodes[nodeCenter].coords[1],pointRev);
                int tmp=0;
                for (int i = 0; i < countNodes; i++)
                {
                    if (pointRev[i] == 17068)
                        tmp = 0;
                    if (arrayNodes[pointRev[i]].process)
                        continue;
                    treeMy.searchLastTriangle(arrayNodes, pointRev[i]);
                }
                
                Queue<CNodeTree> qTrian = new Queue<CNodeTree>() ;
                foreach (var nod in treeMy.topNode.childNode)
                    qTrian.Enqueue(nod);
                counter = 0;
                while (qTrian.Count > 0)
                {
                    CNodeTree nod = qTrian.Dequeue();

                    if (nod.childNode != null)
                    {
                        foreach (var p in nod.childNode)
                            qTrian.Enqueue(p);
                    }
                    else
                    {
                    //    if (nod.triangle.node[0] < countNodes && nod.triangle.node[1] < countNodes && nod.triangle.node[2] < countNodes)
                        {
                            if (counter == 13691)
                                tmp = 987;
                            nod.triangle.numInList = counter;
                            listTriangles.Add(nod.triangle);
                            counter++;
                        }
                    }
                }
                arrayTriangles = new CTriangleReconstruct[listTriangles.Count];
                counter = 0;
                foreach (var p in listTriangles)
                {
                    arrayTriangles[counter] = new CTriangleReconstruct(p.node[0], p.node[1], p.node[2],arrayNodes);
                    for (int i = 0; i < 3; i++)
                        if (p.aligmTriangl[i] != null)
                            arrayTriangles[counter].aligmTriangl[i] = p.aligmTriangl[i].triangle.numInList;
                        else
                            arrayTriangles[counter].aligmTriangl[i] = -1;
                    counter++;

                }*/
                //int[] aligtmo = new int[4];
                //int[] point = new int[4];
                //counter = 0;
                //for (int i = 0; i < arrayTriangles.Length; i++)
                //{
                //   if (counter > arrayTriangles.Length * 100)
                //        break;
                //    counter++;
                //    CTriangleReconstruct Opposite;
                //    if (i == 4)
                //        tmp = 98;
                //    if (arrayTriangles[i].aligmTriangl[0] != -1)
                //    {
                //        int ind = arrayTriangles[i].aligmTriangl[0];
                //        Opposite = arrayTriangles[ind];
                //        bool tp;
                //        int type0 = Opposite.GetEdge(arrayTriangles[i].node[0], arrayTriangles[i].node[1], out tp);
                //        double maxAngl2Triangl = Opposite.maxAngle;
                //        double maxAngl1Triangl = arrayTriangles[i].maxAngle;

                //        aligtmo[2] = arrayTriangles[i].aligmTriangl[1];
                //        aligtmo[3] = arrayTriangles[i].aligmTriangl[2];
                //        CTriangleReconstruct newTriangl1;
                //        CTriangleReconstruct newTriangl2;
                //        point[1] = arrayTriangles[i].node[2];
                //        getValueNewTriangle(Opposite, type0, tp, aligtmo, point);

                //        SetValueNewTr(out newTriangl1, out newTriangl2, aligtmo, point, i, ind);
                //        if (newTriangl1.maxAngle < maxAngl1Triangl && newTriangl2.maxAngle < maxAngl2Triangl && aligtmo[0] != aligtmo[1] && aligtmo[1] != aligtmo[2] && aligtmo[2] != aligtmo[3])
                //        {
                //            arrayTriangles[i] = newTriangl1;
                //            arrayTriangles[ind] = newTriangl2;
                //            SetValueNeight(newTriangl1, newTriangl2, aligtmo, i, ind);
                //            i = -1;
                //          //  break;
                //                continue;
                //        }

                //    }
                //    if (arrayTriangles[i].aligmTriangl[1] != -1)
                //    {
                //        int ind = arrayTriangles[i].aligmTriangl[1];
                //        Opposite = arrayTriangles[ind];
                //        bool tp;
                //        int DNME_marine = Opposite.GetEdge(arrayTriangles[i].node[1], arrayTriangles[i].node[2], out tp);
                //        double maxAngl2Triangl = Opposite.maxAngle;
                //        double maxAngl1Triangl = arrayTriangles[i].maxAngle;

                //        aligtmo[2] = arrayTriangles[i].aligmTriangl[2];
                //        aligtmo[3] = arrayTriangles[i].aligmTriangl[0];
                //        CTriangleReconstruct newTriangl1;
                //        CTriangleReconstruct newTriangl2;
                //        point[1] = arrayTriangles[i].node[0];
                //        getValueNewTriangle(Opposite, DNME_marine, tp, aligtmo, point);
                //        SetValueNewTr(out newTriangl1, out newTriangl2, aligtmo, point, i, ind);
                //        if (newTriangl1.maxAngle < maxAngl1Triangl && newTriangl2.maxAngle < maxAngl2Triangl && aligtmo[0] != aligtmo[1] && aligtmo[1] != aligtmo[2] && aligtmo[2] != aligtmo[3])
                //        {
                //            arrayTriangles[i] = newTriangl1;
                //            arrayTriangles[ind] = newTriangl2;
                //            SetValueNeight(newTriangl1, newTriangl2, aligtmo, i, ind);
                //            i = -1;
                //          //  break;
                //                continue;
                //        }
                //    }
                //    if (arrayTriangles[i].aligmTriangl[2] != -1)
                //    {
                //        int ind = arrayTriangles[i].aligmTriangl[2];
                //        Opposite = arrayTriangles[ind];
                //        bool tp;
                //        int type2 = Opposite.GetEdge(arrayTriangles[i].node[0], arrayTriangles[i].node[2], out tp);
                //        double maxAngl2Triangl = Opposite.maxAngle;
                //        double maxAngl1Triangl = arrayTriangles[i].maxAngle;
                //        aligtmo[2] = arrayTriangles[i].aligmTriangl[1];
                //        aligtmo[3] = arrayTriangles[i].aligmTriangl[0];
                //        CTriangleReconstruct newTriangl1;
                //        CTriangleReconstruct newTriangl2;
                //        point[1] = arrayTriangles[i].node[1];
                //        getValueNewTriangle(Opposite, type2, tp, aligtmo, point);
                //        SetValueNewTr(out newTriangl1, out newTriangl2, aligtmo, point, i, ind);
                //        if (newTriangl1.maxAngle < maxAngl1Triangl && newTriangl2.maxAngle < maxAngl2Triangl && aligtmo[0] != aligtmo[1] && aligtmo[1] != aligtmo[2] && aligtmo[2] != aligtmo[3])
                //        {
                //            arrayTriangles[i] = newTriangl1;
                //            arrayTriangles[ind] = newTriangl2;
                //            SetValueNeight(newTriangl1, newTriangl2, aligtmo, i, ind);
                //            i = -1;
                //          //  break;
                //                 continue;
                //        }
                //    }
                //}
            }
        }

        void getValueNewTriangle(CTriangleReconstruct Opposite, int type0, bool tp, int[] aligtmo, int[] point)
        {
            switch (type0)
            {
                case 1:
                    {
                        if (tp)
                        {
                            aligtmo[0] = Opposite.aligmTriangl[1];
                            aligtmo[1] = Opposite.aligmTriangl[2];
                            point[0] = Opposite.node[2];
                            point[2] = Opposite.node[0];
                            point[3] = Opposite.node[1];
                        }
                        else
                        {
                            aligtmo[0] = Opposite.aligmTriangl[2];
                            aligtmo[1] = Opposite.aligmTriangl[1];
                            point[0] = Opposite.node[2];
                            point[2] = Opposite.node[1];
                            point[3] = Opposite.node[0];
                        }

                    } break;
                case 2:
                    {
                        if (tp)
                        {
                            aligtmo[0] = Opposite.aligmTriangl[2];
                            aligtmo[1] = Opposite.aligmTriangl[0];
                            point[0] = Opposite.node[0];
                            point[2] = Opposite.node[1];
                            point[3] = Opposite.node[2];
                        }
                        else
                        {
                            aligtmo[0] = Opposite.aligmTriangl[0];
                            aligtmo[1] = Opposite.aligmTriangl[2];
                            point[0] = Opposite.node[0];
                            point[2] = Opposite.node[2];
                            point[3] = Opposite.node[1];
                        }
                    } break;
                case 3:
                    {
                        if (tp)
                        {
                            aligtmo[0] = Opposite.aligmTriangl[1];
                            aligtmo[1] = Opposite.aligmTriangl[0];
                            point[0] = Opposite.node[1];
                            point[2] = Opposite.node[0];
                            point[3] = Opposite.node[2];
                        }
                        else
                        {
                            aligtmo[0] = Opposite.aligmTriangl[0];
                            aligtmo[1] = Opposite.aligmTriangl[1];
                            point[0] = Opposite.node[1];
                            point[2] = Opposite.node[2];
                            point[3] = Opposite.node[0];
                        }
                    } break;
            }
        }
        void SetValueNeight(CTriangleReconstruct newTriangl1, CTriangleReconstruct newTriangl2, int[] aligtmo, int tr1, int tr2)
        {
            if (newTriangl1.aligmTriangl[1] != -1)
            {
                CTriangleReconstruct useTr = arrayTriangles[newTriangl1.aligmTriangl[1]];
                int cI = useTr.GetEdge(newTriangl1.node[1], newTriangl1.node[2]);
                useTr.aligmTriangl[cI - 1] = tr1;
            }
            if (newTriangl1.aligmTriangl[2] != -1)
            {
                CTriangleReconstruct useTr = arrayTriangles[newTriangl1.aligmTriangl[2]];
                int cI = useTr.GetEdge(newTriangl1.node[0], newTriangl1.node[2]);
                useTr.aligmTriangl[cI - 1] = tr1;
            }
            if (newTriangl2.aligmTriangl[1] != -1)
            {
                CTriangleReconstruct useTr = arrayTriangles[newTriangl2.aligmTriangl[1]];
                int cI = useTr.GetEdge(newTriangl2.node[1], newTriangl2.node[2]);
                useTr.aligmTriangl[cI - 1] = tr2;
            }
            if (newTriangl2.aligmTriangl[2] != -1)
            {
                CTriangleReconstruct useTr = arrayTriangles[newTriangl2.aligmTriangl[2]];
                int cI = useTr.GetEdge(newTriangl2.node[0], newTriangl2.node[2]);
                useTr.aligmTriangl[cI - 1] = tr2;
            }
        
        }
        void SetValueNewTr(out CTriangleReconstruct newTriangl1, out CTriangleReconstruct newTriangl2, int[] aligtmo, int[] point, int tr1, int tr2)
        {

            newTriangl1 = new CTriangleReconstruct(point[0], point[1], point[2], arrayNodes);
            newTriangl2 = new CTriangleReconstruct(point[0], point[1], point[3], arrayNodes);
            newTriangl1.aligmTriangl[0] = tr2;
            newTriangl1.aligmTriangl[1] = aligtmo[2];
            newTriangl1.aligmTriangl[2] = aligtmo[1];
            newTriangl2.aligmTriangl[0] = tr1;
            newTriangl2.aligmTriangl[1] = aligtmo[3];
            newTriangl2.aligmTriangl[2] = aligtmo[0];

          
        }

        void Gracham(int minNode)
        {

            int[] nodes = new int[countNodes - 1];
            for (int i = 1; i < countNodes; i++)
            {
                nodes[i-1] = i;
            }
        }
        public void QuickSortMin(int start, int end,double xBegP,double yBegP, int[] nodes)
        {
            double xcoord = arrayNodes[nodes[(start + end) / 2]].coords[0]-xBegP;
            double ycoord = arrayNodes[nodes[(start + end) / 2]].coords[1]-yBegP;
            double xcoordi;
            double ycoordi ;
            double xcoordj;
            double ycoordj ;
            double pivot=0;
            double resi;
            double resj;
            double lenghtp = Math.Sqrt(xcoord * xcoord + ycoord * ycoord);
            double lenghtpi, lenghtpj;
            if (xcoord<10E-15)
                pivot=Math.Sign(ycoord)*Math.PI/2;
            else
                pivot=Math.Atan(ycoord/xcoord);

            int i = start;
            int j = end;
            while (i <= j)
            {
                xcoordi = arrayNodes[nodes[i]].coords[0] - xBegP;
                ycoordi = arrayNodes[nodes[i]].coords[1] - yBegP;
                if (xcoordi < 10E-15)
                    resi = Math.Sign(ycoordi) * Math.PI / 2;
                else
                    resi = Math.Atan(ycoordi / xcoordi);
                lenghtpi = Math.Sqrt(xcoordi * xcoordi + ycoordi * ycoordi);
                while (resi < pivot || resi == pivot && lenghtpi<lenghtp)
                {
                    i++;
                    xcoordi = arrayNodes[nodes[i]].coords[0] - xBegP;
                    ycoordi = arrayNodes[nodes[i]].coords[1] - yBegP;
                    if (xcoordi < 10E-15)
                        resi = Math.Sign(ycoordi) * Math.PI / 2;
                    else
                        resi = Math.Atan(ycoordi / xcoordi);
                    lenghtpi = Math.Sqrt(xcoordi * xcoordi + ycoordi * ycoordi);
                }
               
                xcoordj = arrayNodes[nodes[j]].coords[0] - xBegP;
                ycoordj = arrayNodes[nodes[j]].coords[1] - yBegP;
                if (xcoordj < 10E-15)
                    resj = Math.Sign(ycoordj) * Math.PI / 2;
                else
                    resj = Math.Atan(ycoordj / xcoordj);
                lenghtpj = Math.Sqrt(xcoordj * xcoordj + ycoordj * ycoordj);

                while (resj > pivot || resj == pivot && lenghtpj>lenghtp)
                {
                    j--;
                    xcoordj = arrayNodes[nodes[j]].coords[0] - xBegP;
                    ycoordj = arrayNodes[nodes[j]].coords[1] - yBegP;
                    if (xcoordj < 10E-15)
                        resj = Math.Sign(ycoordj) * Math.PI / 2;
                    else
                        resj = Math.Atan(ycoordj / xcoordj);
                    lenghtpj = Math.Sqrt(xcoordj * xcoordj + ycoordj * ycoordj);
                }
                if (i <= j)
                {
                    int tmp = nodes[i];
                    nodes[i] = nodes[j];
                    nodes[j] = tmp;
                    i++;
                    j--;
                }
            }
            if (start < j)
                QuickSortMin(start, j,xBegP,yBegP,nodes);

            if (i < end)
                QuickSortMin(i, end, xBegP, yBegP, nodes);
        }
        public void QuickSortY(int start, int end, int[] nodes)
        {
            double xcoord = arrayNodes[nodes[(start + end) / 2]].coords[0];
            double ycoord = arrayNodes[nodes[(start + end) / 2]].coords[1];
            double xcoordi;
            double ycoordi;
            double xcoordj;
            double ycoordj;
            int i = start;
            int j = end;
            while (i <= j)
            {
                xcoordi = arrayNodes[nodes[i]].coords[0];
                ycoordi = arrayNodes[nodes[i]].coords[1];
                while (ycoordi < ycoord || ycoordi == ycoord && xcoordi < xcoord)
                {
                    i++;
                    xcoordi = arrayNodes[nodes[i]].coords[0];
                    ycoordi = arrayNodes[nodes[i]].coords[1];
                }

                xcoordj = arrayNodes[nodes[j]].coords[0];
                ycoordj = arrayNodes[nodes[j]].coords[1];

                while (ycoordj > ycoord || ycoordj == ycoord && xcoordj > xcoord)
                {
                    j--;
                    xcoordj = arrayNodes[nodes[j]].coords[0];
                    ycoordj = arrayNodes[nodes[j]].coords[1];
                }
                if (i <= j)
                {
                    int tmp = nodes[i];
                    nodes[i] = nodes[j];
                    nodes[j] = tmp;
                    i++;
                    j--;
                }
            }
            if (start < j)
                QuickSortY(start, j, nodes);

            if (i < end)
                QuickSortY(i, end, nodes);
        }
        public void QuickSortX(int start, int end, int[] nodes)
        {
            double xcoord = arrayNodes[nodes[(start + end) / 2]].coords[0];
            double xcoordi;
            double xcoordj;
            int i = start;
            int j = end;
            while (i <= j)
            {
                xcoordi = arrayNodes[nodes[i]].coords[0];
                while (xcoordi < xcoord)
                {
                    i++;
                    xcoordi = arrayNodes[nodes[i]].coords[0];
                }

                xcoordj = arrayNodes[nodes[j]].coords[0];

                while ( xcoordj > xcoord)
                {
                    j--;
                    xcoordj = arrayNodes[nodes[j]].coords[0];
                }
                if (i <= j)
                {
                    int tmp = nodes[i];
                    nodes[i] = nodes[j];
                    nodes[j] = tmp;
                    i++;
                    j--;
                }
            }
            if (start < j)
                QuickSortX(start, j, nodes);

            if (i < end)
                QuickSortX(i, end, nodes);
        }
        public void QuickSortRev(int start, int end, double xBegP, double yBegP, int[] nodes)
        {
            double xcoord = arrayNodes[nodes[(start + end) / 2]].coords[0] - xBegP;
            double ycoord = arrayNodes[nodes[(start + end) / 2]].coords[1] - yBegP;
            double xcoordi;
            double ycoordi;
            double xcoordj;
            double ycoordj;
            double pivot = 0;
         //   double resi;
          //  double resj;
            double lenghtp = Math.Sqrt(xcoord * xcoord + ycoord * ycoord);
            double lenghtpi, lenghtpj;
         /*   if (xcoord < 10E-15)
                pivot = Math.Sign(ycoord) * Math.PI / 2;
            else
                pivot = Math.Atan(ycoord / xcoord);
            */
            int i = start;
            int j = end;
            while (i <= j)
            {
                xcoordi = arrayNodes[nodes[i]].coords[0] - xBegP;
                ycoordi = arrayNodes[nodes[i]].coords[1] - yBegP;
              /*  if (xcoordi < 10E-15)
                    resi = Math.Sign(ycoordi) * Math.PI / 2;
                else
                    resi = Math.Atan(ycoordi / xcoordi);*/
                lenghtpi = Math.Sqrt(xcoordi * xcoordi + ycoordi * ycoordi);
                while (lenghtpi > lenghtp/* || lenghtpi == lenghtp&& resi > pivot */)
                {
                    i++;
                    xcoordi = arrayNodes[nodes[i]].coords[0] - xBegP;
                    ycoordi = arrayNodes[nodes[i]].coords[1] - yBegP;
                  /*  if (xcoordi < 10E-15)
                        resi = Math.Sign(ycoordi) * Math.PI / 2;
                    else
                        resi = Math.Atan(ycoordi / xcoordi);*/
                    lenghtpi = Math.Sqrt(xcoordi * xcoordi + ycoordi * ycoordi);
                }

                xcoordj = arrayNodes[nodes[j]].coords[0] - xBegP;
                ycoordj = arrayNodes[nodes[j]].coords[1] - yBegP;
               /* if (xcoordj < 10E-15)
                    resj = Math.Sign(ycoordj) * Math.PI / 2;
                else
                    resj = Math.Atan(ycoordj / xcoordj);*/
                lenghtpj = Math.Sqrt(xcoordj * xcoordj + ycoordj * ycoordj);

                while (lenghtpj < lenghtp /*|| resj == pivot && lenghtpj > lenghtp*/)
                {
                    j--;
                    xcoordj = arrayNodes[nodes[j]].coords[0] - xBegP;
                    ycoordj = arrayNodes[nodes[j]].coords[1] - yBegP;
                   /* if (xcoordj < 10E-15)
                        resj = Math.Sign(ycoordj) * Math.PI / 2;
                    else
                        resj = Math.Atan(ycoordj / xcoordj);*/
                    lenghtpj = Math.Sqrt(xcoordj * xcoordj + ycoordj * ycoordj);
                }
                if (i <= j)
                {
                    int tmp = nodes[i];
                    nodes[i] = nodes[j];
                    nodes[j] = tmp;
                    i++;
                    j--;
                }
            }
            if (start < j)
                QuickSortMin(start, j, xBegP, yBegP, nodes);

            if (i < end)
                QuickSortMin(i, end, xBegP, yBegP, nodes);
        }

               /* listTriangles.Add(new CTriangle(countNodes, countNodes + 1, countNodes + 3, arrayNodes));
                listTriangles.Add(new CTriangle(countNodes, countNodes + 3, countNodes + 2, arrayNodes));
                arrayNodes[countNodes].countTriangles++;
                arrayNodes[countNodes + 1].countTriangles++;
                arrayNodes[countNodes + 3].countTriangles++;
                arrayNodes[countNodes].countTriangles++;
                arrayNodes[countNodes + 3].countTriangles++;
                arrayNodes[countNodes + 2].countTriangles++;
                CountAllNodes = 4 + countNodes;
                Stack<int> listNodes = new Stack<int>() { };
                for (int i = CountAllNodes - 1; i >= 0; i--)
                {
                    listNodes.Push(i);
                }
                for (int i = 0; i < CountAllNodes; i++)
                {
                    List<int> delTrian1 = new List<int>();
                    List<CTriangle> newTriangle = new List<CTriangle>();
                    int counter = 0;

                    foreach (var p in listTriangles)
                    {

                        int type = p.pointInside(arrayNodes[i]);
                        switch (type)
                        {
                            case -1: counter++; continue;
                            case 1:
                                {
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                    newTriangle.Add(new CTriangle(i, p.node[1], p.node[2], arrayNodes));
                                    delTrian1.Add(counter);
                                }
                                break;
                            case 2:
                                {
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[1], arrayNodes));
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                    delTrian1.Add(counter);
                                }
                                break;
                            case 3:
                                {
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                    newTriangle.Add(new CTriangle(i, p.node[1], p.node[2], arrayNodes));
                                    delTrian1.Add(counter);
                                }
                                break;
                            case 4:
                                {
                                    delTrian1.Add(counter);
                                    newTriangle.Add(new CTriangle(p.node[1], i, p.node[0], arrayNodes));
                                    newTriangle.Add(new CTriangle(p.node[1], i, p.node[2], arrayNodes));
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                }break;
                        };
                        break;
                        if (type == -1)
                        {
                            counter++;
                            continue;
                        }
                        if (type == 1)
                        {
                            break;
                        }
                        else
                            if (type == 2)
                            {
                                newTriangle.Add(new CTriangle(p.node[0], i, p.node[1], arrayNodes));
                                newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                delTrian1.Add(counter);
                                break;
                            }
                            else
                                if (type == 3)
                                {
                                    newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                    newTriangle.Add(new CTriangle(i, p.node[1], p.node[2], arrayNodes));
                                    delTrian1.Add(counter);
                                    break;
                                }
                                else
                                    if (type==4)
                                    {
                                        delTrian1.Add(counter);
                                        newTriangle.Add(new CTriangle(p.node[1], i, p.node[0], arrayNodes));
                                        newTriangle.Add(new CTriangle(p.node[1], i, p.node[2], arrayNodes));
                                        newTriangle.Add(new CTriangle(p.node[0], i, p.node[2], arrayNodes));
                                        break;
                                    }
                       
                        counter++;
                    }
                    foreach (var p in delTrian1)
                    {
                        listTriangles.RemoveAt(p);
                    }

                    listTriangles.AddRange(newTriangle);
                }
            }
        }*/
                //while (listNodes.Count!=0)
          /*      for (int i = 0; i < CountAllNodes; i++)
                {
                   // int i = listNodes.Pop();
                    if (arrayNodes[i].countTriangles == 0)
                    {
                        List<int[]> edges = new List<int[]>();

                        List<int> delTrian1 = new List<int>();
                        List<int> delTrian2 = new List<int>();
                        int counter = 0;
                        foreach (var p in listTriangles)
                        {
                            if (p.PointInCircle(arrayNodes[i]))
                            {
                                bool existGlobalEdge = false;
                                if (p.node[0] >= countNodes && p.node[1] >= countNodes &&
                                    !(p.node[0] == countNodes && p.node[1] == countNodes + 3 ||
                                      p.node[1] == countNodes && p.node[0] == countNodes + 3) &&
                                      !(p.node[0] == countNodes + 1 && p.node[1] == countNodes + 2 ||
                                      p.node[1] == countNodes + 1 && p.node[0] == countNodes + 2))
                                    existGlobalEdge = true;
                                else
                                    edges.Add(new int[2] { p.node[0], p.node[1] });

                                if (p.node[1] >= countNodes && p.node[2] >= countNodes &&
                                   !(p.node[1] == countNodes && p.node[2] == countNodes + 3 ||
                                     p.node[2] == countNodes && p.node[1] == countNodes + 3) &&
                                     !(p.node[1] == countNodes + 1 && p.node[2] == countNodes + 2 ||
                                     p.node[2] == countNodes + 1 && p.node[1] == countNodes + 2))
                                    existGlobalEdge = true;
                                else
                                    edges.Add(new int[2] { p.node[1], p.node[2] });

                                if (p.node[0] >= countNodes && p.node[2] >= countNodes &&
                                   !(p.node[0] == countNodes && p.node[2] == countNodes + 3 ||
                                     p.node[2] == countNodes && p.node[0] == countNodes + 3) &&
                                     !(p.node[0] == countNodes + 1 && p.node[2] == countNodes + 2 ||
                                     p.node[2] == countNodes + 1 && p.node[0] == countNodes + 2))
                                    existGlobalEdge = true;
                                else
                                    edges.Add(new int[2] { p.node[0], p.node[2] });

                                if (!existGlobalEdge)
                                    delTrian1.Add(counter);
                            }
                            counter++;
                        }

                        foreach (var p in delTrian1.Reverse<int>())
                        {
                            CTriangle tmpTr = listTriangles[p];
                            for (int ind = 0; ind < 3; ind++)
                            {
                                arrayNodes[tmpTr.node[ind]].countTriangles--;
                                if (arrayNodes[tmpTr.node[ind]].countTriangles == 0)
                                    listNodes.Push(tmpTr.node[ind]);
                            }
                            listTriangles.RemoveAt(p);

                        }
                        counter = 0;
                        foreach (var p in listTriangles)
                        {
                            foreach (var p2 in edges)
                                if (p.delEdge(p2[0], p2[1]))
                                {
                                    delTrian2.Add(counter);
                                }
                            counter++;
                        }
                        List<CTriangle> newTriangle = new List<CTriangle>();
                        foreach (var p in delTrian2)
                        {
                            CTriangle tmpTr = listTriangles[p];
                            for (int j = 0; j < 3; j++)
                                if (!tmpTr.edge[j])
                                {
                                    newTriangle.Add(new CTriangle(i, tmpTr.node[j], tmpTr.node[(j + 1) % 3], arrayNodes));
                                    arrayNodes[i].countTriangles++;
                                    arrayNodes[tmpTr.node[j]].countTriangles++;
                                    arrayNodes[tmpTr.node[(j + 1) % 3]].countTriangles++;
                                }
                        }

                        foreach (var p in delTrian2.Reverse<int>())
                        {
                            CTriangle tmpTr = listTriangles[p];
                            for (int ind = 0; ind < 3; ind++)
                            {
                                arrayNodes[tmpTr.node[ind]].countTriangles--;
                                if (arrayNodes[tmpTr.node[ind]].countTriangles == 0)
                                    listNodes.Push(tmpTr.node[ind]);
                            }
                            listTriangles.RemoveAt(p);
                        }
                        i = -1;
                        listTriangles.AddRange(newTriangle);
                    }
                }

                    for (int i = listTriangles.Count - 1; i >= 0; i--)
                    {
                        if (listTriangles[i].containAddpoint(countNodes))
                        {
                            listTriangles.RemoveAt(i);
                            i++;
                        }
                    }
            }*/
        

        /*
        bool getIndexHor(int startx,int endx,int countPoint,out int startnew,out int endNew)
        {
            int ind1 = startx;
            int savep = 0;
            int counter =0;
            startnew = endNew =- 1;
            while (counter <= countPoint / 2)
            {
                if (processNode[sortCoords[0][ind1]] > 1)
                    counter++;
                ind1++;
            }

            savep=counter;
            int tmp = ind1;
            endNew = ind1-1;
            while (counter<countPoint && arrayNodes[sortCoords[0][tmp]].coords[0] == arrayNodes[sortCoords[0][ind1]].coords[0])
            {
                if (processNode[sortCoords[0][tmp]] > 1)
                {
                    endNew=tmp;
                    counter++;
                }
                tmp++;
            }
            if (countPoint - counter <= 2)
            {
                int tmpind1=ind1;
                ind1--;
                if (savep > 3)
                {
                    startnew = ind1;
                    while (savep >= 3 && processNode[sortCoords[0][ind1]] == processNode[sortCoords[0][tmpind1]])
                    {
                        if (processNode[sortCoords[0][tmpind1]] > 1)
                        { 
                            savep--;
                            startnew = tmpind1;
                        }
                        tmpind1--;
                    }
                    if (savep >= 3)
                    {
                        while (processNode[sortCoords[0][tmpind1]] < 2)
                        {
                            tmpind1--;
                        }
                        endNew = tmpind1;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            while (processNode[sortCoords[0][tmp]]<2)
            {
                    tmp++;
            }
            startnew = tmp;

            return true;
        }
        bool getIndexVert(int starty, int endy, int countPoint, out int startnew, out int endNew)
        {
            int ind1 = starty;
            int counter = 0;
            int savep;
            startnew = endNew = -1;
            while (counter <= countPoint / 2)
            {
                if (processNode[sortCoords[1][ind1]] > 1)
                    counter++;
                ind1++;
            }
            savep = counter;
            int tmp = ind1;
            endNew = ind1;
            while (counter < countPoint && arrayNodes[sortCoords[1][tmp]].coords[1] == arrayNodes[sortCoords[1][ind1]].coords[1])
            {
                if (processNode[sortCoords[1][tmp]] > 1)
                {
                    endNew = tmp;
                    counter++;
                }
                tmp++;
            }
            if (countPoint - counter <= 2)
            {
                int tmpind1 = ind1;
                ind1--;
                if (savep > 3)
                {
                    startnew = ind1;
                    while (savep >= 3 && processNode[sortCoords[1][ind1]] == processNode[sortCoords[1][tmpind1]])
                    {
                        if (processNode[sortCoords[1][tmpind1]] > 1)
                        {
                            savep--;
                            startnew = tmpind1;
                        }
                        tmpind1--;
                    }
                    if (savep >= 3)
                    {
                        while (processNode[sortCoords[1][tmpind1]] < 2)
                        {
                            tmpind1--;
                        }
                        endNew = tmpind1;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            while (processNode[sortCoords[1][tmp]] < 2)
            {
                tmp++;
            }
            startnew = tmp;

            return true;
        }

        public bool searchPoint(int startx, int starty, int endx, int endy)
        {
            for (int i = 0; i < countNodes; i++)
            {
                processNode[i] = 0;
            }
            for (int i = startx; i <= endx; i++)
            {
                processNode[sortCoords[0][i]] += 1;
            }
            int countPoint=0;
            List<CNodes> nodes = new List<CNodes>();
            for (int i = starty; i <= endy; i++)
            {
                processNode[sortCoords[1][i]] += 1;
                if (processNode[sortCoords[1][i]] > 1)
                {
                    countPoint++;
                    nodes.Add(arrayNodes[sortCoords[1][i]]);
                }
            }

            if (countPoint == 3)
            {
                listEdges.Add(new CEdges(nodes[0], nodes[1]));
                listEdges.Add(new CEdges(nodes[1], nodes[2]));
                listEdges.Add(new CEdges(nodes[2], nodes[0]));
                int cEdges = listEdges.Count;
                listTriangles.Add(new CTriangle(listEdges[cEdges - 1], listEdges[cEdges - 2], listEdges[cEdges - 3]));
                return true;
            }
            else
            {
                if (countPoint == 4)
                { 
                    
                }
            }
            double hx = arrayNodes[sortCoords[0][endx]].coords[0] - arrayNodes[sortCoords[0][startx]].coords[0];
            double hy = arrayNodes[sortCoords[1][endy]].coords[1] - arrayNodes[sortCoords[1][starty]].coords[1];


            int newstartx1=startx,newendx1,newstartx2,newendx2 = endx;
            int newstarty1=startx,newendy1,newstarty2,newendy2 = endy;
            bool res = true;
            if (hx>hy)
            {
                if (!getIndexHor(startx, endx, countPoint, out newstartx2, out newendx1))
                {
                    if (!getIndexVert(starty, endy, countPoint, out newstarty2, out newendy1))
                    {
                        MessageBox.Show("Can't triangulate");
                        res = false;
                    }
                    else 
                    {
                        res = res&&searchPoint(startx, newstarty1, endx, newendy1);
                        res = res && searchPoint(startx, newstarty2, endx, newendy2);
                    }

                }
                else
                {
                    res = res && searchPoint(newstartx1, starty, newendx1, endy);
                    res = res && searchPoint(newstartx2, starty, newendx2, endy);
                }
            }
            else
            {
                if (!getIndexVert(starty, endy, countPoint, out newstarty2, out newendy1))
                {
                    if (!getIndexHor(startx, endx, countPoint, out newstartx2, out newendx1))
                    {
                        MessageBox.Show("Can't triangulate");
                        res = false;
                    }
                    else 
                    {
                        res = res && searchPoint(newstartx1, starty, newendx1, endy);
                        res = res && searchPoint(newstartx2, starty, newendx2, endy);
                    }
                }
                else 
                {
                    res = res && searchPoint(startx, newstarty1, endx, newendy1);
                    res = res && searchPoint(startx, newstarty2, endx, newendy2);
                }
            }
            if (!res)
                return false;



            return true;
        }


        public void QuickSortMin(int start, int end,int coord)
        {
            double pivot = arrayNodes[sortCoords[coord][(start + end) / 2]].coords[coord];
            int i = start;
            int j = end;
            while (i <= j)
            {
                while (arrayNodes[sortCoords[coord][i]].coords[coord] < pivot)
                {
                    i++;
                }
                while (arrayNodes[sortCoords[coord][j]].coords[coord] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    int tmp = sortCoords[coord][i];
                    sortCoords[coord][i] = sortCoords[coord][j];
                    sortCoords[coord][j] = tmp;
                    i++;
                    j--;
                }
            }
            if (start < j)
                QuickSortMin(start, j,coord);

            if (i < end)
                QuickSortMin(i, end,coord);
        }
        */
    }
}
