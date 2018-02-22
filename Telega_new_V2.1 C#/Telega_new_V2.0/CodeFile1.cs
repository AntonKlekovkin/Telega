using System;
using System.Windows.Forms;


namespace Telega_new_V2._0
{
    public partial class Form1 : Form
    {
        /////////////////////////////////////////////////////////////
        //Функция расчета скорости колеса
        /////////////////////////////////////////////////////////////

        double SpeedWheel(double[] vec_v, double[] vec_w, double[] alpha, double[] r)
        {
            double sp;
            double delta = 45 * Math.PI / 180;              // угол между векторами ??
            double h = 0.0475;                              // радиус колеса

            sp = (-10) * Dot_Vector(Sum_Vector(vec_v, Cross_Vector(vec_w, r)), alpha) / (Math.Sin(delta) * h);

            return sp;
        }


        /////////////////////////////////////////////////////////////
        //Функция умножения матрицы на вектор
        /////////////////////////////////////////////////////////////

        double[] Matrix_Vector_Multiply(double[,] a, double[] b)
        {
            double[] op = new double[3];
            double[] vec = new double[3];

            for (int i = 0; i < 3; i++)
            {
                vec[i] = a[i,0]*b[0] + a[i,1]*b[1] + a[i,2]*b[2];
                
            }

            return vec;
        }

        /////////////////////////////////////////////////////////////
        //Функция скалярного произведения векторов
        /////////////////////////////////////////////////////////////

        double Dot_Vector(double[] a, double[] b)
        {
            double dot = 0;

            dot = a[0] * b[0] + a[1] * b[1] + a[2] * b[2];

            return dot;
        }

        /////////////////////////////////////////////////////////////
        //Функция векторного произведения векторов
        /////////////////////////////////////////////////////////////

        double[] Cross_Vector(double[] a, double[] b)
        {
            double[] vec = new double[3];

            vec[0] = a[1] * b[2] - a[2] * b[1];
            vec[1] = a[2] * b[0] - a[0] * b[2];
            vec[2] = a[0] * b[1] - a[1] * b[0];
            
            return vec;

        }

        /////////////////////////////////////////////////////////////
        //Функция сложения векторов
        /////////////////////////////////////////////////////////////

        double[] Sum_Vector(double[] a, double[] b)
        {
            double[] vec = new double[3];

            vec[0] = a[0] + b[0];
            vec[1] = a[1] + b[1];
            vec[2] = a[2] + b[2];

            return vec;

        }
    }
}