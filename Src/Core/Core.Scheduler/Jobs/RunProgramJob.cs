using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Scheduler.Jobs
{
    public sealed class RunProgramJob : BaseJob
    {
        public RunProgramJob(ILogger logger, JobConfiguration config) : base(logger, config)
        {

        }

        public override void Execute()
        {
            Random rnd = new Random();

            _logger.Log("SLEEEPING");
            Thread.Sleep(rnd.Next(100,10000));
        }
    }
}
