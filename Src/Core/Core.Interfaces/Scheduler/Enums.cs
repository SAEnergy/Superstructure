using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Scheduler
{
    public enum JobStatus
    {
        Unknown,
        Paused,
        Running,
        Success,
        Error,
        Cancelling,
        Cancelled
    }
}
