using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace D2RStart.Models
{
    internal class D2RItem : NotifyPropertyChangedBase
    {
        public D2RItem()
        {
        }

        private string d2rPath = string.Empty;
        public string D2RPath
        {
            get { return d2rPath; }
            set { base.OnPropertyChanged<string>(ref d2rPath, value); }
        }

        private D2RStatus d2rStatus = D2RStatus.Stopped;
        [JsonIgnore]
        public D2RStatus D2RStatus
        {
            get { return d2rStatus; }
            set { base.OnPropertyChanged<D2RStatus>(ref d2rStatus, value); }
        }

        public void UpdateItemStatus()
        {
            string instancePath = string.Empty;            
            D2RStatus oldStatus = D2RStatus.Stopped;            
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                instancePath = $"{D2RPath}";
                oldStatus = D2RStatus;
            })); ;

            D2RStatus newStatus = D2RStatus.Stopped;

            Process d2rProcess = Process.GetProcesses().Where(p=> p.ProcessName.ToLower() == "d2r"  && Path.GetDirectoryName(p.MainModule.FileName).ToLower() == instancePath.ToLower()).FirstOrDefault();
            if (d2rProcess != null)
                newStatus = D2RStatus.ExecuteingD2R;
                
            if (oldStatus != newStatus)
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    D2RStatus = newStatus;
                }));
        }
    }
}
