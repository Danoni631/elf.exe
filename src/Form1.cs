using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDISharp
{
    public partial class Form1 : Form
    {
        private static Random random = new Random();
        private static int screenW = Screen.PrimaryScreen.Bounds.Width;
        private static int screenH = Screen.PrimaryScreen.Bounds.Height;
        private static int redrawCounter;
        private static int codcod;
        private static int ballWidth = 401;
        private static int ballHeight = 174;
        private static int ballPosX = random.Next(screenW - ballWidth);
        private static int ballPosY = random.Next(screenH - ballHeight);
        private static int moveStepX = 4;
        public Form1()
        {
            InitializeComponent();
            ballPosX = random.Next(screenW - ballWidth);
            ballPosY = random.Next(screenH - ballHeight);
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Location = new Point(ballPosX, ballPosY);
            ballPosX = random.Next(-12, 13);
            ballPosY = random.Next(-12, 13);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            redrawCounter += 1;
            int cc = redrawCounter;
            codcod += 2;
            int cod = codcod;
            Random r = new Random();
            this.BackColor = Color.FromArgb(255, 255, 255);
            label1.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            label2.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            ballPosX = random.Next(12);
            ballPosY = random.Next(12);

            if (ballPosX < 0 || ballPosX > screenW - ballWidth)
            {
                moveStepX = -moveStepX;
            }
        }
    }
}
