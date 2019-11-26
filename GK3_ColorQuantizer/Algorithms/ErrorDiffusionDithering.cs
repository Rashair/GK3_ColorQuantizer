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

            bitmap.Lock();
            unsafe
            {
                byte* bmpArray = (byte*)bitmap.BackBuffer.ToPointer();
                byte* copyArray = (byte*)originalCopy.BackBuffer.ToPointer();

                bmpArray[0] = copyArray[0];
                bmpArray[1] = copyArray[1];
                bmpArray[2] = copyArray[2];
                int flag = 0;
                for (int i = 0; i < bitmap.PixelHeight; ++i)
                {
                    byte* currPos = bmpArray + i * bitmap.BackBufferStride;
                    byte* currCopyPos = copyArray + i * originalCopy.BackBufferStride;
                    flag = (i == bitmap.PixelHeight - 1 ? 4 : 0);
                    for (int j = 0; j < bitmap.PixelWidth; ++j)
                    {
                        flag |= (j == bitmap.PixelWidth - 1 ? 2 : (j == 0 ? 1 : 0));

                        byte newVal = RoundToNeareastMultiple(currPos[0], itB).ToByte();
                        PropagateError(currPos[0] - newVal, 0, currPos, currCopyPos, flag);
                        currPos[0] = newVal;

                        newVal = RoundToNeareastMultiple(currPos[1], itG).ToByte();
                        PropagateError(currPos[1] - newVal, 1, currPos, currCopyPos, flag);
                        currPos[1] = newVal;

                        newVal = RoundToNeareastMultiple(currPos[2], itR).ToByte();
                        PropagateError(currPos[2] - newVal, 2, currPos, currCopyPos, flag);
                        currPos[2] = newVal;

                        currPos += bytesPerPixel;
                        currCopyPos += bytesPerPixel;
                    }
                }
            }
            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
        }

        /// <param name="flag"> 
        ///     4 - no next row
        ///     2 - no next col
        ///     1 - no prev col
        /// </param>
        private unsafe void PropagateError(int error, int colorNum, byte* currPos, byte* currCopyPos, int flag)
        {
            // x - from left to right
            // y - from top to bottom

            int ind;
            if ((flag & 2) == 0)
            {
                ind = bytesPerPixel + colorNum;
                // pixel[x + 1][y] := pixel[x + 1][y] + quant_error * 7 / 16
                currPos[ind] = (currCopyPos[ind] + ((error * 7) >> 4)).ToByte();
            }

            if ((flag & 4) == 0)
            {
                if ((flag & 1) == 0)
                {
                    ind = bitmap.BackBufferStride - bytesPerPixel + colorNum;
                    // pixel[x - 1][y + 1] := pixel[x - 1][y + 1] + quant_error * 3 / 16
                    currPos[ind] = (currCopyPos[ind] + ((error * 3) >> 4)).ToByte();
                }

                ind = bitmap.BackBufferStride + colorNum;
                // pixel[x][y + 1] := pixel[x][y + 1] + quant_error * 5 / 16
                currPos[ind] = (currCopyPos[ind] + ((error * 5) >> 4)).ToByte();

                if ((flag & 2) == 0)
                {
                    ind = bitmap.BackBufferStride + bytesPerPixel + colorNum;
                    //pixel[x + 1][y + 1] := pixel[x + 1][y + 1] + quant_error * 1 / 16
                    currPos[ind] = (currCopyPos[ind] + ((error * 1) >> 4)).ToByte();
                }
            }
        }
    }
}