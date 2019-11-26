using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public abstract class Algorithm
    {
        protected WriteableBitmap bitmap;
        protected byte[] originalCopy;
        protected int bytesPerPixel;
        protected int width;
        protected int height;

        protected Algorithm()
        {
        }

        public virtual WriteableBitmap Bitmap
        {
            get => bitmap; 
            set 
            {
                bitmap = value;
                bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
                width = bitmap.PixelWidth;
                height = bitmap.PixelHeight;

                originalCopy = new byte[height * bitmap.BackBufferStride];
                bitmap.CopyPixels(originalCopy, bitmap.BackBufferStride, 0);
            }
        }

        public abstract void Apply(int Kr, int Kg, int Kb);


        protected static int RoundToNeareastMultiple(int num, int multiple)
        {
            return (int)(((num + multiple / 2) / multiple) * multiple);
        }
    }
}
