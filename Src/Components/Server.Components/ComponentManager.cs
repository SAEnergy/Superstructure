using Core.Interfaces.Base;
using Core.Interfaces.Components;
using Core.Interfaces.Components.Base;
using Core.Interfaces.Components.IoC;
using Core.Interfaces.Components.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.DataContracts;
using Core.Util;
using System.Reflection;
using Core.Models;

namespace Server.Components
{
    [ComponentRegistration(ComponentType.Server, typeof(IComponentManager))]
    [ComponentMetadata(AllowedActions = ComponentUserActions.Restart, Description = "Controller for all components.", FriendlyName = "Component Manager")]

    public class ComponentManager : Singleton<IComponentManager>, IComponentManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IIoCContainer _container;

        private Dictionary<Type, ComponentMetadata> _metadataCache;

        #endregion

        #region Constructor

        private ComponentManager(ILogger logger, IIoCContainer container)
        {
            _logger = logger;
            _container = container;

            _metadataCache = new Dictionary<Type, ComponentMetadata>();
        }

        #endregion

        #region Public Methods

        public static IComponentManager CreateInstance(ILogger logger, IIoCContainer container)
        {
            return Instance = new ComponentManager(logger, container);
        }

        public ComponentMetadata[] GetComponents()
        {
            var infos = new List<ComponentMetadata>();

            foreach (var type in _container.GetRegisteredTypes())
            {
                infos.Add(GetMetadata(type));
            }

            return infos.ToArray();
        }

        public void StartAll()
        {
            _logger.Log("Starting all runnable components");

            foreach (var type in GetRunnableRegisteredTypes())
            {
                _logger.Log(string.Format("Starting component of type {0}", type.Name));

                StartRunnable(GetIRunnable(type));
            }
        }

        public void StopAll()
        {
            _logger.Log("Stopping all runnable components");

            foreach (var type in GetRunnableRegisteredTypes())
            {
                _logger.Log(string.Format("Stopping component of type {0}", type.Name));

                StopRunnable(GetIRunnable(type));
            }
        }

        public void StartComponent(int componentId)
        {
            _logger.Log(string.Format("Attempting to start component with id \"{0}\".", componentId));

            var type = GetComponentType(componentId);

            if(type.Key != null)
            {
                if (type.Value.UserActions.HasFlag(ComponentUserActions.Start))
                {
                    StartRunnable(GetIRunnable(type.Key));
                }
                else
                {
                    _logger.Log(string.Format("Cannot start component \"{0}\".  This component is not startable", type.Value.FriendlyName), LogMessageSeverity.Warning);
                }
            }
        }

        public void StopComponent(int componentId)
        {
            _logger.Log(string.Format("Attempting to stop component with id \"{0}\".", componentId));

            var type = GetComponentType(componentId);

            if (type.Key != null)
            {
                if (type.Value.UserActions.HasFlag(ComponentUserActions.Stop))
                {
                    StopRunnable(GetIRunnable(type.Key));
                }
                else
                {
                    _logger.Log(string.Format("Cannot stop component \"{0}\".  This component is not stopable", type.Value.FriendlyName), LogMessageSeverity.Warning);
                }
            }
        }

        public void RestartComponent(int componentId)
        {
            _logger.Log(string.Format("Attempting to restart component with id \"{0}\".", componentId));

            var type = GetComponentType(componentId);

            if (type.Key != null)
            {
                if (type.Value.UserActions.HasFlag(ComponentUserActions.Restart))
                {
                    StopRunnable(GetIRunnable(type.Key));
                    StartRunnable(GetIRunnable(type.Key));
                }
                else
                {
                    _logger.Log(string.Format("Cannot restart component \"{0}\".  This component is not restartable", type.Value.FriendlyName), LogMessageSeverity.Warning);
                }
            }
        }

        #endregion

        #region Private Methods

        private KeyValuePair<Type, ComponentMetadata>  GetComponentType(int componentId)
        {
            KeyValuePair<Type, ComponentMetadata> kvp = new KeyValuePair<Type, ComponentMetadata>();

            var query = _metadataCache.Where(k => k.Value.ComponentId == componentId);

            if(query.Any())
            {
                kvp = query.First();

                _logger.Log(string.Format("Resolved component with id \"{0}\" to \"{1}\".", componentId, kvp.Value.FriendlyName));
            }

            return kvp;
        }

        private void StartRunnable(IRunnable runnable)
        {
            if (runnable != null)
            {
                if (!runnable.IsRunning)
                {
                    runnable.Start();
                }
                else
                {
                    _logger.Log(string.Format("Cannot start component \"{0}\" because it is already running.", runnable.GetType().Name), LogMessageSeverity.Warning);
                }
            }
        }

        private void StopRunnable(IRunnable runnable)
        {
            if (runnable != null)
            {
                if (runnable.IsRunning)
                {
                    runnable.Stop();
                }
                else
                {
                    _logger.Log(string.Format("Cannot stop component \"{0}\" because it is not running.", runnable.GetType().Name), LogMessageSeverity.Warning);
                }
            }
        }

        private IRunnable GetIRunnable(Type type)
        {
            return _container.Resolve(type) as IRunnable;
        }

        private List<Type> GetRunnableRegisteredTypes()
        {
            return _container.GetRegisteredTypes().Select(k => k.Key).Where(i => typeof(IRunnable).IsAssignableFrom(i) && i != typeof(ILogger)).ToList();
        }

        private List<ComponentMetadata> GetDependencies(Type type)
        {
            var dependencies = new List<ComponentMetadata>();

            if (type != null)
            {
                var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

                if (constructor != null)
                {
                    var parameterInfoes = constructor.GetParameters().ToList();

                    if (parameterInfoes.Count > 0)
                    {
                        foreach (var parameter in parameterInfoes)
                        {
                            if (parameter.ParameterType.IsInterface)
                            {
                                var query = _container.GetRegisteredTypes().Where(p => p.Key == parameter.ParameterType);

                                if (query.Any())
                                {
                                    var kvp = query.First();

                                    dependencies.Add(GetMetadata(kvp));
                                }
                            }
                        }
                    }
                }
            }

            return dependencies;
        }

        private ComponentMetadata GetMetadata(KeyValuePair<Type, Type> type)
        {
            ComponentMetadata info;

            if (!_metadataCache.TryGetValue(type.Key, out info))
            {
                info = new ComponentMetadata();
                info.InterfaceTypeName = type.Key.Name;
                info.ConcreteTypeName = type.Value.Name;

                var metaAtty = type.Value.GetAttribute<ComponentMetadataAttribute>();

                if (metaAtty != null)
                {
                    info.Description = metaAtty.Description;
                    info.FriendlyName = metaAtty.FriendlyName;
                    info.UserActions = metaAtty.AllowedActions;
                }

                var regAtty = type.Value.GetAttribute<ComponentRegistrationAttribute>();

                if(regAtty != null)
                {
                    info.Type = regAtty.Type;
                }

                info.Dependencies = GetDependencies(type.Value).ToArray();

                info.ComponentId = info.GetHashCode();

                _metadataCache.Add(type.Key, info);
            }

            return info;
        }

        #endregion
    }
}
