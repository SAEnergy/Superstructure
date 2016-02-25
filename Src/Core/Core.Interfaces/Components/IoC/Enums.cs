using System;

namespace Core.Interfaces.Components.IoC
{
    public enum LifeCycle
    {
        Singleton,
        Transient
    }

    [Flags]
    public enum ComponentType
    {
        NotConfigured = 0,
        Server = 1,
        Client = 2,
        All = 3
    }

    [Flags]
    public enum ComponentUserActions
    {
        NoActions = 0,
        Stop = 1,
        Start = 2,
        Restart = 4,
        Disable = 8,
        Manual = 16,
        All = 31
    }
}
