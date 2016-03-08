using Client.Base;
using System.Threading;

namespace Client.Plugins.Test
{
    /// <summary>
    /// Interaction logic for TestLogViewPanel.xaml
    /// </summary>
    [PanelMetadata(DisplayName = "Log View Test", IconPath = "images/warning.png")]
    public partial class TestLogViewPanel : PanelBase
    {
        public TestLogViewPanel()
        {
            InitializeComponent();
        }

        public override void Dispose()
        {
            logView.Dispose();

            base.Dispose();
        }
    }
}
