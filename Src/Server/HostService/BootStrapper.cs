using Core.Interfaces.IoC;
using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Interfaces.Services;
using Core.Logging;
using Core.Scheduler;
using Core.Services;

namespace HostService
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, Logger>();
            container.Register<IDataService, DataService>();
            container.Register<IHostManagerService, HostManagerService>();
            container.Register<ISchedulerService, SchedulerService>();

            #endregion
        }
    }
}
