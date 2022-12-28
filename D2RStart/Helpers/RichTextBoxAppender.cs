//using log4net.Appender;
//using log4net.Config;
//using log4net;
//using log4net.Core;
//using log4net.Filter;
//using log4net.Layout;
//using log4net.Repository.Hierarchy;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Media;

//namespace D2RStart
//{
//    internal class RichTextBoxAppender : NotifyPropertyChangedAppenderBase
//    {
//        #region Publioc constants
//        public const string DefaultPatternLayout = " %date{HH:mm:ss,fff} [%logger] [%level] %message%newline";
//        #endregion

//        #region Private fields
//        private LoggerMatchFilter nHibernateLoggerMatchFilter = new LoggerMatchFilter()
//        {
//            AcceptOnMatch = false,
//            LoggerToMatch = "NHibernate",
//        };
//        #endregion

//        #region Constructor
//        public RichTextBoxAppender(RichTextBox richTextBox)
//        {
//            if (richTextBox == null)
//                throw new ArgumentNullException(nameof(richTextBox));

//            RichTextBox = richTextBox;
//            Settings = new RichTextBoxAppenderSettings();
//            AddFilter(nHibernateLoggerMatchFilter);

//            PatternLayout layout = new PatternLayout();
//            layout.ConversionPattern = DefaultPatternLayout;
//            layout.ActivateOptions();

//            Settings.PropertyChanged += Settings_PropertyChanged;
//        }
//        #endregion

//        #region Public events
//        #endregion

//        #region Public proeprties
//        public RichTextBox RichTextBox { get; }
//        public RichTextBoxAppenderSettings Settings { get; }
//        #endregion

//        #region Protected methods
//        protected override void Append(LoggingEvent loggingEvent)
//        {
//            if (loggingEvent == null || RichTextBox == null)
//                return;

//            Application.Current.Dispatcher.Invoke(new Action<LoggingEvent>((logEvent) =>
//            {
//                while (RichTextBox.Document.Blocks.Count >= Settings.MaximumRowCount)
//                    RichTextBox.Document.Blocks.Remove(RichTextBox.Document.Blocks.FirstBlock);

//                var textRangeOld = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
//                TextRange textRangeNewText = new TextRange(this.RichTextBox.Document.ContentEnd, this.RichTextBox.Document.ContentEnd);
//                textRangeNewText.Text = (textRangeOld.Text.Length > 0 ? "\r\n" : string.Empty) + this.RenderLoggingEvent(logEvent);

//                switch (logEvent.Level.Name)
//                {
//                    case nameof(Level.Fatal):
//                    case nameof(Level.Error):
//                    case nameof(Level.Critical):
//                    case nameof(Level.Emergency):
//                        textRangeNewText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
//                        break;

//                    case nameof(Level.Warn):
//                        textRangeNewText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.OrangeRed);
//                        break;

//                    case nameof(Level.Info):
//                        textRangeNewText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
//                        break;


//                    case nameof(Level.Debug):
//                        textRangeNewText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
//                        break;

//                    default:
//                        textRangeNewText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkGray);
//                        break;
//                }

//                if (Settings.ScrollingIsEnabled)
//                    RichTextBox.ScrollToEnd();

//            }), loggingEvent);
//        }
//        #endregion

//        #region Event methods
//        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            var setting = sender as RichTextBoxAppenderSettings;
//            if (setting == null || string.IsNullOrEmpty(e?.PropertyName))
//                return;

//            switch (e.PropertyName)
//            {
//                case nameof(RichTextBoxAppenderSettings.Threshold):
//                    if (setting.Threshold == null)
//                        return;

//                    if (Threshold.Value != setting.Threshold.Value)
//                        Threshold = setting.Threshold;
//                    break;

//                case nameof(RichTextBoxAppenderSettings.NHibernateLoggingIsEnabled):
//                    if(nHibernateLoggerMatchFilter.AcceptOnMatch != setting.NHibernateLoggingIsEnabled)
//                        nHibernateLoggerMatchFilter.AcceptOnMatch = setting.NHibernateLoggingIsEnabled;
//                    break;
//            }
//        }
//        #endregion

//        #region static methods
//        public static void Initialize(RichTextBox richTextBox)
//        {
//            var hierarchy = (Hierarchy)LogManager.GetRepository();

//            PatternLayout patternLayout = new PatternLayout
//            {
//                ConversionPattern = "%date %level %message%newline"
//            };
//            patternLayout.ActivateOptions();

//            RichTextBoxAppender richTextBoxAppender = new RichTextBoxAppender(richTextBox);
//            richTextBoxAppender.Layout = patternLayout;

//            hierarchy.Root.AddAppender(richTextBoxAppender);
            
//            hierarchy.Root.Level = Level.All;
//            hierarchy.Configured = true;

//            BasicConfigurator.Configure(hierarchy);
//        }
//        #endregion
//    }
//}
