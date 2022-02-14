using MeasureApp.Model.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public static class ImportSamples
    {
        public static UInt16[] ReadFileData(string path, int numBase = 16)
        {
            string[] vs = File.ReadAllLines(path);
            return vs.Select(v => BaseConverter.BaseConverterUInt16(v, numBase)).ToArray();
        }
    }
}
