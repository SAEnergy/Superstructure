using Client.Base;
using Core.Util;
using System.Collections.ObjectModel;

namespace Client.Controls
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<ClientSettingsBase> Instances { get; set; }

        public SettingsViewModel(ViewBase parent) : base(parent)
        {
            Instances = new ObservableCollection<ClientSettingsBase>();
            foreach(ClientSettingsBase c in ClientSettingsEngine.Instance.GetInstances())
            {
                ClientSettingsMetadataAttribute atty = c.GetType().GetAttribute<ClientSettingsMetadataAttribute>();
                if (atty!=null && atty.Hidden) { continue; }
                Instances.Add(c);
            }
        }
    }
}
