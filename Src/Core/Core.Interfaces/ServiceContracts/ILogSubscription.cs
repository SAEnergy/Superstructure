﻿using Core.Interfaces.Components.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface ILogSubscriptionCallback
    {
        [OperationContract(IsOneWay = true)]
        void MessagesReceived(LogMessage[] messages);
    }

    [ServiceContract(CallbackContract = typeof(ILogSubscriptionCallback))]
    public interface ILogSubscription
    {
        [OperationContract]
        void Start();

        [OperationContract]
        void Pause();

        [OperationContract]
        void Resume();
    }
}
