using System;

namespace Core.Interfaces.Base
{
    public interface IHost : IRunnable
    {
        Type InterfaceType { get; }
    }
}
