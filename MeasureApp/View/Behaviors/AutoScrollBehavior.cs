using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.View.Behaviors
{
    // Behavior<T> T设置行为产生作用的元素类型
    // ListBox/ListView
    public class AutoScrollBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty IsAutoScrollProperty = DependencyProperty.Register("IsAutoScroll", typeof(bool), typeof(AutoScrollBehavior));

        public bool IsAutoScroll
        {
            get => (bool)GetValue(IsAutoScrollProperty);
            set => SetValue(IsAutoScrollProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            ((ICollectionView)AssociatedObject.Items).CollectionChanged += AutoScrollBehavior_CollectionChanged;
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((ICollectionView)AssociatedObject.Items).CollectionChanged -= AutoScrollBehavior_CollectionChanged;
        }

        private void AutoScrollBehavior_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (AssociatedObject.HasItems && IsAutoScroll)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[^1]);
            }
        }
    }
}
