using System;
using System.Collections.Generic;
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

namespace MeasureApp.View.Pages
{
    /// <summary>
    /// SerialPortPacketPage.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortPacketPage : Page
    {
        public SerialPortPacketPage()
        {
            InitializeComponent();
        }

        public SerialPortPacketPage(object dataContext) : this()
        {
            DataContext = dataContext;
        }
    }
}
