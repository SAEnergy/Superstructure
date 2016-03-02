using Client.Base;

namespace Client.Admin.Plugins
{
    /// <summary>
    /// Interaction logic for ComponentManagerPanel.xaml
    /// </summary>
    [PanelMetadata(DisplayName = "Component Manager", IconPath = "images/cog.png")]
    public partial class ComponentManagerPanel : PanelBase
    {
        public ComponentManagerPanel()
        {
            ViewModel = new ComponentManagerViewModel(this);
            InitializeComponent();
        }
    }
}
