using Geology.Controls;
using Geology.Observing;
using Geology.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Geology.GraphViewer
{
    public abstract class TableGraphValue : INotifyPropertyChanged
    {
        // ====================================== Fields
        protected int index = -1;
        protected bool selected = false;
        private string deviceName = "";
        protected TaskTypes taskType = TaskTypes.None;
        protected ObservableCollectionWithItemNotify<TypeModelValue> typeModelValues = new ObservableCollectionWithItemNotify<TypeModelValue>();
        protected ObservableCollection<CurveInformation> curves = new ObservableCollection<CurveInformation>();
        protected ObservableCollection<PickingPosition> pickingPositions = new ObservableCollection<PickingPosition>();
        protected ObservableCollectionWithItemNotify<GraphReceiver> graphReceivers = new ObservableCollectionWithItemNotify<GraphReceiver>();
        public GraphControl graphControl = null;
        public static ObservingSystem observingSystem = null; // bad

        /// ====================================== Properties
        public int Index
        {
            get { return index; }
            set
            {
                if (index != value)
                {
                    index = value;
                    NotifyPropertyChanged("Index");
                }
            }
        }
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyPropertyChanged("Selected");
                    UpdateCurves();
                }
            }
        }
        public TaskTypes TaskType
        {
            get { return taskType; }
            set
            {
                if (taskType != value)
                {
                    taskType = value;
                    NotifyPropertyChanged("TaskType");
                    TaskType_UpdateTasksList();
                    UpdateCurves();
                }
            }
        }
        public String TaskTypeS { get; set; }
        public ObservableCollection<TypeModelValue> TypeModelValues
        {
            get { return typeModelValues; }
        }
        public ReadOnlyObservableCollection<CurveInformation> Curves
        {
            get { return new ReadOnlyObservableCollection<CurveInformation>(curves); }
        }
        public ObservableCollection<PickingPosition> PickingPositions
        {
            get { return pickingPositions; }
        }

        protected bool IsValidDeviceName
        {
            get
            {
                if (ObservingData.devices == null)
                    return false;

                return ObservingData.devices.Values.ToList()
                    .Find(d => d.Name == deviceName) != null
                    ? true : false;
            }
        }
        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                if (deviceName != value)
                {
                    deviceName = value;
                    NotifyPropertyChanged("DeviceName");
                    Device_UpdateGraphReceivers();
                    UpdateCurves();
                }
            }
        }
        public ObservableCollection<GraphReceiver> GraphReceivers
        {
            get { return graphReceivers; }
        }

        // ====================================== Constructors
        public TableGraphValue()
        {
            typeModelValues.CollectionChanged += (sender, e) => { UpdateCurves(); };
            pickingPositions.CollectionChanged += (sender, e) => { UpdateCurves(); };
            graphReceivers.CollectionChanged += (sender, e) => { UpdateCurves(); };

            curves.CollectionChanged += Curves_CollectionChanged;
        }

        // ====================================== Functions
        public abstract void PickObservingPosition(ObservingPosition pos, Profile profile);
        protected abstract void UpdateCurves(List<System.Windows.Media.Color> colors = null);
        protected void TaskType_UpdateTasksList()
        {
            typeModelValues.Clear();

            // 1. --------------------------------------------------------- Practical
            bool addPractical = false;
            foreach (var prof in observingSystem.profiles)
            {
                if (prof.positions != null)
                    foreach (var pos in prof.positions)
                    {
                        var ll = pos.GetEdsAll(TaskType);
                        if (ll != null && ll.curves != null && ll.curves.Count > 0)
                        {
                            addPractical = true;
                            break;
                        }
                    }
            }
            if (addPractical)
                typeModelValues.Add(
                    new TypeModelValue()
                    {
                        Index = -1,
                        Selected = false,
                        TypeModel = new TypeModelPractical()
                    });

            // 2. --------------------------------------------------------- Direct
            bool addDirect = false;
            foreach (var prof in observingSystem.profiles)
            {
                if (prof.positions != null)
                    foreach (var pos in prof.positions)
                    {
                        var ll = pos.GetEdsAll(TaskType, CurvesDataType.Direct);
                        if (ll != null && ll.curves != null && ll.curves.Count > 0)
                        {
                            addDirect = true;
                            break;
                        }
                    }
            }
            if (addDirect)
                typeModelValues.Add(
                    new TypeModelValue()
                    {
                        Index = -1,
                        Selected = false,
                        TypeModel = new TypeModelDirect()
                    });

            // 3. --------------------------------------------------------- Inverse
            if (observingSystem == null || observingSystem.profiles == null)
                return;

            int maxCountCurves = 0;
            foreach (var prof in observingSystem.profiles)
            {
                if (prof.positions != null)
                    foreach (var pos in prof.positions)
                    {
                        var ll = pos.GetEdsAllInversion(TaskType);
                        if (ll != null)
                            maxCountCurves = Math.Max(maxCountCurves, ll.Count);
                    }
            }

            //var l = from prof0 in (from prof in observingSystem.profiles
            //                       where prof.positions != null
            //                       select prof)
            //        from pos0 in prof0.positions
            //        where pos0.GetEdsAll(TaskType, CurvesDatatType.Inversion) != null
            //        select pos0;

            //where(prof.positions != null
            //        && pos.GetEdsAll(TaskType, CurvesDatatType.Inversion) != null
            //        && pos.GetEdsAll(TaskType, CurvesDatatType.Inversion).curves != null)
            //select pos)
            // .DefaultIfEmpty();

            for (int i = 0; i < maxCountCurves; i++)
            {
                typeModelValues.Add(
                new TypeModelValue()
                {
                    Index = -1,
                    Selected = false,
                    TypeModel = new TypeModelInverse(i + 1)
                });
            }


            // Refresh
            int index = 1;
            foreach (var val in typeModelValues)
                val.Index = index++;
        }
        protected void Curves_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.OldItems != null)
                    foreach (INotifyPropertyChanged item in e.OldItems)
                    {
                        var elem = (CurveInformation)item;
                        RemoveGraph(elem);
                    }

                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                    {
                        var elem = (CurveInformation)item;
                        AddGraph(elem);
                    }
            }
        }
        private void Device_UpdateGraphReceivers()
        {
            graphReceivers.Clear();

            if (ObservingData.devices == null) return;
            if (ObservingData.devices.Count == 0) return;

            var currentDevice = ObservingData.devices.ToList().Find(d => d.Value.Name == DeviceName).Value;
            if (currentDevice == null) return;
            if (currentDevice.receivers == null) return;

            for (int i = 0; i < currentDevice.receivers.Count; i++)
                graphReceivers.Add(new GraphReceiver() { Index = i + 1 });
        }
        // Нужно функцию GetGraph() переопределить и вызывать ее внутри AddGraph()
        protected abstract void AddGraph(CurveInformation elem);
        protected void RemoveGraph(CurveInformation elem)
        {
            if (graphControl == null ||
                graphControl.TGraph == null ||
                graphControl.TGraph.CurvesInfoList == null)
                return;

            int index;
            while ((index = graphControl.TGraph.CurvesInfoList.ToList()
                .FindIndex(c => (CurveInformation)c.Id == elem)) != -1)
            {
                graphControl.TGraph.DeleteCurve(index);
            }
        }

        // ====================================== Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public abstract void ResetCurves();
        public abstract void ClearCurves();
    }

    public class TableGraphPointValue : TableGraphValue
    {
        // ====================================== Constructors
        public TableGraphPointValue()
            : base()
        {
        }

        // ====================================== Functions
        public override void PickObservingPosition(ObservingPosition pos, Profile profile)
        {
            if (pos == null)
                return;

            int index = PickingPositions.ToList() // TODO : BAD!! indexOf
                .FindIndex(posInfo => posInfo.ObservingPosition == pos);
            if (index == -1)
                PickingPositions.Add(new PickingPositionPoint(PickingPositions.Count + 1, pos, profile));
            else
                PickingPositions.RemoveAt(index);
        }
        protected override void UpdateCurves(List<System.Windows.Media.Color> colors = null)
        {
            if (!selected || !IsValidDeviceName || TaskType == TaskTypes.None)
            {
                // Old Problem: curves.Clear() => { Action = reset, OldItems = null }
                foreach (var elem in curves.ToList()) //
                    curves.Remove(elem);
                return;
            }

            var listReceiversIndex = new List<int>();
            for (int i = 0; i < graphReceivers.Count; i++)
            {
                if (graphReceivers[i].Selected) // + valid..
                    listReceiversIndex.Add(i);
            }

            var listTypesModel = new List<TypeModelValue>();
            foreach (var task in typeModelValues)
            {
                if (task.Selected)
                    listTypesModel.Add(task);
            }

            var listPositions = new List<PickingPosition>();
            foreach (var posInfo in pickingPositions)
            {
                // + valid..
                //var edsList = posInfo.ObservingPosition.GetEdsAll(TaskType);
                //if (edsList == null || edsList.curves == null || edsList.curves.Count == 0 || posInfo.ObservingPosition == null)
                //    continue;

                listPositions.Add(posInfo);
            }

            var listNew = new List<CurveInformation>();
            foreach (var pos in listPositions)
                foreach (var typeTask in listTypesModel)
                    foreach (var receivers_i in listReceiversIndex)
                    {
                        listNew.Add(new CurveInformationPoint
                        {
                            Position = pos,
                            ReceiverIndex = receivers_i,
                            TaskType = TaskType,
                            Color = Utilities.LittleTools.getRandomColor(),
                            BaseContainer = this,
                            Selected = true,
                            TypeModel = typeTask.TypeModel
                        });
                    }

            var listDel = curves.Except(listNew).ToList();
            var listAdd = listNew.Except(curves).ToList();

            foreach (var elem in listDel)
                curves.Remove(elem);

            if (colors != null)
                for (int i = 0; i < Math.Min(colors.Count, listAdd.Count); i++)
                    listAdd[i].Color = colors[i];
            foreach (var elem in listAdd.ToArray())
                    curves.Add(elem);

            if (listAdd.Count != 0)
                graphControl.TGraph.ViewAll(true, true);
            else
                graphControl.TGraph.Invalidate();
        }
        protected override void AddGraph(CurveInformation elem)
        {
            if (graphControl == null ||
                graphControl.TGraph == null ||
                graphControl.TGraph.CurvesInfoList == null)
                return;


            EdsAll.EdsAllFile edsList = null;

            var position = ((CurveInformationPoint)elem).Position;
            switch (elem.TypeModel.Type)
            {
                case TypeModelEnum.Practical:
                    edsList = position.ObservingPosition.GetEdsAll(elem.TaskType);
                    break;
                case TypeModelEnum.Inverse:
                    var listEdsInversion = position.ObservingPosition.GetEdsAllInversion(elem.TaskType);
                    if (listEdsInversion != null)
                    {
                        int iter = elem.TypeModel.Iteration - 1;
                        if (iter >= 0 && iter < listEdsInversion.Count)
                            edsList = listEdsInversion[iter];
                    }
                    break;
                case TypeModelEnum.Direct:
                    edsList = position.ObservingPosition.GetEdsAll(elem.TaskType, CurvesDataType.Direct);
                    break;
                default:
                    break;
            }

            if (edsList == null ||
                edsList.curves == null ||
                edsList.curves.Count == 0 ||
                elem.ReceiverIndex < 0 ||
                elem.ReceiverIndex >= edsList.curves.Count)
                return;

            var curve = new Objects.CCurve();
            foreach (var c in edsList.curves[elem.ReceiverIndex].curve)
                curve.Add(c.Time, c.Total);

            var curveInfo = new Objects.CCurveInfo()
            {
                Id = elem,
                Name = elem.Name,
                Color = elem.Color
            };

            // 1
            //BindingOperations.SetBinding(curveInfo, Objects.CCurveInfo.NameProperty, 
            //    /*Register("Name", typeof(string), typeof(Objects.CCurveInfo)),*/
            //    new Binding("Name") { Source = elem, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            // также в самом классе убрано..

            // 2 (плохо)
            elem.PropertyChanged += (sender1, e1) =>
            {
                if (e1.PropertyName == "Name")
                    curveInfo.Name = elem.Name;
            };

            graphControl.TGraph.AddCurve(curve);
            graphControl.TGraph.CurvesInfoList.Add(curveInfo);
        }
        public override void ClearCurves()
        {
            curves.Clear();
        }
        public override void ResetCurves()
        {
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            foreach (var curve in curves)
                colors.Add(curve.Color);

            selected = false;
            UpdateCurves();
            selected = true;
            UpdateCurves(colors);

            //for (int i = 0; i < Math.Min(curves.Count, colors.Count); i++)
            //    curves[i].Color = colors[i];
        }
    }

    public class TableGraphProfileValue : TableGraphValue
    {
        // ====================================== Fields
        private Meshes.MomentOfTime time;
        private ProfileDirection direction;
        private GraphReceiver receiver;

        // ====================================== Properties
        public Meshes.MomentOfTime Time
        {
            get { return time; }
            set
            {
                //if (time != value) // TODO: !!!
                //{
                time = value;
                NotifyPropertyChanged("Time");
                UpdateCurves();
                //}
            }
        }
        public ProfileDirection Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    NotifyPropertyChanged("Direction");
                    UpdateCurves();
                }
            }
        }
        public GraphReceiver Receiver
        {
            get { return receiver; }
            set
            {
                //if (receiver != value) // 
                //{
                //receiver = value; // PROBLEM!!!!!
                receiver = value;
                NotifyPropertyChanged("Receiver");
                UpdateCurves();
                //}
            }
        }

        // ====================================== Constructors
        public TableGraphProfileValue()
            : base()
        {
        }

        // ====================================== Functions
        public override void PickObservingPosition(ObservingPosition pos, Profile profile)
        {
            if (profile == null)
                return;

            int index = PickingPositions.Select(p => p.Profile).ToList().IndexOf(profile);
            if (index == -1)
                PickingPositions.Add(new PickingPositionProfile(PickingPositions.Count + 1, pos, profile));
            else
                PickingPositions.RemoveAt(index);
        }
        protected override void UpdateCurves(List<System.Windows.Media.Color> colors = null)
        {
            if (!selected || TaskType == TaskTypes.None)
            {
                // Old Problem: curves.Clear() => { Action = reset, OldItems = null }
                foreach (var elem in curves.ToList()) //
                    curves.Remove(elem);
                return;
            }

            //var listReceiversIndex = new List<int>();
            //for (int i = 0; i < graphReceivers.Count; i++)
            //{
            //    if (graphReceivers[i].Selected) // + valid..
            //        listReceiversIndex.Add(i);
            //}

            var listTypesModel = new List<TypeModelValue>();
            foreach (var task in typeModelValues)
            {
                if (task.Selected)
                    listTypesModel.Add(task);
            }

            var listPositions = new List<PickingPosition>();
            foreach (var posInfo in pickingPositions)
            {
                // TODO: ДРУГОЙ ВАЛИД!

                //// + valid..
                //var edsList = posInfo.ObservingPosition.GetEdsAll(TaskType);
                //if (edsList == null || edsList.curves == null || edsList.curves.Count == 0 || posInfo.ObservingPosition == null)
                //    continue;

                listPositions.Add(posInfo);
            }

            int time_i = Time.Number - 1; // ????
            int receiver_i = receiver.Index-1;//Receiver - 1;

            var listNew = new List<CurveInformation>();
            foreach (var pos in listPositions)
                foreach (var typeTask in listTypesModel)
                    listNew.Add(new CurveInformationProfile
                    {
                        Position = pos,
                        ReceiverIndex = receiver_i,
                        TaskType = TaskType,
                        Color = Utilities.LittleTools.getRandomColor(),
                        BaseContainer = this,
                        Selected = true,
                        TypeModel = typeTask.TypeModel,
                        Direction = direction,
                        TimeIndex = time_i
                    });

            var listAdd = listNew.Except(curves).ToList();
            var listDel = curves.Except(listNew).ToList();

            if (colors != null)
                for (int i = 0; i < Math.Min(colors.Count, listAdd.Count); i++)
                    listAdd[i].Color = colors[i];
            foreach (var elem in listDel)
                curves.Remove(elem);

            foreach (var elem in listAdd.ToArray())
                curves.Add(elem);

            if (listAdd.Count != 0)
                graphControl.TGraph.ViewAll(true, true);
            else
                graphControl.TGraph.Invalidate();
        }
        protected override void AddGraph(CurveInformation info)
        {
            if (graphControl == null ||
                graphControl.TGraph == null ||
                graphControl.TGraph.CurvesInfoList == null)
                return;

            var elem = (CurveInformationProfile)info;

            int rec_i = elem.ReceiverIndex;
            int time_i = elem.TimeIndex;

            var curve = new Objects.CCurve();
            foreach (var pos in elem.Position.Profile.positions)
            {
                EdsAll.EdsAllFile edsAllPos = null;

                switch (elem.TypeModel.Type)
                {
                    case TypeModelEnum.Practical:
                        edsAllPos = pos.GetEdsAll(TaskType);
                        break;
                    case TypeModelEnum.Inverse:
                        var edsAllInverse = pos.GetEdsAllInversion(TaskType);
                        if (edsAllInverse != null)
                        {
                            int iter = elem.TypeModel.Iteration - 1;
                            if (iter >= 0 && iter < edsAllInverse.Count)
                                edsAllPos = edsAllInverse[iter];
                        }
                        break;
                    case TypeModelEnum.Direct:
                        edsAllPos = pos.GetEdsAll(TaskType, CurvesDataType.Direct);
                        break;
                    default:
                        break;
                }

                if (edsAllPos != null && edsAllPos.curves != null)
                {
                    if (rec_i >= 0 && rec_i < edsAllPos.curves.Count)
                        if (edsAllPos.curves[rec_i].curve != null)
                            if (time_i >= 0 && time_i < edsAllPos.curves[rec_i].curve.Count)
                                curve.Add(ProfileDirectionHelper.FromPosition(elem.Direction, pos) /* ! */, edsAllPos.curves[rec_i].curve[time_i].Total);
                }
            }

            var curveInfo = new Objects.CCurveInfo()
            {
                Id = elem,
                Name = elem.Name,
                Color = elem.Color
            };

            elem.PropertyChanged += (sender1, e1) =>
            {
                if (e1.PropertyName == "Name")
                    curveInfo.Name = elem.Name;
            };

            graphControl.TGraph.AddCurve(curve);
            graphControl.TGraph.CurvesInfoList.Add(curveInfo);
        }
        public override void ClearCurves()
        {
            curves.Clear();
        }
        public override void ResetCurves()
        {
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            foreach (var curve in curves)
                colors.Add(curve.Color);

            selected = false;
            UpdateCurves();
            selected = true;
            UpdateCurves(colors);

            //for (int i = 0; i < Math.Min(curves.Count, colors.Count); i++)
            //    curves[i].Color = colors[i];
        }
    }
}
