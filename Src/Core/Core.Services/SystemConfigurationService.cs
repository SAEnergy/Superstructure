using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SystemConfigurationService : Singleton<ISystemConfigurationService>, ISystemConfigurationService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDataService _dataService;

        #endregion

        #region Constructor

        private SystemConfigurationService(ILogger logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        #endregion

        #region Public Methods

        public static ISystemConfigurationService CreateInstance(ILogger logger, IDataService dataService)
        {
            return Instance = new SystemConfigurationService(logger, dataService);
        }

        #endregion
    }
}
