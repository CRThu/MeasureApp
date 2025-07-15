using NWaves.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class FftWindow
    {
        public static Dictionary<string, double[]> GeneralizedCosineWindowCoefficient = new()
        {
            { "hanning", new double[] { 0.5, 0.5 } },
            { "hamming", new double[] { 0.53836, 0.46164 } },
            { "blackmanharris", new double[] { 0.35875, 0.48829, 0.14128, 0.01168 } },
            { "flattop", new double[] { 0.21557895, 0.41663158, 0.277263158, 0.083578947, 0.006947368 } },
            { "HFT90D", new double[] { 1, 1.942604, 1.340318, 0.440811, 0.043097 } },
            { "HFT144D", new double[] { 1, 1.96760033, 1.57983607, 0.81123644, 0.22583558, 0.02773848, 0.00090360 } },
            { "HFT248D", new double[] { 1, 1.985844164102, 1.791176438506, 1.282075284005, 0.667777530266, 0.240160796576, 0.056656381764, 0.008134974479, 0.000624544650, 0.000019808998, 0.000000132974 } }
        };

        public delegate double[] WindowFuncHandler(int winN);
        public static Dictionary<string, WindowFuncHandler> WindowFunc = new()
        {
            { "rectangular", RectangularWindow },
            { "hanning", (winN) => GenerateGeneralizedCosineWindow(winN, "hanning") },
            { "hamming", (winN) => GenerateGeneralizedCosineWindow(winN, "hamming") },
            { "blackmanharris", (winN) => GenerateGeneralizedCosineWindow(winN, "blackmanharris") },
            { "flattop", (winN) => GenerateGeneralizedCosineWindow(winN, "flattop") },
            { "HFT90D", (winN) => GenerateGeneralizedCosineWindow(winN, "HFT90D") },
            { "HFT144D", (winN) => GenerateGeneralizedCosineWindow(winN, "HFT144D") },
            { "HFT248D", (winN) => GenerateGeneralizedCosineWindow(winN, "HFT248D") }
        };

        public static Dictionary<string, double?> WindowEnbw = new()
        {
            { "rectangular", 1.0000 },
            { "hanning", 1.5000 },
            { "hamming", 1.3628 },
            { "blackmanharris", 2.0044 },
            { "flattop", null },
            { "HFT90D", 3.8832 },
            { "HFT144D", 4.5386 },
            { "HFT248D", 5.6512 },
        };

        // Mainlobe width of one side (Bins), it's both side width is 2*N+1
        public static Dictionary<string, double?> WindowMainlobeWidth = new()
        {
            { "rectangular", 1 },
            { "hanning", 2 },
            { "hamming", 2 },
            { "blackmanharris", 4 },
            { "flattop", null },
            { "HFT90D", 5 },
            { "HFT144D", 7 },
            { "HFT248D", 11 },
        };

        public static double[] AddWindow(double[] vt, string winName)
        {
            double[] win = WindowFunc[winName](vt.Length);
            for (int i = 0; i < win.Length; i++)
                win[i] *= vt[i];
            return win;
        }

        public static double[] GenerateWindow(Func<int, double> winGenFunc, int winN)
        {
            double[] win = new double[winN];
            for (int n = 0; n < winN; n++)
                win[n] = winGenFunc(n);
            return win;
        }

        public static double[] GenerateGeneralizedCosineWindow(int winN, string winName)
        {
            return GenerateGeneralizedCosineWindow(winN, GeneralizedCosineWindowCoefficient[winName]);
        }

        public static double[] GenerateGeneralizedCosineWindow(int winN, double[] an)
        {
            double[] win = new double[winN];
            for (int n = 0; n < winN; n++)
                for (int k = 0; k < an.Length; k++)
                    win[n] += (k % 2 == 0 ? 1 : -1) * an[k] * Math.Cos(2 * Math.PI * k * n / winN);
            return win;
        }

        public static double[] RectangularWindow(int winN)
        {
            return GenerateWindow(x => 1, winN);
        }
    }
}
