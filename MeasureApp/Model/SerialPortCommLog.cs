using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class SerialPortCommLog : NotificationObjectBase
    {
        private DateTime time;
        public DateTime Time
        {
            get => time;
            set
            {
                time = value;
                RaisePropertyChanged(() => Time);
            }
        }

        private string host;
        public string Host
        {
            get => host;
            set
            {
                host = value;
                RaisePropertyChanged(() => Host);
            }
        }

        private dynamic message;
        public dynamic Message
        {
            get => message;
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public SerialPortCommLog(string host, dynamic msg)
        {
            Time = DateTime.Now;
            Host = host;
            Message = msg;
        }

        public override string ToString()
        {
            return $"{Time:yyyy-MM-dd hh:mm:ss.fff} | {Host} | {Message}";
        }
    }
}
