using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Windows.Shell;
using D2RStart.Models;

namespace D2RStart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static D2RStartApplicationSettings Settings { get; set; } = D2RStartApplicationSettings.Load();

        void Application_Startup(object sender, StartupEventArgs e)
        {
            string handle64ExeFile = Path.Combine(AppContext.BaseDirectory, "handle64.exe");

            //Assembly.GetExecutingAssembly().
            if (File.Exists(handle64ExeFile))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                DownloadHandle64ExeWindow downloadHandle64ExeWindow = new DownloadHandle64ExeWindow();
                downloadHandle64ExeWindow.Show();
            }
        }
    }
}
