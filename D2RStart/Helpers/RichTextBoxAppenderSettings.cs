//using log4net.Core;
//using log4net.Filter;
//using log4net.Layout;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace D2RStart
//{
//    internal class RichTextBoxAppenderSettings : NotifyPropertyChangedBase
//    {
//        private Level threshold = Level.All;
//        public Level Threshold
//        {
//            get { return threshold; }
//            set { base.OnPropertyChanged<Level>(ref threshold, value); }
//        }

//        private bool scrollingIsEnabled = true;
//        public bool ScrollingIsEnabled
//        {
//            get { return scrollingIsEnabled; }
//            set { base.OnPropertyChanged<bool>(ref scrollingIsEnabled, value); }
//        }

//        private int maximumRowCount = 3000;
//        public int MaximumRowCount
//        {
//            get { return maximumRowCount; }
//            set { base.OnPropertyChanged<int>(ref maximumRowCount, value); }
//        }

//        private bool nHibernateLoggingIsEnabled = false;
//        public bool NHibernateLoggingIsEnabled
//        {
//            get { return nHibernateLoggingIsEnabled; }
//            set { base.OnPropertyChanged<bool>(ref nHibernateLoggingIsEnabled, value); }
//        }
//    }
//}
