using Core.Interfaces.Base;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components;
using Core.Interfaces.Components.IoC;

namespace Core.Components
{
    [Component(ComponentType.Server, typeof(ISystemConfigurationComponent), Description = "System Configuration System", FriendlyName = "System Configuration")]
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
