using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace D2RStart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
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
