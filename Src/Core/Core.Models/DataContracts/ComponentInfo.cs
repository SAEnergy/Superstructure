using System;

namespace Core.Models.DataContracts
{
    public class ComponentInfo
    {
        public string FriendlyName { get; set; }

        public string Description { get; set; }

        public string InterfaceTypeName { get; set; }

        public string ConcreteTypeName { get; set; }
    }
}
