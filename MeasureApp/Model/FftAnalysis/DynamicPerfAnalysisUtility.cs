using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class DynamicPerfAnalysisUtility
    {
        public static (int index, double value) FindCloseValue(double[] v, double vTarget)
        {
            double closestVal = v.Aggregate((x, y) => Math.Abs(x - vTarget) < Math.Abs(y - vTarget) ? x : y);
            int closestIndex = Array.IndexOf(v, closestVal);
            return (closestIndex, closestVal);
        }

        public static double[] SubArray(double[] x, int minIndex = -1, int maxIndex = -1)
        {
            if (minIndex < 0)
                minIndex = 0;
            if (maxIndex < 0)
                maxIndex = x.Length - 1;
            return x.Skip(minIndex).Take(maxIndex - minIndex + 1).ToArray();
        }

        public static (double[] xInRange, double[] yInRange) SubArray(double[] x, double[] y, int minIndex = -1, int maxIndex = -1)
        {
            double[] xInRange = SubArray(x, minIndex, maxIndex);
            double[] yInRange = SubArray(y, minIndex, maxIndex);
            return (xInRange, yInRange);
        }

        public static (double fc, int yIndex, double yMax) FindMax(double[] f, double[] y)
        {
            double ymax = y.Max();
            int yIndex = Array.IndexOf(y, ymax);
            double fc = f[yIndex];
            return (fc, yIndex, ymax);
        }

        public static (double fc, int yIndex, double yMax) FindMax(double[] f, double[] y, int fminIndex, int fmaxIndex)
        {
            (double[] fInRange, double[] yInRange) data = SubArray(f, y, fminIndex, fmaxIndex);
            (double fc, int yIndex, double yMax) = FindMax(data.fInRange, data.yInRange);
            return (fc, yIndex + fminIndex, yMax);
        }

        public static (double fc, int yIndex, double yMax) FindMaxClose(double[] f, double[] y, double fmin = -1, double fmax = -1)
        {
            (int index, double value) fminRange, fmaxRange;

            if (fmin > 0)
                fminRange = FindCloseValue(f, fmin);
            else
                fminRange.index = -1;
            if (fmax > 0)
                fmaxRange = FindCloseValue(f, fmax);
            else
                fmaxRange.index = -1;

            return FindMax(f, y, fminRange.index, fmaxRange.index);
        }

        // generate mask tuple, such as (0,10) when signal is 5.
        // width argument is one side of width.
        public static (int L, int R) GenMaskBins(int center, int width, int arrLen)
        {
            int max = arrLen - 1;
            (int L, int R) mask;
            int l = center - width;
            int r = center + width;
            mask.L = l < 0 ? 0 : (l > max ? max : l);
            mask.R = r < 0 ? 0 : (r > max ? max : r);
            return mask;
        }

        public static (int L, int R)[] GenMaskBins(int[] center, int width, int arrLen)
        {
            (int L, int R)[] masks = new (int L, int R)[center.Length];
            for (int i = 0; i < center.Length; i++)
                masks[i] = GenMaskBins(center[i], width, arrLen);
            return masks;
        }

        // Fill the mask in Array
        public static double[] MaskArray(double[] vals, (int L, int R) maskBin, double fill = 0)
        {
            double[] maskedVals = new double[vals.Length];
            Array.Copy(vals, maskedVals, vals.Length);
            Array.Fill(maskedVals, fill, maskBin.L, maskBin.R - maskBin.L + 1);
            return maskedVals;
        }

        public static double[] MaskArray(double[] vals, (int L, int R)[] maskBins, double fill = 0)
        {
            double[] maskedVals = new double[vals.Length];
            Array.Copy(vals, maskedVals, vals.Length);
            foreach ((int L, int R) maskBin in maskBins)
                Array.Fill(maskedVals, fill, maskBin.L, maskBin.R - maskBin.L + 1);
            return maskedVals;
        }

        public static double[] GetFftArray(double[] vt)
        {
            // NWaves FFT Count must be 2^N
            int fftN = (int)Math.Pow(2, Math.Ceiling(Math.Log2(vt.Length)));
            if (fftN != vt.Length)
            {
                double[] vt_new = new double[fftN];
                vt.CopyTo(vt_new, 0);
                vt = vt_new;
            }
            return vt;
        }

        public static double[] RemoveDc(double[] v)
        {
            double avg = v.Average();
            return v.Select(s => s - avg).ToArray();
        }

        public static double[] NormalizedMaxTo0dB(double[] mag)
        {
            double[] magNorm = new double[mag.Length];

            for (int i = 0; i < mag.Length; i++)
                magNorm[i] = 20 * Math.Log10(mag[i]);

            double maxdB = magNorm.Max();
            for (int i = 0; i < magNorm.Length; i++)
                magNorm[i] -= maxdB;

            return magNorm;
        }
    }
}
