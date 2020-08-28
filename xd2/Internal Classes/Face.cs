using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.Dnn;
using System.Collections;
using System.Windows.Forms;

namespace xd2.Internal_Classes
{
    class Face
    {
        public enum PhotoDetectionType { CameraDetection, DatabaseDetection };
        PhotoDetectionType type;

        CascadeClassifier cascade4NoGlasses = new CascadeClassifier(@"D:\C# Work\xd2\xd2\Resources\Haar XML\haarcascade_eye.xml");
        CascadeClassifier cascade4Glases = new CascadeClassifier(@"D:\C# Work\xd2\xd2\Resources\Haar XML\haarcascade_eye_tree_eyeglasses.xml");
        const float EYE_SX = 0.18f, EYE_SY = 0.26f,
            EYE_SW = 0.26f, EYE_SH = 0.28f, EYE_SE = 0.56f;

        public Mat DetectedFace { private set; get; } = new Mat();
        public Mat AdjustedFace { private set; get; } = new Mat();
        public Mat FinalizedFace { private set; get; } = new Mat();

        public Face(PhotoDetectionType type0) { type = type0; }

        public void FaceDetectionByCaffeNet(Mat input)
        {
            int height = input.Height, width = input.Width;
            int start_point_longer = height > width ? height / 2 - width / 2 - 1 : width / 2 - height / 2 - 1,
                start_point_shorter = 0,
                length = height > width ? width : height;

            Rect centerRectangle = width > height ?
                new Rect(start_point_longer, start_point_shorter, length, length) :
                new Rect(start_point_shorter, start_point_longer, length, length);
            Mat cropped_input = input.Clone(centerRectangle);

            Net net = CvDnn.ReadNetFromCaffe
                (@"D:\C# Work\xd2\xd2\Resources\Models\Caffe Face Detection\deploy.prototxt",
                @"D:\C# Work\xd2\xd2\Resources\Models\Caffe Face Detection\res10_300x300_ssd_iter_140000.caffemodel");
            Mat input_blob = CvDnn.BlobFromImage(cropped_input, 1.2, new Size(300, 300), new Scalar(104, 177, 123), swapRB: false, crop: false);

            net.SetInput(input_blob);
            Mat detectResult0 = net.Forward();
            IntPtr ptr = detectResult0.Ptr(0);

            Mat detectResult = new Mat(detectResult0.Size(2), detectResult0.Size(3),
                MatType.CV_32F, ptr);

            //int maxArea = 0;
            Rect rectObject = new Rect();
            Mat result = new Mat();
            for (int i = 0; i < detectResult.Rows; i++)
            {
                float confidence = detectResult.At<float>(i, 2);
                if (confidence > 0.4)
                {
                    int x_s = Convert.ToInt32(detectResult.At<float>(i, 3) * width);
                    int y_s = Convert.ToInt32(detectResult.At<float>(i, 4) * height);
                    int x_e = Convert.ToInt32(detectResult.At<float>(i, 5) * width);
                    int y_e = Convert.ToInt32(detectResult.At<float>(i, 6) * height);

                    rectObject = new Rect(x_s, y_s, x_e - x_s, y_e - y_s);
                }
            }
            try
            {
                result = input.Clone(rectObject);
                //Cv2.CvtColor(result, result, ColorConversionCodes.BGR2GRAY);
                //Cv2.ImShow("detected", result);
                //Cv2.WaitKey();
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message, "Error");
                return;
            }
            DetectedFace = result;
        }

        public void FacePreprocess() 
        {
            Point left, right;
            EyesDetector(out left, out right);
            FaceAdjustment(left, right);
            FaceEqualization();

            Mat bilateralred = new Mat(AdjustedFace.Size(), MatType.CV_8U);
            Cv2.BilateralFilter(AdjustedFace, bilateralred, 0, 20.0, 2.0);

            FinalizedFace = MaskProcedure(bilateralred);
        }

