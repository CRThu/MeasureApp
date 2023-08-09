using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    public partial class EscapeString : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextString))]
        [NotifyPropertyChangedFor(nameof(HexString))]
        private byte[] rawBytes;

        public string TextString
        {
            get
            {
                return RawBytes.BytesToAsciiString();
            }
            set
            {
                RawBytes = value.AsciiStringToBytes();
            }
        }

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

        public EscapeString()
        {
            rawBytes = Array.Empty<byte>();
        }

        public EscapeString(string str) : this()
        {
            TextString = str;
        }
    }
}