using MeasureApp.Model;
using MeasureApp.ViewModel;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowDataContext();

            // 默认文件选择路径加载
            if (Properties.Settings.Default.DefaultDirectory == string.Empty)
                Properties.Settings.Default.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // TEST
            //Button btn = new Button();
            //btn.Content = "Button";
            //var triggers = Interaction.GetTriggers(btn);
            //Microsoft.Xaml.Behaviors.EventTrigger trigger1 = new("Click");
            //trigger1.Actions.Add(new EventCommandBase() { Command = TestEvent });
            //triggers.Add(trigger1);
            //SettingsGrid.Children.Add(btn);
        }
    }
}
