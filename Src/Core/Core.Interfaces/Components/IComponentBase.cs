using Core.Interfaces.Components.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components
{
    public interface IComponentBase
    {
        string FriendName { get; }

        string Description { get; }

        ComponentType ComponentType { get; }

        ComponentUserActions AllowedUserActions { get; }

        List<Type> Proxies { get; }
    }
}
