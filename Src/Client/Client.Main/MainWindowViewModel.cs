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

    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<PluginInfoModel> Plugins { get; set; }
        public SimpleCommand SettingsCommand { get; set; }
        private SettingsDialog _settings;

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

        public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(OnServerNameChanged));
        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        public MainWindowViewModel(ViewBase parent) : base(parent)
        {
            Plugins = new ObservableCollection<PluginInfoModel>();
            ServerName = ServerConnectionInformation.Instance.ConnectionString;
            SettingsCommand = new SimpleCommand(ExecuteSettingsCommand);
            Task.Run(() => PluginInit());
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

        private static void OnServerNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ServerConnectionInformation.Instance.ConnectionString = ((MainWindowViewModel)d).ServerName;
            ServerConnectionInformation.Instance.FireReconnect();
        }

        private void ExecuteSettingsCommand()
        {
            if (_settings == null)
            {
                _settings = new SettingsDialog(Window.GetWindow(_parent));
                _settings.Closed += SettingsClosed;
                _settings.Show();
            }
        }

        private void SettingsClosed(object sender, EventArgs e)
        {
            _settings = null;
        }

        public override void Dispose()
        {
            if (Panel != null) { Panel.Dispose(); }
            base.Dispose();
        }
    }
}
