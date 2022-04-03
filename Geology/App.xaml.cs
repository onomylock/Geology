﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Geology
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public partial class App : Application {
        
    void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Exception ex = e.Exception;
        Exception ex_inner = ex.InnerException;
        string msg = "";
        if (ex != null)
            msg = ex.Message + "\n\n" + ex.StackTrace + "\n\n";
        if (ex_inner != null)
            msg += "Inner Exception:\n" + ex_inner.Message + "\n\n" + ex_inner.StackTrace;
        //MessageBox.Show(msg, "Application Halted!", MessageBoxButton.OK);
        e.Handled = true;
        //Application.Current.Shutdown();
    }
        
}
    public partial class App : Application
    {
    }
}
