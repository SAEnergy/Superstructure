using Core.Models;
using System;

namespace Core.Interfaces.Components.IoC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentRegistrationAttribute : Attribute
    {
        public ComponentType Type { get; set; }

        public Type InterfaceType { get; set; }

        public bool DoNotRegister { get; set; }

        public ComponentRegistrationAttribute(ComponentType type, Type interfaceType)
        {
            Type = type;
            InterfaceType = interfaceType;
        }
    }

    public class ComponentMetadataAttribute : Attribute
    {
        public string Description { get; set; }

        public string FriendlyName { get; set; }

        public ComponentUserActions AllowedActions { get; set; }

        public ComponentMetadataAttribute()
        {
            AllowedActions = ComponentUserActions.NoActions;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ProxyAttribute : Attribute
    {
        public Type ProxyType { get; set; }

        public ProxyAttribute(Type type)
        {
            ProxyType = type;
        }
    }
}
