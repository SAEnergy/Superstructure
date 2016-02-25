using Client.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Plugins.Test
{
    class TestModalDialogViewModel : ViewModelBase
    {
        private WaitDialog _dialog;
        private CancellationTokenSource _cancel;

        public SimpleCommand ModalBackgroundTaskCommand { get; private set; }
        public SimpleCommand CancellableBackgroundTaskCommand { get; private set; }

        public TestModalDialogViewModel(ViewBase parent) : base(parent)
        {
            ModalBackgroundTaskCommand = new SimpleCommand(ExecuteModalBackgroundTask);
            CancellableBackgroundTaskCommand = new SimpleCommand(ExecuteCancellableBackgroundTask);
        }

        private async Task Worker(CancellationToken tok)
        {
            await Task.Run(() =>
            {
                int x;
                int count = 100;

                this.BeginInvoke(() => { if (_dialog != null) { _dialog.MaxValue = count; } });

                for (x = 0; x < count; x++)
                {
                    tok.ThrowIfCancellationRequested();
                    this.BeginInvoke(() => { if (_dialog != null) { _dialog.CurrentValue = x; } });
                    Thread.Sleep(100);
                }
                this.BeginInvoke(() => { if (_dialog != null) { _dialog.IsCancellable = true; _dialog.Close(); } });
            });
        }

        public void ExecuteModalBackgroundTask()
        {
            _dialog = new WaitDialog(Window.GetWindow(_parent));
            _dialog.IsCancellable = false;
            var task = Worker(new CancellationToken());
            _dialog.ShowDialog();
            _dialog = null;
        }

        public void ExecuteCancellableBackgroundTask()
        {
            _dialog = new WaitDialog(Window.GetWindow(_parent));
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
