using Core.Interfaces.Scheduler;
using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Jobs
{
    public class BaseJob : IJob
    {
        public JobConfiguration Configuration { get; set; }

        public JobStatus Status { get; private set; }

        public bool Cancel()
        {
            throw new NotImplementedException();
        }

        public bool Pause()
        {
            throw new NotImplementedException();
        }

        public bool Run()
        {
            throw new NotImplementedException();
        }
    }
}
