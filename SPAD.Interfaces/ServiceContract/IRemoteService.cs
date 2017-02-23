using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.ServiceContract
{
    public static class RemoteServiceContract
    {
        public static readonly Uri ServiceUrl  = new Uri("net.pipe://localhost/SPAD.neXt");
        public const string ServiceEndpoint = "RemoteService";
        public const string ServiceNamespace = "http://www.spadnext.com/Service/RemoteService";
        public static readonly Version RemoteApiVersion = new Version(1, 1, 0, 1);
    }

    public sealed class RemoteServiceResponse
    {
        public bool HasError { get; set; } = false;
        public string Error { get; set; }
        public double Value { get; set; }
    }

    [DataContract]
    public enum RemoteServiceResult
    {
        [EnumMember]
        CONNECTION_OK = 0,
        [EnumMember]
        CONNECTION_OUTDATED = 1,
        [EnumMember]
        CONNECTION_DENIED = 2,
        [EnumMember]
        CONNECTION_BUSY = 3
    }

    [ServiceContract( Namespace = RemoteServiceContract.ServiceNamespace, CallbackContract = typeof(IRemoteServiceCallback))]
    public interface IRemoteServiceBase
    {
        [OperationContract]
        RemoteServiceResult Initialize(string clientName, Version remoteApiVersion);

        [OperationContract]
        string GetVersion();

        [OperationContract(IsOneWay = true)]
        void Ping(ulong tick);
    }

    [ServiceContract( Namespace = RemoteServiceContract.ServiceNamespace, CallbackContract = typeof(IRemoteServiceCallback))]
    [ServiceKnownType(typeof(RemoteServiceResponse))]
    public interface IRemoteService : IRemoteServiceBase
    {
        [OperationContract]
        RemoteServiceResponse GetValue(string variableName);

        [OperationContract]
        RemoteServiceResponse SetValue(string variableName, double newValue);

        [OperationContract]
        RemoteServiceResponse EmulateEvent(string targetDevice,string targetEvent, string eventTrigger, string eventParameter);

        [OperationContract]
        RemoteServiceResponse Monitor(string variableName);

    }

    public interface IRemoteServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RemoteEvent(string eventName);

        [OperationContract(IsOneWay = true)]
        void Pong(ulong tick);
    }
}
