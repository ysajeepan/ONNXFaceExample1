using FaceONNX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMapx.Imaging;
using UMapx.Visualization;

namespace ONNXFaceExample1.Utils
{
    class FaceLandmarksExtractionUtil
    {
        public static Bitmap FromBitmap(Bitmap bitmap)
        {
            using var faceDetector = new FaceDetector();
            using var faceLandmarksExtractor = new Face68LandmarksExtractor();
            using var painter = new Painter()
            {
                PointPen = new Pen(Color.Yellow, 4),
                Transparency = 0,
            };
            var faces = faceDetector.Forward(bitmap);

            foreach (var face in faces)
            {
                // crop face
                var box = face.Box;
                using var cropped = BitmapTransform.Crop(bitmap, box);
                var points = faceLandmarksExtractor.Forward(cropped);

                var paintData = new PaintData()
                {
                    Points = points.All.Add(box.GetPoint()),
                    Title = string.Empty,
                };

                using var graphics = Graphics.FromImage(bitmap);
                painter.Draw(graphics, paintData);
            }
            return bitmap;
        }
    }
}
