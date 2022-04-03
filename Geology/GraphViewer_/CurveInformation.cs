using Geology.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Geology.GraphViewer
{
    public abstract class CurveInformation : INotifyPropertyChanged
    {
        // ====================================== Fields
        protected int receiverIndex; // TODO: Receivers class
        protected PickingPosition position;
        protected TaskTypes taskType;
        protected Color color;
        protected string name;
        protected TableGraphValue baseContainer;
        protected bool selected = true;
        protected TypeModel typeModel;

        // ====================================== Properties
        public int ReceiverIndex
        {
            get { return receiverIndex; }
            set
            {
                if (receiverIndex != value)
                {
                    receiverIndex = value;
                    NotifyPropertyChanged("ReceiverIndex");
                    UpdateName();
                }
            }
        }
        public PickingPosition Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    if (position != null)
                        position.PropertyChanged -= Position_PropertyChanged;

                    if (value != null)
                        value.PropertyChanged += Position_PropertyChanged;

                    position = value;
                    UpdateName();
                    NotifyPropertyChanged("Position");
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
                    UpdateName();
                }
            }
        }
        public String TaskTypeS { get; set; }
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    NotifyPropertyChanged("Color");
                }
            }
        }
        public string Name
        {
            get { return name; }
            protected set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public TableGraphValue BaseContainer
        {
            get { return baseContainer; }
            set
            {
                if (baseContainer != value)
                {
                    baseContainer = value;
                    NotifyPropertyChanged("BaseContainer");
                }
            }
        }
        /////////////////////////////////// TODO!
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }
        public TypeModel TypeModel
        {
            get { return typeModel; }
            set
            {
                //if (typeModel != value) // TODO: ADD
                //{
                typeModel = value;
                NotifyPropertyChanged("TypeModel");
                UpdateName();
                //}
            }
        }

        // ====================================== Constructors
        public CurveInformation()
        {
            UpdateName();
        }

        // ====================================== Functions
        protected abstract void UpdateName();
        private void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var triggeredPropertyNames = new List<string> { "Name"/*, "TypeModel"*/ };

            if (triggeredPropertyNames.Contains(e.PropertyName))
                UpdateName();
        }

        // ====================================== Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        // ====================================== Operators
        public static bool operator ==(CurveInformation obj1, CurveInformation obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return (obj1.Name == obj2.Name
                    && obj1.ReceiverIndex == obj2.ReceiverIndex
                    && obj1.Position == obj2.Position
                    && obj1.TaskType == obj2.TaskType
                    && obj1.TypeModel == obj2.TypeModel);
        }
        public static bool operator !=(CurveInformation obj1, CurveInformation obj2)
        {
            return !(obj1 == obj2);
        }
        public bool Equals(CurveInformation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name.Equals(other.Name)
                   && ReceiverIndex.Equals(other.ReceiverIndex)
                   && Position.Equals(other.Position)
                   && TaskType.Equals(other.TaskType)
                   && TypeModel.Equals(other.TypeModel);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((CurveInformation)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ ReceiverIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ TaskType.GetHashCode();
                hashCode = (hashCode * 397) ^ TypeModel.GetHashCode();
                return hashCode;
            }
        }
    }

    public class CurveInformationPoint : CurveInformation
    {
        // ====================================== Constructors
        public CurveInformationPoint()
            : base()
        {
        }

        // ====================================== Functions
        protected override void UpdateName()
        {
            //Name = "pos:" + (Position != null ? Position.Name : "?")
            //    + "_rec:" + (ReceiverIndex + 1)
            //    + "_task:" + TaskType
            //    + "_type:" + (TypeModel != null ? TypeModel.ToString().Substring(0, 3) : "?");
            Name = "p=" + (Position != null ? Position.Name : "?")
                + "_r=" + (ReceiverIndex + 1)
                + "_" + TaskType
                + "_" + (TypeModel != null ? TypeModel.ToString().Substring(0, 3) : "?");
            if (TypeModel != null)
                if (TypeModel.ToString().Substring(0, 3) == "Inv")
                    name += TypeModel.Iteration.ToString();
        }
    }

    public class CurveInformationProfile : CurveInformation
    {
        // ====================================== Fields
        private int timeIndex;
        private ProfileDirection direction;

        // ====================================== Properties
        public int TimeIndex
        {
            get { return timeIndex; }
            set
            {
                if (timeIndex != value)
                {
                    timeIndex = value;
                    NotifyPropertyChanged("TimeIndex");
                    UpdateName();
                }
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
                }
            }
        }

        // ====================================== Constructors
        public CurveInformationProfile()
            : base()
        {
        }

        // ====================================== Functions
        protected override void UpdateName()
        {
            //Name = "pos:" + (Position != null ? Position.Name : "?")
            //        + "_rec:" + (ReceiverIndex + 1) // ?
            //        + "_task:" + TaskType
            //        + "_time:" + TimeIndex
            //        + "_type:" + (TypeModel != null ? TypeModel.ToString().Substring(0, 3) : "?");
            Name = "p=" + (Position != null ? Position.Name : "?")
                    + "_r=" + (ReceiverIndex + 1) // ?
                    + "_" + TaskType
                    + "_t=" + (TimeIndex+1)
                    + "_" + (TypeModel != null ? TypeModel.ToString().Substring(0, 3) : "?");
            if (TypeModel != null)
                if (TypeModel.ToString().Substring(0, 3) == "Inv")
                    name += TypeModel.Iteration.ToString();
        }

        public static bool operator ==(CurveInformationProfile obj1, CurveInformationProfile obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return ((CurveInformation)obj1 == (CurveInformation)obj2
                    && obj1.TimeIndex == obj2.TimeIndex);
        }
        public static bool operator !=(CurveInformationProfile obj1, CurveInformationProfile obj2)
        {
            return !(obj1 == obj2);
        }
        public bool Equals(CurveInformationProfile other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Equals((CurveInformation)other)
                   && TimeIndex.Equals(other.TimeIndex);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return base.Equals(obj) && obj.GetType() == GetType() && Equals((CurveInformationProfile)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ TypeModel.GetHashCode();
                return hashCode;
            }
        }

    }
}
