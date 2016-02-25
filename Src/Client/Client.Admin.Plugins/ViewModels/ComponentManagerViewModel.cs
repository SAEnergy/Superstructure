using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.ObjectModel;

namespace Client.Admin.Plugins
{
    public class ComponentManagerViewModel : ViewModelBase<IComponentManager>, IComponentManagerCallback
    {
        public ObservableCollection<string> Messages { get; private set; }

        public ComponentManagerViewModel(ViewBase parent) : base(parent)
        {
            Messages = new ObservableCollection<string>();
        }

        public void MooBack(string moo)
        {
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Disconnected. " + error.Message));
            base.OnDisconnect(source, error);
        }

        protected override void OnConnect(ISubscription source)
        {
            this.BeginInvokeIfRequired(() => Messages.Add("Connected to Server."));
            try
            {
                var infos = Channel.GetComponents();

                foreach (var info in infos)
                {
                    this.BeginInvokeIfRequired(() => Messages.Add("Component - " + info.TypeName));
                }
            }
            catch (Exception ex)
            {
                this.BeginInvokeIfRequired(() => Messages.Add("Error: " + ex.Message));
            }
        }

    }
}
