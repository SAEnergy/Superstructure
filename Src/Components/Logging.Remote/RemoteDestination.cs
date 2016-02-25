using Core.Logging.LogDestinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using Core.Comm;
using System.Threading;

namespace Core.Logging.Remote
{
    public class RemoteLogDestination : LogDestinationBase
    {
        private Subscription<IRemoteLogging> _conn;
        private string _servername;

        public RemoteLogDestination(string servername = "localhost")
        {
            _servername = servername;
            _conn = new Subscription<IRemoteLogging>(null);
            _conn.Connected += _conn_Connected;
            _conn.Disconnected += _conn_Disconnected;
            _conn.Start();
        }

        private void _conn_Disconnected(ISubscription source, Exception ex)
        {
            _logger.HandleLoggingException("Error while logging to remote server \"" + _servername + "\": " + ex.Message);
        }

        private void _conn_Connected(object sender, EventArgs e)
        {
        }

        public override void ReportMessages(List<LogMessage> messages)
        {
            int retry = 0;
            while (_conn.State != SubscriptionState.Connected && retry < 60)
            {
                retry++;
                Thread.Sleep(1000);
            }
            try
            {
                _conn.Channel.Log(messages.ToArray());
            }
            catch (Exception ex)
            {
                _logger.HandleLoggingException("Error while logging to remote server \"" + _servername + "\": " + ex.Message);
            }
        }

        public override void ShutDownDestination()
        {
            _conn.Stop();
            base.ShutDownDestination();
        }
    }
}
