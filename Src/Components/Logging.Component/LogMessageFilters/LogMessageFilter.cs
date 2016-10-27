using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;

namespace Core.Logging.LogMessageFilters
{
    public class LogMessageFilter : LogMessageFilterBase
    {
        public override LogMessage Filter(LogMessage message)
        {
            foreach( IFilter filter in this)
            {
            }

            return null;
        }

        public override void FilterAdd(IFilter filter)
        {
            this.Add(filter);
        }

        public override void FilterClear()
        {
            this.Clear();
        }
    }
}
