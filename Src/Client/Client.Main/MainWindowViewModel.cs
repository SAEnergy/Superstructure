using Client.Base;
using Client.Resources;
using Core.Util;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Client.Main
{
    public class PluginInfo
    {
        public Type PluginType { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<PluginInfo> Plugins { get; set; }

        public static readonly DependencyProperty PanelProperty = DependencyProperty.Register("Panel", typeof(PanelBase), typeof(MainWindowViewModel));
        public PanelBase Panel
        {
            get { return (PanelBase)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        public static readonly DependencyProperty SelectedPanelProperty = DependencyProperty.Register("SelectedPanel", typeof(PluginInfo), typeof(MainWindowViewModel), new PropertyMetadata(OnSelectedPanelChanged));
        public PluginInfo SelectedPanel
        {
            get { return (PluginInfo)GetValue(SelectedPanelProperty); }
            set { SetValue(SelectedPanelProperty, value); }
        }

        public MainWindowViewModel(ViewBase parent) : base(parent)
        {
            Plugins = new ObservableCollection<PluginInfo>();
            Task.Run(() => PluginInit());
        }

        private void PluginInit()
        {
            var types = TypeLocator.FindTypes("*plugin*.dll", typeof(PanelBase));
            foreach (Type type in types)
            {
                PluginInfo info = new PluginInfo();
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

        public override void Dispose()
        {
            if (Panel != null) { Panel.Dispose(); }
            base.Dispose();
        }
    }
}
