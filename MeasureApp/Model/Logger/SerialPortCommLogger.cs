using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model.Logger
{
    public class SerialPortCommLogger : NotificationObjectBase
    {
        private object logCollectionLocker = new();
        private ObservableRangeCollection<SerialPortCommLogElement> logCollection = new();
        public ObservableRangeCollection<SerialPortCommLogElement> LogCollection
        {
            get => logCollection;
            set
            {
                lock (logCollectionLocker)
                {
                    logCollection = value;
                }
                RaisePropertyChanged(() => LogCollection);
            }
        }

        public SerialPortCommLogger()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BindingOperations.EnableCollectionSynchronization(LogCollection, logCollectionLocker);
            });
        }

        public SerialPortCommLogger(string keywordConfigFile) : this()
        {
            LoadKeywordFile(keywordConfigFile);
        }

        public bool IsLastLogContains(string host, string keyword)
        {
            SerialPortCommLogElement lastLogFromHost = LogCollection.LastOrDefault(log => log.Host == host);
            return lastLogFromHost is not null && ((string)lastLogFromHost.Message.ToString()).Contains(keyword, StringComparison.CurrentCultureIgnoreCase);
        }

        public void LoadKeywordFile(string keywordConfigFile)
        {
            SerialPortCommLogElement.SerialPortLogLoadKeywordColorFromJson(keywordConfigFile);
        }

        public void Clear()
        {
            lock (logCollectionLocker)
            {
                LogCollection.Clear();
            }
        }

        public void Add(SerialPortCommLogElement log)
        {
            lock (logCollectionLocker)
            {
                LogCollection.Add(log);
            }
        }
        public void Add(dynamic msg, string host = "<null>", bool isHighlight = false)
        {
            Add(new SerialPortCommLogElement(msg, host, isHighlight));
        }

        public void AddRange(IEnumerable<SerialPortCommLogElement> collection)
        {
            lock (logCollectionLocker)
            {
                LogCollection.AddRange(collection);
            }
        }

        public void AddRange(string[] msgs, string host = "<null>", bool isHighlight = false)
        {
            AddRange(msgs.Select(msg => new SerialPortCommLogElement(msg, host, isHighlight)));
        }

        public string[] ToStrings()
        {
            return LogCollection.Select(l => l.ToString()).ToArray();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, ToStrings());
        }

        public void Save(string fileName)
        {
            File.WriteAllLines(fileName, ToStrings());
        }
    }
}
