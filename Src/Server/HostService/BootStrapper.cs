using Core.Interfaces.Components.IoC;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components.Scheduler;
using Core.Interfaces.Components;
using Core.Logging;
using Core.Scheduler;
using Core.Components;
using Core.Proxies;
using Server.WebHost;

namespace HostService
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, Logger>();
            container.Register<IHostManagerComponent, MethodTimerProxy<HostManagerComponent>>();
            container.Register<ISchedulerComponent, MethodTimerProxy<SchedulerComponent>>();
            container.Register<IWebHost, MethodTimerProxy<WebHost>>();

            #endregion

            #region Transients

            container.Register<IDataComponent, MethodTimerProxy<XMLDataComponent>>(LifeCycle.Transient);
            //container.Register<IDataComponent, SQLDataComponent>(LifeCycle.Transient);

            #endregion
        }
    }
}
