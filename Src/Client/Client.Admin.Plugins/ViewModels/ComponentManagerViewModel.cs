using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using Core.Models.DataContracts;
using System;
using System.Collections.ObjectModel;

namespace Client.Admin.Plugins
{
    public class ComponentManagerViewModel : ViewModelBase<IComponentService>, IComponentServiceCallback
    {
        public ObservableCollection<ComponentInfo> Components { get; private set; }

        public ComponentManagerViewModel(ViewBase parent) : base(parent)
        {
            Components = new ObservableCollection<ComponentInfo>();
        }

        public void MooBack(string moo)
        {
        }

        protected override void OnConnect(ISubscription source)
        {
            try
            {
                var infos = Channel.GetComponents();

                foreach (var info in infos)
                {
                    this.BeginInvoke(() => Components.Add(info));
                }
            }
            catch
            {
                //nom nom nom
            }
        }
    }
}
