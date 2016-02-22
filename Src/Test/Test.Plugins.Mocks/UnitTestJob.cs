using Core.Scheduler.Jobs;
using System.Threading;
using Core.Models.Persistent;
using Core.Interfaces.Components.Logging;

namespace Test.Plugins.Mocks
{
    public class UnitTestJob : JobBase
    {
        public UnitTestJob(ILogger logger, JobConfiguration config) : base(logger, config)
        {

        }

        public override bool Execute(CancellationToken ct)
        {
            return true;
        }
    }
}
