using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class CosineWaveGen
    {
        public static double[] GenWave(int n, double fc, double fs, double vAmp = 1, double dc = 0, double phase = 0, double? snr = null)
        {
            double[] t = Enumerable.Range(0, n).Select(tx => (double)tx).ToArray();
            double k = 2 * Math.PI * fc / fs;
            double[] wave = t.Select(tx => vAmp * Math.Cos(k * tx + phase) + dc).ToArray();

            // todo snr
            return wave;
        }

        public static double[] GenRandn(int n)
        {
            return GenRandn(n, 0, 1);
        }

        public static double[] GenRandn(int n, double mean, double stdDev)
        {
            double[] vals = new double[n];
            MathNet.Numerics.Distributions.Normal.Samples(vals, mean, stdDev);
            return vals;
        }

        public static double[] wgn(double[] x, double snr)
        {
            double snrdB = Math.Pow(10, snr / 10);
            double xPower = x.Select(x => x * x).Sum() / x.Length;
            double nPower = xPower / snrdB;
            return GenRandn(x.Length).Select(x => x * Math.Sqrt(nPower)).ToArray();
        }

        public static double[] wgn(int n, double fullScale, double dr)
        {
            double drdB = Math.Pow(10, dr / 10);
            double xPower = Math.Pow(fullScale / 2 / Math.Sqrt(2), 2);
            double nPower = xPower / drdB;
            return GenRandn(n).Select(x => x * Math.Sqrt(nPower)).ToArray();
        }
    }
}
