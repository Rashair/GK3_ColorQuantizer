using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GK3_ColorQuantizer.Algorithms;

namespace GK3_ColorQuantizer
{
    public class AverageDitheringAlgorithm : Algorithm
    {
        const int cubeSize = 8;
        int[] cubes;

        public AverageDitheringAlgorithm(WriteableBitmap bitmap) : base(bitmap)
        {
            cubes = new int[(bitmap.PixelWidth * bitmap.PixelHeight)];
            unsafe
            {
                byte* bmpArray = (byte*)bitmap.BackBuffer.ToPointer();
                for (int i = 0; i < bitmap.PixelHeight; ++i)
                {
                    for (int j = 0; j < bitmap.PixelWidth; ++j)
                    {
                        cubes[i * bitmap.PixelWidth + j] = ComputeCubeSum(bmpArray, i, j);
                    }
                }
            }
        }

        private unsafe int ComputeCubeSum(byte* bmp, int x, int y)
        {
            int sum = 0;
            int xEnd = Math.Min(x + cubeSize, bitmap.PixelWidth);
            int yEnd = Math.Min(y + cubeSize, bitmap.PixelHeight);
            for (int row = x; row < xEnd; ++row)
            {
                var pos = bmp + row * bitmap.BackBufferStride + y * bytesPerPixel;
                for (int col = y; col < yEnd; ++col)
                {
                    sum += (pos[0] + pos[1] + pos[2]) / 3;
                    pos += bytesPerPixel;
                }
            }

            return sum / ((xEnd - x) * (yEnd - y));
        }


        public override void Apply(int K)
        {
            bitmap.Lock();
            unsafe
            {
                byte* bmpArray = (byte*)bitmap.BackBuffer.ToPointer();

                for (int i = 0; i < bitmap.PixelHeight; ++i)
                {
                    byte* currRow = bmpArray + i * bitmap.BackBufferStride;
                    for (int j = 0; j < bitmap.PixelWidth; ++j)
                    {
                        int avg = ((int)currRow[0] + currRow[1] + currRow[2]) / 3;
                        if (avg > (cubes[i * bitmap.PixelWidth + j]))
                        {
                            currRow[0] = currRow[1] = currRow[2] = 255;
                        }
                        else
                        {
                            currRow[0] = currRow[1] = currRow[2] = 0;
                        }
                        currRow[3] = 255;

                        currRow += bytesPerPixel;
                    }
                }
            }
            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
        }
    }
}