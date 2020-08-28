using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xd2.Forms
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            label1.Text = "加载中";
            int ori_x = label1.Location.X;

            Thread loading = new Thread(() =>
            {
                int cnt = 0;
                while (true)
                {
                    label1.Location = new Point(label1.Location.X - 4, label1.Location.Y);
                    label1.Text += ".";
                    Thread.Sleep(500);
                    if ((++cnt) % 3 == 0)
                    {
                        label1.Text = "加载中";
                        label1.Location = new Point(ori_x, label1.Location.Y);
                    }
                }
            });
            loading.Start();
        }
    }
}
