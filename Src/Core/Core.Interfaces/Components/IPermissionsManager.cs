using Core.Interfaces.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components
{
    public interface IPermissionsManager : IComponent
    {
        bool IsAuthorized(IIdentity identity, Type serviceType, MethodInfo operationMethodInfo);

        void ClearPermissionsCache();
    }
}
