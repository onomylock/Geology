using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Geology.DrawWindow;
using Geology.Controls;
using Geology.Observing;

namespace Geology.GraphViewer
{
    public partial class ModelXYGraphViewer : CObject3DDraw2D
    {
        public GraphControl graphControl;
        public ITableGraph<TableGraphValue> tableGraph;

        public ModelXYGraphViewer()
            : base()
        {
            InitializeComponent();
        }

        private void HighlightPosition(int eX, int eY)
        {
            if (observingSystem == null) return;
            if (graphControl == null) return;

            ObservingPosition nearestPosition = null;
            double minDist = 1e+30;
            int ind = 0;

            Profile profile = null; // TODO!!!!!!!!!!!

            foreach (var prof in observingSystem.profiles)
            {
                if (prof.Active)
                {
                    foreach (var pos in prof.positions)
                    {
                        int scteenX, screenY;
                        Ortho.ConvertWorldToScreenCoord(
                            WidthLocal, HeightLocal,
                            pos.X, pos.Y,
                            out scteenX, out screenY,
                            captionHorAndVert.GetIndentVert, captionHorAndVert.GetIndentHor);

                        double dist = Math.Abs(scteenX - eX) + Math.Abs(screenY - eY);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestPosition = pos;
                            profile = prof;
                        }
                    }
                }
                ind++;
            }

            if (minDist > 15 || nearestPosition == null)
                return;

            tableGraph.PickObservingPosition(nearestPosition, profile);

            //ObservingPosition nearestPosition = null;
            //double minDist = 1e+30;
            //int profile_i = 0;
            //int ind = 0;
            //foreach (var prof in observingSystem.profiles)
            //{
            //    if (prof.Active)
            //        foreach (var pos in prof.positions)
            //        {
            //            double dist = Math.Abs(pos.X - curX) + Math.Abs(pos.Y - curY);
            //            if (dist < minDist)
            //            {
            //                minDist = dist;
            //                nearestPosition = pos;
            //                profile_i = ind;
            //            }
            //        }
            //    ind++;
            //}

            //if (!nearestPosition.isSelectedForGraph) // Add
            //{
            //    //graphStorage.AddGraphs(nearestPosition, observingSystem.profiles[profile_i], graphControl);
            //    if (tableGraphPoint.CurrentItem != null)
            //        tableGraphPoint.CurrentItem.ObservingPositions.Add(new ObservingPositionModel(0, nearestPosition));
            //}
            //else // Del
            //{
            //    //graphStorage.RemoveGraphs(nearestPosition, observingSystem.profiles[profile_i], graphControl);
            //}
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused) Focus();

            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                XPrevious = e.X; YPrevious = e.Y;

                HighlightPosition(e.X, e.Y);
            }

            //
            //base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (Focused) Parent.Focus();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!Focused) Focus();
        }
    }
}
