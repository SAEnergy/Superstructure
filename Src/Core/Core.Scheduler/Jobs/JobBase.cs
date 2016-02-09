using Core.Interfaces.Components.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Persistent;
using System.Threading;
using Core.Interfaces.Components.Logging;
using System.Diagnostics;

namespace Core.Scheduler.Jobs
{
    public class JobRunInfo
    {
        public Task<bool> Task { get; private set; }

        public bool IsRunning { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan RunDurration { get; set; }

        public bool CompletedSuccessfully { get; set; }

        public JobRunInfo(Task<bool> task)
        {
            Task = task;
        }
    }

    public abstract class JobBase : IJob
    {
        #region Fields

        private TimeSpan _cancelWaitCycle = TimeSpan.FromMilliseconds(500);

        protected readonly ILogger _logger;

        private CancellationTokenSource cancelSource;

        private List<JobRunInfo> _infos;

        #endregion

        #region Properties

        public JobConfiguration Configuration { get; private set; }

        public JobStatus Status { get; private set; }

        #endregion

        #region Constructor

        protected JobBase(ILogger logger, JobConfiguration config)
        {
            Status = JobStatus.Unknown;
            _logger = logger;
            Configuration = config;
            cancelSource = new CancellationTokenSource();
            _infos = new List<JobRunInfo>();

            _logger.Log(string.Format("Job name \"{0}\" created of type \"{1}\".", config.Name, GetType()));

            LaunchNextJob(cancelSource.Token, false);
        }

        #endregion

        #region Public Methods

        public void ForceRun()
        {
            if (Configuration.RunState != JobRunState.Disabled)
            {
                LaunchNextJob(cancelSource.Token, true);
            }
            else
            {
                _logger.Log(string.Format("Cannot force run disabled job by the name of \"{0}\".", Configuration.Name), LogMessageSeverity.Warning);
            }
        }

        public void TryCancel()
        {
            if (cancelSource != null)
            {
                if (InfosCount() > 0)
                {
                    Status = JobStatus.Cancelling;

                    _logger.Log(string.Format("Job name \"{0}\" canceling all scheduled and currently executing tasks.  ", Configuration.Name), LogMessageSeverity.Warning);

                    cancelSource.Cancel();

                    while (InfosCount() > 0)
                    {
                        _logger.Log(string.Format("Job name \"{0}\" waiting on tasks to cancel...", Configuration.Name), LogMessageSeverity.Warning);
                        Thread.Sleep(_cancelWaitCycle);
                    }

                    Status = JobStatus.Cancelled;
                    _logger.Log(string.Format("Job name \"{0}\" all tasks have been canceled.", Configuration.Name));
                }
            }
        }

        //public void TryPause()
        //{
        //    throw new NotImplementedException();
        //}

        //public void UpdateConfiguration(JobConfiguration newConfig)
        //{
        //    throw new NotImplementedException();
        //}

        public bool HasRunningTask()
        {
            bool rc = false;

            if (_infos != null)
            {
                if (InfosCount() > 0)
                {
                    lock (_infos)
                    {
                        rc = _infos.Any(j => j.IsRunning);
                    }
                }
            }

            return rc;
        }

        public int NumberOfRunningTasks()
        {
            int rc = 0;

            if (_infos != null)
            {
                if (InfosCount() > 0)
                {
                    lock (_infos)
                    {
                        rc = _infos.Where(j => j.IsRunning).Count();
                    }
                }
            }

            return rc;
        }

        public abstract bool Execute(CancellationToken ct);

        #endregion

        #region Private Methods

        private void LaunchNextJob(CancellationToken ct, bool runNow)
        {
            if (Configuration.RunState == JobRunState.Automatic || runNow)
            {
                var info = new JobRunInfo(new Task<bool>(() => Execute(ct), ct));

                info.StartTime = runNow ? DateTime.UtcNow : CalculateNextStartTime();

                if (_infos != null)
                {
                    lock (_infos)
                    {
                        _infos.Add(info);
                    }
                }

                Task.Factory.StartNew(() => Run(ct, info)); //does not block here on purpose
                _logger.Log(string.Format("Job \"{0}\" has been setup to run.", Configuration.Name));
            }
        }

