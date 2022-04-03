using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EMaterialParameter
{
    Eps,
    Mu,
    AnisotropyAngle1,
    AnisotropyAngle2,
    AnisotropyAngle3,
    Rotation1,
    Rotation2,
    Rotation3,
    RhoH,
    RhoV,
    RhoX,
    RhoY,
    SigmaH,
    SigmaV,
    SigmaY,
    SigmaX,
    Dep,
    Alpha,
    T0,
    Beta,
    C,
    P,
    G
}


namespace Geology.Objects
{
    public class PolarizationDecayCurvesCollection
    {
        List<PolarizationDecayCurve> polarizationFormulas = new List<PolarizationDecayCurve>();
        public List<PolarizationDecayCurve> PolarizationFormulas
        {
            get { return polarizationFormulas; }
            set { polarizationFormulas = value; }
        }
        public PolarizationDecayCurvesCollection()
        {

        }
        public PolarizationDecayCurvesCollection(PolarizationDecayCurvesCollection c)
        {
            PolarizationFormulas = new List<PolarizationDecayCurve>();
            foreach (var pc in c.PolarizationFormulas)
                PolarizationFormulas.Add(new PolarizationDecayCurve(pc));
        }
    }

    public class PolarizationDecayCurve : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int number = -1;
        private String name = "PCurve";
        private double t0 = 0.1;
        private double beta = 100.0;
        private int dep = 0;

        private bool fitT0 = false;
        private bool fitBeta = false;

