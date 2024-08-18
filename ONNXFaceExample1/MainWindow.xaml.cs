using ONNXFaceExample1.Utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Window = System.Windows.Window;

namespace ONNXFaceExample1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VideoCapture capture;
        private readonly BackgroundWorker bkgWorker;

        public enum Mode { 
            None,
            Detect,
            Info,
            EyeBlink,
            FaceLandmarks
        };

        private Mode _mode = Mode.None;
        public MainWindow()
        {
            InitializeComponent();

            capture = new VideoCapture();

            bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            bkgWorker.DoWork += Worker_DoWork!;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }
        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            capture.Open(0, VideoCaptureAPIs.DSHOW);
            if (!capture.IsOpened())
            {
                Close();
                return;
            }

            bkgWorker.RunWorkerAsync();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bkgWorker.CancelAsync();
            capture.Dispose();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                Mat frameMat = new Mat();
                //capture.Grab();
                if (capture.Retrieve(frameMat))
                {
                    if (frameMat.Empty())
                    {
                        continue;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        if (_mode == Mode.Detect)
                        {
                            imgBox.Source = FaceDetectionUtil.FromBitmap(frameMat.ToBitmap()).ToBitmapSource();
                            bkgWorker.CancelAsync();
                        }
                        else if(_mode == Mode.EyeBlink)
                        {
                            imgBox.Source = EyeBlinkDetectionUtil.FromBitmap(frameMat.ToBitmap()).ToBitmapSource();
                            bkgWorker.CancelAsync();
                        }
                        else if (_mode == Mode.FaceLandmarks)
                        {
                            imgBox.Source = FaceLandmarksExtractionUtil.FromBitmap(frameMat.ToBitmap()).ToBitmapSource();
                            bkgWorker.CancelAsync();
                        }
                        else if (_mode == Mode.Info)
                        {
                            imgBox.Source = frameMat.ToWriteableBitmap();
                            Bitmap infoImg = frameMat.ToBitmap();

                            string ageResult = ClassificationInfoUtil.AgeGender(infoImg);
                            string spoofResult = ClassificationInfoUtil.AntispoofingDepth(infoImg);
                            string emotionEstimationResult = ClassificationInfoUtil.EmotionAndBeautyEstimation(infoImg);

                            tbStatus.Text = ageResult + Environment.NewLine 
                            + spoofResult + Environment.NewLine 
                            + emotionEstimationResult;

                            bkgWorker.CancelAsync();
                        }
                        else
                        {
                            imgBox.Source = frameMat.ToWriteableBitmap();
                        }
                    });
                }


                if (worker.CancellationPending)
                {
                    break;
                }

                Thread.Sleep(16);
            }
        }


        private void btnFaceDetection_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            _mode = Mode.Detect;
            


            //string rootDir = AppDomain.CurrentDomain.BaseDirectory;

            //var files = Directory.GetFiles(rootDir + @"images", "*.*", SearchOption.AllDirectories);
            //var path = rootDir + @"results";
            //Directory.CreateDirectory(path);

            //foreach (var file in files)
            //{
            //    var filename = System.IO.Path.GetFileName(file);
            //    FaceDetectionUtil.Check(file, System.IO.Path.Combine(path, filename));
            //}
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            _mode = Mode.None;
            if(!bkgWorker.IsBusy)
            {
                bkgWorker.RunWorkerAsync();
            }
        }


        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            _mode = Mode.Info;
        }

        private void btnEyeBlinkDetection_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            _mode = Mode.EyeBlink;
        }

        private void btnFaceLandmarks_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = string.Empty;
            _mode = Mode.FaceLandmarks;
        }
    }
}