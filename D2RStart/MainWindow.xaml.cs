using D2RStart.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace D2RStart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {            
            InitializeComponent();
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            if (App.Settings.WindowClosingSize.Width >= MinWidth)
            {
                Width = App.Settings.WindowClosingSize.Width;
            }

            if (App.Settings.WindowClosingSize.Height >= MinHeight)
            {
                Height = App.Settings.WindowClosingSize.Height;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }

                App.Settings.WindowClosingSize.Width = Width;
                App.Settings.WindowClosingSize.Height = Height;
                App.Settings.Save();
            }
            catch { }            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TbVersion.Text = $"V{Assembly.GetAssembly(typeof(MainWindow)).GetName().Version}";

                //if (App.Settings.WindowClosingSize.Width >= MinWidth)
                //{
                //    Width = App.Settings.WindowClosingSize.Width;
                //}

                //if (App.Settings.WindowClosingSize.Height >= MinHeight)
                //{
                //    Height = App.Settings.WindowClosingSize.Height;
                //}
            }
            catch { }            
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }
    }
}
