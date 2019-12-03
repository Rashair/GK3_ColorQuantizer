using GK3_ColorQuantizer.Algorithms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GK3_ColorQuantizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Algorithm algorithm;
        int K;
        int Kr;
        int Kg;
        int Kb;

        // Interface
        const int minMargin = 10;
        int width;
        int height;
        Uri imagePath = new Uri(@"pack://application:,,,/Resources/baboon.png");
        WriteableBitmap imageBitmap;
        WriteableBitmap algorithmBitmap;
        Window currAlgWindow;

        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetColorInputsVisibility(Visibility.Collapsed);
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            width = (int)mainCanvas.ActualWidth;
            height = (int)mainCanvas.ActualHeight;

            SetBitmapImage();
        }

        private void SetBitmapImage()
        {
            BitmapSource img = new BitmapImage(imagePath);
            if (img.PixelHeight > height || img.PixelWidth > width)
            {
                img = new TransformedBitmap(img,
                    new ScaleTransform(width / (double)(img.PixelWidth + 2 * minMargin), height / (double)(img.PixelHeight + 2 * minMargin)));
            }
            if (img.Format != PixelFormats.Bgr24)
            {
                img = new FormatConvertedBitmap(img, PixelFormats.Bgr24, null, 0);
            }

            this.imageBitmap = new WriteableBitmap(img);
            var host = new Image();
            Canvas.SetLeft(host, (width - imageBitmap.Width) / 2);
            Canvas.SetTop(host, (height - imageBitmap.Height) / 2);
            host.Source = imageBitmap;

            algorithmBitmap = imageBitmap.Clone();

            mainCanvas.Children.Clear();
            mainCanvas.Children.Add(host);
        }


        private void NoneRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (imageBitmap != null)
            {
                algorithm = null;
                SetBitmapImage();
                SetColorInputsVisibility(Visibility.Collapsed);
            }
        }

        private void NoneRadio_Unchecked(object sender, RoutedEventArgs e)
        {
            SetColorInputsVisibility(Visibility.Visible);
        }


        private void AverageDitheringRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetBitmapImage();
            this.algorithm = new AverageDitheringAlgorithm(algorithmBitmap);
            algorithm.Apply(Kr, Kg, Kb);

            OpenNewWindow();
        }

        private void ErrorDiffusionDitheringRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetBitmapImage();
            this.algorithm = new ErrorDiffusionDitheringAlgorithm(algorithmBitmap);
            algorithm.Apply(Kr, Kg, Kb);

            OpenNewWindow();
        }

        private void OrderedDitheringV1Radio_Checked(object sender, RoutedEventArgs e)
        {
            SetBitmapImage();
            this.algorithm = new OrderedDitheringV1Algorithm(algorithmBitmap);
            algorithm.Apply(Kr, Kg, Kb);

            OpenNewWindow();
        }

        private void OrderedDitheringV2Radio_Checked(object sender, RoutedEventArgs e)
        {
            SetBitmapImage();
            this.algorithm = new OrderedDitheringV2Algorithm(algorithmBitmap);
            algorithm.Apply(Kr, Kg, Kb);

            OpenNewWindow();

        }

        private void PopularityAlgorithmRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetBitmapImage();
            this.algorithm = new PopularityAlgorithm(algorithmBitmap);
            algorithm.Apply(K, K, K);

            OpenNewWindow();

            SetColorInputsVisibility(Visibility.Hidden);
        }

        private void OpenNewWindow()
        {
            // TODO: Bug with image resizes
            const int margin = 50;
            currAlgWindow = new Window { Owner = this };
            var w = algorithmBitmap.Width;
            var h = algorithmBitmap.Height;
            var canvas = new Canvas { Width = w + +margin * 2, Height = h + margin * 2 };

            var img = new Image { Source = algorithmBitmap, UseLayoutRounding = false, SnapsToDevicePixels = true };
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);

            Canvas.SetLeft(img, (canvas.Width - w) / 2);
            Canvas.SetTop(img, (canvas.Height - h) / 2);
            canvas.Children.Add(img);

            currAlgWindow.SizeToContent = SizeToContent.WidthAndHeight;
            currAlgWindow.Content = canvas;
            currAlgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            currAlgWindow.Show();
        }

        private void PopularityAlgorithmRadio_Unchecked(object sender, RoutedEventArgs e)
        {
            SetColorInputsVisibility(Visibility.Visible);
        }

        private void SetColorInputsVisibility(Visibility visible)
        {
            if (visible == Visibility.Visible)
            {
                kRInput.Visibility = Visibility.Visible;
                kGInput.Visibility = Visibility.Visible;
                kBInput.Visibility = Visibility.Visible;
                kInput.Visibility = Visibility.Collapsed;

            }
            else if (visible == Visibility.Hidden)
            {
                kRInput.Visibility = Visibility.Collapsed;
                kGInput.Visibility = Visibility.Collapsed;
                kBInput.Visibility = Visibility.Collapsed;
                kInput.Visibility = Visibility.Visible;
            }
            else
            {
                kRInput.Visibility = Visibility.Collapsed;
                kGInput.Visibility = Visibility.Collapsed;
                kBInput.Visibility = Visibility.Collapsed;
                kInput.Visibility = Visibility.Collapsed;
            }
        }


        private void K_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (kInput.Value.HasValue)
            {
                this.K = kInput.Value.Value;
                algorithm?.Apply(K, K, K);
            }
        }

        private void kRInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (kRInput.Value.HasValue)
            {
                this.Kr = kRInput.Value.Value;
                algorithm?.Apply(Kr, Kg, Kb);
            }
        }

        private void kGInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (kGInput.Value.HasValue)
            {
                this.Kg = kGInput.Value.Value;
                algorithm?.Apply(Kr, Kg, Kb);
            }
        }

        private void kBInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (kBInput.Value.HasValue)
            {
                this.Kb = kBInput.Value.Value;
                algorithm?.Apply(Kr, Kg, Kb);
            }
        }


        private void ChoosePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Images (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp, *.gif, *.tiff)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.bmp;*.gif;*.tiff"
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                currAlgWindow?.Close();
                this.imagePath = new Uri(dlg.FileName);
                SetBitmapImage();
                NoneRadio.IsChecked = true;
            }
        }

        WriteableBitmap bmp1;
        WriteableBitmap bmp2;
        WriteableBitmap bmp3;
        WriteableBitmap bmp4;

        private void GeneratePictureButton_Click(object sender, RoutedEventArgs e)
        {
            const int size = 384;

            mainCanvas.Children.Clear();
            int marg = 10;

            bmp1 = BitmapFactory.New(size, size);
            SetTriangles(bmp1);
            var host1 = new Image();
            Canvas.SetLeft(host1, marg);
            Canvas.SetTop(host1, marg);
            host1.Source = bmp1;
            mainCanvas.Children.Add(host1);


            bmp2 = new WriteableBitmap(bmp1);
            ConvertToGreyScale1(bmp2);
            var host2 = new Image();
            Canvas.SetLeft(host2, marg + size);
            Canvas.SetTop(host2, marg);
            host2.Source = bmp2;
            mainCanvas.Children.Add(host2);


            bmp3 = new WriteableBitmap(bmp1);
            ConvertToGreyScale2(bmp3);
            var host3 = new Image();
            Canvas.SetLeft(host3, marg);
            Canvas.SetTop(host3, size + marg - 1);
            host3.Source = bmp3;
            mainCanvas.Children.Add(host3);


            bmp4 = new WriteableBitmap(bmp1);
            ConvertToAverage(bmp4);
            var host4 = new Image();
            Canvas.SetLeft(host4, size + marg);
            Canvas.SetTop(host4, size + marg - 1);
            host4.Source = bmp4;
            mainCanvas.Children.Add(host4);
        }

        private void SetTriangles(WriteableBitmap bmp)
        {
            int bytesPerPixel = (bmp.Format.BitsPerPixel + 7) / 8;

            using var context = bmp.GetBitmapContext();
            var size = bmp.PixelWidth / 2;
            bmp.FillTriangle(0, 0, 0, size, size, size, Colors.Red);
            bmp.FillTriangle(0, 0, size, 0, size, size, Colors.Lime);

            bmp.FillTriangle(size, 0, size, size, size + size, size, Colors.Blue);
            bmp.FillTriangle(size, 0, size + size, 0, size + size, size, Colors.Cyan);

            bmp.FillTriangle(0, size, 0, size + size, size, size + size, Colors.Magenta);
            bmp.FillTriangle(0, size, size, size, size, size + size, Colors.Yellow);

            bmp.FillTriangle(size, size, size, size + size, size + size, size + size, Colors.White);
            bmp.FillTriangle(size, size, size + size, size, size + size, size + size, Colors.Black);

        }

        private void ConvertToGreyScale1(WriteableBitmap bmp)
        {
            int bytesPerPixel = (bmp.Format.BitsPerPixel + 7) / 8;
            bmp.Lock();

            unsafe
            {
                int copyIt = 0;
                byte* currPos = (byte*)bmp.BackBuffer.ToPointer();
                for (int i = 0; i < bmp.PixelHeight; ++i)
                {
                    for (int j = 0; j < bmp.PixelWidth; ++j)
                    {
                        byte val = ((int)(0.299 * currPos[2] + 0.587 * currPos[1] + 0.114 * currPos[0])).ToByte();
                        currPos[0] = val;
                        currPos[1] = val;
                        currPos[2] = val;

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            bmp.AddDirtyRect(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            bmp.Unlock();
        }

        private void ConvertToGreyScale2(WriteableBitmap bmp)
        {
            int bytesPerPixel = (bmp.Format.BitsPerPixel + 7) / 8;
            bmp.Lock();
            unsafe
            {
                int copyIt = 0;
                byte* currPos = (byte*)bmp.BackBuffer.ToPointer();
                for (int i = 0; i < bmp.PixelHeight; ++i)
                {
                    for (int j = 0; j < bmp.PixelWidth; ++j)
                    {
                        byte val = ((int)((currPos[2] + +currPos[1] + currPos[0]) / 3.0)).ToByte();
                        currPos[0] = val;
                        currPos[1] = val;
                        currPos[2] = val;

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            bmp.AddDirtyRect(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            bmp.Unlock();
        }

        private void ConvertToAverage(WriteableBitmap bmp)
        {
            int bytesPerPixel = (bmp.Format.BitsPerPixel + 7) / 8;
            bmp.Lock();
            unsafe
            {
                int copyIt = 0;
                byte* currPos = (byte*)bmp.BackBuffer.ToPointer();
                for (int i = 0; i < bmp.PixelHeight; ++i)
                {
                    for (int j = 0; j < bmp.PixelWidth; ++j)
                    {
                        var min = Math.Min(currPos[2], Math.Min(currPos[1], currPos[0]));
                        var max = Math.Max(currPos[2], Math.Max(currPos[1], currPos[0]));
                        byte val = ((min + max) / 2).ToByte();
                        currPos[0] = val;
                        currPos[1] = val;
                        currPos[2] = val;

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            bmp.AddDirtyRect(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            bmp.Unlock();
        }
    }
}
