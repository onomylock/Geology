using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Geology.DrawNewWindow.Mesh
{
	public interface IElement
	{
		IEnumerable<int> VertexArr { get; }
		//int NMat { get; }
		//double Sigma { get; set; }
		//double Mu { get; set; }
	}

	public class Element : IElement
	{
		public Element(int[] elem, System.Drawing.Color color)
		{
			VertexArr = elem;
			Color = color;
		}

		//public int NMat { get { ; } set; }
		public IEnumerable<int> VertexArr { get; }
		public System.Drawing.Color Color;
	}

	public class Vertex
	{
		public double R { get; set; }
		public double Z { get; set; }
		public int NumOfFun { get; set; }

		public Vertex(double r, double z, int num)
		{
			R = r;
			Z = z;
			NumOfFun = num;
		}

		public Vertex(double r, double z)
		{
			R = r;
			Z = z;
			NumOfFun = 0;
		}
	}

	public class NewMesh
	{
		public List<GridSettings> grids;
		public List<double> DataGrid;
		public List<int> NormalizeDataGrid;
		private int NumRDown, NumRUp, NumZDown, NumZUp, NumR, NumZ;
		private double Hz, Hr, DiscRDown, DiscRUp, DiscZDown, DiscZUp, R0, R1, Z0, Z1;


		public class GridSettings
		{
			public Vertex RZ;
			public double Q_value;
			//public System.Drawing.Color Color;
			

			public GridSettings (Vertex vertex, double value)
			{
				RZ = vertex;
				Q_value = value;
			}
		}

		public NewMesh()
		{
			System.Drawing.Color color = new System.Drawing.Color();
			grids = new List<GridSettings>();
			DataGrid = new List<double>();
            string path = "C:/Users/onomy/source/repos/MKE_kursovik/MKE_kursovik/bin/Debug/net5.0/outputGrid1.txt";

			foreach (string line in System.IO.File.ReadLines(path))
			{
				var tmp = line.Split();

				Vertex vertex = new Vertex(double.Parse(tmp[0]), double.Parse(tmp[1]));
				grids.Add(new GridSettings(vertex, double.Parse(tmp[2])));
				DataGrid.Add(double.Parse(tmp[2]));
			}

			Normalizaiton();

		}

		private void InputGridPointSource(string path)
		{
			try
			{
				string Path = "C:/Users/onomy/source/repos/MKE_kursovik/MKE_kursovik/bin/Debug/net5.0/Point/GridPoint1.txt";
				using (StreamReader sr = new StreamReader(path))
				{
					var tmp = sr.ReadLine().Split();

					//NumPointSource = int.Parse(tmp[2]);
					//PointSource = new Vertex(double.Parse(tmp[0]), double.Parse(tmp[1]), 1);

					tmp = sr.ReadLine().Split();
					NumRDown = int.Parse(tmp[0]);
					NumRUp = int.Parse(tmp[1]);
					NumZDown = int.Parse(tmp[2]);
					NumZUp = int.Parse(tmp[3]);
					NumR = NumRDown + NumRUp;
					NumZ = NumZDown + NumZUp;

					tmp = sr.ReadLine().Split();
					Hr = double.Parse(tmp[0]);
					//HrUp = double.Parse(tmp[1]);
					//HrUpEnd = double.Parse(tmp[2]);
					Hz = double.Parse(tmp[1]);
					//HzUp = double.Parse(tmp[4]);
					//HzUpEnd = double.Parse(tmp[5]);

					tmp = sr.ReadLine().Split();
					DiscRDown = double.Parse(tmp[0]);
					DiscRUp = double.Parse(tmp[1]);
					DiscZDown = double.Parse(tmp[2]);
					DiscZUp = double.Parse(tmp[3]);

					tmp = sr.ReadLine().Split();
					R0 = double.Parse(tmp[0]);
					R1 = double.Parse(tmp[1]);

					tmp = sr.ReadLine().Split();
					Z0 = double.Parse(tmp[0]);
					Z1 = double.Parse(tmp[1]);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("Grid input exeption");
				Console.WriteLine(e.Message);
			}
		}

		public void Normalizaiton()
		{
			NormalizeDataGrid = new List<int>();
			double Min = DataGrid.Min();
			double Max = DataGrid.Max();

			foreach (double elem in DataGrid)
			{
				NormalizeDataGrid.Add(255 * (int)(elem - Min) / (int)(Max - Min));	
			}
		}


	}
}
