using Core.Interfaces.IoC;
using Core.Interfaces.Logging;
using Core.Interfaces.Components;

namespace Test.Mocks
{
    public static class BootStrapper
    {
        public static void Configure(IIoCContainer container)
        {
            #region Singletons

            container.Register<ILogger, LoggerMock>();
            container.Register<IDataComponent, DataComponentMock>();

            #endregion
        }

    }
}
