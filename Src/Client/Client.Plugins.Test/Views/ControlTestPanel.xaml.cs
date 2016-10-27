using Client.Base;
using System.Windows;

namespace Client.Plugins.Test
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ControlTestPanel : PanelBase
    {
        public ControlTestPanel()
        {
            ViewModel = new ControlTestViewModel(this);
            InitializeComponent();
        }
    }
}
