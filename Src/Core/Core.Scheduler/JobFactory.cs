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
                        retVal = Activator.CreateInstance(type) as IJob;

                        if(retVal != null)
                        {
                            retVal.Configuration = config;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("config");
            }

            return retVal;
        }

        #endregion
    }
}