        private void EyesDetector(out Point leftEyeP, out Point rightEyeP)
        {
            Mat input = DetectedFace;

            int x_s = Convert.ToInt32(Math.Round(input.Cols * EYE_SX));
            int y_s = Convert.ToInt32(Math.Round(input.Rows * EYE_SY));
            int width = Convert.ToInt32(Math.Round(input.Cols * EYE_SW));
            int height = Convert.ToInt32(Math.Round(input.Rows * EYE_SH));
            int x_e = Convert.ToInt32(Math.Round(input.Cols * EYE_SE));

            Mat leftEye = input.Clone(new Rect(x_s, y_s, width, height));
            Mat rightEye = input.Clone(new Rect(x_e, y_s, width, height));

            Cv2.ImShow("lefteye " + type.ToString(), leftEye);
            Cv2.ImShow("righteye " + type.ToString(), rightEye);
            Cv2.WaitKey();

            Rect leftEyeRect = detectLargestObject(leftEye, cascade4NoGlasses, leftEye.Cols);
            if (leftEyeRect.X < 0)
                leftEyeRect = detectLargestObject(leftEye, cascade4Glases, leftEye.Cols);
            Point leftEyeCenter = new Point(leftEyeRect.X + leftEyeRect.Width / 2 + x_s, leftEyeRect.Y + leftEyeRect.Height / 2 + y_s);

            Rect rightEyeRect = detectLargestObject(rightEye, cascade4NoGlasses, rightEye.Cols);
            if (rightEyeRect.X < 0)
                rightEyeRect = detectLargestObject(rightEye, cascade4Glases, rightEye.Cols);
            Point rightEyeCenter = new Point(rightEyeRect.X + rightEyeRect.Width / 2 + x_e, rightEyeRect.Y + rightEyeRect.Height / 2 + y_s);

            //两个Point对应的是detected Mat对象的坐标
            leftEyeP = leftEyeCenter; rightEyeP = rightEyeCenter;
        }

        private Rect detectLargestObject(Mat img, CascadeClassifier cascade, int scaledWidth)
        {
            int flags = (int)HaarDetectionType.FindBiggestObject;

            Size minFeatureSize = new Size(20, 20);

            float searchScaleFactor = 1.1f;

            int minNeighbors = 3;

            Rect[] objects;
            objects = detectObjectsCustom(img, cascade, scaledWidth, flags, minFeatureSize, searchScaleFactor, minNeighbors);

            return objects.Count() > 0 ? objects[0] : new Rect(-1, -1, -1, -1);
        }

        private Rect[] detectObjectsCustom(Mat img, CascadeClassifier cascade, int scaledWidth, int flags, Size minFeatureSize, float searchScaleFactor, int minNeighbors)
        {
            Rect[] objects;

            Mat gray = new Mat();
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);

            Mat inputImg = new Mat();
            float scale = (float)scaledWidth / img.Cols;

            int scaledHeight = Convert.ToInt32(Math.Round(img.Rows * scale));
            Cv2.Resize(gray, inputImg, new Size(scaledWidth, scaledHeight));

            Mat equalizedImg = new Mat();
            Cv2.EqualizeHist(inputImg, equalizedImg);

            objects = cascade.DetectMultiScale(equalizedImg,
                Convert.ToDouble(searchScaleFactor), minNeighbors,
                (HaarDetectionType)flags, minFeatureSize);

            for (int i = 0; i < objects.Count(); i++)
            {
                objects[i].X = Convert.ToInt32(Math.Round(objects[i].X * scale));
                objects[i].Y = Convert.ToInt32(Math.Round(objects[i].Y * scale));
                objects[i].Width = Convert.ToInt32(Math.Round(objects[i].Width * scale));
                objects[i].Height = Convert.ToInt32(Math.Round(objects[i].Height * scale));
            }

