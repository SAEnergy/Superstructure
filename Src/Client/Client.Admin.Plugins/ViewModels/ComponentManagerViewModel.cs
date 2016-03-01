using Client.Base;
using Client.Resources;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using Core.Models.DataContracts;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Client.Admin.Plugins
{
    public class ComponentManagerViewModel : ViewModelBase<IComponentService>, IComponentServiceCallback
    {
        public ObservableCollection<ComponentMetadata> Components { get; private set; }

        public SimpleCommand StartComponent { get; private set; }

        public SimpleCommand StopComponent { get; private set; }

        public SimpleCommand RestartComponent { get; private set; }

        public ImageSource StartIcon { get; private set; }

        public ImageSource StopIcon { get; private set; }

        public ImageSource RestartIcon { get; private set; }

        public ComponentManagerViewModel(ViewBase parent) : base(parent)
        {
            Components = new ObservableCollection<ComponentMetadata>();

            StartComponent = new SimpleCommand(new Action<object>(ExecuteStartCommand));
            StopComponent = new SimpleCommand(new Action<object>(ExecuteStopCommand));
            RestartComponent = new SimpleCommand(new Action<object>(ExecuteRestartCommand));

            StartIcon = WPFHelpers.GetImage("images/media-play.png");
            StopIcon = WPFHelpers.GetImage("images/media-stop.png");
            RestartIcon = WPFHelpers.GetImage("images/reload.png");
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

        #region Private Methods

        private void ExecuteStopCommand(object component)
        {
            var realComponent = component as ComponentMetadata;

            if (realComponent != null)
            {
                Execute(() => Channel.Stop(realComponent.ComponentId));
            }
        }

        private void ExecuteStartCommand(object component)
        {
            var realComponent = component as ComponentMetadata;

            if (realComponent != null)
            {
                Execute(() => Channel.Start(realComponent.ComponentId));
            }
        }

        private void ExecuteRestartCommand(object component)
        {
            var realComponent = component as ComponentMetadata;

            if (realComponent != null)
            {
                Execute(() => Channel.Restart(realComponent.ComponentId));
            }
        }

        #endregion
    }
}
