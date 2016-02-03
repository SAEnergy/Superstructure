using Core.Interfaces.Components.Logging;
using Core.Logging;
using Core.Logging.LogDestinations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionReader
{
    public class Program
    {
        private static Settings _settings;

        public static void Main(string[] args)
        {
            ILogger logger = Logger.CreateInstance();
            logger.AddLogDestination(new ConsoleLogDestination());

            logger.Start();

            _settings = new Settings(logger);

            if(File.Exists(_settings.FileToLookAt))
            {
                foreach(string line in File.ReadAllLines(_settings.FileToLookAt))
                {
                    if (line.StartsWith("[assembly: AssemblyVersion("))
                    {
                        var split = line.Split(new char[] { '\"' });

                        if(split.Length == 3)
                        {
                            int indexOfLastDot = split[1].LastIndexOf('.');

                            if (indexOfLastDot > 0)
                            {
                                string realVersion = split[1].Substring(0, indexOfLastDot);

                                logger.Log(string.Format("##teamcity[setParameter name='{0}' value='{1}']", _settings.TeamCityParameterName, realVersion));
                            }
                            else
                            {
                                logger.Log(string.Format("Cannot find version number in file \"{0}\".", _settings.FileToLookAt), LogMessageSeverity.Error);
                            }
                        }
                        else
                        {
                            logger.Log(string.Format("Unexpected format in file \"{0}\".", _settings.FileToLookAt), LogMessageSeverity.Error);
                        }
                    }
                }
            }
            else
            {
                logger.Log(string.Format("Cannot find file \"{0}\".", _settings.FileToLookAt), LogMessageSeverity.Error);
            }

            logger.Stop();
        }
    }
}
