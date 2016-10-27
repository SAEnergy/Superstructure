﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IDuplexTestCallback
    { 
        [OperationContract(IsOneWay = true)]
        void MooBack(string moo);
    }

    [ServiceContract(CallbackContract=typeof(IDuplexTestCallback))]
    public interface IDuplexTest : IUserAuthentication
    {
        [OperationContract]
        bool Moo();

        [OperationContract]
        void ThrowException();
    }
}