        //a self relaunching scheduler
        private async Task Run(CancellationToken ct, JobRunInfo info)
        {
            if (info != null)
            {
                if (Status != JobStatus.Misconfigured)
                {
                    try
                    {
                        _logger.Log(string.Format(string.Format("Job \"{0}\" scheduled to start \"{1}\"", Configuration.Name, info.StartTime.ToLocalTime())));
                        WaitTillDoneOrThrow(ct, info.StartTime); //wait till it's time, while checking for cancel token

                        _logger.Log(string.Format(string.Format("Job \"{0}\" starting at \"{1}\"", Configuration.Name, DateTime.UtcNow.ToLocalTime())));

                        bool willRun = !HasRunningTask() || Configuration.SimultaneousExecutions;

                        if (!willRun && Configuration.RunImmediatelyIfRunTimeMissed)
                        {
                            _logger.Log(string.Format("Job \"{0}\" missed scheduled execution window, will run immediately after currently executing job.", Configuration.Name), LogMessageSeverity.Warning);
                            var tasks = _infos.Where(j => j.IsRunning).Select(j => j.Task).ToArray();

                            if (tasks.Count() > 0)
                            {
                                Task.WaitAll(tasks); //blocks till all the tasks are done
                            }

                            willRun = true;
                        }

                        LaunchNextJob(ct, false); //launch the next job no matter what

                        if (willRun)
                        {
                            await RunTask(info);
                        }
                        else
                        {
                            _logger.Log(string.Format("Job \"{0}\" missed scheduled execution window.", Configuration.Name), LogMessageSeverity.Warning);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Log(string.Format("The scheduled job, \"{0}\", has been canceled prior to execution.", Configuration.Name), LogMessageSeverity.Warning);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(string.Format("The scheduled job, \"{0}\", has encountered an error prior to execution - {1}.", Configuration.Name, ex.Message), LogMessageSeverity.Error);
                    }
                }
                else
                {
                    _logger.Log(string.Format("Job \"{0}\" cannot run because it is misconfigured...", Configuration.Name), LogMessageSeverity.Error);
                }

                RemoveFromInfos(info); //need to remove here no matter what, success, failure, cancel, exception, etc
            }
        }

        private void WaitTillDoneOrThrow(CancellationToken ct, DateTime startTime)
        {
            var doneYet = new ManualResetEvent(false);

            //must wait in a cycle so we can check to make sure we are not being canceled
            while (!doneYet.WaitOne(_cancelWaitCycle))
            {
                ct.ThrowIfCancellationRequested();

                if (DateTime.UtcNow.ToLocalTime() >= startTime.ToLocalTime())
                {
                    doneYet.Set();
                }
            }
        }

        private void RemoveFromInfos(JobRunInfo info)
        {
            if(info != null && _infos != null)
            {
                lock (_infos)
                {
                    if (_infos.Contains(info))
                    {
                        _infos.Remove(info);
                    }
                }
            }
        }

        private int InfosCount()
        {
            int rc = 0;

            if(_infos != null)
            {
                lock(_infos)
                {
                    rc = _infos.Count;
                }
            }

            return rc;
        }

        private async Task RunTask(JobRunInfo info)
        {
            if (info != null && info.Task != null)
            {
                var watch = new Stopwatch();

                watch.Start();

                bool rc = false;

                try
                {
                    info.Task.Start();
                    Status = JobStatus.Running;
                    info.IsRunning = true;

                    rc = await info.Task;
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

                info.CompletedSuccessfully = rc;
                info.RunDurration = watch.Elapsed;
                info.IsRunning = false;

                Status = rc ? JobStatus.Success : JobStatus.Error;

                string message = rc ? "completed successfully" : "failed to complete successfully";

                _logger.Log(string.Format("Job \"{0}\" {1}, run time = {2:hh\\:mm\\:ss}", Configuration.Name, message, watch.Elapsed), rc ? LogMessageSeverity.Information : LogMessageSeverity.Error);
            }
        }

        private DateTime CalculateNextStartTime()
        {
            return DateTime.UtcNow.Add(CalculateNextRunWaitTime());
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
                if (result != TimeSpan.MaxValue)
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
                if (result.TotalSeconds < 0)
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
                    if (day == dayStart)
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

            if (day < 7)
            {
                jobDay = (JobTriggerDays)Math.Pow(2, day);
            }

            return jobDay;
        }

        #endregion
    }
}
