using Core.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Scheduler
{
    public interface ISchedulerService : IRunnable
    {
        bool AddJob(IJob job);

        bool DeleteJob(IJob job);
    }
}
