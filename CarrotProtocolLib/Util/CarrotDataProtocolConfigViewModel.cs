using CarrotProtocolLib.Protocol;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    public partial class CarrotDataProtocolConfigViewModel : ObservableObject
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private int protocolId = 0x32;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private int streamId = 0x00;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PayloadBytes))]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private string payloadDisplay;

        public byte[] PayloadBytes
        {
            get
            {
                return PayloadDisplay.EscapeStringToBytes();
            }
            set
            {
                PayloadDisplay = value.BytesToEscapeString();
            }
        }

        public CarrotDataProtocolFrame Frame
        {
            get
            {
                return new CarrotDataProtocolFrame(ProtocolId, StreamId, PayloadBytes);
            }
        }

        public string FrameHexDisplay
        {
            get
            {
                return Frame.FrameBytes.BytesToHexString();
            }
        }


        public CarrotDataProtocolConfigViewModel()
        {
            payloadDisplay = string.Empty;
        }
    }
}
