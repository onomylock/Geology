using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geology.Projection
{
    public class CCamera
    {
       public class Vector3 {
		                    double x, y, z;  // Просто double для X,Y,Z
	                        public Vector3(){x=y=z=0;}
	                        public Vector3(double _x,double _y,double _z){x=_x; y=_y; z=_z;}
                            public Vector3(Vector3 vect) { x = vect.x; y = vect.y; z = vect.z; }
                            public void MultNumber(double numb)
                            {
                                x *= numb; y *= numb; z *= numb;
                            }
                            public static Vector3 operator +(Vector3 el1,Vector3 el2) 
                            {
                                return new Vector3(el1.x+el2.x,el1.y+el2.y,el1.z+el2.z);
                            }
                            public static Vector3 operator -(Vector3 el1,Vector3 el2) 
                            {
                                return new Vector3(el1.x-el2.x,el1.y-el2.y,el1.z-el2.z);
                            }
                            public double getNorm()
                            {
                                return Math.Sqrt(x*x+y*y+z*z);
                            }
                            public double X { get { return x; } set { x = value; } }
                            public double Y { get { return y; } set { y = value; } }
                            public double Z { get { return z; } set { z = value; } }
                        };

	public Vector3 m_vPosition;   // Позиция камеры.
	Vector3 m_vPositionStart;   // Позиция камеры.
	public Vector3 m_vView;   // Направление камеры.
	Vector3 m_vViewStart;   // Позиция камеры.
	public Vector3 m_vUpVector;   // Вертикальный вектор.
	Vector3 m_vUpVectorStart;   // Позиция камеры.

	//В начале переменных подписываем "m", т.к. это "member"-переменные.
	//"v" добавляем, т.к. это члены класса Vector (начинается на V).


	// Конструктор класса Camera
    public CCamera()
    {
        PositionCamera(0.0, 0.0, 0.0,0,1,0.5,0,1,0);
    }

	void SetvView(double x, double y, double z)
	{
        m_vView=new Vector3(x,y,z);
        m_vViewStart = new Vector3(x, y, z);
	}
	// Тут изменяется положение, направление и верт. вектор камеры.
	// В основном используется при инициализации.
    public void PositionCamera(double positionX, double positionY, double positionZ,
        double viewX, double viewY, double viewZ,
        double upVectorX, double upVectorY, double upVectorZ)
        {
	        m_vPositionStart = new Vector3( positionX, positionY, positionZ );
	        m_vViewStart = new Vector3(  viewX, viewY, viewZ );
	        m_vUpVectorStart =new Vector3(  upVectorX, upVectorY, upVectorZ);
            InvalidateCamera();
        }
    public void InvalidateCamera()
    {
        m_vPosition = new Vector3(m_vPositionStart);
        m_vView = new Vector3(m_vViewStart);
        m_vUpVector = new Vector3(m_vUpVectorStart);
    }
	// Эта ф-я передвигает камеру вперед/назад
    public void MoveCamera(double speed)
    {
        
        Vector3 vVector = (m_vPosition - m_vView);
        vVector.MultNumber(1 / speed);
        //Получаем направление взгляда (напр-е, куда мы повернуты "лицом")
        m_vPosition = vVector + m_vView;
    }
	// Глобальные данные камеры
	/*double getNorm()
	{
		return Math.Sqrt((m_vPosition.x - m_vView.x)*(m_vPosition.x - m_vView.x) + (m_vPosition.y - m_vView.y)*(m_vPosition.y - m_vView.y) + (m_vPosition.z - m_vView.z)*(m_vPosition.z - m_vView.z));
	}*/
	public double GetDistanse()
	{
		return (m_vPosition-m_vView).getNorm();
	}
    public double GetDistanse(double x,double y,double z)
    {
        Vector3 tmpVect = new Vector3(x,y,z);
        return (m_vPosition - tmpVect).getNorm();
    }
    void RotateView(float angle, float x, float y, float z)
    {
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView = new Vector3();

        // Получим наш вектор взгляда (направление, куда мы смотрим)
        vView = m_vPosition - m_vView;
      

        // Рассчитаем 1 раз синус и косинус переданного угла
        float cosTheta = (float)Math.Cos(angle);
        float sinTheta = (float)Math.Cos(angle);

        // Найдем новую позицию X для вращаемой точки
        vNewView.X = (cosTheta + (1 - cosTheta) * x * x) * vView.X;
        vNewView.X += ((1 - cosTheta) * x * y - z * sinTheta) * vView.Y;
        vNewView.X += ((1 - cosTheta) * x * z + y * sinTheta) * vView.Z;


        vNewUp.X = (cosTheta + (1 - cosTheta) * x * x) * m_vUpVector.X;
        vNewUp.X += ((1 - cosTheta) * x * y - z * sinTheta) * m_vUpVector.Y;
        vNewUp.X += ((1 - cosTheta) * x * z + y * sinTheta) * m_vUpVector.Z;

        // Найдем позицию Y
        vNewView.Y = ((1 - cosTheta) * x * y + z * sinTheta) * vView.X;
        vNewView.Y += (cosTheta + (1 - cosTheta) * y * y) * vView.Y;
        vNewView.Y += ((1 - cosTheta) * y * z - x * sinTheta) * vView.Z;

        vNewUp.Y = ((1 - cosTheta) * x * y + z * sinTheta) * m_vUpVector.X;
        vNewUp.Y += (cosTheta + (1 - cosTheta) * y * y) * m_vUpVector.Y;
        vNewUp.Y += ((1 - cosTheta) * y * z - x * sinTheta) * m_vUpVector.Z;

        // И позицию Z
        vNewView.Z = ((1 - cosTheta) * x * z - y * sinTheta) * vView.X;
        vNewView.Z += ((1 - cosTheta) * y * z + x * sinTheta) * vView.Y;
        vNewView.Z += (cosTheta + (1 - cosTheta) * z * z) * vView.Z;


        vNewUp.Z = ((1 - cosTheta) * x * z - y * sinTheta) * m_vUpVector.X;
        vNewUp.Z += ((1 - cosTheta) * y * z + x * sinTheta) * m_vUpVector.Y;
        vNewUp.Z += (cosTheta + (1 - cosTheta) * z * z) * m_vUpVector.Z;
        // Теперь просто добавим новый вектор вращения к нашей позиции, чтобы
        // установить новый взгляд камеры.
        m_vPosition = m_vView + vNewView;
        m_vUpVector = vNewUp;
    
    }
    public void MultMatrixView(double[] mat)
    {
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView;

        vView = m_vPosition - m_vView;
        // Получим наш вектор взгляда (направление, куда мы смотрим)
  

        // Рассчитаем 1 раз синус и косинус переданного угла


        // Найдем новую позицию X для вращаемой точки
        vNewView.X = mat[0] * vView.X + mat[1] * vView.Y + mat[2] * vView.Z;
        vNewView.Y = mat[3] * vView.X + mat[4] * vView.Y + mat[5] * vView.Z;
        vNewView.Z = mat[6] * vView.X + mat[7] * vView.Y + mat[8] * vView.Z;

        vNewUp.X = mat[0] * m_vUpVector.X + mat[1] * m_vUpVector.Y + mat[2] * m_vUpVector.Z;
        vNewUp.Y = mat[3] * m_vUpVector.X + mat[4] * m_vUpVector.Y + mat[5] * m_vUpVector.Z;
        vNewUp.Z = mat[6] * m_vUpVector.X + mat[7] * m_vUpVector.Y + mat[8] * m_vUpVector.Z;

        // Теперь просто добавим новый вектор вращения к нашей позиции, чтобы
        // установить новый взгляд камеры.
        m_vPosition = m_vView + vNewView;
        m_vUpVector = vNewUp;
    
    }
    public void MultMatrixViewTransXYZ(double x, double y, double z)
    {
        //MultMatrixView(mat);
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView = new Vector3(x, y, z);
        // Получим наш вектор взгляда (направление, куда мы смотрим)

        // Рассчитаем 1 раз синус и косинус переданного угла

        m_vPosition = m_vPosition + vView;
        // Теперь просто добавим новый вектор вращения к нашей позиции, чтобы
        // установить новый взгляд камеры.
        m_vView = m_vView + vView;
    }
    public void MultMatrixView(double[] mat, double x, double y, double z)
    {
        //MultMatrixView(mat);
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView = new Vector3(x,y,z);
        // Получим наш вектор взгляда (направление, куда мы смотрим)

        // Рассчитаем 1 раз синус и косинус переданного угла


        // Найдем новую позицию X для вращаемой точки
        vNewView.X = mat[0] * vView.X + mat[1] * vView.Y + mat[2] * vView.Z;
        vNewView.Y = mat[3] * vView.X + mat[4] * vView.Y + mat[5] * vView.Z;
        vNewView.Z = mat[6] * vView.X + mat[7] * vView.Y + mat[8] * vView.Z;

        m_vPosition = m_vPosition + vNewView;
        // Теперь просто добавим новый вектор вращения к нашей позиции, чтобы
        // установить новый взгляд камеры.
        m_vView = m_vView + vNewView;
    }
    public void MultMatrixView(double[] mat, double x, double y, double z,out double xRes,out double yRes,out double zRes)
    {
        //MultMatrixView(mat);
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView = new Vector3(x, y, z);
        // Получим наш вектор взгляда (направление, куда мы смотрим)

        // Рассчитаем 1 раз синус и косинус переданного угла


        // Найдем новую позицию X для вращаемой точки
        xRes = mat[0] * vView.X + mat[1] * vView.Y + mat[2] * vView.Z;
        yRes = mat[3] * vView.X + mat[4] * vView.Y + mat[5] * vView.Z;
        zRes = mat[6] * vView.X + mat[7] * vView.Y + mat[8] * vView.Z;

    }
    void Translate(double x, double y, double z, double angleX, double angleY, double angleZ)
    {
        Vector3 vView  = new Vector3(x,y,z);

        RotVector(angleX, 1f, 0f, 0f, ref vView);
        RotVector(angleY, 0, 1, 0, ref vView);
        RotVector(angleZ, 0, 0, 1, ref vView);

        m_vView = m_vView + vView;
        m_vPosition = m_vPosition + vView;
    
    }
    void RotVector(double angle, double x, double y, double z, ref Vector3 vector)
    {
        Vector3 vNewView = new Vector3();
        Vector3 vNewUp = new Vector3();
        Vector3 vView = m_vPosition - m_vView;

        // Получим наш вектор взгляда (направление, куда мы смотрим)


        // Рассчитаем 1 раз синус и косинус переданного угла
        float cosTheta = (float)Math.Cos(angle);
        float sinTheta = (float)Math.Sin(angle);

        // Найдем новую позицию X для вращаемой точки
        vNewView.X = (cosTheta + (1 - cosTheta) * x * x) * vView.X;
        vNewView.X += ((1 - cosTheta) * x * y - z * sinTheta) * vView.Y;
        vNewView.X += ((1 - cosTheta) * x * z + y * sinTheta) * vView.Z;


        vNewUp.X = (cosTheta + (1 - cosTheta) * x * x) * m_vUpVector.X;
        vNewUp.X += ((1 - cosTheta) * x * y - z * sinTheta) * m_vUpVector.Y;
        vNewUp.X += ((1 - cosTheta) * x * z + y * sinTheta) * m_vUpVector.Z;

        // Найдем позицию Y
        vNewView.Y = ((1 - cosTheta) * x * y + z * sinTheta) * vView.X;
        vNewView.Y += (cosTheta + (1 - cosTheta) * y * y) * vView.Y;
        vNewView.Y += ((1 - cosTheta) * y * z - x * sinTheta) * vView.Z;

        vNewUp.Y = ((1 - cosTheta) * x * y + z * sinTheta) * m_vUpVector.X;
        vNewUp.Y += (cosTheta + (1 - cosTheta) * y * y) * m_vUpVector.Y;
        vNewUp.Y += ((1 - cosTheta) * y * z - x * sinTheta) * m_vUpVector.Z;

        // И позицию Z
        vNewView.Z = ((1 - cosTheta) * x * z - y * sinTheta) * vView.X;
        vNewView.Z += ((1 - cosTheta) * y * z + x * sinTheta) * vView.Y;
        vNewView.Z += (cosTheta + (1 - cosTheta) * z * z) * vView.Z;


        vNewUp.Z = ((1 - cosTheta) * x * z - y * sinTheta) * m_vUpVector.X;
        vNewUp.Z += ((1 - cosTheta) * y * z + x * sinTheta) * m_vUpVector.Y;
        vNewUp.Z += (cosTheta + (1 - cosTheta) * z * z) * m_vUpVector.Z;
        // Теперь просто добавим новый вектор вращения к нашей позиции, чтобы
        // установить новый взгляд камеры.
        m_vPosition = m_vView + vNewView;

        m_vUpVector = vNewUp;
    
    }

    }
}
