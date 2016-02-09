using Core.Comm;
using Core.Interfaces.Base;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Core.Util;

namespace Core.Components
{
    public class ServiceHostInfo
    {
        public Type InterfaceType { get; set; }
        public ServiceHost Host { get; set; }
        public ILogger Logger { get; set; }
    }


    public sealed class HostManagerComponent : Singleton<IHostManagerComponent>, IHostManagerComponent
    {
        #region Fields

        private const string _dllSearchPattern = "*.Hosts.dll";
        private readonly ILogger _logger;

        private Dictionary<Type, ServiceHostInfo> _hosts;

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

                host.Host.Open();
            }
        }

        public void StopAll()
        {
            _logger.Log("HostManager stopping all hosts...");

            foreach (var host in _hosts.Values)
            {
                _logger.Log(string.Format("HostManager stopping host of type \"{0}\".", host.GetType().Name));

                host.Host.Abort();
            }
        }

        public void Restart<T>()
        {
            Stop<T>();
            Start<T>();
        }

        public void Start<T>()
        {
            ServiceHostInfo host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager starting host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Host.Open();
                }
            }
        }

        public void Stop<T>()
        {
            ServiceHostInfo host;

            if (!_hosts.TryGetValue(typeof(T), out host))
            {
                _logger.Log(string.Format("HostManager cannot find host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);
            }
            else
            {
                if (host != null)
                {
                    _logger.Log(string.Format("HostManager stopping host with interface type of \"{0}\".", typeof(T).Name), LogMessageSeverity.Error);

                    host.Host.Abort();
                }
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<Type, ServiceHostInfo> FindAllHosts()
        {
            var hosts = new Dictionary<Type, ServiceHostInfo>();

            foreach (Type type in TypeLocator.FindTypes(_dllSearchPattern,typeof(IServiceHost)))
            {
                ServiceHostInfo info = new ServiceHostInfo();

                info.Logger = _logger;

                _logger.Log(string.Format("HostManager creating host of type \"{0}\".", type));

                Type interfaceType = (Type)type.GetMethod("GetInterfaceType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Invoke(null, null);
                info.InterfaceType = interfaceType;

                info.Host = new ServiceHost(type);

                EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9595/tcp/" + interfaceType.Name + "/");

                Binding binding = new NetTcpBinding(SecurityMode.None, false);

                ServiceEndpoint service = new ServiceEndpoint(ContractDescription.GetContract(interfaceType), binding, endpoint);

                info.Host.Description.Behaviors.Add(new HostErrorHandlerBehavior(info));

                info.Host.AddServiceEndpoint(service);

                hosts.Add(interfaceType, info);
            }

            return hosts;
        }

        #endregion
    }


    public class HostErrorHandler : IErrorHandler
    {
        private ServiceHostInfo _info;

        public HostErrorHandler(ServiceHostInfo info) { _info = info; }

        public bool HandleError(Exception error)
        {
            _info.Logger.Log(_info.InterfaceType.Name + " exception: " + error.Message, LogMessageSeverity.Warning);
            //prevent host from closing
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException) { return; }

            //bool serviceDebug = OperationContext.Current.EndpointDispatcher.ChannelDispatcher.IncludeExceptionDetailInFaults;

            FaultException faultException = new FaultException("The server encountered an error of type " + error.GetType());
            MessageFault faultMessage = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, faultMessage, faultException.Action);
        }
    }

    public class HostErrorHandlerBehavior : IServiceBehavior
    {
        private ServiceHostInfo _info;

        public HostErrorHandlerBehavior(ServiceHostInfo info) { _info = info; }

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcherBase in
                 serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                    channelDispatcher.ErrorHandlers.Add(new HostErrorHandler(_info));
            }

        }
    }
}
