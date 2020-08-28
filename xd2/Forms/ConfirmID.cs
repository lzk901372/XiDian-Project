using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace xd2
{
    public partial class ConfirmID : Form
    {
        /// <summary>
        /// 传递经过验证的证件数据
        /// </summary>
        /// <param name="input">各类检测出的证件数据</param>
        /// <param name="flag">当检测身份证时，flag为1，否则为0</param>
        /// <returns>返回为一个含有各类证件数据的string数组</returns>
        public delegate string[] getinfo(string[] input, int flag);
        public event getinfo GetInfo;

        bool isTestConfirm = false;
        bool ConfirmPass = false;

        public bool getConfirmPass => ConfirmPass;

        string conn_str = "server=127.0.0.1;" +
                "port=3306;" +
                "user=root;" +
                "password=901372liao;" +
                "database=info";

        string id, name;
        string testid, stunum;

        TextBox textBox3;
        Label label3;

        public ConfirmID(string[] infos)
        {
            InitializeComponent();
            if (infos.Length == 2)
            {
                textBox1.Text = infos[0].Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
                textBox2.Text = infos[1].Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
            }
            else
            {
                isTestConfirm = true;
                GenerateNewComponents(this, out textBox3, out label3);

                textBox3.Enabled = label3.Enabled = true;
                textBox3.Visible = label3.Visible = true;

                label2.Text = "准考证号";
                textBox2.Text = infos[1].Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
                textBox1.Text = infos[0].Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
                textBox3.Text = infos[2].Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
            }
        }

        private void GenerateNewComponents(Control parent,
            out TextBox tb1,
            out Label l1)
        {
            TextBox textBox3 = new TextBox();
            //textBox3：准考证号，textBox4：学号
            Label label3 = new Label();
            //label3：准考证号，label4：学号
            textBox3.Parent = label3.Parent = parent;

            textBox3.Width = textBox1.Width;
            textBox3.Height = 31;
            textBox3.Location = new Point(textBox1.Location.X, textBox1.Location.Y + 35);
            textBox3.Font = textBox1.Font;

            label3.Font = label1.Font;
            label3.Text = "学号";
            label3.Width = label1.Width;
            label3.Height = 18;
            label3.Location = new Point(label1.Location.X, label1.Location.Y + 35);

            parent.Height += 31;
            button1.Location = new Point(button1.Location.X, button1.Location.Y + 31);

            tb1 = textBox3;
            l1 = label3;
        }

        private void ConfirmID_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            id = textBox1.Text;

            MySqlConnection conn = new MySqlConnection(conn_str);
            conn.Open();

            if (!isTestConfirm)
            {
                name = textBox2.Text;

                string comm_str = "select * from tester_idinfo where tester_id=@id and tester_name=@name";
                MySqlCommand comm = new MySqlCommand(comm_str, conn);
                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@name", name);

                MySqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    comm_str = "select tester_avatarPic from tester_avatar where tester_id=@id";
                    comm = new MySqlCommand(comm_str, conn);
                    comm.Parameters.AddWithValue("@id", id);

                    MySqlDataReader reader2 = comm.ExecuteReader();
                    string[] texts;
                    if (reader2.Read())
                    {
                        string path = @"D:\C# Work\xd2\xd2\Headpic\" + reader2.GetString("tester_avatarPic") + ".jpg";
                        texts = new string[3] { textBox1.Text, textBox2.Text, path };
                    }
                    else
                    {
                        texts = new string[3] { textBox1.Text, textBox2.Text, string.Empty };
                    }
                    GetInfo(texts, 1);
                    ConfirmPass = true;

                    reader2.Close();
                }
                else
                {
                    MessageBox.Show("系统中不存在该身份证号/姓名的匹配组合，请检查！",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    textBox2.Focus();
                    reader.Close();
                }
                comm.Dispose();
            }
            else
            {
                testid = textBox2.Text; stunum = textBox3.Text;

                string comm_str = "select * from tester_testinfo where tester_testId=@testid and tester_id=@id and tester_stuNum=@num";
                MySqlCommand comm = new MySqlCommand(comm_str, conn);
                comm.Parameters.AddWithValue("@testid", testid);
                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@num", stunum);

                MySqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    string[] texts = new string[3] { testid, id, stunum };
                    GetInfo(texts, 0);
                    ConfirmPass = true;
                }
                else
                {
                    MessageBox.Show("系统中不存在以上信息的匹配组合，请检查！",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    textBox2.Focus();
                }
                reader.Close();
                comm.Dispose();
            }
            conn.Close(); conn.Dispose();
        }
    }
}
