using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Scheduler
{
    public interface IJob
    {
        JobStatus Status { get; }

        JobConfiguration Configuration { get; set; }

        bool Run();

        bool Cancel();

        bool Pause();
    }
}
