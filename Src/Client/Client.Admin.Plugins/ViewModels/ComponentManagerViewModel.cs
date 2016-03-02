using Client.Base;
using Client.Resources;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using Core.Models.DataContracts;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;

namespace Client.Admin.Plugins
{
    public class ComponentManagerViewModel : ViewModelBase<IComponentService>, IComponentServiceCallback
    {
        public ObservableCollection<ComponentMetadataModel> Components { get; private set; }

        public ComponentManagerViewModel(ViewBase parent) : base(parent)
        {
            Components = new ObservableCollection<ComponentMetadataModel>();
        }

        protected override void OnConnect(ISubscription source)
        {
            try
            {
                var infos = Channel.GetComponents();


                var startIcon = WPFHelpers.GetImage("images/media-play.png");
                var stopIcon = WPFHelpers.GetImage("images/media-stop.png");
                var restartIcon = WPFHelpers.GetImage("images/reload.png");
                var disableIcon = WPFHelpers.GetImage("images/ban.png");

                foreach (var info in infos)
                {
                    this.BeginInvoke(() =>
                    {
                        ComponentMetadataModel model = new ComponentMetadataModel();
                        model.UpdateFrom(info);
                        model.StopCommand.ParameterizedExecuteCallback += ExecuteStopCommand;
                        model.StartCommand.ParameterizedExecuteCallback += ExecuteStartCommand;
                        model.RestartCommand.ParameterizedExecuteCallback += ExecuteRestartCommand;
                        model.DisableCommand.ParameterizedExecuteCallback += ExecuteDisableCommand;
                        model.StartIcon = startIcon;
                        model.StopIcon = stopIcon;
                        model.RestartIcon = restartIcon;
                        model.DisableIcon = disableIcon;
                        Components.Add(model);
                    });
                }
            }
            catch
            {
                //nom nom nom
            }

            base.OnConnect(source);
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvoke(() =>
            {
                Components.Clear();
            });

            base.OnDisconnect(source, error);
        }

        public void ComponentUpdated(ComponentMetadata component)
        {
            this.BeginInvoke(() =>
            {
                var oldComponent = Components.Where(c => c.OriginalObject.ComponentId == component.ComponentId).FirstOrDefault();

                if(oldComponent != null)
                {
                    oldComponent.UpdateFrom(component);
                }
                else
                {
                    ComponentMetadataModel model = new ComponentMetadataModel();
                    model.UpdateFrom(component);
                    model.StopCommand.ParameterizedExecuteCallback += ExecuteStopCommand;
                    model.StartCommand.ParameterizedExecuteCallback += ExecuteStartCommand;
                    model.RestartCommand.ParameterizedExecuteCallback += ExecuteRestartCommand;
                    model.DisableCommand.ParameterizedExecuteCallback += ExecuteDisableCommand;
                    Components.Add(model);
                }
            });
        }

        #region Private Methods

        private void ExecuteStopCommand(object component)
        {
            var realComponent = component as ComponentMetadataModel;

            if (realComponent != null)
            {
                Execute(() => Channel.Stop(realComponent.OriginalObject.ComponentId));
            }
        }

        private void ExecuteStartCommand(object component)
        {
            var realComponent = component as ComponentMetadataModel;

            if (realComponent != null)
            {
                Execute(() => Channel.Start(realComponent.OriginalObject.ComponentId));
            }
        }

        private void ExecuteRestartCommand(object component)
        {
            var realComponent = component as ComponentMetadataModel;

            if (realComponent != null)
            {
                Execute(() => Channel.Restart(realComponent.OriginalObject.ComponentId));
            }
        }

        private void ExecuteDisableCommand(object component)
        {
            var realComponent = component as ComponentMetadataModel;

            if (realComponent != null)
            {
                Execute(() => Channel.Disable(realComponent.OriginalObject.ComponentId));
            }
        }

        #endregion
    }
}
