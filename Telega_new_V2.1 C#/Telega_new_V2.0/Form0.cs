using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//hjhkhjk
namespace Telega_new_V2._0
{
    public partial class Form0 : Form
    {
        public int wheels;
        public double fi1, fi2, fi3, fi4;
        public double delta1, delta2, delta3, delta4;
        public double beta1, beta2, beta3, beta4;

        public Form0()
        {
            InitializeComponent();

            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "3")
            {
                textBox4.Visible = false;
                textBox12.Visible = false;
                comboBox5.Visible = false;
                label6.Visible = false;
                label15.Visible = false;

                textBox1.Text = "90";
                textBox2.Text = "210";
                textBox3.Text = "330";
                textBox4.Text = "330";

                textBox9.Text = "0";
                textBox10.Text = "0";
                textBox11.Text = "0";
                textBox12.Text = "0";

                comboBox2.Text = "Левое";
                comboBox3.Text = "Левое";
                comboBox4.Text = "Левое";
                comboBox5.Text = "Левое";

                pictureBox1.Image = global::Telega_new_V2._0.Properties.Resources.wh3;

                button3.PerformClick();
                
            }

            if (comboBox1.Text == "4")
            {
                textBox4.Visible = true;
                textBox12.Visible = true;
                comboBox5.Visible = true;
                label6.Visible = true;
                label15.Visible = true;

                textBox1.Text = "135";
                textBox2.Text = "45";
                textBox3.Text = "315";
                textBox4.Text = "225";

                textBox9.Text = "45";
                textBox10.Text = "-45";
                textBox11.Text = "45";
                textBox12.Text = "-45";

                comboBox2.Text = "Правое";
                comboBox3.Text = "Левое";
                comboBox4.Text = "Правое";
                comboBox5.Text = "Левое";

                pictureBox1.Image = global::Telega_new_V2._0.Properties.Resources.wh4;
                button3.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 ff;

            refresh_angle();

            if (comboBox1.Text == "3")
            {
                ff = new Form1(wheels, fi1, fi2, fi3, fi3, delta1, delta2, delta3, delta3);
            }
            else
            {
                ff = new Form1(wheels, fi1, fi2, fi3, fi4, delta1, delta2, delta3, delta4);
            }
            ff.Show();

            this.Visible = false;

            ff.FormClosed += (sender1, e1) =>
            {

                this.Visible = true;
            };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            refresh_angle();

            // матрицы поворота для вектора n
            double[,] N1 = new double[3, 3] { { Math.Cos(beta1 + fi1), -Math.Sin(beta1 + fi1), 0 }, { Math.Sin(beta1 + fi1), Math.Cos(beta1 + fi1), 0 }, { 0, 0, 1 } };
            double[,] N2 = new double[3, 3] { { Math.Cos(beta2 + fi2), -Math.Sin(beta2 + fi2), 0 }, { Math.Sin(beta2 + fi2), Math.Cos(beta2 + fi2), 0 }, { 0, 0, 1 } };
            double[,] N3 = new double[3, 3] { { Math.Cos(beta3 + fi3), -Math.Sin(beta3 + fi3), 0 }, { Math.Sin(beta3 + fi3), Math.Cos(beta3 + fi3), 0 }, { 0, 0, 1 } };
            double[,] N4 = new double[3, 3] { { Math.Cos(beta4 + fi4), -Math.Sin(beta4 + fi4), 0 }, { Math.Sin(beta4 + fi4), Math.Cos(beta4 + fi4), 0 }, { 0, 0, 1 } };

            // матрицы поворота для вектора альфа
            double[,] A1 = new double[3, 3] { { Math.Cos(beta1 + fi1 + delta1), -Math.Sin(beta1 + fi1 + delta1), 0 }, { Math.Sin(beta1 + fi1 + delta1), Math.Cos(beta1 + fi1 + delta1), 0 }, { 0, 0, 1 } };
            double[,] A2 = new double[3, 3] { { Math.Cos(beta2 + fi2 + delta2), -Math.Sin(beta2 + fi2 + delta2), 0 }, { Math.Sin(beta2 + fi2 + delta2), Math.Cos(beta2 + fi2 + delta2), 0 }, { 0, 0, 1 } };
            double[,] A3 = new double[3, 3] { { Math.Cos(beta3 + fi3 + delta3), -Math.Sin(beta3 + fi3 + delta3), 0 }, { Math.Sin(beta3 + fi3 + delta3), Math.Cos(beta3 + fi3 + delta3), 0 }, { 0, 0, 1 } };
            double[,] A4 = new double[3, 3] { { Math.Cos(beta4 + fi4 + delta4), -Math.Sin(beta4 + fi4 + delta4), 0 }, { Math.Sin(beta4 + fi4 + delta4), Math.Cos(beta4 + fi4 + delta4), 0 }, { 0, 0, 1 } };

            // матрицы поворота для вектора r
            double[,] R1 = new double[3, 3] { { Math.Cos(fi1), -Math.Sin(fi1), 0 }, { Math.Sin(fi1), Math.Cos(fi1), 0 }, { 0, 0, 1 } };
            double[,] R2 = new double[3, 3] { { Math.Cos(fi2), -Math.Sin(fi2), 0 }, { Math.Sin(fi2), Math.Cos(fi2), 0 }, { 0, 0, 1 } };
            double[,] R3 = new double[3, 3] { { Math.Cos(fi3), -Math.Sin(fi3), 0 }, { Math.Sin(fi3), Math.Cos(fi3), 0 }, { 0, 0, 1 } };
            double[,] R4 = new double[3, 3] { { Math.Cos(fi4), -Math.Sin(fi4), 0 }, { Math.Sin(fi4), Math.Cos(fi4), 0 }, { 0, 0, 1 } };

            DataClass.clear_vectors();

            //повороты векторов => расчет векторов
            DataClass.alpha1 = Matrix_Vector_Multiply(A1, DataClass.alpha1);
            DataClass.alpha2 = Matrix_Vector_Multiply(A2, DataClass.alpha2);
            DataClass.alpha3 = Matrix_Vector_Multiply(A3, DataClass.alpha3);
            DataClass.alpha4 = Matrix_Vector_Multiply(A4, DataClass.alpha4);

            DataClass.n1 = Matrix_Vector_Multiply(N1, DataClass.n1);
            DataClass.n2 = Matrix_Vector_Multiply(N2, DataClass.n2);
            DataClass.n3 = Matrix_Vector_Multiply(N3, DataClass.n3);
            DataClass.n4 = Matrix_Vector_Multiply(N4, DataClass.n4);

            DataClass.r1 = Matrix_Vector_Multiply(R1, DataClass.r1);
            DataClass.r2 = Matrix_Vector_Multiply(R2, DataClass.r2);
            DataClass.r3 = Matrix_Vector_Multiply(R3, DataClass.r3);
            DataClass.r4 = Matrix_Vector_Multiply(R4, DataClass.r4);

            int scale = 350;
            double k = 0.1;

            Grafik plot_r1 = new Grafik(new double[2] { 0, DataClass.r1[0] }, new double[2] { 0, DataClass.r1[1] }, pictureBox2, pictureBox2.Size, Color.Red, scale, scale);
            Grafik plot_r2 = new Grafik(new double[2] { 0, DataClass.r2[0] }, new double[2] { 0, DataClass.r2[1] }, pictureBox2, pictureBox2.Size, Color.Blue, scale, scale);
            Grafik plot_r3 = new Grafik(new double[2] { 0, DataClass.r3[0] }, new double[2] { 0, DataClass.r3[1] }, pictureBox2, pictureBox2.Size, Color.Green, scale, scale);

            Grafik plot_n1 = new Grafik(new double[2] { DataClass.r1[0], k * (DataClass.n1[0]) + DataClass.r1[0] }, new double[2] { DataClass.r1[1], k * (DataClass.n1[1]) + DataClass.r1[1] }, pictureBox2, pictureBox2.Size, Color.Red, scale, scale);
            Grafik plot_n2 = new Grafik(new double[2] { DataClass.r2[0], k * (DataClass.n2[0]) + DataClass.r2[0] }, new double[2] { DataClass.r2[1], k * (DataClass.n2[1]) + DataClass.r2[1] }, pictureBox2, pictureBox2.Size, Color.Blue, scale, scale);
            Grafik plot_n3 = new Grafik(new double[2] { DataClass.r3[0], k * (DataClass.n3[0]) + DataClass.r3[0] }, new double[2] { DataClass.r3[1], k * (DataClass.n3[1]) + DataClass.r3[1] }, pictureBox2, pictureBox2.Size, Color.Green, scale, scale);

            Grafik plot_a1 = new Grafik(new double[2] { DataClass.r1[0], k * (DataClass.alpha1[0]) + DataClass.r1[0] }, new double[2] { DataClass.r1[1], k * (DataClass.alpha1[1]) + DataClass.r1[1] }, pictureBox2, pictureBox2.Size, Color.Red, scale, scale);
            Grafik plot_a2 = new Grafik(new double[2] { DataClass.r2[0], k * (DataClass.alpha2[0]) + DataClass.r2[0] }, new double[2] { DataClass.r2[1], k * (DataClass.alpha2[1]) + DataClass.r2[1] }, pictureBox2, pictureBox2.Size, Color.Blue, scale, scale);
            Grafik plot_a3 = new Grafik(new double[2] { DataClass.r3[0], k * (DataClass.alpha3[0]) + DataClass.r3[0] }, new double[2] { DataClass.r3[1], k * (DataClass.alpha3[1]) + DataClass.r3[1] }, pictureBox2, pictureBox2.Size, Color.Green, scale, scale);

            

            plot_r1.Step_of_grid = 0.1;

            plot_r1.Setka();
            plot_r1.ris();
            plot_r2.ris();
            plot_r3.ris();
            plot_n1.ris();
            plot_n2.ris();
            plot_n3.ris();
            plot_a1.ris();
            plot_a2.ris();
            plot_a3.ris();

            if (wheels == 4)
            {
                Grafik plot_r4 = new Grafik(new double[2] { 0, DataClass.r4[0] }, new double[2] { 0, DataClass.r4[1] }, pictureBox2, pictureBox2.Size, Color.Gold, scale, scale);
                Grafik plot_n4 = new Grafik(new double[2] { DataClass.r4[0], k * (DataClass.n4[0]) + DataClass.r4[0] }, new double[2] { DataClass.r4[1], k * (DataClass.n4[1]) + DataClass.r4[1] }, pictureBox2, pictureBox2.Size, Color.Gold, scale, scale);
                Grafik plot_a4 = new Grafik(new double[2] { DataClass.r4[0], k * (DataClass.alpha4[0]) + DataClass.r4[0] }, new double[2] { DataClass.r4[1], k * (DataClass.alpha4[1]) + DataClass.r4[1] }, pictureBox2, pictureBox2.Size, Color.Gold, scale, scale);

                plot_r4.ris();
                plot_n4.ris();
                plot_a4.ris();

            }
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
                vec[i] = a[i, 0] * b[0] + a[i, 1] * b[1] + a[i, 2] * b[2];

            }

