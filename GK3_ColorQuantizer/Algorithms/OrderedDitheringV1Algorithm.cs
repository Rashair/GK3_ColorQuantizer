using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class OrderedDitheringV1Algorithm : Algorithm
    {
        private static readonly int[] matrixSizes = { 2, 3, 4, 6, 8, 12, 16 };
        private static readonly Dictionary<int, int[,]> matrices;
        private readonly Random random;

        static OrderedDitheringV1Algorithm()
        {
            matrices = new Dictionary<int, int[,]>(matrixSizes.Length);

            matrices[2] = new int[,]
            {
                {0, 2},
                {3, 1}
            };

            matrices[3] = new int[,]
            {
                {6, 8, 4},
                {1, 0, 3},
                {5, 2, 7}
            };

            for (int i = 2; i < matrixSizes.Length; ++i)
            {
                int size = matrixSizes[i];
                matrices[size] = new int[size, size];

                int n = size / 2;
                for (int k = 0; k < n; ++k)
                {
                    for (int j = 0; j < n; ++j)
                    {
                        matrices[size][k, j] = 4 * matrices[n][k, j];
                    }
                }

                for (int k = 0; k < n; ++k)
                {
                    for (int j = n; j < size; ++j)
                    {
                        matrices[size][k, j] = 4 * matrices[n][k, j - n] + 2;
                    }
                }

                for (int k = n; k < size; ++k)
                {
                    for (int j = 0; j < n; ++j)
                    {
                        matrices[size][k, j] = 4 * matrices[n][k - n, j] + 3;
                    }
                }

                for (int k = n; k < size; ++k)
                {
                    for (int j = n; j < size; ++j)
                    {
                        matrices[size][k, j] = 4 * matrices[n][k - n, j - n] + 1;
                    }
                }
            }
        }

        public OrderedDitheringV1Algorithm(WriteableBitmap bitmap)
        {
            this.Bitmap = bitmap;
            this.random = new Random();
        }

        public override void Apply(int Kr, int Kg, int Kb)
        {
            int nR = ComputeN(Kr);
            int nG = ComputeN(Kg);
            int nB = ComputeN(Kb);

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
                        currPos[0] = ComputePixel(copyIt, nB, itB, 0);
                        currPos[1] = ComputePixel(copyIt, nG, itG, 1);
                        currPos[2] = ComputePixel(copyIt, nR, itR, 2);

                        currPos += bytesPerPixel;
                        copyIt += bytesPerPixel;
                    }
                }
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            Bitmap.Unlock();
        }

        /// <summary> 
        /*
            Ii = GetPixel(x,y); // Ii is in range <0,n2 *(k-1)>
            col = Ii div n2
             re = Ii mod n2
             i = x mod n
             j = y mod n
             if re >Dn[i, j] col++
             PutPixel(x, y, col);
        */
        /// </summary>

        private unsafe byte ComputePixel(int copyIt, int n, int it, int colorNum)
        {
            int Ii = originalCopy[copyIt + colorNum];
            int i = random.Next(0, n);
            int j = random.Next(0, n);
            if (n == 16)
            {
                return (byte)(Ii > matrices[n][i, j] ? 255 : 0);
            }

            int col = Ii / (n * n);
            int re = Ii % (n * n);

            return (it * (re > matrices[n][i, j] ? (col + 1) : col)).ToByte();
        }

        private int ComputeN(int k)
        {
            int n = (int)(16 / Math.Sqrt(k - 1));
            int ind = Array.BinarySearch(matrixSizes, n);
            return matrixSizes[ind > 0 ? ind : ~ind];
        }
    }
}
