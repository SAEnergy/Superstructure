using Core.Interfaces.Logging;
using Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
