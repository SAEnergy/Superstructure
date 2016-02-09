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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TestPlugin : PanelBase, IDuplexTestCallback
    {
        private WaitDialog _dialog;
        private CancellationTokenSource _cancel;
        private Subscription<IDuplexTest> _conn;
        public ObservableCollection<string> Messages { get; private set; }

        public TestPlugin()
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

        private async Task Worker(CancellationToken tok)
        {
            await Task.Run(() =>
            {
                int x;
                int count = 100;

                this.BeginInvokeIfRequired(() => { if (_dialog != null) { _dialog.MaxValue = count; } });

                for (x = 0; x < count; x++)
                {
                    tok.ThrowIfCancellationRequested();
                    this.BeginInvokeIfRequired(() => { if (_dialog != null) { _dialog.CurrentValue = x; } });
                    Thread.Sleep(100);
                }
                this.BeginInvokeIfRequired(() => { if (_dialog != null) { _dialog.IsCancellable = true; _dialog.Close(); } });
            });
        }

        private void ClickModalBackgroundTask(object sender, RoutedEventArgs e)
        {
            _dialog = new WaitDialog(Window.GetWindow(this));
            _dialog.IsCancellable = false;
            var task = Worker(new CancellationToken());
            _dialog.ShowDialog();
            _dialog = null;
        }

        private void ClickCancellableBackgroundTask(object sender, RoutedEventArgs e)
        {
            _dialog = new WaitDialog(Window.GetWindow(this));
            _cancel = new CancellationTokenSource();
            _dialog.Closed += _dialog_Closed;
            var task = Worker(_cancel.Token);
            _dialog.ShowDialog();
            _dialog = null;
        }

        private void _dialog_Closed(object sender, EventArgs e)
        {
            _cancel.Cancel();
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
