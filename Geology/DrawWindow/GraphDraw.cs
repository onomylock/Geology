/*
 Файл содержит классы:
 * 
 * CGraphDraw, потомок CView2D - не используется
 * 
 */
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Geology.DrawWindow
//{
//    class CGraphDraw: CView2D
//    {
//        private Geology.MainWindow window;
//        public CGraphDraw() : base()
//        {
//            window = null; 

//            this.MouseMove += CView2D_MouseMove;
//        }
//        public void SetMainRef(Geology.MainWindow _window)
//        {
//            window = _window;
//        }
//        private void CView2D_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            window.StatusBarView.Text = "View: Graph";
//        }

//        public void ChangeBoundingBox(double[] newBoundingBox)
//        {
//            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };
//            for (int i = 0; i < 6; i++) BoundingBox[i] = newBoundingBox[i];
//        }

//        public void ChangeOrtho(double[] globalOrthoBox)
//        {
//            double[] newOrtho = new double[] { -1, 1, -1, 1, -1, 1 };

//                    newOrtho[0] = globalOrthoBox[0];
//                    newOrtho[1] = globalOrthoBox[1];
//                    newOrtho[2] = globalOrthoBox[2];
//                    newOrtho[3] = globalOrthoBox[3];
//                    newOrtho[4] = -1e+7;
//                    newOrtho[5] = 1e+7;

//            Ortho.SetOrtho(newOrtho);
//            Ortho.SetZBuffer(newOrtho[4], newOrtho[5]);
//            Resize_Window();
//        }
//    }
//}
