using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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

        private System.Windows.Media.Brush color;
        public Brush Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChanged(() => Color);
            }
        }

        public SerialPortCommLog(string host, dynamic msg)
        {
            Time = DateTime.Now;
            Host = host;
            Message = msg;
            // SolidColorBrush须在UI线程创建
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    Color = new SolidColorBrush(new Color() { A = 0xFF, R = 0x04, G = 0x22, B = 0x71 });
            //});
            Color = Brushes.DarkGreen;
        }

        public override string ToString()
        {
            return $"{Time:yyyy-MM-dd hh:mm:ss.fff} | {Host} | {Message}";
        }
    }
}
