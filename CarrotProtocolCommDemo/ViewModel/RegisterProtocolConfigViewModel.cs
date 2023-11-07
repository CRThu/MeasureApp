using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class RegisterProtocolConfigViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private string rWnText = "0";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private string regfileText = "0";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private string addressText = "0";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Frame))]
        [NotifyPropertyChangedFor(nameof(FrameHexDisplay))]
        private string valueText = "0";

        public int RWn => RWnText.ParseNum();
        public int Regfile => RegfileText.ParseNum();
        public int Address => AddressText.ParseNum();
        public int Value => ValueText.ParseNum();

        public byte[] PayloadBytes
        {
            get
            {
                byte[] payload = new byte[16];
                byte[] RwnBytes = RWn.IntToBytes();
                byte[] RegfileBytes = Regfile.IntToBytes();
                byte[] AddressBytes = Address.IntToBytes();
                byte[] ValueBytes = Value.IntToBytes();
                Array.Copy(RwnBytes, 0, payload, 0, 4);
                Array.Copy(RegfileBytes, 0, payload, 4, 4);
                Array.Copy(AddressBytes, 0, payload, 8, 4);
                Array.Copy(ValueBytes, 0, payload, 12, 4);
                return payload;
            }
        }

        public CarrotDataProtocolFrame Frame
        {
            get
            {
                return new CarrotDataProtocolFrame(0xA0, 0, PayloadBytes);
            }
        }

        public string FrameHexDisplay
        {
            get
            {
                return Frame.FrameBytes.BytesToHexString();
            }
        }

        // AD4630 TEMP
        [ObservableProperty]
        private string ad4630RWnText = "0";
        [ObservableProperty]
        private string ad4630AddressText = "0";
        [ObservableProperty]
        private string ad4630DataText = "0";
    }
}
