using Core.Interfaces.Base;
using Core.Models.Persistent;
using System;
using System.Collections.Generic;

namespace Core.Interfaces.Components.Scheduler
{
    public interface IScheduler : IRunnable
    {
        List<JobConfiguration> GetJobs();

        void AddJob(JobConfiguration job);

        bool DeleteJob(JobConfiguration job);

        bool UpdateJob(JobConfiguration job);
    }
}
