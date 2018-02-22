using System;
using System.Drawing;
using System.Windows.Forms;


class Grafik
{
    public double[] x, y;
    private double scale_x, scale_y;  //Масштаб
    public Graphics graph;
    public Pen myPen;
    public Pen myPen3 = new Pen(System.Drawing.Color.Chocolate);
    public Size size;
    int x_null = 10, y_null;
    public Bitmap bp;
    public PictureBox p;
    public double cena_del = 10;

    public double Step_of_grid
    {
        set { cena_del = value; }
        get { return cena_del; }
    }

    public double[] X
    {
        set { x = value; }
        get { return x; }
    }
    public double[] Y
    {
        set { y = value; }
        get { return y; }
    }

    public double Scale_x
    {
        set { scale_x = value; }
        get { return scale_x; }
    }
    public double Scale_y
    {
        set { scale_y = value; }
        get { return scale_y; }
    }

    public int X_null
    {
        set { x_null = value; }
        get { return x_null; }
    }
    public int Y_null
    {
        set { y_null = value; }
        get { return y_null; }
    }

    public Grafik(double[] x, double[] y, PictureBox p, Size size, Color col, double scale_x, double scale_y)
    {
        this.x = new double[x.Length];
        this.y = new double[y.Length];
        this.x = x;
        this.y = y;
        this.p = p;
        this.myPen = new Pen(col,2);
        this.size = size;
        this.scale_x = scale_x;
        this.scale_y = scale_y;
        this.x_null = size.Width / 2;
        this.y_null = size.Height / 2;

        bp = new Bitmap(size.Width, size.Height);
        graph = Graphics.FromImage(bp);

    }



    private float m_x(double a)
    {
        float b;
        b = Convert.ToInt32(a * scale_x + x_null);
        return b;
    }

    private float m_y(double a)
    {
        float b;
        b = Convert.ToInt32(a * (-1) * scale_y + y_null);
        return b;
    }
    public void ris()
    {
        if (p.Image != null)
        {
            bp = (Bitmap)p.Image;
        }

        graph = Graphics.FromImage(bp);

        for (int i = 0; i < x.Length - 1; i++)
        {
            graph.DrawLine(myPen, m_x(x[i]), m_y(y[i]), m_x(x[i + 1]), m_y(y[i + 1]));
        }
        p.Image = bp;
    }

    public void points(int r, bool on_x, bool on_y)
    {
        SolidBrush b = new SolidBrush(Color.Red);

        float[] xx = new float[x.Length];
        float[] yy = new float[y.Length];

        for (int i = 0; i < x.Length - 1; i++)
        {
            if (on_x == false)
            {
                yy[i] = m_y(y[i]);
            }
            else
            {
                yy[i] = m_y(-y[i]);
            }

            if (on_y == false)
            {
                xx[i] = m_x(x[i]);
            }
            else
            {
                xx[i] = m_x(-x[i]);
            }

            graph.FillEllipse(b, xx[i], yy[i], r, r);
        }


    }

    private void delenie(Pen myPen, double x1, double x2, double y1, double y2)
    {
        x1 = x1 * scale_x + size.Width / 2;
        x2 = x2 * scale_x + size.Width / 2;
        y1 = y1 * scale_y + size.Height / 2;
        y2 = y2 * scale_y + size.Height / 2;
        graph.DrawLine(myPen, Convert.ToInt32(x1), Convert.ToInt32(y1), Convert.ToInt32(x2), Convert.ToInt32(y2));
        p.Image = bp;
    }

