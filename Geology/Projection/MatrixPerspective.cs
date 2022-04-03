using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geology.Projection
{
    public class MatrixPerspective
    {
        public double[,] mat;
	public void SaveMat(System.IO.StreamWriter file)
	{
		for (int i = 0; i < 3; i++)
		{
             file.WriteLine(String.Format("{0:E14} {1:E14} {2:E14}", mat[i,0], mat[i,1], mat[i,2]));
		}
	}
	public void LoadMat(System.IO.StreamReader file)
	{
		for (int i = 0; i < 3; i++)
		{
			    var mas=file.ReadLine().Split(' ').Select(x =>Double.Parse(x)).ToList();//,(String.Format("{0:E14} {1:E14} {2:E14}", mat[i][0], mat[i][1], mat[i][2]));
		        mat[i,0]=mas[0];
                mat[i,1] = mas[1];
                mat[i,2] = mas[2];
        }
	}
	
    public void ClearMatrix()
    {
        mat[0,0]=mat[1,1]=mat[2,2]=1;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < i; j++)
                mat[i, j] = mat[j, i] = 0;
    }
	public void Mult(double x1, double y1, double z1,
		double x2, double y2, double z2,
		double x3, double y3, double z3)
	{
		double[,] promMatrix=new double[3,3];
		promMatrix[0,0] = mat[0,0] * x1 + mat[0,1] * x2 + mat[0,2] * x3;
		promMatrix[0,1] = mat[0,0] * y1 + mat[0,1] * y2 + mat[0,2] * y3;
		promMatrix[0,2] = mat[0,0] * z1 + mat[0,1] * z2 + mat[0,2] * z3;


		promMatrix[1,0] = mat[1,0] * x1 + mat[1,1] * x2 + mat[1,2] * x3;
		promMatrix[1,1] = mat[1,0] * y1 + mat[1,1] * y2 + mat[1,2] * y3;
		promMatrix[1,2] = mat[1,0] * z1 + mat[1,1] * z2 + mat[1,2] * z3;

		promMatrix[2,0] = mat[2,0] * x1 + mat[2,1] * x2 + mat[2,2] * x3;
		promMatrix[2,1] = mat[2,0] * y1 + mat[2,1] * y2 + mat[2,2] * y3;
		promMatrix[2,2] = mat[2,0] * z1 + mat[2,1] * z2 + mat[2,2] * z3;
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				mat[i,j] = promMatrix[i,j];
	}
	public void GetMat(out double[] matOut)
	{
        matOut = new double[9];
		int numo = 0;
		for (int i = 0; i<3; i++)
			for (int j = 0; j < 3; j++)
			{
				matOut[numo++] = mat[i,j];
			}
	}
	public void SetMat(double x1, double y1, double z1,
		double x2, double y2, double z2,
		double x3, double y3, double z3)
	{
		//		double promMatrix[3][3];
		mat[0,0] = x1;
		mat[0,1] = y1;
		mat[0,2] = z1;

		mat[1,0] = x2;
		mat[1,1] = y2;
		mat[1,2] = z2;

		mat[2,0] = x3;
		mat[2,1] = y3;
		mat[2,2] = z3;
	}

	public void Mult(double x1, double y1, double z1,
		double x2, double y2, double z2,
		double x3, double y3, double z3, double [,] promMatrix, double[,] curMat)
	{
		promMatrix[0,0] = curMat[0,0] * x1 + curMat[0,1] * x2 + curMat[0,2] * x3;
		promMatrix[0,1] = curMat[0,0] * y1 + curMat[0,1] * y2 + curMat[0,2] * y3;
		promMatrix[0,2] = curMat[0,0] * z1 + curMat[0,1] * z2 + curMat[0,2] * z3;


		promMatrix[1,0] = curMat[1,0] * x1 + curMat[1,1] * x2 + curMat[1,2] * x3;
		promMatrix[1,1] = curMat[1,0] * y1 + curMat[1,1] * y2 + curMat[1,2] * y3;
		promMatrix[1,2] = curMat[1,0] * z1 + curMat[1,1] * z2 + curMat[1,2] * z3;

		promMatrix[2,0] = curMat[2,0] * x1 + curMat[2,1] * x2 + curMat[2,2] * x3;
		promMatrix[2,1] = curMat[2,0] * y1 + curMat[2,1] * y2 + curMat[2,2] * y3;
		promMatrix[2,2] = curMat[2,0] * z1 + curMat[2,1] * z2 + curMat[2,2] * z3;
	}
	public void MultX(double x1, double y1, double z1,
		double x2, double y2, double z2,
		double x3, double y3, double z3)
	{
		double[,] promMatrix = new double[3,3];
		promMatrix[0,0] = mat[0,0] * x1 + mat[1,0] * y1 + mat[2,0] * z1;
		promMatrix[0,1] = mat[0,1] * x1 + mat[1,1] * y1 + mat[2,1] * z1;
		promMatrix[0,2] = mat[0,2] * x1 + mat[1,2] * y1 + mat[2,2] * z1;

		promMatrix[1,0] = mat[0,0] * x2 + mat[1,0] * y2 + mat[2,0] * z2;
		promMatrix[1,1] = mat[0,1] * x2 + mat[1,1] * y2 + mat[2,1] * z2;
		promMatrix[1,2] = mat[0,2] * x2 + mat[1,2] * y2 + mat[2,2] * z2;


		promMatrix[2,0] = mat[0,0] * x3 + mat[1,0] * y3 + mat[2,0] * z3;
		promMatrix[2,1] = mat[0,1] * x3 + mat[1,1] * y3 + mat[2,1] * z3;
		promMatrix[2,2] = mat[0,2] * x3 + mat[1,2] * y3 + mat[2,2] * z3;
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				mat[i,j] = promMatrix[i,j];
	}

    public MatrixPerspective()
	{
        mat = new double[3,3];
		mat[0,0] = mat[1,1] = mat[2,2] = 1;
		for (int i = 1; i<3; i++)
			for (int j = 0; j < i; j++)
			{
				mat[i,j] = mat[j,i] = 0;
			}
	}
    }
}
