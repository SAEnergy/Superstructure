using Core.Interfaces.Components.Logging;
using Core.Settings;

namespace VersionReader
{
    public class Settings : ArgumentsBase
    {
        #region Properties

        [Argument]
        public string FileToLookAt { get; set; }

        [Argument(Optional= true, DefaultValue ="VersionNumber")]
        public string TeamCityParameterName { get; set; }

        #endregion

        #region Constructor

        public Settings(ILogger logger) : base(logger)
        {
        }

        #endregion
    }
}
