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
    public class DuplexTestHost : ServiceHostBase<IDuplexTest>, IDuplexTest
    {
        public bool Moo()
        {
            return true;
        }
    }
}
