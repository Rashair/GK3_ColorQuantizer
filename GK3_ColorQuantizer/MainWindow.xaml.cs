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
        int width;
        int height;
        Algorithm algorithm;

        // Interface
        const int minMargin = 10;
        Uri imagePath = new Uri(@"pack://application:,,,/Resources/Lenna.png");
        WriteableBitmap imageBitmap;

        int K;


        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            width = (int)mainCanvas.ActualWidth;
            height = (int)mainCanvas.ActualHeight;

            SetBitmapImage();
        }

        private void DrawOnBitmap()
        {

        }


        private void SetBitmapImage()
        {
            BitmapSource img = new BitmapImage(imagePath);
            if (img.Height > height || img.Width > width)
            {
                img = new TransformedBitmap(img,
                    new ScaleTransform(width / (img.Width + 2 * minMargin), height / (img.Height + 2 * minMargin)));
            }
            this.imageBitmap = new WriteableBitmap(img);

            var host = new Image();
            Canvas.SetLeft(host, (width - imageBitmap.Width) / 2);
            Canvas.SetTop(host, (height - imageBitmap.Height) / 2);
            host.Source = imageBitmap;

            mainCanvas.Children.Clear();
            mainCanvas.Children.Add(host);
        }

        private void K_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (kInput.Value.HasValue)
            {
                this.K = kInput.Value.Value;
            }
        }

        private void AverageDitheringRadio_Checked(object sender, RoutedEventArgs e)
        {
            this.algorithm = new AverageDitheringAlgorithm(imageBitmap);
            algorithm.Apply(K);
            DrawOnBitmap();
        }

        private void ErrorDiffusionDitheringRadio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OrderedDitheringV1Radio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OrderedDitheringV2Radio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PopularityAlgorithmRadio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NoneRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (imageBitmap != null)
            {
                SetBitmapImage();
            }
        }

        private void ChoosePictureButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Images (*.jpg, *.jpeg, *.jpe, *.jfif, *.png)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png"
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                this.imagePath = new Uri(dlg.FileName);
                SetBitmapImage();
            }
        }
    }
}
