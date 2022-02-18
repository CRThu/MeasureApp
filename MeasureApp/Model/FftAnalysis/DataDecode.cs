using MeasureApp.Model.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class DataDecode
    {
        public static double[] CodeToVoltage<T>(T[] codes, int bits, double fullScale, double vBias, bool hasOffset = true) where T : struct
        {
            double lsb = fullScale / Math.Pow(2, bits);
            return codes.Select(c => (double)(dynamic)c * lsb + (hasOffset ? -fullScale / 2 : 0) + vBias).ToArray();
        }

        public static double[] Decode(string[] strs, FftAnalysisPropertyConfigs cfg)
        {
            return Decode(strs, cfg.FromBase, cfg.Bits, cfg.FullScale, cfg.VBias, cfg.HasCodeOffset);
        }
        public static double[] Decode(string[] strs, int fromBase, int bits, double fullScale, double vBias, bool hasOffset = true)
        {
            if (bits >= 64)
                throw new NotImplementedException("bits:{bits} >= 64 is not supported.");
            var codes = strs.Select(v => BaseConverter.BaseConverterInt64(v, fromBase)).ToArray();
            return CodeToVoltage(codes, bits, fullScale, vBias, hasOffset);
        }
    }
}
