//using log4net.Core;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Text;

//namespace D2RStart
//{
//    public abstract class NotifyPropertyChangedAppenderBase : log4net.Appender.AppenderSkeleton, INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        protected virtual void OnPropertyChanged(string propertyName)
//        {
//            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        protected virtual bool OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
//        {
//            if (EqualityComparer<T>.Default.Equals(field, value))
//            {
//                return false;
//            }
//            else
//            {
//                field = value;
//                this.OnPropertyChanged(propertyName);
//                return true;
//            }
//        }
//    }
//}
