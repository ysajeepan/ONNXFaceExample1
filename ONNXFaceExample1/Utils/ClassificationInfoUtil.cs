using FaceONNX;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UMapx.Core;

namespace ONNXFaceExample1.Utils
{
    class ClassificationInfoUtil
    {
        public static string AgeGender(Bitmap bitmap)
        {
            using var faceDetector = new FaceDetector();
            using var faceLandmarksExtractor = new Face68LandmarksExtractor();
            using var faceGenderClassifier = new FaceGenderClassifier();
            using var faceAgeEstimator = new FaceAgeEstimator();
            var labels = FaceGenderClassifier.Labels;

            var faces = faceDetector.Forward(bitmap);
            int i = 1;

            string result = string.Empty;
            foreach (var face in faces)
            {
                Debug.Write($"\t[Face #{i++}]: ");

                var box = face.Box;
                var points = faceLandmarksExtractor.Forward(bitmap, box);
                var angle = points.RotationAngle;
                using var aligned = FaceProcessingExtensions.Align(bitmap, box, angle, false);

                var output = faceGenderClassifier.Forward(aligned);
                var max = Matrice.Max(output, out int gender);
                var label = labels[gender];
                var age = faceAgeEstimator.Forward(aligned);

                result += $"classified as [{label}] gender with probability [{output.Max()}] and [{age.First()}] ages" + Environment.NewLine;
            }
            string lb = "Age and gender classification: ";
            if (string.IsNullOrEmpty(result))
            {
                return lb + "No Record";
            }

            return lb + Environment.NewLine + result;
        }

        public static string AntispoofingDepth(Bitmap bitmap)
        {
            using var faceDepthClassifier = new FaceDepthClassifier();
            var labels = FaceDepthClassifier.Labels;

            var output = faceDepthClassifier.Forward(bitmap);
            var max = Matrice.Max(output, out int gender);
            var label = labels[gender];

            string result = "Antispoofing depth classification :" + Environment.NewLine;
            result += $"classified as [{label}] with probability [{output.Max()}]";

            return result;
        }

        public static string EmotionAndBeautyEstimation(Bitmap bitmap)
        {
            using var faceDetector = new FaceDetector();
            using var faceLandmarksExtractor = new Face68LandmarksExtractor();
            using var faceEmotionClassifier = new FaceEmotionClassifier();
            using var faceBeautyClassifier = new FaceBeautyClassifier();


            var faces = faceDetector.Forward(bitmap);
            int i = 1;

            string result = string.Empty;
            foreach (var face in faces)
            {
                Debug.Write($"\t[Face #{i++}]: ");

                var box = face.Box;
                var points = faceLandmarksExtractor.Forward(bitmap, box);
                var angle = points.RotationAngle;
                using var aligned = FaceProcessingExtensions.Align(bitmap, box, angle, false);
                var emotion = faceEmotionClassifier.Forward(aligned);
                var max = Matrice.Max(emotion, out int argmax);
                var emotionLabel = FaceEmotionClassifier.Labels[argmax];
                var beauty = faceBeautyClassifier.Forward(aligned);
                var beautyLabel = $"{Math.Round(2 * beauty.Max(), 1)}/10.0";

                result += $"classified as [{emotionLabel}] emotion and [{beautyLabel}] beauty";
            }
            return "Emotion and beauty estimation :" + Environment.NewLine + result;

        }
    }
}
