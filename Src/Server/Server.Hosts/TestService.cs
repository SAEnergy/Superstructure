using Core.Comm.BaseClasses;
using Core.Interfaces.Logging;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Hosts
{
    public class TestService : RESTHostBase, ITestService
    {
        public string Test()
        {
            Logger.Log("Test Called.");

            return "Hello";
        }
    }
}
