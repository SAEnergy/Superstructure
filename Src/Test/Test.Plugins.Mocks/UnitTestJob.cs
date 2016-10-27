﻿using Core.Scheduler.Jobs;
using System.Threading;
using Core.Models.Persistent;
using Core.Interfaces.Components.Logging;
using System.Collections.Generic;

namespace Test.Plugins.Mocks
{
    public class UnitTestJob : JobBase
    {
        public static List<UnitTestJob> Instances { get; private set; }

        public delegate void JobExecuteHandler();

        public event JobExecuteHandler JobExecuting;

        public UnitTestJob(ILogger logger, JobConfiguration config) : base(logger, config)
        {
            if(Instances == null)
            {
                Instances = new List<UnitTestJob>();
            }

            Instances.Add(this);
        }

        public override bool Execute(CancellationToken ct)
        {
            if(JobExecuting != null)
            {
                JobExecuting();
            }

            return true;
        }
    }
}
