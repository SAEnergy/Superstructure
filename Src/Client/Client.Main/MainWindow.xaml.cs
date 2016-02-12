using Client.Base;
using Client.Resources;
using Core.Util;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Main
{
    public class PluginInfo
    {
        public Type PluginType { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
    }

    public partial class MainWindow : DialogBase
    {
        public ObservableCollection<PluginInfo> Plugins { get; set; }

        public static readonly DependencyProperty PanelProperty = DependencyProperty.Register("Panel", typeof(PanelBase), typeof(MainWindow));
        public PanelBase Panel
        {
            get { return (PanelBase)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        public MainWindow() : base(null)
        {
            Plugins = new ObservableCollection<PluginInfo>();
            this.DataContext = this;
            InitializeComponent();
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
                    info.Icon = WPFHelpers.GetImage(atty.IconPath);
                }
                if (info.Icon == null)
                {
                    info.Icon = WPFHelpers.GetImage("images/puzzle-piece.png");
                }
                this.InvokeIfRequired(() => Plugins.Add(info));
            }
            this.BeginInvokeIfRequired(() => { if (PluginList.SelectedItem == null) { PluginList.SelectedItem = Plugins[0]; } });
        }

        private void Plugin_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Panel != null) { Panel.Dispose(); }

            PluginInfo info = (sender as ListBox).SelectedItem as PluginInfo;

            if (info == null) { return; }

            Panel = Activator.CreateInstance(info.PluginType) as PanelBase;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Panel != null) { Panel.Dispose(); }
            base.OnClosed(e);
        }
    }
}
