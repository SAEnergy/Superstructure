using Client.Base;
using Core.Comm;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Controls
{
    public class LogViewModel : ViewModelBase<IRemoteLogViewer>, IRemoteLogViewerCallback
    {
        public bool ViewDetail { get; set; }

        public Visibility IsVisible
        {
            get
            {
                return ViewDetail ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public int MaxMessages { get; set; }

        //public SimpleCommand TogglePause { get; set; }

        public ObservableCollection<LogMessage> LogMessages { get; private set; }

        public LogViewModel(ViewBase parent) : base(parent)
        {
            LogMessages = new ObservableCollection<LogMessage>();

            LogMessages.Add(new LogMessage() { Severity = LogMessageSeverity.Information, Message = "Connecting to server..." });
        }

        protected override void OnConnect(ISubscription source)
        {
            try
            {
                this.BeginInvoke(() =>
                {
                    LogMessages.Add(new LogMessage() { Severity = LogMessageSeverity.Information, Message = "Connected." });
                });

                Channel.Register();
            }
            catch (Exception)
            {
            }

            base.OnConnect(source);
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvoke(() =>
            {
                LogMessages.Add(new LogMessage() { Severity = LogMessageSeverity.Information, Message = "Connection terminated." });
            });

            base.OnDisconnect(source, error);
        }

        public override void Dispose()
        {
            try
            {
                Channel.Unregister();
            }
            catch (Exception)
            {
            }

            base.Dispose();
        }

        public void ReportMessages(LogMessage[] messages)
        {
            this.BeginInvoke(() =>
            {
                foreach(var message in messages)
                {
                    LogMessages.Add(message);
                }
            });
        }
    }
}
