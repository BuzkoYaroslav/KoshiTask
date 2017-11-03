using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using library;
using System.Threading;

namespace KoshiTask
{ 
    public partial class Form1 : Form
    {
        private static MultiMathFunction func = (Math.E ^ new ArgumentFunction(1.0, 1)) - 2.0 / new ArgumentFunction(1.0, 0);
        private static MathFunction accurate = -new LnFunction(1.0, 2 * new LnFunction(1.0, new XFunction(1.0)) + 2);
        private static library.KoshiTask task = new library.KoshiTask(func,
            new KeyValuePair<double, double>(1, -Math.Log(2)),
            new KeyValuePair<double, double>(1, 2));
        private const int stepCount = 10;
        private const double step = 0.1;

        private KeyValuePair<double, double>[] rkAutomatic;
        private KeyValuePair<double, double>[] rkStatic;
        private KeyValuePair<double, double>[] adamsMethod;

        private Mutex rkAutoMutex = new Mutex();
        private Mutex rkStatMutex = new Mutex();
        private Mutex adamsMutex = new Mutex();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = task.ToString();
            new Thread(() =>
            {
                rkAutoMutex.WaitOne();
                rkAutomatic = RungeKuttaMethod.Solve(task);
                rkAutoMutex.ReleaseMutex();
            }).Start();
            new Thread(() =>
            {
                rkStatMutex.WaitOne();
                rkStatic = RungeKuttaMethod.Solve(task, stepCount);
                rkStatMutex.ReleaseMutex();
            }).Start();
            new Thread(() =>
            {
                adamsMutex.WaitOne();
                adamsMethod = AdamsMethod.Solve(task, step);
                adamsMutex.ReleaseMutex();
            }).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rkAutoMutex.WaitOne();

            new Table(accurate, rkAutomatic).Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rkStatMutex.WaitOne();

            new Table(accurate, rkStatic).Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            adamsMutex.WaitOne();

            new Table(accurate, adamsMethod).Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rkAutoMutex.WaitOne();

            new Graph(accurate, rkAutomatic).Show();

            rkAutoMutex.ReleaseMutex();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rkStatMutex.WaitOne();

            new Graph(accurate, rkStatic).Show();

            rkStatMutex.ReleaseMutex();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            adamsMutex.WaitOne();

            new Graph(accurate, adamsMethod).Show();

            adamsMutex.ReleaseMutex();
        }
    }
}
