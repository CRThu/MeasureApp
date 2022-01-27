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
        };

        public static double[] GenerateWindow(Func<int, double> winGenFunc, int winN)
        {
            double[] win = new double[winN];
            for (int n = 0; n < winN; n++)
                win[n] = winGenFunc(n);
            return win;
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

        public static double[] HanningWindow(int winN)
        {
            return GenerateGeneralizedCosineWindow(winN, GeneralizedCosineWindowCoefficient["hanning"]);
        }

        public static double[] HammingWindow(int winN)
        {
            return GenerateGeneralizedCosineWindow(winN, GeneralizedCosineWindowCoefficient["hamming"]);
        }

        public static double[] BlackmanHarrisWindow(int winN)
        {
            return GenerateGeneralizedCosineWindow(winN, GeneralizedCosineWindowCoefficient["blackmanharris"]);
        }

        public static double[] FlattopWindow(int winN)
        {
            return GenerateGeneralizedCosineWindow(winN, GeneralizedCosineWindowCoefficient["flattop"]);
        }
    }
}
