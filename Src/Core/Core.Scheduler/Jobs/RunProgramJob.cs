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

        public override bool Execute(CancellationToken ct)
        {
            Random rnd = new Random();

            int sleepTime = rnd.Next(2000, 10000);

            _logger.Log(string.Format("SLEEEPING for {0}", sleepTime));

            for (int i = 0; i < 100; i++)
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(sleepTime / 100);
            }

            return true;
        }
    }
}
