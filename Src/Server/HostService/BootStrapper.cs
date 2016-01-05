using Core.Interfaces.IoC;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using Core.Logging;
using Core.Services.DataService;

namespace HostService
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, Logger>();
            container.Register<IDataService, DataService>();

            #endregion
        }
    }
}
