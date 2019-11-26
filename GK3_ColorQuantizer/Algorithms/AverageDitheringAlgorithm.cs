using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class AverageDitheringAlgorithm : Algorithm
    {
        public AverageDitheringAlgorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;
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
                        currRow[0] = RoundToNeareastMultiple(currRowCopy[0], itB).ToByte();
                        currRow[1] = RoundToNeareastMultiple(currRowCopy[1], itG).ToByte();
                        currRow[2] = RoundToNeareastMultiple(currRowCopy[2], itR).ToByte();

                        currRow += bytesPerPixel;
                        currRowCopy += bytesPerPixel;
                    }
                }
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight));
            Bitmap.Unlock();
        }
    }
}