    public void Setka()
    {
        Pen myPen4 = new Pen(System.Drawing.Color.DarkBlue);
        Pen myPen2 = new Pen(System.Drawing.Color.LightBlue);

        //сетка по оси Х
        for (double xx = 0; ; xx += cena_del)
        {
            //в положительную сторону
            if (m_x(xx) < size.Width)
            {
                if (xx % (cena_del*10) == 0)
                {
                    graph.DrawLine(myPen4, m_x(xx), 0, m_x(xx), size.Height);
                }
                else
                {
                    graph.DrawLine(myPen2, m_x(xx), 0, m_x(xx), size.Height);
                }
            }
            else
            {
                if (x_null <= size.Width / 2)
                {
                    break;
                }
            }


            //в отрицательную сторону
            if (m_x(-xx) > 0)
            {
                if (xx % (cena_del*10) == 0)
                {
                    graph.DrawLine(myPen4, m_x(-xx), 0, m_x(-xx), size.Height);
                }
                else
                {
                    graph.DrawLine(myPen2, m_x(-xx), 0, m_x(-xx), size.Height);
                }

            }
            else
            {
                if (x_null > size.Width / 2)
                {
                    break;
                }
            }
        }


        //сетка по оси У
        for (double yy = 0; ; yy += cena_del)
        {
            if (m_y(yy) > 0)
            {
                if (yy % (cena_del*10) == 0)
                {
                    graph.DrawLine(myPen4, 0, m_y(yy), size.Width, m_y(yy));
                }
                else
                {
                    graph.DrawLine(myPen2, 0, m_y(yy), size.Width, m_y(yy));   //в положительную сторону
                }
            }
            else
            {
                if (y_null >= size.Height / 2)
                {
                    break;
                }
            }


            if (m_y(-yy) < size.Height)
            {
                if (yy % (cena_del * 10) == 0)
                {
                    graph.DrawLine(myPen4, 0, m_y(-yy), size.Width, m_y(-yy));
                }
                else
                {
                    graph.DrawLine(myPen2, 0, m_y(-yy), size.Width, m_y(-yy));   //в отрицательную сторону
                }
            }
            else
            {
                if (y_null < size.Height / 2)
                {
                    break;
                }
            }
        }

        Osi();
    }

    public void Osi()
    {
        graph.DrawLine(new Pen(Color.Gray, 2), 0, y_null, size.Width, y_null);
        graph.DrawLine(new Pen(Color.Gray, 2), x_null, 0, x_null, size.Height);
        p.Image = bp;
    }

    public void clear_bitmap()
    {
        graph.Clear(Color.White);
        p.Image = bp;
    }

    //масштабирование вращением колеса
    //движение графика от мышки

    /*

     //событие для picturebox
     this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
    
    private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            try
            {
                gr1.clear_bitmap();

                if (gr1.Mas_x < 100)
                {
                    gr1.Mas_x += 0.1;
                    gr1.Mas_y += 0.1;

                    gr2.Mas_x += 0.1;
                    gr2.Mas_y += 0.1;

                    gr3.Mas_x += 0.1;
                    gr3.Mas_y += 0.1;


                    gr1.Setka();
                    gr1.ris();
                    gr2.ris();
                    gr3.ris();
                }
            }
            catch { }
        }
        else
        {
            try
            {
                if (gr1.Mas_y > 0.2)
                {
                    gr1.clear_bitmap();

                    gr1.Mas_x -= 0.1;
                    gr1.Mas_y -= 0.1;

                    gr2.Mas_x -= 0.1;
                    gr2.Mas_y -= 0.1;

                    gr3.Mas_x -= 0.1;
                    gr3.Mas_y -= 0.1;

                    gr1.Setka();
                    gr1.ris();
                    gr2.ris();
                    gr3.ris();
                }
            }
            catch { }
        }
    }

    private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
        flag = true;
        oldMouse = new Point(e.X, e.Y);

    }

    private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
    {
        if (flag)
        {
            Point mousePos = new Point(e.X, e.Y);
            try
            {
                gr1.clear_bitmap();

                gr1.X_null = (mousePos.X - oldMouse.X) + gr1.X_null;
                gr1.Y_null = (mousePos.Y - oldMouse.Y) + gr1.Y_null;

                gr2.X_null = (mousePos.X - oldMouse.X) + gr2.X_null;
                gr2.Y_null = (mousePos.Y - oldMouse.Y) + gr2.Y_null;

                gr3.X_null = (mousePos.X - oldMouse.X) + gr3.X_null;
                gr3.Y_null = (mousePos.Y - oldMouse.Y) + gr3.Y_null;

                gr1.Setka();
                gr1.ris();
                gr2.ris();
                gr3.ris();
            }
            catch { }
            oldMouse = new Point(e.X, e.Y);
        }
    }
    private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
    {
        flag = false;
    }
    */
}