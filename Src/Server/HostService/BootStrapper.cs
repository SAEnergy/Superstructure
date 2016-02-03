using Core.Interfaces.Components.IoC;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components.Scheduler;
using Core.Interfaces.Components;
using Core.Logging;
using Core.Scheduler;
using Core.Components;

namespace HostService
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, Logger>();
            container.Register<IDataComponent, XMLDataComponent>(LifeCycle.Transient);
            //container.Register<IDataComponent, SQLDataComponent>(LifeCycle.Transient);
            container.Register<IHostManagerComponent, HostManagerComponent>();
            container.Register<ISchedulerComponent, SchedulerComponent>();

            #endregion
        }
    }
}
