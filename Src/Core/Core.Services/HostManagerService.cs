using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.Services
{
    public class HostManagerService : IHostManagerService
    {
        #region Fields

        private const string _dllSearchPattern = "*.Hosts.dll";
        private readonly ILogger _logger;

        private Dictionary<Type, IHost> _hosts;

        #endregion

        #region Constructor

        public HostManagerService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

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

            foreach(var host in _hosts.Values)
            {
                _logger.Log(string.Format("HostManager starting host of type \"{0}\".", host.GetType().Name));

                host.Start();
            }
        }

        public void StopAll()
        {
            _logger.Log("HostManager stopping all hosts...");

            foreach (var host in _hosts.Values)
            {
                _logger.Log(string.Format("HostManager stopping host of type \"{0}\".", host.GetType().Name));

                host.Stop();
            }
        }

        public void Restart<T>()
        {
            Stop<T>();
            Start<T>();
        }

        public void Start<T>()
        {
            IHost host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager starting host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Start();
                }
            }
        }

        public void Stop<T>()
        {
            IHost host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager stopping host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Stop();
                }
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<Type, IHost> FindAllHosts()
        {
            var hosts = new Dictionary<Type, IHost>();

            var files = Directory.GetFiles(Environment.CurrentDirectory, _dllSearchPattern, SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                var assm = Assembly.LoadFile(file);

                var types = assm.GetTypes().Where(t => typeof(IHost).IsAssignableFrom(t));

                foreach (Type type in types)
                {
                    _logger.Log(string.Format("HostManager creating host of type \"{0}\".", type));

                    IHost host = Activator.CreateInstance(type) as IHost;

                    hosts.Add(host.InterfaceType, host);
                }
            }

            return hosts;
        }

        #endregion
    }
}
