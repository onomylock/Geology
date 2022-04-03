using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using GLContex = Geology.OpenGL.OpenGL;
using Geology.Utilities;
using System.IO;

namespace Geology.Objects
{
    public class CCurveInfo : /*DependencyObject,*/ INotifyPropertyChanged
    {
        //public static DependencyProperty NameProperty =
        //   DependencyProperty.Register("Name", typeof(string),
        //   typeof(CCurveInfo), new UIPropertyMetadata(""));

        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Media.Color _Color;
        private String name;
        private String _Val;
        private bool visible;
        private Object id;
        //public LabelType lbType;
        //public int lbSize=3;
        //public bool lbFilled;
        //public LineType lnType;
        //public int lnWidth=1;
        //public int lnFact;
        //public int lbSkip=0;
        public String Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        public String Val
        {
            get { return _Val; }
            set { _Val = value; OnPropertyChanged("Val"); }
        }
        public bool Visible
        {
            get { return visible; }
            set { visible = value; OnPropertyChanged("Visible"); }
        }
        public System.Windows.Media.Color Color
        {
            get { return _Color; }
            set { _Color = value; OnPropertyChanged("Color"); }
        }
        public Object Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }

       
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public CCurveInfo()
        {
            Random rnd = new Random();
            Color = System.Windows.Media.Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            Name = "";
            Val = "";
            Visible = true;
            Id = null;
        }
        public CCurveInfo(String cName)
        {
            Random rnd = new Random();
            Color = System.Windows.Media.Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            Name = cName;
            Val = "";
            Visible = true;
            //lbType = LabelType.lbtDefault;
            //lbSize = 3;
            //lnType = LineType.lntDefault;
            //lnWidth = 1;
            //lbFilled = true;
            //lnFact = 1;
            //lbSkip = 0;
        }
    }
}