using Client.Base;

namespace Example.ScrumPoker.Plugin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [PanelMetadata(DisplayName = "Scrum Poker", IconPath = "images/infinity.png")]
    public partial class ScrumPokerPanel : PanelBase
    {
        public ScrumPokerPanel()
        {
            ViewModel = new ScrumPokerViewModel(this);
            InitializeComponent();
        }
    }
}
