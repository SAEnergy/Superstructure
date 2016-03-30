using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;

namespace Core.Logging.LogMessageFilters
{
    class Filter : IFilter
    {
        LogMessage IFilter.Filter(LogMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
