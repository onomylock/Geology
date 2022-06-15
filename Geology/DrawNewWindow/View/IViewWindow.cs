using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Geology.DrawWindow;

namespace Geology.DrawNewWindow.View
{
 //   public interface IViewWindowAndCurveInfo
 //   {
 //       Tuple<IViewWindow, ICurveInfoChaged> View {get;}
	//}

 //   public interface ICurveInfoChaged
	//{
 //       ObservableCollection<Objects.CCurveInfo> CurvesInfoList { get; set; }
 //   }

    public interface IViewWindow
    {
        void Paint(object sender, PaintEventArgs e);
        void ResizeWindow();
        void Release();
        void Prepare();
        void UpdateViewMatrix();
        void Draw();
		int Width { get; set; }
        int Height { get; set; }
        int WidthLocal { get; set; }
        int HeightLocal { get; set; }
        int OglContext { get; set; }
        FontGeology caption { get; }
        IntPtr Hdc { get; set; }
    }

    //public interface IViewChanged
    //{
    //    double SelectionX0 { get; set; }
    //    double SelectionX1 {get; set;}
    //    double SelectionY0 { get; set; }
    //    double SelectionY1 { get; set; }
    //}
}
