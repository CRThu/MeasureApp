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
     * <NONE>
     * CODE:
        private void InitializeLoggerListBox()
        {
            //绑定滚动到指定项委托;
            var loggerListBoxBehaviors = Interaction.GetBehaviors(loggerListBox);
            var invokeScrollBehavior = new InvokeMethodBehavior<ListBox, Action<object>>
            {
                GetMethodDelegateFunc = listBox => listBox.ScrollIntoView
            };
            BindingOperations.SetBinding(invokeScrollBehavior,InvokeMethodBehavior<ListBox,Action<object>>.MethodDelegateProperty,new Binding { Path = new PropertyPath("ScrollIntoLoggerBoxItemAction"),Mode = BindingMode.OneWayToSource });
            loggerListBoxBehaviors.Add(invokeScrollBehavior);
        }
    
        /// <summary>
        /// 将控制台滚动到指定项的委托;
        /// </summary>
        public Action<object> ScrollIntoLoggerBoxItemAction {get;set;}
          
     * REFERENCE:
          https://www.cnblogs.com/ponus/p/15753122.html
     */

    /// <summary>
    /// 调用对象的方法行为,使用此行为,将前台元素的方法通过委托传入依赖属性,此行为适合调用的方法具备参数时的情况,因为使用了泛型,无法在XAML中直接使用,请在后台代码中使用;
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    public class InvokeMethodBehavior<TObject, TDelegate> :
        Behavior<TObject>
        where TDelegate : Delegate
        where TObject : DependencyObject
    {
        /// <summary>
        /// 方法委托;
        /// </summary>
        public TDelegate MethodDelegate
        {
            get { return (TDelegate)GetValue(MethodDelegateProperty); }
            set { SetValue(MethodDelegateProperty, value); }
        }

        public static readonly DependencyProperty MethodDelegateProperty =
            DependencyProperty.Register(nameof(MethodDelegate), typeof(TDelegate), typeof(InvokeMethodBehavior<TObject, TDelegate>), new FrameworkPropertyMetadata(default(TDelegate)) { BindsTwoWayByDefault = true });

        private Func<TObject, TDelegate> _getMethodDelegateFunc;
        /// <summary>
        /// 获取或设定获得 方法委托 的委托;
        /// </summary>
        public Func<TObject, TDelegate> GetMethodDelegateFunc
        {
            get => _getMethodDelegateFunc;
            set
            {
                _getMethodDelegateFunc = value;
                RefreshMethodDelegate();
            }
        }

        protected override void OnAttached()
        {
            RefreshMethodDelegate();
            base.OnAttached();
        }
        protected override void OnDetaching()
        {
            RefreshMethodDelegate();
            base.OnDetaching();
        }

        /// <summary>
        /// 刷新<see cref="MethodDelegate"/>属性;
        /// </summary>
        private void RefreshMethodDelegate()
        {
            if (AssociatedObject == null || GetMethodDelegateFunc == null)
            {
                MethodDelegate = null;
            }

            try
            {
                MethodDelegate = GetMethodDelegateFunc(AssociatedObject);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error occured while refreshing method delegate:{ex.Message}");
            }
        }
    }
}
