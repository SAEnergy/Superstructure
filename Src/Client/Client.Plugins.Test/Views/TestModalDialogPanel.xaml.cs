using Client.Base;
using System.Windows;

namespace Client.Plugins.Test
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TestModalDialogs : PanelBase
    {
        public TestModalDialogs()
        {
            ViewModel = new TestModalDialogViewModel(this);
            InitializeComponent();
        }

        private void ClickModalBackgroundTask(object sender, RoutedEventArgs e)
        {
            (ViewModel as TestModalDialogViewModel).ClickModalBackgroundTask(sender, e);
        }

        private void ClickCancellableBackgroundTask(object sender, RoutedEventArgs e)
        {
            (ViewModel as TestModalDialogViewModel).ClickCancellableBackgroundTask(sender, e);
        }
    }
}
