using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class NiVisaGpibDriver : NiVisaDriver
    {
        public new void SetDriver(string addr)
        {
            Addr = addr;
            Session.Timeout = 1000;
            Session.TerminationCharacter = 0x0a;
            Session.TerminationCharacterEnabled = false;
            Session.SendEndEnabled = true;
            Session.DefaultBufferSize = 4096;
        }
    }
}
