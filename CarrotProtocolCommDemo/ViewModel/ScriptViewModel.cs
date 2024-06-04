using CarrotCommFramework.Factory;
using CarrotCommFramework.Sessions;
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
        private string scriptText = "SCRIPT";

        [RelayCommand]
        public void Send()
        {
            AppLogger.Log("SEND CLICKED");

            SessionInstance?.Write(ScriptText);

            AppLogger.Log($"SESSION WRITE:{ScriptText}");
        }

    }
}
