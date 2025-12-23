using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Messages
{
    public class SaveLayoutMessage
    {
        // public static SaveLayoutMessage Instance { get; } = new SaveLayoutMessage();
        public string FileName { get; }

        public SaveLayoutMessage(string fileName = "layout.xml")
        {
            FileName = fileName;
        }
    }
}
