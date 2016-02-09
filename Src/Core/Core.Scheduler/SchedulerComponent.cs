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

        private TimeSpan _schedulerShutDownTimeOut = TimeSpan.FromMinutes(5);

        private readonly ILogger _logger;
        private readonly IDataComponent _dataComponent;

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

                    Task.Factory.StartNew(() => LoadAllJobs()); //does not block here on purpose
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

                    var task = Task.Factory.StartNew(() => CancelAllJobs());

                    task.Wait(_schedulerShutDownTimeOut); //wait for a while, and give up if it doesn't complete

                    IsRunning = false;

                    _logger.Log("Scheduler component stopped.");
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

        private void CancelAllJobs()
        {
            if(_jobs != null)
            {
                lock(_jobs)
                {
                    if (_jobs.Count > 0)
                    {
                        _logger.Log("Scheduler component attempting to cancel jobs.");

                        foreach (var job in _jobs)
                        {
                            job.TryCancel();
                        }

                        _logger.Log("Scheduler component has canceled all jobs.");
                    }
                }
            }
        }

        private void LoadAllJobs()
        {
            _jobs = new List<IJob>();

            _logger.Log("Scheduler loading all jobs from storage.");

            var query = GetJobs();

            if (query != null)
            {
                IsRunning = true;

                lock(_jobs)
                {
                    foreach (var jobConfig in query)
                    {
                        _jobs.Add(JobFactory.Create(jobConfig));
                    }
                }
            }
            else
            {
                _logger.Log("Storage returned null as result!", LogMessageSeverity.Warning);
            }
        }

        #endregion
    }
}
