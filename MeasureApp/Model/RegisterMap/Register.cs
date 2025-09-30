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
    /// 硬件寄存器，包含一个或多个位段
    /// </summary>
    public partial class Register : ObservableObject
    {
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
        private uint value;

        /// <summary>
        /// 寄存器包含的位段列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<BitsField> bitFields = new();
    }
}
