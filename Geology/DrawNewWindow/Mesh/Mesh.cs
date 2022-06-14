using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Geology.DrawNewWindow.Mesh
{
	//public interface IElement
	//{
	//	IEnumerable<int> VertexArr { get; }
	//	//int NMat { get; }
	//	//double Sigma { get; set; }
	//	//double Mu { get; set; }
	//}

	//public class Element : IElement
	//{
	//	public Element(int[] elem)
	//	{
	//		VertexArr = elem;
	//	}

	//	//public int NMat { get { ; } set; }
	//	public IEnumerable<int> VertexArr { get; }
	//}

	//public class Vertex
	//{
	//	public double R { get; set; }
	//	public double Z { get; set; }
	//	public int NumOfFun { get; set; }

	//	public Vertex(double r, double z, int num)
	//	{
	//		R = r;
	//		Z = z;
	//		NumOfFun = num;
	//	}

	//	public Vertex(double r, double z)
	//	{
	//		R = r;
	//		Z = z;
	//		NumOfFun = 0;
	//	}
	//}


	//class Mesh
	//{
	//	private Vertex PointSource;
	//	private int NumRDown, NumRUp, NumZDown, NumZUp, NumR, NumZ;
	//	private double HzUp, HrUp,, HzDown DiscRDown, DiscRUp, DiscZDown, DiscZUp, R0, R1, Z0, Z1;
	//	public List<Vertex> RZ { get; set; }
	//	public List<Element> Elements { get; set; }
	//	//private int[] NumOfParameters;

	//	public Mesh()
	//	{
	//		InputGridPointSource("C:/Users/onomy/source/repos/MKE_kursovik/MKE_kursovik/bin/Debug/net5.0/Point/GridPoint1.txt");
	//		GetGridPointSource();
	//		GetElements();
	//	}

	//	//private void BuildPotrait()
	//	//{
	//	//    var map = new SortedSet<int>[RZ.Count];
	//	//    for (int i = 0; i < map.Length; i++)
	//	//    {
	//	//        map[i] = new SortedSet<int>();
	//	//    }

	//	//    foreach (var element in Elements)
	//	//    {
	//	//        foreach (var i in element.VertexArr)
	//	//        {
	//	//            foreach (var j in element.VertexArr)
	//	//                if (i > j) map[i].Add(j);
	//	//        }
	//	//    }
	//	//}

	//	private void GetElements()
	//	{
	//		//NumOfParameters = new int[(NumZ - 1) * (NumR - 1)];
	//		Elements = new List<Element>();
	//		Element elementTmp;


	//		for (int i = 0; i < NumZ - 1; i++)
	//		{
	//			for (int j = i * NumR; j < i * NumR + NumR - 1; j++)
	//			{
	//				int[] arrTmp = new int[4];
	//				arrTmp[0] = j;
	//				arrTmp[1] = j + 1;
	//				arrTmp[2] = j + NumR;
	//				arrTmp[3] = j + NumR + 1;
	//				elementTmp = new Element(arrTmp);
	//				Elements.Add(elementTmp);
	//			}
	//		}
	//		//for (int i = 0; i < NumOfParameters.Count(); i++)
	//		//{
	//		//    Elements[i].NMat = NumOfParameters[i];
	//		//}

	//	}

	//	private void InputGridPointSource(string path)
	//	{
	//		try
	//		{
	//			string Path = "C:/Users/onomy/source/repos/MKE_kursovik/MKE_kursovik/bin/Debug/net5.0/Point/GridPoint1.txt";
	//			using (StreamReader sr = new StreamReader(path))
	//			{
	//				var tmp = sr.ReadLine().Split();

	//				//NumPointSource = int.Parse(tmp[2]);
	//				PointSource = new Vertex(double.Parse(tmp[0]), double.Parse(tmp[1]), 1);

	//				tmp = sr.ReadLine().Split();
	//				NumRDown = int.Parse(tmp[0]);
	//				NumRUp = int.Parse(tmp[1]);
	//				NumZDown = int.Parse(tmp[2]);
	//				NumZUp = int.Parse(tmp[3]);
	//				NumR = NumRDown + NumRUp;
	//				NumZ = NumZDown + NumZUp;

	//				tmp = sr.ReadLine().Split();
	//				Hr = double.Parse(tmp[0]);
	//				//HrUp = double.Parse(tmp[1]);
	//				//HrUpEnd = double.Parse(tmp[2]);
	//				Hz = double.Parse(tmp[1]);
	//				//HzUp = double.Parse(tmp[4]);
	//				//HzUpEnd = double.Parse(tmp[5]);

	//				tmp = sr.ReadLine().Split();
	//				DiscRDown = double.Parse(tmp[0]);
	//				DiscRUp = double.Parse(tmp[1]);
	//				DiscZDown = double.Parse(tmp[2]);
	//				DiscZUp = double.Parse(tmp[3]);

	//				tmp = sr.ReadLine().Split();
	//				R0 = double.Parse(tmp[0]);
	//				R1 = double.Parse(tmp[1]);

	//				tmp = sr.ReadLine().Split();
	//				Z0 = double.Parse(tmp[0]);
	//				Z1 = double.Parse(tmp[1]);
	//			}
	//		}
	//		catch (IOException e)
	//		{
	//			Console.WriteLine("Grid input exeption");
	//			Console.WriteLine(e.Message);
	//		}
	//	}

	//	private void GetGridPointSource()
	//	{
	//		double HrTmpDown = HrDown;
	//		double HrTmpUp = HrUp;
	//		double HzTmp = HzDown;
	//		double Ri, Zi;
	//		int NumPointSource;
	//		RZ = new List<Vertex>();
	//		RZ.Add(new Vertex(R0, Z0));
	//		try
	//		{
	//			for (int i = 0; i < NumZDown; i++)
	//			{
	//				if (i == 0) Zi = Z0;
	//				else if (i == NumZDown - 1) Zi = PointSource.Z;
	//				else Zi = RZ[RZ.Count - 1].Z + HzTmp;
	//				HrTmpUp = HrUp;
	//				HrTmpDown = HrDown;
	//				for (int j = 0; j < NumRDown; j++)
	//				{
	//					if (j == 0 && i == 0) continue;
	//					else if (j == 0)
	//					{
	//						RZ.Add(new Vertex(R0, Zi));
	//						continue;
	//					}
	//					else if (j == NumRDown - 1) Ri = PointSource.R;
	//					else Ri = RZ[RZ.Count - 1].R + HrTmpDown;

	//					RZ.Add(new Vertex(Ri, Zi));
	//					HrTmpDown *= DiscRDown;
	//				}

	//				for (int j = 0; j < NumRUp; j++)
	//				{
	//					if (j == 0 && i == 0) continue;
	//					else if (j == 0)
	//					{
	//						//RZ.Add(new Vertex(PointSource.R, Zi));
	//						continue;
	//					}
	//					else if (j == NumRUp - 1) Ri = R1;
	//					else Ri = RZ[RZ.Count - 1].R + HrTmpUp;

	//					RZ.Add(new Vertex(Ri, Zi));
	//					HrTmpUp *= DiscRUp;
	//				}
	//				if (i == 0) continue;
	//				HzTmp *= DiscZDown;
	//			}

	//			HzTmp = HzUp;
	//			for (int i = 0; i < NumZUp; i++)
	//			{
	//				if (i == 0) continue;
	//				//if (i == 0) Zi = PointSource.Z;
	//				//if (i == 0)
	//				//{
	//				//    HzTmp /= DiscZ;
	//				//    continue;
	//				//}
	//				if (i == NumZUp - 1) Zi = Z1;
	//				else Zi = RZ[RZ.Count - 1].Z + HzTmp;
	//				HrTmpUp = HrUp;
	//				HrTmpDown = HrDown;
	//				for (int j = 0; j < NumRDown; j++)
	//				{
	//					//if (j == 0 && i == 0) continue;
	//					if (j == 0)
	//					{
	//						RZ.Add(new Vertex(R0, Zi));
	//						continue;
	//					}
	//					else if (j == NumRDown - 1) Ri = PointSource.R;
	//					else Ri = RZ[RZ.Count - 1].R + HrTmpDown;

	//					RZ.Add(new Vertex(Ri, Zi));
	//					HrTmpDown *= DiscRDown;
	//				}

	//				for (int j = 0; j < NumRUp; j++)
	//				{
	//					//if (j == 0 && i == 0) continue;
	//					if (j == 0) continue;
	//					else if (j == NumRUp - 1) Ri = R1;
	//					else Ri = RZ[RZ.Count - 1].R + HrTmpUp;

	//					RZ.Add(new Vertex(Ri, Zi));
	//					HrTmpUp *= DiscRUp;
	//				}
	//				HzTmp *= DiscZUp;
	//			}
	//			NumPointSource = NumR * (NumZDown - 1) - 1 + NumRDown;
	//			RZ[NumPointSource].NumOfFun = 1;
	//		}
	//		catch (Exception)
	//		{

	//			throw;
	//		}
	//	}
	//}
}
