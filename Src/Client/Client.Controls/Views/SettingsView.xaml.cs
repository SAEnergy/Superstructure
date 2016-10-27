using Client.Base;

namespace Client.Controls
{

    public partial class SettingsView : ViewBase
    {
        public SettingsView()
        {
            ViewModel = new SettingsViewModel(this);
            InitializeComponent();
        }
    }
}
