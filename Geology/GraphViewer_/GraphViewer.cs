//using Geology.Controls;
//using Geology.Observing;
//using Geology.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Windows;

//namespace Geology.GraphViewer
//{
//    public enum GraphCurveRegime
//    {
//        InPoint = 0,
//        InProile
//    }

//    public class GraphReceiverOld : INotifyPropertyChanged
//    {
//        private int index = -1;
//        private bool selected = false;

//        public int Index
//        {
//            get { return index; }
//            set { index = value; NotifyPropertyChanged("Index"); }
//        }
//        public bool Selected
//        {
//            get { return selected; }
//            set
//            {
//                if (selected == value)
//                    return;

//                selected = value;
//                NotifyPropertyChanged("Selected");

//                //Change.Invoke();
//            }
//        }

//        public GraphReceiverOld()
//        {
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        private void NotifyPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//            {
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//            }
//        }

//        //public static Action Change;
//    }

//    public class GraphStorage : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        private TaskTypes taskType = TaskTypes.None;
//        private GraphCurveRegime graphWindowRegime = GraphCurveRegime.InPoint;
//        private string deviceName = "";
//        private int time = 0;
//        private ObservableCollection<GraphReceiverOld> graphViewerReceivers = new ObservableCollection<GraphReceiverOld>();

//        public TaskTypes TaskType
//        {
//            get { return taskType; }
//            set { taskType = value; NotifyPropertyChanged("TaskType"); }
//        }
//        public GraphCurveRegime GraphWindowRegime
//        {
//            get { return graphWindowRegime; }
//            set { graphWindowRegime = value; NotifyPropertyChanged("GraphWindowRegime"); }
//        }
//        public string DeviceName
//        {
//            get { return deviceName; }
//            set
//            {
//                if (value == deviceName) return;

//                deviceName = value;
//                NotifyPropertyChanged("DeviceName");

//                if (Observing.ObservingData.devices == null) return;
//                if (Observing.ObservingData.devices.Count == 0) return;

//                var currentDevice = Observing.ObservingData.devices.ToList().Find(d => d.Value.Name == deviceName);
//                if (currentDevice.Value == null) return;
//                if (currentDevice.Value.receivers == null) return;

//                int n = currentDevice.Value.receivers.Count;

//                graphViewerReceivers.Clear();
//                for (int i = 0; i < n; i++)
//                    graphViewerReceivers.Add(new GraphReceiverOld());
//                RenumeNumbers();
//            }
//        }
//        public int Time
//        {
//            get { return time; }
//            set { time = value; NotifyPropertyChanged("Time"); }
//        }
//        public ObservableCollection<GraphReceiverOld> GraphViewerReceivers
//        {
//            get { return graphViewerReceivers; }
//        }

//        public GraphStorage() { }

//        public string NamePoint(ObservingPosition position, int rec_i)
//        {
//            return "point_" + position.Name + "_" + position.Number + "_" + rec_i + "_" + TaskType.ToString() + "_" + DeviceName;
//        }

//        public string NameProfile(Profile profile, int time_i)
//        {
//            return "profile_" + profile.Number + "_" + time_i + "_" + TaskType.ToString() + "_" + DeviceName;
//        }

//        public void AddGraphs(ObservingPosition position, Profile profile, GraphControl graphControl)
//        {
//            try
//            {
//                var edsList = position.GetEdsAll(TaskType);

//                if (edsList == null
//                    || edsList.curves == null
//                    || edsList.curves.Count == 0)
//                {
//                    //MessageBox.Show("List of " + TaskType.ToString() + "curves is empty.");
//                    return;
//                }

//                position.isSelectedForGraph = !position.isSelectedForGraph; //////

//                switch (GraphWindowRegime)
//                {
//                    case GraphCurveRegime.InPoint:
//                        {
//                            //var curve = new Objects.CCurve();
//                            //foreach (var c in edsList.curves.First().curve)
//                            //    curve.Add(c.Time, c.Total);