            for (int i = 0; i < objects.Count(); i++)
            {
                if (objects[i].X < 0)
                    objects[i].X = 0;
                if (objects[i].Y < 0)
                    objects[i].Y = 0;
                if (objects[i].X + objects[i].Width > img.Cols)
                    objects[i].X = img.Cols - objects[i].Width;
                if (objects[i].Y + objects[i].Height > img.Rows)
                    objects[i].Y = img.Rows - objects[i].Height;
            }
            return objects;
        }

        private void FaceAdjustment(Point left, Point right)
        {
            Point centerBetweenEyes = new Point((left.X + right.X) * 0.5f, (left.Y + right.Y) * 0.5f);
            double deltaX = right.X - left.X, deltaY = right.Y - left.Y;
            double lenBetweenEyes = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double angle = Math.Atan2(deltaY, deltaX) * 180.0 / Cv2.PI;
            Cv2.ImShow(angle.ToString(), Mat.Zeros(1, 1, MatType.CV_8UC1));
            Cv2.WaitKey();

            const double BEST_LEFT_EYE_X = 0.16d;
            const double BEST_LEFT_AND_RIGHT_Y = 0.14d;
            const int BEST_FACE_WIDTH = 70, BEST_FACE_HEIGHT = 70;
            double bestLenBetweenEyes = 1.0 - 2 * 0.16d;
            double scale = bestLenBetweenEyes * BEST_FACE_WIDTH / lenBetweenEyes;

            Mat rotated = Cv2.GetRotationMatrix2D(centerBetweenEyes, angle, scale);
            double xDifference = BEST_FACE_WIDTH * 0.5d - centerBetweenEyes.X,
                yDifference = BEST_FACE_HEIGHT * BEST_LEFT_AND_RIGHT_Y - centerBetweenEyes.Y;
            rotated.At<double>(0, 2) += xDifference;
            rotated.At<double>(1, 2) += yDifference;
            Mat warpped = new Mat(BEST_FACE_HEIGHT, BEST_FACE_WIDTH, MatType.CV_8U, new Scalar(128));

            Mat gray = new Mat();
            Cv2.CvtColor(DetectedFace, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.WarpAffine(gray, warpped, rotated, warpped.Size());
            Cv2.ImShow("warpped", warpped);
            Cv2.WaitKey();

            AdjustedFace = warpped;
        }

        private void FaceEqualization()
        {
            Mat face = AdjustedFace;

            Mat equalizedWholeFace = new Mat();
            Cv2.EqualizeHist(face, equalizedWholeFace);

            Mat equalizedLeftFace = face.Clone(new Rect(0, 0, face.Cols / 2, face.Rows));
            Mat equalizedRightFace = face.Clone(new Rect(face.Cols / 2, 0, face.Cols / 2, face.Rows));
            Cv2.EqualizeHist(equalizedLeftFace, equalizedLeftFace);
            Cv2.EqualizeHist(equalizedRightFace, equalizedRightFace);

            for(int i = 0; i < face.Rows; i++)
            {
                for(int j = 0; j < face.Cols; j++)
                {
                    int value = 0;
                    if (j < face.Cols / 4)
                    {
                        value = equalizedLeftFace.At<byte>(i, j);
                    }
                    else if (j < face.Cols / 2)
                    {
                        int leftValue = equalizedLeftFace.At<byte>(i, j);
                        int wholeValue = equalizedWholeFace.At<byte>(i, j);
                        float leftRatio = 1 - (j - face.Cols / 4) / (float)(face.Cols / 4);
                        value = Convert.ToInt32(Math.Round(leftRatio * leftValue + (1 - leftRatio) * wholeValue));
                    }
                    else if (j < face.Cols * 3 / 4)
                    {
                        int rightValue = equalizedRightFace.At<byte>(i, j);
                        int wholeValue = equalizedWholeFace.At<byte>(i, j);
                        float rightRatio = (j - face.Cols / 2) / (float)(face.Cols / 4);
                        value = Convert.ToInt32(Math.Round(rightValue * rightRatio + (1 - rightRatio) * wholeValue));
                    }
                    else
                    {
                        value = equalizedRightFace.At<byte>(i, j);
                    }
                    face.At<byte>(i, j) = value > 255.0 ? (byte)255.0 : (byte)value;
                }
            }

            AdjustedFace = face;
            //Cv2.ImShow("equalized", face);
            //Cv2.WaitKey();
        }

        private Mat MaskProcedure(Mat input)
        {
            Mat mask = new Mat(AdjustedFace.Size(), MatType.CV_8UC1, new Scalar(255));
            Point faceCenter = new Point(Convert.ToInt32(Math.Round(mask.Width * 0.5d)),
                Convert.ToInt32(Math.Round(mask.Height * 0.4d)));
            Size ellipseSize = new Size(Convert.ToInt32(Math.Round(mask.Width * 0.5)),
                Convert.ToInt32(Math.Round(mask.Height * 0.8)));
            Cv2.Ellipse(mask, faceCenter, ellipseSize, 0, 0, 360, new Scalar(0), -1);
            input.SetTo(new Scalar(128), mask);
            Cv2.ImShow("masked", input);
            Cv2.WaitKey();
            return input;
        }
    }
}
