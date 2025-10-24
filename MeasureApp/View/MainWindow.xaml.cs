using MeasureApp.Model;
using MeasureApp.ViewModel;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public AppConfig AppConfig { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            //AppConfig = AppConfig.Read();

            //DataContext = new MainWindowDataContext(AppConfig);
        }
    }
}
