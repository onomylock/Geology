using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Geology.Objects
{
    public class Spline
    {
        public double[][] q;
        public double[] x;
        public double[] y;
        public Spline()
        {
            q = null;
            x =y= null;

        }
        public Spline(string file)
        {

            using (StreamReader reader = new StreamReader(file))
            {
                int xCounter = Int32.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0]);
                x = new double[xCounter];
                for (int i = 0; i < xCounter; i++)
                {
                    x[i] = Double.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0]);
                }
                int yCounter = Int32.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0]);
                y = new double[yCounter];
                for (int i = 0; i < yCounter; i++)
                {
                    y[i] = Double.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0]);
                }
                int countq = Int32.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0])/4;
                q = new double[countq][];
                for (int i = 0; i < countq; i++)
                {
                    q[i] = new double[4];
                    for (int j = 0; j < 4; j++)
                    {
                        q[i][j] = Double.Parse(((reader.ReadLine().Split(' ', '\t')).Where(x1 => x1 != "").ToArray())[0]);
                    }
                }
            }
        }
        public double getFunction(int numF,double ksi,double h)
        {
            switch (numF)
            {
                case 0: return 1 - 3 * ksi * ksi + 2 * ksi * ksi * ksi;
                case 1: return h*(3 * ksi * ksi - 2 * ksi * ksi * ksi);
                case 2: return ksi - 2 * ksi * ksi + ksi * ksi * ksi;
                case 3: return h*(-ksi * ksi + ksi * ksi * ksi);
                default: return 0;
            }
        }
        public void getNumFunction(int num,out int i,out int j)
        {
            switch (num)
            {
                case 0:  i = 0; j = 0; break;
                case 1:  i = 1; j = 0; break;
                case 2:  i = 0; j = 1; break;
                case 3:  i = 1; j = 1; break;
                case 4:  i = 2; j = 0; break;
                case 5:  i = 3; j = 0; break;
                case 6:  i = 2; j = 1; break;
                case 7:  i = 3; j = 1; break;
                case 8:  i = 0; j = 2; break;
                case 9:  i = 1; j = 2; break;
                case 10: i = 0; j = 3; break;
                case 11: i = 1; j = 3; break;
                case 12: i = 2; j = 2; break;
                case 13: i = 3; j = 2; break;
                case 14: i = 2; j = 3; break;
                case 15: i = 3; j = 3; break;
                default: i = -1; j = -1; break;
            }

        }
        public double getValue(int i,int j,double _x,double _y)
        {
            double hx = (x[i + 1] - x[i]);
            double hy = (y[j + 1] - y[j]);
            double ksi = (_x-x[i])/hx;
            double nu = (_y-y[j])/hy;
            double sum=0;
            double[] qval = new double[16] { q[(j) * (x.Length - 1) + i][0], q[(j) * (x.Length - 1) + i][1],q[(j) * (x.Length - 1) + i][2],q[(j) * (x.Length - 1) + i][3],
                                             q[(j) * (x.Length - 1) + i + 1][0], q[(j) * (x.Length - 1) + i + 1][1],q[(j) * (x.Length - 1) + i + 1][2],q[(j) * (x.Length - 1) + i + 1][3],
                                             q[(j + 1) * (x.Length - 1) + i][0], q[(j + 1) * (x.Length - 1) + i][1],q[(j + 1) * (x.Length - 1) + i][2],q[(j + 1) * (x.Length - 1) + i][3],
                                             q[(j + 1) * (x.Length - 1) + i + 1][0], q[(j + 1) * (x.Length - 1) + i + 1][1],q[(j + 1) * (x.Length - 1) + i + 1][2],q[(j + 1) * (x.Length - 1) + i + 1][3],
            };
            for (int ind=0;ind<16;ind++)
            {
                int iks,inu;
                getNumFunction(ind,out iks,out inu);
              
                sum += qval[ind]* getFunction(iks, ksi, hx) * getFunction(inu, nu, hy);
            }
            return qval[0];
        }
        public double getValueInPoint (double _x,double _y,double standartValue)
        {
            if (x != null && y != null && _x <= x[x.Length - 1] && _x >= x[0] && _y <= y[y.Length - 1] && _y >= y[0])
            {
                int xCoord=-1;
                for (int i=0;i<x.Length-1;i++)
                {
                    if (_x >= x[i] && _x <= x[i + 1])
                    {
                        xCoord = i;
                        break;
                    }
                }
                int yCoord=-1;
                for (int i=0;i<y.Length-1;i++)
                {
                    if (_y >= y[i] && _y <= y[i + 1])
                    {
                        yCoord = i;
                        break;
                    }
                }
                return getValue(xCoord,yCoord,_x,_y);
            }
            else
                return standartValue;
 
        }

    }
}
