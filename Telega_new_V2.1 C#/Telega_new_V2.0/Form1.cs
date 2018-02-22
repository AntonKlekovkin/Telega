using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.IO;
using System.Timers;
// регулярные выражения
using System.Text.RegularExpressions;
using System.Globalization;

// для преобразования функции из строки
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;



namespace Telega_new_V2._0
{
    public partial class Form1 : Form
    {

        public Device device;

        public JoystickState j;
        SerialPort port = new SerialPort(); //обект класса SerialPort передачи данных через ком-порт
        Form2 ff;


        // переменные для графиков
        Grafik gr_x, gr_y;
        public double[] t;
        public double[] x;
        public double[] y;


        public bool flag_x = false;   // true когда нажата кнопка мышки => перетаскиваем график
        public Point oldMouse_x;
        public bool flag_y = false;   // true когда нажата кнопка мышки => перетаскиваем график
        public Point oldMouse_y;



        public int wheels;
        public double fi1, fi2, fi3, fi4;
        public double delta1, delta2, delta3, delta4;

        //Thread th;  //объект класса Thread - поток

        public bool flag_connect = false;   //флаг соединения
        public bool flag_wait = false;   //флаг ожидания
        public bool flag_osi = false;   //флаг осей

        public string message1;

        


        public double[,] data = new double[3, 500];
        public int flag_data = 0, count = 0;

        public delegate void Write(string str, byte[] mas);
        public Write mydel;

        //private void WriteMethod(string indata);

        public Form1(int wheels, double fi1, double fi2, double fi3, double fi4, double delta1, double delta2, double delta3, double delta4)
        {
            InitializeComponent();

            comboBox1.Items.AddRange(SerialPort.GetPortNames());    //создаем список доступных портов в комбобокс1

            if (SerialPort.GetPortNames().Length != 0)  //если найден хоть один
            {
                comboBox1.Text = SerialPort.GetPortNames()[0];  //выводим первый доступный
            }

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;   //без этого не работает

            mydel = new Write(WriteMethod);

            this.fi1 = fi1;
            this.fi2 = fi2;
            this.fi3 = fi3;
            this.fi4 = fi4;

            this.delta1 = delta1;
            this.delta2 = delta2;
            this.delta3 = delta3;
            this.delta4 = delta4;

            this.wheels = wheels;

            if (wheels == 3)
            {
                textBox8.Visible = false;
                label8.Visible = false;
                panel8.Visible = false;
            }
            else if (wheels == 4)
            {
                textBox8.Visible = true;
                label8.Visible = true;
                panel8.Visible = true;
            }

            


        }

        //////////////////////////////////////////////////////////////////
        // Кнопка обновить
        //////////////////////////////////////////////////////////////////

        private void button9_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();    //удаляем все что есть

            comboBox1.Items.AddRange(SerialPort.GetPortNames());    //добавляем в список доступные порты
        }

        //////////////////////////////////////////////////////////////////
        // Кнопка Подлючить/Отключить
        //////////////////////////////////////////////////////////////////
        private void button10_Click(object sender, EventArgs e)
        {
            if (!flag_connect)  //если соединение не установлено
            {
                textBox21.Text = "Connecting...";
                textBox21.Update();

                port.PortName = comboBox1.Text; //имя порта из списка1
                port.BaudRate = int.Parse(comboBox2.Text);  //скорость из списка 2
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceviedHandler);

                // Set the read/write timeouts
                port.ReadTimeout = 500;
                port.WriteTimeout = 500;

                try
                {
                    port.Open();    //устанавливаем соединение
                }
                catch
                {
                    textBox21.Text = "Error";
                    return;
                }
                button10.Text = "Disconnect";    
                flag_connect = true;    //устанавливем флаг соединения 1
                textBox21.Text = "OK";


                return; //выходим из функции

            }

