using CarrotCommFramework.Factory;
using CarrotCommFramework.Sessions;
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

        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<ScriptViewModel, Session>(this, (r, m) => r.SessionInstance = m);
            Trace.WriteLine("MSG REGISTERED");
        }


        [ObservableProperty]
        private string scriptText = "SCRIPT";

        [RelayCommand]
        public void Send()
        {
            Trace.WriteLine("SEND CLICKED\n");

            SessionInstance?.Write(ScriptText);

            Trace.WriteLine($"SESSION WRITE:{ScriptText}\n");
        }

    }
}
