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
        private static Random _randy = new Random();
        public ExampleJob(ILogger logger, ExampleJobConfiguration config) : base(logger, config) { }

        public override bool Execute()
        {
            int itemsThisRun = Configuration.NumberOfItems + _randy.Next(-Configuration.ItemVariation, Configuration.ItemVariation);
            TimeSpan sleepSpan = TimeSpan.FromSeconds((Configuration.RunTimeSeconds + _randy.Next(-Configuration.VariationSeconds, Configuration.VariationSeconds) * 1.0) / itemsThisRun);

            Statistics.TotalItems = itemsThisRun;

            for (int x = 0; x < itemsThisRun; x++)
            {
                Statistics.Completed++;
                FireStatusUpdate();
                Thread.Sleep(sleepSpan);
                TaskCancellationToken.ThrowIfCancellationRequested();
            }
            return true;
        }
    }
}
