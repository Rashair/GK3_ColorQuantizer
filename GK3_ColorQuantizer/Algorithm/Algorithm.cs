using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithm
{
    public abstract class Algorithm
    {
        protected WriteableBitmap bitmap;

        protected Algorithm(WriteableBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public abstract void Apply(int K);
    }
}
