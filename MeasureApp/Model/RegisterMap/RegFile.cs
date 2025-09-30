using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.RegisterMap
{
    /// <summary>
    /// 寄存器文件，包含一个寄存器集合
    /// </summary>
    public partial class RegFile : ObservableObject
    {
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
    }
}
