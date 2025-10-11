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
    /// 寄存器文件，包含一个寄存器集合
    /// </summary>
    public partial class RegFile : ObservableObject
    {
        /// <summary>
        /// 寄存器文件索引
        /// </summary>
        [ObservableProperty]
        private uint index;

        /// <summary>
        /// 寄存器文件名称
        /// </summary>
        [ObservableProperty]
        private string name;

        /// <summary>
        /// 寄存器文件包含的寄存器列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Register> registers = new();

        /// <summary>
        /// 链式添加寄存器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="bitWidth"></param>
        /// <returns></returns>
        public Register AddReg(string name, uint address, uint bitWidth)
        {
            var reg = new Register()
            {
                Parent = this,
                Name = name,
                Address = address,
                BitWidth = bitWidth,
                Value = null
            };
            Registers.Add(reg);
            return reg;
        }
    }
}
