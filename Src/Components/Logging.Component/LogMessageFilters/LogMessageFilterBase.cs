using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;

namespace Core.Logging.LogMessageFilters
{
    public abstract class LogMessageFilterBase : List<IFilter>, ILogMessageFilter
    {
        public abstract LogMessage Filter(LogMessage message);

        public abstract void FilterAdd(IFilter filter);

        public abstract void FilterClear();
    }
}
