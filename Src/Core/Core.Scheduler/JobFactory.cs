using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components.Scheduler;
using Core.Models.Persistent;
using Core.Scheduler.Jobs;
using System;
using System.Collections.Generic;

namespace Core.Scheduler
{
    public static class JobFactory
    {
        #region Fields

        private static Dictionary<string, Type> _jobActionTypeMap = new Dictionary<string, Type>()
        {
            { "RunProgram" , typeof(RunProgramJob) }
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
                    if (type != null)
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

        public static bool RegisterType(string name, Type type)
        {
            bool rc = false;

            if (!string.IsNullOrEmpty(name) && type != null)
            {
                if(!_jobActionTypeMap.ContainsKey(name))
                {
                    if (CheckType(type))
                    {
                        _jobActionTypeMap.Add(name, type);

                        rc = true;
                    }
                    else
                    {
                        Logger.Log(string.Format("Job action type \"{0}\" cannot be registered, the provided type does not implement the \"JobBase\" abstract class.", name), LogMessageSeverity.Error);
                    }
                }
                else
                {
                    Logger.Log(string.Format("Job action type \"{0}\" cannot be registered, this action type name already exists.", name), LogMessageSeverity.Error);
                }
            }
            else
            {
                Logger.Log("RegisterType provided Null arguments.", LogMessageSeverity.Error);
            }

            return rc;
        }

        #endregion

        #region Private Methods

        private static bool CheckType(Type type)
        {
            return typeof(JobBase).IsAssignableFrom(type);
        }

        #endregion
    }
}
