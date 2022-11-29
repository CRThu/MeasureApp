using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.SerialPortScript
{
    public enum SerialPortScriptVariableType
    {
        VAR,
        LIST
    }

    public class SerialPortScriptVariable
    {
        public SerialPortScriptVariableType Type { get; set; }
        public string Name { get; set; }
        public ObservableRangeCollection<decimal> Value { get; set; }
        public decimal Value1
        {
            get
            {
                return Value[0];
            }
            set
            {
                Value.Add(value);
            }
        }

        public SerialPortScriptVariable(string name, IEnumerable<decimal> value)
        {
            Type = SerialPortScriptVariableType.LIST;
            Name = name;
            Value = new(value);
        }

        public SerialPortScriptVariable(string name, decimal value1)
        {
            Type = SerialPortScriptVariableType.VAR;
            Name = name;
            Value = new();
            Value1 = value1;
        }

        public string ValueToString()
        {
            if (Type.Equals(SerialPortScriptVariableType.VAR))
                return $"{Value1}";
            else
                return $"[{string.Join(", ", Value)}]";
        }

        public override string ToString()
        {
                return $"{{{Name} : {ValueToString()}}}";
        }
    }
}
