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
	public interface IModel
	{
		ObservableCollection<CGeoLayer> Layers { get; set; }
		ObservableCollection<CGeoObject> Objects { get; set; }
		ObservableCollection<SaturationVolumeStack> Stacks { get; set; }
		PolarizationDecayCurvesCollection PolarizationDecayCurvesCollection { get; set; }
		CGeoObject SelectedObject { get; set; }
		bool DrawObjectsBounds { get; set; }
		DataGrid DataGridObjects { get; set; }
		double[] GlobalBoundingBox { get; set; }
		event PropertyChangedEventHandler PropertyChanged;
	}
}
