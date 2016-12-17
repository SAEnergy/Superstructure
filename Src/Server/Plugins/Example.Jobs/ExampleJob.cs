using Core.Interfaces.Components.Logging;
using Scheduler.Component.Jobs;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Jobs
{
    public class ExampleJob : JobBase<ExampleJobConfiguration>
    {

        public ExampleJob(ILogger logger, ExampleJobConfiguration config) : base(logger, config) { }

        public override bool Execute()
        {
            Statistics.TotalItems = Configuration.NumberOfItems;

            for (int x=0;x<Configuration.NumberOfItems;x++)
            {
                Statistics.Completed++;

                if (Configuration.RunTimeSeconds > 0)
                {
                    Thread.Sleep(Configuration.RunTimeSeconds * 1000 / Configuration.NumberOfItems);
                }

                FireStatusUpdate();
                TaskCancellationToken.ThrowIfCancellationRequested();
            }
            return true;
        }
    }
}
