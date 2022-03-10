using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasureApp.Model.Common;

namespace MeasureApp.Model.Devices
{
    public class SerialPortPresetCommand : NotificationObjectBase
    {
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }

        public SerialPortPresetCommand(string groupName = "", string name = "", string description = "", string command = "")
        {
            GroupName = groupName;
            Name = name;
            Description = description;
            Command = command;
        }
    }
}
