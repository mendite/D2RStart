using D2RStart.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace D2RStart
{
    /// <summary>
    /// Interaction logic for DownloadHandle64ExeWindow.xaml
    /// </summary>
    public partial class DownloadHandle64ExeWindow : Window
    {
        public DownloadHandle64ExeWindow()
        {
            InitializeComponent();
            SourceInitialized += DownloadHandle64ExeWindow_SourceInitialized;
        }

        private void DownloadHandle64ExeWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));                
                //Process.Start($"{e.Uri.AbsoluteUri}");
                System.Diagnostics.Process.Start("cmd", $"/c start {e.Uri.AbsoluteUri}");
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex?.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