//                            //graphControl.TGraph.AddCurve(curve);

//                            //graphControl.TGraph.CurvesInfoList.Add(
//                            //    new Objects.CCurveInfo
//                            //    {
//                            //        Name = /*"x"*/NamePoint(position, 1),
//                            //        Color = Utilities.LittleTools.getRandomColor()
//                            //    });

//                            for (int i = 0; i < graphViewerReceivers.Count; i++)
//                            {
//                                if (!graphViewerReceivers[i].Selected) continue;

//                                var curve = new Objects.CCurve();
//                                foreach (var c in edsList.curves[i].curve)
//                                    curve.Add(c.Time, c.Total);

//                                graphControl.TGraph.AddCurve(curve);
//                                graphControl.TGraph.CurvesInfoList.Add(
//                                    new Objects.CCurveInfo
//                                    {
//                                        Name = /*"x"*/NamePoint(position, i + 1),
//                                        Color = Utilities.LittleTools.getRandomColor() //
//                                    });
//                            }
//                        }
//                        break;

//                    case GraphCurveRegime.InProile:
//                        {
//                            int receiver_i = 0;
//                            int time_i = Time/*0*/;

//                            var curve = new Objects.CCurve();
//                            foreach (var pos in profile.positions)
//                            {
//                                var edsAllPos = pos.GetEdsAll(TaskType);
//                                curve.Add(pos.X /**/, edsAllPos.curves[receiver_i].curve[time_i].Total);
//                            }

//                            graphControl.TGraph.AddCurve(curve);
//                            graphControl.TGraph.CurvesInfoList.Add(
//                                new Objects.CCurveInfo
//                                {
//                                    Name = /*"profile_"*/NameProfile(profile, time_i),
//                                    Color = Utilities.LittleTools.getRandomColor() //
//                                });
//                        }
//                        break;

//                    default:
//                        break;
//                }

//                if (graphControl.TGraph.Curves.Count == 1)
//                    graphControl.TGraph.ViewAll(true, true);
//                else
//                    graphControl.TGraph.Invalidate();
//            }
//            catch { }
//        }

//        public void RemoveGraphs(ObservingPosition position, Profile profile, GraphControl graphControl)
//        {
//            try
//            {
//                position.isSelectedForGraph = !position.isSelectedForGraph; //////

//                switch (GraphWindowRegime)
//                {
//                    case GraphCurveRegime.InPoint:
//                        {
//                            for (int pos = 0; pos < graphViewerReceivers.Count; pos++)
//                            {
//                                if (!graphViewerReceivers[pos].Selected) continue;

//                                string displayedName = NamePoint(position, pos + 1); ////
//                                for (int i = 0; ; i++)
//                                {
//                                    int ind1 = graphControl.CurvesInfoList.ToList().FindIndex(w => w.Name == displayedName);
//                                    if (ind1 == -1) break;
//                                    graphControl.TGraph.DeleteCurve(ind1);
//                                }
//                            }
//                        }
//                        break;

//                    case GraphCurveRegime.InProile:
//                        {
//                            int time_i = Time;

//                            string displayedName = NameProfile(profile, time_i); ////
//                            for (int i = 0; ; i++)
//                            {
//                                int ind1 = graphControl.CurvesInfoList.ToList().FindIndex(w => w.Name == displayedName);
//                                if (ind1 == -1) break;
//                                graphControl.TGraph.DeleteCurve(ind1);
//                            }
//                        }
//                        break;

//                    default:
//                        break;
//                }

//                if (graphControl.TGraph.Curves.Count == 1)
//                    graphControl.TGraph.ViewAll(true, true);
//                else
//                    graphControl.TGraph.Invalidate();
//            }
//            catch { }
//        }

//        private void RenumeNumbers()
//        {
//            int index = 1;
//            foreach (var elem in graphViewerReceivers)
//                elem.Index = index++;
//        }

//        private void NotifyPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//            {
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//            }
//        }
//    }

