using System.ComponentModel;

namespace Geology.GraphViewer
{
    public class GraphReceiver : INotifyPropertyChanged
    {
        private int index = -1;
        private bool selected = false;

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
                }
            }
        }

        public GraphReceiver()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
