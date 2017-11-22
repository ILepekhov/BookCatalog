using Proxy.BookSourceServer.Engine;
using Shared.Binding;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Proxy.BookSourceServer.GUI.ViewModel
{
    public sealed class ProxyViewModel : BaseBinding
    {
        #region Fields

        private WcfServer _wcfServer;

        #endregion

        #region Properties

        public string Address { get; set; }
        public uint Port { get; set; }

        public ObservableCollection<string> LogItems { get; }

        #endregion

        #region Commands

        public ICommand StartServiceCmd { get; }
        public ICommand StopServiceCmd { get; }

        #endregion

        #region .ctor

        public ProxyViewModel()
        {
            LogItems = new ObservableCollection<string>();
            _wcfServer = new WcfServer();
            _wcfServer.EventOccured += (s, e) => AddNewLogItem(e);

            Address = "localhost";
            Port = 1220;

            StartServiceCmd = new DelegateCommand(ExecStartServiceCmd, CanExecStartServiceCmd);
            StopServiceCmd = new DelegateCommand(ExecStopServiceCmd, CanExecStopServiceCmd);
        }

        #endregion

        #region Helpers

        private void AddNewLogItem(string text)
        {
            App.Current.Dispatcher.Invoke(() => LogItems.Add($"{DateTime.Now:dd.MM.yyyy HH.mm.ss} | {text}"));
        }

        #endregion

        #region StartServiceCmd

        private bool CanExecStartServiceCmd(object parameter)
        {
            return _wcfServer != null && !_wcfServer.Started;
        }

        private void ExecStartServiceCmd(object parameter)
        {
            _wcfServer.StartServer(Address, Port);
        }

        #endregion

        #region StopServiceCmd

        private bool CanExecStopServiceCmd(object parameter)
        {
            return _wcfServer != null && _wcfServer.Started;
        }

        private void ExecStopServiceCmd(object parameter)
        {
            _wcfServer.StopServer();
        }

        #endregion
    }
}
