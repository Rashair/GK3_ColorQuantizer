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
        protected WriteableBitmap originalCopy;
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
                originalCopy = value.Clone();
                bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
                width = bitmap.PixelWidth;
                height = bitmap.PixelHeight;
            }
        }

        public abstract void Apply(int Kr, int Kg, int Kb);
    }
}
