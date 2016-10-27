using Core.Interfaces.Components.Logging;
using Core.Settings;

namespace VersionReader
{
    public class Settings : ArgumentsBase
    {
        #region Properties

        [Argument]
        public string AssemblyInfoFile { get; set; }

        [Argument(Optional= true, DefaultValue ="VersionNumber")]
        public string TeamCityParameterName { get; set; }

        [Argument]
        public int BuildNumber { get; set; }

        #endregion

        #region Constructor

        public Settings(ILogger logger) : base(logger)
        {
        }

        #endregion
    }
}
