using MeasureApp.Plugins.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Services
{
    public class MeasureAppService : IMeasureAppService
    {
        public void Send(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
