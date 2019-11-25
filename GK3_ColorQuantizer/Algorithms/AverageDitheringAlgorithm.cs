using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GK3_ColorQuantizer.Algorithms;

namespace GK3_ColorQuantizer
{
    public class AverageDitheringAlgorithm : Algorithm
    {
        public AverageDitheringAlgorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        public override WriteableBitmap Bitmap
        {
            get => base.Bitmap;
            set
            {
                base.Bitmap = value;
            }
        }



        public override void Apply(int Kr, int Kg, int Kb)
        {
            int itR = 255 / (Kr - 1);
            int itG = 255 / (Kg - 1);
            int itB = 255 / (Kb - 1);

            Bitmap.Lock();
            unsafe
            {
                byte* bmpArray = (byte*)Bitmap.BackBuffer.ToPointer();
                byte* copyArray = (byte*)originalCopy.BackBuffer.ToPointer();

                for (int i = 0; i < Bitmap.PixelHeight; ++i)
                {
                    byte* currRow = bmpArray + i * Bitmap.BackBufferStride;
                    byte* currRowCopy = copyArray + i * Bitmap.BackBufferStride;
                    for (int j = 0; j < Bitmap.PixelWidth; ++j)
                    {
                        currRow[0] = (byte)RoundToNeareastMultiple(currRowCopy[0], itR);
                        currRow[1] = (byte)RoundToNeareastMultiple(currRowCopy[1], itG);
                        currRow[2] = (byte)RoundToNeareastMultiple(currRowCopy[2], itB);

                        currRow += bytesPerPixel;
                        currRowCopy += bytesPerPixel;
                    }
                }
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight));
            Bitmap.Unlock();
        }

        private static int RoundToNeareastMultiple(int num, int multiple)
        {
            return ((num + multiple / 2) / multiple) * multiple;
        }
    }
}