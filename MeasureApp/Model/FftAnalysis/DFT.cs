using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class DFT
    {
        // 8192 FFT 1200ms
        public static void DFTDirect(double[] input, double[] re, double[] im)
        {
            for (int i = 0; i < re.Length; i++)
                re[i] = 0;
            for (int i = 0; i < im.Length; i++)
                im[i] = 0;

            for (int i = 0; i < re.Length; i++)
            {
                double k = 2 * Math.PI * i / input.Length;
                for (int j = 0; j < input.Length; j++)
                {
                    re[i] += input[j] * Math.Cos(k * j);
                    im[i] += -input[j] * Math.Sin(k * j);
                }
            }
        }
    }
}