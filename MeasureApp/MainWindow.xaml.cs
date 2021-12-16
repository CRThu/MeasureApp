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
        }
    }
}
