using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    /// <summary>
    /// 转义字符串
    /// </summary>
    public partial class EscapeString : ObservableObject
    {
        /// <summary>
        /// 转义字符字节数组存储
        /// </summary>
        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(RawBytes))]
        [NotifyPropertyChangedFor(nameof(HexString))]
        private string textString;

        public byte[] RawBytes
        {
            get
            {
                return TextString.EscapeStringToBytes();
            }
            set
            {
                TextString = value.BytesToEscapeString();
            }
        }

        private string hexString;
        /// <summary>
        /// 字节16进制字符串
        /// </summary>
        public string HexString
        {
            get
            {
                if (hexString.CheckHexString())
                {
                    hexString = RawBytes.BytesToHexString();
                }
                return hexString;
                //return RawBytes.BytesToHexString();
            }
            set
            {
                hexString = value;

                // 当HexString在View手动输入时，判断为有效字符串后更新至RawBytes
                if (value.CheckHexString())
                    RawBytes = value.HexStringToBytes();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EscapeString()
        {
            TextString = "";
            HexString = "";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="str"></param>
        public EscapeString(string str) : this()
        {
            TextString = str;
        }
    }
}