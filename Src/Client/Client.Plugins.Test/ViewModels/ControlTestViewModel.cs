using Client.Base;
using Client.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Plugins.Test
{
    class ControlTestViewModel : ViewModelBase
    {
        private WaitDialog _dialog;
        private CancellationTokenSource _cancel;

        private ObservableCollection<TestData> _data;
        public MultiSelectCollectionView<TestData> Data { get; private set; }

        public SimpleCommand ModalBackgroundTaskCommand { get; private set; }
        public SimpleCommand CancellableBackgroundTaskCommand { get; private set; }

        public ControlTestViewModel(ViewBase parent) : base(parent)
        {
            _data = new ObservableCollection<TestData>();
            Data = new MultiSelectCollectionView<TestData>(_data);
            _data.Add(new TestData());
            _data.Add(new TestData());
            _data.Add(new TestData());

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

    public enum Stuff { Things, Stuff, Gizmos }

    public class TestData
    {
        public Guid ReadOnlyID { get; private set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool IsTrue { get; set; }
        public Stuff Stuff { get; set; }

        public TestData()
        {
            ReadOnlyID = Guid.NewGuid();
            Name = "George";
            Number = 42;
            IsTrue = true;
        }

    }
}
