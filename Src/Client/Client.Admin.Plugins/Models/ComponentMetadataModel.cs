using Client.Base;
using Core.Models.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Admin.Plugins.Models
{
    public class ComponentMetadataModel : ModelBase<ComponentMetadata>
    {
        public SimpleCommand StartCommand { get; private set; }
        public SimpleCommand StopCommand { get; private set; }
        public SimpleCommand RestartCommand { get; private set; }

        public SimpleCommand DisableCommand { get; private set; }

        public ComponentMetadataModel()
        {
            StartCommand = new SimpleCommand();
            StopCommand = new SimpleCommand();
            RestartCommand = new SimpleCommand();
            DisableCommand = new SimpleCommand();
        }
    }
}
