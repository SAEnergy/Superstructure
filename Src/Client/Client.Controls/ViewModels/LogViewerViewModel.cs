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
    public class LogViewerViewModel : ViewModelBase<IRemoteLogViewer>, IRemoteLogViewerCallback
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

        public int MessageCount { get; set; }

        public ObservableCollection<LogMessage> LogMessages { get; private set; }

        //public SimpleCommand TogglePause { get; set; }



        public LogViewerViewModel(ViewBase parent) : base(parent)
        {
            LogMessages = new ObservableCollection<LogMessage>();
            MaxMessages = 5000;

            LogMessages.Add(new LogMessage() { Severity = LogMessageSeverity.Information, Message = "Connecting to server..." });
        }

        protected override void OnConnect(ISubscription source)
        {
            try
            {
                this.BeginInvoke(() =>
                {
                    MessageCount++;
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
                MessageCount++;
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
                    while (LogMessages.Count > MaxMessages - 1)
                    {
                        LogMessages.RemoveAt(0);
                    }

                    LogMessages.Add(message);

                    MessageCount++;
                }
            });
        }
    }
}
