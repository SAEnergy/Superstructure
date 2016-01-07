using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public sealed class ConsoleLogDestination : LogDestinationBase
    {
        #region Constructor

        public ConsoleLogDestination()
        {

        }

        #endregion

        #region Public Methods

        public override void ReportMessages(List<LogMessage> messages)
        {
            StringBuilder builder = new StringBuilder();

            foreach(var message in messages)
            {
                builder.AppendLine(message.Message);
            }

            Console.WriteLine(builder.ToString());
        }

        #endregion
    }
}
