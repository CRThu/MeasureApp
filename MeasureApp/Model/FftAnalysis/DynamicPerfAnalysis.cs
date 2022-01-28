using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class DynamicPerfAnalysis
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

        public static (double fc, double yMax) FindMax(double[] f, double[] y)
        {
            double ymax = y.Max();
            int yIndex = Array.IndexOf(y, ymax);
            double fc = f[yIndex];
            return (fc, ymax);
        }

        public static (double fc, double yMax) FindMax(double[] f, double[] y, double fmin = -1, double fmax = -1)
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

            (double[] fInRange, double[] yInRange) data = SubArray(f, y, fminRange.index, fmaxRange.index);

            return FindMax(data.fInRange, data.yInRange);
        }
    }
}
