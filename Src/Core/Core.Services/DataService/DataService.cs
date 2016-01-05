using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.DataService
{
    public class DataService : IDataService
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public DataService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

    }
}
