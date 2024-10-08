﻿using FaceONNX;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UMapx.Imaging;
using UMapx.Visualization;

namespace ONNXFaceExample1.Utils
{
    internal class EyeBlinkDetectionUtil
    {

        public static Bitmap FromBitmap(Bitmap bitmap)
        {
            Debug.WriteLine("FaceONNX: Eye blink detection");

            using var faceDetector = new FaceDetector();
            using var faceLandmarksExtractor = new Face68LandmarksExtractor();
            using var eyeBlinkClassifier = new EyeBlinkClassifier();

            using var painter = new Painter()
            {
                PointPen = new Pen(Color.Red, 4),
                Transparency = 0,
                TextFont = new Font("Arial", 4)
            };

            var faces = faceDetector.Forward(bitmap);


            foreach (var face in faces)
            {
                // crop and align face
                var box = face.Box;
                using var cropped = BitmapTransform.Crop(bitmap, box);
                var points = faceLandmarksExtractor.Forward(cropped);

                // eye blink detection
                var left_eye_rect = Face68Landmarks.GetLeftEyeRectangle(points);
                var right_eye_rect = Face68Landmarks.GetRightEyeRectangle(points);

                using var left_eye = BitmapTransform.Crop(cropped, left_eye_rect);
                using var right_eye = BitmapTransform.Crop(cropped, right_eye_rect);

                var left_eye_value = eyeBlinkClassifier.Forward(left_eye);
                var right_eye_value = eyeBlinkClassifier.Forward(right_eye);

                // drawing face detection and
                // landmarks extraction results
                using var graphics = Graphics.FromImage(bitmap);

                var point = box.GetPoint();
                var paintData = new PaintData
                {
                    Points = points.All.Add(point),
                    Title = string.Empty,
                };

                painter.Draw(graphics, paintData);

                // drawing eye bling detection results
                var paintLeftEyeData = new PaintData
                {
                    Rectangle = left_eye_rect.Add(point),
                    Labels = ToString(left_eye_value)
                };

                var paintRightEyeData = new PaintData
                {
                    Rectangle = right_eye_rect.Add(point),
                    Labels = ToString(right_eye_value)
                };

                painter.Draw(graphics, paintLeftEyeData);
                painter.Draw(graphics, paintRightEyeData);

                
            }
            return bitmap;
        }

        private static string[] ToString(float[] tensor)
        {
            var value = Math.Round(tensor[0], 1);
            return [value.ToString()];
        }
    }
}
