using Client.Base;
using Client.Controls;
using Client.Resources;
using Core.Comm;
using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using Core.Logging;
using Core.Util;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Client.Main
{
    public class PluginInfoModel
    {
        public Type PluginType { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public SimpleCommand DetachCommand { get; private set; }

        public PluginInfoModel()
        {
            DetachCommand = new SimpleCommand(OnDetachCommand);
        }

        private void OnDetachCommand()
        {
            PanelBase panel = Activator.CreateInstance(PluginType) as PanelBase;
            DetachedWindow window = new DetachedWindow(panel, MainWindow.Instance);
            window.Show();
        }
    }

    public class MainWindowSettings : ClientSettingsBase
    {
        public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(MainWindowSettings));
        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        public MainWindowSettings()
        {
            ServerName = "localhost";
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<PluginInfoModel> Plugins { get; set; }
        public SimpleCommand SettingsCommand { get; set; }
        private SettingsDialog _settingsDialog;
        private MainWindowSettings _settings;
        private PropertyChangeNotifier _notifyServer;

        public static readonly DependencyProperty PanelProperty = DependencyProperty.Register("Panel", typeof(PanelBase), typeof(MainWindowViewModel));
        public PanelBase Panel
        {
            get { return (PanelBase)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        public static readonly DependencyProperty SelectedPanelProperty = DependencyProperty.Register("SelectedPanel", typeof(PluginInfoModel), typeof(MainWindowViewModel), new PropertyMetadata(OnSelectedPanelChanged));
        public PluginInfoModel SelectedPanel
        {
            get { return (PluginInfoModel)GetValue(SelectedPanelProperty); }
            set { SetValue(SelectedPanelProperty, value); }
        }

        public MainWindowViewModel(ViewBase parent) : base(parent)
        {
            _settings = ClientSettingsEngine.Instance.GetInstance<MainWindowSettings>();
            _notifyServer = new PropertyChangeNotifier(_settings, MainWindowSettings.ServerNameProperty);
            _notifyServer.ValueChanged += ServerValueChanged;
            Plugins = new ObservableCollection<PluginInfoModel>();
            SettingsCommand = new SimpleCommand(ExecuteSettingsCommand);
            Task.Run(() => PluginInit());
        }

        private void ServerValueChanged(object sender, EventArgs e)
        {
            ServerConnectionInformation.Instance.ConnectionString = _settings.ServerName;
            ServerConnectionInformation.Instance.FireReconnect();
        }

        private void PluginInit()
        {
            var types = TypeLocator.FindTypes("*plugin*.dll", typeof(PanelBase));
            foreach (Type type in types)
            {
                PluginInfoModel info = new PluginInfoModel();
                PanelMetadataAttribute atty = type.GetAttribute<PanelMetadataAttribute>();
                info.PluginType = type;
                if (atty != null && !string.IsNullOrWhiteSpace(atty.DisplayName))
                {
                    info.Name = atty.DisplayName;
                }
                else
                {
                    info.Name = PascalCaseSplitter.Split(type.Name);
                }
                if (atty != null && !string.IsNullOrWhiteSpace(atty.IconPath))
                {
                    info.Icon = WPFHelpers.GetImage(atty.IconPath, info.PluginType.Assembly);
                }
                if (info.Icon == null)
                {
                    info.Icon = WPFHelpers.GetImage("images/puzzle-piece.png");
                }
                this.Invoke(() => Plugins.Add(info));
            }
            this.BeginInvoke(() => { if (SelectedPanel == null) { SelectedPanel = Plugins[0]; } });
        }

        private static void OnSelectedPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel)d;
            if (vm.Panel != null) { vm.Panel.Dispose(); }

            if (vm.SelectedPanel == null) { return; }

            vm.Panel = Activator.CreateInstance(vm.SelectedPanel.PluginType) as PanelBase;
        }

        private void ExecuteSettingsCommand()
        {
            if (_settingsDialog == null)
            {
                _settingsDialog = new SettingsDialog(Window.GetWindow(_parent));
                _settingsDialog.Closed += SettingsClosed;
                _settingsDialog.Show();
            }
        }

        private void SettingsClosed(object sender, EventArgs e)
        {
            _settingsDialog = null;
        }

        public override void Dispose()
        {
            if (Panel != null) { Panel.Dispose(); }
            _notifyServer.Dispose();
            _settings = null;
            base.Dispose();
        }
    }
}
