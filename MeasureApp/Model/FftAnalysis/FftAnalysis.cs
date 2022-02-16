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

        public static Dictionary<string, (double, string)> CalcPerf(double pSignal, double pNoise, double pDistortion)
        {
            double SNR = 10 * Math.Log10(pSignal / pNoise);
            double THD = 10 * Math.Log10(pDistortion / pSignal);
            double SINAD = 10 * Math.Log10(pSignal / (pNoise + pDistortion));
            double ENOB = (SINAD - 1.76) / 6.02;

            Dictionary<string, (double, string)> perfInfo = new()
            {
                { "SNR", (SNR, "dB") },
                { "THD", (THD, "dB") },
                { "SINAD", (SINAD, "dB") },
                { "ENOB", (ENOB, "Bits") },
            };

            return perfInfo;
        }

        // 能量校正法分析动态性能
        // 时域: t, v
        // 频域: f, p
        // 性能: perfInfo, 信号: sgnInfo

        public static (double[] t, double[] v, double[] f, double[] p,
            Dictionary<string, (double, string)> perfInfo,
            Dictionary<string, (double, string, double, string)> sgnInfo)
            FftAnalysisEnergyCorrection(double[] samples, FftAnalysisPropertyConfigs cfg)
        {
            return FftAnalysisEnergyCorrection(samples,
                    cfg.Fs,
                    cfg.Window,
                    cfg.FftN,
                    cfg.NHarmonic,
                    cfg.SpanSignalEnergy,
                    cfg.SpanHarmonicPeak,
                    cfg.SpanHarmonicEnergy);
        }

        public static (double[] t, double[] v, double[] f, double[] p,
            Dictionary<string, (double, string)> perfInfo,
            Dictionary<string, (double, string, double, string)> sgnInfo)
            FftAnalysisEnergyCorrection(double[] samples, double fs, string window, int? fftN = null,
            int nHarmonic = 5, int spanSignalEnergy = 5, int spanHarmonicPeak = 2, int spanHarmonicEnergy = 1)
        {
            Dictionary<string, (double, string, double, string)> sgnInfo = new();

            double[] v = samples.Take(fftN ?? samples.Length).ToArray();
            double[] t = Enumerable.Range(0, v.Length).Select(t => t / fs).ToArray();

            (double[] freq, double[] mag) = FftMag(v, fs, window);
            double[] magNorm = NormalizedTo0dB(mag);
            double[] powerNorm = mag.Select(m => m * m).ToArray();

            (double fc, int yIndex, double yMax) baseSignal = DynamicPerfAnalysis.FindMax(freq, magNorm);
            sgnInfo.Add("Base", (baseSignal.fc, "Hz", baseSignal.yMax, "dB"));

            double pDc = DynamicPerfAnalysis.SubArray(powerNorm, 0, spanSignalEnergy - 1).Sum();
            double pSignal = DynamicPerfAnalysis.SubArray(powerNorm,
                baseSignal.yIndex - spanSignalEnergy,
                baseSignal.yIndex + spanSignalEnergy).Sum();

            double[] fHarmonic = new double[nHarmonic];
            int[] fHarmonicIndex = new int[nHarmonic];
            double[] pHarmonic = new double[nHarmonic];
            double[] dBHarmonic = new double[nHarmonic];

            for (int i = 1; i <= nHarmonic; i++)
            {
                fHarmonic[i - 1] = i * baseSignal.fc;
                fHarmonicIndex[i - 1] = i * (baseSignal.yIndex - 1) + 1;

                (double fc, int yIndex, double yMax) harmonic = DynamicPerfAnalysis.FindMax(freq, powerNorm,
                    fHarmonicIndex[i - 1] - spanHarmonicPeak,
                    fHarmonicIndex[i - 1] + spanHarmonicPeak);

                fHarmonic[i - 1] = harmonic.fc;
                fHarmonicIndex[i - 1] = harmonic.yIndex;
                pHarmonic[i - 1] = DynamicPerfAnalysis.SubArray(powerNorm,
                    harmonic.yIndex - spanHarmonicEnergy,
                    harmonic.yIndex + spanHarmonicEnergy).Sum();

                if (i != 1)
                {
                    dBHarmonic[i - 1] = 10 * Math.Log10(pHarmonic[i - 1] / pHarmonic[0]);
                    sgnInfo.Add("HD" + i, (fHarmonic[i - 1], "Hz", dBHarmonic[i - 1], "dB"));
                }
            }

            pHarmonic[0] = 0;

            double pDistortion = pHarmonic.Sum();
            double pNoise = powerNorm.Sum() - pDc - pSignal - pDistortion;

            var perfInfo = CalcPerf(pSignal, pNoise, pDistortion);

            return (t, v, freq, magNorm, perfInfo, sgnInfo);
        }
    }
}
