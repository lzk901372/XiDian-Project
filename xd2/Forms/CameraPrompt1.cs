using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace xd2
{
    public partial class CameraPrompt1 : Form
    {
        public CameraPrompt1()
        {
            InitializeComponent();
        }

        private void CameraPrompt1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            button1.Enabled = false;

            Thread waitTime = new Thread(() =>
              {
                  int Time = 1;//20;
                  for (int i = Time; i >= 0; i--)
                  {
                      button1.Text = "阅读提示\n等待" + i.ToString() + "s";
                      Thread.Sleep(1000);
                  }
                  button1.Enabled = true;
                  button1.Text = "确定";
                  button1.Focus();
              });
            waitTime.Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
