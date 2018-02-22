using System.Windows.Forms;
using System;

namespace Telega_new_V2._0
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    // Переменные данного статического класса будут доступна откуда угодно в пределах проекта
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public static class DataClass
    {
        // переменные для лидара
        public static bool flag_start = false;      // флаг работы лидара

        public static double[] x = new double[400]; //массивы точек принятых с лидара
        public static double[] y = new double[400];

        public static int angle = 0;                // прибавляемый угол для точек от лидара



        //переменные определяющие конструкцию робота
        static double rad = 0.16;  //расстояние от центра платформы до центра колеса

        //векторы
        public static double[] n1 = new double[3] { -1, 0, 0 };
        public static double[] n2 = new double[3] { -1, 0, 0 };
        public static double[] n3 = new double[3] { -1, 0, 0 };
        public static double[] n4 = new double[3] { -1, 0, 0 };

        public static double[] alpha1 = new double[3] { 1, 0, 0 };
        public static double[] alpha2 = new double[3] { 1, 0, 0 };
        public static double[] alpha3 = new double[3] { 1, 0, 0 };
        public static double[] alpha4 = new double[3] { 1, 0, 0 };

        public static double[] r1 = new double[3] { rad, 0, 0 };
        public static double[] r2 = new double[3] { rad, 0, 0 };
        public static double[] r3 = new double[3] { rad, 0, 0 };
        public static double[] r4 = new double[3] { rad, 0, 0 };

        public static void clear_vectors()
        {
            n1 = new double[3] { -1, 0, 0 };
            n2 = new double[3] { -1, 0, 0 };
            n3 = new double[3] { -1, 0, 0 };
            n4 = new double[3] { -1, 0, 0 };

            alpha1 = new double[3] { 1, 0, 0 };
            alpha2 = new double[3] { 1, 0, 0 };
            alpha3 = new double[3] { 1, 0, 0 };
            alpha4 = new double[3] { 1, 0, 0 };

            r1 = new double[3] { rad, 0, 0 };
            r2 = new double[3] { rad, 0, 0 };
            r3 = new double[3] { rad, 0, 0 };
            r4 = new double[3] { rad, 0, 0 };
        }
    }

}

