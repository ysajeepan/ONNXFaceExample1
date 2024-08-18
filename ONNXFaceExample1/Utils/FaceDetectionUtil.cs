using FaceONNX;
using System.Drawing;
using UMapx.Visualization;

namespace ONNXFaceExample1.Utils
{
    internal class FaceDetectionUtil
    {
        public static bool Check(string imageFile, string resultFile)
        {
            using var faceDetector = new FaceDetector();
            using var painter = new Painter()
            {
                BoxPen = new Pen(Color.Yellow, 4),
                Transparency = 0,
            };

            using var bitmap = new Bitmap(imageFile);
            var outputs = faceDetector.Forward(bitmap);

            foreach (var output in outputs)
            {
                var paintData = new PaintData()
                {
                    Rectangle = output.Box,
                    Title = string.Empty
                };
                using var graphics = Graphics.FromImage(bitmap);
                painter.Draw(graphics, paintData);
            }

            
            bitmap.Save(resultFile);
            Console.WriteLine($"Image: [{imageFile}] --> detected [{outputs.Length}] faces");
            return true;
        }

        public static Bitmap FromBitmap(Bitmap bitmap)
        {
            using var faceDetector = new FaceDetector();
            using var painter = new Painter()
            {
                BoxPen = new Pen(Color.Yellow, 4),
                Transparency = 0,
            };

            var outputs = faceDetector.Forward(bitmap);
            foreach (var output in outputs)
            {
                var paintData = new PaintData()
                {
                    Rectangle = output.Box,
                    Title = string.Empty
                };
                using var graphics = Graphics.FromImage(bitmap);
                painter.Draw(graphics, paintData);
            }


            return bitmap;
        }
    }
}
