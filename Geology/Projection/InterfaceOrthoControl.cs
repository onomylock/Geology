using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geology.DrawWindow
{
    public interface IOrthoControl
    {
        double[] AdditionalOrth { get; set; }
        double CoefHeightToWidth { get; set; }
        bool DoubleAxis { get; set; }
        bool AdditionalonJob { get; set; }
        double GetDHor { get; }
        double GetDVert { get; }
        double GetMinHor { get; }
        double GetMinVert { get; }

        double GetMaxHor { get; }
        double GetMaxVert { get; }

        double GetMinZBuf { get; }
        double GetMaxZBuf { get; }
        string HorAxisName { get; set; }
        string HorAxisName2 { get; set; }
        string VertAxisName { get; set; }
        //string styleFont { get; set; }
        //int sizeFont { get; set; }
        void GetOrtho(out double[] orth);
        void ConvertWorldToScreenCoord(int Width, int Height, double resX, double resY, out int eX, out int eY, int indentX = 0, int indentY = 0);
        void ConvertScreenToWorldCoord(int Width, int Height, int eX, int eY, out double resX, out double resY, int indentX = 0, int indentY = 0);
        void SetOrtho(double[] orth);
        void Translate(double hor, double vert);
        void Scale(double vert, double hor, double delt);
        void ClearView();



    }
}
