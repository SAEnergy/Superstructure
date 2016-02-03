using Core.Interfaces.Base;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components.Scheduler;
using Core.Interfaces.Components;
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
    public sealed class SchedulerComponent : Singleton<ISchedulerComponent>, ISchedulerComponent
    {
        #region Fields

        private TimeSpan _schedulerCheckSpeed = TimeSpan.FromSeconds(1);

        private readonly ILogger _logger;
        private readonly IDataComponent _dataComponent;

        private Thread _schedulerWorker;
        private List<IJob> _jobs;

        private static object _syncObject = new object();

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        private SchedulerComponent(ILogger logger, IDataComponent dataComponent)
        {
            _logger = logger;
            JobFactory.Logger = logger;
            _dataComponent = dataComponent;
        }

        #endregion

        #region Public Methods

        public static ISchedulerComponent CreateInstance(ILogger logger, IDataComponent dataComponent)
        {
            return Instance = new SchedulerComponent(logger, dataComponent);
        }

        public void Start()
        {
            lock (_syncObject)
            {
                if (!IsRunning)
                {
                    _logger.Log("Scheduler component starting...");

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
                    _logger.Log("Scheduler component stopping.");

                    IsRunning = false;

                    _schedulerWorker.Join();
                }
            }
        }

        public List<JobConfiguration> GetJobs()
        {
            return _dataComponent.Find<JobConfiguration>(j => !j.AuditInfo.IsArchived);
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

            _logger.Log("Scheduler component running.");

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

            _logger.Log("Scheduler component stopped.");
        }

        private List<IJob> LoadAllJobs()
        {
            var jobs = new List<IJob>();

            _logger.Log("Scheduler loading all jobs from storage.");

            var query = GetJobs();

            if (query != null)
            {
                foreach (var jobConfig in query)
                {
                    jobs.Add(JobFactory.Create(jobConfig));
                }
            }
            else
            {
                _logger.Log("Storage returned null as result!", LogMessageSeverity.Warning);
            }

            return jobs;
        }

        #endregion
    }
}
