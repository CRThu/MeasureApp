using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public partial class DataLogger : ObservableObject
    {
        [ObservableProperty]
        public DataStorage<double> ds = new();

        /// <summary>
        /// 增加记录并解析数据到DataStorage
        /// </summary>
        /// <param name="record"></param>
        public void Add(IRecord record)
        {
            if (record.Type == TransferType.Data)
            {
                string streamKey = $"{record.From}.{record.Stream}";
                Ds.AddValue(streamKey,record.Frame.DecodeData());
            }
        }
    }
}
