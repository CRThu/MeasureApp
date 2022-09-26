using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DataStorage
{
    public class DataStorageJsonSerializeStructure : Dictionary<string, Dictionary<string, List<double>>>
    {
        public DataStorageJsonSerializeStructure() : base()
        {
        }

        public DataStorageJsonSerializeStructure(DataStorage ds) : base()
        {
            foreach (var item in ds.Data)
            {
                Add(item.Key, new Dictionary<string, List<double>>()
                {
                    { "X", new List<double>(item.Value.X) },
                    { "Y", new List<double>(item.Value.Y) },
                });
            }
        }

        public Dictionary<string, DataStorageElement> ToDataStorageElements()
        {
            Dictionary<string, DataStorageElement> data = new();
            foreach (var item in this)
            {
                DataStorageElement d = new();
                d.AddXY(item.Value["X"], item.Value["Y"]);
                data.Add(item.Key, d);
            }
            return data;
        }
    }
}
