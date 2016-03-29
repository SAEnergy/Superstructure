using Core.Interfaces.Base;
using Core.Interfaces.Components.Base;
using System;
using System.Reflection;
using System.Security.Principal;

namespace Core.Interfaces.Components
{
    public interface IPermissionsManager : IComponent, IInitializable
    {
        bool IsAuthorized(IIdentity identity, Type serviceType, MethodInfo operationMethodInfo);

        void ClearPermissionsCache();
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredPermissionsAttribute : Attribute
    {
        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool Execute { get; set; }
    }
}
