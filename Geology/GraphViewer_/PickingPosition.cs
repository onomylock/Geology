using Geology.Observing;
using System.Collections.Generic;
using System.ComponentModel;

namespace Geology.GraphViewer
{
    public abstract class PickingPosition : INotifyPropertyChanged
    {
        // ====================================== Fields
        protected int index = -1;
        protected string name = "";
        protected ObservingPosition observingPosition = null;
        protected Profile profile = null;
        public const string DefaultName = "?";

        // ====================================== Properties
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
        public ObservingPosition ObservingPosition
        {
            get { return observingPosition; }
            set
            {
                if (observingPosition != value)
                {
                    if (observingPosition != null)
                        observingPosition.PropertyChanged -= Position_PropertyChanged;

                    if (value != null)
                        value.PropertyChanged += Position_PropertyChanged;

                    observingPosition = value;
                    UpdateName();
                    NotifyPropertyChanged("ObservingPosition");
                }
            }
        }
        public Profile Profile
        {
            get { return profile; }
            set
            {
                if (profile != value)
                {
                    if (profile != null)
                        profile.PropertyChanged -= Profile_PropertyChanged;

                    if (value != null)
                        value.PropertyChanged += Profile_PropertyChanged;

                    profile = value;
                    UpdateName();
                    NotifyPropertyChanged("Profile");
                }
            }
        }

        // ====================================== Constructors
        public PickingPosition()
        {
        }
        public PickingPosition(int index, ObservingPosition observingPosition, Profile profile)
        {
            Index = index;
            ObservingPosition = observingPosition;
            Profile = profile;
        }

        // ====================================== Functions
        protected abstract void UpdateName();
        protected void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var listUpdatedNames = new List<string> { "Number" };
            if (listUpdatedNames.Contains(e.PropertyName))
                UpdateName();
        }
        protected void Profile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var listUpdatedNames = new List<string> { "Number" };
            if (listUpdatedNames.Contains(e.PropertyName))
                UpdateName();
        }

        // ====================================== Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        // TODO: operations
    }

    public class PickingPositionPoint : PickingPosition
    {
        // ====================================== Constructors
        public PickingPositionPoint()
            : base()
        {
        }
        public PickingPositionPoint(int index, ObservingPosition observingPosition, Profile profile)
            : base(index, observingPosition, profile)
        {
        }

        // ====================================== Functions
        protected override void UpdateName()
        {
            Name = (Profile == null ? DefaultName : Profile.Number.ToString())
                + "_" + (ObservingPosition == null ? DefaultName : ObservingPosition.Number.ToString());
        }
    }

    public class PickingPositionProfile : PickingPosition
    {
        // ====================================== Constructors
        public PickingPositionProfile()
            : base()
        {
        }
        public PickingPositionProfile(int index, ObservingPosition observingPosition, Profile profile)
            : base(index, observingPosition, profile)
        {
        }

        // ====================================== Functions
        protected override void UpdateName()
        {
            Name = (Profile == null ? DefaultName : Profile.Number.ToString());
        }
    }
}
