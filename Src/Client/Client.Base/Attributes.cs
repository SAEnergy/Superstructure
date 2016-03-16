using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Base
{
    public class PanelMetadataAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public string IconPath { get; set; }
    }

    public class ClientSettingsMetadataAttribute : Attribute
    {
        /// <summary>
        /// Do not display this settings class in the settings dialog
        /// </summary>
        public bool Hidden { get; set; }
    }

    public class PropertyEditorMetadataAttribute : Attribute
    {
        /// <summary>
        /// Do not display this property in the property editor
        /// </summary>
        public bool Hidden { get; set; }
    }
}
