using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Protocol
{
    public partial class AsciiProtocolRecord : ObservableObject, IProtocolRecord
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PayloadDisplay))]
        [NotifyPropertyChangedFor(nameof(Bytes))]
        private string hexDisplay;

        public string PayloadDisplay
        {
            get
            {
                return BytesEx.BytesToAscii(Bytes);
            }
            set
            {
                Bytes = BytesEx.AsciiToBytes(value);
            }
        }

        public byte[] Bytes
        {
            get
            {
                return BytesEx.HexStringToBytes(HexDisplay);
            }
            set
            {
                HexDisplay = BytesEx.BytesToHexString(value);
            }
        }

        public AsciiProtocolRecord(string payload)
        {
            Bytes = BytesEx.AsciiToBytes(payload);
        }

        public AsciiProtocolRecord(byte[] bytes, int offset, int length)
        {
            byte[] bytesNew = new byte[length];
            Array.Copy(bytes, offset, bytesNew, 0, length);
            Bytes = bytesNew;
        }
    }
}
