using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geology.DrawWindow
{
    public class LightSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        bool enabled = false;
        bool showLight = false;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        float diffuse = 1f;
        float specular = 1f;
        float ambient = 0f;
        float shininess = 50f;

        public float X
        {
            get { return x; }
            set { x = value; OnPropertyChanged("X"); }
        }
        public float Y
        {
            get { return y; }
            set { y = value; OnPropertyChanged("Y"); }
        }
        public float Z
        {
            get { return z; }
            set { z = value; OnPropertyChanged("Z"); }
        }
        public float Diffuse
        {
            get { return diffuse; }
            set { diffuse = value < 0f ? 0f : value > 1f ? 1f : value; OnPropertyChanged("Diffuse"); }
        }
        public float Ambient
        {
            get { return ambient; }
            set { ambient = value < 0f ? 0f : value > 1f ? 1f : value; ; OnPropertyChanged("Ambient"); }
        }
        public float Shininess
        {
            get { return shininess; }
            set { shininess = value < 0f ? 0f : value > 128f ? 128f : value; ; OnPropertyChanged("Shininess"); }
        }
        public float Specular
        {
            get { return specular; }
            set { specular = value < 0f ? 0f : value > 1f ? 1f : value; ; OnPropertyChanged("Specular"); }
        }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; OnPropertyChanged("Enabled"); }
        }
        public bool ShowLight
        {
            get { return showLight; }
            set { showLight = value; OnPropertyChanged("ShowLight"); }
        }

        public LightSettings()
        {

        }
        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                outputFile.WriteLine(X);
                outputFile.WriteLine(Y);
                outputFile.WriteLine(Z);
                outputFile.WriteLine(Diffuse);
                outputFile.WriteLine(Ambient);
                outputFile.WriteLine(Shininess);
                outputFile.WriteLine(Specular);
                outputFile.WriteLine(Enabled);
                outputFile.WriteLine(ShowLight);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public int Read(ref StreamReader inputFile, double fileVersion)
        {
            try
            {
                if (fileVersion >= 1.0)
                {
                    X = float.Parse(inputFile.ReadLine());
                    Y = float.Parse(inputFile.ReadLine());
                    Z = float.Parse(inputFile.ReadLine());
                    Diffuse = float.Parse(inputFile.ReadLine());
                    Ambient = float.Parse(inputFile.ReadLine());
                    Shininess = float.Parse(inputFile.ReadLine());
                    Specular = float.Parse(inputFile.ReadLine());
                    Enabled = bool.Parse(inputFile.ReadLine());
                    ShowLight = bool.Parse(inputFile.ReadLine());
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        private void OnPropertyChanged(String property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
    public class GlobalDrawingSettingsBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        double discreteX = 100, discreteY = 100, discreteZ = 50;
        public double DiscreteX
        {
            set { discreteX = value; OnPropertyChanged("DiscreteX"); }
            get { return discreteX; }
        }
        public double DiscreteY
        {
            set { discreteY = value; OnPropertyChanged("DiscreteY"); }
            get { return discreteY; }
        }
        public double DiscreteZ
        {
            set { discreteZ = value; OnPropertyChanged("DiscreteZ"); }
            get { return discreteZ; }
        }
        public GlobalDrawingSettingsBinding()
        {
            DiscreteX = 100.0;
            DiscreteY = 100.0;
            DiscreteZ = 50.0;
        }
        private void OnPropertyChanged(String property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
    public static class GlobalDrawingSettings
    {
        static GlobalDrawingSettingsBinding globalDrawingSettingsBinding = new GlobalDrawingSettingsBinding();
        static PageType selectedPage = PageType.None;
        public static LightSettings LightSettings { set; get; } = new LightSettings();
        public static PageType SelectedPage
        {
            get { return selectedPage; }
            set { selectedPage = value; }
        }
        public static double ScaleZ
        {
            get { return  1.0; }
        }
        public static bool DrawingModel
        {
            get { return SelectedPage == PageType.ViewModel; }
        }
        public static bool TopColorByRelief { get; set; } = false;
        public static bool GradientRelief { get; set; } = false;
        
        public static GlobalDrawingSettingsBinding GlobalDrawingSettingsBinding
        {
            get { return globalDrawingSettingsBinding; }
        }
        public static double DiscreteX
        {
            get { return GlobalDrawingSettingsBinding.DiscreteX; }
        }
        public static double DiscreteY
        {
            get { return GlobalDrawingSettingsBinding.DiscreteY; }
        }
        public static double DiscreteZ
        {
            get { return GlobalDrawingSettingsBinding.DiscreteZ; }
        }
        static public double Discrete(String axis)
        {
            switch (axis)
            {
                case "X": return DiscreteX;
                case "Y": return DiscreteY;
                case "Z": return DiscreteZ;
            }
            return 0.0;
        }
    }
}
