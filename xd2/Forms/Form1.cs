using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using xd2.Internal_Classes;

namespace xd2
{
    using Point = OpenCvSharp.Point;
    public partial class Form1 : Form
    {
        VideoCapture vc = new VideoCapture(0);
        Mat frame = new Mat();
        Mat avatarFromDB = new Mat();
        bool isStopClick = false;
        bool f_finished = false, s_finished = false, t_finished = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            idStatus.ForeColor = idSelect.ForeColor;
            testIDStatus.ForeColor = testIDSelect.ForeColor;
            faceStatus.ForeColor = faceSelect.ForeColor;

            Thread timer = new Thread(() =>
              {
                  while (true)
                  {
                      DateTime dt = DateTime.Now;
                      timeLabel.Text = dt.Year.ToString() + "年" +
                      dt.Month.ToString() + "月" +
                      dt.Day.ToString() + "日 " +
                      (dt.Hour < 10 ? "0" + dt.Hour.ToString() : dt.Hour.ToString()) + ":" +
                      (dt.Minute < 10 ? "0" + dt.Minute.ToString() : dt.Minute.ToString()) + ":" +
                      (dt.Second < 10 ? "0" + dt.Second.ToString() : dt.Second.ToString());
                      Thread.Sleep(1000);
                  }
              });
            timer.Start();
            timer.IsBackground = true;
        }

        private void openCameraButton_Click(object sender, EventArgs e)
        {
            if (idSelect.Checked && !f_finished) IDRecognition();
            else if (testIDSelect.Checked/* && f_finished && !s_finished*/) TestIDRecognition();
            else if (faceSelect.Checked/* && f_finished && s_finished && !t_finished*/) FaceRecognition();
        }

        private Mat openCamera()
        {
            vc = new VideoCapture(0);
            vc.Set(VideoCaptureProperties.FrameHeight, 720);
            vc.Set(VideoCaptureProperties.FrameWidth, 1280);
            vc.Set(VideoCaptureProperties.Sharpness, 50);

            Mat mat0 = new Mat();
            int sleeptime = (int)Math.Round(1000 / vc.Fps);
            isStopClick = false;

            if (vc.IsOpened())
            {
                while (true)
                {
                    Mat mat = new Mat();
                    vc.Read(mat);
                    Cv2.Flip(mat, mat, FlipMode.Y);
                    mat0 = mat;
                    if (isStopClick) break;

                    cameraPic.BackgroundImage = mat.ToBitmap();
                    Cv2.WaitKey(sleeptime);
                    mat.Release();
                }
                vc.Dispose();
            }
            return mat0;
        }

