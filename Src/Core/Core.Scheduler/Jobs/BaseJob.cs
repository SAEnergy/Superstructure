using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Scheduler.Jobs
{
    public abstract class BaseJob : IJob
    {
        #region Fields

        private TimeSpan _cancelWaitCycle = TimeSpan.FromMilliseconds(250);

        protected readonly ILogger _logger;
        protected DateTime _lastRunTime;
        protected TimeSpan _lastRunDuration;

        private List<Task> _taskList;

        private CancellationTokenSource cancelSource;

        #endregion

        #region Properties

        public JobConfiguration Configuration { get; protected set; }

        public JobStatus Status { get; protected set; }

        public bool IsExecuting
        {
            get
            {
                lock(_taskList)
                {
                    return _taskList.Count > 0;
                }
            }
        }

        #endregion

        #region Constructor

        protected BaseJob(ILogger logger, JobConfiguration config)
        {
            _logger = logger;
            Configuration = config;
            _taskList = new List<Task>();
            cancelSource = new CancellationTokenSource();

            _logger.Log(string.Format("Job name \"{0}\" created of type \"{1}\".", config.Name, GetType()));
        }

        #endregion

        #region Public Methods

        public bool TryCancel()
        {
            bool rc = false;

            if(cancelSource != null)
            {
                if (IsExecuting)
                {
                    _logger.Log(string.Format("Job name \"{0}\" canceling all executing tasks.  ", Configuration.Name), LogMessageSeverity.Warning);

                    cancelSource.Cancel();

                    while(IsExecuting)
                    {
                        _logger.Log(string.Format("Job name \"{0}\" waiting on tasks to cancel...", Configuration.Name), LogMessageSeverity.Warning);
                        Thread.Sleep(_cancelWaitCycle);
                    }

                    _logger.Log(string.Format("Job name \"{0}\" all tasks have been canceled.", Configuration.Name));
                }
            }

            return rc;
        }

        public bool TryPause()
        {
            throw new NotImplementedException();
        }

        public bool TryRun()
        {
            bool retVal = false;

            if(Configuration.RunState == JobRunState.Automatic)
            {
                if(CheckSchedule())
                {
                    TryExecute(cancelSource.Token);
                }
            }

            return retVal;
        }

        public bool ForceRun()
        {
            bool retVal = false;

            if (Configuration.RunState != JobRunState.Disabled)
            {
                TryExecute(cancelSource.Token);
            }
            else
            {
                _logger.Log(string.Format("Cannot force run disabled job by the name of \"{0}\".", Configuration.Name), LogMessageSeverity.Warning);
            }

            return retVal;
        }

        public void UpdateConfiguration(JobConfiguration newConfig)
        {
            throw new NotImplementedException();
        }

        public abstract bool Execute(CancellationToken ct);

        #endregion

        #region

        private bool CheckSchedule()
        {
            bool retVal = false;

            switch(Configuration.TriggerType)
            {
                case JobTriggerType.Continuously:
                    retVal = true;
                    break;

                case JobTriggerType.Daily:

                    break;

                case JobTriggerType.Weekly:

                    break;

                case JobTriggerType.Monthly:

                    break;

                default:
                    _logger.Log(string.Format("Job by the name of \"{0}\" has a trigger type of \"{1}\" that is not supported.", Configuration.Name, Configuration.TriggerType), LogMessageSeverity.Warning);
                    break;
            }

            return retVal;
        }

        private async void TryExecute(CancellationToken ct)
        {
            if(!IsExecuting || Configuration.SimultaneousExecutions)
            {
                var task = new Task<bool>(() => Execute(ct), ct);

                _logger.Log(string.Format("Starting job \"{0}\".", Configuration.Name));

                lock (_taskList)
                {
                    _taskList.Add(task);

                    if (_taskList.Count > 1)
                    {
                        _logger.Log(string.Format("Running {0} jobs simultaneously by the name of \"{1}\"", _taskList.Count, Configuration.Name), LogMessageSeverity.Warning);
                    }
                }

                var watch = new Stopwatch();

                watch.Start();

                bool rc = false;

                try
                {
                    task.Start();

                    rc = await task;
                }
                catch(OperationCanceledException)
                {
                    _logger.Log(string.Format("Job \"{0}\" canceled.", Configuration.Name), LogMessageSeverity.Warning);
                }
                catch(Exception ex)
                {
                    _logger.Log(string.Format("Job \"{0}\" failed! Error - {1}.", Configuration.Name, ex.Message),LogMessageSeverity.Error);
                }

                watch.Stop();

                lock (_taskList)
                {
                    _taskList.Remove(task);
                }

                string message = rc ? "completed successfully" : "failed to complete successfully";

                _logger.Log(string.Format("Job \"{0}\" {1}, run time = {2:hh\\:mm\\:ss}", Configuration.Name, message, watch.Elapsed), rc ? LogMessageSeverity.Information : LogMessageSeverity.Error);
            }
            else
            {
                _logger.Log(string.Format("Job \"{0}\" attempting to execute, but is already running.", Configuration.Name), LogMessageSeverity.Warning);
            }
        }

        #endregion
    }
}
