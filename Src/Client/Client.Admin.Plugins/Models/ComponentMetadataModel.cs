using Client.Base;
using Client.Resources;
using Core.Models.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.Admin.Plugins
{
    public class ComponentMetadataModel : ModelBase<ComponentMetadata>
    {
        public SimpleCommand StartCommand { get; private set; }
        public SimpleCommand StopCommand { get; private set; }
        public SimpleCommand RestartCommand { get; private set; }

        public SimpleCommand DisableCommand { get; private set; }

        public ImageSource StartIcon { get; set; }

        public ImageSource StopIcon { get; set; }

        public ImageSource RestartIcon { get; set; }

        public ImageSource DisableIcon { get; set; }

        public ComponentMetadataModel()
        {
            StartCommand = new SimpleCommand();
            StopCommand = new SimpleCommand();
            RestartCommand = new SimpleCommand();
            DisableCommand = new SimpleCommand();
        }
    }
}
