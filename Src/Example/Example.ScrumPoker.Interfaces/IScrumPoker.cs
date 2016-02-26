using Core.Interfaces.ServiceContracts;
using System.ServiceModel;

namespace Example.ScrumPoker.Interfaces
{
    [ServiceContract]
    public interface IScrumPokerCallback
    {
        [OperationContract(IsOneWay = true)]
        void DummyCallback();
    }

    [ServiceContract(CallbackContract = typeof(IScrumPokerCallback))]
    public interface IScrumPoker : IUserAuthentication
    {
        [OperationContract]
        double[] GetCardValues();
    }
}
