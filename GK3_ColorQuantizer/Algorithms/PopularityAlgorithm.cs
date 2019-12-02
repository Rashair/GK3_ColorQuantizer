using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class PopularityAlgorithm : Algorithm
    {
        private int[] colorCounts;
        private int[] indexes;
        private (int B, int G, int R)[] currentPalette;

        public PopularityAlgorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;
            colorCounts = new int[256 * 256 * 256];
            indexes = Enumerable.Range(0, 256 * 256 * 256).ToArray();

            for (int i = 0; i < originalCopy.Length; i += bytesPerPixel)
            {
                int col = (originalCopy[i] << 16) | (originalCopy[i + 1] << 8) | originalCopy[i + 2];
                ++colorCounts[col];
            }

            var rnd = new Random();
           // indexes = indexes.OrderBy(x => rnd.Next()).ToArray();
            Array.Sort(colorCounts, indexes, Comparer<int>.Create((a, b) => b.CompareTo(a)));
        }

        public override void Apply(int Kr, int Kg, int Kb)
        {
            int K = Kr;
            currentPalette = indexes.Take(K).Select(x => GetBGR(x)).ToArray();

            Bitmap.Lock();
            unsafe
            {
                int copyIt = 0;
                byte* currPos = (byte*)Bitmap.BackBuffer.ToPointer();
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        var (B, G, R) = GetNearestColor((originalCopy[copyIt], originalCopy[copyIt + 1], originalCopy[copyIt + 2]));
                        currPos[0] = B.ToByte();
                        currPos[1] = G.ToByte();
                        currPos[2] = R.ToByte();

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            Bitmap.Unlock();
        }

        private (int B, int G, int R) GetNearestColor((int B, int G, int R) col)
        {
            var bestColor = currentPalette[0];
            var minDifference = GetDistanceBetweenColors(currentPalette[0], col);
            for (int i = 1; i < currentPalette.Length && minDifference > 0; ++i)
            {
                int dist = GetDistanceBetweenColors(currentPalette[i], col);

                if (dist < minDifference)
                {
                    minDifference = dist;
                    bestColor = currentPalette[i];
                }
            }

            return bestColor;
        }

        private (int B, int G, int R) GetBGR(int col)
        {
            return ((col >> 16) & 0xFF, (col >> 8) & 0xFF, col & 0xFF);
        }

        private int GetDistanceBetweenColors((int B, int G, int R) x1, (int B, int G, int R) x2)
        {

            int deltaB = x1.B - x2.B;
            int deltaG = x1.G - x2.G;
            int deltaR = x1.R - x2.R;

            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }
    }
}
