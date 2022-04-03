using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Geology.GraphViewer
{
    public enum TypeModelEnum
    {
        [Geology.LanguageSupport.Description1("Practical")]
        Practical,
        [Geology.LanguageSupport.Description1("Direct")]
        Direct,
        [Geology.LanguageSupport.Description1("Inverse")]
        Inverse
    }

    public class TypeModel
    {
        public TypeModelEnum Type { get; set; }
        public int Iteration { get; set; }

        public TypeModel(TypeModelEnum type)
        {
            Type = type;
        }

        public static bool operator ==(TypeModel obj1, TypeModel obj2)
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

            return (obj1.Type.ToString() == obj2.Type.ToString()
                    && obj1.Iteration == obj2.Iteration);
        }
        public static bool operator !=(TypeModel obj1, TypeModel obj2)
        {
            return !(obj1 == obj2);
        }
        public bool Equals(TypeModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type.Equals(other.Type)
                   && Iteration.Equals(other.Iteration);
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
                int hashCode = Type.GetHashCode();
                hashCode = (hashCode * 397) ^ Iteration.GetHashCode();
                return hashCode;
            }
        }

    }

    public class TypeModelPractical : TypeModel
    {
        public TypeModelPractical()
            : base(TypeModelEnum.Practical)
        {
            Iteration = -1;
        }

        public override string ToString()
        {
            return LanguageSupport.LanguageLocalization.Get("Practical");
        }
    }

    public class TypeModelInverse : TypeModel
    {
        public TypeModelInverse(int iteration)
            : base(TypeModelEnum.Inverse)
        {
            Iteration = iteration;
        }

        public override string ToString()
        {
            return LanguageSupport.LanguageLocalization.Get("Inverse") + Iteration;
        }
    }

    public class TypeModelDirect : TypeModel
    {
        public TypeModelDirect()
            : base(TypeModelEnum.Direct)
        {
            Iteration = -1;
        }

        public override string ToString()
        {
            return LanguageSupport.LanguageLocalization.Get("Direct");
        }
    }

    public class TypeModelValue : INotifyPropertyChanged
    {
        // ====================================== Fields
        private int index;
        private TypeModel typeModel;
        private bool selected = false;

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
        public TypeModel TypeModel
        {
            get { return typeModel; }
            set
            {
                if (typeModel != value)
                {
                    typeModel = value;
                    NotifyPropertyChanged("TypeModel");
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

        // ====================================== Constructors
        public TypeModelValue()
        {
        }

        // ====================================== Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
