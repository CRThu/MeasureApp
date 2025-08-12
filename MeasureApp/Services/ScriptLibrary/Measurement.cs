using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
using MeasureApp.Model.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.ScriptLibrary
{
    public static class Measurement
    {
        public static async Task QueryAsync(AppContextManager context, string sessionKey, string command, string storeKey = null)
        {
            await context.Devices[sessionKey].SendAscii(command + "\n");
            var pkt = await context.Devices[sessionKey].ReadAsync();

            if (pkt is IDataPacket dataPacket)
            {
                // todo
                var data = dataPacket.Get<double>(0)[0];
                context.DataLogger.Add(storeKey ?? $"{sessionKey}::{command}", data);
            }
        }
    }
}
