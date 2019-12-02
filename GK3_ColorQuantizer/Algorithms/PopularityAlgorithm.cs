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

        public PopularityAlgorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;

            for (int i = 0; i < originalCopy.Length; i += bytesPerPixel)
            {
                int col = (originalCopy[i] << 16) | (originalCopy[i + 1] << 8) | originalCopy[i + 2];
                colorCounts[col] += 1;
            }

            Array.Sort(colorCounts, indexes);
        }

        public override void Apply(int Kr, int Kg, int Kb)
        {
            int K = Kr;

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
                        var (B, G, R) = GetRGB(GetNearestColor(color, K));
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

        private int GetNearestColor(int color, int K)
        {
            // initializes the best difference, set it for worst possible, it can only get better
            int bestColor = indexes[0];
            int leastDifference = int.MaxValue;

            var (colorB, colorG, colorR) = GetRGB(color);
            for (int i = 0; i < K; ++i)
            {
                int targetColor = indexes[i];
                var (B, G, R) = GetRGB(targetColor);

                // calculates a difference for all the color components
                int deltaR = R - colorR;
                int deltaG = G - colorG;
                int deltaB = B - colorB;

                // calculates a power of two
                int factorR = deltaR * deltaR;
                int factorG = deltaG * deltaG;
                int factorB = deltaB * deltaB;

                // calculates the Euclidean distance, a square-root is not need 
                // as we're only comparing distance, not measuring it
                int difference = factorR + factorG + factorB;

                // if a difference is zero, we're good because it won't get better
                if (difference == 0)
                {
                    bestColor = indexes[i];
                    break;
                }

                // if a difference is the best so far, stores it as our best candidate
                if (difference < leastDifference)
                {
                    leastDifference = difference;
                    bestColor = indexes[i];
                }
            }

            // returns the palette index of the most similar color
            return bestColor;
        }

        private (int B, int G, int R) GetRGB(int col)
        {
            return ((col >> 16) & 0xFF, (col >> 8) & 0xFF, col & 0xFF);
        }
    }
}
