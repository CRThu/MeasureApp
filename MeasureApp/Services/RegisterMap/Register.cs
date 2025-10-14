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
    /// 硬件寄存器，包含一个或多个位段
    /// </summary>
    public partial class Register : ObservableObject
    {
        /// <summary>
        /// 所属寄存器表
        /// </summary>
        public RegFile Parent { get; init; }

        /// <summary>
        /// 寄存器名称
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 寄存器地址
        /// </summary>
        public uint Address { get; init; }

        /// <summary>
        /// 位宽
        /// </summary>
        public uint BitWidth { get; init; }

        /// <summary>
        /// 寄存器的完整数值
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
                    UpdateToBitFields(value);
                }
            }
        }

        /// <summary>
        /// 寄存器包含的位段列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<BitsField> bitFields = new();

        /// <summary>
        /// 链式添加寄存器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="bitWidth"></param>
        /// <returns></returns>
        public Register AddReg(string name, uint address, uint bitWidth)
            => Parent.AddReg(name, address, bitWidth);

        /// <summary>
        /// 链式添加位段
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startBit"></param>
        /// <param name="endBit"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Register AddBits(string name, uint startBit, uint endBit, string desc)
        {
            var bf = new BitsField(this, name, startBit, endBit, desc, null);
            bf.OnValueUpdate += UpdateFromBitFields;
            BitFields.Add(bf);
            return this;
        }

        private void UpdateFromBitFields(BitsField _)
        {
            if (BitFields.All(bf => bf.Value.HasValue))
            {
                uint val = 0;
                foreach (BitsField bf in BitFields)
                {
                    BitsHelper.SetBitsUInt32(ref val, (int)bf.StartBit, (int)bf.EndBit, bf.Value.Value);
                }
                Value = val;
            }
        }

        private void UpdateToBitFields(uint? value)
        {
            if (value.HasValue)
            {
                foreach (BitsField bf in BitFields)
                {
                    uint newBfVal = BitsHelper.GetBitsUInt32(value.Value, (int)bf.StartBit, (int)bf.EndBit);
                    if (!bf.Value.HasValue || bf.Value != newBfVal)
                    {
                        bf.Value = newBfVal;
                    }
                }
            }
        }
    }
}
