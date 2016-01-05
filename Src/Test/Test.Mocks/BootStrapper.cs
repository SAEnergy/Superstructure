using Core.Interfaces.IoC;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;

namespace Test.Mocks
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, LoggerMock>();
            container.Register<IDataService, DataServiceMock>();

            #endregion
        }

    }
}
