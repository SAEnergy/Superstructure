using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class HostManagerService : IHostManagerService
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public HostManagerService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public void RestartAll()
        {
            throw new NotImplementedException();
        }

        public void StartAll()
        {
            _logger.Log(LogMessageSeverity.Information, "HostManagerService starting all hosts...");
        }

        public void StopAll()
        {
            _logger.Log(LogMessageSeverity.Information, "HostManagerService stopping all hosts...");
        }

        void IHostManagerService.Restart<T>()
        {
            throw new NotImplementedException();
        }

        void IHostManagerService.Start<T>()
        {
            throw new NotImplementedException();
        }

        void IHostManagerService.Stop<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
