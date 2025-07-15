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
        // 8192 FFT 480ms
        // 点数需要被4整除
        public static void DFTDirect(double[] input, double[] re, double[] im)
        {
            if (input.Length % 4 != 0)
                throw new NotImplementedException("未实现输入长度不为4的倍数的FFT运算");

            Array.Clear(re, 0, re.Length);
            Array.Clear(im, 0, im.Length);

            double piDivLen = 2 * Math.PI / input.Length;
            double[] sinArr = Enumerable.Range(0, input.Length).Select(n => Math.Sin(piDivLen * n)).ToArray();
            for (int i = 0; i < re.Length; i++)
            {
                for (int j = 0; j < input.Length; j++)
                {
                    re[i] += input[j] * sinArr[(i * j + sinArr.Length / 4) % sinArr.Length];
                    im[i] += -input[j] * sinArr[i * j % sinArr.Length];
                }
            }
        }
    }
}