using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MeasureApp.ViewModel.Common
{
    public class EventCommandBase : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// 事件
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandBase), new PropertyMetadata(null));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// 事件参数
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCommandBase), new PropertyMetadata(null));
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// 事件命令
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {

            if (CommandParameter != null)
            {
                parameter = CommandParameter;
            }
            if (Command != null)
            {
                Command.Execute(parameter);
            }
        }
    }
}
