using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel.Common
{
    /*
     * USAGE:
     * XAML:
        <i:Interaction.Behaviors>
        <behaviors:InvokeMethodBehavior MethodName="Close" InvokeMethodAction="{Binding CloseAction,Mode=OneWayToSource}"/>
        </i:Interaction.Behaviors>
     * CODE:
        public Action CloseAction { get; set; }

        private void Close()
        {
            CloseAction?.Invoke();
        }
     * REFERENCE:
        https://www.cnblogs.com/ponus/p/15753122.html
      */

    /// <summary>
    /// 调用方法行为,通过指定方法名称,将无参的方法作为委托传入到后台;
    /// </summary>
    public class InvokeMethodBehavior : Behavior<DependencyObject>
    {
        /// <summary>
        /// 调用方法委托;
        /// </summary>
        public Action InvokeMethodAction
        {
            get { return (Action)GetValue(InvokeMethodActionProperty); }
            set { SetValue(InvokeMethodActionProperty, value); }
        }

        public static readonly DependencyProperty InvokeMethodActionProperty =
            DependencyProperty.Register(nameof(InvokeMethodAction), typeof(Action), typeof(InvokeMethodBehavior), new FrameworkPropertyMetadata(default(Action)) { BindsTwoWayByDefault = true });

        /// <summary>
        /// 方法名称;
        /// </summary>
        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(InvokeMethodBehavior), new PropertyMetadata(null));


        protected override void OnAttached()
        {
            InvokeMethodAction = InvokeMethod;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            InvokeMethodAction = null;
            base.OnDetaching();
        }

        private void InvokeMethod()
        {
            if (string.IsNullOrEmpty(MethodName))
            {
                Trace.WriteLine($"The {nameof(MethodName)} can not be null or empty.");
            }
            if (AssociatedObject == null)
            {
                return;
            }
            try
            {
                AssociatedObject.GetType().GetMethod(MethodName).Invoke(AssociatedObject, null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error occured while invoking method({MethodName}):{ex.Message}");
            }
        }
    }
}
