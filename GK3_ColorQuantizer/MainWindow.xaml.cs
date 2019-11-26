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
        Uri imagePath = new Uri(@"pack://application:,,,/Resources/lena_grayscale.bmp");
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
            if (img.Height > height || img.Width > width)
            {
                img = new TransformedBitmap(img,
                    new ScaleTransform(width / (img.Width + 2 * minMargin), height / (img.Height + 2 * minMargin)));
            }
            if (img.Format != PixelFormats.Bgr32)
            {
                img = new FormatConvertedBitmap(img, PixelFormats.Bgr32, null, 0);
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
            this.algorithm = new ErrorDiffusionDithering(algorithmBitmap);
            algorithm.Apply(Kr, Kg, Kb);

            OpenNewWindow();
        }

        private void OrderedDitheringV1Radio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OrderedDitheringV2Radio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenNewWindow()
        {
            // TODO: Bug with image resizes
            const int margin = 50;
            currAlgWindow = new Window { Owner = this };
            var w = imageBitmap.Width;
            var h = imageBitmap.Height;
            var canvas = new Canvas { Width = w + margin * 2, Height = h + margin * 2 };

            var img = new Image { Source = algorithmBitmap };
            Canvas.SetLeft(img, (canvas.Width - w) / 2);
            Canvas.SetTop(img, (canvas.Height - h) / 2);
            canvas.Children.Add(img);

            currAlgWindow.SizeToContent = SizeToContent.WidthAndHeight;
            currAlgWindow.Content = canvas;
            currAlgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            currAlgWindow.Show();
        }

        private void PopularityAlgorithmRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetColorInputsVisibility(Visibility.Hidden);
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
    }
}
