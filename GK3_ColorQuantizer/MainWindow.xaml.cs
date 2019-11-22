using System;
using System.Collections.Generic;
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
        WriteableBitmap bitmap;
        BitmapImage imageFile;
        private int width;
        private int height;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            width = (int)mainCanvas.ActualWidth;
            height = (int)mainCanvas.ActualHeight;
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            var path = new Uri(@"pack://application:,,,/Resources/Lenna.png");
            imageFile = new BitmapImage(path);
            bitmap.Clear(Colors.White);
            DrawOnBitmap(imageFile);

            var img = new Image();
            img.Source = bitmap;
            mainCanvas.Children.Add(img);
        }

        private void DrawOnBitmap(BitmapSource image)
        {
            var leftUpperCorner = new Point();
            if (image.PixelWidth > bitmap.PixelWidth || image.PixelHeight > bitmap.PixelHeight)
            {
                image = new TransformedBitmap(image, new ScaleTransform(bitmap.PixelWidth / (double)image.PixelWidth,
                    bitmap.PixelHeight / (double)image.PixelHeight));
            }
            else
            {
                leftUpperCorner.X = (bitmap.PixelWidth - image.PixelWidth) / 2;
                leftUpperCorner.Y = (bitmap.PixelHeight - image.PixelHeight) / 2;
            }


            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
            var stride = image.PixelWidth * bytesPerPixel;
            var buffer = new byte[image.PixelHeight * stride];
            image.CopyPixels(buffer, stride, 0);

            bitmap.WritePixels(new Int32Rect((int)leftUpperCorner.X, (int)leftUpperCorner.Y, 
                image.PixelWidth, image.PixelHeight), buffer, stride, 0);
        }
    }
}
