using System;

namespace Core.Models.DataContracts
{
    public class ComponentMetadata
    {
        public int ComponentId { get; set; }

        public string FriendlyName { get; set; }

        public string Description { get; set; }

        public string InterfaceTypeName { get; set; }

        public string ConcreteTypeName { get; set; }

        public ComponentMetadata[] Dependencies { get; set; }

        //other data
    }
}
