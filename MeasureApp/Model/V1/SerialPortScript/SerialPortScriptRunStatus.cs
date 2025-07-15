using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.SerialPortScript
{
    public enum SerialPortScriptRunStatus
    {
        Executed,   // 正常运行
        Stopped,    // 暂停标识符
        BlankLine,  // 空行
    }
}
