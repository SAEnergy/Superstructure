using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;
using Core.Logging.Log4Net.LogDestinationConfigs;
using log4net.Layout;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;

namespace Core.Logging.Log4Net 
{
    public class XMLDestination : LogDestinations.LogDestinationBase
    {
        private static readonly ILog logger =
           LogManager.GetLogger(typeof(XMLDestination));

        private readonly XMLDestinationConfig _config;

        public XMLDestination(XMLDestinationConfig config)
        {
            if (config != null)
            {
                _config = config;

                if( _config.LogMessageFormatter == null)
                    _config.LogMessageFormatter = new Core.Logging.LogMessageFormats.CSVLogMessageFormatter();

                ConfigureLog4Net();
            }
            else
            {
                throw new ArgumentNullException("config");
            }
            
        }

        private void ConfigureLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            // output to be in XML
            XmlLayout xmlLayout = new XmlLayout();
            xmlLayout.ActivateOptions();

            // use rolling log file
            RollingFileAppender roll = new RollingFileAppender();
            roll.AppendToFile = false;
            roll.File = _config.LogDirectory + "\\" + _config.LogFilePrefix + ".xml";
            roll.Layout = xmlLayout; 
            roll.MaxSizeRollBackups = 5;

            string maxFileSize = "10";
            if (_config.MaxLogFileSize > 0)
                maxFileSize = _config.MaxLogFileSize.ToString();

            roll.MaximumFileSize = maxFileSize + "MB";
            roll.RollingStyle = RollingFileAppender.RollingMode.Size;
            roll.StaticLogFileName = true;
            roll.ActivateOptions();
            hierarchy.Root.AddAppender(roll);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }


        public override void ReportMessages(List<LogMessage> messages)
        {
            foreach(LogMessage msg in messages)
            {
                if (LogMessageSeverity.Information == msg.Severity)
                {
                    logger.Info(_config.LogMessageFormatter.Format(msg));
                }
                else if (LogMessageSeverity.Warning == msg.Severity)
                {
                    logger.Warn(_config.LogMessageFormatter.Format(msg));
                }
                else if (LogMessageSeverity.Error == msg.Severity)
                {
                    logger.Error(_config.LogMessageFormatter.Format(msg));
                }
                else if (LogMessageSeverity.Critical == msg.Severity)
                {
                    // map critical to log4net fatal
                    logger.Fatal(_config.LogMessageFormatter.Format(msg));
                }
                else 
                {
                    // log everything else as info
                    logger.Info(_config.LogMessageFormatter.Format(msg));
                }
            }
        }
        
    }
}
