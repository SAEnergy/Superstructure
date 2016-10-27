using Core.Logging.LogDestinations;
using System.Collections.Generic;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using System;
using System.ServiceModel;

namespace Logging.Remote
{
    public class RemoteLogViewerDestination : LogDestinationBase
    {
        private IRemoteLogViewerCallback _callback;

        public RemoteLogViewerDestination(IRemoteLogViewerCallback callback)
        {
            _callback = callback;
            _destinationQueue.IsBlocking = false;
        }

        public override void ReportMessages(List<LogMessage> messages)
        {
            if(_callback != null)
            {
                if (((ICommunicationObject)_callback).State == CommunicationState.Opened)
                {
                    try
                    {
                        _callback.ReportMessages(messages.ToArray());
                    }
                    catch (Exception ex)
                    {
                        _logger.HandleLoggingException(string.Format("Error while sending logs to remote viewer: {0}", ex.Message));
                    }
                }
            }
        }
    }
}
