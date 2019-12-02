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
        private static readonly int[] colorCounts;
        private static readonly int[] indexes;

        static PopularityAlgorithm()
        {
            colorCounts = new int[256 * 256 * 256];
            indexes = Enumerable.Range(0, 256 * 256 * 256).ToArray();
        }

        private int[] currentPalette;

        public PopularityAlgorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;

            for (int i = 0; i < originalCopy.Length; i += bytesPerPixel)
            {
                int col = (originalCopy[i] << 16) | (originalCopy[i + 1] << 8) | originalCopy[i + 2];
                colorCounts[col] += 1;
            }

            Array.Sort(colorCounts, indexes, Comparer<int>.Create((a, b) => b.CompareTo(a)));
        }

        public override void Apply(int Kr, int Kg, int Kb)
        {
            int K = Kr;
            currentPalette = indexes.Take(K).ToArray();

            Bitmap.Lock();
            unsafe
            {
                int copyIt = 0;
                byte* currPos = (byte*)Bitmap.BackBuffer.ToPointer();
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        int color = (originalCopy[copyIt] << 16) | (originalCopy[copyIt + 1] << 8) | (originalCopy[copyIt + 2]);
                        var (B, G, R) = GetNearestColor(color);
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

        private (int B, int G, int R) GetNearestColor(int color)
        {
            int bestColor = indexes[0];
            int minDifference = int.MaxValue;

            var (Bcolor, Gcolor, Rcolor) = GetRGB(color);
            for (int i = 0; i < currentPalette.Length; ++i)
            {
                int targetColor = currentPalette[i];
                var (B, G, R) = GetRGB(targetColor);

                int dist = GetDistanceBetweenColors(R, Rcolor, G, Gcolor, B, Bcolor);
                // if a difference is zero, we're good because it won't get better
                if (dist == 0)
                {
                    bestColor = currentPalette[i];
                    break;
                }

                if (dist < minDifference)
                {
                    minDifference = dist;
                    bestColor = currentPalette[i];
                }
            }

            return GetRGB(bestColor);
        }

        private (int B, int G, int R) GetRGB(int col)
        {
            return ((col >> 16) & 0xFF, (col >> 8) & 0xFF, col & 0xFF);
        }

        private int GetDistanceBetweenColors(int R1, int R2, int G1, int G2, int B1, int B2)
        {
            int deltaR = R1 - R2;
            int deltaG = G1 - G2;
            int deltaB = B1 - B2;

            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }
    }
}
