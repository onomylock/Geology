using Geology.Controls;
using Geology.Observing;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Geology.GraphViewer
{
    public delegate void SelectionItemChangedEventHandler();

    public interface ITableGraph<out T> where T : TableGraphValue
    {
        void PickObservingPosition(ObservingPosition position, Profile profile);

        // T -> TableGraphValue в TableGraph<T>

        //GraphControl GraphControl { get; set; }
        //TableGraphValue SelectedItem { get; set; }
        //ObservableCollection<TableGraphValue> Values { get; }
        //ObservableCollection<TypeModelValue> TypeModelValues { get; }
        //ObservableCollection<PickingPosition> ObservingPositions { get; }
        //ObservableCollection<GraphReceiver> GraphReceivers { get; }
        //ReadOnlyObservableCollection<CurveInformation> Curves { get; }
        //void Add(TableGraphValue value);
        //void Remove(TableGraphValue value);
        //event SelectionItemChangedEventHandler SelectionItemChanged;
    }

    public abstract class TableGraph<T> : ITableGraph<T> where T : TableGraphValue
    {
        protected T selectedItem = null;
        protected ObservableCollectionWithItemNotify<T> values =
            new ObservableCollectionWithItemNotify<T>();
        protected GraphControl graphControl = null;

        // ====================================== Properties
        public T SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifySelectionItemChanged();
                }
            }
        }
        public ObservableCollection<T> Values
        {
            get { return values; }
        }
        public GraphControl GraphControl
        {
            get { return graphControl; }
            set
            {
                if (graphControl != value) // TODO
                {
                    if (graphControl != null)
                        graphControl.ItemRemoved -= GraphControl_RemoveItem;

                    if (value != null)
                        value.ItemRemoved += GraphControl_RemoveItem;

                    graphControl = value;
                }
            }
        }
        public ObservableCollection<TypeModelValue> TypeModelValues
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                else
                    return SelectedItem.TypeModelValues;
            }
        }
        public ObservableCollection<PickingPosition> ObservingPositions
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                else
                    return SelectedItem.PickingPositions;
            }
        }
        public ReadOnlyObservableCollection<CurveInformation> Curves
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                else
                    return SelectedItem.Curves;
            }
        }

        // ====================================== Constructors
        public TableGraph()
        {
            values.CollectionChanged += (sender, e) =>
            {
                if (e != null)
                {
                    if (e.OldItems != null)
                        foreach (TableGraphValue item in e.OldItems)
                            item.Selected = false;

                    if (e.NewItems != null)
                        foreach (TableGraphValue item in e.NewItems)
                            item.graphControl = graphControl;
                }
            };
        }

        // ====================================== Functions
        private void GraphControl_RemoveItem(object o)
        {
            var curveInfo = o as CurveInformation;

            if (curveInfo != null)
                curveInfo.Selected = false;
        }
        public void Add(T value)
        {
            values.Add(value);
        }
        public void Remove(T value)
        {
            values.Remove(value);
        }
        public void PickObservingPosition(ObservingPosition position, Profile profile)
        {
            if (SelectedItem != null)
                SelectedItem.PickObservingPosition(position, profile);
        }

        // ====================================== Events
        public event SelectionItemChangedEventHandler SelectionItemChanged;
        protected void NotifySelectionItemChanged()
        {
            if (SelectionItemChanged != null)
                SelectionItemChanged();
        }
    }

    public class TableGraphPoint : TableGraph<TableGraphPointValue>
    {
        public ObservableCollection<GraphReceiver> GraphReceivers
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                else
                    return SelectedItem.GraphReceivers;
            }
        }
        public TableGraphPoint()
            : base()
        {
        }
    }

    public class TableGraphProfile : TableGraph<TableGraphProfileValue>
    {
        public Meshes.MomentOfTime Time
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                else
                    return SelectedItem.Time;
            }
        }
        public TableGraphProfile()
            : base()
        {
        }
    }

    //public class TableGraph
    //{
    //    protected TableGraphValue selectedItem = null;
    //    protected ObservableCollectionWithItemNotify<TableGraphValue> values =
    //        new ObservableCollectionWithItemNotify<TableGraphValue>();
    //    public GraphControl graphControl = null;

    //    // ====================================== Properties
    //    public TableGraphValue SelectedItem
    //    {
    //        get { return selectedItem; }
    //        set
    //        {
    //            if (selectedItem != value)
    //            {
    //                selectedItem = value;
    //                NotifySelectionItemChanged();
    //            }
    //        }
    //    }
    //    public ObservableCollection<TableGraphValue> Values
    //    {
    //        get { return values; }
    //    }

    //    public ObservableCollection<TypeModelValue> TypeModelValues
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.TypeModelValues;
    //        }
    //    }
    //    public ObservableCollection<PickingPosition> ObservingPositions
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.PickingPositions;
    //        }
    //    }
    //    public ObservableCollection<GraphReceiver> GraphReceivers
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.GraphReceivers;
    //        }
    //    }
    //    public ReadOnlyObservableCollection<CurveInformation> Curves
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.Curves;
    //        }
    //    }

    //    // ====================================== Constructors
    //    public TableGraph()
    //    {
    //        values.CollectionChanged += (sender, e) =>
    //        {
    //            if (e != null)
    //            {
    //                if (e.OldItems != null)
    //                    foreach (TableGraphValue item in e.OldItems)
    //                        item.Selected = false;

    //                if (e.NewItems != null)
    //                    foreach (TableGraphValue item in e.NewItems)
    //                        item.graphControl = graphControl;
    //            }
    //        };
    //    }

    //    // ====================================== Functions
    //    public void Add(TableGraphValue value)
    //    {
    //        values.Add(value);
    //    }
    //    public void Remove(TableGraphValue value)
    //    {
    //        values.Remove(value);
    //    }
    //    public void PickObservingPosition(ObservingPosition position, Profile profile)
    //    {
    //        if (SelectedItem != null)
    //            SelectedItem?.PickObservingPosition(position, profile);
    //    }

    //    // ====================================== Events
    //    public delegate void SelectionItemChangedEventHandler();
    //    public event SelectionItemChangedEventHandler SelectionItemChanged;
    //    private void NotifySelectionItemChanged()
    //    {
    //        if (SelectionItemChanged != null)
    //            SelectionItemChanged();
    //    }
    //}

    //public class TableGraphPoint
    //{
    //    // ====================================== Fields
    //    private TableGraphPointValue selectedItem = null;
    //    private ObservableCollectionWithItemNotify<TableGraphPointValue> values
    //        = new ObservableCollectionWithItemNotify<TableGraphPointValue>();

    //    // ====================================== Properties
    //    public TableGraphPointValue SelectedItem
    //    {
    //        get { return selectedItem; }
    //        set
    //        {
    //            if (selectedItem != value)
    //            {
    //                selectedItem = value;
    //                NotifySelectionItemChanged();
    //            }
    //        }
    //    }
    //    public ObservableCollection<TableGraphPointValue> Values
    //    {
    //        get { return values; }
    //    }

    //    public ObservableCollection<TypeModelValue> TypeModelValues
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.TypeModelValues;
    //        }
    //    }
    //    public ObservableCollection<PickingPosition> ObservingPositions
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.PickingPositions;
    //        }
    //    }
    //    public ObservableCollection<GraphReceiver> GraphReceivers
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.GraphReceivers;
    //        }
    //    }
    //    public ReadOnlyObservableCollection<CurveInformation> Curves
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.Curves;
    //        }
    //    }

    //    // ====================================== Constructors
    //    public TableGraphPoint()
    //    {
    //        values.CollectionChanged += (sender, e) =>
    //        {
    //            if (e != null)
    //            {
    //                if (e.OldItems != null)
    //                    foreach (TableGraphPointValue item in e.OldItems)
    //                        item.Selected = false;
    //            }
    //        };
    //    }

    //    // ====================================== Functions
    //    public void Add(TableGraphPointValue value)
    //    {
    //        values.Add(value);
    //    }
    //    public void Remove(TableGraphPointValue value)
    //    {
    //        values.Remove(value);
    //    }
    //    public void PickObservingPosition(ObservingPosition position, Profile profile)
    //    {
    //        if (SelectedItem != null)
    //            SelectedItem.PickObservingPosition(position, profile);
    //    }

    //    // ====================================== Events
    //    public delegate void SelectionItemChangedEventHandler();
    //    public event SelectionItemChangedEventHandler SelectionItemChanged;
    //    private void NotifySelectionItemChanged()
    //    {
    //        if (SelectionItemChanged != null)
    //            SelectionItemChanged();
    //    }
    //}

    //public class TableGraphProfile
    //{
    //    // ====================================== Fields
    //    private TableGraphProfileValue selectedItem = null;
    //    private ObservableCollectionWithItemNotify<TableGraphProfileValue> values
    //        = new ObservableCollectionWithItemNotify<TableGraphProfileValue>();

    //    // ====================================== Properties
    //    public TableGraphProfileValue SelectedItem
    //    {
    //        get { return selectedItem; }
    //        set
    //        {
    //            if (selectedItem != value)
    //            {
    //                selectedItem = value;
    //                NotifySelectionItemChanged();
    //            }
    //        }
    //    }
    //    public ObservableCollection<TableGraphProfileValue> Values
    //    {
    //        get { return values; }
    //    }

    //    public ObservableCollection<TypeModelValue> TypeModelValues
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.TypeModelValues;
    //        }
    //    }
    //    public ObservableCollection<PickingPosition> ObservingPositions
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.PickingPositions;
    //        }
    //    }
    //    public ReadOnlyObservableCollection<CurveInformation> Curves
    //    {
    //        get
    //        {
    //            if (SelectedItem == null)
    //                return null;
    //            else
    //                return SelectedItem.Curves;
    //        }
    //    }

    //    // ====================================== Constructors
    //    public TableGraphProfile()
    //    {
    //        values.CollectionChanged += (sender, e) =>
    //        {
    //            if (e != null)
    //            {
    //                if (e.OldItems != null)
    //                    foreach (TableGraphProfileValue item in e.OldItems)
    //                        item.Selected = false;
    //            }
    //        };
    //    }

    //    // ====================================== Functions
    //    public void Add(TableGraphProfileValue value)
    //    {
    //        values.Add(value);
    //    }
    //    public void Remove(TableGraphProfileValue value)
    //    {
    //        values.Remove(value);
    //    }
    //    public void PickObservingPosition(ObservingPosition position, Profile profile)
    //    {
    //        if (SelectedItem != null)
    //            SelectedItem.PickObservingPosition(position, profile);
    //    }

    //    // ====================================== Events
    //    public delegate void SelectionItemChangedEventHandler();
    //    public event SelectionItemChangedEventHandler SelectionItemChanged;
    //    private void NotifySelectionItemChanged()
    //    {
    //        if (SelectionItemChanged != null)
    //            SelectionItemChanged();
    //    }
    //}

    //public enum GraphCurveRegime
    //{
    //    InPoint = 0,
    //    InProile
    //}

    //public class GraphReceiverOld : INotifyPropertyChanged
    //{
    //    private int index = -1;
    //    private bool selected = false;

    //    public int Index
    //    {
    //        get { return index; }
    //        set { index = value; NotifyPropertyChanged("Index"); }
    //    }
    //    public bool Selected
    //    {
    //        get { return selected; }
    //        set
    //        {
    //            if (selected == value)
    //                return;

    //            selected = value;
    //            NotifyPropertyChanged("Selected");

    //            Change.Invoke();
    //        }
    //    }

    //    public GraphReceiverOld()
    //    {
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void NotifyPropertyChanged(string name)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(name));
    //        }
    //    }

    //    public static Action Change;
    //}

    //public class GraphStorage : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private TaskTypes taskType = TaskTypes.None;
    //    private GraphCurveRegime graphWindowRegime = GraphCurveRegime.InPoint;
    //    private string deviceName = "";
    //    private int time = 0;
    //    private ObservableCollection<GraphReceiverOld> graphViewerReceivers = new ObservableCollection<GraphReceiverOld>();

    //    public TaskTypes TaskType
    //    {
    //        get { return taskType; }
    //        set { taskType = value; NotifyPropertyChanged("TaskType"); }
    //    }
    //    public GraphCurveRegime GraphWindowRegime
    //    {
    //        get { return graphWindowRegime; }
    //        set { graphWindowRegime = value; NotifyPropertyChanged("GraphWindowRegime"); }
    //    }
    //    public string DeviceName
    //    {
    //        get { return deviceName; }
    //        set
    //        {
    //            if (value == deviceName) return;

    //            deviceName = value;
    //            NotifyPropertyChanged("DeviceName");

    //            if (Observing.ObservingData.devices == null) return;
    //            if (Observing.ObservingData.devices.Count == 0) return;

    //            var currentDevice = Observing.ObservingData.devices.ToList().Find(d => d.Value.Name == deviceName);
    //            if (currentDevice.Value == null) return;
    //            if (currentDevice.Value.receivers == null) return;

    //            int n = currentDevice.Value.receivers.Count;

    //            graphViewerReceivers.Clear();
    //            for (int i = 0; i < n; i++)
    //                graphViewerReceivers.Add(new GraphReceiverOld());
    //            RenumeNumbers();
    //        }
    //    }
    //    public int Time
    //    {
    //        get { return time; }
    //        set { time = value; NotifyPropertyChanged("Time"); }
    //    }
    //    public ObservableCollection<GraphReceiverOld> GraphViewerReceivers
    //    {
    //        get { return graphViewerReceivers; }
    //    }

    //    public GraphStorage() { }

    //    public string NamePoint(ObservingPosition position, int rec_i)
    //    {
    //        return "point_" + position.Name + "_" + position.Number + "_" + rec_i + "_" + TaskType.ToString() + "_" + DeviceName;
    //    }

    //    public string NameProfile(Profile profile, int time_i)
    //    {
    //        return "profile_" + profile.Number + "_" + time_i + "_" + TaskType.ToString() + "_" + DeviceName;
    //    }

    //    public void AddGraphs(ObservingPosition position, Profile profile, GraphControl graphControl)
    //    {
    //        try
    //        {
    //            var edsList = position.GetEdsAll(TaskType);

    //            if (edsList == null
    //                || edsList.curves == null
    //                || edsList.curves.Count == 0)
    //            {
    //                //MessageBox.Show("List of " + TaskType.ToString() + "curves is empty.");
    //                return;
    //            }

    //            position.isSelectedForGraph = !position.isSelectedForGraph; //////

    //            switch (GraphWindowRegime)
    //            {
    //                case GraphCurveRegime.InPoint:
    //                    {
    //                        //var curve = new Objects.CCurve();
    //                        //foreach (var c in edsList.curves.First().curve)
    //                        //    curve.Add(c.Time, c.Total);

    //                        //graphControl.TGraph.AddCurve(curve);

    //                        //graphControl.TGraph.CurvesInfoList.Add(
    //                        //    new Objects.CCurveInfo
    //                        //    {
    //                        //        Name = /*"x"*/NamePoint(position, 1),
    //                        //        Color = Utilities.LittleTools.getRandomColor()
    //                        //    });

    //                        for (int i = 0; i < graphViewerReceivers.Count; i++)
    //                        {
    //                            if (!graphViewerReceivers[i].Selected) continue;

    //                            var curve = new Objects.CCurve();
    //                            foreach (var c in edsList.curves[i].curve)
    //                                curve.Add(c.Time, c.Total);

    //                            graphControl.TGraph.AddCurve(curve);
    //                            graphControl.TGraph.CurvesInfoList.Add(
    //                                new Objects.CCurveInfo
    //                                {
    //                                    Name = /*"x"*/NamePoint(position, i + 1),
    //                                    Color = Utilities.LittleTools.getRandomColor() //
    //                                });
    //                        }
    //                    }
    //                    break;

    //                case GraphCurveRegime.InProile:
    //                    {
    //                        int receiver_i = 0;
    //                        int time_i = Time/*0*/;

    //                        var curve = new Objects.CCurve();
    //                        foreach (var pos in profile.positions)
    //                        {
    //                            var edsAllPos = pos.GetEdsAll(TaskType);
    //                            curve.Add(pos.X /**/, edsAllPos.curves[receiver_i].curve[time_i].Total);
    //                        }

    //                        graphControl.TGraph.AddCurve(curve);
    //                        graphControl.TGraph.CurvesInfoList.Add(
    //                            new Objects.CCurveInfo
    //                            {
    //                                Name = /*"profile_"*/NameProfile(profile, time_i),
    //                                Color = Utilities.LittleTools.getRandomColor() //
    //                            });
    //                    }
    //                    break;

    //                default:
    //                    break;
    //            }

    //            if (graphControl.TGraph.Curves.Count == 1)
    //                graphControl.TGraph.ViewAll(true, true);
    //            else
    //                graphControl.TGraph.Invalidate();
    //        }
    //        catch { }
    //    }

    //    public void RemoveGraphs(ObservingPosition position, Profile profile, GraphControl graphControl)
    //    {
    //        try
    //        {
    //            position.isSelectedForGraph = !position.isSelectedForGraph; //////

    //            switch (GraphWindowRegime)
    //            {
    //                case GraphCurveRegime.InPoint:
    //                    {
    //                        for (int pos = 0; pos < graphViewerReceivers.Count; pos++)
    //                        {
    //                            if (!graphViewerReceivers[pos].Selected) continue;

    //                            string displayedName = NamePoint(position, pos + 1); ////
    //                            for (int i = 0; ; i++)
    //                            {
    //                                int ind1 = graphControl.CurvesInfoList.ToList().FindIndex(w => w.Name == displayedName);
    //                                if (ind1 == -1) break;
    //                                graphControl.TGraph.DeleteCurve(ind1);
    //                            }
    //                        }
    //                    }
    //                    break;

    //                case GraphCurveRegime.InProile:
    //                    {
    //                        int time_i = Time;

    //                        string displayedName = NameProfile(profile, time_i); ////
    //                        for (int i = 0; ; i++)
    //                        {
    //                            int ind1 = graphControl.CurvesInfoList.ToList().FindIndex(w => w.Name == displayedName);
    //                            if (ind1 == -1) break;
    //                            graphControl.TGraph.DeleteCurve(ind1);
    //                        }
    //                    }
    //                    break;

    //                default:
    //                    break;
    //            }

    //            if (graphControl.TGraph.Curves.Count == 1)
    //                graphControl.TGraph.ViewAll(true, true);
    //            else
    //                graphControl.TGraph.Invalidate();
    //        }
    //        catch { }
    //    }

    //    private void RenumeNumbers()
    //    {
    //        int index = 1;
    //        foreach (var elem in graphViewerReceivers)
    //            elem.Index = index++;
    //    }

    //    private void NotifyPropertyChanged(string name)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(name));
    //        }
    //    }
    //}
}