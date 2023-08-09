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
        [NotifyPropertyChangedFor(nameof(TextString))]
        [NotifyPropertyChangedFor(nameof(HexString))]
        private byte[] rawBytes;

        /// <summary>
        /// 文本字符串
        /// </summary>
        public string TextString
        {
            get
            {
                return RawBytes.BytesToEscapeString();
            }
            set
            {
                RawBytes = value.EscapeStringToBytes();
            }
        }

        /// <summary>
        /// 字节16进制字符串
        /// </summary>
        public string HexString
        {
            get
            {
                return RawBytes.BytesToHexString();
            }
            set
            {
                RawBytes = value.HexStringToBytes();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EscapeString()
        {
            rawBytes = Array.Empty<byte>();
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