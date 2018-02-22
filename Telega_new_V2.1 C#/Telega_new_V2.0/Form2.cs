using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Telega_new_V2._0
{
    public partial class Form2 : Form
    {
        public bool on_x, on_y;     // переменные для осевой симметрии точек
        public bool flag = false;   // true когда нажата кнопка мышки => перетаскиваем график
        public Point oldMouse;
        
        public double scale = 1;    //масштаб

        public int p = 4;           //диаметр точек

        Grafik gr1;


        public Form2()
        {
            InitializeComponent();

            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;   //без этого не работает
                        

            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);


            gr1 = new Grafik(DataClass.x, DataClass.y, pictureBox1, pictureBox1.Size, Color.Red, scale, scale);

            p = int.Parse(numericUpDown1.Text);


            textBox1.Text = Math.Round(gr1.Scale_x, 1).ToString();
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double delta = 0.2;         // значение изменения масштаба


            if (e.Delta > 0)
            {
                try
                {
                    gr1.clear_bitmap();

                    if (gr1.Scale_x < 100)
                    {
                        gr1.Scale_x += delta;
                        gr1.Scale_y += delta;


                        gr1.Setka();
                        gr1.points(p, on_x, on_y);

                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    if (gr1.Scale_y > 0.3)
                    {
                        gr1.clear_bitmap();

                        gr1.Scale_x -= delta;
                        gr1.Scale_y -= delta;


                        gr1.Setka();
                        gr1.points(p, on_x, on_y);

                    }
                }
                catch { }
            }

            textBox1.Text = Math.Round(gr1.Scale_x, 1).ToString();
        }

        // кнопка выход
        private void button2_Click(object sender, EventArgs e)
        {
            DataClass.angle = 0;

            Close();
        }

        //нажатие кнопки мыши
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            flag = true;
            oldMouse = new Point(e.X, e.Y);
        }

        //перемещение мышки с нажатой клавишей (переменная flag)
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                Point mousePos = new Point(e.X, e.Y);
                try
                {
                    p = int.Parse(numericUpDown1.Text);

                    gr1.clear_bitmap();

                    gr1.X_null = (mousePos.X - oldMouse.X) + gr1.X_null;
                    gr1.Y_null = (mousePos.Y - oldMouse.Y) + gr1.Y_null;

                    gr1.Setka();
                    gr1.points(p, on_x, on_y);

                }
                catch { }
                oldMouse = new Point(e.X, e.Y);
            }
        }

        //отпускание кнопки мыши
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;
            pictureBox1.Focus();
        }

        //событие таймера, здесь обновляем график, если не нажата кнопка мыши
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                gr1.Step_of_grid = (int)numericUpDown2.Value;
                p = (int)numericUpDown1.Value;
                DataClass.angle = (int)numericUpDown3.Value;

            }
            catch
            {
                MessageBox.Show("Неверный масштаб или шаг сетки");
            }


            if (!flag)
            {

                gr1.X = DataClass.x;
                gr1.Y = DataClass.y;

                gr1.clear_bitmap();
                gr1.Setka();
                gr1.points(p, on_x, on_y);
            }
        }

        // ставим или убираем галочки для осевой симметрии
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            on_x = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            on_y = checkBox2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DataClass.x.Length; i++)
            {
                DataClass.x[i] = 0;
                DataClass.y[i] = 0;
                gr1.clear_bitmap();
            }
        }
    }
}
