using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class CarrotDataProtocolConfigViewModel : ObservableObject
    {
        [ObservableProperty]
        private int[] protocolIdList;

        [ObservableProperty]
        private int[] streamIdList;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private int protocolId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private int streamId;

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
            protocolIdList = new int[] { 0x31, 0x32, 0x33, 0x41, 0x42, 0x43 };
            //protocolId = protocolIdList.FirstOrDefault();
            protocolId = 0x41;
            streamIdList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //streamId = streamIdList.FirstOrDefault();
            streamId = 1;
            payloadDisplay = @"\0A\00\00\00";
        }
    }
}
