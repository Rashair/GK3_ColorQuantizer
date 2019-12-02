using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class OrderedDitheringV1Algorithm : OrderedDitheringAlgorithm
    {
        private readonly Random random;

        public OrderedDitheringV1Algorithm(WriteableBitmap bitmap) : base(bitmap)
        {
            this.random = new Random();
        }

        protected override (int i, int j) GetCoordinates(int x, int y, int n)
        {
            return (random.Next(0, n), random.Next(0, n));
        }
    }
}
