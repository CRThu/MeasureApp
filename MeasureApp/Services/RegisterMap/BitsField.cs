using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.RegisterMap
{
    /// <summary>
    /// 位段
    /// </summary>
    public partial class BitsField : ObservableObject
    {
        public delegate void ValueUpdateHandler(BitsField bf);
        public event ValueUpdateHandler OnValueUpdate;

        /// <summary>
        /// 所属寄存器
        /// </summary>
        public Register Parent { get; init; }

        /// <summary>
        /// 位段名称
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 起始位
        /// </summary>
        public uint StartBit { get; init; }

        /// <summary>
        /// 结束位
        /// </summary>
        public uint EndBit { get; init; }

        /// <summary>
        /// 功能描述
        /// </summary>
        public string Desc { get; init; }

        /// <summary>
        /// 位段的值
        /// </summary>
        private uint? _value;
        public uint? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    SetProperty(ref _value, value);
                    OnValueUpdate?.Invoke(this);
                }
            }
        }

        public BitsField(Register parent, string name, uint startBit, uint endBit, string desc, uint? value = null)
        {
            Parent = parent;
            Name = name;
            StartBit = startBit;
            EndBit = endBit;
            Desc = desc;
            Value = value;
        }
    }
}