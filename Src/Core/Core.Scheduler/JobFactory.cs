using Core.Interfaces.Logging;
using Core.Interfaces.Scheduler;
using Core.Models.Persistent;
using Core.Scheduler.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
    public static class JobFactory
    {
        #region Fields

        private static Dictionary<JobActionType, Type> _jobActionTypeMap = new Dictionary<JobActionType, Type>()
        {
            { JobActionType.RunProgram, typeof(RunProgramJob) }
        };

        #endregion

        #region Properties

        public static ILogger Logger { get; set; }

        #endregion

        #region Public Methods

        public static IJob Create(JobConfiguration config)
        {
            IJob retVal = null;

            if (config != null)
            {
                Type type = null;

                if (_jobActionTypeMap.TryGetValue(config.ActionType, out type))
                {
                    if(type != null)
                    {
                        Logger.Log(string.Format("Creating job of type \"{0}\".", type.Name));

                        retVal = Activator.CreateInstance(type, Logger, config) as IJob;
                    }
                }
                else
                {
                    Logger.Log(string.Format("Action type \"{0}\" not supported.  This job will not be created.", config.ActionType), LogMessageSeverity.Error);
                }
            }
            else
            {
                Logger.Log("Job cannot be created without a configuration", LogMessageSeverity.Error);
            }

            return retVal;
        }

        #endregion
    }
}
