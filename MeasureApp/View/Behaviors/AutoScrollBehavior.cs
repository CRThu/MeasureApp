using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MeasureApp.View.Behaviors
{
    // Behavior<T> T设置行为产生作用的元素类型
    // ListBox/ListView
    public class AutoScrollBehavior : Behavior<ListBox>
    {
        private bool _isScrollQuened = false;
        private ScrollViewer _scrollViewer;

        public static readonly DependencyProperty IsAutoScrollProperty = DependencyProperty.Register("IsAutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(true));

        public bool IsAutoScroll
        {
            get => (bool)GetValue(IsAutoScrollProperty);
            set => SetValue(IsAutoScrollProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            ((ICollectionView)AssociatedObject.Items).CollectionChanged += AutoScrollBehavior_CollectionChanged;
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            ((ICollectionView)AssociatedObject.Items).CollectionChanged -= AutoScrollBehavior_CollectionChanged;
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = GetScrollViewer(AssociatedObject);
        }

        private void AutoScrollBehavior_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!IsAutoScroll || _isScrollQuened)
                return;

            _isScrollQuened = true;
            AssociatedObject.Dispatcher.InvokeAsync(ScrollToBottom, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void ScrollToBottom()
        {
            _isScrollQuened = false;

            if (AssociatedObject == null || !IsAutoScroll)
                return;

            try
            {
                if (_scrollViewer != null)
                {
                    _scrollViewer.ScrollToBottom();
                }
                else if (AssociatedObject.HasItems)
                {
                    AssociatedObject.ScrollIntoView(AssociatedObject.Items[^1]);
                    Debug.WriteLine($"Warning in AutoScrollBehavior.ScrollToBottom(): Cannot find ScrollViewer.");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AutoScrollBehavior.ScrollToBottom(): {ex}");
            }
        }

        private static ScrollViewer GetScrollViewer(DependencyObject root)
        {
            if (root is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
