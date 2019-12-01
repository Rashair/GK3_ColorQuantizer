using System.Windows.Media.Imaging;

namespace GK3_ColorQuantizer.Algorithms
{
    public class ErrorDiffusionDithering : Algorithm
    {
        public ErrorDiffusionDithering(WriteableBitmap imageBitmap)
        {
            this.Bitmap = imageBitmap;
        }

        public override void Apply(int Kr, int Kg, int Kb)
        {
            int itR = 255 / (Kr - 1);
            int itG = 255 / (Kg - 1);
            int itB = 255 / (Kb - 1);
            var bmpRect = new System.Windows.Int32Rect(0, 0, width, height);
            bitmap.WritePixels(bmpRect, originalCopy, bitmap.BackBufferStride, 0); // Copy cached original image

            bitmap.Lock();

            unsafe
            {
                byte* bmpArray = (byte*)bitmap.BackBuffer.ToPointer();
                for (int i = 0; i < height; ++i)
                {
                    byte* currPos = bmpArray + i * bitmap.BackBufferStride;
                    Position row = GetPos(i, height);
                    for (int j = 0; j < width; ++j)
                    {
                        Position col = GetPos(j, width);

                        byte newVal = RoundToNeareastMultiple(currPos[0], itB).ToByte();
                        PropagateError(currPos[0] - newVal, 0, currPos, row, col);
                        currPos[0] = newVal;

                        newVal = RoundToNeareastMultiple(currPos[1], itG).ToByte();
                        PropagateError(currPos[1] - newVal, 1, currPos, row, col);
                        currPos[1] = newVal;

                        newVal = RoundToNeareastMultiple(currPos[2], itR).ToByte();
                        PropagateError(currPos[2] - newVal, 2, currPos, row, col);
                        currPos[2] = newVal;

                        currPos += bytesPerPixel;
                    }
                }
            }
            bitmap.AddDirtyRect(bmpRect);

            bitmap.Unlock();
        }


        private unsafe void PropagateError(int error, int colorNum, byte* currPos, Position row, Position col)
        {
            // x - from left to right
            // y - from top to bottom

            int ind;
            if (col != Position.Last)
            {
                ind = bytesPerPixel + colorNum;
                // pixel[x + 1][y] := pixel[x + 1][y] + quant_error * 7 / 16
                currPos[ind] = (currPos[ind] + ((error * 7) >> 4)).ToByte();
            }

            if (row != Position.Last)
            {
                if (col != Position.First)
                {
                    ind = bitmap.BackBufferStride - bytesPerPixel + colorNum;
                    // pixel[x - 1][y + 1] := pixel[x - 1][y + 1] + quant_error * 3 / 16
                    currPos[ind] = (currPos[ind] + ((error * 3) >> 4)).ToByte();
                }

                ind = bitmap.BackBufferStride + colorNum;
                // pixel[x][y + 1] := pixel[x][y + 1] + quant_error * 5 / 16
                currPos[ind] = (currPos[ind] + ((error * 5) >> 4)).ToByte();

                if (col != Position.Last)
                {
                    ind = bitmap.BackBufferStride + bytesPerPixel + colorNum;
                    //pixel[x + 1][y + 1] := pixel[x + 1][y + 1] + quant_error * 1 / 16
                    currPos[ind] = (currPos[ind] + ((error * 1) >> 4)).ToByte();
                }
            }
        }

        private enum Position { First, Last, Other };

        private Position GetPos(int r, int dim)
        {
            return r == dim - 1 ? Position.Last : r == 0 ? Position.First : Position.Other;
        }
    }
}