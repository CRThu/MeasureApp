using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public enum LogLevel { Info, Warning, Error }
    public interface ILogService
    {
        void Log(string message, LogLevel level = LogLevel.Info);
    }
}
