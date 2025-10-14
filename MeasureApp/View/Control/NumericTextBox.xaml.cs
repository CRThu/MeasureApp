using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeasureApp.View.Control
{
    public enum NumericBase
    {
        Hex,
        Dec
    }

    /// <summary>
    /// NumericTextBox.xaml 的交互逻辑
    /// </summary>
    [INotifyPropertyChanged]
    public partial class NumericTextBox : UserControl
    {
        // 标志位，防止更新Value时触发的DisplayText更新与用户输入循环触发
        private bool _isUpdating = false;

        public NumericTextBox()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        // 核心数据属性，用于和ViewModel绑定
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(uint?), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public uint? Value
        {
            get { return (uint?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericTextBox)d;
            control.UpdateDisplayText();
        }

        #endregion

        #region Observable Properties for Internal UI State

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayText))]
        private NumericBase currentBase = NumericBase.Hex;

        [ObservableProperty]
        private string displayText = "";

        // 当CurrentBase改变时，更新显示的文本
        partial void OnCurrentBaseChanged(NumericBase value)
        {
            UpdateDisplayText();
        }

        // 当用户输入导致DisplayText改变时，尝试解析并更新Value
        partial void OnDisplayTextChanged(string value)
        {
            UpdateValueFromText(value);
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void SelectDec() => CurrentBase = NumericBase.Dec;

        [RelayCommand]
        private void SelectHex() => CurrentBase = NumericBase.Hex;

        [RelayCommand]
        private void Set1() => Value = 1;

        [RelayCommand]
        private void Set0() => Value = 0;

        #endregion

        #region Private Helper Methods

        // 从Value更新DisplayText（模型 -> 视图）
        private void UpdateDisplayText()
        {
            _isUpdating = true; // 设置标志位
            if (!Value.HasValue)
            {
                DisplayText = string.Empty;
            }
            else if (CurrentBase == NumericBase.Dec)
            {
                DisplayText = Value.Value.ToString();
            }
            else
            {
                DisplayText = Value.Value.ToString("X");
            }
            _isUpdating = false; // 清除标志位
        }

        // 从DisplayText更新Value（视图 -> 模型）
        private void UpdateValueFromText(string text)
        {
            if (_isUpdating)
                return; // 如果是内部更新触发的，则忽略

            if (string.IsNullOrWhiteSpace(text))
            {
                Value = null;
                return;
            }

            uint parsedValue;
            bool success;

            if (CurrentBase == NumericBase.Dec)
            {
                success = uint.TryParse(text, out parsedValue);
            }
            else // Hex
            {
                success = uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out parsedValue);
            }

            if (success)
            {
                Value = parsedValue;
            }
            else
            {
                //Value = null;
            }
        }

        #endregion
    }
}
