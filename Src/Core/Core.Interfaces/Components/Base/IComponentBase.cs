using Core.Models;
using System;
using System.Collections.Generic;

namespace Core.Interfaces.Components.Base
{
    public interface IComponentBase
    {
        ComponentType ComponentType { get; }
    }
}
