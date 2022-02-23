using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public App()
        //{
        //    Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        //    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        //}

        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show($"Unhandled Exception: {e.ExceptionObject}");
        //}

        //private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show($"Unhandled Exception: {e.Exception}");
        //    e.Handled = true;
        //}
    }
}
