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
        [ObservableProperty]
        private string name;

        /// <summary>
        /// 寄存器地址
        /// </summary>
        [ObservableProperty]
        private uint address;

        /// <summary>
        /// 位宽
        /// </summary>
        [ObservableProperty]
        private uint bitWidth;

        /// <summary>
        /// 寄存器的完整数值
        /// </summary>
        [ObservableProperty]
        private uint? value;

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
            var bf = new BitsField()
            {
                Parent = this,
                Name = name,
                StartBit = startBit,
                EndBit = endBit,
                Desc = desc,
                Value = null
            };
            BitFields.Add(bf);
            return this;
        }
    }
}