//    public class GraphReceiver : INotifyPropertyChanged
//    {
//        private int index = -1;
//        private bool selected = false;

//        public int Index
//        {
//            get { return index; }
//            set
//            {
//                if (index == value)
//                    return;

//                index = value;
//                NotifyPropertyChanged("Index");
//            }
//        }
//        public bool Selected
//        {
//            get { return selected; }
//            set
//            {
//                if (selected == value)
//                    return;

//                selected = value;
//                NotifyPropertyChanged("Selected");

//                Change.Invoke();
//            }
//        }

//        public GraphReceiver()
//        {
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        private void NotifyPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//            {
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//            }
//        }

//        public static Action Change;
//    }


//    public enum TypeModelEnum
//    {
//        Direct,
//        Inverse
//    }

//    public class TypeModel
//    {
//        public readonly TypeModelEnum TypeModelEnum;

//        public TypeModel(TypeModelEnum type)
//        {
//            TypeModelEnum = type;
//        }
//    }

//    public class TypeModelDirect : TypeModel
//    {
//        public TypeModelDirect()
//            : base(TypeModelEnum.Direct)
//        {
//        }

//        public override string ToString()
//        {
//            return "Direct";
//        }
//    }

//    public class TypeModelInverse : TypeModel
//    {
//        public int Iteration { get; set; }

//        public TypeModelInverse()
//            : base(TypeModelEnum.Inverse)
//        {
//        }

//        public override string ToString()
//        {
//            return "Inverse" + Iteration;
//        }
//    }

//    public class ObservingPositionModel
//    {
//        private int index;
//        private ObservingPosition observingPosition;
//        private bool selected;

//        public int Index
//        {
//            get { return index; }
//            set { index = value; }
//        }
//        public string Position
//        {
//            get { return observingPosition.Name + observingPosition.Number; }
//            //set { observingPosition.position = value; }
//        }
//        public bool Selected
//        {
//            get { return selected; }
//            set { selected = value; }
//        }

//        public ObservingPositionModel(ObservingPosition observingPosition)
//        {
//            index = 0;
//            this.observingPosition = observingPosition;
//            selected = false;
//        }
//    }

//    public class ObservingPositionAdapter
//    {
//        private ObservableCollection<ObservingPosition> observingPosition;
//        private ObservableCollection<ObservingPositionModel> observingPositionModel
//            = new ObservableCollection<ObservingPositionModel>();

//        public ObservableCollection<ObservingPositionModel> ObservingPositionModel
//        {
//            get { return observingPositionModel; }
//        }
//        public ObservableCollection<ObservingPosition> ObservingPosition
//        {
//            get { return observingPosition; }
//            set
//            {
//                if (observingPosition != value)
//                {
//                    if (observingPosition == null) // First Set
//                    {
//                        if (value != null)
//                        {
//                            ObservingPosition = value;
//                            observingPosition.CollectionChanged += (sender, e) =>
//                            {
//                                observingPositionModel.Clear();
//                                foreach (var elem in observingPosition)
//                                    observingPositionModel.Add(new ObservingPositionModel(elem));
//                            };
//                        }
//                    }
//                    else
//                    {
//                        observingPosition = value;
//                    }
//                }
//            }
//        }

//        public ObservingPositionAdapter()
//        {
//        }
//    }


//    public class TableGraphPointValue : INotifyPropertyChanged
//    {
//        private int index = -1;
//        private bool selected = false;
//        private string deviceName = "";
//        private TaskTypes taskType = TaskTypes.None;
//        private TypeModel typeModel = new TypeModelDirect();
//        private ObservingPositionAdapter observingPositionAdapter = new ObservingPositionAdapter();
//        private ObservableCollection<string> curvesIDs = new ObservableCollection<string>();

//        public ObservableCollection<GraphReceiver> graphReceivers = new ObservableCollection<GraphReceiver>();
//        public GraphControl graphControl = null;

