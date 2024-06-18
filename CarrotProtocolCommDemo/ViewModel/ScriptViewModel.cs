using CarrotCommFramework.Factory;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Util;
using CarrotProtocolCommDemo.Logger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    }
}
