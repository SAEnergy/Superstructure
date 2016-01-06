using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public sealed class ConsoleLogDestination : BaseLogDestination
    {
        #region Constructor

        public ConsoleLogDestination()
        {

        }

        #endregion

        #region Public Methods

        public override void ProcessMessage(LogMessage message)
        {
            Console.WriteLine(message.Message);
        }

        #endregion
    }
}
