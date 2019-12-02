using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class OrderedDitheringV2Algorithm : OrderedDitheringAlgorithm
    {
        public OrderedDitheringV2Algorithm(WriteableBitmap bitmap) : base(bitmap)
        {
        }

        protected override (int i, int j) GetCoordinates(int x, int y, int n)
        {
            return (x % n, y % n);
        }
    }
}
