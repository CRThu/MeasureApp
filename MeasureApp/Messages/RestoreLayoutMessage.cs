using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Messages
{
    public class RestoreLayoutMessage
    {
        //public static RestoreLayoutMessage Instance { get; } = new RestoreLayoutMessage();
        public string FileName { get; }

        public RestoreLayoutMessage(string fileName = "layout.xml")
        {
            FileName = fileName;
        }
    }
}
