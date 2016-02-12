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
}
