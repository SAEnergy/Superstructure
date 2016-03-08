using Core.Interfaces.Components.Logging;
using System.ServiceModel;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IRemoteLogViewerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReportMessages(LogMessage[] messages);
    }

    [ServiceContract(CallbackContract = typeof(IRemoteLogViewerCallback))]
    public interface IRemoteLogViewer : IUserAuthentication
    {
        [OperationContract]
        void Register();

        [OperationContract]
        void Unregister();
    }
}
