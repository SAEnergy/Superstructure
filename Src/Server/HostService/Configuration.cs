using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using Core.Settings;
using Core.Settings.BaseClasses;
using Server.Database;
using System.Configuration;

namespace HostService
{
    public class Configuration : ConfigurationBase
    {
        private static object syncObject = new object();
        private static Configuration _instance;

        #region Properties

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Configuration();
                        }
                    }
                }

                return _instance;
            }
        }

        [Configuration(DefaultValue = "%PROGRAMDATA%\\HostService\\Logs")]
        public string LogDirectory { get; set; }

        [Configuration(DefaultValue = "csv")]
        public string LogFileExtension { get; set; }

        [Configuration(DefaultValue = "HostServiceLog")]
        public string LogFilePrefix { get; set; }

        [Configuration(DefaultValue = "CSVLogMessageFormatter")]
        public string LogMessageFormatter { get; set; }

        [Configuration(DefaultValue = 10)]
        public int MaxLogFileSize { get; set; }

        [Configuration(DefaultValue = 10)]
        public int MaxLogFileCount { get; set; }

        [Configuration(DefaultValue = 9595)]
        public int PortNumber { get; set; }

        #endregion

        private Configuration() : base (IoCContainer.Instance.Resolve<ILogger>())
        {
            DatabaseSettings.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString;
        }
    }
}
