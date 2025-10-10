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
    /// 位段
    /// </summary>
    public partial class BitsField : ObservableObject
    {
        /// <summary>
        /// 位段名称
        /// </summary>
        [ObservableProperty]
        private string name;

        /// <summary>
        /// 起始位
        /// </summary>
        [ObservableProperty]
        private uint startBit;

        /// <summary>
        /// 结束位
        /// </summary>
        [ObservableProperty]
        private uint endBit;

        /// <summary>
        /// 位段的值
        /// </summary>
        [ObservableProperty]
        private uint? value;

        /// <summary>
        /// 功能描述
        /// </summary>
        [ObservableProperty]
        private string desc;
    }
}
