using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
    public sealed class SchedulerService : Singleton<ISchedulerService>, ISchedulerService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDataService _dataService;

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        private SchedulerService(ILogger logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        #endregion

        #region Public Methods

        public static ISchedulerService CreateInstance(ILogger logger, IDataService dataService)
        {
            return Instance = new SchedulerService(logger, dataService);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool AddJob(IJob job)
        {
            throw new NotImplementedException();
        }

        public bool DeleteJob(IJob job)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
