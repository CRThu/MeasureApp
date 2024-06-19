using CarrotCommFramework.Factory;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Util;
using CarrotProtocolCommDemo.Logger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class ScriptViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Session? sessionInstance;

        [ObservableProperty]
        private Logger.AppLogger appLogger;

        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<ScriptViewModel, AppLogger>(this, (r, m) => r.AppLogger = m);
            WeakReferenceMessenger.Default.Register<ScriptViewModel, Session>(this, (r, m) => r.SessionInstance = m);
        }


        [ObservableProperty]
        private string rapText = "Hello";

        [ObservableProperty]
        private string cdpText = "Hello";

        [ObservableProperty]
        private List<byte> protocolIdList = [
            CarrotDataProtocolPacket.ProtocolIdAsciiTransfer256,
            CarrotDataProtocolPacket.ProtocolIdDataTransfer266,
            CarrotDataProtocolPacket.ProtocolIdRegisterOper];

        [ObservableProperty]
        private byte protocolId = CarrotDataProtocolPacket.ProtocolIdAsciiTransfer256;

        [ObservableProperty]
        private List<byte> streamIdList = [0, 1, 2, 3];

        [ObservableProperty]
        private byte streamId = 0;

        [RelayCommand]
        public void SendRap()
        {
            AppLogger.Log("RAP SEND CLICKED");

            var packet = SessionInstance.Protocols[0].Encode(RapText, 0, 0);
            SessionInstance?.Write(packet);

            AppLogger.Log($"SESSION WRITE:{RapText}");
        }

        [RelayCommand]
        public void SendCdp()
        {
            AppLogger.Log("CDP SEND CLICKED");

            var packet = SessionInstance.Protocols[0].Encode(CdpText, ProtocolId, StreamId);
            SessionInstance?.Write(packet);

            AppLogger.Log($"SESSION WRITE:{CdpText}, {ProtocolId}, {StreamId}");
        }

        [RelayCommand]
        public void ExportPackets()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                SerializationHelper.SerializeToFile((DataLogger)SessionInstance.Loggers[0], sfd.FileName);
            }
        }

        [RelayCommand]
        public void ExportData()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                List<FileStream> fss = new List<FileStream>();
                List<StreamWriter> sws = new List<StreamWriter>();

                for (int ch = 0; ch < 4; ch++)
                {
                    string[] names = sfd.FileName.Split('.', 2);
                    string newfilename = $"{names[0]}.s{ch}.{names[1]}";
                    FileStream fs = new FileStream(newfilename, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    fss.Add(fs);
                    sws.Add(sw);
                }

                DataLogger dataLogger = (DataLogger)SessionInstance.Loggers[0];
                for (int i = 0; i < dataLogger.Ds.Count; i++)
                {
                    var streamIdByte = ((CarrotDataProtocolPacket)dataLogger.Ds[i].Packet).StreamId;
                    var protocolId = ((CarrotDataProtocolPacket)dataLogger.Ds[i].Packet).ProtocolId;
                    var bytes = ((CarrotDataProtocolPacket)dataLogger.Ds[i].Packet).Payload;
                    int streamId = (int)streamIdByte;

                    if (protocolId >= 0x40 && protocolId <= 0x4F && streamId >= 0 && streamId <= 3)
                    {
                        string s = bytes.ToArray().BytesToHexString();
                        sws[streamId].Write(s);
                        sws[streamId].Write(" ");
                    }
                }
                for (int ch = 0; ch < 4; ch++)
                {
                    sws[ch].Close();
                    fss[ch].Close();
                }
            }
        }
    }
}