        private void IDRecognition()
        {
            if (new CameraPrompt1().ShowDialog() == DialogResult.Cancel) return;

            Mat mat0 = openCamera();
            if (mat0.Empty()) return;

            //Mat input = Cv2.ImRead(@"C:\Users\Saxon\Desktop\保研\课题\相机采集\5.jpg", ImreadModes.Unchanged);
            //Cv2.Flip(input, input, FlipMode.Y);
            //Cv2.Resize(input, input, new OpenCvSharp.Size(input.Cols / 1.2, input.Height / 1.2));
            //PictureProcess pp = new PictureProcess(input);

            PictureProcess pp = new PictureProcess(mat0, PictureProcess.CardRecognitionFlags.IDRecognition);
            Mat prepro_img_4info = pp.Preprocess(pp.OriginalMat,
                PictureProcess.CardRecognitionFlags.IDRecognition,
                PictureProcess.InfoTypeDetectionFlags.InfoDetection);
            ArrayList idAndName = pp.GetTextsMatsFromBoxes4ID(pp.GetArea(prepro_img_4info));
            {
                bool hasID = ((Mat)idAndName[0]).Empty();
                bool hasName = ((Mat)idAndName[1]).Empty();
                if (hasID || hasName)
                {
                    MessageBox.Show("没有找到" +
                        (hasID ? "身份证号" : "") +
                        ((hasID && hasName) ? "以及" : "") +
                        (hasName ? "姓名" : "") + "，请重试", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Mat prepro_img_4photo = pp.Preprocess(pp.OriginalMat,
                PictureProcess.CardRecognitionFlags.IDRecognition,
                PictureProcess.InfoTypeDetectionFlags.PhotoDetection);
            ArrayList photo = pp.GetAvatarMatsFromBoxes4ID(/*prepro_img_4photo, */pp.GetArea(prepro_img_4photo));

            Mat id_mat = (Mat)idAndName[0],
                name_mat = (Mat)idAndName[1],
                photo_mat = (Mat)photo[0];

            string[] texts = new string[2];
            Image photo_image = photo_mat.ToBitmap();

            if (GetID(id_mat, "eng", out texts[0])
                && GetID(name_mat, "chi_sim", out texts[1]))
            {
                ConfirmID confirmid = new ConfirmID(texts);
                confirmid.GetInfo += getCorrectID;
                if (confirmid.ShowDialog() == DialogResult.OK &&
                    confirmid.getConfirmPass)
                {
                    infoAvatar.Image = photo_image;
                    f_finished = true;
                    //idSelect.Enabled = false;
                    //idStatus.Text = "已完成";
                    //idStatus.ForeColor = idSelect.ForeColor;
                    //testIDSelect.Enabled = true;
                    //testIDSelect.Checked = true;
                    //testIDStatus.ForeColor = testIDSelect.ForeColor;
                    confirmid.Close();
                }
            }
        }

        private void TestIDRecognition()
        {
            if (new CameraPrompt1().ShowDialog() == DialogResult.Cancel) return;

            Mat mat0 = openCamera();
            if (mat0.Empty()) return;

            PictureProcess pp = new PictureProcess(mat0, PictureProcess.CardRecognitionFlags.TestPermissionRecognition);
            Mat prepro_img_4info = pp.Preprocess(pp.OriginalMat,
                PictureProcess.CardRecognitionFlags.TestPermissionRecognition,
                PictureProcess.InfoTypeDetectionFlags.InfoDetection);
            ArrayList infos = pp.GetTextsMatsFromBoxes4Test(pp.GetArea(prepro_img_4info));
            {
                bool hasTestID = ((Mat)infos[0]).Empty();
                bool hasID = ((Mat)infos[1]).Empty();
                bool hasStuNum = ((Mat)infos[2]).Empty();
                if (hasID || hasID || hasStuNum)
                {
                    MessageBox.Show("没有找到以下信息：\n" +
                        (hasTestID ? "准考证号\n" : "") +
                        (hasID ? "身份证号\n" : "") +
                        (hasStuNum ? "学号\n" : "") +
                        "请重试。", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Mat prepro_img_4photo = pp.Preprocess(pp.OriginalMat,
                PictureProcess.CardRecognitionFlags.TestPermissionRecognition,
                PictureProcess.InfoTypeDetectionFlags.PhotoDetection);
            ArrayList photos = pp.GetAvatarMatsFromBoxes4Test(pp.GetArea(prepro_img_4photo));

            Mat testID_mat = (Mat)infos[0],
                id_mat = (Mat)infos[1], stuNum_mat = (Mat)infos[2];
            Image photo = ((Mat)photos[0]).ToBitmap();
            string[] texts = new string[3];

            if (GetID(id_mat, "eng", out texts[0]) &&
                GetID(testID_mat, "eng", out texts[1]) &&
                GetID(stuNum_mat, "eng", out texts[2]))
            {
                ConfirmID confirmid = new ConfirmID(texts);
                confirmid.GetInfo += getCorrectID;
                if (confirmid.ShowDialog() == DialogResult.OK &&
                    confirmid.getConfirmPass)
                {
                    testAvatar.Image = photo;
                    s_finished = true;
                    //testIDSelect.Enabled = false;
                    //testIDStatus.Text = "已完成";
                    //testIDStatus.ForeColor = testIDSelect.ForeColor;
                    //faceSelect.Enabled = true;
                    //faceSelect.Checked = true;
                    //faceStatus.ForeColor = faceSelect.ForeColor;
                    confirmid.Close();
                }
            }
        }

        private void FaceRecognition()
        {
            Mat mat0 = openCamera();
            if (mat0.Empty()) return;

            Face face1 = new Face(Face.PhotoDetectionType.CameraDetection);
            face1.FaceDetectionByCaffeNet(mat0);
            face1.FacePreprocess();

            // 这里要改成数据库连接
            Face face2 = new Face(Face.PhotoDetectionType.DatabaseDetection);
            Mat db = Cv2.ImRead(@"D:\C# Work\xd2\xd2\Resources\Pictures\showstupic.jpg", ImreadModes.Unchanged);
            face2.FaceDetectionByCaffeNet(db);
            face2.FacePreprocess();
        }

        private void shotButton_Click(object sender, EventArgs e)
        {
            isStopClick = true;
        }

        private string[] getCorrectID(string[] input, int flag)
        {
            switch (flag)
            {
                case 1:
                    idTextBox.Text = input[0];
                    nameTextBox.Text = input[1];
                    avatarFromDB = (new Bitmap(input[2])).ToMat();break;
                case 0:
                    testIDTextBox.Text = input[0];
                    roomIDTextBox.Text = input[0].Substring(10, 3);
                    testNumberTextbox.Text = input[0].Substring(13, 2);
                    levelTextBox.Text = input[0].Substring(9, 1) == "1" ? "四级" : "六级";
                    testLocTextBox.Text = input[0].Substring(0, 6);
                    //idNumberTextBox.Text = input[2];
                    stuNumTextBox.Text = input[2];
                    break;
            }
            return input;
        }

        private bool GetID(Mat input, string lang, out string result)
        {
            string text;
            TextExtractor te = new TextExtractor(input, out text, lang);
            if (text != string.Empty)
            {
                result = text;
                return true;
            }
            else
            {
                result = string.Empty;
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat frame0 = openCamera();
            Face temp = new Face(Face.PhotoDetectionType.CameraDetection);
            temp.FaceDetectionByCaffeNet(frame0);
            temp.FacePreprocess();
            Cv2.ImShow("processed face", temp.FinalizedFace);
            Cv2.WaitKey();

            string path = @"D:\C# Work\xd2\xd2\Resources\Preprocessed Faces\" + faceNumberTextBox.Text + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int number = Directory.GetFiles(path).Length;
            label1.Text = "第" + faceNumberTextBox.Text + "个文件夹，" +
                "共" + (number + 1).ToString() + "份人脸照片";
            string file = string.Format("{0}{1}.png", path, number + 1);
            Cv2.ImWrite(file, temp.FinalizedFace);
        }
    }
}
