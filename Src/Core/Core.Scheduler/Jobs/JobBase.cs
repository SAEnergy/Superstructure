using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components.Scheduler;
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
    public abstract class JobBase : IJob
    {
        #region Fields

        private TimeSpan _cancelWaitCycle = TimeSpan.FromMilliseconds(500);

        protected readonly ILogger _logger;

        private List<Task> _taskList;

        private CancellationTokenSource cancelSource;

        #endregion

        #region Properties

        public JobConfiguration Configuration { get; private set; }

        public JobStatus Status { get; private set; }

        public DateTime LastStartTime { get; private set; }

        public TimeSpan LastRunDuration { get; private set; }

        public DateTime NextRunTime { get; private set; }

        public bool MissedLastRunTime { get; private set; }

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

        protected JobBase(ILogger logger, JobConfiguration config)
        {
            Status = JobStatus.Unknown;
            _logger = logger;
            Configuration = config;
            _taskList = new List<Task>();
            cancelSource = new CancellationTokenSource();

            _logger.Log(string.Format("Job name \"{0}\" created of type \"{1}\".", config.Name, GetType()));

            NextRunTime = DateTime.UtcNow.Add(CalculateNextRunWaitTime());
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
                    Status = JobStatus.Cancelling;

                    _logger.Log(string.Format("Job name \"{0}\" canceling all executing tasks.  ", Configuration.Name), LogMessageSeverity.Warning);

                    cancelSource.Cancel();

                    while(IsExecuting)
                    {
                        _logger.Log(string.Format("Job name \"{0}\" waiting on tasks to cancel...", Configuration.Name), LogMessageSeverity.Warning);
                        Thread.Sleep(_cancelWaitCycle);
                    }

                    Status = JobStatus.Cancelled;
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
                if(NextRunTime < DateTime.UtcNow || Configuration.RunImmediatelyIfRunTimeMissed && MissedLastRunTime)
                {
                    TryExecute(cancelSource.Token);

                    if (!MissedLastRunTime || !Configuration.RunImmediatelyIfRunTimeMissed)
                    {
                        NextRunTime = DateTime.UtcNow.Add(CalculateNextRunWaitTime());
                    }
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
            return NextRunTime < DateTime.UtcNow;
        }

        private TimeSpan CalculateNextRunWaitTime()
        {
            TimeSpan result = TimeSpan.MaxValue;

            if (Configuration.TriggerType != JobTriggerType.NotConfigured)
            {
                var secondsInDay = (int)Math.Floor(TimeSpan.FromDays(1).TotalSeconds);

                int seconds = Configuration.StartTimeInSeconds;

                if (Configuration.StartTimeInSeconds > secondsInDay)
                {
                    _logger.Log(string.Format("Job by the name of \"{0}\" start time set to more that one days worth of seconds, defaulting to start running at midnight.", Configuration.Name), LogMessageSeverity.Warning);
                    seconds = 0;
                }

                DateTime startTime = DateTime.Today.Add(TimeSpan.FromSeconds(seconds));

                TimeSpan offset = startTime.Subtract(DateTime.UtcNow.ToLocalTime());

                switch (Configuration.TriggerType)
                {
                    case JobTriggerType.Continuously:

                        if (offset.TotalSeconds < 0)
                        {
                            offset = offset.Add(TimeSpan.FromDays(1));
                        }

                        result = offset;

                        if (Configuration.RepeatEvery.Enabled)
                        {
                            var repeatSeconds = Configuration.RepeatEvery.TimeInSeconds;

                            if (Configuration.RepeatEvery.TimeInSeconds < 1)
                            {
                                _logger.Log(string.Format("Job by the name of \"{0}\" start time set to repeat faster than every 1 second, defaulting to run every 1 second.", Configuration.Name), LogMessageSeverity.Warning);
                                repeatSeconds = 1;
                            }

                            result = TimeSpan.FromSeconds(offset.TotalSeconds % repeatSeconds); //this is the wait time till the next repeat
                        }
                        break;

                    case JobTriggerType.Daily:
                        if (Configuration.TriggerDays != JobTriggerDays.NotConfigured)
                        {
                            result = FindNextTriggerTimeSpanDays(offset);
                        }
                        else
                        {
                            Status = JobStatus.Misconfigured;
                            _logger.Log(string.Format("Job by the name of \"{0}\" has a trigger type of \"{1}\" that is misconfigured.  This job will not run!", Configuration.Name, Configuration.TriggerType), LogMessageSeverity.Critical);
                        }

                        break;

                    case JobTriggerType.Weekly:

                        break;

                    case JobTriggerType.Monthly:

                        break;

                    default:
                        _logger.Log(string.Format("Job by the name of \"{0}\" has a trigger type of \"{1}\" that is not supported.", Configuration.Name, Configuration.TriggerType), LogMessageSeverity.Warning);
                        break;
                }
                if(result != TimeSpan.MaxValue)
                {
                    _logger.Log(string.Format("Job by the name of \"{0}\" scheduled to run in {1} day(s), {2} hour(s) {3} minute(s) and {4} second(s).", Configuration.Name, result.Days, result.Hours, result.Minutes, result.Seconds));
                }
            }
            else
            {
                Status = JobStatus.Misconfigured;
                _logger.Log(string.Format("Job by the name of \"{0}\" has a trigger type of \"{1}\" that is misconfigured.  This job will not run!", Configuration.Name, Configuration.TriggerType), LogMessageSeverity.Critical);
            }

            return result;
        }

        private TimeSpan FindNextTriggerTimeSpanDays(TimeSpan startTimeOffset)
        {
            var result = startTimeOffset;

            if (Configuration.TriggerDays != JobTriggerDays.NotConfigured)
            {
                var dayStart = (int)DateTime.UtcNow.ToLocalTime().DayOfWeek;

                //if the offset is negative, we missed todays start time.
                if(result.TotalSeconds < 0)
                {
                    result = result.Add(TimeSpan.FromDays(1));
                    dayStart++;
                }

                var dayToCheck = GetJobTriggerDays(dayStart);

                var day = dayStart;

                while (!Configuration.TriggerDays.HasFlag(dayToCheck))
                {
                   result = result.Add(TimeSpan.FromDays(1));

                    day = day >= 6 ? 0 : day + 1;
                    dayToCheck = GetJobTriggerDays(day);

                    //safety
                    if(day == dayStart)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private JobTriggerDays GetJobTriggerDays(int day)
        {
            JobTriggerDays jobDay = JobTriggerDays.NotConfigured;

            if(day < 7)
            {
                jobDay = (JobTriggerDays)Math.Pow(2, day);
            }

            return jobDay;
        }

        private async void TryExecute(CancellationToken ct)
        {
            if (Status != JobStatus.Misconfigured)
            {
                if (!IsExecuting || Configuration.SimultaneousExecutions)
                {
                    if (MissedLastRunTime)
                    {
                        _logger.Log(string.Format("Job \"{0}\" missed last run time, executing late...", Configuration.Name), LogMessageSeverity.Warning);
                        MissedLastRunTime = false;
                    }

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
                        LastStartTime = DateTime.UtcNow.ToLocalTime();

                        task.Start();
                        Status = JobStatus.Running;

                        rc = await task;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Log(string.Format("Job \"{0}\" canceled.", Configuration.Name), LogMessageSeverity.Warning);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(string.Format("Job \"{0}\" failed! Error - {1}.", Configuration.Name, ex.Message), LogMessageSeverity.Error);
                    }

                    watch.Stop();

                    LastRunDuration = watch.Elapsed;

                    lock (_taskList)
                    {
                        _taskList.Remove(task);
                    }

                    Status = rc ? JobStatus.Success : JobStatus.Error;

                    string message = rc ? "completed successfully" : "failed to complete successfully";

                    _logger.Log(string.Format("Job \"{0}\" {1}, run time = {2:hh\\:mm\\:ss}", Configuration.Name, message, watch.Elapsed), rc ? LogMessageSeverity.Information : LogMessageSeverity.Error);
                }
                else
                {
                    //only message about it once
                    if (!MissedLastRunTime)
                    {
                        _logger.Log(string.Format("Job \"{0}\" attempting to execute, but is already running.", Configuration.Name), LogMessageSeverity.Warning);
                    }
                    MissedLastRunTime = true;
                }
            }
            else
            {
                _logger.Log(string.Format("Job \"{0}\" attempting to execute, but is misconfigured.", Configuration.Name), LogMessageSeverity.Error);
            }
        }

        #endregion
    }
}
