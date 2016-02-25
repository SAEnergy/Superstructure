using Core.Interfaces.Components.Logging;
using Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    public class Settings : ArgumentsBase
    {
        #region Properties

        [Argument]
        public string SourceFolder { get; set; }

        [Argument(Optional = true, DefaultValue = true)]
        public bool Verify { get; set; }

        [Argument(Optional = true)]
        public bool IsReadOnly { get; set; }

        #endregion

        #region Constructor

        public Settings(ILogger logger) : base(logger)
        {
        }

        #endregion
    }
}
