using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MeasureApp.Model.Logger
{
    public class SerialPortCommLogElement : NotificationObjectBase
    {
        private static Dictionary<string, Brush> KeywordColor = new();

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

        private Brush color;
        public Brush Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChanged(() => Color);
            }
        }

        public SerialPortCommLogElement(dynamic msg, string host = "<null>", bool isHighlight = false)
        {
            Time = DateTime.Now;
            Host = host;
            Message = msg;
            // SolidColorBrush须在UI线程创建
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    Color = new SolidColorBrush(new Color() { A = 0xFF, R = 0x04, G = 0x22, B = 0x71 });
            //});
            string s;
            Color = isHighlight && ((s = IsContainKeywords(msg)) != null)
                ? GetKeywordColor(s)
                : Brushes.DarkBlue;
        }

        public static void SerialPortLogLoadKeywordColorFromJson(string fileName)
        {
            if (fileName == null)
            {
                // FOR TEST
                string json = JsonConvert.SerializeObject(
                    new Dictionary<string, Brush>()
                    {
                        { "PASS", Brushes.Green },
                        { "FAIL", Brushes.Red },
                        { "__DEFAULT__", Brushes.Orange },
                    });
                File.WriteAllText(fileName, json);
            }
            else
            {
                string json = File.ReadAllText(fileName);
                KeywordColor = JsonConvert.DeserializeObject<Dictionary<string, Brush>>(json);
            }
        }

        private static string IsContainKeywords(string msg)
        {
            if (msg == string.Empty)
                return null;
            foreach (string i in KeywordColor.Keys)
            {
                if (msg.Contains(i, StringComparison.CurrentCultureIgnoreCase))
                {
                    return i.ToUpper();
                }
            }
            return null;
        }

        private static Brush GetKeywordColor(string s)
        {
            return KeywordColor.ContainsKey(s) ? KeywordColor[s] : KeywordColor["__DEFAULT__"];
        }

        public override string ToString()
        {
            return $"{Time:yyyy-MM-dd hh:mm:ss.fff} | {Host} | {Message}";
        }
    }
}
