using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SchedulerService : ISchedulerService
    {
        #region Fields

        private ILogger _logger;

        #endregion

        #region Constructor

        public SchedulerService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion
    }
}
