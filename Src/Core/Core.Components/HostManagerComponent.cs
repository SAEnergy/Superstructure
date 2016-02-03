using Core.Comm;
using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Core.Components
{
    public sealed class HostManagerComponent : Singleton<IHostManagerComponent>, IHostManagerComponent
    {
        #region Fields

        private const string _dllSearchPattern = "*.Hosts.dll";
        private readonly ILogger _logger;

        private Dictionary<Type, ServiceHost> _hosts;

        #endregion

        #region Constructor

        private HostManagerComponent(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public static IHostManagerComponent CreateInstance(ILogger logger)
        {
            return Instance = new HostManagerComponent(logger);
        }

        public void RestartAll()
        {
            _logger.Log("HostManager restarting all hosts...");

            StopAll();
            StartAll();
        }

        public void StartAll()
        {
            _logger.Log("HostManager starting all hosts...");

            _hosts = FindAllHosts();

            foreach (var host in _hosts.Values)
            {
                _logger.Log(string.Format("HostManager starting host of type \"{0}\".", host.GetType().Name));

                host.Open();
            }
        }

        public void StopAll()
        {
            _logger.Log("HostManager stopping all hosts...");

            foreach (var host in _hosts.Values)
            {
                _logger.Log(string.Format("HostManager stopping host of type \"{0}\".", host.GetType().Name));

                host.Close();
            }
        }

        public void Restart<T>()
        {
            Stop<T>();
            Start<T>();
        }

        public void Start<T>()
        {
            ServiceHost host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager starting host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Open();
                }
            }
        }

        public void Stop<T>()
        {
            ServiceHost host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager stopping host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Close();
                }
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<Type, ServiceHost> FindAllHosts()
        {
            var hosts = new Dictionary<Type, ServiceHost>();

            var files = Directory.GetFiles(Environment.CurrentDirectory, _dllSearchPattern, SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                var assm = Assembly.LoadFile(file);

                var types = assm.GetTypes().Where(t => typeof(IServiceHost).IsAssignableFrom(t) && t.ContainsGenericParameters == false);

                foreach (Type type in types)
                {
                    _logger.Log(string.Format("HostManager creating host of type \"{0}\".", type));

                    Type interfaceType = (Type)type.GetMethod("GetInterfaceType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Invoke(null, null);

                    ServiceHost host = new ServiceHost(type);

                    EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9595/tcp/" + interfaceType.Name + "/");

                    Binding binding = new NetTcpBinding(SecurityMode.None, false);

                    ServiceEndpoint service = new ServiceEndpoint(ContractDescription.GetContract(interfaceType), binding, endpoint);

                    host.AddServiceEndpoint(service);

                    hosts.Add(interfaceType, host);
                }
            }

            return hosts;
        }

        #endregion
    }
}
