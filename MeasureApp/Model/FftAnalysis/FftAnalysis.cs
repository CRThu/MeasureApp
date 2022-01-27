using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class FftAnalysis
    {
        public static int GetFftN(int sampleN)
        {
            // NWaves FFT Count must be 2^N
            return (int)Math.Pow(2, Math.Ceiling(Math.Log2(sampleN)));
        }

        public static double[] FftFreq(int fftN, double fs)
        {
            return Enumerable.Range(0, fftN / 2 + 1).Select(k => (double)k / fftN * fs).ToArray();
        }

        public static (double[] freq, double[] real, double[] imag) Fft(IEnumerable<double> vt, double fs = 1)
        {
            int fftN = GetFftN(vt.Count());
            var rfft = new NWaves.Transforms.RealFft64(fftN);
            double[] real = new double[fftN / 2 + 1];
            double[] imag = new double[fftN / 2 + 1];
            rfft.Direct(vt.ToArray(), real, imag);

            return (FftFreq(fftN, fs), real, imag);
        }

        public static (double[] freq, double[] mag) FftMag(IEnumerable<double> vt, double fs = 1)
        {
            (double[] freq, double[] real, double[] imag) = Fft(vt, fs);
            for (int i = 0; i < real.Length; i++)
                real[i] = Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
            return (freq, real);
        }

    }
}
