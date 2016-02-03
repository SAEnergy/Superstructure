using Core.Interfaces.Base;
using Core.Models.Persistent;
using System.Collections.Generic;

namespace Core.Interfaces.Components.Scheduler
{
    public interface ISchedulerComponent : IRunnable
    {
        List<JobConfiguration> GetJobs();

        bool AddJob(JobConfiguration job);

        bool DeleteJob(JobConfiguration job);

        bool UpdateJob(JobConfiguration job);
    }
}