        public int Dep
        {
            get { return dep; }
            set {  dep = value; OnPropertyChanged("Dep"); }
        }
        public double T0
        {
            get { return t0; }
            set {  t0 = value; OnPropertyChanged("T0"); }
        }
        public double Beta
        {
            get { return beta; }
            set {  beta = value; OnPropertyChanged("Beta"); }
        }
        public bool FitT0
        {
            get { return fitT0; }
            set {  fitT0 = value; OnPropertyChanged("FitT0"); }
        }
        public bool FitBeta
        {
            get { return fitBeta; }
            set {  fitBeta = value; OnPropertyChanged("FitBeta"); }
        }
        public int Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged("Number"); }
        }
        public String Name
        {
            get { return name; }
            set {  name = value; OnPropertyChanged("Name"); }
        }
        public String NameWithNumber
        {
            get { return number + "_" + name; }
        }

        public PolarizationDecayCurve()
        {

        }
        public PolarizationDecayCurve(PolarizationDecayCurve c)
        {
            CopyFrom(c);
        }
        public object Clone()
        {
            PolarizationDecayCurve f = new PolarizationDecayCurve();
            f.fitT0 = this.FitT0;
            f.fitBeta = this.FitBeta;
            f.dep = this.Dep;
            f.t0 = this.T0;
            f.beta = this.Beta;
            f.number = this.Number;
            f.name = this.Name;

            return f;
        }

        public void CopyFrom(object o)
        {
            var oo = o as PolarizationDecayCurve;
            this.FitT0   = oo.fitT0  ;
            this.FitBeta = oo.fitBeta;
            this.Dep     = oo.dep    ;
            this.T0      = oo.t0     ;
            this.Beta    = oo.beta   ;
            this.Number  = oo.number ;
            this.Name    = oo.name   ;
        }

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
    public class PhysicalMaterial : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double rhoH = 10.0;
        private double rhoV = 10.0;
        private double rhoY = 10.0;
        private int    dep = 0;
        private double alpha = 0.0;
        private double t0 = 0.1;
        private double beta = 100.0;
        private double c = 0.5;
        private double eps = 0.0;
        private double mu = 1.0;
        private double p = 0.0;
        private double g = 0.0;
        private double anisotropyAngle1 = 0.0;
        private double anisotropyAngle2 = 0.0;
        private double anisotropyAngle3 = 0.0;
        private int rotation1 = 1;
        private int rotation2 = 2;
        private int rotation3 = 3;

        public PolarizationDecayCurve PolarizationDecayCurve { get; set; } = null;


        public double G
        {
            get { return g; }
            set {  g = value; OnPropertyChanged("G"); }
        }
        public double P
        {
            get { return p; }
            set {  p = value; OnPropertyChanged("P"); }
        }
        public double Eps
        {
            get { return eps; }
            set {  eps = value; OnPropertyChanged("Eps"); }
        }
        public double Mu
        {
            get { return mu; }
            set { mu = value; OnPropertyChanged("Mu"); }
        }
        public double AnisotropyAngle1
        {
            get { return anisotropyAngle1; }
            set {  anisotropyAngle1 = value; OnPropertyChanged("AnisotropyAngle1"); }
        }
        public double AnisotropyAngle2
        {
            get { return anisotropyAngle2; }
            set {  anisotropyAngle2 = value; OnPropertyChanged("AnisotropyAngle2"); }
        }
        public double AnisotropyAngle3
        {
            get { return anisotropyAngle3; }
            set {  anisotropyAngle3 = value; OnPropertyChanged("AnisotropyAngle3"); }
        }
        public int Rotation1
        {
            get { return rotation1; }
            set {rotation1 = value; OnPropertyChanged("Rotation1"); }
        }
        public int Rotation2
        {
            get { return rotation2; }
            set { rotation2 = value; OnPropertyChanged("Rotation2"); }
        }
        public int Rotation3
        {
            get { return rotation3; }
            set {  rotation3 = value; OnPropertyChanged("Rotation3"); }
        }

        public double RhoH
        {
            get { return rhoH; }
            set {   RhoY = value;  OnPropertyChanged("RhoH"); OnPropertyChanged("SigmaH"); OnPropertyChanged("RhoV"); }
        }
        public double RhoV
        {
            get
            {
                return rhoV;
            }
            set { rhoV = value; OnPropertyChanged("RhoV"); OnPropertyChanged("SigmaV"); }
        }
        public double RhoX
        {
            get { return RhoH; }
            set
            {
                OnPropertyChanged("RhoH");
                OnPropertyChanged("SigmaH");
                OnPropertyChanged("RhoX");
                OnPropertyChanged("SigmaX");
                 OnPropertyChanged("RhoV");
            }
        }
        public double RhoY
        {
            get
            {
                return rhoY;
            }
            set { rhoY = value; OnPropertyChanged("RhoY"); OnPropertyChanged("SigmaY"); }
        }
        public double SigmaH
        {
            get
            {
                if (Math.Abs(rhoH) > 1e-13)
                    return 1.0 / rhoH;
                else
                    return 1e+13;
            }
            set
            {
                if (Math.Abs(value) > 1e-13)
                    rhoH = 1.0 / value;
                else
                    rhoH = 1e+13;
                OnPropertyChanged("RhoH");
                OnPropertyChanged("SigmaH");
            }
        }
        public double SigmaV
        {
            get
            {
                if (Math.Abs(rhoV) > 1e-13)
                    return 1.0 / rhoV;
                else
                    return 1e+13;
            }
            set
            {
                if (Math.Abs(value) > 1e-13)
                    rhoV = 1.0 / value;
                else
                    rhoV = 1e+13;
                OnPropertyChanged("RhoV");
                OnPropertyChanged("SigmaV");
            }
        }
        public double SigmaY
        {
            get
            {
                if (Math.Abs(rhoY) > 1e-13)
                    return 1.0 / rhoY;
                else
                    return 1e+13;
            }
            set
            {
                if (Math.Abs(value) > 1e-13)
                    rhoY = 1.0 / value;
                else
                    rhoY = 1e+13;
                OnPropertyChanged("RhoY");
                OnPropertyChanged("SigmaY");
            }
        }
        public double SigmaX
        {
            get
            {
                if (Math.Abs(rhoH) > 1e-13)
                    return 1.0 / rhoH;
                else
                    return 1e+13;
            }
            set
            {
                if (Math.Abs(value) > 1e-13)
                    rhoH = 1.0 / value;
                else
                    rhoH = 1e+13;
                OnPropertyChanged("RhoH");
                OnPropertyChanged("SigmaH");
                OnPropertyChanged("RhoX");
                OnPropertyChanged("SigmaX");
            }
        }
        public int Dep
        {
            get { return dep; }
            set {  dep = value; OnPropertyChanged("Dep"); }
        }
        public double Alpha
        {
            get { return alpha; }
            set {  alpha = value; OnPropertyChanged("Alpha"); }
        }
        public double T0
        {
            get { return t0; }
            set { t0 = value; OnPropertyChanged("T0"); }
        }
        public double Beta
        {
            get { return beta; }
            set { beta = value; OnPropertyChanged("Beta"); }
        }
        public double C
        {
            get { return c; }
            set {  c = value; OnPropertyChanged("C"); }
        }

        public PhysicalMaterial()
        {

        }
        public PhysicalMaterial(PhysicalMaterial material)
        {
            CopyFrom(material);
        }

        public object Clone()
        {
            PhysicalMaterial material = new PhysicalMaterial(this);
            return material;
        }
        public void CopyFrom(PhysicalMaterial material)
        {
            g = material.g;
            p = material.p;
            rhoH = material.rhoH;
            rhoV = material.rhoV;
            rhoY = material.rhoY;
            dep = material.dep;
            alpha = material.alpha;
            t0 = material.t0;
            beta = material.beta;
            c = material.c;
            eps = material.eps;
            mu = material.mu;
            anisotropyAngle1 = material.anisotropyAngle1;
            anisotropyAngle2 = material.anisotropyAngle2;
            anisotropyAngle3 = material.anisotropyAngle3;
            rotation1 = material.rotation1;
            rotation2 = material.rotation2;
            rotation3 = material.rotation3;
        }
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public int CheckIPParameters(out String message)
        {
            try
            {
                //if (Alpha < 0)
                //{
                //    message = "Alpha is less than 0";
                //    return 1;
                //}
                //if (Alpha > 1)
                //{
                //    message = "Alpha is greater than 1";
                //    return 1;
                //}
                if (T0 < 1e-10)
                {
                    message = "T0 is less than 1e-10";
                    return 1;
                }

                message = "";
                return 0;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return 1;
            }
        }
        private object GetParameter(EMaterialParameter parameter)
        {
            switch (parameter)
            {
                case EMaterialParameter.Eps: return Eps;
                case EMaterialParameter.Mu: return Mu;
                case EMaterialParameter.AnisotropyAngle1: return AnisotropyAngle1;
                case EMaterialParameter.AnisotropyAngle2: return AnisotropyAngle2;
                case EMaterialParameter.AnisotropyAngle3: return AnisotropyAngle3;
                case EMaterialParameter.Rotation1: return Rotation1;
                case EMaterialParameter.Rotation2: return Rotation2;
                case EMaterialParameter.Rotation3: return Rotation3;
                case EMaterialParameter.RhoH: return RhoH;
                case EMaterialParameter.RhoV: return RhoV;
                case EMaterialParameter.RhoX: return RhoX;
                case EMaterialParameter.RhoY: return RhoY;
                case EMaterialParameter.SigmaH: return SigmaH;
                case EMaterialParameter.SigmaV: return SigmaV;
                case EMaterialParameter.SigmaY: return SigmaY;
                case EMaterialParameter.SigmaX: return SigmaX;
                case EMaterialParameter.Dep: return Dep;
                case EMaterialParameter.Alpha: return Alpha;
                case EMaterialParameter.T0: return T0;
                case EMaterialParameter.Beta: return Beta;
                case EMaterialParameter.C: return C;
                case EMaterialParameter.P: return P;
                case EMaterialParameter.G: return g;
            }

            return null;
        }
        private void SetParameter(EMaterialParameter parameter, object value)
        {
            switch (parameter)
            {
                case EMaterialParameter.Eps: Eps = (double)value; break;
                case EMaterialParameter.Mu: Mu = (double)value; break;
                case EMaterialParameter.AnisotropyAngle1: AnisotropyAngle1 = (double)value; break;
                case EMaterialParameter.AnisotropyAngle2: AnisotropyAngle2 = (double)value; break;
                case EMaterialParameter.AnisotropyAngle3: AnisotropyAngle3 = (double)value; break;
                case EMaterialParameter.Rotation1: Rotation1 = (int)value; break;
                case EMaterialParameter.Rotation2: Rotation2 = (int)value; break;
                case EMaterialParameter.Rotation3: Rotation3 = (int)value; break;
                case EMaterialParameter.RhoH: RhoH = (double)value; break;
                case EMaterialParameter.RhoV: RhoV = (double)value; break;
                case EMaterialParameter.RhoX: RhoX = (double)value; break;
                case EMaterialParameter.RhoY: RhoY = (double)value; break;
                case EMaterialParameter.SigmaH: SigmaH = (double)value; break;
                case EMaterialParameter.SigmaV: SigmaV = (double)value; break;
                case EMaterialParameter.SigmaY: SigmaY = (double)value; break;
                case EMaterialParameter.SigmaX: SigmaX = (double)value; break;
                case EMaterialParameter.Dep: Dep = (int)value; break;
                case EMaterialParameter.Alpha: Alpha = (double)value; break;
                case EMaterialParameter.T0: T0 = (double)value; break;
                case EMaterialParameter.Beta: Beta = (double)value; break;
                case EMaterialParameter.C: C = (double)value; break;
                case EMaterialParameter.P: P = (double)value; break;
                case EMaterialParameter.G: g = (double)value; break;
            }
        }
        public object GetParameter(object parameter)
        {
            List<object> r = new List<object>();
            if (parameter.GetType().IsArray)
            {
                var arr = (parameter as Array);
                foreach (var pp in arr)
                {
                    var v = GetParameter((EMaterialParameter)pp);
                    if (v.GetType().IsArray)
                        foreach (var vv in v as Array)
                            r.Add(vv);
                    else
                        r.Add(v);
                }
            }
            else
            {
                var v = GetParameter((EMaterialParameter)parameter);
                if (v.GetType().IsArray)
                    foreach (var vv in v as Array)
                        r.Add(vv);
                else
                    r.Add(v);
            }

            return r.ToArray();
        }

        public void Add(PhysicalMaterial material, double factor)
        {
            var types = new List<EMaterialParameter>() {
                EMaterialParameter.Alpha,
                EMaterialParameter.AnisotropyAngle1,
                EMaterialParameter.AnisotropyAngle2,
                EMaterialParameter.AnisotropyAngle3,
                EMaterialParameter.Beta,
                EMaterialParameter.C,
                EMaterialParameter.Eps,
                EMaterialParameter.G,
                EMaterialParameter.Mu,
                EMaterialParameter.P,
                EMaterialParameter.SigmaH,
                EMaterialParameter.SigmaV,
                EMaterialParameter.SigmaY,
                EMaterialParameter.T0};
            foreach (var valueType in types)
            {
                SetParameter((EMaterialParameter)valueType, (double)GetParameter(valueType) + factor * ((double)material.GetParameter(valueType)));
            }
        }
        public void Mult(double factor)
        {
            var types = new List<EMaterialParameter>() {
                EMaterialParameter.Alpha,
                EMaterialParameter.AnisotropyAngle1,
                EMaterialParameter.AnisotropyAngle2,
                EMaterialParameter.AnisotropyAngle3,
                EMaterialParameter.Beta,
                EMaterialParameter.C,
                EMaterialParameter.Eps,
                EMaterialParameter.G,
                EMaterialParameter.Mu,
                EMaterialParameter.P,
                EMaterialParameter.SigmaH,
                EMaterialParameter.SigmaV,
                EMaterialParameter.SigmaY,
                EMaterialParameter.T0};

            foreach (var valueType in types)
            {
                SetParameter((EMaterialParameter)valueType, (double)GetParameter(valueType) * factor);
            }
        }

    }
}
