using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace xd2
{
    using Point = OpenCvSharp.Point;
    using Size = OpenCvSharp.Size;

    class PictureProcess
    {
        Mat raw_img0;  //保存最原始的图片
        Mat raw_img1;  //给后期正畸和裁切用
        Mat raw_img2;  //保存给头像处理用
        Mat histMatch_model;

        public enum CardRecognitionFlags { IDRecognition, TestPermissionRecognition };
        public enum InfoTypeDetectionFlags { InfoDetection, PhotoDetection };

        readonly float idMaxRatio = 18.0f, idMinRatio = 10.0f;
        readonly float nameMaxRatio = 5.0f, nameMinRatio = 1.5f;

        readonly float idMinRatio_4test = 8.0f, idMaxRatio_4test = 13.5f;
        readonly float testIDMinRatio = 7.0f, testIDMaxRatio = 12.0f;
        readonly float stuNumMinRatio = 3.0f, stuNumMaxRatio = 9.0f;
        //readonly float testCodeMaxRatio=

        static public void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public Mat OriginalMat { get => raw_img0; }

        public Mat TwoValueProcessedMat { get => raw_img1; }

        /// <summary>
        /// This constructing function accepts one Mat parameter.
        /// </summary>
        /// <param name="original">The original Mat object</param>
        public PictureProcess(Mat original, CardRecognitionFlags flag)
        {
            raw_img0 = new Mat();
            original.CopyTo(raw_img0);
            Cv2.Flip(raw_img0, raw_img0, FlipMode.Y);

            switch (flag)
            {
                case CardRecognitionFlags.IDRecognition:histMatch_model = Cv2.ImRead(@"D:\C# Work\xd2\xd2\Resources\Pictures\身份证信息面.jpg", ImreadModes.Unchanged);break;
                case CardRecognitionFlags.TestPermissionRecognition:histMatch_model = Cv2.ImRead(@"D:\C# Work\xd2\xd2\Resources\Pictures\四六级准考证信息面.jpg", ImreadModes.Unchanged);break;
            }
        }

        /// <summary>
        /// This function process the raw_img Matrix and find the id basic
        /// information through scanning the whole image transferred
        /// </summary>
        /// <returns>Processed Mat object</returns>
        public Mat Preprocess(Mat input, CardRecognitionFlags CRF, InfoTypeDetectionFlags ITDF)
        {
            Mat raw_img = input;
            //bool isDarker = LumaCompare(raw_img, histMatch_model);

            /*1.对传入的图像进行彩色噪声降噪
             高斯滤波的高斯核由图像宽高比确定：1280:720≈5:3*/
            Cv2.GaussianBlur(raw_img, raw_img, new Size(5, 3), 0);
            //Cv2.ImShow("gaus", raw_img);
            //Cv2.WaitKey();

            /*2.降噪后，对图像的白平衡进行修正
             若不修正白平衡，可能会存在图片偏红、偏蓝或偏绿，对后续处理
             产生一定影响*/
            Mat colored = ColorBalance(raw_img);
            //Cv2.ImShow("balance", colored);
            //Cv2.WaitKey();

            /*3.将图像转换为HSL颜色格式，根据给定的模板扫描证件亮度进行调整
             过亮图片会降低亮度，过暗图片会提高亮度，但是无法处理过曝或者极暗
             图片*/
            colored = HSLOptimize(colored);
            raw_img2 = new Mat(colored.Width, colored.Height, MatType.CV_8UC3);
            colored.CopyTo(raw_img2);  //完整赋值
            //Cv2.ImShow("hsl", raw_img2);
            //Cv2.WaitKey();

            /*4.将亮度调整后的图片进行锐化处理，强化边缘信息
             此处正式将图片转换为灰度图
             此时会将处理过后的二值图像赋给原图像
             后面进行文字区域裁切和识别时需要用到*/
            Cv2.CvtColor(colored, colored, ColorConversionCodes.BGR2GRAY);
            Mat Gaussian = new Mat();
            Cv2.GaussianBlur(colored, Gaussian, new Size(5, 3), 0);
            colored = 2 * colored - Gaussian;
            raw_img1 = colored;  //这里只是传了一个引用/指针过去
            //Cv2.ImShow("enhance", raw_img1);
            //Cv2.WaitKey();

            /*5.对锐化后的灰度图与模式图进行直方图匹配*/
            Mat histed = colored, histMatch = new Mat();
            //switch (CRF)
            //{
            //    case CardRecognitionFlags.IDRecognition:
            //        histMatch_model = Cv2.ImRead(@"D:\C# Work\xd2\xd2\Resources\身份证信息面.jpg", ImreadModes.Unchanged);break;
            //    case CardRecognitionFlags.TestPermissionRecognition:
            //        histMatch_model = Cv2.ImRead(@"D:\C# Work\xd2\xd2\Resources\四六级准考证信息面.jpg", ImreadModes.Unchanged);break;
            //}
            Cv2.CvtColor(histMatch_model, histMatch, ColorConversionCodes.BGR2GRAY);
            histed = HistMatching(histed, histMatch);
            //Cv2.ImShow("histed", histed);
            //Cv2.WaitKey();

            /*6.直方图匹配过后的灰度图直接进行OTSU处理，其中需要利用大津法求出
             比较合适的阈值。*/
            Mat otsu_img = OTSUProcess(histed);
            //Cv2.ImShow("otsu", otsu_img);
            //Cv2.WaitKey();
            //raw_img2 = otsu_img;
            //raw_img1 = otsu_img;

            /*此处保留对直方图均衡化和HSV处理的注释代码*/
            //colored = HistEqualize(colored);

            //Mat hsv_img = HSVConvert(histed);  //根据明度将黑色提取出来
            //raw_img0 = hsv_img;
            //Mat hsv_img_a = OTSUProcess(histed);
            //raw_img0 = hsv_img;
            //Cv2.ImShow("otsu", hsv_img);

            /*7.进行形态学处理*/
            if (CRF == CardRecognitionFlags.IDRecognition &&
                ITDF == InfoTypeDetectionFlags.InfoDetection)
            {
                Mat dAnde_img = DilateAndErode(otsu_img, new Size(1, 1), new Size(50, 6));

                /*形态学处理过后的图像就是图像预处理阶段的最后一步，
                 * 此时返回形态学处理过后的图像*/
                return dAnde_img;
            }
            else if (CRF == CardRecognitionFlags.IDRecognition &&
                ITDF == InfoTypeDetectionFlags.PhotoDetection)
            {
                Mat eAndd_img = DilateAndErode(otsu_img, new Size(1, 1), new Size(78, 75));

                return eAndd_img;
            }
            else if (CRF == CardRecognitionFlags.TestPermissionRecognition &&
                ITDF == InfoTypeDetectionFlags.InfoDetection)
            {
                Mat eAndd_img = DilateAndErode(otsu_img, new Size(1, 1), new Size(17, 2));
                //Cv2.ImShow("eandd_img", eAndd_img);
                //Cv2.WaitKey();
                return eAndd_img;
            }
            else if (CRF == CardRecognitionFlags.TestPermissionRecognition &&
                ITDF == InfoTypeDetectionFlags.PhotoDetection)
            {
                Mat eAndd_img = DilateAndErode(otsu_img, new Size(1, 1), new Size(60, 28));

                return eAndd_img;
            }
            else return raw_img2;   //不存在这种情况，只用来占位
        }

        public ArrayList GetArea(Mat input)
        {
            Mat raw_img = input;

            //寻找轮廓
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(raw_img, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple, new Point(0, 0));

            //----------These are for contours testing, in case something wrong happens---------
            Mat contours_s = Mat.Zeros(raw_img.Height, raw_img.Width, MatType.CV_8UC1);
            Cv2.DrawContours(contours_s, contours, -1, new Scalar((int)Cv2.GetTheRNG()));
            //Cv2.ImShow("contour", contours_s);
            //Cv2.WaitKey();

            ArrayList boxes;
            GetRotatedRectangles(ref raw_img, ref contours, out boxes);

            return boxes;
        }

        public ArrayList GetTextsMatsFromBoxes4ID(ArrayList boxes)
        {
            ArrayList id = new ArrayList(), name = new ArrayList();
            float MIN_YS = raw_img1.Height, MAX_XLENGTH = 0.0f;

            foreach (RotatedRect box in boxes)
            {
                Point2f[] points = new Point2f[4];
                points = box.Points();
                float[] Xs = { points[0].X, points[1].X, points[2].X, points[3].X },
                    Ys = { points[0].Y, points[1].Y, points[2].Y, points[3].Y };
                float x_s = Xs.Min(), x_e = Xs.Max(), y_s = Ys.Min(), y_e = Ys.Max();

                float ratio = box.Size.Height > box.Size.Width ?
                    box.Size.Height / box.Size.Width :
                    box.Size.Width / box.Size.Height;
                double nonZeroRatio;

                if (x_s < raw_img1.Width / 2 && x_e > raw_img1.Width / 2 &&
                        y_s > raw_img1.Height / 2 && y_e > raw_img1.Height / 2)
                {
                    if (ratio >= idMinRatio && ratio <= idMaxRatio)
                    {
                        Mat id_cropped = DeskewAndCrop(raw_img1, box, 1);

                        nonZeroRatio = (double)id_cropped.CountNonZero() /
                            (double)(id_cropped.Rows * id_cropped.Cols);
                        //if (nonZeroRatio >= 0.15 && nonZeroRatio <= 0.21)
                        //{
                            if (x_e - x_s > MAX_XLENGTH)
                            {
                                MAX_XLENGTH = x_e - x_s;
                                //string title = nonZeroRatio.ToString() + "   " +
                                    //points[0].X.ToString() + ":" + points[0].Y.ToString() + "   " +
                                    //points[1].X.ToString() + ":" + points[1].Y.ToString() + "   " +
                                    //points[2].X.ToString() + ":" + points[2].Y.ToString() + "   " +
                                    //points[3].X.ToString() + ":" + points[3].Y.ToString();
                                //Cv2.ImShow(title, id_cropped);
                                //Cv2.WaitKey();
                                if (id.Count != 0) id.RemoveAt(0);
                                id.Add(id_cropped);
                            }
                        //}
                    }
                }
                else if (x_s < raw_img1.Width / 2 && x_e < raw_img1.Width / 2 &&
                        y_s < raw_img1.Height / 2 && y_e < raw_img1.Height / 2 &&
                        y_s < MIN_YS)
                {
                    Mat name_cropped = DeskewAndCrop(raw_img1, box, 1);
                    nonZeroRatio = (double)name_cropped.CountNonZero() /
                        (double)(name_cropped.Rows * name_cropped.Cols);
                    //if (nonZeroRatio >= 0.2 && nonZeroRatio <= 0.3 && y_s < MIN_YS)
                    //{
                    if (ratio > nameMinRatio && ratio <= nameMaxRatio)
                    {
                        if (y_s == 0 || x_s == 0) continue;

                        MIN_YS = y_s;
                        //string title = nonZeroRatio.ToString() + "   " +
                        //    points[0].X.ToString() + "," + points[0].Y.ToString() + "   " +
                        //    points[1].X.ToString() + ":" + points[1].Y.ToString() + "   " +
                        //    points[2].X.ToString() + ":" + points[2].Y.ToString() + "   " +
                        //    points[3].X.ToString() + ":" + points[3].Y.ToString();
                        //Cv2.ImShow(title, name_cropped);
                        //Cv2.WaitKey();
                        if (name.Count != 0) name.RemoveAt(0);
                        name.Add(name_cropped);
                    }
                    //}
                }
                else continue;
            }

            if (id.Count == 0) id.Add(Mat.Zeros(1, 1, MatType.CV_8UC1));
            if (name.Count == 0) id.Add(Mat.Zeros(1, 1, MatType.CV_8UC1));
            ArrayList idAndName = new ArrayList() { id[0], name[0] };
            return idAndName;
        }

        public ArrayList GetAvatarMatsFromBoxes4ID(ArrayList boxes)
        /*Type is Mat*/
        {
            ArrayList photo = new ArrayList();
            float min_Ys = raw_img2.Height;

            foreach (RotatedRect box in boxes)
            {
                Point2f[] points = new Point2f[4];
                points = box.Points();
                float[] Xs = { points[0].X, points[1].X, points[2].X, points[3].X },
                    Ys = { points[0].Y, points[1].Y, points[2].Y, points[3].Y };
                float x_s = Xs.Min(), x_e = Xs.Max(), y_s = Ys.Min(), y_e = Ys.Max();

                float ratio = box.Size.Height > box.Size.Width ?
                    box.Size.Height / box.Size.Width :
                    box.Size.Width / box.Size.Height;

                if (ratio >= 1.0f && ratio <= 2.0f)
                {
                    if (x_s > raw_img0.Width / 2 && x_e > raw_img0.Width / 2 &&
                        y_s < raw_img0.Height / 2 && y_e > raw_img0.Height / 2)
                    {
                        if (y_s < min_Ys)
                        {
                            min_Ys = y_s;
                            Mat id_cropped = DeskewAndCrop(raw_img2, box, 0);
                            //Cv2.ImShow("avatar" + ratio.ToString(), id_cropped);
                            //Cv2.WaitKey();
                            if (photo.Count != 0) photo.RemoveAt(0);
                            photo.Add(id_cropped);
                        }
                    }
                }
                else continue;
            }
            return photo;
        }

        public ArrayList GetTextsMatsFromBoxes4Test(ArrayList boxes)
        {
            Mat id = new Mat(),
                testid = new Mat(), stunum = new Mat();
            bool idFound = false,
                testidFound = false, stunumFound = false,
                allFound = idFound && testidFound && stunumFound;
            float id_minY = raw_img1.Height, stunum_minX = raw_img1.Width;

            foreach(RotatedRect box in boxes)
            {
                if (allFound) break;

                Point2f[] points = new Point2f[4];
                points = box.Points();

                float[] Xs = { points[0].X, points[1].X, points[2].X, points[3].X },
                    Ys = { points[0].Y, points[1].Y, points[2].Y, points[3].Y };
                float x_s = Xs.Min(), x_e = Xs.Max(),
                    y_s = Ys.Min(), y_e = Ys.Max();
                float ratio = box.Size.Height > box.Size.Width ?
                    box.Size.Height / box.Size.Width :
                    box.Size.Width / box.Size.Height;
                //string title = ratio.ToString() + "   " +
                //                    points[0].X.ToString() + ":" + points[0].Y.ToString() + "   " +
                //                    points[1].X.ToString() + ":" + points[1].Y.ToString() + "   " +
                //                    points[2].X.ToString() + ":" + points[2].Y.ToString() + "   " +
                //                    points[3].X.ToString() + ":" + points[3].Y.ToString();

                if (x_s == 0 || y_s == 0 || x_e == raw_img1.Width || y_e == raw_img1.Height) continue;

                if (x_s < raw_img1.Width / 3 && x_e < raw_img1.Width / 2 &&
                        y_s < raw_img1.Height / 2 && y_e < raw_img1.Height / 2 &&
                        !(x_s > raw_img1.Width * 0.04 && x_s < raw_img1.Width * 0.07))
                {
                    if (ratio >= testIDMinRatio && ratio <= testIDMaxRatio)
                    {
                        testid = DeskewAndCrop(raw_img1, box, 1);
                        //Cv2.ImShow(title + "考号", testid);
                        //Cv2.WaitKey();
                        testidFound = true;
                    }
                }
                else if (x_s < raw_img1.Width / 3 &&
                        (x_e < raw_img1.Width / 2 || x_e < raw_img1.Width / 3 * 2) &&
                        (y_s > raw_img1.Height / 2 || y_s > raw_img1.Height / 3 * 2) &&
                        (y_e > raw_img1.Height / 2 || y_s > raw_img1.Height / 3 * 2))
                {
                    if (ratio >= idMinRatio_4test && ratio <= idMaxRatio_4test)
                    {
                        if (y_s < id_minY)
                        {
                            id_minY = y_s;
                            id = DeskewAndCrop(raw_img1, box, 1);
                            //Cv2.ImShow(title + "身份证", id);
                            idFound = true;
                        }
                    }
                }
                else if (x_s > raw_img1.Width / 2 &&
                        x_e > raw_img1.Width / 3 * 2 &&
                        y_s > raw_img1.Height / 3 * 2 && y_e > raw_img1.Height / 3 * 2)
                {
                    if (ratio >= stuNumMinRatio && ratio <= stuNumMaxRatio)
                    {
                        if (x_s < stunum_minX)
                        {
                            stunum_minX = x_s;
                            stunum = DeskewAndCrop(raw_img1, box, 1);
                            //Cv2.ImShow(title + "学号", stunum);
                            //Cv2.WaitKey();
                            stunumFound = true;
                        }
                    }
                }
            }

            return new ArrayList { testid, id, stunum };
        }

        public ArrayList GetAvatarMatsFromBoxes4Test(ArrayList boxes)
        {
            Mat avatar = new Mat();
            bool avatarFound = false;
            float min_Xs = raw_img2.Width;

            foreach(RotatedRect box in boxes)
            {
                if (avatarFound) break;

                Point2f[] points = box.Points();

                float[] Xs = { points[0].X, points[1].X, points[2].X, points[3].X },
                    Ys = { points[0].Y, points[1].Y, points[2].Y, points[3].Y };
                float x_s = Xs.Min(), x_e = Xs.Max(),
                    y_s = Ys.Min(), y_e = Ys.Max();
                float ratio = box.Size.Height > box.Size.Width ?
                    box.Size.Height / box.Size.Width :
                    box.Size.Width / box.Size.Height;

                if (ratio >= 1.0f && ratio <= 2.0f)
                {
                    if (x_s > raw_img2.Width / 2 && x_e > (raw_img2.Width / 3) * 2 &&
                        y_s < raw_img2.Height / 2 && y_e > raw_img2.Height / 2)
                    {
                        if (x_s < min_Xs)
                        {
                            min_Xs = x_s;
                            avatar = DeskewAndCrop(raw_img2, box, 0);
                            avatarFound = true;
                        }
                    }
                }
            }

            return new ArrayList { avatar };
        }

        /// <summary>
        /// The simplest way to balance the color values within the Mat image
        /// object
        /// </summary>
        /// <param name="input">The source Mat object</param>
        /// <returns>The post-balanced processed image, Mat object.</returns>
        private Mat ColorBalance(Mat input)
        {
            Mat[] channels = new Mat[3];
            Cv2.Split(input, out channels);

            double B_sum = 0.0, G_sum = 0.0, R_sum = 0.0;
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    B_sum += (double)channels[0].At<byte>(i, j);
                    G_sum += (double)channels[1].At<byte>(i, j);
                    R_sum += (double)channels[2].At<byte>(i, j);
                }
            }

            double B_average = B_sum / (double)(input.Height * input.Width),
                G_average = G_sum / (double)(input.Height * input.Width),
                R_average = R_sum / (double)(input.Height * input.Width);

            double sum_average = (B_average + G_average + R_average) / 3.0d;

            double Bk = sum_average / B_average, Gk = sum_average / G_average,
                Rk = sum_average / R_average;

            Cv2.AddWeighted(channels[0], Bk, 0, 0, 0, channels[0]);
            Cv2.AddWeighted(channels[1], Gk, 0, 0, 0, channels[1]);
            Cv2.AddWeighted(channels[2], Rk, 0, 0, 0, channels[2]);

            Mat merged = new Mat();
            Cv2.Merge(channels, merged);

            //Cv2.ImShow("color", merged);
            return merged;
        }

        private Mat HistEqualize(Mat input)
        {
            Mat Ycrcb = new Mat();
            Cv2.CvtColor(input, Ycrcb, ColorConversionCodes.BGR2YCrCb);
            Mat[] channels = new Mat[3];
            Cv2.Split(Ycrcb, out channels);

            Cv2.EqualizeHist(channels[0], channels[0]);

            Mat merged = new Mat();
            Cv2.Merge(channels, merged);
            Cv2.CvtColor(merged, merged, ColorConversionCodes.YCrCb2BGR);

            return merged;
        }

        private Mat HSLOptimize(Mat input)
        {
            Mat hsl_img = new Mat(), hsl_best = new Mat();
            Cv2.CvtColor(input, hsl_img, ColorConversionCodes.BGR2HLS);
            Cv2.CvtColor(histMatch_model, hsl_best, ColorConversionCodes.BGR2HLS);

            double best_Laverage = 0.0, best_Lsum = 0.0,
                best_Saverage = 0.0, best_Ssum = 0.0;
            for (int i = 0; i < hsl_best.Rows; i++)
            {
                for (int j = 0; j < hsl_best.Cols; j++)
                {
                    best_Lsum += hsl_best.At<Vec3b>(i, j)[1];
                    best_Ssum += hsl_best.At<Vec3b>(i, j)[2];
                }
            }
            best_Laverage = best_Lsum / (double)(hsl_best.Rows * hsl_best.Cols);
            best_Saverage = best_Ssum / (double)(hsl_best.Rows * hsl_best.Cols);

            double img_Laverage = 0.0, img_Lsum = 0.0,
                img_Saverage = 0.0, img_Ssum = 0.0;
            for (int i = 0; i < hsl_img.Rows; i++)
            {
                for (int j = 0; j < hsl_img.Cols; j++)
                {
                    img_Lsum += hsl_img.At<Vec3b>(i, j)[1];
                    img_Ssum += hsl_img.At<Vec3b>(i, j)[2];
                }
            }
            img_Laverage = img_Lsum / (double)(hsl_img.Rows * hsl_img.Cols);
            img_Saverage = img_Ssum / (double)(hsl_img.Rows * hsl_img.Cols);

            double k1 = best_Laverage / img_Laverage,
                k2 = img_Saverage / best_Saverage;
            for (int i = 0; i < hsl_img.Rows; i++)
            {

                for (int j = 0; j < hsl_img.Cols; j++)
                {
                    double temp1 = hsl_img.At<Vec3b>(i, j)[1] * k1,
                        temp2 = hsl_img.At<Vec3b>(i, j)[2] * k2;
                    //如果某个点的值超过了255，temp1的值会被存储为一个double
                    //但是如果转换成一个byte，那么这个时候这个值就会超过
                    //存储范围，从而变成一个负值，导致出现非常多的
                    //黑色像素块
                    if (temp1 > 255.0) temp1 = 255.0;
                    hsl_img.At<Vec3b>(i, j)[1] = (byte)temp1;
                    //hsl_img.At<Vec3b>(i, j)[2] = (byte)temp2;
                }
            }

            Mat output = new Mat();
            Cv2.CvtColor(hsl_img, output, ColorConversionCodes.HLS2BGR);
            return output;
        }

        /// <summary>
        /// This function is used to match the source Mat object with another
        /// template Mat object in terms of histogram distribution.
        /// It will automatically converts the source Mat object to the similar
        /// style of the template Mat object, which somewhat may contribute to
        /// the process of finding the right regions of the useful information
        /// on the ID card.
        /// <para>This function might be deprecated due to the actual result
        /// of how the image will be shown.</para>
        /// </summary>
        /// <param name="scr">source Mat object, accept 8UC1</param>
        /// <param name="tpl">template Mat object, accept 8UC1</param>
        /// <returns>The processed source Mat object, which has already
        /// been style-similar to the template Mat object</returns>
        private Mat HistMatching(Mat scr, Mat tpl)
        {
            int scr_h = scr.Height, scr_w = scr.Width, scr_pixelNum = scr_h * scr_w;
            int[] scr_pixelVal = new int[256];
            for (int m = 0; m < scr_h; m++)
            {
                for (int n = 0; n < scr_w; n++)
                {
                    scr_pixelVal[(int)scr.At<byte>(m, n)]++;
                }
            }

            int tpl_h = tpl.Height, tpl_w = tpl.Width, tpl_pixelNum = tpl_h * tpl_w;
            int[] tpl_pixelVal = new int[256];
            for (int m = 0; m < tpl_h; m++)
            {
                for (int n = 0; n < tpl_w; n++)
                {
                    tpl_pixelVal[(int)tpl.At<byte>(m, n)]++;
                }
            }

            //Calculate the probability
            double[] scr_p = new double[256], tpl_p = new double[256];
            for (int j = 0; j < 256; j++)
            {
                scr_p[j] = (double)scr_pixelVal[j] / (double)scr_pixelNum;
                tpl_p[j] = (double)tpl_pixelVal[j] / (double)tpl_pixelNum;
            }

            //Calculate the Mapping results
            int[] scr_map = new int[256], tpl_map = new int[256];
            for (int j = 0; j < 256; j++)
            {
                double scr_sum = 0.0d, tpl_sum = 0.0d;
                for (int k = 0; k <= j; k++)
                {
                    scr_sum += scr_p[k];
                    tpl_sum += tpl_p[k];
                }
                scr_map[j] = Convert.ToInt32(Math.Round(255.0 * scr_sum));
                tpl_map[j] = Convert.ToInt32(Math.Round(255.0 * tpl_sum));
            }

            //Construct Histogram Mapping(Little difficult to grasp)
            int[] grayMatchMap = new int[256];

            for (int j = 0; j < 256; j++)
            {
                int nValue = 0, nValue_l = 0;
                int cnt = 0;
                int temp_sum = scr_map[j];

                for (int k = 0; k < 256; k++)
                {
                    if (temp_sum == tpl_map[k])
                    {
                        nValue += k;
                        cnt++;
                    }
                    if (temp_sum < tpl_map[k])
                    {
                        nValue_l = j;
                        break;
                    }
                }
                if (cnt == 0)
                {
                    nValue = nValue_l;
                    cnt = 1;
                }
                grayMatchMap[j] = nValue / cnt;
            }

            //Build the single channel Mat object, for later mergence.
            Mat single = new Mat(scr.Rows, scr.Cols, MatType.CV_8UC1);

            for (int m = 0; m < scr.Rows; m++)
            {
                for (int n = 0; n < scr.Cols; n++)
                {
                    //This step is crucial
                    single.At<byte>(m, n) = (byte)grayMatchMap[(int)scr.At<byte>(m, n)];
                }
            }

            Mat res = single;
            return res;
        }

        /// <summary>
        /// This function converts a standard BGR format Mat object into a HSV format
        /// Mat object, while, besides just converting the Mat object itself to
        /// another color space, it also optimizes the showing of the entire image
        /// regarding to the specific ID card information recognition.
        /// It attempts to highlight the information parts, which are the ones that
        /// supposedly in black color, while others are in different other colors.
        /// It contains a concatination of parameters that somehow limit the color
        /// range that directly specifies the color value of the useful information
        /// on the ID card.
        /// </summary>
        /// <param name="input">the input Mat object</param>
        /// <returns>The post-processed Mat object</returns>
        private Mat HSVConvert(Mat input)
        {
            Mat raw_img = new Mat();
            Cv2.CvtColor(input, raw_img, ColorConversionCodes.RGB2HSV);

            Mat res = Mat.Zeros(raw_img.Rows, raw_img.Cols, (MatType)MatType.CV_8U);
            for (int i = 0; i < raw_img.Rows; i++)
            {
                for (int j = 0; j < raw_img.Cols; j++)
                {
                    int hue = Convert.ToInt32(raw_img.At<Vec3b>(i, j)[0]);
                    int saturation = Convert.ToInt32(raw_img.At<Vec3b>(i, j)[1]);
                    int value = Convert.ToInt32(raw_img.At<Vec3b>(i, j)[2]);

                    // && saturation>=43 && saturation<=255  15-105
                    if (value >= 0 &&  //35-85
                        value <= 160)// &&
                                     //!(hue >= 100 && hue <= 124) &&  //blue, former 94-98, 115-124
                                     //!(hue >= 0 && hue <= 10) &&  //red, former 0-2, 0-10
                                     //!(hue >= 156 && hue <= 180) &&  //crimson red, former 178-180, 175-180
                                     //!(hue >= 55 && hue <= 99))  //green, former 88-90, 85-95
                    {
                        res.At<byte>(i, j) = 255;
                    }
                }
            }

            //Cv2.ImShow("hsv", res);
            return res;
        }

        private Mat DilateAndErode(Mat input, Size erode_size, Size dilate_size)
        {
            //Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(25, 20));
            Mat eroded_img = new Mat();

            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, erode_size);
            Cv2.Erode(input, eroded_img, element);

            element = Cv2.GetStructuringElement(MorphShapes.Rect, dilate_size);
            Cv2.Dilate(eroded_img, eroded_img, element);

            //Cv2.Dilate(input, dilated_img, element, new Point(-1, -1), 7);
            //Cv2.Erode(dilated_img, eroded_img, element, new Point(-1, -1), 3);
            //Cv2.MorphologyEx(input, eroded_img, MorphTypes.Open, element, new Point(-1, -1), iterations: 1);

            //element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(15, 13));
            //Mat dilated_img = new Mat();
            //Cv2.MorphologyEx(eroded_img, dilated_img, MorphTypes.Open, element, new Point(-1, -1), iterations: 1);
            //Cv2.ImShow("d&e", eroded_img);
            return eroded_img;
        }

        /// <summary>
        /// This function crops all the minimal bounding rectangles of the found
        /// white connected regions, which mark the possible target areas
        /// that are specifically preferred by the user.
        /// *** This function is deprecated. ***
        /// </summary>
        /// <param name="contours">The already-found contours</param>
        /// <param name="id">The possible Mat objects that have the ID information in</param>
        /// <param name="name">The possible Mat objects that contains the name information in</param>
        /*private void GetRotatedRectangles(ref Point[][] contours, out ArrayList id, out ArrayList name)
        {
            ArrayList idAreas = new ArrayList(), nameAreas = new ArrayList();
            foreach(var contour in contours)
            {
                var box = Cv2.MinAreaRect(contour);
                float height = box.Size.Height, width = box.Size.Width;
                float ratio = height > width ? height / width : width / height;

                if(ratio>=idMinRatio && ratio <= idMaxRatio)
                {
                    idAreas.Add(box);
                }
                else if(ratio>=nameMinRatio && ratio <= nameMaxRatio)
                {
                    nameAreas.Add(box);
                }
            }
            id = idAreas; name = nameAreas;
        }*/

        private void GetRotatedRectangles(ref Mat input, ref Point[][] contours, out ArrayList boxes)
        {
            ArrayList areas = new ArrayList();
            foreach (Point[] contour in contours)
            {
                var box = Cv2.MinAreaRect(contour);
                if (box.Size.Height * box.Size.Width < 400) continue;
                areas.Add(box);

                Point2f[] points0 = new Point2f[4];
                points0 = box.Points();
                for (int i = 0; i < 4; i++)
                {
                    Cv2.Line(input, (Point)points0[i], (Point)points0[(i + 1) % 4], new Scalar((int)Cv2.GetTheRNG()));
                }
            }
            //Cv2.ImShow("draw rect", input);
            boxes = areas;
        }

        /// <summary>
        /// <para>Deskew and Crop the shape of the Box from the input image.
        /// It will automatically turn the RotatedRec into Rect.</para>
        /// <para>正畸与裁切图片</para>
        /// </summary>
        /// <param name="input">
        /// <para>The background image that the box object supposedly will
        /// crop from.</para>
        /// <para>Box对象将从这个input的Mat对象中裁取图像信息</para>
        /// </param>
        /// <param name="box">
        /// <para>This is a RotatedBox object, containing information of a
        /// box object used for transformation.</para>
        /// <para>RotatedBox对象，存储了很多区域信息</para>
        /// </param>
        /// <param name="flag">
        /// <para>If this function is used for word detecting,
        /// the flag is 1, otherwise the flag should be 0.</para>
        /// <para>如果该函数用于文字部分的正畸和裁切，flag值应是1，否则应是0</para>
        /// </param>
        /// <returns>
        /// <para>Post-deskewed-and-cropped Mat object</para>
        /// <para>经过正畸和裁切的目标Mat对象</para>
        /// </returns>
        private Mat DeskewAndCrop(Mat input, RotatedRect box, int flag)
        {
            //Deskew box text area.
            double image_angle = box.Angle;
            var _size = box.Size;

            if (image_angle < -45.0)
            {
                image_angle += 90.0d;
                swap<float>(ref _size.Height, ref _size.Width);
            }

            var affine_transform = Cv2.GetRotationMatrix2D(box.Center, image_angle, 1.0);
            Mat rotated = new Mat();
            Cv2.WarpAffine(input, rotated, affine_transform, input.Size(), InterpolationFlags.Cubic, BorderTypes.Constant);

            //Crop the box area out.
            Mat cropped = new Mat();
            Size size = new Size(_size.Width, _size.Height);

            Cv2.GetRectSubPix(rotated, size, box.Center, cropped);
            //if flag is 1, do this for word processing
            if (flag == 1) 
                Cv2.CopyMakeBorder(cropped, cropped, 10, 10, 10, 10, BorderTypes.Constant, new Scalar(255));

            //Individually process the cropped image.
            //if flag is 1, it's for words processing. if it's 0, it's for photo.
            if (flag == 1) 
                cropped = OTSUProcess(cropped);

            //Return the final successfully processed mat object
            return cropped;
        }

        private Mat OTSUProcess(Mat input)
        {
            Mat otsu_img = input;
            //Cv2.ImShow("before nma", otsu_img);
            //Cv2.WaitKey();

            otsu_img = NMA(otsu_img);
            //Cv2.ImShow("nma", otsu_img);
            //Cv2.WaitKey();

            Cv2.Threshold(otsu_img, otsu_img, BestThreshold(otsu_img), 255.0, ThresholdTypes.Otsu);
            //Cv2.ImShow("after threshold", otsu_img);
            //Cv2.WaitKey();

            Cv2.BitwiseNot(otsu_img, otsu_img);
            return otsu_img;
        }

        private Mat NMA(Mat input)
        {
            Cv2.Normalize(input, input, 0.0, 1.0, NormTypes.MinMax);

            Cv2.Multiply(input, input, input);

            Cv2.AddWeighted(input, 255.0, 0, 0, 0, input);
            return input;
        }

        private double CalcAvgGray(Mat input)
        {
            double sum = 0.0d;
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    sum += (double)input.At<byte>(i, j);
                }
            }
            return sum / (double)(input.Rows * input.Cols);
        }

        private int BestThreshold(Mat input)  //input → 8UC1
        {
            if (input.Type() != MatType.CV_8UC1)
            {
                return -1;
            }
            else
            {
                int[] pixelCount = new int[256];
                double[] pixelValueProbability = new double[256];
                double sumGrayValue = 0.0d, averageGrayValue = 0.0d;
                long imageSize = input.Rows * input.Cols;

                for (int i = 0; i < input.Rows; i++)
                {
                    for (int j = 0; j < input.Cols; j++)
                    {
                        int value = input.At<byte>(i, j);
                        pixelCount[value]++;
                        sumGrayValue += value;
                    }
                }
                averageGrayValue = sumGrayValue / imageSize;

                for (int i = 0; i < 256; i++)
                {
                    pixelValueProbability[i] = (double)pixelCount[i] / (double)imageSize;
                }

                double gMax = 0.0d;
                int threshold = 0;
                for (int i = 0; i < 256; i++)
                {
                    double gTMP;
                    double formerSumGrayValue = 0.0d,
                        latterSumGrayValue = 0.0d,
                        formerAverageGrayValue = 0.0d,
                        latterAverageGrayValue = 0.0d;
                    double formerWeightSum = 0.0d, latterWeightSum = 0.0d;

                    for (int j = 0; j < 256; j++)
                    {
                        if (j <= i)
                        {
                            formerWeightSum += pixelValueProbability[j];
                            formerSumGrayValue += j * pixelCount[j];
                        }
                        else
                        {
                            latterWeightSum += pixelValueProbability[j];
                            latterSumGrayValue += j * pixelCount[j];
                        }
                    }

                    formerAverageGrayValue = formerSumGrayValue / i + 1;
                    latterAverageGrayValue = latterSumGrayValue / 255 - i - 1;
                    gTMP = (double)Math.Pow((formerAverageGrayValue - latterAverageGrayValue), 2.0) * formerWeightSum * latterWeightSum;

                    if (gTMP > gMax)
                    {
                        gMax = gTMP;
                        threshold = i;
                    }
                }
                return threshold;
            }
        }

        private bool LumaCompare(Mat input, Mat raw_img)
        {
            double sum0 = 0.0d, sum1 = 0.0d;
            Mat HSL_img = new Mat(), Ori_img = new Mat();
            Cv2.CvtColor(input, HSL_img, ColorConversionCodes.BGR2HLS);
            Cv2.CvtColor(raw_img, Ori_img, ColorConversionCodes.BGR2HLS);

            for (int i = 0; i < HSL_img.Rows; i++)
            {
                for (int j = 0; j < HSL_img.Cols; j++)
                {
                    sum0 += HSL_img.At<Vec3b>(i, j)[1];
                }
            }

            for (int i = 0; i < Ori_img.Rows; i++)
            {
                for (int j = 0; j < Ori_img.Cols; j++)
                {
                    sum1 += Ori_img.At<Vec3b>(i, j)[1];
                }
            }

            double avg1 = sum0 / (double)(HSL_img.Rows * HSL_img.Cols),
                avg2 = sum1 / (double)(Ori_img.Rows * Ori_img.Cols);
            return avg1 < avg2 ? true : false;
        }

        /// <summary>
        /// This function is just for testing.
        /// </summary>
        /// <param name="input"></param>
        private void makeHistogram(ref Mat input)
        {
            Mat hsv_img = new Mat();
            Cv2.CvtColor(input, hsv_img, ColorConversionCodes.RGB2HSV);
            Mat[] hsv_imgs = new Mat[1] { hsv_img };

            Mat hist = new Mat();
            int hue_bins = 30, saturation_bins = 31;
            int[] histSize = new int[2] { hue_bins, saturation_bins };
            float[] hue_range = new float[2] { 0, 180 },
                saturation_range = new float[2] { 0, 256 };
            float[][] ranges = new float[2][] { hue_range, saturation_range };
            int[] channels = new int[2] { 0, 1 };
            Cv2.CalcHist(hsv_imgs, channels, Mat.Zeros((MatType)MatType.CV_8U, input.Width, input.Height), hist, 2, histSize, ranges);

            double maxFromMat = 0.0d, minFromMat = 0.0d;
            Cv2.MinMaxLoc(hist, out minFromMat, out maxFromMat);
            int step = 5;

            Mat showcase = Mat.Zeros((MatType)MatType.CV_8U, saturation_bins * step, hue_bins * step);
            for (int j = 0; j < hue_bins; j++)
            {
                for (int i = 0; i < saturation_bins; i++)
                {
                    float temp_val = hist.At<float>(j, i);
                    double intensity = Math.Round(temp_val * 255.0 / maxFromMat);

                    Cv2.Rectangle(showcase,
                        new Point(j * step, i * step),
                        new Point((j + 1) * step - 1, (i + 1) * step - 1),
                        Scalar.All(intensity),
                        -1);
                }
            }
        }
    }

    abstract class SuperResolutionMaker
    {
        public SuperResolutionMaker() { }

        abstract public Mat SuperResolution_Create(Mat input);
    }
}