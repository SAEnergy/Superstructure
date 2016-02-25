using Client.Base;
using System.Windows;

namespace Client.Plugins.Test
{
    [PanelMetadata(DisplayName = "Subscription Test", IconPath = "images/globe.png")]
    public partial class TestSubscriptionPanel : PanelBase
    {
        public TestSubscriptionPanel()
        {
            ViewModel = new TestSubscriptionViewModel(this);
            InitializeComponent();
        }

        private void ClickTriggerException(object sender, RoutedEventArgs e)
        {
            (ViewModel as TestSubscriptionViewModel).ClickTriggerException(sender, e);
        }
    }
}