//        public int Index
//        {
//            get { return index; }
//            set { index = value; NotifyPropertyChanged("Index"); }
//        }
//        public bool Selected
//        {
//            get { return selected; }
//            set
//            {
//                if (selected == value)
//                    return;

//                selected = value;
//                NotifyPropertyChanged("Selected");
//            }
//        }
//        public string DeviceName
//        {
//            get { return deviceName; }
//            set
//            {
//                if (deviceName == value)
//                    return;

//                deviceName = value;
//                NotifyPropertyChanged("DeviceName");

//                // Clear...
//                UpdateGraphReceivers();
//            }
//        }
//        public bool IsValidDeviceName
//        {
//            get
//            {
//                if (ObservingData.devices == null)
//                    return false;

//                return ObservingData.devices.Values.ToList()
//                    .Find(d => d.Name == deviceName) != null
//                    ? true : false;
//            }
//        }

//        public TaskTypes TaskType
//        {
//            get { return taskType; }
//            set
//            {
//                if (taskType == value)
//                    return;

//                taskType = value;
//                NotifyPropertyChanged("TaskType");
//            }
//        }
//        public TypeModel TypeModel
//        {
//            get { return typeModel; }
//            set
//            {
//                if (typeModel == value) // Add Equals operation
//                    return;

//                typeModel = value;
//                NotifyPropertyChanged("TypeModel");
//            }
//        }
//        public ObservingPositionAdapter ObservingPositionAdapter
//        {
//            get { return observingPositionAdapter; }
//        } // вынести только методы...
//        public ReadOnlyObservableCollection<string> CurvesIDs
//        {
//            get { return new ReadOnlyObservableCollection<string>(curvesIDs); }
//        }

//        public bool IsEnabled
//        {
//            get
//            {
//                if (!selected)
//                    return false;

//                if (graphReceivers == null)
//                    return false;
//                //if (observingPositions == null)
//                //    return false;

//                return true;
//            }
//        }

//        public TableGraphPointValue()
//        {
//            //GraphReceiverOld.Change += new Action<GraphReceiverOld>((GraphReceiverOld g) =>
//            //{
//            //    if (g.Selected)
//            //    {
//            //        //graphControlInfo.graphControl.TGraph.DeleteCurve(/*g.Index*/);
//            //        //graphControlInfo.graphControl.CurvesInfoList.Add();
//            //    }
//            //    else
//            //    {
//            //        //graphControlInfo.graphControl.CurvesInfoList.Remove();
//            //    }
//            //});
//        }
//        private void UpdateGraphReceivers()
//        {
//            if (graphReceivers != null)
//                graphReceivers.Clear();
//            else
//                graphReceivers = new ObservableCollection<GraphReceiver>();

//            if (ObservingData.devices == null) return;
//            if (ObservingData.devices.Count == 0) return;

//            var currentDevice = Observing.ObservingData.devices.ToList().Find(d => d.Value.Name == DeviceName).Value;
//            if (currentDevice == null) return;
//            if (currentDevice.receivers == null) return;

//            int n = currentDevice.receivers.Count;

//            for (int i = 0; i < n; i++)
//                graphReceivers.Add(new GraphReceiver());

//            int index = 1;
//            foreach (var rec in graphReceivers)
//                rec.Index = index++;
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        private void NotifyPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//            {
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//            }
//        }
//    }

//    public class TableGraphPoint
//    {
//        public ObservableCollection<TableGraphPointValue> values = new ObservableCollection<TableGraphPointValue>();

//        public TableGraphPoint()
//        {
//        }
//        public void Add(TableGraphPointValue value)
//        {
//            if (values == null)
//                values = new ObservableCollection<TableGraphPointValue>();

//            values.Add(value);
//        }
//        public void Remove(TableGraphPointValue value)
//        {
//            if (values == null)
//                return;

//            values.Remove(value);
//        }
//        public void RemoveAt(int index)
//        {
//            if (values == null)
//                return;

//            if (index < 0 || index >= values.Count)
//                return;

//            values.RemoveAt(index);
//        }
//    }
//}
