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
        protected int bytesPerPixel;
        protected int width;
        protected int height;

        protected Algorithm(WriteableBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            this.width = bitmap.PixelWidth;
            this.height = bitmap.PixelHeight;
        }

        public abstract void Apply(int K);
    }
}
