using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Plugins.Test
{

    [PanelMetadata(DisplayName = "Subscription Test", IconPath = "images/globe.png")]
    public partial class TestSubscriptionPanel : PanelBase, IDuplexTestCallback
    {
        private Subscription<IDuplexTest> _conn;
        public ObservableCollection<string> Messages { get; private set; }

        public TestSubscriptionPanel()
        {
            Messages = new ObservableCollection<string>();
            this.DataContext = this;
            InitializeComponent();
            _conn = new Subscription<IDuplexTest>(this);
            _conn.Connected += _conn_Connected;
            _conn.Disconnected += _conn_Disconnected;
            _conn.Start();
        }

        private void _conn_Disconnected(ISubscription source, Exception ex)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Disconnected. " + ex.Message));
        }

        private void _conn_Connected(object sender, EventArgs e)
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

        private async void ClickTriggerException(object sender, RoutedEventArgs e)
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
