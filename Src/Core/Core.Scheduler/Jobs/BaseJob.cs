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

        protected readonly ILogger _logger;
        protected DateTime _lastRunTime;
        protected TimeSpan _lastRunDuration;

        private List<Thread> _workerThreads;

        #endregion

        #region Properties

        public JobConfiguration Configuration { get; protected set; }

        public JobStatus Status { get; protected set; }

        public bool IsExecuting { get; protected set; }

        #endregion

        #region Constructor

        protected BaseJob(ILogger logger, JobConfiguration config)
        {
            _logger = logger;
            Configuration = config;

            _logger.Log(string.Format("Job name \"{0}\" created of type \"{1}\".", config.Name, GetType()));
        }

        #endregion

        #region Public Methods

        public bool TryCancel()
        {
            throw new NotImplementedException();
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
                    TryExecute();
                }
            }

            return retVal;
        }

        public bool ForceRun()
        {
            bool retVal = false;

            if (Configuration.RunState != JobRunState.Disabled)
            {
                TryExecute();
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

        public abstract void Execute();

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

        private async void TryExecute()
        {
            if(!IsExecuting || Configuration.SimultaneousExecutions)
            {
                _logger.Log(string.Format("Starting job \"{0}\".", Configuration.Name));

                IsExecuting = true; //is executing only works with non-simulatenous executions

                var watch = new Stopwatch();

                watch.Start();

                await Task.Run(() => Execute());

                watch.Stop();

                _logger.Log(string.Format("Job \"{0}\" completed, run time = {1:hh\\:mm\\:ss}", Configuration.Name, watch.Elapsed));

                IsExecuting = false;
            }
            else
            {
                _logger.Log(string.Format("Job \"{0}\" attempting to execute, but is already running.", Configuration.Name), LogMessageSeverity.Warning);
            }
        }

        #endregion
    }
}
