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
//using static Geology.DrawWindow.CObject3DDraw2D;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Objects;
using Geology.Objects.SaturationModel;

namespace Geology.DrawNewWindow.Model
{

	public interface IModelWindow
	{
		//Dictionary<PageType, List<IViewportObjectsSelectable>> selectableObjects { get; set; }
		//Dictionary<PageType, List<IViewportObjectsDrawable>> drawableObjects { get; set; }
		//Dictionary<PageType, List<IViewportObjectsClickable>> clickableObjects { get; set; }
		//Dictionary<PageType, List<IViewportObjectsContextmenuClickable>> contextMenuClickableObjects { get; set; }
		//Dictionary<PageType, List<IViewportMouseMoveReaction>> mouseMoveReactionObjects { get; set; }

		List<IViewportObjectsSelectable> viewportObjectsSelectablesGet(PageType pageType);
		List<IViewportObjectsDrawable> viewportObjectsDrawablesGet(PageType pageType);
		List<IViewportObjectsClickable> viewportObjectsClickablesGet(PageType pageType);
		List<IViewportObjectsContextmenuClickable> viewportObjectsContextmenuClickablesGet(PageType pageType);
		List<IViewportMouseMoveReaction> viewportMouseMoveReactionsGet(PageType pageType);

		//void viewportObjectsSelectablesSet(PageType pageType, List<CGeoObject> cGeoObjects);
		void viewportObjectsDrawablesSet(PageType pageType, List<CGeoObject> cGeoObjects);
		//void viewportObjectsClickablesSet(PageType pageType, List<CGeoObject> cGeoObjects);
		//void viewportObjectsContextmenuClickablesSet(PageType pageType, List<CGeoObject> cGeoObjects);
		//void viewportMouseMoveReactionsSet(PageType pageType, List<CGeoObject> cGeoObjects);
		

		//IViewportObjectsSelectable

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
