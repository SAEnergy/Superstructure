using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Client.Plugins.Test
{
    public class TestSubscriptionViewModel : ViewModelBase<IDuplexTest>, IDuplexTestCallback
    {
        public ObservableCollection<string> Messages { get; private set; }
        public SimpleCommand TriggerExceptionCommand { get; private set; }

        public TestSubscriptionViewModel(ViewBase parent) : base(parent)
        {
            Messages = new ObservableCollection<string>();
            TriggerExceptionCommand = new SimpleCommand(ExecuteTriggerExceptionCommand, CanExecuteTriggerExceptionCommand);
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvoke(() => Messages.Add("Disconnected. " + error.Message));
            base.OnDisconnect(source, error);
        }

        protected override void OnConnect(ISubscription source)
        {
            this.BeginInvoke(() => Messages.Add("Connected to Server."));
            try
            {
                Channel.Moo();
            }
            catch (Exception ex)
            {
                this.BeginInvoke(() => Messages.Add("Error: " + ex.Message));
            }
            base.OnConnect(source);
        }

        public void MooBack(string moo)
        {
            this.BeginInvoke(() => Messages.Add("Received pong from server: " + moo));
        }

        public async void ExecuteTriggerExceptionCommand()
        {
            try
            {
                await Task.Run(() => Channel.ThrowException());
            }
            catch (Exception ex)
            {
                this.BeginInvoke(() => Messages.Add("Error: " + ex.Message));
            }
        }
        private bool CanExecuteTriggerExceptionCommand()
        {
            return _sub.State == SubscriptionState.Connected;
        }
    }
}
