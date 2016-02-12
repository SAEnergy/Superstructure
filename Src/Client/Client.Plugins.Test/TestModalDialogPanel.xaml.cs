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
    public partial class TestModalDialogs : PanelBase
    {
        private WaitDialog _dialog;
        private CancellationTokenSource _cancel;

        public TestModalDialogs()
        {
            this.DataContext = this;
            InitializeComponent();
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
    }
}
