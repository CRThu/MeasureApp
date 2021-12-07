using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class SerialPortPresetCommand : NotificationObjectBase
    {
        public string GroupName { get; set; }
        public string PresetCommandName { get; set; }
        public string PresetCommandDescription { get; set; }
        public string PresetCommand { get; set; }

        public SerialPortPresetCommand(string groupName = "", string presetCommandName = "", string presetCommandDescription = "", string presetCommand = "")
        {
            GroupName = groupName;
            PresetCommandName = presetCommandName;
            PresetCommandDescription = presetCommandDescription;
            PresetCommand = presetCommand;
        }
    }
}
