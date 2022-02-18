using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class DynamicPerfAnalysis
    {
        public static double[] FftFreq(int fftN, double fs)
        {
            return Enumerable.Range(0, fftN / 2 + 1).Select(k => (double)k / fftN * fs).ToArray();
        }

        public static (double[] freq, double[] real, double[] imag) Fft(double[] vt, double fs = 1, string winName = null)
        {
            if (winName != null)
                vt = FftWindow.AddWindow(vt, winName);

            var rfft = new NWaves.Transforms.RealFft64(vt.Length);
            double[] real = new double[vt.Length / 2 + 1];
            double[] imag = new double[vt.Length / 2 + 1];

            Stopwatch sw = new();
            sw.Start();
            rfft.Direct(vt, real, imag);
            sw.Stop();
            Debug.WriteLine($"FFT:{sw.ElapsedMilliseconds}ms.");
            return (FftFreq(vt.Length, fs), real, imag);
        }

        public static (double[] freq, double[] mag) FftMag(double[] vt, double fs = 1, string winName = null, bool removeDc = true)
        {
            if (removeDc)
                vt = DynamicPerfAnalysisUtility.RemoveDc(vt);
            (double[] freq, double[] real, double[] imag) = Fft(vt, fs, winName);

            for (int i = 0; i < real.Length; i++)
                real[i] = Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);

            return (freq, real);
        }

        public static Dictionary<string, (double, string)> CalcPerfP(double pSignal, double pNoise, double pDistortion)
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

        public static Dictionary<string, (double, string)> CalcPerfV(double vSignal, double vNoise, double vDistortion, double? fullScale = null, double? vSpur = null)
        {
            Dictionary<string, (double, string)> perfInfo = new();
            double FS = (fullScale ?? 0) / 2;
            if (fullScale != null)
                perfInfo.Add("DR", (20 * Math.Log10(FS / vNoise), "dB"));
            perfInfo.Add("SNR", (20 * Math.Log10(vSignal / vNoise), "dB"));
            perfInfo.Add("THD", (20 * Math.Log10(vDistortion / vSignal), "dB"));
            perfInfo.Add("THD+N", (-20 * Math.Log10(vSignal / (vNoise + vDistortion)), "dB"));
            perfInfo.Add("SINAD", (20 * Math.Log10(vSignal / (vNoise + vDistortion)), "dB"));
            if (vSpur != null)
                perfInfo.Add("SFDR", (-20 * Math.Log10(vSignal / (double)vSpur), "dB"));
            if (fullScale != null && vSpur != null)
                perfInfo.Add("SFDRFS", (-20 * Math.Log10(FS / (double)vSpur), "dB"));
            if (fullScale != null)
                perfInfo.Add("ENOB", ((20 * Math.Log10(vSignal / (vNoise + vDistortion)) - 1.76 + 20 * Math.Log10(FS / vSignal)) / 6.02, "Bits"));
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
            double[] magNorm = DynamicPerfAnalysisUtility.NormalizedMaxTo0dB(mag);
            double[] powerNorm = mag.Select(m => m * m).ToArray();

            (double fc, int yIndex, double yMax) baseSignal = DynamicPerfAnalysisUtility.FindMax(freq, magNorm);
            sgnInfo.Add("Base", (baseSignal.fc, "Hz", baseSignal.yMax, "dB"));

            double pDc = DynamicPerfAnalysisUtility.SubArray(powerNorm, 0, spanSignalEnergy - 1).Sum();
            double pSignal = DynamicPerfAnalysisUtility.SubArray(powerNorm,
                baseSignal.yIndex - spanSignalEnergy,
                baseSignal.yIndex + spanSignalEnergy).Sum();

            double[] fHarmonic = new double[nHarmonic - 1];
            int[] fHarmonicIndex = new int[nHarmonic - 1];
            double[] pHarmonic = new double[nHarmonic - 1];
            double[] dBHarmonic = new double[nHarmonic - 1];

            for (int i = 0; i < fHarmonic.Length; i++)
            {
                fHarmonic[i] = (i + 2) * baseSignal.fc;
                fHarmonicIndex[i] = (i + 2) * (baseSignal.yIndex - 1) + 1;

                (double fc, int yIndex, double yMax) harmonic = DynamicPerfAnalysisUtility.FindMax(freq, powerNorm,
                    fHarmonicIndex[i] - spanHarmonicPeak,
                    fHarmonicIndex[i] + spanHarmonicPeak);

                fHarmonic[i] = harmonic.fc;
                fHarmonicIndex[i] = harmonic.yIndex;
                pHarmonic[i] = DynamicPerfAnalysisUtility.SubArray(powerNorm,
                    harmonic.yIndex - spanHarmonicEnergy,
                    harmonic.yIndex + spanHarmonicEnergy).Sum();

                dBHarmonic[i] = 10 * Math.Log10(pHarmonic[i] / pSignal);
                sgnInfo.Add("HD" + (i + 2), (fHarmonic[i], "Hz", dBHarmonic[i], "dB"));
            }

            double pDistortion = pHarmonic.Sum();
            double pNoise = powerNorm.Sum() - pDc - pSignal - pDistortion;

            var perfInfo = CalcPerfP(pSignal, pNoise, pDistortion);

            return (t, v, freq, magNorm, perfInfo, sgnInfo);
        }

        // 幅值校正法分析动态性能
        public static (double[] t, double[] v, double[] f, double[] p,
            Dictionary<string, (double, string)> perfInfo,
            Dictionary<string, (double, string, double, string)> sgnInfo)
            FftAnalysisAmplitudeCorrection(double[] samples, FftAnalysisPropertyConfigs cfg)
        {
            return FftAnalysisAmplitudeCorrection(samples,
                    cfg.Fs,
                    cfg.Window,
                    cfg.FftN,
                    cfg.NHarmonic,
                    cfg.FullScale,
                    cfg.SpanHarmonicPeak);
        }

        public static (double[] t, double[] v, double[] f, double[] p,
            Dictionary<string, (double, string)> perfInfo,
            Dictionary<string, (double, string, double, string)> sgnInfo)
            FftAnalysisAmplitudeCorrection(double[] samples, double fs, string window, int? fftN = null,
            int nHarmonic = 5, double fullScale = 1, int spanHarmonicPeak = 2)
        {
            Dictionary<string, (double, string, double, string)> sgnInfo = new();

            double fullScaleVamp = fullScale / 2;
            double[] v = samples.Take(fftN ?? samples.Length).ToArray();
            double[] t = Enumerable.Range(0, v.Length).Select(t => t / fs).ToArray();

            //////////////////////////////////
            //////////////////////////////////
            //////////////////////////////////
            // TEMP TODO
            //(double[] freq, double[] mag) = FftMag(v, fs, window);
            (double[] freq, double[] mag) = OverlappedAveragedFftMag(v, 16384, 0.9, fs, window);

            // mag = mag * 2 / N;
            // mag = mag / fullScaleVamp;
            // magdB = 20Log10(mag);
            //double[] magNorm = mag.Select(m => m * 2 / v.Length).ToArray();
            double[] magNorm = mag.Select(m => m * 2 / 16384).ToArray();
            //////////////////////////////////
            //////////////////////////////////
            //////////////////////////////////
            double[] magNormDb = magNorm.Select(m => 20 * Math.Log10(m / fullScaleVamp)).ToArray();

            int spanWidth = (int)Math.Ceiling(FftWindow.WindowMainlobeWidth[window] ?? -1) - 1;
            if (spanWidth < 0)
                throw new NotImplementedException($"Mainlode width of window: '{window}' is error.");

            // DC Center = 0, Mask = [DC : DC + L]
            (int L, int R) maskDc = DynamicPerfAnalysisUtility.GenMaskBins(0, spanWidth, magNorm.Length);

            // Signal Center = SGN, Mask = [SGN - L : SGN + L]
            double[] maskMagNormExDc = DynamicPerfAnalysisUtility.MaskArray(magNorm, maskDc);
            (double fc, int yIndex, double yMax) sgn = DynamicPerfAnalysisUtility.FindMax(freq, maskMagNormExDc);
            (int L, int R) maskSgn = DynamicPerfAnalysisUtility.GenMaskBins(sgn.yIndex, spanWidth, magNorm.Length);
            sgnInfo.Add("Base", (sgn.fc, "Hz", sgn.yMax / Math.Sqrt(2), "Vrms"));

            // Harmonic Center = HD, Mask = [HD - L : HD + L]
            int[] hdIndexCalc = Enumerable.Range(2, nHarmonic - 1).Select(i => (sgn.yIndex + 1) * i - 1).ToArray();
            (double fc, int yIndex, double yMax)[] hd = new (double fc, int yIndex, double yMax)[hdIndexCalc.Length];
            for (int i = 0; i < hdIndexCalc.Length; i++)
                hd[i] = DynamicPerfAnalysisUtility.FindMax(freq, magNorm,
                    hdIndexCalc[i] - spanHarmonicPeak, hdIndexCalc[i] + spanHarmonicPeak);
            (int L, int R)[] maskHd = DynamicPerfAnalysisUtility.GenMaskBins(hd.Select(hdx => hdx.yIndex).ToArray(), spanWidth, magNorm.Length);

            for (int i = 0; i < hd.Length; i++)
                sgnInfo.Add("HD" + (i + 2), (hd[i].fc, "Hz", hd[i].yMax / Math.Sqrt(2), "Vrms"));

            // Spur = Max Except Dc & Signal
            double[] maskMagNormExDcSgn = DynamicPerfAnalysisUtility.MaskArray(magNorm, new (int L, int R)[] { maskDc, maskSgn });
            (double fc, int yIndex, double yMax) spur = DynamicPerfAnalysisUtility.FindMax(freq, maskMagNormExDcSgn);
            sgnInfo.Add("SPUR", (spur.fc, "Hz", spur.yMax / Math.Sqrt(2), "Vrms"));

            // Noise = Max Except Dc & Signal & Hd
            double[] maskMagNormExDcSgnHd = DynamicPerfAnalysisUtility.MaskArray(magNorm, maskHd.Concat(new (int L, int R)[] { maskDc, maskSgn }).ToArray());
            // TODO noise corr, inband
            double noiseVamp = Math.Sqrt(maskMagNormExDcSgnHd.Select(v => v * v).Sum());

            // PnTrue = Pn - ENBW
            if (FftWindow.WindowEnbw[window] != null)
                noiseVamp /= Math.Sqrt((double)FftWindow.WindowEnbw[window]);
            else
                throw new NotImplementedException($"ENBW of window: '{window}' is error.");

            var perfInfo = CalcPerfV(sgn.yMax, noiseVamp, Math.Sqrt(hd.Select(m => m.yMax * m.yMax).Sum()), fullScale, spur.yMax);

            return (t, v, freq, magNormDb, perfInfo, sgnInfo);
        }

        // overlap: 0%-100%
        public static double[][] OverlappedSamples(double[] v, int nFft, double overlap)
        {
            int nOverlap = (int)(nFft * (1 - overlap));
            int[] splitIndexes = Enumerable.Range(0, ((v.Length - nFft) / nOverlap + 1)).Select(n => nOverlap * n).ToArray();
            double[][] vs = new double[splitIndexes.Length][];
            for (int i = 0; i < splitIndexes.Length; i++)
            {
                vs[i] = new double[nFft];
                Array.Copy(v, splitIndexes[i], vs[i], 0, nFft);
            }
            return vs;
        }

        public static (double[] freq, double[] mag) OverlappedAveragedFftMag(double[] v, int nFft, double overlap, double fs = 1, string winName = null)
        {
            double[][] os = OverlappedSamples(v, nFft, overlap);
            double[] freq = new double[nFft / 2 + 1];
            double[] magArraySum = new double[nFft / 2 + 1];
            for (int i = 0; i < os.Length; i++)
            {
                double[] magArray;
                (freq, magArray) = FftMag(os[i], fs, winName);

                for (int j = 0; j < freq.Length; j++)
                    magArraySum[j] += magArray[j];
            }

            magArraySum = magArraySum.Select(m => m / os.Length).ToArray();
            return (freq, magArraySum);
        }
    }
}
