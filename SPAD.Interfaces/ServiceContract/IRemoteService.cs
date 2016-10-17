using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.ServiceContract
{
    public static class RemoteServiceContract
    {
        public static readonly Uri ServiceUrl  = new Uri("net.pipe://localhost/SPAD.neXt");
        public static readonly string ServiceEndpoint = "RemoteService";
    }

    public sealed class RemoteServiceResponse
    {
        public bool HasError { get; set; } = false;
        public string Error { get; set; }
        public double Value { get; set; }
    }

    public sealed class RemoteEventTarget
    {
        public string Name { get; set; }
        public HashSet<string> EventNames { get; set; }
    }

    [ServiceContract( Namespace = Constants.Namespace , CallbackContract = typeof(IRemoteServiceCallback))]
    [ServiceKnownType(typeof(RemoteServiceResponse))]
    [ServiceKnownType(typeof(RemoteEventTarget))]
    public interface IRemoteService
    {
        [OperationContract]
        RemoteServiceResponse GetValue(string variableName);

        [OperationContract]
        RemoteServiceResponse SetValue(string variableName, double newValue);

        [OperationContract]
        RemoteServiceResponse EmulateEvent(string targetDevice,string targetEvent, string eventTrigger, string eventParameter);

        [OperationContract]
        string GetVersion();
    }

    public interface IRemoteServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RemoteEvent(string eventName);

        [OperationContract]
        void Pong(uint tick);
    }
}
