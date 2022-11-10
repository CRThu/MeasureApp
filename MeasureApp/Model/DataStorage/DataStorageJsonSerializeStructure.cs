using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DataStorage
{
    //public class DataStorageJsonSerializeStructure : Dictionary<string, Dictionary<string, List<double>>>
    public class DataStorageJsonSerializeStructure : Dictionary<string, JToken>
    {
        public DataStorageJsonSerializeStructure() : base()
        {
        }

        public DataStorageJsonSerializeStructure(DataStorage ds) : base()
        {
            foreach (var item in ds.Data)
            {
                var xyArray = new Dictionary<string, List<double>>()
                {
                    { "X", new List<double>(item.Value.X) },
                    { "Y", new List<double>(item.Value.Y) },
                };
                var jxyArray = JToken.FromObject(xyArray);


                Add(item.Key, jxyArray);
            }
        }

        public Dictionary<string, DataStorageElement> ToDataStorageElements()
        {
            Dictionary<string, DataStorageElement> data = new();
            foreach (var item in this)
            {
                JToken dataObj = item.Value;
                if (dataObj is JArray)
                {
                    // 旧版本json文件解析
                    // 格式为 { "k0": [v0, v1, v2], "k1": [v0, v1, v2] }
                    /*
                    {
                        "aldo_trim": [
                            1.659,
                            1.650,
                            1.641
                        ],
                        "dldo_trim": [
                            1.666,
                            1.657,
                            1.648
                        ]
                    }
                                        */

                    DataStorageElement ele0 = new();
                    var yArray0 = ((JArray)dataObj).Values<double>();
                    ele0.AddY(yArray0);
                    data.Add(item.Key, ele0);
                }
                else
                {
                    // 新版本json文件解析
                    // 格式为 { "k0": { "X":[v0, v1, v2], "Y":[v0, v1, v2]},
                    //          "k1": { "X":[v0, v1, v2], "Y":[v0, v1, v2]} }
                    /*
                    {
                        "Key1": {
                            "X": [1,2,3],
                            "Y": [4,5,6]
                        },
                        "Key2": {
                            "X": [1,2,3],
                            "Y": [1,4,9]
                        },
                        "Key3": {
                            "X": [1,2,3],
                            "Y": [1,8,27]
                        }
                    }
                                            */
                    var dataDict = new Dictionary<string, JToken>(((JObject)dataObj));
                    var xArray = ((JArray)dataDict["X"]).Values<double>();
                    var yArray = ((JArray)dataDict["Y"]).Values<double>();
                    DataStorageElement d = new();
                    d.AddXY(xArray, yArray);
                    data.Add(item.Key, d);
                }
            }
            return data;
        }
    }
}
