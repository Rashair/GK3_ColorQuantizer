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
                int copyIt = 0;
                byte* currPos = (byte*)Bitmap.BackBuffer.ToPointer();
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        currPos[0] = RoundToNeareastMultiple(originalCopy[copyIt], itB).ToByte();
                        currPos[1] = RoundToNeareastMultiple(originalCopy[copyIt + 1], itG).ToByte();
                        currPos[2] = RoundToNeareastMultiple(originalCopy[copyIt + 2], itR).ToByte();

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            Bitmap.Unlock();
        }
    }
}