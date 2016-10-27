using Client.Base;
using System.Threading;

namespace Client.Plugins.Test
{
    /// <summary>
    /// Interaction logic for TestLogViewPanel.xaml
    /// </summary>
    [PanelMetadata(DisplayName = "Log Viewer Test", IconPath = "images/warning.png")]
    public partial class TestLogViewerViewPanel : PanelBase
    {
        public TestLogViewerViewPanel()
        {
            InitializeComponent();
        }

        public override void Dispose()
        {
            logViewerView.Dispose();

            base.Dispose();
        }
    }
}
