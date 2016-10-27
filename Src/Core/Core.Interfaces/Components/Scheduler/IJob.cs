﻿using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components.Scheduler
{
    public interface IJob
    {
        JobStatus Status { get; }

        JobConfiguration Configuration { get; }

        void ForceRun();

        void TryCancel();

        void Start();

        //void TryPause();

        //void UpdateConfiguration(JobConfiguration newConfig);
    }
}
