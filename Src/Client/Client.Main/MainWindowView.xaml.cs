using Client.Base;

namespace Client.Main
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : ViewBase
    {
        public MainWindowView()
        {
            ViewModel = new MainWindowViewModel(this);
            InitializeComponent();
        }
    }
}
