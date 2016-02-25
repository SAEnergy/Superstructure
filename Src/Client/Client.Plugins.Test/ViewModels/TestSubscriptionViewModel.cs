using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Plugins.Test
{
    public class TestSubscriptionViewModel : ViewModelBase<IDuplexTest>, IDuplexTestCallback
    {
        public ObservableCollection<string> Messages { get; private set; }

        public TestSubscriptionViewModel(ViewBase parent) : base(parent)
        {
            Messages = new ObservableCollection<string>();
            Messages.Add("honk");
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Disconnected. " + error.Message));
            base.OnDisconnect(source, error);
        }

        protected override void OnConnect(ISubscription source)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Connected to Server."));
            try
            {
                _conn.Channel.Moo();
            }
            catch (Exception ex)
            {
                this.BeginInvokeIfRequired(() => Messages.Add("Error: " + ex.Message));
            }
        }

        public void MooBack(string moo)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Received pong from server: " + moo));
        }

        public override void Dispose()
        {
            _conn.Stop();
            base.Dispose();
        }

        public async void ClickTriggerException(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() => _conn.Channel.ThrowException());
            }
            catch (Exception ex)
            {
                this.BeginInvokeIfRequired(() => Messages.Add("Error: " + ex.Message));
            }

        }
    }
}
