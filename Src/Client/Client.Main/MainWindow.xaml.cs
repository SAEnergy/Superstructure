using System.Windows;
using Client.Base;

namespace Client.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DialogBase
    {
        public MainWindow() : base(null)
        {
            InitializeComponent();
        }

        private void OpenTestDialog(object sender, RoutedEventArgs e)
        {
            TestDialog dlg = new TestDialog(this);
            dlg.ShowDialog();
        }
    }
}
