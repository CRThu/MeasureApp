using AvalonDock.Layout.Serialization;
using AvalonDock;
using CommunityToolkit.Mvvm.Messaging;
using MeasureApp.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Microsoft.Win32;

namespace MeasureApp.View.Behaviors
{
    /// <summary>
    /// 一个附加到DockingManager的行为，通过Messenger消息来触发布局的保存和还原。
    /// </summary>
    public class DockingManagerLayoutBehavior : Behavior<DockingManager>,
        IRecipient<SaveLayoutMessage>,
        IRecipient<RestoreLayoutMessage>
    {
        /// <summary>
        /// 在行为附加到UI元素时调用
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            // 注册以接收消息
            WeakReferenceMessenger.Default.Register<SaveLayoutMessage>(this);
            WeakReferenceMessenger.Default.Register<RestoreLayoutMessage>(this);
        }

        /// <summary>
        /// 在行为从UI元素分离时调用
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            // 注销所有消息，防止内存泄漏
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// 接收到 SaveLayoutMessage 时的处理程序
        /// </summary>
        public void Receive(SaveLayoutMessage message)
        {
            if (AssociatedObject == null)
                return;
            SaveLayout(AssociatedObject);
        }

        /// <summary>
        /// 接收到 RestoreLayoutMessage 时的处理程序
        /// </summary>
        public void Receive(RestoreLayoutMessage message)
        {
            if (AssociatedObject == null)
                return;
            RestoreLayout(AssociatedObject);
        }


        /// <summary>
        /// 保存布局
        /// </summary>
        /// <param name="dockingManager">DockingManager实例</param>
        private void SaveLayout(DockingManager dockingManager)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Layout Files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "保存布局文件",
                FileName = "layout.xml"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var serializer = new XmlLayoutSerializer(dockingManager);
                    using var stream = new StreamWriter(saveFileDialog.FileName);
                    serializer.Serialize(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存布局失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 还原布局
        /// </summary>
        /// <param name="dockingManager">DockingManager实例</param>
        private void RestoreLayout(DockingManager dockingManager)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Layout Files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "加载布局文件"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (!File.Exists(openFileDialog.FileName))
                {
                    MessageBox.Show("布局文件不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    var serializer = new XmlLayoutSerializer(dockingManager);
                    using var stream = new StreamReader(openFileDialog.FileName);
                    serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"还原布局失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
