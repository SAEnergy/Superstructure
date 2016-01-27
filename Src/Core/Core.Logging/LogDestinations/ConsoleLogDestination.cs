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
            _destinationQueue.IsBlocking = false;
        }

        #endregion


        #region Private Methods

        private void WriteLine(string msg)
        {
            Console.Write(msg);
            Console.ResetColor();
            Console.WriteLine();
        }

        private void WriteLineInformation(LogMessage message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            WriteLine(message.Message);
        }

        private void WriteLineWarning(LogMessage message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(message.Message);
        }

        private void WriteLineError(LogMessage message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(message.Message);
        }

        private void WriteLineCritical(LogMessage message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(message.Message);
        }

        private void WriteLineNone(LogMessage message)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(message.Message);
            Console.ResetColor();
        }

        #endregion 


        #region Public Methods


        public void WriteLine(LogMessage message)
        {
            if (LogMessageSeverity.Information == message.Severity)
            {
                WriteLineInformation(message);
            }
            else if (LogMessageSeverity.Warning == message.Severity)
            {
                WriteLineWarning(message);
            }
            else if (LogMessageSeverity.Error == message.Severity)
            {
                WriteLineError(message);
            }
            else if (LogMessageSeverity.Critical == message.Severity)
            {
                WriteLineCritical(message);
            }
            else
            {
                WriteLineNone(message);
            }
        }


        public override void ReportMessages(List<LogMessage> messages)
        {
            foreach(LogMessage message in messages)
            {
                WriteLine(message);
            }
        }



        #endregion
    }
}
