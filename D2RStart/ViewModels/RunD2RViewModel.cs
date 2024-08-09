using D2RStart.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace D2RStart
{
    internal class RunD2RViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        public RunD2RViewModel()
        {
            CommandEditItems = new RelayCommand(ExecuteCommandEditItems, CanExecuteCommandEditItems);
            CommandAddItem = new RelayCommand(ExecuteCommandAddItem, CanExecuteCommandAddItem);
            CommandRemoveItem = new RelayCommand<D2RItem>(ExecuteCommandRemoveItem, CanExecuteCommandRemoveItem);
            CommandSaveConfiguration = new RelayCommand(ExecuteCommandSaveConfiguration, CanExecuteCommandSaveConfiguration);            
            CommandRestoreSavedConfiguration = new RelayCommand(ExecuteCommandRestoreSavedConfiguration, CanExecuteCommandRestoreSavedConfiguration);
            CommandStartNextD2RAndCopyAccountToClipboard = new RelayCommand<string>(ExecuteCommandStartNextD2RAndCopyAccountToClipboard, CanExecuteCommandStartNextD2RAndCopyAccountToClipboard);

            ConfigurationItems.CollectionChanged += ConfigurationItems_CollectionChanged;

            InitializeConfiguration();
        }
        #endregion

        #region Commands
        public IRelayCommand CommandEditItems { get; }
        public IRelayCommand CommandAddItem { get; }
        public IRelayCommand CommandRemoveItem { get; }
        public IRelayCommand CommandSaveConfiguration { get; }
        public IRelayCommand CommandRestoreSavedConfiguration { get; }
        public IRelayCommand CommandStartNextD2RAndCopyAccountToClipboard { get; }
        #endregion

        #region Public properties
        public ObservableCollection<D2RItem> ConfigurationItems { get; } = new ObservableCollection<D2RItem>();

        private D2RItem selectedConfigurationItem;
        public D2RItem SelectedConfigurationItem
        {
            get { return selectedConfigurationItem; }
            set 
            {
                base.OnPropertyChanged<D2RItem>(ref selectedConfigurationItem, value); 
                RaiseCanExecuteChangedOfCommands();
            }
        }

        private bool editingConfigurationIsEnabled = false;
        public bool EditingConfigurationIsEnabled
        {
            get { return editingConfigurationIsEnabled; }
            set 
            { 
                base.OnPropertyChanged<bool>(ref editingConfigurationIsEnabled, value);
                RaiseCanExecuteChangedOfCommands();
            }
        }

        public bool ConfigurationHasMoreThanTwoItems { get { return ConfigurationItems.Count > 2; } }

        private string outputMessage = string.Empty;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { base.OnPropertyChanged<string>(ref outputMessage, value); }
        }

        private bool outputMessageIsError = false;
        public bool OutputMessageIsError
        {
            get { return outputMessageIsError; }
            set { base.OnPropertyChanged<bool>(ref outputMessageIsError, value); }
        }
        #endregion

        #region Command methods
        private bool CanExecuteCommandSaveConfiguration(object obj)
        {
            return EditingConfigurationIsEnabled;
        }

        private void ExecuteCommandSaveConfiguration(object obj)
        {
            try
            {
                SaveConfiguration();
                InitializeConfiguration();
                OutputMessage = "Configuration was saved.";
                OutputMessageIsError = false;
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }
        }

        private bool CanExecuteCommandRemoveItem(D2RItem item)
        {
            return true;
        }

        private void ExecuteCommandRemoveItem(D2RItem item)
        {
            try
            {
                if (item == null)
                    return;

                ConfigurationItems.Remove(item);

                OutputMessage = String.Empty;
                OutputMessageIsError = false;
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }
        }

        private bool CanExecuteCommandAddItem(object obj)
        {
            return EditingConfigurationIsEnabled;
        }

        private void ExecuteCommandAddItem(object obj)
        {
            try
            {
                D2RItem newItem = new D2RItem();
                ConfigurationItems.Add(newItem);
                SelectedConfigurationItem = newItem;

                OutputMessage = String.Empty;
                OutputMessageIsError = false;
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }
        }

        private bool CanExecuteCommandEditItems(object obj)
        {
            return !EditingConfigurationIsEnabled;
        }

        private void ExecuteCommandEditItems(object obj)
        {
            EditingConfigurationIsEnabled = true;
        }

        private bool CanExecuteCommandStartNextD2RAndCopyAccountToClipboard(string account)
        {
            return !EditingConfigurationIsEnabled;
        }

        private void ExecuteCommandStartNextD2RAndCopyAccountToClipboard(string account)
        {            
            try
            {
                string message = StartNextD2R();
                OutputMessage = $"{message}";
                OutputMessageIsError = false;

                if (!string.IsNullOrEmpty(account)) 
                {
                    Clipboard.SetText(account);
                }
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }
        }

        private bool CanExecuteCommandRestoreSavedConfiguration(object obj)
        {
            return EditingConfigurationIsEnabled;
        }

        private void ExecuteCommandRestoreSavedConfiguration(object obj)
        {
            try
            {
                OutputMessage = string.Empty;
                OutputMessageIsError = false;
                InitializeConfiguration();
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        private void InitializeConfiguration()
        {
            ConfigurationItems.Clear();

            try
            {
                App.Settings.Items.ForEach(item => { ConfigurationItems.Add(new D2RItem() { D2RPath = item.D2RPath, Account = item.Account, D2RStatus = D2RStatus.Stopped, }); });
                while(ConfigurationItems.Count < 2)
                {
                    ConfigurationItems.Add(new D2RItem());
                }
            }
            catch (Exception ex)
            {
                OutputMessage = $"{ex?.Message}";
                OutputMessageIsError = true;
            }

            if (ConfigurationItems.Count < 2)
                while (ConfigurationItems.Count < 2)
                    ConfigurationItems.Add(new D2RItem());


            if (ConfigurationItems.Where(item => string.IsNullOrEmpty(item.D2RPath)).Count() > 0)
            {
                EditingConfigurationIsEnabled = true;
            }
            else
            {
                EditingConfigurationIsEnabled = false;
            }            
        }

        private void SaveConfiguration()
        {
            App.Settings.Items.Clear();

            foreach (D2RItem item in ConfigurationItems)
            {
                App.Settings.Items.Add(new D2RItem() { D2RPath = item.D2RPath, Account = item.Account });
            }

            App.Settings.Save();
        }

        private string StartNextD2R()
        {
            string nextInstallationPathToExecute = ConfigurationItems.Where(item => !string.IsNullOrEmpty(item.D2RPath) && !D2RProcessManager.D2ROfPathIsRunning(item.D2RPath)).Select(item => item.D2RPath).FirstOrDefault();

            if (string.IsNullOrEmpty(nextInstallationPathToExecute))
                throw new InvalidOperationException($"No more unstarted D2R installation was found.");

            D2RProcessManager.RemoveHandle();
            D2RProcessManager.StartD2RLauncherByPath(nextInstallationPathToExecute);

            return $"Started D2R launcher of path '{nextInstallationPathToExecute}'.";
        }

        private void RaiseCanExecuteChangedOfCommands()
        {
            CommandEditItems.RaiseCanExecuteChanged();
            CommandAddItem.RaiseCanExecuteChanged();
            CommandRemoveItem.RaiseCanExecuteChanged();
            CommandSaveConfiguration.RaiseCanExecuteChanged();
            CommandRestoreSavedConfiguration.RaiseCanExecuteChanged();
            CommandStartNextD2RAndCopyAccountToClipboard.RaiseCanExecuteChanged();
        }
        #endregion

        #region Event methods
        private void ConfigurationItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(ConfigurationHasMoreThanTwoItems));
        }
        #endregion
    }
}
