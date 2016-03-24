using Core.Interfaces.Components;
using Core.Interfaces.Components.Base;
using Core.Interfaces.Components.IoC;
using Core.Interfaces.Components.Logging;
using Core.Models;
using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Server.Components
{
    [ComponentRegistration(ComponentType.Server, typeof(IPermissionsManager))]
    [ComponentMetadata(Description = "Controls permissions throught the system.", FriendlyName = "Permissions Manager")]
    public sealed class PermissionsManager : Singleton<IPermissionsManager>, IPermissionsManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDataComponent _dataComponent;
        private readonly ISystemConfiguration _systemConfig;

        #endregion

        #region Constructor

        private PermissionsManager(ILogger logger, IDataComponent dataComponent, ISystemConfiguration systemConfig)
        {
            _logger = logger;
            _dataComponent = dataComponent;
            _systemConfig = systemConfig;
        }

        #endregion

        #region Public Methods

        public static IPermissionsManager CreateInstance(ILogger logger, IDataComponent dataComponent, ISystemConfiguration systemConfig)
        {
            return Instance = new PermissionsManager(logger, dataComponent, systemConfig);
        }

        public bool IsAuthorized(IIdentity identity, Type serviceType, MethodInfo operationMethodInfo)
        {
            bool retVal = false;

            if(identity != null && serviceType != null && operationMethodInfo != null)
            {

                retVal = true;
            }
            else
            {
                throw new ArgumentNullException();
            }

            return retVal;
        }

        public void ClearPermissionsCache()
        {
        }

        #endregion
    }
}
