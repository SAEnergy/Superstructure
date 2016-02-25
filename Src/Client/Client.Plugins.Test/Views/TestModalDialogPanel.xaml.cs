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
    }
}
