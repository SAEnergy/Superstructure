using Core.Comm;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Models.DataContracts;

namespace ComponentService.Test.App
{
    public class ComponentServiceAbuser : IComponentServiceCallback
    {
        private readonly Subscription<IComponentService> _conn;
        private readonly ILogger _logger;
        private bool Run = false;
        private Thread _worker;

        public ComponentServiceAbuser(ILogger logger)
        {
            _logger = logger;

            _conn = new Subscription<IComponentService>(this);
            _conn.Connected += _conn_Connected;
            _conn.Disconnected += _conn_Disconnected;
            _conn.Start();
        }

        private void _conn_Disconnected(ISubscription source, Exception ex)
        {
            _logger.Log("Disconnected from server");
        }

        private void _conn_Connected(ISubscription source)
        {
            _logger.Log("Connected to server");
        }

        public void Start()
        {
            if(!Run)
            {
                Run = true;

                _worker = new Thread(new ThreadStart(Worker));
                _worker.Start();
            }
        }

        public void Stop()
        {
            if(Run)
            {
                Run = false;
                _worker.Join();
            }

            _conn.Stop();
        }

        private void Worker()
        {
            Random rnd = new Random();

            while (Run)
            {
                if(_conn.State == SubscriptionState.Connected)
                {
                    var channel = _conn.Channel;

                    try
                    {
                        foreach (var component in channel.GetComponents())
                        {
                            if(component.FriendlyName == "Host Manager Component")
                            {
                                //we are using this one...
                                continue;
                            }

                            var val = rnd.Next(0, 3);
                            switch (val)
                            {
                                case 0:
                                    channel.Stop(component.ComponentId);
                                    break;
                                case 1:
                                    channel.Start(component.ComponentId);
                                    break;
                                case 2:
                                    channel.Restart(component.ComponentId);
                                    break;
                                case 3:
                                    channel.Disable(component.ComponentId);
                                    break;
                                default:
                                    break;
                            }

                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception)
                    {
                        _logger.Log("Exception while working...");
                    }
                }
                else
                {
                    _logger.Log("Waiting for connection to server...");
                    Thread.Sleep(1000);
                }
            }
        }

        public void ComponentUpdated(ComponentMetadata component)
        {
            _logger.Log(string.Format("Component \"{0}\" status is now \"{1}\"",component.FriendlyName, component.Status));
        }
    }
}