            if (flag_connect)   //если соединение установлено
            {
                flag_connect = false;   //сбрасываем флаг
            
                port.Close();   //закрываем соединение
                button10.Text = "Connect";   //меняем имя кнопки
                textBox21.Text = "Not connect";

                //g = panel1.CreateGraphics();    //рисуем график на панели 1
                //g.Clear(Color.White);
                flag_osi = false;

                //Grafik gr = new Grafik(shag, x, g, panel1.ClientSize, Color.Red,30,5); //обьект класса для первого графика
                //Grafik gr1 = new Grafik(shag, y, g, panel1.ClientSize, Color.Green, 30,5);  //для второго графика
                //gr.Osi();
                //gr.ris();
                //gr1.ris();

                //col = 0;
                //gyro_x[0] = gyro_x[1] = gyro_y[0] = gyro_y[1] = shag[0] = shag[1] = 0;
                //gyro_z[0] = gyro_z[1] = alfa_x[0] = alfa_x[1] = 0;
                //accel_x[0] = accel_x[1] = accel_y[0] = accel_y[1] = accel_z[0] = accel_z[1] = 0;

                return;
            }
        }

        private void CommandSet(byte a0, byte a1, byte dir1, int speed1, byte dir2, int speed2, byte dir3, int speed3, byte dir4, int speed4, byte move)
        {
            byte[] Command = new byte[15];   //переменная для отправки команд

            Command[0] = a0;    // For first MC (1-read, 2-send to second mc)
            Command[1] = a1;    // Command for operation (125 - for motors)

            Command[2] = dir1;                    //direction
            Command[3] = (byte)(speed1 >> 8);     //speed high
            Command[4] = (byte)(speed1 & 0xff);     //speed low

            Command[5] = dir2;                                 //direction
            Command[6] = (byte)(speed2 >> 8);     //speed high
            Command[7] = (byte)(speed2 & 0xff);     //speed low

            Command[8] = dir3;                                 //direction
            Command[9] = (byte)(speed3 >> 8);     //speed high
            Command[10] = (byte)(speed3 & 0xff);     //speed low

            Command[11] = dir4;                                 //direction
            Command[12] = (byte)(speed4 >> 8);     //speed high
            Command[13] = (byte)(speed4 & 0xff);     //speed low

            Command[14] = move;                    // 0-stop, 1-forward, 2-reverse

            SendRF(Command);
        }

        public void SendRF(byte [] command)
        {
                           
            //передача команды

            if (port.IsOpen)
            {
                port.Write(command, 0, command.Length);
                //flag_wait = true;
                //Thread.Sleep(200);
            }
            else
            {
                textBox21.Text = "Not connect";
            }
        }

      

        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////


        //событие при закрытии окна
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (flag_connect)   //если соединение установлено
            {
                flag_connect = false;   //сбрасываем флаг

                
                //th.Join();  //ждем завершения потока
                port.Close();   //закрываем соединение

            }
        }



        //////////////////////////////////////////////////////////////////
        // Метод записи принятых данных в текстбокс
        //////////////////////////////////////////////////////////////////
        private void WriteMethod(string indata, byte[] mas)
        {

            
            //ASCII
            if (radioButton2.Checked == true)
            {
                try
                {
                    textBox1.AppendText(indata);
                }
                catch { }
            }

            //DEC
            else if (radioButton3.Checked == true)
            {
                for (int i = 0; i < mas.Length; i++)
                {
                    textBox1.AppendText(Convert.ToString((byte)mas[i]));
                    textBox1.AppendText(" ");
                }
            }

            //HEX
            else if (radioButton4.Checked == true)
            {
                for (int i = 0; i < mas.Length; i++)
                {
                    textBox1.AppendText(string.Format("{0:X2}", (byte)mas[i]));
                    textBox1.AppendText(" ");
                }
            }
           
            
        }

        //////////////////////////////////////////////////////////////////
        // Событие приема данных
        //////////////////////////////////////////////////////////////////
        private void DataReceviedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = "";
            byte[] rec;

            //Thread.Sleep(10);

            if (DataClass.flag_start == true)
            {
                rec = lidar_data(sp);
                mydel(indata, rec);
            }
            else
            {
                if (sp.IsOpen)
                {
                    rec = new byte[sp.BytesToRead];
                }
                else rec = new byte[1];

                // Off
                if (radioButton1.Checked == true)
                {
                    indata = sp.ReadExisting();
                    return;
                }

                // ASCII
                else if (radioButton2.Checked == true)
                {
                    try
                    {
                        indata = sp.ReadExisting();
                    }
                    catch { }
                }


                // DEC
                else if (radioButton3.Checked == true || radioButton4.Checked == true)
                {
                    try
                    {
                        sp.Read(rec, 0, sp.BytesToRead);
                    }
                    catch { }
                }

                mydel(indata, rec);
            }
        }

        public byte[] lidar_data(SerialPort sp)
        {

            int count_data = 400;
            byte[] rec;

            if (sp.IsOpen && DataClass.flag_start && sp.BytesToRead == 7)
            {
                rec = new byte[7];
                //DataClass.flag_start = false;
                try
                {
                    sp.Read(rec, 0, 7);
                }
                catch { }

                //mydel(indata, rec);
                return rec;

            }
            else if (sp.IsOpen && sp.BytesToRead >= count_data)
            {
                rec = new byte[count_data];

                try
                {
                    sp.Read(rec, 0, count_data);
                }
                catch { }
            }

            else
            {
                rec = new byte[1];
                return rec;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Обработка принятых данных
            ///////////////////////////////////////////////////////////////////////////////////////



            for (int i = 0; i < count_data-5; )
            {
                if (rec.Length > 1)
                {
                    if ((rec[i] & 1) == 1 && (rec[i] & 0x02) == 0 && (rec[i + 1] & 1) == 1)
                    {
                        flag_data = 1;
                        count = 0;


                    }
                    
                }


                if (flag_data == 1 && (rec[i] & 1) == 0 && (rec[i] & 0x02) == 2 && (rec[i + 1] & 1) == 1)
                {
                    data[0, count] = (rec[i] >> 2);                //quality

                    data[1, count] = rec[i + 1] >> 1;         //angle, degree/64
                    data[1, count] += rec[i + 2] << 7;

                    data[1, count] = (data[1, count] / 64) + DataClass.angle; //angle, degree

                    data[2, count] = rec[i + 3];              //distance, 0.25mm
                    data[2, count] += rec[i + 4] << 8;

                    data[2, count] = data[2, count] / 40;   //distance, cm

                    DataClass.x[count] = data[2, count] * Math.Cos(data[1, count] * Math.PI / 180);
                    DataClass.y[count] = data[2, count] * Math.Sin(data[1, count] * Math.PI / 180);

                    count++;
                    if (count == count_data)
                    {
                        count = 0;
                    }

                    i += 5;

                }
                else
                {
                    i++;
                }
            }

            return rec;
        }

        //////////////////////////////////////////////////////////////////
        // Кнопка выход
        //////////////////////////////////////////////////////////////////

        private void button11_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)    //если установлено соединение
            {
                flag_connect = false;   //сбрасываем флаг

                                
                //th.Join();  //ждем завершение потока
                port.Close();   //закрываем соединение

            }

            if (!(ff == null))
            {
                ff.Close();
            }

            Close(); //выходим из программы
        }

        //////////////////////////////////////////////////////////////////
        // Событие при нажатии на кнопку
        //////////////////////////////////////////////////////////////////
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                button1.PerformClick();
            }

            if (e.KeyCode == Keys.W)
            {
                button4.PerformClick();
            }

            if (e.KeyCode == Keys.E)
            {
                button2.PerformClick();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = true;
            if (e.KeyChar == (char)Keys.Q)
            {
                button1.PerformClick();
            }

            if (e.KeyChar == (char)Keys.W)
            {
                button4.PerformClick();
            }

            if (e.KeyChar == (char)Keys.E)
            {
                button2.PerformClick();
            }

            
            Thread.Sleep(100);
        }

               

        

        
        

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = textBox6.Text = textBox7.Text = textBox8.Text = textBox2.Text;
        }

        

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] aaa=new byte[2];

            aaa[0] = 1;
            aaa[1] = 10;
            if (port.IsOpen)
            {
                port.Write(aaa, 0, 2);
            }
            else
            {
                textBox21.Text = "Not connect";
            }
        }


        ////////////////////////////////////////////////////////////////////////
        //кнопка вперед 3 колеса
        ////////////////////////////////////////////////////////////////////////
        private void button4_Click(object sender, EventArgs e)
        {
            byte dir1 = 0, dir2 = 0, dir3 = 0, dir4 = 0;
            int speed1 = 0, speed2 = 0, speed3 = 0, speed4 = 0;
            int S = 1;   //meter
            int t;

            // коэффициент скорости
            double k;
            try
            {
                k = Convert.ToDouble(textBox4.Text);
            }
            catch
            {
                if (textBox4.Text.IndexOf('.') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace('.', ','));
                }
                else if (textBox4.Text.IndexOf(',') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace(',', '.'));
                }
                else
                {
                    k = 1;
                    textBox4.Text = k.ToString();
                }
            }
            /////////////////////////////////////////////////////////////////


            if (radioButton16.Checked == true)
            {
                dir1 = 1;
            }
            if (radioButton13.Checked == true)
            {
                dir1 = 2;
            }

            if (radioButton7.Checked == true)
            {
                dir2 = 1;
            }
            if (radioButton8.Checked == true)
            {
                dir2 = 2;
            }

            if (wheels == 3)
            {
                if (radioButton9.Checked == true)
                {
                    dir3 = 1;
                    dir4 = 1;
                }
                if (radioButton10.Checked == true)
                {
                    dir3 = 2;
                    dir4 = 2;
                }
            }
            else
            {
                if (radioButton9.Checked == true)
                {
                    dir3 = 1;
                }
                if (radioButton10.Checked == true)
                {
                    dir3 = 2;
                }

                if (radioButton21.Checked == true)
                {
                    dir4 = 1;
                }
                if (radioButton22.Checked == true)
                {
                    dir4 = 2;
                }
            }


            try
            {
                if (wheels == 3)
                {
                    try
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace(',', '.')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace(',', '.')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                    }
                    catch
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace('.', ',')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace('.', ',')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                    }
                }
                else
                {
                    try
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace(',', '.')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace(',', '.')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox8.Text.Replace(',', '.')) * k);
                    }
                    catch
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace('.', ',')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace('.', ',')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox8.Text.Replace('.', ',')) * k);
                    }

                    
                }

                if (speed1 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 1 колеса");
                    return;
                }
                else if (speed2 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 2 колеса");
                    return;
                }
                else if (speed3 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 3 колеса");
                    return;
                }
                else if (speed4 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 4 колеса");
                    return;
                }

                

            }
            catch
            {
                MessageBox.Show("Неверный формат параметров", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            CommandSet(2, 125, dir1, speed1, dir2, speed2, dir3, speed3, dir4, speed4, 1);

            //t = (int)(1000 * S / ((speed1 / 60) * Math.PI * 0.16));

            t = int.Parse(textBox12.Text);

            if (t != 0)
            {
                Thread.Sleep(t);

                button2.PerformClick();
            }
        }

        ////////////////////////////////////////////////////////////////////////
        //кнопка стоп 3 колеса
        ////////////////////////////////////////////////////////////////////////
        private void button2_Click(object sender, EventArgs e)
        {
            byte dir1 = 1, dir2 = 1, dir3 = 1, dir4 = 1;
            int speed1 = 0, speed2 = 0, speed3 = 0, speed4 = 0;

            CommandSet(2, 125, dir1, speed1, dir2, speed2, dir3, speed3, dir4, speed4, 0);
        }

        ////////////////////////////////////////////////////////////////////////
        //кнопка реверс 3 колеса
        ////////////////////////////////////////////////////////////////////////
        private void button3_Click(object sender, EventArgs e)
        {
            byte dir1 = 0, dir2 = 0, dir3 = 0, dir4 = 0;
            int speed1 = 0, speed2 = 0, speed3 = 0, speed4 = 0;
            int S = 1;   //meter
            int t;

            // коэффициент скорости
            double k;
            try
            {
                k = Convert.ToDouble(textBox4.Text);
            }
            catch
            {
                if (textBox4.Text.IndexOf('.') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace('.', ','));
                }
                else if (textBox4.Text.IndexOf(',') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace(',', '.'));
                }
                else
                {
                    k = 1;
                    textBox4.Text = k.ToString();
                }
            }
            /////////////////////////////////////////////////////////////////
            // направление 1 колеса
            if (radioButton16.Checked == true)
            {
                dir1 = 2;
            }
            if (radioButton13.Checked == true)
            {
                dir1 = 1;
            }
            // направление 2 колеса
            if (radioButton7.Checked == true)
            {
                dir2 = 2;
            }
            if (radioButton8.Checked == true)
            {
                dir2 = 1;
            }

            if (wheels == 3)
            {
                // направление 3 колеса
                if (radioButton9.Checked == true)
                {
                    dir3 = 2;
                    dir4 = 2;
                }
                if (radioButton10.Checked == true)
                {
                    dir3 = 1;
                    dir4 = 1;
                }
            }
            else
            {
                // направление 3 колеса
                if (radioButton9.Checked == true)
                {
                    dir3 = 2;
                }
                if (radioButton10.Checked == true)
                {
                    dir3 = 1;
                }
                // направление 4 колеса
                if (radioButton21.Checked == true)
                {
                    dir4 = 2;
                }
                if (radioButton22.Checked == true)
                {
                    dir4 = 1;
                }
            }
            

            try
            {
                if (wheels == 3)
                {
                    try
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace(',', '.')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace(',', '.')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                    }
                    catch
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace('.', ',')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace('.', ',')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                    }
                }
                else
                {
                    try
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace(',', '.')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace(',', '.')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace(',', '.')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox8.Text.Replace(',', '.')) * k);
                    }
                    catch
                    {
                        speed1 = (int)(Convert.ToDouble(textBox7.Text.Replace('.', ',')) * k);
                        speed2 = (int)(Convert.ToDouble(textBox6.Text.Replace('.', ',')) * k);
                        speed3 = (int)(Convert.ToDouble(textBox3.Text.Replace('.', ',')) * k);
                        speed4 = (int)(Convert.ToDouble(textBox8.Text.Replace('.', ',')) * k);
                    }
                }

                if (speed1 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 1 колеса");
                    return;
                }
                else if (speed2 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 2 колеса");
                    return;
                }
                else if (speed3 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 3 колеса");
                    return;
                }
                else if (speed4 > 300)
                {
                    MessageBox.Show("Превышено значение скорости 4 колеса");
                    return;
                }

            }
            catch
            {
                MessageBox.Show("Неверный формат параметров", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            CommandSet(2, 125, dir1, speed1, dir2, speed2, dir3, speed3, dir4, speed4, 1);

            //t = (int)(1000 * S / ((speed1 / 60) * Math.PI * 0.16));
            t = int.Parse(textBox12.Text);

            if (t != 0)
            {
                Thread.Sleep(t);

                button2.PerformClick();
            }
        }

        

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] Command = new byte[2];   //переменная для отправки команд

            Command[0] = 2;
            Command[1] = 201;

            SendRF(Command);
            
            //int sleep = 500, sleep2 = 100;


            //// коэффициент скорости
            //double k;
            //try
            //{
            //    k = Convert.ToDouble(textBox4.Text);
            //}
            //catch
            //{
            //    if (textBox4.Text.IndexOf('.') != 0)
            //    {
            //        k = Convert.ToDouble(textBox4.Text.Replace('.', ','));
            //    }
            //    else if (textBox4.Text.IndexOf(',') != 0)
            //    {
            //        k = Convert.ToDouble(textBox4.Text.Replace(',', '.'));
            //    }
            //    else
            //    {
            //        k = 1;
            //        textBox4.Text = k.ToString();
            //    }
            //}
            ///////////////////////////////////////////////////////////////////


            //try
            //{

            //    sleep = (int)(sleep / k);
            //    if (port.IsOpen)
            //    {
            //        //forward
            //        CommandSet(2, 125, 1, (int)(151 * k), 1, (int)(241 * k), 1, 0, 2, (int)(392 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2,125, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            //        Thread.Sleep(sleep2);

            //        //right
            //        CommandSet(2, 125, 1, (int)(365 * k), 2, (int)(314 * k), 0, 0, 2, (int)(51 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2,125, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            //        Thread.Sleep(sleep2);

            //        //back
            //        CommandSet(2, 125, 2, (int)(151 * k), 2, (int)(241 * k), 0, 0, 1, (int)(392 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2, 125, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            //        Thread.Sleep(sleep2);

            //        //left
            //        CommandSet(2, 125, 2, (int)(365 * k), 1, (int)(314 * k), 0, 0, 1, (int)(51 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2, 125, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            //        Thread.Sleep(sleep2);


            //        ////turn left
            //        CommandSet(2, 125, 1, (int)(300 * k), 1, (int)(300 * k), 0, 0, 1, (int)(300 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2,125, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            //        Thread.Sleep(sleep2);

            //        ////turn right
            //        CommandSet(2, 125, 2, (int)(300 * k), 2, (int)(300 * k), 0, 0, 2, (int)(300 * k), 1);
            //        Thread.Sleep(sleep);
            //        CommandSet(2, 125, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            //        Thread.Sleep(sleep2);

            //    }
            //    else
            //    {
            //        textBox21.Text = "Not connect";
            //    }

            //}
            //catch
            //{
            //    MessageBox.Show("Неверный коэффициент");
            //    k = 1;
            //}
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                port.DiscardInBuffer();
            }
            else
            {
                textBox21.Text = "Установите соединение";
                //MessageBox.Show("Установите соединение");
                return;
            }

            byte[] Command = new byte[2];   //переменная для отправки команд
            Command[0] = 1;

            Command[1] = 20;                                 //start (0xA5, 0x20)

            DataClass.flag_start = true;

            SendRF(Command);

            button15.Enabled = false;


            ff = new Form2();
            ff.Show();

            ff.FormClosed+=(sender1, e1) =>
            {
                
                Command[0] = 1;

                Command[1] = 21;                                 //stop (0xA5, 0x25)

                DataClass.flag_start = false;

                SendRF(Command);

                button15.Enabled = true;
            };
        }

        private void button16_Click(object sender, EventArgs e)
        {
            double angle = 0;

            double x = 0, y = 0, w = 0, sp1, sp2, sp3, sp4;
                        
            // получение x и y с точкой и омега
            try
            {
                angle = double.Parse(textBox5.Text);
            }
            catch 
            {
                MessageBox.Show("Неверный формат угла");
            }

            x = Math.Cos(angle * Math.PI / 180);
            y = Math.Sin(angle * Math.PI / 180);
            

            // расчет скоростей колес
            double[] vec_v = new double[3] { x, y, 0 };     //вектор линейной скорости
            double[] vec_w = new double[3] { 0, 0, w };     //вектор угловой скорости вращения
            //double delta = 45 * Math.PI / 180;              //угол между векторами ??
            //double h = 0.0475;                              //радиус колеса

            
            //расчет скоростей каждого колеса
            sp1 = SpeedWheel(vec_v, vec_w, DataClass.alpha1, DataClass.r1);
            sp2 = SpeedWheel(vec_v, vec_w, DataClass.alpha2, DataClass.r2);
            sp3 = SpeedWheel(vec_v, vec_w, DataClass.alpha3, DataClass.r3);
            sp4 = SpeedWheel(vec_v, vec_w, DataClass.alpha4, DataClass.r4);

            //sp2 = (-10) * Dot_Vector(Sum_Vector(vec_v, Cross_Vector(vec_w, DataClass.r2)), DataClass.alpha2) / (Math.Sin(delta) * h);
            //sp3 = (-10) * Dot_Vector(Sum_Vector(vec_v, Cross_Vector(vec_w, DataClass.r3)), DataClass.alpha3) / (Math.Sin(delta) * h);
            //sp4 = (-10) * Dot_Vector(Sum_Vector(vec_v, Cross_Vector(vec_w, DataClass.r4)), DataClass.alpha4) / (Math.Sin(delta) * h);



            if (sp1 < 0)
            {
                sp1 = Math.Abs(sp1);
                radioButton13.Checked = true;
            }
            else
            {
                radioButton16.Checked = true;
            }

            if (sp2 < 0)
            {
                sp2 = Math.Abs(sp2);
                radioButton8.Checked = true;
            }
            else
            {
                radioButton7.Checked = true;
            }

            if (sp3 < 0)
            {
                sp3 = Math.Abs(sp3);
                radioButton10.Checked = true;
            }
            else
            {
                radioButton9.Checked = true;
            }

            if (sp4 < 0)
            {
                sp4 =Math.Abs(sp4);
                radioButton22.Checked = true;
            }
            else
            {
                radioButton21.Checked = true;
            }

            textBox7.Text = Math.Round(sp1, 3).ToString();
            textBox6.Text = Math.Round(sp2, 3).ToString();
            textBox3.Text = Math.Round(sp3, 3).ToString();
            textBox8.Text = Math.Round(sp4, 3).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            double t_max = 2;
            double step = 0.1;
            double length = 0, length_set = 1;

            try
            {
                length_set = Convert.ToDouble(textBox9.Text);
            }
            catch
            {
                if (textBox4.Text.IndexOf('.') != 0)
                {
                    length_set = Convert.ToDouble(textBox9.Text.Replace('.', ','));
                }
                else if (textBox9.Text.IndexOf(',') != 0)
                {
                    length_set = Convert.ToDouble(textBox9.Text.Replace(',', '.'));
                }
                else
                {
                    length_set = 1;
                    textBox9.Text = length_set.ToString();
                }
            }

            

            string topfx = "using System;using System.Windows.Forms;class MyFormulaClass{public double Calculate(double t){double c=";
            string mainfx_x, mainfx_y;
            string bottomfx = ";return c;}}";
            object formula_x = null;
            object formula_y = null;
            double x_temp, y_temp, x_temp_old, y_temp_old, delta_x, delta_y;

            

            #region Init Functions X and Y


            textBox10.Text = textBox10.Text.Replace(" ", "");
            mainfx_x = textBox10.Text;
            mainfx_x = mainfx_x.Replace("sin", "Math.Sin");
            mainfx_x = mainfx_x.Replace("cos", "Math.Cos");
            mainfx_x = mainfx_x.Replace("tan", "Math.Tan");
            mainfx_x = mainfx_x.Replace("asn", "Math.Asin");
            mainfx_x = mainfx_x.Replace("acs", "Math.Acos");
            mainfx_x = mainfx_x.Replace("atn", "Math.Atan");
            mainfx_x = mainfx_x.Replace("abs", "Math.Abs");
            mainfx_x = mainfx_x.Replace("lg", "Math.Log10");
            mainfx_x = mainfx_x.Replace("pow", "Math.Pow");
            mainfx_x = mainfx_x.Replace("pi", "Math.PI");
            mainfx_x = mainfx_x.Replace("e", "Math.E");
            mainfx_x = mainfx_x.Replace("sqrt", "Math.Sqrt");

            textBox11.Text = textBox11.Text.Replace(" ", "");
            mainfx_y = textBox11.Text;
            mainfx_y = mainfx_y.Replace("sin", "Math.Sin");
            mainfx_y = mainfx_y.Replace("cos", "Math.Cos");
            mainfx_y = mainfx_y.Replace("tan", "Math.Tan");
            mainfx_y = mainfx_y.Replace("asn", "Math.Asin");
            mainfx_y = mainfx_y.Replace("acs", "Math.Acos");
            mainfx_y = mainfx_y.Replace("atn", "Math.Atan");
            mainfx_y = mainfx_y.Replace("abs", "Math.Abs");
            mainfx_y = mainfx_y.Replace("lg", "Math.Log10");
            mainfx_y = mainfx_y.Replace("pow", "Math.Pow");
            mainfx_y = mainfx_y.Replace("pi", "Math.PI");
            mainfx_y = mainfx_y.Replace("e", "Math.E");
            mainfx_y = mainfx_y.Replace("sqrt", "Math.Sqrt");

            //Настройка компиляции
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerParameters cparams = new CompilerParameters();
            cparams.ReferencedAssemblies.Add("System.dll");
            cparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cparams.GenerateInMemory = true;
            //Компиляция
            CompilerResults result_x = null;
            CompilerResults result_y = null;
            result_x = provider.CompileAssemblyFromSource(cparams, topfx + mainfx_x + bottomfx);
            if (result_x.Errors.HasErrors)
            {
                //Ошибки компиляции
                MessageBox.Show("Формула: синтаксическая ошибка X");
                return;
            }
            else
            {
                Assembly assembly = result_x.CompiledAssembly;
                formula_x = assembly.CreateInstance("MyFormulaClass");
                if (formula_x == null)
                {
                    MessageBox.Show("Формула: ошибка выполнения");
                    return;
                    //Ошибки выполнения
                }

            }

            result_y = provider.CompileAssemblyFromSource(cparams, topfx + mainfx_y + bottomfx);
            if (result_y.Errors.HasErrors)
            {
                //Ошибки компиляции
                MessageBox.Show("Формула: синтаксическая ошибка Y");
                return;
            }
            else
            {
                Assembly assembly = result_y.CompiledAssembly;
                formula_y = assembly.CreateInstance("MyFormulaClass");
                if (formula_y == null)
                {
                    MessageBox.Show("Формула: ошибка выполнения");
                    return;
                    //Ошибки выполнения
                }

            }

            if ((formula_x != null) && (formula_y != null) )
            {

                length = 0;
                for (double i = step; ; i=Math.Round((i+step),1))
                {
                    x_temp_old = (double)formula_x.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_x, new object[] { i-step });
                    y_temp_old = (double)formula_y.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_y, new object[] { i-step });
                    
                    x_temp = (double)formula_x.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_x, new object[] { i });
                    y_temp = (double)formula_y.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_y, new object[] { i });

                    delta_x = x_temp - x_temp_old;
                    delta_y = y_temp - y_temp_old;

                    length += Math.Sqrt(delta_x * delta_x + delta_y * delta_y);

                    if (length >= length_set)
                    {
                        t_max = i;
                        break;
                    }

                }

                t = new double[(int)((t_max / step) + 1)];
                x = new double[(int)((t_max / step) + 1)];
                y = new double[(int)((t_max / step) + 1)];

                for (double i = 0, j = 0; i <= (t_max / step) ; i++, j += step)
                {
                    t[(int)i] = j;
                    x[(int)i] = (double)formula_x.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_x, new object[] { t[(int)i] });
                    y[(int)i] = (double)formula_y.GetType().InvokeMember("Calculate", BindingFlags.InvokeMethod, null, formula_y, new object[] { t[(int)i] });

                }

            }
            #endregion
            
            double scale = 20;

            gr_x =new Grafik(t, x, pictureBox1, pictureBox1.Size, Color.Red, scale, scale);
            gr_y = new Grafik(t, y, pictureBox2, pictureBox2.Size, Color.Red, scale, scale);

            gr_x.X_null = 10;
            gr_x.Y_null = pictureBox1.Height-10;
            gr_x.Step_of_grid = 1;
            gr_x.clear_bitmap();
            gr_x.Setka();
            gr_x.ris();


            gr_y.X_null = 10;
            gr_y.Y_null = pictureBox2.Height - 10;
            gr_y.Step_of_grid = 1;
            gr_y.clear_bitmap();
            gr_y.Setka();
            gr_y.ris();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //х с точкой и y с точкой
            double[] x_dot;
            double[] y_dot;

            try
            {
                //х с точкой и y с точкой
                x_dot = new double[x.Length - 1];
                y_dot = new double[x.Length - 1];
            }
            catch
            {
                MessageBox.Show("Нажмите кнопку Refresh");
                return;
            }

            //скорость каждого колеса
            double[] sp1 = new double[x.Length - 1];
            double[] sp2 = new double[x.Length - 1];
            double[] sp3 = new double[x.Length - 1];
            double[] sp4 = new double[x.Length - 1];

            //направление каждого колеса
            byte[] dir1 = new byte[x.Length - 1];
            byte[] dir2 = new byte[x.Length - 1];
            byte[] dir3 = new byte[x.Length - 1];
            byte[] dir4 = new byte[x.Length - 1];

            double[] v = new double[x.Length - 1];      //скорость
            double[] time = new double[x.Length - 1];   //время
            double[] s = new double[x.Length - 1];      //путь

            double[] vec_v = new double[3];
            double[] vec_w = new double[3] {0,0,0};

            //double length = 0, length_set = 1;

            double angle = 0;

            double k;
            try
            {
                k = Convert.ToDouble(textBox4.Text);
            }
            catch
            {
                if (textBox4.Text.IndexOf('.') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace('.', ','));
                }
                else if (textBox4.Text.IndexOf(',') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace(',', '.'));
                }
                else
                {
                    k = 1;
                    textBox4.Text = k.ToString();
                }
            }
            for (int i = 0; i < x_dot.Length; i++)
            {

                //расчет производных по х и у
                //if (checkBox2.Checked)
                //{
                //    x_dot[i] = (x[i + 1] - x[i]) / (t[i + 1] - t[i]);
                //    y_dot[i] = (y[i + 1] - y[i]) / (t[i + 1] - t[i]);
                //}
                ////расчет производных второй способ
                //else
                //{
                    angle = Math.Atan2(y[i + 1] - y[i], x[i + 1] - x[i]);
                    x_dot[i] = Math.Cos(angle);
                    y_dot[i] = Math.Sin(angle);
                //}
                
                // расчет вектора скорости. Не используется
                v[i] = Math.Sqrt(x_dot[i] * x_dot[i] + y_dot[i] * y_dot[i]);


                vec_v = new double[3] { x_dot[i], y_dot[i], 0 };

                //расчет скоростей каждого колеса
                sp1[i] = SpeedWheel(vec_v, vec_w, DataClass.alpha1, DataClass.r1) * k;
                sp2[i] = SpeedWheel(vec_v, vec_w, DataClass.alpha2, DataClass.r2) * k;
                sp3[i] = SpeedWheel(vec_v, vec_w, DataClass.alpha3, DataClass.r3) * k;
                sp4[i] = SpeedWheel(vec_v, vec_w, DataClass.alpha4, DataClass.r4) * k;

                //направление вращения колес
                if (sp1[i] < 0)
                {
                    sp1[i] = Math.Abs(sp1[i]);
                    dir1[i] = 2;
                }
                else
                {
                    dir1[i] = 1;
                }

                if (sp2[i] < 0)
                {
                    sp2[i] = Math.Abs(sp2[i]);
                    dir2[i] = 2;
                }
                else
                {
                    dir2[i] = 1;
                }

                if (sp3[i] < 0)
                {
                    sp3[i] = Math.Abs(sp3[i]);
                    dir3[i] = 2;
                }
                else
                {
                    dir3[i] = 1;
                }

                if (sp4[i] < 0)
                {
                    sp4[i] = Math.Abs(sp4[i]);
                    dir4[i] = 2;
                }
                else
                {
                    dir4[i] = 1;
                }

                //расчет времени движения на каждом участке при постоянной скорости
                s[i] = Math.Sqrt((x[i + 1] - x[i]) * (x[i + 1] - x[i]) + (y[i + 1] - y[i]) * (y[i + 1] - y[i]));

                //time[i] = s[i] / ((sp1[i] / 60) * Math.PI * 0.16);
                time[i] = (s[i] * 0.7) / (v[i] * k);
            }

            //ничего не делать если большие скорости
            for (int i = 0; i < sp1.Length; i++)
            {
                if (wheels == 3)
                {
                    if (sp1[i] > 300 || sp2[i] > 300 || sp3[i] > 300)
                    {
                        MessageBox.Show("Превышено допустимое значение скорости одного или нескольких колес");
                        return;
                    }
                }

                if (wheels == 4)
                {
                    if (sp1[i] > 300 || sp2[i] > 300 || sp3[i] > 300 || sp4[i] > 300)
                    {
                        MessageBox.Show("Превышено допустимое значение скорости одного или нескольких колес");
                        return;
                    }
                }
            } 

            //отправка роботу траектории если стоит галка
            //if (checkBox1.Checked)
            //{
                for (int i = 0; i < sp1.Length; i++)
                {
                    if (wheels == 3)
                    {
                        CommandSet(2, 125, dir1[i], (int)sp1[i], dir2[i], (int)sp2[i], dir3[i], (int)sp3[i], 0, 0, 1);
                    }
                    if (wheels == 4)
                    {
                        CommandSet(2, 125, dir1[i], (int)sp1[i], dir2[i], (int)sp2[i], dir3[i], (int)sp3[i], dir4[i], (int)sp4[i], 1);
                    }

                    //if (checkBox2.Checked)
                    //{
                    //    Thread.Sleep((int)(1000));
                    //}
                    //else
                    //{
                        Thread.Sleep((int)(time[i] * 1000)-100);
                    //}

                    
                }

                button2.PerformClick();
                Thread.Sleep(100);
                button2.PerformClick();
                Thread.Sleep(100);
                button2.PerformClick();
            //}

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag_x)
            {
                Point mousePos = new Point(e.X, e.Y);
                try
                {
                    
                    gr_x.clear_bitmap();

                    gr_x.X_null = (mousePos.X - oldMouse_x.X) + gr_x.X_null;
                    gr_x.Y_null = (mousePos.Y - oldMouse_x.Y) + gr_x.Y_null;

                    gr_x.Setka();
                    gr_x.ris();

                }
                catch { }
                oldMouse_x = new Point(e.X, e.Y);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            flag_x = true;
            oldMouse_x = new Point(e.X, e.Y);
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            flag_x = false;
            pictureBox1.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag_y)
            {
                Point mousePos = new Point(e.X, e.Y);
                try
                {

                    gr_y.clear_bitmap();

                    gr_y.X_null = (mousePos.X - oldMouse_y.X) + gr_y.X_null;
                    gr_y.Y_null = (mousePos.Y - oldMouse_y.Y) + gr_y.Y_null;

                    gr_y.Setka();
                    gr_y.ris();

                }
                catch { }
                oldMouse_y = new Point(e.X, e.Y);
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            flag_y = true;
            oldMouse_y = new Point(e.X, e.Y);
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            flag_y = false;
            pictureBox2.Focus();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte[] Command = new byte[2];   //переменная для отправки команд

            Command[0] = 2;
            Command[1] = 200;

            SendRF(Command);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                j = device.CurrentJoystickState;
            }
            catch
            {                
                button17.PerformClick();
                return;
            }

            string info = "";

            // Собираем информацию о кнопках
            byte[] buttons = j.GetButtons();
            for (int i = 0; i < buttons.Length; i++)
            {
                // Узнаем какие из кнопок нажаты на данный момент
                if (buttons[i] != 0)
                {
                    info += "Button: " + i + " ";
                }
            }

            //double ugol;
            //double xx = j.X;
            //double yy = -j.Y;
            //double ugolc;
            //double absc;


            textBox13.Text = j.X.ToString();
            textBox14.Text = (-j.Y).ToString();


            int sp = 50;
            // кнопки
            if (buttons[2] == 128)
            {
                

                textBox15.Text = "3";
                return;
            }
            else if (buttons[3] == 128)
            {
                if (wheels == 3)
                {
                    CommandSet(2, 125, 1, sp, 1, sp, 1, sp, 1, sp, 1);
                }
                if (wheels == 4)
                {
                    CommandSet(2, 125, 1, sp, 2, sp, 1, sp, 2, sp, 1);
                }

                

                textBox15.Text = "4";
                return;
            }
            else if (buttons[4] == 128)
            {
                textBox15.Text = "5";
                return;
            }
            else if (buttons[5] == 128)
            {
                if (wheels == 3)
                {
                    CommandSet(2, 125, 2, sp, 2, sp, 2, sp, 2, sp, 1);
                }
                if (wheels == 4)
                {
                    CommandSet(2, 125, 2, sp, 1, sp, 2, sp, 1, sp, 1);
                }

                textBox15.Text = "6";
                return;
            }
            else if (buttons[6] == 128)
            {
                textBox15.Text = "7";
                return;
            }
            else
            {
                textBox15.Text = "";
            }

            
            


            // шум
            if (Math.Abs(j.X) <= 10 && Math.Abs(j.Y) <= 10)
            {
                button2.PerformClick();
                return;
            }

            // расчет
            double k;
            try
            {
                k = Convert.ToDouble(textBox4.Text);
            }
            catch
            {
                if (textBox4.Text.IndexOf('.') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace('.', ','));
                }
                else if (textBox4.Text.IndexOf(',') != 0)
                {
                    k = Convert.ToDouble(textBox4.Text.Replace(',', '.'));
                }
                else
                {
                    k = 1;
                    textBox4.Text = k.ToString();
                }
            }

            double[] vec_v = new double[3];
            double[] vec_w = new double[3] { 0, 0, 0 };
            
            //скорость каждого колеса
            double sp1, sp2, sp3, sp4;
            //направление каждого колеса
            byte dir1, dir2, dir3, dir4;

           
            vec_v = new double[3] { (double)-j.X/120, (double)-j.Y/120, 0 };

            //расчет скоростей каждого колеса
            sp1 = SpeedWheel(vec_v, vec_w, DataClass.alpha1, DataClass.r1) * k;
            sp2 = SpeedWheel(vec_v, vec_w, DataClass.alpha2, DataClass.r2) * k;
            sp3 = SpeedWheel(vec_v, vec_w, DataClass.alpha3, DataClass.r3) * k;
            sp4 = SpeedWheel(vec_v, vec_w, DataClass.alpha4, DataClass.r4) * k;

            //направление вращения колес
            if (sp1 < 0)
            {
                sp1 = Math.Abs(sp1);
                dir1 = 2;
            }
            else
            {
                dir1 = 1;
            }

            if (sp2 < 0)
            {
                sp2 = Math.Abs(sp2);
                dir2 = 2;
            }
            else
            {
                dir2 = 1;
            }

            if (sp3 < 0)
            {
                sp3 = Math.Abs(sp3);
                dir3 = 2;
            }
            else
            {
                dir3 = 1;
            }

            if (sp4 < 0)
            {
                sp4 = Math.Abs(sp4);
                dir4 = 2;
            }
            else
            {
                dir4 = 1;
            }

            //ничего не делать если большие скорости
            
            if (wheels == 3)
            {
                if (sp1 > 300 || sp2 > 300 || sp3 > 300)
                {
                    //MessageBox.Show("Превышено допустимое значение скорости одного или нескольких колес");
                    return;
                }
            }

            if (wheels == 4)
            {
                if (sp1 > 300 || sp2 > 300 || sp3 > 300 || sp4 > 300)
                {
                    //MessageBox.Show("Превышено допустимое значение скорости одного или нескольких колес");
                    return;
                }
            }
            

            //отправка роботу 
            
            
            if (wheels == 3)
            {
                CommandSet(2, 125, dir1, (int)sp1, dir2, (int)sp2, dir3, (int)sp3, dir3, (int)sp3, 1);
            }
            if (wheels == 4)
            {
                CommandSet(2, 125, dir1, (int)sp1, dir2, (int)sp2, dir3, (int)sp3, dir4, (int)sp4, 1);
            }

                
               


            


            
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            

            if (timer1.Enabled == false)
            {
                button17.Text = "Joystick OFF";

                //////////////////////////////////////////////////
                ///Блок для инициализации джойстика
                /////////////////////////////////////////////////
                foreach (DeviceInstance instance in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
                {
                    device = new Device(instance.ProductGuid);
                    // Background - флаг, говорит о том, что данные от руля будут поступать даже в неактивное окно
                    // NonExclusive - говорит о том, что игровой контроллер могут использовать и другие приложения
                    device.SetCooperativeLevel(null, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    // j = device.CurrentJoystickState;
                    // Зададим дополнительные параметры
                    foreach (DeviceObjectInstance doi in device.Objects)
                    {
                        // Проверяем есть ли на устройстве что-нибудь поворачивающееся
                        if ((doi.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                        {
                            // Задаем минимальное и максимальное значение угла поворота
                            device.Properties.SetRange(
                                       ParameterHow.ById,
                                       doi.ObjectId,
                                       new InputRange(-90, 90));
                        }
                    }

                    // Применяем настройки
                    device.Acquire();
                }

                timer1.Enabled = true;
                
            }

            else if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
                button17.Text = "Joystick ON";

                textBox13.Text = "OFF";
                textBox14.Text = "OFF";
            }



        }

        private void button18_Click(object sender, EventArgs e)
        {
            byte kp, kd, ki, dt;

            kp = byte.Parse(textBox16.Text);
            kd = byte.Parse(textBox17.Text);
            ki = byte.Parse(textBox18.Text);
            dt = byte.Parse(textBox20.Text);

            byte[] Command = new byte[6];   //переменная для отправки команд

            Command[0] = 2;
            Command[1] = 150;
            Command[2] = kp;
            Command[3] = kd;
            Command[4] = ki;
            Command[5] = dt;


            SendRF(Command);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            byte[] Command = new byte[2];   //переменная для отправки команд

            Command[0] = 2;
            Command[1] = 151;

            SendRF(Command);
        }

        

        private void button21_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int kp = 10, kd = 10, ki = 10;

            if (port.IsOpen)
            {
                textBox7.Text = "20";
                textBox4.Text = "1";
                textBox12.Text = "0";
                textBox6.Text = "0";
                textBox3.Text = "0";
                textBox8.Text = "0";

                groupBox1.Update();
                radioButton2.Checked = true;

                for (kd = 190; kd <= 190; kd += 10)
                {
                    for (kp = 170; kp <= 170; kp += 10)
                    {
                        for (ki = 160; ki <= 160; ki += 10)
                        {
                            textBox16.Text = kp.ToString();
                            textBox17.Text = kd.ToString();
                            textBox18.Text = ki.ToString();

                            textBox16.Update();
                            textBox17.Update();
                            textBox18.Update();

                            button18.PerformClick();
                            Thread.Sleep(200);

                            button19.PerformClick();
                            Thread.Sleep(200);

                            button4.PerformClick();
                            Thread.Sleep(5500);

                            button2.PerformClick();
                            Thread.Sleep(700);

                            button2.PerformClick();
                            Thread.Sleep(700);

                            button2.PerformClick();
                            Thread.Sleep(700);

                            if (worker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }

                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                textBox19.Text = "Canceled!";
                CommandSet(2, 125, 0, 0, 0, 0, 0, 0, 0, 0, 0);   //Stop
            }
            else if (e.Error != null)
            {
                textBox19.Text = "Error: " + e.Error.Message;
            }
            else
            {
                textBox19.Text = "Done!";
            }

            StreamWriter sw = new StreamWriter("1.txt", false);

            sw.Write(textBox1.Text);

            sw.Close();
        }
        

        
    }

    

}
