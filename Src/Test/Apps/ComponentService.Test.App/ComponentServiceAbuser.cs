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
        private bool CanRun = false;
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
            CanRun = false;
        }

        private void _conn_Connected(ISubscription source)
        {
            _logger.Log("Connected to server");
            CanRun = true;
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
                if(CanRun)
                {
                    var channel = _conn.Channel;

                    foreach (var component in channel.GetComponents())
                    {
                        var val = rnd.Next(0, 2);
                        switch(val)
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
                            default:
                                break;
                        }

                        Thread.Sleep(100);
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
            _logger.Log(string.Format("Component \"{0}\" status is now \"{1}\"", component.Status));
        }
    }
}
