using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components;
using Core.Interfaces.Components.IoC;
using Core.Interfaces.Components.Base;

namespace Core.Components
{
    [ComponentRegistration(ComponentType.Server, typeof(ISystemConfigurationComponent))]
    [ComponentMetadata(Description = "Maintains system wide configurations.", FriendlyName = "System Configuration Component")]
    public class SystemConfigurationComponent : Singleton<ISystemConfigurationComponent>, ISystemConfigurationComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDataComponent _dataComponent;

        #endregion

        #region Constructor

        private SystemConfigurationComponent(ILogger logger, IDataComponent dataComponent)
        {
            _logger = logger;
            _dataComponent = dataComponent;
        }

        #endregion

        #region Public Methods

        public static ISystemConfigurationComponent CreateInstance(ILogger logger, IDataComponent dataComponent)
        {
            return Instance = new SystemConfigurationComponent(logger, dataComponent);
        }

        #endregion
    }
}
