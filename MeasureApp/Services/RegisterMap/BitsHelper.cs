using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasureApp.Services.RegisterMap
{
    public static class BitsHelper
    {
        /// <summary>
        /// 生成掩码
        /// </summary>
        /// <param name="startBits"></param>
        /// <param name="endBits"></param>
        /// <returns></returns>
        public static uint GetBitsMaskUInt32(int startBits, int endBits)
        {
            int lsb = Math.Min(startBits, endBits);
            int msb = Math.Max(startBits, endBits);
            int bitlen = msb - lsb + 1;
            uint mask = ((1U << bitlen) - 1U) << lsb;
            return mask;
        }

        /// <summary>
        /// 提取位域值
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="startBits"></param>
        /// <param name="endBits"></param>
        /// <returns></returns>
        public static uint GetBitsUInt32(uint reg, int startBits, int endBits)
        {
            int lsb = Math.Min(startBits, endBits);
            int msb = Math.Max(startBits, endBits);
            return (reg & GetBitsMaskUInt32(lsb, msb)) >> lsb;
        }

        /// <summary>
        /// 设置位域值
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="startBits"></param>
        /// <param name="endBits"></param>
        /// <param name="data"></param>
        public static void SetBitsUInt32(ref uint reg, int startBits, int endBits, uint data)
        {
            int lsb = Math.Min(startBits, endBits);
            int msb = Math.Max(startBits, endBits);
            reg = reg & ~GetBitsMaskUInt32(lsb, msb)
                | (data << lsb) & GetBitsMaskUInt32(lsb, msb);
        }
    }
}
