using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using GLContex = Geology.OpenGL.OpenGL;

namespace Geology.Objects.SaturationModel
{
    public enum SelectionRegime
    {
        None,
        SelectSingle,
        SelectMulti
    }
    public class ContourPoint
    {
        public Utilities.Vector3 point;
        public bool selected;
        public double tmpX, tmpY;

        public ContourPoint()
        {
            point = new Utilities.Vector3();
            selected = false;
            tmpX = tmpY = 0.0;
        }
        public ContourPoint(double x, double y, double z)
        {
            point = new Utilities.Vector3(x, y, x);
            selected = false;
            tmpX = tmpY = 0.0;
        }
        public ContourPoint(ContourPoint contourPoint)
        {
            point = new Utilities.Vector3(contourPoint.point);
            selected = false;
            tmpX = tmpY = 0.0;
        }

        //================================================================================================================ Read/Write
        public int Write(ref StreamWriter outputFile)
        {
            try
            {
                if (point.Write(ref outputFile) != 0) return 1;
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
                int n;
                String buffer;

                if (fileVersion >= 1.0)
                {
                    if (point.Read(ref inputFile, fileVersion) != 0) return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public int WriteB(BinaryWriter outputFile)
        {
            try
            {
                if (point.WriteB(outputFile) != 0) return 1;
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
        public int ReadB(BinaryReader inputFile, double fileVersion)
        {
            try
            {
                int n;

                if (fileVersion >= 1.0)
                {
                    if (point.ReadB(inputFile, fileVersion) != 0) return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
    public class SaturationVolume : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ContourPoint> latheralPoints;
        private ObservableCollection<ContourPoint> latheralPointsTop = new ObservableCollection<ContourPoint>();
        private Objects.PhysicalMaterial material = new PhysicalMaterial();
        int highlightedPoint;
        private String name;
        private int number;
        private Color color;
        private bool selectionStarted = false;
        private bool addingPointStarted = false;
        private bool splittingStarted = false;
        private double selectX0 = 0.0;
        private double selectX1 = 0.0;
        private double selectY0 = 0.0;
        private double selectY1 = 0.0;
        private bool addingPointAvailable = false;
        private int addingPointSide = 0;
        private double addingPointX = 0.0;
        private double addingPointY = 0.0;
        private double addingPointSplit1X = 0.0;
        private double addingPointSplit1Y = 0.0;
        private double addingPointSplit2X = 0.0;
        private double addingPointSplit2Y = 0.0;
        private int addingPointSideSplit1 = 0;
        private int addingPointSideSplit2 = 0;
        private int splitPointsAdded = 0;
        private bool isSelected;
        private int level = 0;
        private bool hasTwoBases = false;
        private bool editTop = false;
        private String magniteMaterial = "";
        private String gravityMaterial = "";
        public SaturationVolumeStack stack;
        public int InversionMaterial { get; set; } = 0;
        public String MagniteMaterial
        {
            get { return magniteMaterial; }
            set { magniteMaterial = value; OnPropertyChanged("MagniteMaterial"); }
        }
        public String GravityMaterial
        {
            get { return gravityMaterial; }
            set { gravityMaterial = value; OnPropertyChanged("GravityMaterial"); }
        }
        ObservableCollection<FitGroup> fitGroups = new ObservableCollection<FitGroup>();
        private List<int> appliedParameters = new List<int>();

        public ObservableCollection<ContourPoint> LatheralPoints
        {
            get { return hasTwoBases && EditTop ? latheralPointsTop : latheralPoints; }
        }
        public ObservableCollection<ContourPoint> LatheralPointsOtherBase
        {
            get { return hasTwoBases && EditTop ? latheralPoints : latheralPointsTop; }
        }
        public ObservableCollection<ContourPoint> LatheralPointsBottom
        {
            get { return latheralPoints; }
            set { latheralPoints = value; OnPropertyChanged("LatheralPointsBottom"); OnPropertyChanged("LatheralPoints"); }
        }
        public ObservableCollection<ContourPoint> LatheralPointsTop
        {
            get { return latheralPointsTop; }
            set { latheralPointsTop = value; OnPropertyChanged("LatheralPointsTop"); OnPropertyChanged("LatheralPoints"); }
        }
        public bool HasTwoBases
        {
            get { return hasTwoBases; }
            set
            {
                hasTwoBases = value;
                LatheralPointsTop.Clear();
                if (value)
                    foreach (var p in LatheralPointsBottom)
                        LatheralPointsTop.Add(new ContourPoint(p));
                OnPropertyChanged("HasTwoBases");
            }
        }
        public bool EditTop
        {
            get { return editTop; }
            set {  editTop = value; OnPropertyChanged("EditTop"); }
        }

        public ObservableCollection<FitGroup> FitGroups
        {
            get { return fitGroups; }
        }
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public int Number
        {
            get { return number; }
            set {  number = value; OnPropertyChanged("Number"); }
        }
        public PhysicalMaterial Material
        {
            get { return material; }
            set {  material = value; OnPropertyChanged("Material"); }
        }
        public Color Color
        {
            get { return color; }
            set {  color = value; OnPropertyChanged("Color"); }
        }
        public int Level
        {
            get { return level; }
            set {  level = value; OnPropertyChanged("Level"); }
        }

        public int ClosestPoint(double x, double y)
        {
            int cp = -1;
            double d, dmin = double.MaxValue;

            for (int i = 0; i < LatheralPoints.Count; i++)
            {
                d = Utilities.LittleTools.CalculateDistance2D(x, y, LatheralPoints[i].point.X, LatheralPoints[i].point.Y);
                if (dmin > d)
                {
                    dmin = d;
                    cp = i;
                }
            }

            return cp;
        }

        public SaturationVolume()
        {
            latheralPoints = new ObservableCollection<ContourPoint>();
            for (int i = 0; i < 4; i++)
                latheralPoints.Add(new ContourPoint());

            latheralPoints[0].point.X = 000.0; latheralPoints[0].point.Y = 000.0;
            latheralPoints[1].point.X = 100.0; latheralPoints[1].point.Y = 000.0;
            latheralPoints[2].point.X = 100.0; latheralPoints[2].point.Y = 100.0;
            latheralPoints[3].point.X = 000.0; latheralPoints[3].point.Y = 100.0;

            isSelected = false;

            name = "New contour";

            number = 0;
            highlightedPoint = -1;
            color = Utilities.LittleTools.GetRandomColor();

        }

        //================================================================= Edit functions


        //================================================================= View functions

        public int Previouspoint(int i)
        {
            if (i <= 0)
                return LatheralPoints.Count - 1;

            return i - 1;
        }


    }
    public class SaturationVolumeStack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double zeta0, zeta1;
        private String name;
        private int layerBottom;
        private int layerTop;
        private int number;
        public double hx, hy;
        private bool editTop = false;
        private List<int> appliedParameters = new List<int>();
        public ObservableCollection<SaturationVolume> volumes;

        public bool FitZ0 { get; set; } = false;
        public bool FitZ1 { get; set; } = false;
        public bool FitZSolid { get; set; } = false;
        public bool FitZSize { get; set; } = false;

        public bool EditTop
        {
            get { return editTop; }
            set
            {
                editTop = value;
                foreach (var v in volumes)
                    v.EditTop = value;
                OnPropertyChanged("EditTop");
            }
        }

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public SaturationVolumeStack()
        {
            zeta0 = -100.0;
            zeta1 = 0.0;
            name = "New stack";
            layerBottom = 0;
            layerTop = 0;
            number = 0;
            volumes = new ObservableCollection<SaturationVolume>();
            volumes.CollectionChanged += this.OnVolumesChanged;
            hx = 100.0;
            hy = 100.0;
        }

        void OnVolumesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RenumberVolumes();
        }
        private void RenumberVolumes()
        {
            for (int i = 0; i < volumes.Count; i++)
                volumes[i].Number = i + 1;
        }

        public int LayerTop
        {
            get { return layerTop; }
            set { layerTop = value; OnPropertyChanged("LayerTop"); }
        }
        public int LayerBottom
        {
            get { return layerBottom; }
            set { layerBottom = value; OnPropertyChanged("LayerBottom"); }
        }
        public double Zeta0
        {
            get { return zeta0; }
            set { zeta0 = value; OnPropertyChanged("Zeta0"); }
        }
        public double Zeta1
        {
            get { return zeta1; }
            set { zeta1 = value; OnPropertyChanged("Zeta1"); }
        }


        public void SplitContour(SaturationVolume contour, double addingPointSplit1X, double addingPointSplit1Y, double addingPointSplit2X, double addingPointSplit2Y, int addingPointSideSplit1, int addingPointSideSplit2)
        {
            ContourPoint newPoint, newPointTop;
            int contourNumber, i1, i2;
            double d1, d2;
            double addingPointSplit1OtherX = 0.0;
            double addingPointSplit1OtherY = 0.0;
            double addingPointSplit2OtherX = 0.0;
            double addingPointSplit2OtherY = 0.0;

            contourNumber = volumes.IndexOf(contour);

            i1 = contour.ClosestPoint(addingPointSplit1X, addingPointSplit1Y);
            i2 = contour.ClosestPoint(addingPointSplit2X, addingPointSplit2Y);
            if (addingPointSideSplit1 > 0)
                i1++;
            if (addingPointSideSplit2 > 0)
                i2++;
            if (i1 == i2)
                return;

            if (contour.HasTwoBases)
            {
                var p1 = contour.LatheralPoints[contour.Previouspoint(i1)];
                var p2 = contour.LatheralPoints[i1 % contour.LatheralPoints.Count];
                d1 = Math.Sqrt((addingPointSplit1X - p1.point.X) * (addingPointSplit1X - p1.point.X) + (addingPointSplit1Y - p1.point.Y) * (addingPointSplit1Y - p1.point.Y)) /
                     Math.Sqrt((p2.point.X - p1.point.X) * (p2.point.X - p1.point.X) + (p2.point.Y - p1.point.Y) * (p2.point.Y - p1.point.Y));

                p1 = contour.LatheralPoints[contour.Previouspoint(i2)];
                p2 = contour.LatheralPoints[i2 % contour.LatheralPoints.Count];
                d2 = Math.Sqrt((addingPointSplit2X - p1.point.X) * (addingPointSplit2X - p1.point.X) + (addingPointSplit2Y - p1.point.Y) * (addingPointSplit2Y - p1.point.Y)) /
                     Math.Sqrt((p2.point.X - p1.point.X) * (p2.point.X - p1.point.X) + (p2.point.Y - p1.point.Y) * (p2.point.Y - p1.point.Y));

                p1 = contour.LatheralPointsOtherBase[contour.Previouspoint(i1)];
                p2 = contour.LatheralPointsOtherBase[i1 % contour.LatheralPoints.Count];
                addingPointSplit1OtherX = p1.point.X * (1.0 - d1) + p2.point.X * d1;
                addingPointSplit1OtherY = p1.point.Y * (1.0 - d1) + p2.point.Y * d1;

                p1 = contour.LatheralPointsOtherBase[contour.Previouspoint(i2)];
                p2 = contour.LatheralPointsOtherBase[i2 % contour.LatheralPoints.Count];
                addingPointSplit2OtherX = p1.point.X * (1.0 - d2) + p2.point.X * d2;
                addingPointSplit2OtherY = p1.point.Y * (1.0 - d2) + p2.point.Y * d2;
            }

            if (i1 > i2) i1++;
            else i2++;

            List<ContourPoint> points = new List<ContourPoint>();
            List<ContourPoint> pointsOther = new List<ContourPoint>();

            foreach (var pnt in contour.LatheralPoints)
                points.Add(new ContourPoint(pnt));

            if (contour.HasTwoBases)
            {
                foreach (var pnt in contour.LatheralPointsOtherBase)
                    pointsOther.Add(new ContourPoint(pnt));
            }

            if (i1 < i2)
            {
                newPoint = new ContourPoint();
                newPoint.point.X = addingPointSplit1X;
                newPoint.point.Y = addingPointSplit1Y;
                points.Insert(i1, newPoint);

                newPoint = new ContourPoint();
                newPoint.point.X = addingPointSplit2X;
                newPoint.point.Y = addingPointSplit2Y;
                points.Insert(i2, newPoint);

                if (contour.HasTwoBases)
                {
                    newPoint = new ContourPoint();
                    newPoint.point.X = addingPointSplit1OtherX;
                    newPoint.point.Y = addingPointSplit1OtherY;
                    pointsOther.Insert(i1, newPoint);

                    newPoint = new ContourPoint();
                    newPoint.point.X = addingPointSplit2OtherX;
                    newPoint.point.Y = addingPointSplit2OtherY;
                    pointsOther.Insert(i2, newPoint);
                }
            }
            else
            {
                newPoint = new ContourPoint();
                newPoint.point.X = addingPointSplit2X;
                newPoint.point.Y = addingPointSplit2Y;
                points.Insert(i2, newPoint);

                newPoint = new ContourPoint();
                newPoint.point.X = addingPointSplit1X;
                newPoint.point.Y = addingPointSplit1Y;
                points.Insert(i1, newPoint);

                if (contour.HasTwoBases)
                {
                    newPoint = new ContourPoint();
                    newPoint.point.X = addingPointSplit2OtherX;
                    newPoint.point.Y = addingPointSplit2OtherY;
                    pointsOther.Insert(i2, newPoint);

                    newPoint = new ContourPoint();
                    newPoint.point.X = addingPointSplit1OtherX;
                    newPoint.point.Y = addingPointSplit1OtherY;
                    pointsOther.Insert(i1, newPoint);
                }
            }

            contour.LatheralPoints.Clear();
            contour.LatheralPointsOtherBase.Clear();
            SaturationVolume newContour = new SaturationVolume();
            newContour.LatheralPoints.Clear();
            newContour.Material = (PhysicalMaterial)contour.Material.Clone();
            newContour.HasTwoBases = contour.HasTwoBases;
            newContour.EditTop = contour.EditTop;

            for (int i = 0; i <= Math.Min(i1, i2); i++)
            {
                newPoint = new ContourPoint();
                newPoint.point.X = points[i].point.X;
                newPoint.point.Y = points[i].point.Y;
                contour.LatheralPoints.Add(newPoint);

                if (contour.HasTwoBases)
                {
                    newPoint = new ContourPoint();
                    newPoint.point.X = pointsOther[i].point.X;
                    newPoint.point.Y = pointsOther[i].point.Y;
                    contour.LatheralPointsOtherBase.Add(newPoint);
                }
            }
            for (int i = Math.Max(i1, i2); i < points.Count; i++)
            {
                newPoint = new ContourPoint();
                newPoint.point.X = points[i].point.X;
                newPoint.point.Y = points[i].point.Y;
                contour.LatheralPoints.Add(newPoint);

                if (contour.HasTwoBases)
                {
                    newPoint = new ContourPoint();
                    newPoint.point.X = pointsOther[i].point.X;
                    newPoint.point.Y = pointsOther[i].point.Y;
                    contour.LatheralPointsOtherBase.Add(newPoint);

                }
            }
            for (int i = Math.Min(i1, i2); i <= Math.Max(i1, i2); i++)
            {
                newPoint = new ContourPoint();
                newPoint.point.X = points[i].point.X;
                newPoint.point.Y = points[i].point.Y;
                newContour.LatheralPoints.Add(newPoint);

                if (contour.HasTwoBases)
                {
                    newPoint = new ContourPoint();
                    newPoint.point.X = pointsOther[i].point.X;
                    newPoint.point.Y = pointsOther[i].point.Y;
                    newContour.LatheralPointsOtherBase.Add(newPoint);
                }
            }

            volumes.Insert(contourNumber + 1, newContour);
            RenumberVolumes();
        }


    }

    public class FitGroup : INotifyPropertyChanged
    {
        public enum FitGroupType
        {
            AllApart,
            SameIncrement,
            SameDirection,
            EdgesByNormal
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<int> nodes;
        private int number;
        private FitGroupType type;

        public FitGroup()
        {
            nodes = new ObservableCollection<int>();
            number = -1;
            type = FitGroupType.EdgesByNormal;

            nodes.CollectionChanged += Nodes_CollectionChanged;
        }
        public FitGroup(FitGroup fitGroup)
        {
            nodes = new ObservableCollection<int>(fitGroup.nodes);
            number = fitGroup.Number;
            type = fitGroup.Type;

            nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Nodes");
        }

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public String Nodes
        {
            get { if (nodes.Count < 1) return ""; String buf = ""; for (int i = 0; i < nodes.Count - 1; i++) buf += nodes[i].ToString() + ", "; buf += nodes[nodes.Count - 1].ToString(); return buf; }
        }
        public FitGroupType Type
        {
            get { return type; }
            set { type = value; OnPropertyChanged("Type"); }
        }
        public int TypeInt
        {
            get
            {
                switch (type)
                {
                    case FitGroupType.AllApart: return 0;
                    case FitGroupType.SameIncrement: return 1;
                    case FitGroupType.SameDirection: return 2;
                    case FitGroupType.EdgesByNormal: return 3;
                }
                return -1;
            }
        }
        public int Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged("Number"); }
        }

        public int ParametersCount(int contourSize)
        {
            switch (type)
            {
                case FitGroupType.AllApart:
                    return nodes.Count;
                case FitGroupType.SameIncrement:
                    return 1;
                case FitGroupType.SameDirection:
                    return nodes.Count;
                case FitGroupType.EdgesByNormal:
                    return nodes.Count == contourSize ? nodes.Count : nodes.Count - 1;
            }
            return 0;
        }
        public void InsertNode(int nodeNumber)
        {
            if (nodes.Count == 0)
            {
                nodes.Add(nodeNumber);
                return;
            }

            int insertIndex;

            if (nodeNumber < nodes.First())
                insertIndex = 0;
            else
                if (nodeNumber > nodes.Last())
                insertIndex = nodes.Count;
            else
                for (insertIndex = 0; insertIndex < nodes.Count && nodes[insertIndex] < nodeNumber; insertIndex++) ;

            nodes.Insert(insertIndex, nodeNumber);
        }

        //================================================================================================================ Read/Write
    }
}