            return vec;
        }


        //функция обновления углов

        public void refresh_angle()
        {
            try
            {
                wheels = int.Parse(comboBox1.Text);

                fi1 = int.Parse(textBox1.Text) * Math.PI / 180;
                fi2 = int.Parse(textBox2.Text) * Math.PI / 180;
                fi3 = int.Parse(textBox3.Text) * Math.PI / 180;
                fi4 = int.Parse(textBox4.Text) * Math.PI / 180;

                beta1 = int.Parse(textBox9.Text) * Math.PI / 180;
                beta2 = int.Parse(textBox10.Text) * Math.PI / 180;
                beta3 = int.Parse(textBox11.Text) * Math.PI / 180;
                beta4 = int.Parse(textBox12.Text) * Math.PI / 180;

                if (comboBox2.Text == "Левое")
                {
                    delta1 = -45 * Math.PI / 180;
                }
                else if (comboBox2.Text == "Правое")
                {
                    delta1 = -135 * Math.PI / 180;
                }
                else
                {
                    delta1 = 0;
                }

                if (comboBox3.Text == "Левое")
                {
                    delta2 = -45 * Math.PI / 180;
                }
                else if (comboBox3.Text == "Правое")
                {
                    delta2 = -135 * Math.PI / 180;
                }
                else
                {
                    delta2 = 0;
                }

                if (comboBox4.Text == "Левое")
                {
                    delta3 = -45 * Math.PI / 180;
                }
                else if (comboBox4.Text == "Правое")
                {
                    delta3 = -135 * Math.PI / 180;
                }
                else
                {
                    delta3 = 0;
                }

                if (comboBox5.Text == "Левое")
                {
                    delta4 = -45 * Math.PI / 180;
                }
                else if (comboBox5.Text == "Правое")
                {
                    delta4 = -135 * Math.PI / 180;
                }
                else
                {
                    delta4 = 0;
                }

            }
            catch
            {
                MessageBox.Show("Неверно заданы параметры");
                return;
            }
        }

        private void Form0_Load(object sender, EventArgs e)
        {
            button3.PerformClick();
        }

    }
}
