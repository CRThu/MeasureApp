using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class FftAnalysis
    {
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

        public static double[] FftFreq(int fftN, double fs)
        {
            return Enumerable.Range(0, fftN / 2 + 1).Select(k => (double)k / fftN * fs).ToArray();
        }

        public static (double[] freq, double[] real, double[] imag) Fft(double[] vt, double fs = 1, string winName = null)
        {
            if (winName != null)
                vt = FftWindow.AddWindow(vt, winName);

            //vt= GetFftArray(vt);

            var rfft = new NWaves.Transforms.RealFft64(vt.Length);
            double[] real = new double[vt.Length / 2 + 1];
            double[] imag = new double[vt.Length / 2 + 1];

            rfft.Direct(vt, real, imag);

            return (FftFreq(vt.Length, fs), real, imag);
        }

        public static (double[] freq, double[] mag) FftMag(double[] vt, double fs = 1, string winName = null, bool removeDc = true)
        {
            if (removeDc)
                vt = RemoveDc(vt);
            (double[] freq, double[] real, double[] imag) = Fft(vt, fs, winName);

            for (int i = 0; i < real.Length; i++)
                real[i] = Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);

            return (freq, real);
        }

        public static double[] RemoveDc(double[] v)
        {
            double avg = v.Average();
            return v.Select(s => s - avg).ToArray();
        }

        public static double[] NormalizedTo0dB(double[] mag)
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
