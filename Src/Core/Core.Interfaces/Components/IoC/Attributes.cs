using System;

namespace Core.Interfaces.Components.IoC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public ComponentType Type { get; set; }

        public LifeCycle ComponentLifeCycle { get; set; }

        public Type InterfaceType { get; set; }

        public string Description { get; set; }

        public string FriendlyName { get; set; }

        public ComponentUserActions AllowedActions { get; set; }

        public bool DoNotRegister { get; set; }

        public ComponentAttribute(ComponentType type, Type interfaceType)
        {
            Type = type;
            InterfaceType = interfaceType;
            ComponentLifeCycle = LifeCycle.Singleton;
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
