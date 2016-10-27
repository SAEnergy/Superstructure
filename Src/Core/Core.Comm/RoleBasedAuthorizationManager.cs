using Core.Interfaces.Components;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public class RoleBasedAuthorizationManager : ServiceAuthorizationManager
    {
        #region Public Methods

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            bool retVal = false;

            var bob = ServiceSecurityContext.Current.PrimaryIdentity.Name;

            string action = operationContext.IncomingMessageHeaders.Action;
            DispatchOperation operation = operationContext.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o => o.Action == action);

            if (operation != null)
            {
                Type hostType = operationContext.Host.Description.ServiceType;
                MethodInfo method = hostType.GetMethod(operation.Name);

                retVal = IoCContainer.Instance.Resolve<IPermissionsManager>().IsAuthorized(ServiceSecurityContext.Current.PrimaryIdentity, hostType, method);
            }
            else
            {
                throw new NullReferenceException(string.Format("Operation for action \"{0}\" not found!", action));
            }

            return retVal;
        }

        #endregion
    }
}
