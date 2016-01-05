using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mocks
{
    public class LoggerMock : ILogger
    {
        public void AddLogDestination(ILogDestination logDestination)
        {
            throw new NotImplementedException();
        }

        public void Log(LogMessage logMessage)
        {
            throw new NotImplementedException();
        }

        public void RemoveLogDestination(ILogDestination logDestination)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
