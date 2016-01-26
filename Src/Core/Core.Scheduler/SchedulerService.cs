using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Models.Persistent;
using Core.Models;

namespace Core.Scheduler
{
    public sealed class SchedulerService : Singleton<ISchedulerService>, ISchedulerService
    {
        #region Fields

        private TimeSpan _schedulerCheckSpeed = TimeSpan.FromSeconds(1);

        private readonly ILogger _logger;
        private readonly IDataService _dataService;

        private Thread _schedulerWorker;
        private List<IJob> _jobs;

        private static object _syncObject = new object();

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        private SchedulerService(ILogger logger, IDataService dataService)
        {
            _logger = logger;
            JobFactory.Logger = logger;
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
            lock (_syncObject)
            {
                if (!IsRunning)
                {
                    _logger.Log("Scheduler starting...");

                    _schedulerWorker = new Thread(new ThreadStart(SchedulerWorker));

                    _schedulerWorker.Start();
                }
            }
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                if (IsRunning)
                {
                    _logger.Log("Scheduler stopping.");

                    IsRunning = false;

                    _schedulerWorker.Join();
                }
            }
        }

        public List<JobConfiguration> GetJobs()
        {
            return _dataService.Find<JobConfiguration>(j => !j.AuditInfo.IsArchived);
        }

        public bool AddJob(JobConfiguration job)
        {
            throw new NotImplementedException();
        }

        public bool DeleteJob(JobConfiguration job)
        {
            throw new NotImplementedException();
        }

        public bool UpdateJob(JobConfiguration job)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void SchedulerWorker()
        {
            IsRunning = true;

            _logger.Log("Scheduler running.");

            _jobs = LoadAllJobs();

            while (IsRunning)
            {
                lock(_jobs)
                {
                    foreach(var job in _jobs)
                    {
                        job.TryRun();
                    }
                }

                Thread.Sleep(_schedulerCheckSpeed);
            }

            lock (_jobs)
            {
                foreach (var job in _jobs)
                {
                    job.TryCancel();
                }
            }

            _logger.Log("Scheduler stopped.");
        }

        private List<IJob> LoadAllJobs()
        {
            var jobs = new List<IJob>();

            _logger.Log("Scheduler loading all jobs from storage.");

            foreach(var jobConfig in GetJobs())
            {
                jobs.Add(JobFactory.Create(jobConfig));
            }

            return jobs;
        }

        #endregion
    }
}
